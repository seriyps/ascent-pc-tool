import { readAsciiField } from "./ascii.ts";

// FIND_DEVICE ack payload — 300 bytes. Field layout ported from
// src/CaddxTool.Protocol/ResDeviceInfoV2.cs, verified live against real
// hardware in that project.
export interface DeviceInfo {
  receiveMaxSize: number;
  sdkVersion: string;
  deviceName: string;
  cpuTemp: number;
  firmwareInfo: string;
  serialNumber: string;
  hardwareVersion: string;
  status: number;
  detail: string;
}

export const DEVICE_INFO_PAYLOAD_LENGTH = 300;

export function parseDeviceInfo(payload: Uint8Array): DeviceInfo {
  if (payload.length < DEVICE_INFO_PAYLOAD_LENGTH) {
    throw new Error(`FIND_DEVICE payload too short: ${payload.length} bytes`);
  }
  const dv = new DataView(payload.buffer, payload.byteOffset, payload.byteLength);
  let o = 0;
  const receiveMaxSize = dv.getInt32(o, true); o += 4;
  const sdkVersion = readAsciiField(payload, o, 32); o += 32;
  const deviceName = readAsciiField(payload, o, 64); o += 64;
  const cpuTemp = dv.getInt32(o, true); o += 4;
  const firmwareInfo = readAsciiField(payload, o, 64); o += 64;
  const serialNumber = readAsciiField(payload, o, 32); o += 32;
  const hardwareVersion = readAsciiField(payload, o, 32); o += 32;
  const status = dv.getInt32(o, true); o += 4;
  const detail = readAsciiField(payload, o, 64);
  return {
    receiveMaxSize,
    sdkVersion,
    deviceName,
    cpuTemp,
    firmwareInfo,
    serialNumber,
    hardwareVersion,
    status,
    detail,
  };
}
