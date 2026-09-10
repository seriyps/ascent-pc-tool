namespace Caddx_PCTool;

public static class MSPCMD
{
	public const byte MSP_API_VERSION = 1;

	public const byte MSP_FC_VARIANT = 2;

	public const byte MSP_FC_VERSION = 3;

	public const byte MSP_BOARD_INFO = 4;

	public const byte MSP_BUILD_INFO = 5;

	public const byte MSP_INAV_PID = 6;

	public const byte MSP_SET_INAV_PID = 7;

	public const byte MSP_NAME = 10;

	public const byte MSP_SET_NAME = 11;

	public const byte MSP_NAV_POSHOLD = 12;

	public const byte MSP_SET_NAV_POSHOLD = 13;

	public const byte MSP_CALIBRATION_DATA = 14;

	public const byte MSP_SET_CALIBRATION_DATA = 15;

	public const byte MSP_POSITION_ESTIMATION_CONFIG = 16;

	public const byte MSP_SET_POSITION_ESTIMATION_CONFIG = 17;

	public const byte MSP_WP_MISSION_LOAD = 18;

	public const byte MSP_WP_MISSION_SAVE = 19;

	public const byte MSP_WP_GETINFO = 20;

	public const byte MSP_RTH_AND_LAND_CONFIG = 21;

	public const byte MSP_SET_RTH_AND_LAND_CONFIG = 22;

	public const byte MSP_FW_CONFIG = 23;

	public const byte MSP_SET_FW_CONFIG = 24;

	public const byte MSP_CHANNEL_FORWARDING = 32;

	public const byte MSP_SET_CHANNEL_FORWARDING = 33;

	public const byte MSP_MODE_RANGES = 34;

	public const byte MSP_SET_MODE_RANGE = 35;

	public const byte MSP_FEATURE = 36;

	public const byte MSP_SET_FEATURE = 37;

	public const byte MSP_BOARD_ALIGNMENT = 38;

	public const byte MSP_SET_BOARD_ALIGNMENT = 39;

	public const byte MSP_CURRENT_METER_CONFIG = 40;

	public const byte MSP_SET_CURRENT_METER_CONFIG = 41;

	public const byte MSP_RX_CONFIG = 44;

	public const byte MSP_SET_RX_CONFIG = 45;

	public const byte MSP_LED_COLORS = 46;

	public const byte MSP_SET_LED_COLORS = 47;

	public const byte MSP_LED_STRIP_CONFIG = 48;

	public const byte MSP_SET_LED_STRIP_CONFIG = 49;

	public const byte MSP_ADJUSTMENT_RANGES = 52;

	public const byte MSP_SET_ADJUSTMENT_RANGE = 53;

	public const byte MSP_CF_SERIAL_CONFIG = 54;

	public const byte MSP_SET_CF_SERIAL_CONFIG = 55;

	public const byte MSP_SONAR = 58;

	public const byte MSP_ARMING_CONFIG = 61;

	public const byte MSP_SET_ARMING_CONFIG = 62;

	public const byte MSP_DATAFLASH_SUMMARY = 70;

	public const byte MSP_DATAFLASH_READ = 71;

	public const byte MSP_DATAFLASH_ERASE = 72;

	public const byte MSP_LOOP_TIME = 73;

	public const byte MSP_SET_LOOP_TIME = 74;

	public const byte MSP_FAILSAFE_CONFIG = 75;

	public const byte MSP_SET_FAILSAFE_CONFIG = 76;

	public const byte MSP_RXFAIL_CONFIG = 77;

	public const byte MSP_SET_RXFAIL_CONFIG = 78;

	public const byte MSP_SDCARD_SUMMARY = 79;

	public const byte MSP_BLACKBOX_CONFIG = 80;

	public const byte MSP_SET_BLACKBOX_CONFIG = 81;

	public const byte MSP_OSD_CONFIG = 84;

	public const byte MSP_SET_OSD_CONFIG = 85;

	public const byte MSP_OSD_CHAR_READ = 86;

	public const byte MSP_OSD_CHAR_WRITE = 87;

	public const byte MSP_VTX_CONFIG = 88;

