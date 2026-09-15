import type { UpgradeStatusInfo } from "./ack.ts";

// Ported verbatim from UpgradeStatusErrorCode.cs. Negative values are hard
// errors; STAT_VERIFY_IMAGE (0) and above mean the upgrade is still
// progressing.
export const UpgradeStatusErrorCode = {
  UPGRATE_ERR_CHIPID: -12,
  UPGRATE_ERR_FLASH_ERR: -11,
  UPGRATE_ERR_MAX: -10,
  UPGRATE_ERR_IMG_CRC: -9,
  UPGRATE_ERR_IMG_TYPE: -8,
  UPGRATE_ERR_UPGRADE_MODE: -7,
  UPGRATE_ERR_APP_VERSION: -6,
  UPGRATE_ERR_BOARD_TYPE: -5,
  UPGRATE_ERR_BAD_FILE: -4,
  UPGRATE_ERR_NO_FILE: -3,
  UPGRATE_ERR_NO_PC: -2,
  UPGRATE_ERR_NO_SD: -1,
  STAT_VERIFY_IMAGE: 0,
} as const;

// Ported verbatim from FirmwareUpgradeFlow.JudgeUpgradeStatus's switch.
// Returns null when the upgrade is still progressing/succeeded, else the
// user-facing error message.
export function judgeUpgradeStatus(status: UpgradeStatusInfo): string | null {
  switch (status.status) {
    case UpgradeStatusErrorCode.STAT_VERIFY_IMAGE:
      return null;
    case UpgradeStatusErrorCode.UPGRATE_ERR_NO_SD:
      return "Upgrade failed, no SD card detected";
    case UpgradeStatusErrorCode.UPGRATE_ERR_NO_PC:
      return "Upgrade failed, no computer connected";
    case UpgradeStatusErrorCode.UPGRATE_ERR_NO_FILE:
      return "Upgrade failed, no upgrade file found";
    case UpgradeStatusErrorCode.UPGRATE_ERR_BAD_FILE:
      return "Upgrade failed, upgrade file is corrupted";
    case UpgradeStatusErrorCode.UPGRATE_ERR_BOARD_TYPE:
      return "Upgrade failed, the device model does not match the upgraded firmware";
    case UpgradeStatusErrorCode.UPGRATE_ERR_APP_VERSION:
      return "Upgrade failed, The firmware version number is too low";
    case UpgradeStatusErrorCode.UPGRATE_ERR_UPGRADE_MODE:
      return "Upgrade failed, invalid upgrade mode";
    case UpgradeStatusErrorCode.UPGRATE_ERR_IMG_TYPE:
      return "Upgrade failed, image type error";
    case UpgradeStatusErrorCode.UPGRATE_ERR_IMG_CRC:
      return "Upgrade failed, image CRC error";
    case UpgradeStatusErrorCode.UPGRATE_ERR_MAX:
      return "Upgrade failed, unknown error";
    case UpgradeStatusErrorCode.UPGRATE_ERR_FLASH_ERR:
      return "Upgrade failed, flash error";
    case UpgradeStatusErrorCode.UPGRATE_ERR_CHIPID:
      return "Upgrade failed, chip ID error";
    default:
      return status.status >= 0 ? null : "Upgrade failed, unknown error";
  }
}
