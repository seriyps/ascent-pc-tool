import { parseDeviceInfo, type DeviceInfo } from "./deviceInfo.ts";
import {
  ACK_RETRY_INTERVAL_MS,
  ACK_TOTAL_TIMEOUT_MS,
  CMD_FIND_DEVICE,
  HEADER_LENGTH,
  MAGIC,
  MSGTYPE_REQUEST,
} from "./constants.ts";
import { buildPacket, parseHeader } from "./packet.ts";
import { TransportTimeoutError, type SerialTransport } from "./transport.ts";

export interface AckResult {
  matched: boolean;
  cmd: number;
  seq: number;
  retryCount: number;
  payload: Uint8Array;
}

export interface SendWithAckOptions {
  expectedAckCmd?: number;
  totalTimeoutMs?: number;
  retryIntervalMs?: number;
  // Reports cumulative bytes handed to the transport for the current
  // (re)send attempt — see transport.ts's write() docs.
  onWriteProgress?: (bytesWritten: number) => void;
}

// AscentClient equivalent. Async/Promise-based throughout, a natural fit for
// the Streams-based transport (unlike the .NET port's deliberately
// synchronous design).
export class AscentClient {
  // Starts at 0 to match the vendor's observed first-request seq (see
  // src/CaddxTool.Protocol/AscentClient.cs's own comment on this).
  private seq = 0;
  private readonly transport: SerialTransport;

  constructor(transport: SerialTransport) {
    this.transport = transport;
  }

  // Fire-and-forget send, no ack expected — used for the reboot command.
  async sendNoAck(cmd: number, payload?: Uint8Array): Promise<void> {
    const seq = this.seq++;
    const packet = buildPacket(cmd, MSGTYPE_REQUEST, seq, 0, payload);
    await this.transport.write(packet);
  }

  async getDeviceInfo(): Promise<DeviceInfo> {
    const ack = await this.sendWithAck(CMD_FIND_DEVICE);
    if (!ack.matched) {
      throw new Error("FIND_DEVICE timed out");
    }
    return parseDeviceInfo(ack.payload);
  }

  // Sends `cmd` and waits for a response whose header `command` matches
  // `expectedAckCmd` (defaults to `cmd`) and whose `seq` matches this
  // request's. Resends the identical packet (same seq, incrementing retry)
  // every `retryIntervalMs` until `totalTimeoutMs` has elapsed overall.
  // Unrelated/stale packets (wrong cmd or seq) are ignored, not accepted.
  async sendWithAck(cmd: number, payload?: Uint8Array, options?: SendWithAckOptions): Promise<AckResult> {
    const expected = options?.expectedAckCmd ?? cmd;
    const totalTimeoutMs = options?.totalTimeoutMs ?? ACK_TOTAL_TIMEOUT_MS;
    const retryIntervalMs = options?.retryIntervalMs ?? ACK_RETRY_INTERVAL_MS;
    const seq = this.seq++;
    let retry = 0;

    let packet = buildPacket(cmd, MSGTYPE_REQUEST, seq, retry, payload);
    await this.transport.write(packet, options?.onWriteProgress);

    const deadline = Date.now() + totalTimeoutMs;
    while (true) {
      const remaining = deadline - Date.now();
      if (remaining <= 0) {
        return { matched: false, cmd: expected, seq, retryCount: retry, payload: new Uint8Array(0) };
      }

      const attemptTimeout = Math.min(retryIntervalMs, remaining);
      let header;
      try {
        header = parseHeader(await this.transport.readExact(HEADER_LENGTH, attemptTimeout));
      } catch (err) {
        if (err instanceof TransportTimeoutError) {
          retry++;
          packet = buildPacket(cmd, MSGTYPE_REQUEST, seq, retry, payload);
          await this.transport.write(packet, options?.onWriteProgress);
          continue;
        }
        throw err;
      }

      if (header.magic !== MAGIC) {
        throw new Error(`bad magic in response: 0x${header.magic.toString(16)}`);
      }
      const responsePayload = header.length > 0 ? await this.transport.readExact(header.length, remaining) : new Uint8Array(0);

      if (header.command !== expected || header.seq !== seq) {
        // Unrelated/stale packet — keep waiting for the one we asked for.
        continue;
      }

      return { matched: true, cmd: header.command, seq: header.seq, retryCount: retry, payload: responsePayload };
    }
  }
}
