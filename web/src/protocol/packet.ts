import {
  FMT_BINARY,
  HEADER_LENGTH,
  MAGIC,
  UNUSED_FIELD,
  VERSION,
} from "./constants.ts";

export interface PacketHeader {
  magic: number;
  version: number;
  type: number;
  msgid: number;
  unused: number;
  command: number;
  format: number;
  userid: number;
  length: number;
  seq: number;
  retry: number;
  crc32: number;
}

let crcTable: Uint32Array | undefined;

function getCrcTable(): Uint32Array {
  if (crcTable) return crcTable;
  const table = new Uint32Array(256);
  const poly = 0xedb88320;
  for (let i = 0; i < 256; i++) {
    let c = i;
    for (let k = 0; k < 8; k++) {
      c = c & 1 ? (c >>> 1) ^ poly : c >>> 1;
    }
    table[i] = c >>> 0;
  }
  crcTable = table;
  return table;
}

// Standard IEEE/zlib CRC-32: init 0xFFFFFFFF, final XOR 0xFFFFFFFF.
export function crc32(...chunks: Uint8Array[]): number {
  const table = getCrcTable();
  let crc = 0xffffffff;
  for (const bytes of chunks) {
    for (let i = 0; i < bytes.length; i++) {
      crc = table[(crc ^ bytes[i]!) & 0xff]! ^ (crc >>> 8);
    }
  }
  return (crc ^ 0xffffffff) >>> 0;
}

export function buildPacket(
  cmd: number,
  type: number,
  seq: number,
  retry: number,
  payload?: Uint8Array,
): Uint8Array {
  const payloadLen = payload?.length ?? 0;
  const header = new Uint8Array(HEADER_LENGTH);
  writeHeader(header, {
    magic: MAGIC,
    version: VERSION,
    type,
    msgid: 0,
    unused: UNUSED_FIELD,
    command: cmd,
    format: FMT_BINARY,
    userid: 0,
    length: payloadLen,
    seq,
    retry,
    crc32: 0,
  });

  const crc = crc32(header, payload ?? new Uint8Array(0));
  new DataView(header.buffer).setUint32(HEADER_LENGTH - 4, crc, true);

  const packet = new Uint8Array(HEADER_LENGTH + payloadLen);
  packet.set(header, 0);
  if (payload) packet.set(payload, HEADER_LENGTH);
  return packet;
}

function writeHeader(buf: Uint8Array, h: PacketHeader): void {
  const dv = new DataView(buf.buffer, buf.byteOffset, buf.byteLength);
  let o = 0;
  dv.setUint32(o, h.magic, true); o += 4;
  dv.setUint16(o, h.version, true); o += 2;
  dv.setUint16(o, h.type, true); o += 2;
  dv.setUint16(o, h.msgid, true); o += 2;
  dv.setUint16(o, h.unused, true); o += 2;
  dv.setUint32(o, h.command, true); o += 4;
  dv.setUint16(o, h.format, true); o += 2;
  dv.setUint16(o, h.userid, true); o += 2;
  dv.setUint32(o, h.length, true); o += 4;
  dv.setUint32(o, h.seq, true); o += 4;
  dv.setUint32(o, h.retry, true); o += 4;
  dv.setUint32(o, h.crc32, true); o += 4;
  if (o !== HEADER_LENGTH) {
    throw new Error(`header packing produced ${o} bytes, expected ${HEADER_LENGTH}`);
  }
}

export function parseHeader(bytes: Uint8Array): PacketHeader {
  if (bytes.length < HEADER_LENGTH) {
    throw new Error(`header too short: ${bytes.length} bytes`);
  }
  const dv = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);
  let o = 0;
  const magic = dv.getUint32(o, true); o += 4;
  const version = dv.getUint16(o, true); o += 2;
  const type = dv.getUint16(o, true); o += 2;
  const msgid = dv.getUint16(o, true); o += 2;
  const unused = dv.getUint16(o, true); o += 2;
  const command = dv.getUint32(o, true); o += 4;
  const format = dv.getUint16(o, true); o += 2;
  const userid = dv.getUint16(o, true); o += 2;
  const length = dv.getUint32(o, true); o += 4;
  const seq = dv.getUint32(o, true); o += 4;
  const retry = dv.getUint32(o, true); o += 4;
  const crc32Field = dv.getUint32(o, true); o += 4;
  return { magic, version, type, msgid, unused, command, format, userid, length, seq, retry, crc32: crc32Field };
}
