import { describe, expect, it } from "vitest";
import { writeAsciiField } from "../ascii.ts";
import { AscentClient } from "../client.ts";
import { CMD_FIND_DEVICE } from "../constants.ts";
import { buildAckFrame, FakeTransport, type SentPacket } from "../testing/fakeTransport.ts";

function buildDeviceInfoPayload(deviceName: string, firmwareInfo: string): Uint8Array {
  const buf = new Uint8Array(300);
  const dv = new DataView(buf.buffer);
  dv.setInt32(0, 1_048_576, true); // receiveMaxSize
  writeAsciiField(buf, 4, 32, "sdk-1.0");
  writeAsciiField(buf, 36, 64, deviceName);
  dv.setInt32(100, 42, true); // cpuTemp
  writeAsciiField(buf, 104, 64, firmwareInfo);
  writeAsciiField(buf, 168, 32, "SN0001");
  writeAsciiField(buf, 200, 32, "HW1.0");
  dv.setInt32(232, 0, true); // status
  writeAsciiField(buf, 236, 64, "");
  return buf;
}

describe("AscentClient.getDeviceInfo", () => {
  it("parses a matching FIND_DEVICE ack", async () => {
    const transport = new FakeTransport((sent: SentPacket) => {
      if (sent.header.command !== CMD_FIND_DEVICE) return undefined;
      return buildAckFrame(CMD_FIND_DEVICE, sent.header.seq, buildDeviceInfoPayload("Ascent_lite_plus", "Ascent_H_Sky_18_21_10"));
    });
    const client = new AscentClient(transport);
    const info = await client.getDeviceInfo();
    expect(info.deviceName).toBe("Ascent_lite_plus");
    expect(info.firmwareInfo).toBe("Ascent_H_Sky_18_21_10");
    expect(info.receiveMaxSize).toBe(1_048_576);
  });
});

describe("AscentClient.sendWithAck", () => {
  it("starts seq at 0 and increments per request", async () => {
    const transport = new FakeTransport((sent) => buildAckFrame(sent.header.command, sent.header.seq));
    const client = new AscentClient(transport);
    await client.sendWithAck(1);
    await client.sendWithAck(2);
    expect(transport.sent.map((s) => s.header.seq)).toEqual([0, 1]);
  });

  it("resends on timeout and eventually succeeds (retry-then-succeed)", async () => {
    let attempt = 0;
    const transport = new FakeTransport((sent) => {
      attempt++;
      if (attempt < 3) return undefined; // drop the first two attempts
      return buildAckFrame(sent.header.command, sent.header.seq);
    });
    const client = new AscentClient(transport);
    const ack = await client.sendWithAck(99, undefined, { retryIntervalMs: 20, totalTimeoutMs: 500 });
    expect(ack.matched).toBe(true);
    expect(ack.retryCount).toBe(2);
    expect(transport.sent.length).toBe(3);
    // All three attempts share the same seq, with incrementing retry field.
    expect(transport.sent.map((s) => s.header.seq)).toEqual([0, 0, 0]);
    expect(transport.sent.map((s) => s.header.retry)).toEqual([0, 1, 2]);
  });

  it("gives up after the total timeout and reports not matched", async () => {
    const transport = new FakeTransport(() => undefined); // never respond
    const client = new AscentClient(transport);
    const ack = await client.sendWithAck(5, undefined, { retryIntervalMs: 15, totalTimeoutMs: 50 });
    expect(ack.matched).toBe(false);
    expect(transport.sent.length).toBeGreaterThan(1);
  });

  it("ignores an unrelated cmd/seq arriving mid-wait instead of accepting it", async () => {
    const transport = new FakeTransport((sent) => [
      // A stale/unrelated ack (wrong cmd) followed by the real one.
      buildAckFrame(sent.header.command + 1, sent.header.seq),
      buildAckFrame(sent.header.command, sent.header.seq),
    ]);
    const client = new AscentClient(transport);
    const ack = await client.sendWithAck(7);
    expect(ack.matched).toBe(true);
    expect(ack.cmd).toBe(7);
  });
});
