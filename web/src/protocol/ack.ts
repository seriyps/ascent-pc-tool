import { readAsciiField } from "./ascii.ts";

// Generic small ack payload (status + detail string), used by several
// file-transfer commands. Ported from ResAckInfoV2.cs. 68 bytes.
export interface AckInfo {
  status: number;
  detail: string;
}

export function parseAckInfo(payload: Uint8Array): AckInfo {
  const dv = new DataView(payload.buffer, payload.byteOffset, payload.byteLength);
  const status = dv.getInt32(0, true);
  const detail = readAsciiField(payload, 4, 64);
  return { status, detail };
}

// SENDFILE_END ack payload. Ported from ResFileDataInfoV2.cs. 80 bytes.
export interface FileDataInfo {
  length: number;
  cursize: number;
  totalsize: number;
  status: number;
  detail: string;
}

export function parseFileDataInfo(payload: Uint8Array): FileDataInfo {
  const dv = new DataView(payload.buffer, payload.byteOffset, payload.byteLength);
  const length = dv.getInt32(0, true);
  const cursize = dv.getInt32(4, true);
  const totalsize = dv.getInt32(8, true);
  const status = dv.getInt32(12, true);
  const detail = readAsciiField(payload, 16, 64);
  return { length, cursize, totalsize, status, detail };
}

// UPGRADE_STATUS poll ack payload. Ported from ResUpgradeStatusV2.cs. 72 bytes.
export interface UpgradeStatusInfo {
  percent: number;
  status: number;
  detail: string;
}

export function parseUpgradeStatusInfo(payload: Uint8Array): UpgradeStatusInfo {
  const dv = new DataView(payload.buffer, payload.byteOffset, payload.byteLength);
  const percent = dv.getInt32(0, true);
  const status = dv.getInt32(4, true);
  const detail = readAsciiField(payload, 8, 64);
  return { percent, status, detail };
}
