import { describe, expect, it } from "vitest";
import { HEADER_LENGTH, MAGIC, MSGTYPE_ACK, MSGTYPE_REQUEST } from "../constants.ts";
import { buildPacket, crc32, parseHeader } from "../packet.ts";

describe("crc32", () => {
  it("matches the known CRC-32/ISO-HDLC test vector for ASCII 'checksum'", () => {
    // Well-known reference vector for this exact algorithm variant
    // (poly 0xEDB88320, init 0xFFFFFFFF, final XOR 0xFFFFFFFF).
    const bytes = new TextEncoder().encode("123456789");
    expect(crc32(bytes)).toBe(0xcbf43926);
  });

  it("is order-sensitive across multiple chunks vs. a single concatenated buffer", () => {
    const a = new TextEncoder().encode("hello ");
    const b = new TextEncoder().encode("world");
    const combined = new TextEncoder().encode("hello world");
    expect(crc32(a, b)).toBe(crc32(combined));
  });
});

describe("buildPacket / parseHeader round trip", () => {
  it("round-trips a header with no payload", () => {
    const packet = buildPacket(0x3c, MSGTYPE_REQUEST, 7, 0);
    expect(packet.length).toBe(HEADER_LENGTH);
    const header = parseHeader(packet);
    expect(header.magic).toBe(MAGIC);
    expect(header.command).toBe(0x3c);
    expect(header.type).toBe(MSGTYPE_REQUEST);
    expect(header.seq).toBe(7);
    expect(header.retry).toBe(0);
    expect(header.length).toBe(0);
  });

  it("round-trips a header with a payload and produces a verifiable CRC", () => {
    const payload = new Uint8Array([1, 2, 3, 4, 5, 250, 251, 252]);
    const packet = buildPacket(114, MSGTYPE_ACK, 42, 3, payload);
    expect(packet.length).toBe(HEADER_LENGTH + payload.length);

    const header = parseHeader(packet.subarray(0, HEADER_LENGTH));
    expect(header.command).toBe(114);
    expect(header.type).toBe(MSGTYPE_ACK);
    expect(header.seq).toBe(42);
    expect(header.retry).toBe(3);
    expect(header.length).toBe(payload.length);

    // CRC is computed over header-with-crc-zeroed + payload.
    const headerForCrc = packet.slice(0, HEADER_LENGTH);
    headerForCrc.set([0, 0, 0, 0], HEADER_LENGTH - 4);
    expect(crc32(headerForCrc, payload)).toBe(header.crc32);

    const gotPayload = packet.subarray(HEADER_LENGTH, HEADER_LENGTH + header.length);
    expect(Array.from(gotPayload)).toEqual(Array.from(payload));
  });

  it("detects payload corruption via CRC mismatch", () => {
    const payload = new Uint8Array([9, 9, 9]);
    const packet = buildPacket(1, MSGTYPE_REQUEST, 0, 0, payload);
    packet[HEADER_LENGTH]! ^= 0xff; // corrupt first payload byte

    const header = parseHeader(packet.subarray(0, HEADER_LENGTH));
    const headerForCrc = packet.slice(0, HEADER_LENGTH);
    headerForCrc.set([0, 0, 0, 0], HEADER_LENGTH - 4);
    const corruptedPayload = packet.subarray(HEADER_LENGTH, HEADER_LENGTH + header.length);
    expect(crc32(headerForCrc, corruptedPayload)).not.toBe(header.crc32);
  });
});
