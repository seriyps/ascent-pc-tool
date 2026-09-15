import { describe, expect, it } from "vitest";
import { writeAsciiField } from "../ascii.ts";
import {
  CMD_FIND_DEVICE,
  CMD_REBOOT,
  CMD_REMOTE_UPGRADE,
  CMD_SENDFILE_DATA,
  CMD_SENDFILE_END,
  CMD_SENDFILE_START,
  CMD_UPGRADE_STATUS,
} from "../constants.ts";
import { buildAckFrame, FakeTransport, type SentPacket } from "../testing/fakeTransport.ts";
import { runFirmwareUpgrade, type FileLike, type FlowProgress } from "../upgradeFlow.ts";
import { UpgradeStatusErrorCode } from "../upgradeStatus.ts";

// Replays the command shape recorded in a real captured session (see
// /tmp/gui-test-capture.log from an earlier manual test): the .NET
// Avalonia GUI driving a full firmware upgrade against
// CaddxTool.FakeDevice's "lite_plus" profile. That capture confirmed
// (structural facts this test asserts, not exact byte-for-byte replay —
// the real capture's firmware file size differed from this test's
// synthetic one, so exact chunk counts aren't meant to match):
//   - the flow's own internal FIND_DEVICE call starts from seq=0
//   - REBOOT (no ack) at seq=1 on that same client
//   - after reconnect, a BRAND NEW client (seq resets to 0 again) for
//     REMOTE_UPGRADE/SENDFILE_START/SENDFILE_DATA×N/SENDFILE_END/UPGRADE_STATUS×N
//   - UPGRADE_STATUS payload status field is the percent-so-far as an int32
//     (FakeDevice ramps it 40→80→100 in the real trace)
//   - after the second reconnect, one more silent FIND_DEVICE (seq=0 again)
// This test doesn't touch a real fake-device process or browser — it's a
// pure protocol-level replay verifying the TS orchestrator produces the
// same wire-level structure a known-good captured run did.
function buildDeviceInfoPayload(): Uint8Array {
  const buf = new Uint8Array(300);
  const dv = new DataView(buf.buffer);
  dv.setInt32(0, 1_048_576, true); // receiveMaxSize — matches the capture's 1MB chunking
  writeAsciiField(buf, 4, 32, "");
  writeAsciiField(buf, 36, 64, "Ascent_lite_plus");
  dv.setInt32(100, 42, true);
  writeAsciiField(buf, 104, 64, "Ascent_H_Sky_18_21_10");
  writeAsciiField(buf, 168, 32, "FAKE0000001");
  writeAsciiField(buf, 200, 32, "FPV-Ascent-Sky-472-V1.3-1.1");
  dv.setInt32(232, 0, true);
  writeAsciiField(buf, 236, 64, "");
  return buf;
}

// Mirrors CaddxTool.FakeDevice's Program.cs switch-case behavior, including
// its persistent upgradeStatusPercent counter across reconnects (real
// FakeDevice keeps this as long as the process lives, regardless of how
// many times a client reconnects the underlying transport).
function makeFakeDeviceState() {
  let upgradeStatusPercent = 0;
  return {
    respond(sent: SentPacket): Uint8Array | undefined {
      switch (sent.header.command) {
        case CMD_REBOOT:
          upgradeStatusPercent = 0;
          return undefined; // no ack, matches SendNoAck
        case CMD_FIND_DEVICE:
          return buildAckFrame(CMD_FIND_DEVICE, sent.header.seq, buildDeviceInfoPayload());
        case CMD_REMOTE_UPGRADE:
        case CMD_SENDFILE_START:
          return buildAckFrame(sent.header.command, sent.header.seq, new Uint8Array(0));
        case CMD_SENDFILE_DATA:
        case CMD_SENDFILE_END: {
          const info = new Uint8Array(80);
          const dv = new DataView(info.buffer);
          dv.setInt32(0, sent.payload.length, true);
          dv.setInt32(4, sent.payload.length, true);
          dv.setInt32(8, sent.payload.length, true);
          dv.setInt32(12, 0, true);
          writeAsciiField(info, 16, 64, "OK");
          return buildAckFrame(sent.header.command, sent.header.seq, info);
        }
        case CMD_UPGRADE_STATUS: {
          upgradeStatusPercent = Math.min(100, upgradeStatusPercent + 40);
          const info = new Uint8Array(72);
          const dv = new DataView(info.buffer);
          dv.setInt32(0, upgradeStatusPercent, true);
          dv.setInt32(4, 0, true);
          return buildAckFrame(CMD_UPGRADE_STATUS, sent.header.seq, info);
        }
        default:
          return undefined;
      }
    },
  };
}

function makeTestFile(sizeBytes: number): FileLike {
  const bytes = new Uint8Array(sizeBytes);
  for (let i = 0; i < bytes.length; i++) bytes[i] = i % 251;
  return {
    name: "Ascent_H_Sky_18_21_10.img",
    arrayBuffer: async () => bytes.buffer,
  };
}

