import { md5 } from "js-md5";
import { parseFileDataInfo, parseUpgradeStatusInfo } from "./ack.ts";
import { AscentClient } from "./client.ts";
import {
  CMD_REBOOT,
  CMD_REMOTE_UPGRADE,
  CMD_SENDFILE_DATA,
  CMD_SENDFILE_END,
  CMD_SENDFILE_START,
  CMD_UPGRADE_STATUS,
  DEFAULT_CHUNK_SIZE,
  RECONNECT_POLL_INTERVAL_MS,
  RECONNECT_TIMEOUT_MS,
  rebootWaitMs,
} from "./constants.ts";
import { buildFileInfo } from "./fileTransfer.ts";
import type { SerialTransport } from "./transport.ts";
import { judgeUpgradeStatus } from "./upgradeStatus.ts";

export interface FlowProgress {
  stepName: string;
  percent: number; // 0..1
  message: string;
}

export interface FlowResult {
  success: boolean;
  message: string;
}

export type ProgressCallback = (progress: FlowProgress) => void;

// Only what upgradeFlow actually needs from a browser File — lets tests pass
// a plain object instead of a real File/Blob.
export interface FileLike {
  name: string;
  arrayBuffer(): Promise<ArrayBuffer>;
}

export type DelayFn = (ms: number, signal?: AbortSignal) => Promise<void>;

class AbortedError extends Error {
  constructor() {
    super("Cancelled");
    this.name = "AbortedError";
  }
}

// Mechanical port of FirmwareUpgradeFlow.RunAsync (src/CaddxTool.Protocol) —
// see AGENTS.md's "Safety principle": keep the step order/commands/timeouts
// close to the vendor's own state machine rather than redesigning it, even
// though this code is freshly written, not transliterated. Verified against
// a real captured .NET-client-vs-CaddxTool.FakeDevice wire trace (see
// upgradeFlow.test.ts) — same command sequence, same per-reconnect seq
// reset (each reconnect makes a brand new AscentClient, matching
// FirmwareUpgradeFlow.ReconnectAsync's `new AscentClient(_transport, Now)`).
//
// Takes a transport *factory*, not a live transport/SerialPort, so the whole
// flow (including its two reconnects) is testable against an in-memory
// FakeTransport with no browser involved — mirrors the .NET port's own
// injectable `transportFactory` seam on FirmwareUpgradeFlow.
//
// Reconnect strategy deliberately does NOT depend on Web Serial firing
// connect/disconnect events on the SerialPort object itself — that behavior
// isn't confirmed reliable across browser versions. Instead it just calls
// createTransport().open() again after the device's reboot delay, the same
// belt-and-suspenders approach the .NET port uses (there via re-scanning
// sysfs for a matching VID; here there's no need to re-scan since
// createTransport() already knows which port to reopen).
export async function runFirmwareUpgrade(
  createTransport: () => SerialTransport,
  deviceName: string,
  file: FileLike,
  onProgress?: ProgressCallback,
  signal?: AbortSignal,
  delayFn: DelayFn = delay,
): Promise<FlowResult> {
  let transport = createTransport();
  let client: AscentClient;

  try {
    await transport.open();
    client = new AscentClient(transport);

    report(onProgress, "FindDevice", 0.02, "Querying device info");
    const deviceInfo = await client.getDeviceInfo();
    const chunkSize = deviceInfo.receiveMaxSize > 0 ? deviceInfo.receiveMaxSize : DEFAULT_CHUNK_SIZE;

    report(onProgress, "RebootClean", 0.08, "Rebooting into clean mode");
    await client.sendNoAck(CMD_REBOOT, asciiField("clean", 32));
    ({ transport, client } = await reconnect(createTransport, transport, deviceName, signal, delayFn));

    report(onProgress, "RemoteUpgrade", 0.13, "Entering upgrade mode");
    ensureSuccess((await client.sendWithAck(CMD_REMOTE_UPGRADE)).matched, "REMOTE_UPGRADE failed");

    report(onProgress, "SendFileStart", 0.17, "Starting file transfer");
    const fileBytes = new Uint8Array(await file.arrayBuffer());
    const md5Hex = md5(fileBytes);
    const remotePath = "/tmp/pc/" + file.name;
    const fileInfoPayload = buildFileInfo(md5Hex, fileBytes.length, remotePath, file.name);
    ensureSuccess((await client.sendWithAck(CMD_SENDFILE_START, fileInfoPayload)).matched, "SENDFILE_START failed");

    report(onProgress, "SendFileData", 0.17, "Transferring firmware");
    let sent = 0;
    while (sent < fileBytes.length) {
      if (signal?.aborted) throw new AbortedError();
      const len = Math.min(chunkSize, fileBytes.length - sent);
      const chunk = fileBytes.subarray(sent, sent + len);
      const chunkStart = sent;
      const ack = await client.sendWithAck(CMD_SENDFILE_DATA, chunk, {
        onWriteProgress: (written) => {
          const overallSent = chunkStart + written;
          report(onProgress, "SendFileData", 0.17 + 0.43 * (overallSent / fileBytes.length), "Transferring firmware");
        },
      });
      ensureSuccess(ack.matched, "SENDFILE_DATA failed");
      sent += len;
      report(onProgress, "SendFileData", 0.17 + 0.43 * (sent / fileBytes.length), "Transferring firmware");
    }

    report(onProgress, "SendFileEnd", 0.62, "Finishing transfer");
    const endAck = await client.sendWithAck(CMD_SENDFILE_END);
    ensureSuccess(endAck.matched, "SENDFILE_END failed");
    const fileEndInfo = parseFileDataInfo(endAck.payload);
    if (fileEndInfo.status !== 0 || fileEndInfo.detail !== "OK") {
      throw new Error(fileEndInfo.detail || "SENDFILE_END reported failure");
    }

    report(onProgress, "UpgradeStatus", 0.65, "Polling upgrade status");
    await pollUpgradeStatus(client, onProgress, signal, delayFn);

    report(onProgress, "Reboot", 0.98, "Rebooting device");
    ({ transport, client } = await reconnect(createTransport, transport, deviceName, signal, delayFn));

    // Matches the vendor's own post-upgrade behavior: a final silent
    // FIND_DEVICE refresh so the UI can show the new firmware version.
    // Not gating success — failure here is ignored.
    try {
      await client.getDeviceInfo();
    } catch {
      // silent refresh only
    }

    report(onProgress, "Done", 1.0, "Upgrade complete");
    return { success: true, message: "Upgrade complete" };
  } catch (err) {
    if (err instanceof AbortedError) {
      return { success: false, message: "Cancelled" };
    }
    return { success: false, message: err instanceof Error ? err.message : String(err) };
  } finally {
    await transport.close();
  }
}

