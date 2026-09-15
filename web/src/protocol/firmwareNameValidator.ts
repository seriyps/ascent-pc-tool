// Ported from src/CaddxTool.Protocol/FirmwareNameValidator.cs (itself ported
// from the decompiled vendor source, Caddx_PCTool.AscentUpdataFrm.
// SelectWrongFWFilePrompt). This is a soft, dismissible pre-flight warning —
// the real safety net is the device's own UPGRATE_ERR_BOARD_TYPE rejection
// mid-upgrade (see upgradeStatus.ts).
//
// The device's firmware-info string implicitly encodes its board-type name,
// e.g. "Ascent_VRX_Pro_18_21_7". The original strips a trailing run of 3+
// digits, or falls back to a 12-char prefix (which can truncate mid-word —
// harmless, since real firmware filenames share the same prefix).
export function expectedPrefix(firmwareInfo: string): string {
  const match = /(\d\d\d+)$/.exec(firmwareInfo);
  if (match) {
    return firmwareInfo.slice(0, match.index - 1).replace("Ascent", "");
  }
  return firmwareInfo.slice(0, Math.min(12, firmwareInfo.length));
}

// True if the selected file's name doesn't look like it belongs to the
// connected device. Fails open (no mismatch reported) on unexpected input —
// deliberately unlike the original's silent block-with-no-dialog on parse
// failure, since a silent block with no explanation is worse UX than an
// occasional missed warning.
export function looksMismatched(fileName: string, firmwareInfo: string): boolean {
  try {
    const name = fileName.replace(".img", "");
    return !name.includes(expectedPrefix(firmwareInfo));
  } catch {
    return false;
  }
}
