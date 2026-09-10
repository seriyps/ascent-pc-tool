namespace CaddxTool.Protocol;

// Ported verbatim from Caddx_PCTool.ArConstantsV2.
public static class ArConstantsV2
{
    public const uint MAGIC = 1095914575u;
    public const ushort MSGTYPE_REQUEST = 1;
    public const ushort MSGTYPE_ACK = 2;
    public const ushort FMT_BINARY = 2;
    public const ushort VERSION = 3292;
    public const ushort UNUSED = 43981;
    public const int HEADER_LENGTH = 36;

    // Commands observed live against real hardware (see FirmwareUpgradeFlowV2 /
    // UpgradeProcessFSM in the decompiled source for the full set).
    public const uint CMD_FIND_DEVICE = 0x3C; // 60 - device info request

    // Firmware-upgrade command set, ported from FirmwareUpgradeFlowV2/ArTransportV2.
    public const uint CMD_REBOOT = 3; // "clean"/mode reboot, sent with SendNoAck in the original
    public const uint CMD_REMOTE_UPGRADE = 114;
    public const uint CMD_SENDFILE_START = 115;
    public const uint CMD_SENDFILE_DATA = 116;
    public const uint CMD_SENDFILE_END = 117;
    public const uint CMD_UPGRADE_STATUS = 118;

    // Ack retry/timeout, ported from ArTransportV2.AckRetryIntervalMs/AckTotalTimeoutMs.
    public const int ACK_RETRY_INTERVAL_MS = 2000;
    public const int ACK_TOTAL_TIMEOUT_MS = 10000;

    // Reconnect-after-reboot timing, ported from GD.cs (ReOpenTimeout_Asce/ReElectDelay).
    public const int RECONNECT_TIMEOUT_MS = 60000;
    public const int RECONNECT_POLL_INTERVAL_MS = 1000;

    // Device-specific reboot wait before starting reconnect polling, ported verbatim
    // from GD.CreateDefaultReOpenDelayByDevName_Asce. Fallback for unlisted device
    // names below is REBOOT_WAIT_DEFAULT_MS.
    public const int REBOOT_WAIT_DEFAULT_MS = 10000;

    public static readonly System.Collections.Generic.IReadOnlyDictionary<string, int> RebootWaitByDeviceName =
        new System.Collections.Generic.Dictionary<string, int>(System.StringComparer.OrdinalIgnoreCase)
        {
            ["ascent_vrx"] = 9500,
            ["ascent_vrx_pro"] = 12000,
            ["cx485_pro"] = 12000,
            ["ascent_vrx_max"] = 12000,
            ["ascent_vrx_max_hf"] = 12000,
            ["ascent_vrx_max_wf"] = 12000,
            ["ascent_vrx_cine"] = 12000,
            ["ascent_gt_pro"] = 9500,
            ["ascent_gt_pro_z40"] = 9500,
            ["ascent_gt_pro_z8"] = 9500,
            ["ascent_gt_pro_hub"] = 9500,
            ["ascent_gt"] = 9500,
            ["ascent_gt_27"] = 9500,
            ["ascent_gt_night"] = 9500,
            ["ascent_gt_ultra"] = 9500,
            ["ascent_gt_max"] = 11500,
            ["ascent_lite"] = 8000,
            ["ascent_lite_plus"] = 8000,
            ["yohd_micro"] = 8000,
            ["ascent_rc"] = 8000,
            ["caddx_gm3"] = 6000,
            ["caddx_gm1"] = 6000,
        };
}
