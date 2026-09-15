import { describe, expect, it } from "vitest";
import { judgeUpgradeStatus, UpgradeStatusErrorCode } from "../upgradeStatus.ts";

function status(code: number) {
  return { percent: 0, status: code, detail: "" };
}

describe("judgeUpgradeStatus", () => {
  it("returns null (still progressing) for STAT_VERIFY_IMAGE and any positive status", () => {
    expect(judgeUpgradeStatus(status(UpgradeStatusErrorCode.STAT_VERIFY_IMAGE))).toBeNull();
    expect(judgeUpgradeStatus(status(1))).toBeNull();
    expect(judgeUpgradeStatus(status(100))).toBeNull();
  });

  it("maps representative hard-error codes to their exact ported messages", () => {
    expect(judgeUpgradeStatus(status(UpgradeStatusErrorCode.UPGRATE_ERR_BOARD_TYPE))).toBe(
      "Upgrade failed, the device model does not match the upgraded firmware",
    );
    expect(judgeUpgradeStatus(status(UpgradeStatusErrorCode.UPGRATE_ERR_BAD_FILE))).toBe(
      "Upgrade failed, upgrade file is corrupted",
    );
    expect(judgeUpgradeStatus(status(UpgradeStatusErrorCode.UPGRATE_ERR_IMG_CRC))).toBe(
      "Upgrade failed, image CRC error",
    );
  });

  it("falls back to a generic message for an unrecognized negative status", () => {
    expect(judgeUpgradeStatus(status(-999))).toBe("Upgrade failed, unknown error");
  });
});
