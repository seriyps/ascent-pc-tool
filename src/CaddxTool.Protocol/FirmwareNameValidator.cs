using System;
using System.Text.RegularExpressions;

namespace CaddxTool.Protocol;

// Ported from Caddx_PCTool.AscentUpdataFrm.SelectWrongFWFilePrompt (decompiled
// source, secret-not-secret branch) — the vendor's PC-side pre-flight check
// before a firmware upgrade starts. This is not the actual safety net: the
// device itself independently rejects a mismatched image mid-upgrade
// (UpgradeStatusErrorCode.UPGRATE_ERR_BOARD_TYPE, already surfaced by
// FirmwareUpgradeFlow.JudgeUpgradeStatus). This check exists purely to catch
// an obviously-wrong file before spending minutes on a doomed transfer, and
// in the original app it's a dismissible warning, not a hard block — ported
// the same way here.
public static class FirmwareNameValidator
{
    // The device's own reported firmware-info string implicitly encodes its
    // board-type name, e.g. "Ascent_VRX_Pro_18_21_7", "Ascent_G_Sky_17_5_15",
    // "Ascent_H_Sky_18_21_10" (real values, see CaddxTool.FakeDevice's
    // FakeDeviceProfile.cs). The original strips a trailing run of 3+ digits
    // (rarely hit in practice — Ascent version suffixes are usually 1-2
    // digits, so this branch mostly doesn't fire) or, failing that, just
    // takes the first 12 characters — which can truncate mid-word
    // ("Ascent_VRX_Pro..." -> "Ascent_VRX_P") but that's harmless, since real
    // firmware filenames share the same prefix and a plain substring check
    // still matches.
    public static string ExpectedPrefix(string firmwareInfo)
    {
        Match match = Regex.Match(firmwareInfo, @"(\d+\d+\d+)$");
        return match.Success
            ? firmwareInfo.Substring(0, match.Index - 1).Replace("Ascent", "")
            : firmwareInfo.Substring(0, Math.Min(12, firmwareInfo.Length));
    }

    // True if the selected file's name doesn't look like it belongs to the
    // connected device (mirrors the original's `!text.Contains(value)`).
    // Unlike the original — which silently treats a parse failure as "block,
    // no dialog shown" (a broad try/catch around the whole method) — this
    // deliberately fails open (no mismatch) on unexpected input, since a
    // silent block with no explanation is worse UX than an occasional missed
    // warning; the device-side hard check still catches a genuinely wrong
    // image either way.
    public static bool LooksMismatched(string fileName, string firmwareInfo)
    {
        try
        {
            string name = fileName.Replace(".img", "");
            return !name.Contains(ExpectedPrefix(firmwareInfo));
        }
        catch
        {
            return false;
        }
    }
}