describe("runFirmwareUpgrade", () => {
  it("replays the captured command sequence and succeeds, matching the known-good .NET-vs-FakeDevice trace", async () => {
    const device = makeFakeDeviceState();
    const allSent: SentPacket[] = [];
    const createTransport = () =>
      new FakeTransport((sent) => {
        allSent.push(sent);
        return device.respond(sent);
      });

    // 2.5 chunks worth, so the trace includes a full chunk + a partial tail
    // chunk, same shape as the real capture's 16 full + 1 partial chunk.
    const file = makeTestFile(2.5 * 1_048_576);
    const progressEvents: FlowProgress[] = [];

    const result = await runFirmwareUpgrade(
      createTransport,
      "ascent_lite_plus",
      file,
      (p) => progressEvents.push(p),
      undefined,
      async () => {}, // no real waiting — this replay doesn't test timing
    );

    expect(result.success).toBe(true);
    expect(result.message).toBe("Upgrade complete");

    // Command sequence: FindDevice, Reboot(noAck), [reconnect],
    // RemoteUpgrade, SendFileStart, SendFileData x3, SendFileEnd,
    // UpgradeStatus x3 (40/80/100), [reconnect], FindDevice.
    expect(allSent.map((s) => s.header.command)).toEqual([
      CMD_FIND_DEVICE,
      CMD_REBOOT,
      CMD_REMOTE_UPGRADE,
      CMD_SENDFILE_START,
      CMD_SENDFILE_DATA,
      CMD_SENDFILE_DATA,
      CMD_SENDFILE_DATA,
      CMD_SENDFILE_END,
      CMD_UPGRADE_STATUS,
      CMD_UPGRADE_STATUS,
      CMD_UPGRADE_STATUS,
      CMD_FIND_DEVICE,
    ]);

    // Seq resets to 0 after each reconnect — a fresh AscentClient per
    // reconnect, matching FirmwareUpgradeFlow.ReconnectAsync's behavior.
    const findDeviceSeqs = allSent.filter((s) => s.header.command === CMD_FIND_DEVICE).map((s) => s.header.seq);
    expect(findDeviceSeqs).toEqual([0, 0]);
    const remoteUpgradeSeq = allSent.find((s) => s.header.command === CMD_REMOTE_UPGRADE)!.header.seq;
    expect(remoteUpgradeSeq).toBe(0);

    // Chunk sizes: two full 1MB chunks, one 0.5MB tail chunk.
    const chunkLens = allSent.filter((s) => s.header.command === CMD_SENDFILE_DATA).map((s) => s.payload.length);
    expect(chunkLens).toEqual([1_048_576, 1_048_576, 0.5 * 1_048_576]);

    // Progress reaches 1.0 (Done) as the final event.
    expect(progressEvents.at(-1)).toMatchObject({ stepName: "Done", percent: 1 });
  });

  it("surfaces the exact ported UPGRATE_ERR_BOARD_TYPE message and aborts when the device rejects the image", async () => {
    const createTransport = () =>
      new FakeTransport((sent) => {
        switch (sent.header.command) {
          case CMD_FIND_DEVICE:
            return buildAckFrame(CMD_FIND_DEVICE, sent.header.seq, buildDeviceInfoPayload());
          case CMD_REBOOT:
            return undefined;
          case CMD_REMOTE_UPGRADE:
          case CMD_SENDFILE_START: {
            const empty = new Uint8Array(0);
            return buildAckFrame(sent.header.command, sent.header.seq, empty);
          }
          case CMD_SENDFILE_DATA:
          case CMD_SENDFILE_END: {
            const info = new Uint8Array(80);
            const dv = new DataView(info.buffer);
            dv.setInt32(0, sent.payload.length, true);
            dv.setInt32(4, sent.payload.length, true);
            dv.setInt32(8, sent.payload.length, true);
            dv.setInt32(12, 0, true);
            writeAsciiField(info, 16, 64, "OK");
            return buildAckFrame(sent.header.command, sent.header.seq, info);
          }
          case CMD_UPGRADE_STATUS: {
            const info = new Uint8Array(72);
            new DataView(info.buffer).setInt32(4, UpgradeStatusErrorCode.UPGRATE_ERR_BOARD_TYPE, true);
            return buildAckFrame(CMD_UPGRADE_STATUS, sent.header.seq, info);
          }
          default:
            return undefined;
        }
      });

    const file = makeTestFile(1024);
    const result = await runFirmwareUpgrade(createTransport, "ascent_lite_plus", file, undefined, undefined, async () => {});

    expect(result.success).toBe(false);
    expect(result.message).toBe("Upgrade failed, the device model does not match the upgraded firmware");
  });
});
