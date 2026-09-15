// Wire-protocol constants for the Ascent device protocol. Values confirmed
// against the .NET native port (src/CaddxTool.Protocol/ArConstantsV2.cs) and
// the official app's own wire traffic (see the project's decompiled-source
// notes) — these are not stylistic choices, they must match exactly for a
// device to accept the frame.

export const MAGIC = 1095914575;
export const VERSION = 3292;
export const MSGTYPE_REQUEST = 1;
export const MSGTYPE_ACK = 2;
export const UNUSED_FIELD = 43981;
export const FMT_BINARY = 2;
export const HEADER_LENGTH = 36;

export const CMD_FIND_DEVICE = 0x3c; // 60
export const CMD_REBOOT = 3;
export const CMD_REMOTE_UPGRADE = 114;
export const CMD_SENDFILE_START = 115;
export const CMD_SENDFILE_DATA = 116;
export const CMD_SENDFILE_END = 117;
export const CMD_UPGRADE_STATUS = 118;

// Ack retry/timeout. The .NET port widened its total timeout to 300s to
// survive tty0tty's baud-rate pacing during vendor-app wire-capture testing
// — not relevant to a browser talking to real USB CDC-ACM hardware, so these
// stay close to the vendor's own (unused-in-retail) ArTransportV2 defaults.
// Easy to widen later if live testing shows it's needed.
export const ACK_RETRY_INTERVAL_MS = 2000;
export const ACK_TOTAL_TIMEOUT_MS = 20000;

// Reconnect-after-reboot timing. Unlike the .NET port (which re-scans sysfs
// by VID because a closed SerialPort is gone for good), Web Serial's granted
// SerialPort object persists across a USB replug and fires connect/disconnect
// events on it — see transport.ts. These constants bound how long to wait
// for that connect event / the open() retry loop after it fires.
export const RECONNECT_TIMEOUT_MS = 60000;
export const RECONNECT_POLL_INTERVAL_MS = 1000;
export const REBOOT_WAIT_DEFAULT_MS = 10000;

// Ported verbatim from ArConstantsV2.RebootWaitByDeviceName (itself ported
// from the decompiled GD.CreateDefaultReOpenDelayByDevName_Asce).
export const REBOOT_WAIT_BY_DEVICE_NAME: Readonly<Record<string, number>> = {
  ascent_vrx: 9500,
  ascent_vrx_pro: 12000,
  cx485_pro: 12000,
  ascent_vrx_max: 12000,
  ascent_vrx_max_hf: 12000,
  ascent_vrx_max_wf: 12000,
  ascent_vrx_cine: 12000,
  ascent_gt_pro: 9500,
  ascent_gt_pro_z40: 9500,
  ascent_gt_pro_z8: 9500,
  ascent_gt_pro_hub: 9500,
  ascent_gt: 9500,
  ascent_gt_27: 9500,
  ascent_gt_night: 9500,
  ascent_gt_ultra: 9500,
  ascent_gt_max: 11500,
  ascent_lite: 8000,
  ascent_lite_plus: 8000,
  yohd_micro: 8000,
  ascent_rc: 8000,
  caddx_gm3: 6000,
  caddx_gm1: 6000,
};

export function rebootWaitMs(deviceName: string): number {
  const key = deviceName.toLowerCase();
  return REBOOT_WAIT_BY_DEVICE_NAME[key] ?? REBOOT_WAIT_DEFAULT_MS;
}

// Default SENDFILE_DATA chunk size, used only as a fallback if a device ever
// reports ReceiveMaxSize <= 0. Real chunk size always comes from the
// connected device's own FIND_DEVICE response.
export const DEFAULT_CHUNK_SIZE = 1_048_576;

// Ported from AscentVids.KnownVids — a hint for UI display only, not a
// source of truth: 0x1d76 is genuinely shared by two different retail
// products (Ascent GT Pro and Ascent Lite+), distinguishable only via the
// live FIND_DEVICE response.
export const KNOWN_VIDS: Readonly<Record<number, string>> = {
  0x1a86: "GM_V2",
  0x1d6b: "ASCENT_L_GND401",
  0x1d6c: "ASCENT_L_SKY402",
  0x1d6d: "ASCENT_H_SKY482",
  0x1d6e: "ASCENT_L_GND",
  0x1d6f: "ASCENT_GND_EX1",
  0x1d70: "ASCENT_SKY_EX1",
  0x1d71: "ASCENT_GND_EX2",
  0x1d72: "ASCENT_SKY_EX2",
  0x1d73: "ASCENT_GND_EX3",
  0x1d74: "ASCENT_SKY_EX3",
  0x1d75: "Ascent_VRX",
  0x1d76: "Ascent_GT_Pro / Lite+",
  0x1d77: "Ascent_VRX_Pro",
  0x1d78: "Ascent_VRX_Max",
  0x1d79: "ASCENT_VRX_Cine",
  0x1d7a: "ASCENT_VRX_EX",
  0x1d7b: "ASCENT_VRX_EX2",
  0x1d80: "ASCENT_JoyStick",
  0x1d81: "OPV2_VTX",
  0x1d82: "OPV2_Z40VTX",
  0x1d83: "OPV2_Z8VTX",
  0x1d84: "OPV2_VRX",
};