	public const byte MSP_SET_VTX_CONFIG = 89;

	public const byte MSP_ADVANCED_CONFIG = 90;

	public const byte MSP_SET_ADVANCED_CONFIG = 91;

	public const byte MSP_FILTER_CONFIG = 92;

	public const byte MSP_SET_FILTER_CONFIG = 93;

	public const byte MSP_PID_ADVANCED = 94;

	public const byte MSP_SET_PID_ADVANCED = 95;

	public const byte MSP_SENSOR_CONFIG = 96;

	public const byte MSP_SET_SENSOR_CONFIG = 97;

	public const byte MSP_STATUS = 101;

	public const byte MSP_RAW_IMU = 102;

	public const byte MSP_SERVO = 103;

	public const byte MSP_MOTOR = 104;

	public const byte MSP_RC = 105;

	public const byte MSP_RAW_GPS = 106;

	public const byte MSP_COMP_GPS = 107;

	public const byte MSP_ATTITUDE = 108;

	public const byte MSP_ALTITUDE = 109;

	public const byte MSP_ANALOG = 110;

	public const byte MSP_RC_TUNING = 111;

	public const byte MSP_PID = 112;

	public const byte MSP_ACTIVEBOXES = 113;

	public const byte MSP_MISC = 114;

	public const byte MSP_MOTOR_PINS = 115;

	public const byte MSP_BOXNAMES = 116;

	public const byte MSP_PIDNAMES = 117;

	public const byte MSP_WP = 118;

	public const byte MSP_BOXIDS = 119;

	public const byte MSP_SERVO_CONFIGURATIONS = 120;

	public const byte MSP_3D = 124;

	public const byte MSP_RC_DEADBAND = 125;

	public const byte MSP_SENSOR_ALIGNMENT = 126;

	public const byte MSP_LED_STRIP_MODECOLOR = 127;

	public const byte MSP_STATUS_EX = 150;

	public const byte MSP_SENSOR_STATUS = 151;

	public const byte MSP_SET_RAW_RC = 200;

	public const byte MSP_SET_RAW_GPS = 201;

	public const byte MSP_SET_PID = 202;

	public const byte MSP_SET_BOX = 203;

	public const byte MSP_SET_RC_TUNING = 204;

	public const byte MSP_ACC_CALIBRATION = 205;

	public const byte MSP_MAG_CALIBRATION = 206;

	public const byte MSP_SET_MISC = 207;

	public const byte MSP_RESET_CONF = 208;

	public const byte MSP_SET_WP = 209;

	public const byte MSP_SELECT_SETTING = 210;

	public const byte MSP_SET_HEAD = 211;

	public const byte MSP_SET_SERVO_CONFIGURATION = 212;

	public const byte MSP_SET_MOTOR = 214;

	public const byte MSP_SET_3D = 217;

	public const byte MSP_SET_RC_DEADBAND = 218;

	public const byte MSP_SET_RESET_CURR_PID = 219;

	public const byte MSP_SET_SENSOR_ALIGNMENT = 220;

	public const byte MSP_SET_LED_STRIP_MODECOLOR = 221;

	public const byte MSP_SERVO_MIX_RULES = 241;

	public const byte MSP_SET_SERVO_MIX_RULE = 242;

	public const byte MSP_RTC = 246;

	public const byte MSP_SET_RTC = 247;

	public const byte MSP_EEPROM_WRITE = 250;

	public const byte MSP_DEBUGMSG = 253;

	public const byte MSP_DEBUG = 254;

	public const byte MSP_UID = 160;

	public const byte MSP_ACC_TRIM = 240;

	public const byte MSP_SET_ACC_TRIM = 239;

	public const byte MSP_GPS_SV_INFO = 164;

	public const byte MSP_GPSSTATISTICS = 166;

	public const byte MSP_RX_MAP = 64;

	public const byte MSP_SET_RX_MAP = 65;

	public const byte MSP_BF_CONFIG = 66;

	public const byte MSP_SET_BF_CONFIG = 67;

	public const byte MSP_SET_REBOOT = 68;

	public const byte MSP_BF_BUILD_INFO = 69;
}
