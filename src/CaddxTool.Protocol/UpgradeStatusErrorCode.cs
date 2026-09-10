namespace CaddxTool.Protocol;

// Ported verbatim from Caddx_PCTool.UpgradeStatusErrorCode. Negative values are
// hard errors; STAT_VERIFY_IMAGE and above mean the upgrade is still progressing.
public enum UpgradeStatusErrorCode
{
    UPGRATE_ERR_CHIPID = -12,
    UPGRATE_ERR_FLASH_ERR,
    UPGRATE_ERR_MAX,
    UPGRATE_ERR_IMG_CRC,
    UPGRATE_ERR_IMG_TYPE,
    UPGRATE_ERR_UPGRADE_MODE,
    UPGRATE_ERR_APP_VERSION,
    UPGRATE_ERR_BOARD_TYPE,
    UPGRATE_ERR_BAD_FILE,
    UPGRATE_ERR_NO_FILE,
    UPGRATE_ERR_NO_PC,
    UPGRATE_ERR_NO_SD,
    STAT_VERIFY_IMAGE,
    STAT_START,
    STAT_IN_PROGRESS,
    STAT_DONE,
    STAT_ENTER_CLEAN_SYSTEM,
    STAT_ENTER_CLEAN_SYSTEM_FAILED,
}