async function pollUpgradeStatus(
  client: AscentClient,
  onProgress: ProgressCallback | undefined,
  signal: AbortSignal | undefined,
  delayFn: DelayFn,
): Promise<void> {
  for (let i = 0; i < 200; i++) {
    if (signal?.aborted) throw new AbortedError();
    await delayFn(1000, signal);
    const ack = await client.sendWithAck(CMD_UPGRADE_STATUS);
    ensureSuccess(ack.matched, "UPGRADE_STATUS failed");
    const status = parseUpgradeStatusInfo(ack.payload);
    const errorDesc = judgeUpgradeStatus(status);
    if (errorDesc !== null) {
      throw new Error(errorDesc);
    }
    const percent = Math.min(99, Math.max(0, status.percent));
    report(onProgress, "UpgradeStatus", 0.65 + percent * 0.0034, "Flashing firmware");
    if (status.percent > 99) {
      return;
    }
  }
  throw new Error("upgrade status query limit exceeded");
}

async function reconnect(
  createTransport: () => SerialTransport,
  oldTransport: SerialTransport,
  deviceName: string,
  signal: AbortSignal | undefined,
  delayFn: DelayFn,
): Promise<{ transport: SerialTransport; client: AscentClient }> {
  await oldTransport.close();
  await delayFn(rebootWaitMs(deviceName), signal);

  const deadline = Date.now() + RECONNECT_TIMEOUT_MS;
  while (true) {
    if (signal?.aborted) throw new AbortedError();
    if (Date.now() >= deadline) {
      throw new Error(`device did not reappear within ${RECONNECT_TIMEOUT_MS}ms after reboot`);
    }
    try {
      const transport = createTransport();
      await transport.open();
      return { transport, client: new AscentClient(transport) };
    } catch {
      // Port not ready yet (device still rebooting) — keep polling.
      await delayFn(RECONNECT_POLL_INTERVAL_MS, signal);
    }
  }
}

function ensureSuccess(matched: boolean, message: string): void {
  if (!matched) throw new Error(message);
}

function report(onProgress: ProgressCallback | undefined, stepName: string, percent: number, message: string): void {
  onProgress?.({ stepName, percent, message });
}

function asciiField(value: string, length: number): Uint8Array {
  const buf = new Uint8Array(length);
  buf.set(new TextEncoder().encode(value).subarray(0, length));
  return buf;
}

function delay(ms: number, signal?: AbortSignal): Promise<void> {
  return new Promise((resolve, reject) => {
    if (signal?.aborted) {
      reject(new AbortedError());
      return;
    }
    const timer = setTimeout(resolve, ms);
    signal?.addEventListener(
      "abort",
      () => {
        clearTimeout(timer);
        reject(new AbortedError());
      },
      { once: true },
    );
  });
}
