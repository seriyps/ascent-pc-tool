using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace Caddx_PCTool;

public class MAVLink
{
	public struct message_info
	{
		public uint msgid { get; internal set; }

		public string name { get; internal set; }

		public byte crc { get; internal set; }

		public uint minlength { get; internal set; }

		public uint length { get; internal set; }

		public Type type { get; internal set; }

		public message_info(uint msgid, string name, byte crc, uint minlength, uint length, Type type)
		{
			this.msgid = msgid;
			this.name = name;
			this.crc = crc;
			this.minlength = minlength;
			this.length = length;
			this.type = type;
		}

		public override string ToString()
		{
			return $"{name} - {msgid}";
		}
	}

	public enum MAVLINK_MSG_ID
	{
		HEARTBEAT = 0,
		SYS_STATUS = 1,
		SYSTEM_TIME = 2,
		PING = 4,
		CHANGE_OPERATOR_CONTROL = 5,
		CHANGE_OPERATOR_CONTROL_ACK = 6,
		AUTH_KEY = 7,
		LINK_NODE_STATUS = 8,
		SET_MODE = 11,
		PARAM_REQUEST_READ = 20,
		PARAM_REQUEST_LIST = 21,
		PARAM_VALUE = 22,
		PARAM_SET = 23,
		GPS_RAW_INT = 24,
		GPS_STATUS = 25,
		SCALED_IMU = 26,
		RAW_IMU = 27,
		RAW_PRESSURE = 28,
		SCALED_PRESSURE = 29,
		ATTITUDE = 30,
		ATTITUDE_QUATERNION = 31,
		LOCAL_POSITION_NED = 32,
		GLOBAL_POSITION_INT = 33,
		RC_CHANNELS_SCALED = 34,
		RC_CHANNELS_RAW = 35,
		SERVO_OUTPUT_RAW = 36,
		MISSION_REQUEST_PARTIAL_LIST = 37,
		MISSION_WRITE_PARTIAL_LIST = 38,
		MISSION_ITEM = 39,
		MISSION_REQUEST = 40,
		MISSION_SET_CURRENT = 41,
		MISSION_CURRENT = 42,
		MISSION_REQUEST_LIST = 43,
		MISSION_COUNT = 44,
		MISSION_CLEAR_ALL = 45,
		MISSION_ITEM_REACHED = 46,
		MISSION_ACK = 47,
		SET_GPS_GLOBAL_ORIGIN = 48,
		GPS_GLOBAL_ORIGIN = 49,
		PARAM_MAP_RC = 50,
		MISSION_REQUEST_INT = 51,
		SAFETY_SET_ALLOWED_AREA = 54,
		SAFETY_ALLOWED_AREA = 55,
		ATTITUDE_QUATERNION_COV = 61,
		NAV_CONTROLLER_OUTPUT = 62,
		GLOBAL_POSITION_INT_COV = 63,
		LOCAL_POSITION_NED_COV = 64,
		RC_CHANNELS = 65,
		REQUEST_DATA_STREAM = 66,
		DATA_STREAM = 67,
		MANUAL_CONTROL = 69,
		RC_CHANNELS_OVERRIDE = 70,
		MISSION_ITEM_INT = 73,
		VFR_HUD = 74,
		COMMAND_INT = 75,
		COMMAND_LONG = 76,
		COMMAND_ACK = 77,
		COMMAND_CANCEL = 80,
		MANUAL_SETPOINT = 81,
		SET_ATTITUDE_TARGET = 82,
		ATTITUDE_TARGET = 83,
		SET_POSITION_TARGET_LOCAL_NED = 84,
		POSITION_TARGET_LOCAL_NED = 85,
		SET_POSITION_TARGET_GLOBAL_INT = 86,
		POSITION_TARGET_GLOBAL_INT = 87,
		LOCAL_POSITION_NED_SYSTEM_GLOBAL_OFFSET = 89,
		HIL_STATE = 90,
		HIL_CONTROLS = 91,
		HIL_RC_INPUTS_RAW = 92,
		HIL_ACTUATOR_CONTROLS = 93,
		OPTICAL_FLOW = 100,
		GLOBAL_VISION_POSITION_ESTIMATE = 101,
		VISION_POSITION_ESTIMATE = 102,
		VISION_SPEED_ESTIMATE = 103,
		VICON_POSITION_ESTIMATE = 104,
		HIGHRES_IMU = 105,
		OPTICAL_FLOW_RAD = 106,
		HIL_SENSOR = 107,
		SIM_STATE = 108,
		RADIO_STATUS = 109,
		FILE_TRANSFER_PROTOCOL = 110,
		TIMESYNC = 111,
		CAMERA_TRIGGER = 112,
		HIL_GPS = 113,
		HIL_OPTICAL_FLOW = 114,
		HIL_STATE_QUATERNION = 115,
		SCALED_IMU2 = 116,
		LOG_REQUEST_LIST = 117,
		LOG_ENTRY = 118,
		LOG_REQUEST_DATA = 119,
		LOG_DATA = 120,
		LOG_ERASE = 121,
		LOG_REQUEST_END = 122,
		GPS_INJECT_DATA = 123,
		GPS2_RAW = 124,
		POWER_STATUS = 125,
		SERIAL_CONTROL = 126,
		GPS_RTK = 127,
		GPS2_RTK = 128,
		SCALED_IMU3 = 129,
		DATA_TRANSMISSION_HANDSHAKE = 130,
		ENCAPSULATED_DATA = 131,
		DISTANCE_SENSOR = 132,
		TERRAIN_REQUEST = 133,
		TERRAIN_DATA = 134,
		TERRAIN_CHECK = 135,
		TERRAIN_REPORT = 136,
		SCALED_PRESSURE2 = 137,
		ATT_POS_MOCAP = 138,
		SET_ACTUATOR_CONTROL_TARGET = 139,
		ACTUATOR_CONTROL_TARGET = 140,
		ALTITUDE = 141,
		RESOURCE_REQUEST = 142,
		SCALED_PRESSURE3 = 143,
		FOLLOW_TARGET = 144,
		CONTROL_SYSTEM_STATE = 146,
		BATTERY_STATUS = 147,
		AUTOPILOT_VERSION = 148,
		LANDING_TARGET = 149,
		FENCE_STATUS = 162,
		MAG_CAL_REPORT = 192,
		EFI_STATUS = 225,
		ESTIMATOR_STATUS = 230,
		WIND_COV = 231,
		GPS_INPUT = 232,
		GPS_RTCM_DATA = 233,
		HIGH_LATENCY = 234,
		HIGH_LATENCY2 = 235,
		VIBRATION = 241,
		HOME_POSITION = 242,
		SET_HOME_POSITION = 243,
		MESSAGE_INTERVAL = 244,
		EXTENDED_SYS_STATE = 245,
		ADSB_VEHICLE = 246,
		COLLISION = 247,
		V2_EXTENSION = 248,
		MEMORY_VECT = 249,
		DEBUG_VECT = 250,
		NAMED_VALUE_FLOAT = 251,
		NAMED_VALUE_INT = 252,
		STATUSTEXT = 253,
		DEBUG = 254,
		SETUP_SIGNING = 256,
		BUTTON_CHANGE = 257,
		PLAY_TUNE = 258,
		CAMERA_INFORMATION = 259,
		CAMERA_SETTINGS = 260,
		STORAGE_INFORMATION = 261,
		CAMERA_CAPTURE_STATUS = 262,
		CAMERA_IMAGE_CAPTURED = 263,
		FLIGHT_INFORMATION = 264,
		MOUNT_ORIENTATION = 265,
		LOGGING_DATA = 266,
		LOGGING_DATA_ACKED = 267,
		LOGGING_ACK = 268,
		VIDEO_STREAM_INFORMATION = 269,
		VIDEO_STREAM_STATUS = 270,
		CAMERA_FOV_STATUS = 271,
		CAMERA_TRACKING_IMAGE_STATUS = 275,
		CAMERA_TRACKING_GEO_STATUS = 276,
		CAMERA_THERMAL_RANGE = 277,
		GIMBAL_MANAGER_INFORMATION = 280,
		GIMBAL_MANAGER_STATUS = 281,
		GIMBAL_MANAGER_SET_ATTITUDE = 282,
		GIMBAL_DEVICE_INFORMATION = 283,
		GIMBAL_DEVICE_SET_ATTITUDE = 284,
		GIMBAL_DEVICE_ATTITUDE_STATUS = 285,
		AUTOPILOT_STATE_FOR_GIMBAL_DEVICE = 286,
		GIMBAL_MANAGER_SET_PITCHYAW = 287,
		GIMBAL_MANAGER_SET_MANUAL_CONTROL = 288,
		ESC_INFO = 290,
		ESC_STATUS = 291,
		WIFI_CONFIG_AP = 299,
		PROTOCOL_VERSION = 300,
		AIS_VESSEL = 301,
		UAVCAN_NODE_STATUS = 310,
		UAVCAN_NODE_INFO = 311,
		PARAM_EXT_REQUEST_READ = 320,
		PARAM_EXT_REQUEST_LIST = 321,
		PARAM_EXT_VALUE = 322,
		PARAM_EXT_SET = 323,
		PARAM_EXT_ACK = 324,
		OBSTACLE_DISTANCE = 330,
		ODOMETRY = 331,
		TRAJECTORY_REPRESENTATION_WAYPOINTS = 332,
		TRAJECTORY_REPRESENTATION_BEZIER = 333,
		CELLULAR_STATUS = 334,
		ISBD_LINK_STATUS = 335,
		CELLULAR_CONFIG = 336,
		RAW_RPM = 339,
		UTM_GLOBAL_POSITION = 340,
		DEBUG_FLOAT_ARRAY = 350,
		ORBIT_EXECUTION_STATUS = 360,
		SMART_BATTERY_INFO = 370,
		FUEL_STATUS = 371,
		BATTERY_INFO = 372,
		GENERATOR_STATUS = 373,
		ACTUATOR_OUTPUT_STATUS = 375,
		TIME_ESTIMATE_TO_TARGET = 380,
		TUNNEL = 385,
		CAN_FRAME = 386,
		CANFD_FRAME = 387,
		CAN_FILTER_MODIFY = 388,
		ONBOARD_COMPUTER_STATUS = 390,
		COMPONENT_INFORMATION = 395,
		COMPONENT_INFORMATION_BASIC = 396,
		COMPONENT_METADATA = 397,
		PLAY_TUNE_V2 = 400,
		SUPPORTED_TUNES = 401,
		EVENT = 410,
		CURRENT_EVENT_SEQUENCE = 411,
		REQUEST_EVENT = 412,
		RESPONSE_EVENT_ERROR = 413,
		AVAILABLE_MODES = 435,
		CURRENT_MODE = 436,
		AVAILABLE_MODES_MONITOR = 437,
		ILLUMINATOR_STATUS = 440,
		WHEEL_DISTANCE = 9000,
		WINCH_STATUS = 9005,
		OPEN_DRONE_ID_BASIC_ID = 12900,
		OPEN_DRONE_ID_LOCATION = 12901,
		OPEN_DRONE_ID_AUTHENTICATION = 12902,
		OPEN_DRONE_ID_SELF_ID = 12903,
		OPEN_DRONE_ID_SYSTEM = 12904,
		OPEN_DRONE_ID_OPERATOR_ID = 12905,
		OPEN_DRONE_ID_MESSAGE_PACK = 12915,
		OPEN_DRONE_ID_ARM_STATUS = 12918,
		OPEN_DRONE_ID_SYSTEM_UPDATE = 12919,
		HYGROMETER_SENSOR = 12920
	}

	public enum HL_FAILURE_FLAG : ushort
	{
		[Description("GPS failure.")]
		GPS = 1,
		[Description("Differential pressure sensor failure.")]
		DIFFERENTIAL_PRESSURE = 2,
		[Description("Absolute pressure sensor failure.")]
		ABSOLUTE_PRESSURE = 4,
		[Description("Accelerometer sensor failure.")]
		_3D_ACCEL = 8,
		[Description("Gyroscope sensor failure.")]
		_3D_GYRO = 0x10,
		[Description("Magnetometer sensor failure.")]
		_3D_MAG = 0x20,
		[Description("Terrain subsystem failure.")]
		TERRAIN = 0x40,
		[Description("Battery failure/critical low battery.")]
		BATTERY = 0x80,
		[Description("RC receiver failure/no RC connection.")]
		RC_RECEIVER = 0x100,
		[Description("Offboard link failure.")]
		OFFBOARD_LINK = 0x200,
		[Description("Engine failure.")]
		ENGINE = 0x400,
		[Description("Geofence violation.")]
		GEOFENCE = 0x800,
		[Description("Estimator failure, for example measurement rejection or large variances.")]
		ESTIMATOR = 0x1000,
		[Description("Mission failure.")]
		MISSION = 0x2000
	}

	public enum MAV_GOTO
	{
		[Description("Hold at the current position.")]
		DO_HOLD,
		[Description("Continue with the next item in mission execution.")]
		DO_CONTINUE,
		[Description("Hold at the current position of the system")]
		HOLD_AT_CURRENT_POSITION,
		[Description("Hold at the position specified in the parameters of the DO_HOLD action")]
		HOLD_AT_SPECIFIED_POSITION
	}

	public enum MAV_MODE : byte
	{
		[Description("System is not ready to fly, booting, calibrating, etc. No flag is set.")]
		PREFLIGHT = 0,
		[Description("System is allowed to be active, under manual (RC) control, no stabilization (MAV_MODE_FLAG_MANUAL_INPUT_ENABLED)")]
		MANUAL_DISARMED = 64,
		[Description("UNDEFINED mode. This solely depends on the autopilot - use with caution, intended for developers only. (MAV_MODE_FLAG_MANUAL_INPUT_ENABLED, MAV_MODE_FLAG_TEST_ENABLED).")]
		TEST_DISARMED = 66,
		[Description("System is allowed to be active, under assisted RC control (MAV_MODE_FLAG_SAFETY_ARMED, MAV_MODE_FLAG_STABILIZE_ENABLED)")]
		STABILIZE_DISARMED = 80,
		[Description("System is allowed to be active, under autonomous control, manual setpoint (MAV_MODE_FLAG_SAFETY_ARMED, MAV_MODE_FLAG_STABILIZE_ENABLED, MAV_MODE_FLAG_GUIDED_ENABLED)")]
		GUIDED_DISARMED = 88,
		[Description("System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by waypoints). (MAV_MODE_FLAG_SAFETY_ARMED, MAV_MODE_FLAG_STABILIZE_ENABLED, MAV_MODE_FLAG_GUIDED_ENABLED, MAV_MODE_FLAG_AUTO_ENABLED).")]
		AUTO_DISARMED = 92,
		[Description("System is allowed to be active, under manual (RC) control, no stabilization (MAV_MODE_FLAG_SAFETY_ARMED, MAV_MODE_FLAG_MANUAL_INPUT_ENABLED)")]
		MANUAL_ARMED = 192,
		[Description("UNDEFINED mode. This solely depends on the autopilot - use with caution, intended for developers only (MAV_MODE_FLAG_SAFETY_ARMED, MAV_MODE_FLAG_MANUAL_INPUT_ENABLED, MAV_MODE_FLAG_TEST_ENABLED)")]
		TEST_ARMED = 194,
		[Description("System is allowed to be active, under assisted RC control (MAV_MODE_FLAG_SAFETY_ARMED, MAV_MODE_FLAG_MANUAL_INPUT_ENABLED, MAV_MODE_FLAG_STABILIZE_ENABLED)")]
		STABILIZE_ARMED = 208,
		[Description("System is allowed to be active, under autonomous control, manual setpoint (MAV_MODE_FLAG_SAFETY_ARMED, MAV_MODE_FLAG_MANUAL_INPUT_ENABLED, MAV_MODE_FLAG_STABILIZE_ENABLED, MAV_MODE_FLAG_GUIDED_ENABLED)")]
		GUIDED_ARMED = 216,
		[Description("System is allowed to be active, under autonomous control and navigation (the trajectory is decided onboard and not pre-programmed by waypoints). (MAV_MODE_FLAG_SAFETY_ARMED, MAV_MODE_FLAG_MANUAL_INPUT_ENABLED, MAV_MODE_FLAG_STABILIZE_ENABLED, MAV_MODE_FLAG_GUIDED_ENABLED,MAV_MODE_FLAG_AUTO_ENABLED).")]
		AUTO_ARMED = 220
	}

	public enum MAV_SYS_STATUS_SENSOR : uint
	{
		[Description("0x01 3D gyro")]
		_3D_GYRO = 1u,
		[Description("0x02 3D accelerometer")]
		_3D_ACCEL = 2u,
		[Description("0x04 3D magnetometer")]
		_3D_MAG = 4u,
		[Description("0x08 absolute pressure")]
		ABSOLUTE_PRESSURE = 8u,
		[Description("0x10 differential pressure")]
		DIFFERENTIAL_PRESSURE = 0x10u,
		[Description("0x20 GPS")]
		GPS = 0x20u,
		[Description("0x40 optical flow")]
		OPTICAL_FLOW = 0x40u,
		[Description("0x80 computer vision position")]
		VISION_POSITION = 0x80u,
		[Description("0x100 laser based position")]
		LASER_POSITION = 0x100u,
		[Description("0x200 external ground truth (Vicon or Leica)")]
		EXTERNAL_GROUND_TRUTH = 0x200u,
		[Description("0x400 3D angular rate control")]
		ANGULAR_RATE_CONTROL = 0x400u,
		[Description("0x800 attitude stabilization")]
		ATTITUDE_STABILIZATION = 0x800u,
		[Description("0x1000 yaw position")]
		YAW_POSITION = 0x1000u,
		[Description("0x2000 z/altitude control")]
		Z_ALTITUDE_CONTROL = 0x2000u,
		[Description("0x4000 x/y position control")]
		XY_POSITION_CONTROL = 0x4000u,
		[Description("0x8000 motor outputs / control")]
		MOTOR_OUTPUTS = 0x8000u,
		[Description("0x10000 RC receiver")]
		RC_RECEIVER = 0x10000u,
		[Description("0x20000 2nd 3D gyro")]
		_3D_GYRO2 = 0x20000u,
		[Description("0x40000 2nd 3D accelerometer")]
		_3D_ACCEL2 = 0x40000u,
		[Description("0x80000 2nd 3D magnetometer")]
		_3D_MAG2 = 0x80000u,
		[Description("0x100000 geofence")]
		MAV_SYS_STATUS_GEOFENCE = 0x100000u,
		[Description("0x200000 AHRS subsystem health")]
		MAV_SYS_STATUS_AHRS = 0x200000u,
		[Description("0x400000 Terrain subsystem health")]
		MAV_SYS_STATUS_TERRAIN = 0x400000u,
		[Description("0x800000 Motors are reversed")]
		MAV_SYS_STATUS_REVERSE_MOTOR = 0x800000u,
		[Description("0x1000000 Logging")]
		MAV_SYS_STATUS_LOGGING = 0x1000000u,
		[Description("0x2000000 Battery")]
		BATTERY = 0x2000000u,
		[Description("0x4000000 Proximity")]
		PROXIMITY = 0x4000000u,
		[Description("0x8000000 Satellite Communication ")]
		SATCOM = 0x8000000u,
		[Description("0x10000000 pre-arm check status. Always healthy when armed")]
		MAV_SYS_STATUS_PREARM_CHECK = 0x10000000u,
		[Description("0x20000000 Avoidance/collision prevention")]
		MAV_SYS_STATUS_OBSTACLE_AVOIDANCE = 0x20000000u,
		[Description("0x40000000 propulsion (actuator, esc, motor or propellor)")]
		PROPULSION = 0x40000000u,
		[Description("0x80000000 Extended bit-field are used for further sensor status bits (needs to be set in onboard_control_sensors_present only)")]
		MAV_SYS_STATUS_EXTENSION_USED = 0x80000000u
	}

	public enum MAV_SYS_STATUS_SENSOR_EXTENDED : uint
	{
		[Description("0x01 Recovery system (parachute, balloon, retracts etc)")]
		MAV_SYS_STATUS_RECOVERY_SYSTEM = 1u
	}

	public enum MAV_FRAME : byte
	{
		[Description("Global (WGS84) coordinate frame + altitude relative to mean sea level (MSL).")]
		GLOBAL,
		[Description("NED local tangent frame (x: North, y: East, z: Down) with origin fixed relative to earth.")]
		LOCAL_NED,
		[Description("NOT a coordinate frame, indicates a mission command.")]
		MISSION,
		[Description("           Global (WGS84) coordinate frame + altitude relative to the home position.         ")]
		GLOBAL_RELATIVE_ALT,
		[Description("ENU local tangent frame (x: East, y: North, z: Up) with origin fixed relative to earth.")]
		LOCAL_ENU,
		[Description("Global (WGS84) coordinate frame (scaled) + altitude relative to mean sea level (MSL).")]
		GLOBAL_INT,
		[Description("Global (WGS84) coordinate frame (scaled) + altitude relative to the home position. ")]
		GLOBAL_RELATIVE_ALT_INT,
		[Description("NED local tangent frame (x: North, y: East, z: Down) with origin that travels with the vehicle.")]
		LOCAL_OFFSET_NED,
		[Description("Same as MAV_FRAME_LOCAL_NED when used to represent position values. Same as MAV_FRAME_BODY_FRD when used with velocity/acceleration values.")]
		BODY_NED,
		[Description("This is the same as MAV_FRAME_BODY_FRD.")]
		BODY_OFFSET_NED,
		[Description("Global (WGS84) coordinate frame with AGL altitude (altitude at ground level).")]
		GLOBAL_TERRAIN_ALT,
		[Description("Global (WGS84) coordinate frame (scaled) with AGL altitude (altitude at ground level).")]
		GLOBAL_TERRAIN_ALT_INT,
		[Description("FRD local frame aligned to the vehicle's attitude (x: Forward, y: Right, z: Down) with an origin that travels with vehicle.")]
		BODY_FRD,
		[Description("MAV_FRAME_BODY_FLU - Body fixed frame of reference, Z-up (x: Forward, y: Left, z: Up).")]
		RESERVED_13,
		[Description("MAV_FRAME_MOCAP_NED - Odometry local coordinate frame of data given by a motion capture system, Z-down (x: North, y: East, z: Down).")]
		RESERVED_14,
		[Description("MAV_FRAME_MOCAP_ENU - Odometry local coordinate frame of data given by a motion capture system, Z-up (x: East, y: North, z: Up).")]
		RESERVED_15,
		[Description("MAV_FRAME_VISION_NED - Odometry local coordinate frame of data given by a vision estimation system, Z-down (x: North, y: East, z: Down).")]
		RESERVED_16,
		[Description("MAV_FRAME_VISION_ENU - Odometry local coordinate frame of data given by a vision estimation system, Z-up (x: East, y: North, z: Up).")]
		RESERVED_17,
		[Description("MAV_FRAME_ESTIM_NED - Odometry local coordinate frame of data given by an estimator running onboard the vehicle, Z-down (x: North, y: East, z: Down).")]
		RESERVED_18,
		[Description("MAV_FRAME_ESTIM_ENU - Odometry local coordinate frame of data given by an estimator running onboard the vehicle, Z-up (x: East, y: North, z: Up).")]
		RESERVED_19,
		[Description("FRD local tangent frame (x: Forward, y: Right, z: Down) with origin fixed relative to earth. The forward axis is aligned to the front of the vehicle in the horizontal plane.")]
		LOCAL_FRD,
		[Description("FLU local tangent frame (x: Forward, y: Left, z: Up) with origin fixed relative to earth. The forward axis is aligned to the front of the vehicle in the horizontal plane.")]
		LOCAL_FLU
	}

	public enum MAVLINK_DATA_STREAM_TYPE : byte
	{
		[Description("")]
		MAVLINK_DATA_STREAM_IMG_JPEG,
		[Description("")]
		MAVLINK_DATA_STREAM_IMG_BMP,
		[Description("")]
		MAVLINK_DATA_STREAM_IMG_RAW8U,
		[Description("")]
		MAVLINK_DATA_STREAM_IMG_RAW32U,
		[Description("")]
		MAVLINK_DATA_STREAM_IMG_PGM,
		[Description("")]
		MAVLINK_DATA_STREAM_IMG_PNG
	}

	public enum FENCE_BREACH : byte
	{
		[Description("No last fence breach")]
		NONE,
		[Description("Breached minimum altitude")]
		MINALT,
		[Description("Breached maximum altitude")]
		MAXALT,
		[Description("Breached fence boundary")]
		BOUNDARY
	}

	public enum FENCE_MITIGATE : byte
	{
		[Description("Unknown")]
		UNKNOWN,
		[Description("No actions being taken")]
		NONE,
		[Description("Velocity limiting active to prevent breach")]
		VEL_LIMIT
	}

	public enum FENCE_TYPE
	{
		[Description("Maximum altitude fence")]
		ALT_MAX = 1,
		[Description("Circle fence")]
		CIRCLE = 2,
		[Description("Polygon fence")]
		POLYGON = 4,
		[Description("Minimum altitude fence")]
		ALT_MIN = 8
	}

	public enum MAV_MOUNT_MODE
	{
		[Description("Load and keep safe position (Roll,Pitch,Yaw) from permanent memory and stop stabilization")]
		RETRACT,
		[Description("Load and keep neutral position (Roll,Pitch,Yaw) from permanent memory.")]
		NEUTRAL,
		[Description("Load neutral position and start MAVLink Roll,Pitch,Yaw control with stabilization")]
		MAVLINK_TARGETING,
		[Description("Load neutral position and start RC Roll,Pitch,Yaw control with stabilization")]
		RC_TARGETING,
		[Description("Load neutral position and start to point to Lat,Lon,Alt")]
		GPS_POINT,
		[Description("Gimbal tracks system with specified system ID")]
		SYSID_TARGET,
		[Description("Gimbal tracks home position")]
		HOME_LOCATION
	}

	[Flags]
	public enum GIMBAL_DEVICE_CAP_FLAGS : ushort
	{
		[Description("Gimbal device supports a retracted position.")]
		HAS_RETRACT = 1,
		[Description("Gimbal device supports a horizontal, forward looking position, stabilized.")]
		HAS_NEUTRAL = 2,
		[Description("Gimbal device supports rotating around roll axis.")]
		HAS_ROLL_AXIS = 4,
		[Description("Gimbal device supports to follow a roll angle relative to the vehicle.")]
		HAS_ROLL_FOLLOW = 8,
		[Description("Gimbal device supports locking to a roll angle (generally that's the default with roll stabilized).")]
		HAS_ROLL_LOCK = 0x10,
		[Description("Gimbal device supports rotating around pitch axis.")]
		HAS_PITCH_AXIS = 0x20,
		[Description("Gimbal device supports to follow a pitch angle relative to the vehicle.")]
		HAS_PITCH_FOLLOW = 0x40,
		[Description("Gimbal device supports locking to a pitch angle (generally that's the default with pitch stabilized).")]
		HAS_PITCH_LOCK = 0x80,
		[Description("Gimbal device supports rotating around yaw axis.")]
		HAS_YAW_AXIS = 0x100,
		[Description("Gimbal device supports to follow a yaw angle relative to the vehicle (generally that's the default).")]
		HAS_YAW_FOLLOW = 0x200,
		[Description("Gimbal device supports locking to an absolute heading, i.e., yaw angle relative to North (earth frame, often this is an option available).")]
		HAS_YAW_LOCK = 0x400,
		[Description("Gimbal device supports yawing/panning infinitely (e.g. using slip disk).")]
		SUPPORTS_INFINITE_YAW = 0x800,
		[Description("Gimbal device supports yaw angles and angular velocities relative to North (earth frame). This usually requires support by an autopilot via AUTOPILOT_STATE_FOR_GIMBAL_DEVICE. Support can go on and off during runtime, which is reported by the flag GIMBAL_DEVICE_FLAGS_CAN_ACCEPT_YAW_IN_EARTH_FRAME.")]
		SUPPORTS_YAW_IN_EARTH_FRAME = 0x1000,
		[Description("Gimbal device supports radio control inputs as an alternative input for controlling the gimbal orientation.")]
		HAS_RC_INPUTS = 0x2000
	}

	[Flags]
	public enum GIMBAL_MANAGER_CAP_FLAGS : uint
	{
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_RETRACT.")]
		HAS_RETRACT = 1u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_NEUTRAL.")]
		HAS_NEUTRAL = 2u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_ROLL_AXIS.")]
		HAS_ROLL_AXIS = 4u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_ROLL_FOLLOW.")]
		HAS_ROLL_FOLLOW = 8u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_ROLL_LOCK.")]
		HAS_ROLL_LOCK = 0x10u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_PITCH_AXIS.")]
		HAS_PITCH_AXIS = 0x20u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_PITCH_FOLLOW.")]
		HAS_PITCH_FOLLOW = 0x40u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_PITCH_LOCK.")]
		HAS_PITCH_LOCK = 0x80u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_YAW_AXIS.")]
		HAS_YAW_AXIS = 0x100u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_YAW_FOLLOW.")]
		HAS_YAW_FOLLOW = 0x200u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_YAW_LOCK.")]
		HAS_YAW_LOCK = 0x400u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_SUPPORTS_INFINITE_YAW.")]
		SUPPORTS_INFINITE_YAW = 0x800u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_SUPPORTS_YAW_IN_EARTH_FRAME.")]
		SUPPORTS_YAW_IN_EARTH_FRAME = 0x1000u,
		[Description("Based on GIMBAL_DEVICE_CAP_FLAGS_HAS_RC_INPUTS.")]
		HAS_RC_INPUTS = 0x2000u,
		[Description("Gimbal manager supports to point to a local position.")]
		CAN_POINT_LOCATION_LOCAL = 0x10000u,
		[Description("Gimbal manager supports to point to a global latitude, longitude, altitude position.")]
		CAN_POINT_LOCATION_GLOBAL = 0x20000u
	}

	[Flags]
	public enum GIMBAL_DEVICE_FLAGS : ushort
	{
		[Description("Set to retracted safe position (no stabilization), takes precedence over all other flags.")]
		RETRACT = 1,
		[Description("Set to neutral/default position, taking precedence over all other flags except RETRACT. Neutral is commonly forward-facing and horizontal (roll=pitch=yaw=0) but may be any orientation.")]
		NEUTRAL = 2,
		[Description("Lock roll angle to absolute angle relative to horizon (not relative to vehicle). This is generally the default with a stabilizing gimbal.")]
		ROLL_LOCK = 4,
		[Description("Lock pitch angle to absolute angle relative to horizon (not relative to vehicle). This is generally the default with a stabilizing gimbal.")]
		PITCH_LOCK = 8,
		[Description("Lock yaw angle to absolute angle relative to North (not relative to vehicle). If this flag is set, the yaw angle and z component of angular velocity are relative to North (earth frame, x-axis pointing North), else they are relative to the vehicle heading (vehicle frame, earth frame rotated so that the x-axis is pointing forward).")]
		YAW_LOCK = 0x10,
		[Description("Yaw angle and z component of angular velocity are relative to the vehicle heading (vehicle frame, earth frame rotated such that the x-axis is pointing forward).")]
		YAW_IN_VEHICLE_FRAME = 0x20,
		[Description("Yaw angle and z component of angular velocity are relative to North (earth frame, x-axis is pointing North).")]
		YAW_IN_EARTH_FRAME = 0x40,
		[Description("Gimbal device can accept yaw angle inputs relative to North (earth frame). This flag is only for reporting (attempts to set this flag are ignored).")]
		ACCEPTS_YAW_IN_EARTH_FRAME = 0x80,
		[Description("The gimbal orientation is set exclusively by the RC signals feed to the gimbal's radio control inputs. MAVLink messages for setting the gimbal orientation (GIMBAL_DEVICE_SET_ATTITUDE) are ignored.")]
		RC_EXCLUSIVE = 0x100,
		[Description("The gimbal orientation is determined by combining/mixing the RC signals feed to the gimbal's radio control inputs and the MAVLink messages for setting the gimbal orientation (GIMBAL_DEVICE_SET_ATTITUDE). How these two controls are combined or mixed is not defined by the protocol but is up to the implementation.")]
		RC_MIXED = 0x200
	}

	[Flags]
	public enum GIMBAL_MANAGER_FLAGS : uint
	{
		[Description("Based on GIMBAL_DEVICE_FLAGS_RETRACT.")]
		RETRACT = 1u,
		[Description("Based on GIMBAL_DEVICE_FLAGS_NEUTRAL.")]
		NEUTRAL = 2u,
		[Description("Based on GIMBAL_DEVICE_FLAGS_ROLL_LOCK.")]
		ROLL_LOCK = 4u,
		[Description("Based on GIMBAL_DEVICE_FLAGS_PITCH_LOCK.")]
		PITCH_LOCK = 8u,
		[Description("Based on GIMBAL_DEVICE_FLAGS_YAW_LOCK.")]
		YAW_LOCK = 0x10u,
		[Description("Based on GIMBAL_DEVICE_FLAGS_YAW_IN_VEHICLE_FRAME.")]
		YAW_IN_VEHICLE_FRAME = 0x20u,
		[Description("Based on GIMBAL_DEVICE_FLAGS_YAW_IN_EARTH_FRAME.")]
		YAW_IN_EARTH_FRAME = 0x40u,
		[Description("Based on GIMBAL_DEVICE_FLAGS_ACCEPTS_YAW_IN_EARTH_FRAME.")]
		ACCEPTS_YAW_IN_EARTH_FRAME = 0x80u,
		[Description("Based on GIMBAL_DEVICE_FLAGS_RC_EXCLUSIVE.")]
		RC_EXCLUSIVE = 0x100u,
		[Description("Based on GIMBAL_DEVICE_FLAGS_RC_MIXED.")]
		RC_MIXED = 0x200u
	}

	[Flags]
	public enum GIMBAL_DEVICE_ERROR_FLAGS : uint
	{
		[Description("Gimbal device is limited by hardware roll limit.")]
		AT_ROLL_LIMIT = 1u,
		[Description("Gimbal device is limited by hardware pitch limit.")]
		AT_PITCH_LIMIT = 2u,
		[Description("Gimbal device is limited by hardware yaw limit.")]
		AT_YAW_LIMIT = 4u,
		[Description("There is an error with the gimbal encoders.")]
		ENCODER_ERROR = 8u,
		[Description("There is an error with the gimbal power source.")]
		POWER_ERROR = 0x10u,
		[Description("There is an error with the gimbal motors.")]
		MOTOR_ERROR = 0x20u,
		[Description("There is an error with the gimbal's software.")]
		SOFTWARE_ERROR = 0x40u,
		[Description("There is an error with the gimbal's communication.")]
		COMMS_ERROR = 0x80u,
		[Description("Gimbal device is currently calibrating.")]
		CALIBRATION_RUNNING = 0x100u,
		[Description("Gimbal device is not assigned to a gimbal manager.")]
		NO_MANAGER = 0x200u
	}

	public enum GRIPPER_ACTIONS
	{
		[Description("Gripper release cargo.")]
		GRIPPER_ACTION_RELEASE,
		[Description("Gripper grab onto cargo.")]
		GRIPPER_ACTION_GRAB
	}

	public enum WINCH_ACTIONS
	{
		[Description("Allow motor to freewheel.")]
		WINCH_RELAXED,
		[Description("Wind or unwind specified length of line, optionally using specified rate.")]
		WINCH_RELATIVE_LENGTH_CONTROL,
		[Description("Wind or unwind line at specified rate.")]
		WINCH_RATE_CONTROL,
		[Description("Perform the locking sequence to relieve motor while in the fully retracted position. Only action and instance command parameters are used, others are ignored.")]
		WINCH_LOCK,
		[Description("Sequence of drop, slow down, touch down, reel up, lock. Only action and instance command parameters are used, others are ignored.")]
		WINCH_DELIVER,
		[Description("Engage motor and hold current position. Only action and instance command parameters are used, others are ignored.")]
		WINCH_HOLD,
		[Description("Return the reel to the fully retracted position. Only action and instance command parameters are used, others are ignored.")]
		WINCH_RETRACT,
		[Description("Load the reel with line. The winch will calculate the total loaded length and stop when the tension exceeds a threshold. Only action and instance command parameters are used, others are ignored.")]
		WINCH_LOAD_LINE,
		[Description("Spool out the entire length of the line. Only action and instance command parameters are used, others are ignored.")]
		WINCH_ABANDON_LINE,
		[Description("Spools out just enough to present the hook to the user to load the payload. Only action and instance command parameters are used, others are ignored")]
		WINCH_LOAD_PAYLOAD
	}

	public enum UAVCAN_NODE_HEALTH : byte
	{
		[Description("The node is functioning properly.")]
		OK,
		[Description("A critical parameter went out of range or the node has encountered a minor failure.")]
		WARNING,
		[Description("The node has encountered a major failure.")]
		ERROR,
		[Description("The node has suffered a fatal malfunction.")]
		CRITICAL
	}

	public enum UAVCAN_NODE_MODE : byte
	{
		[Description("The node is performing its primary functions.")]
		OPERATIONAL = 0,
		[Description("The node is initializing; this mode is entered immediately after startup.")]
		INITIALIZATION = 1,
		[Description("The node is under maintenance.")]
		MAINTENANCE = 2,
		[Description("The node is in the process of updating its software.")]
		SOFTWARE_UPDATE = 3,
		[Description("The node is no longer available online.")]
		OFFLINE = 7
	}

	public enum ESC_CONNECTION_TYPE : byte
	{
		[Description("Traditional PPM ESC.")]
		PPM,
		[Description("Serial Bus connected ESC.")]
		SERIAL,
		[Description("One Shot PPM ESC.")]
		ONESHOT,
		[Description("I2C ESC.")]
		I2C,
		[Description("CAN-Bus ESC.")]
		CAN,
		[Description("DShot ESC.")]
		DSHOT
	}

	[Flags]
	public enum ESC_FAILURE_FLAGS
	{
		[Description("Over current failure.")]
		ESC_FAILURE_OVER_CURRENT = 1,
		[Description("Over voltage failure.")]
		ESC_FAILURE_OVER_VOLTAGE = 2,
		[Description("Over temperature failure.")]
		ESC_FAILURE_OVER_TEMPERATURE = 4,
		[Description("Over RPM failure.")]
		ESC_FAILURE_OVER_RPM = 8,
		[Description("Inconsistent command failure i.e. out of bounds.")]
		ESC_FAILURE_INCONSISTENT_CMD = 0x10,
		[Description("Motor stuck failure.")]
		ESC_FAILURE_MOTOR_STUCK = 0x20,
		[Description("Generic ESC failure.")]
		ESC_FAILURE_GENERIC = 0x40
	}

	public enum STORAGE_STATUS : byte
	{
		[Description("Storage is missing (no microSD card loaded for example.)")]
		EMPTY,
		[Description("Storage present but unformatted.")]
		UNFORMATTED,
		[Description("Storage present and ready.")]
		READY,
		[Description("Camera does not supply storage status information. Capacity information in STORAGE_INFORMATION fields will be ignored.")]
		NOT_SUPPORTED
	}

	public enum STORAGE_TYPE : byte
	{
		[Description("Storage type is not known.")]
		UNKNOWN = 0,
		[Description("Storage type is USB device.")]
		USB_STICK = 1,
		[Description("Storage type is SD card.")]
		SD = 2,
		[Description("Storage type is microSD card.")]
		MICROSD = 3,
		[Description("Storage type is CFast.")]
		CF = 4,
		[Description("Storage type is CFexpress.")]
		CFE = 5,
		[Description("Storage type is XQD.")]
		XQD = 6,
		[Description("Storage type is HD mass storage type.")]
		HD = 7,
		[Description("Storage type is other, not listed type.")]
		OTHER = 254
	}

	public enum STORAGE_USAGE_FLAG : byte
	{
		[Description("Always set to 1 (indicates STORAGE_INFORMATION.storage_usage is supported).")]
		SET = 1,
		[Description("Storage for saving photos.")]
		PHOTO = 2,
		[Description("Storage for saving videos.")]
		VIDEO = 4,
		[Description("Storage for saving logs.")]
		LOGS = 8
	}

	public enum ORBIT_YAW_BEHAVIOUR
	{
		[Description("Vehicle front points to the center (default).")]
		HOLD_FRONT_TO_CIRCLE_CENTER,
		[Description("Vehicle front holds heading when message received.")]
		HOLD_INITIAL_HEADING,
		[Description("Yaw uncontrolled.")]
		UNCONTROLLED,
		[Description("Vehicle front follows flight path (tangential to circle).")]
		HOLD_FRONT_TANGENT_TO_CIRCLE,
		[Description("Yaw controlled by RC input.")]
		RC_CONTROLLED,
		[Description("Vehicle uses current yaw behaviour (unchanged). The vehicle-default yaw behaviour is used if this value is specified when orbit is first commanded.")]
		UNCHANGED
	}

	public enum WIFI_CONFIG_AP_RESPONSE : sbyte
	{
		[Description("Undefined response. Likely an indicative of a system that doesn't support this request.")]
		UNDEFINED,
		[Description("Changes accepted.")]
		ACCEPTED,
		[Description("Changes rejected.")]
		REJECTED,
		[Description("Invalid Mode.")]
		MODE_ERROR,
		[Description("Invalid SSID.")]
		SSID_ERROR,
		[Description("Invalid Password.")]
		PASSWORD_ERROR
	}

	public enum CELLULAR_CONFIG_RESPONSE : byte
	{
		[Description("Changes accepted.")]
		ACCEPTED,
		[Description("Invalid APN.")]
		APN_ERROR,
		[Description("Invalid PIN.")]
		PIN_ERROR,
		[Description("Changes rejected.")]
		REJECTED,
		[Description("PUK is required to unblock SIM card.")]
		CELLULAR_CONFIG_BLOCKED_PUK_REQUIRED
	}

	public enum WIFI_CONFIG_AP_MODE : sbyte
	{
		[Description("WiFi mode is undefined.")]
		UNDEFINED,
		[Description("WiFi configured as an access point.")]
		AP,
		[Description("WiFi configured as a station connected to an existing local WiFi network.")]
		STATION,
		[Description("WiFi disabled.")]
		DISABLED
	}

	public enum COMP_METADATA_TYPE
	{
		[Description("General information about the component. General metadata includes information about other metadata types supported by the component. Files of this type must be supported, and must be downloadable from vehicle using a MAVLink FTP URI.")]
		GENERAL,
		[Description("Parameter meta data.")]
		PARAMETER,
		[Description("Meta data that specifies which commands and command parameters the vehicle supports. (WIP)")]
		COMMANDS,
		[Description("Meta data that specifies external non-MAVLink peripherals.")]
		PERIPHERALS,
		[Description("Meta data for the events interface.")]
		EVENTS,
		[Description("Meta data for actuator configuration (motors, servos and vehicle geometry) and testing.")]
		ACTUATORS
	}

	public enum ACTUATOR_CONFIGURATION
	{
		[Description("Do nothing.")]
		NONE,
		[Description("Command the actuator to beep now.")]
		BEEP,
		[Description("Permanently set the actuator (ESC) to 3D mode (reversible thrust).")]
		_3D_MODE_ON,
		[Description("Permanently set the actuator (ESC) to non 3D mode (non-reversible thrust).")]
		_3D_MODE_OFF,
		[Description("Permanently set the actuator (ESC) to spin direction 1 (which can be clockwise or counter-clockwise).")]
		SPIN_DIRECTION1,
		[Description("Permanently set the actuator (ESC) to spin direction 2 (opposite of direction 1).")]
		SPIN_DIRECTION2
	}

	public enum ACTUATOR_OUTPUT_FUNCTION
	{
		[Description("No function (disabled).")]
		NONE = 0,
		[Description("Motor 1")]
		MOTOR1 = 1,
		[Description("Motor 2")]
		MOTOR2 = 2,
		[Description("Motor 3")]
		MOTOR3 = 3,
		[Description("Motor 4")]
		MOTOR4 = 4,
		[Description("Motor 5")]
		MOTOR5 = 5,
		[Description("Motor 6")]
		MOTOR6 = 6,
		[Description("Motor 7")]
		MOTOR7 = 7,
		[Description("Motor 8")]
		MOTOR8 = 8,
		[Description("Motor 9")]
		MOTOR9 = 9,
		[Description("Motor 10")]
		MOTOR10 = 10,
		[Description("Motor 11")]
		MOTOR11 = 11,
		[Description("Motor 12")]
		MOTOR12 = 12,
		[Description("Motor 13")]
		MOTOR13 = 13,
		[Description("Motor 14")]
		MOTOR14 = 14,
		[Description("Motor 15")]
		MOTOR15 = 15,
		[Description("Motor 16")]
		MOTOR16 = 16,
		[Description("Servo 1")]
		SERVO1 = 33,
		[Description("Servo 2")]
		SERVO2 = 34,
		[Description("Servo 3")]
		SERVO3 = 35,
		[Description("Servo 4")]
		SERVO4 = 36,
		[Description("Servo 5")]
		SERVO5 = 37,
		[Description("Servo 6")]
		SERVO6 = 38,
		[Description("Servo 7")]
		SERVO7 = 39,
		[Description("Servo 8")]
		SERVO8 = 40,
		[Description("Servo 9")]
		SERVO9 = 41,
		[Description("Servo 10")]
		SERVO10 = 42,
		[Description("Servo 11")]
		SERVO11 = 43,
		[Description("Servo 12")]
		SERVO12 = 44,
		[Description("Servo 13")]
		SERVO13 = 45,
		[Description("Servo 14")]
		SERVO14 = 46,
		[Description("Servo 15")]
		SERVO15 = 47,
		[Description("Servo 16")]
		SERVO16 = 48
	}

	public enum AUTOTUNE_AXIS
	{
		[Description("Autotune roll axis.")]
		ROLL = 1,
		[Description("Autotune pitch axis.")]
		PITCH = 2,
		[Description("Autotune yaw axis.")]
		YAW = 4
	}

	public enum PREFLIGHT_STORAGE_PARAMETER_ACTION
	{
		[Description("Read all parameters from persistent storage. Replaces values in volatile storage.")]
		PARAM_READ_PERSISTENT,
		[Description("Write all parameter values to persistent storage (flash/EEPROM)")]
		PARAM_WRITE_PERSISTENT,
		[Description("Reset all user configurable parameters to their default value (including airframe selection, sensor calibration data, safety settings, and so on). Does not reset values that contain operation counters and vehicle computed statistics.")]
		PARAM_RESET_CONFIG_DEFAULT,
		[Description("Reset only sensor calibration parameters to factory defaults (or firmware default if not available)")]
		PARAM_RESET_SENSOR_DEFAULT,
		[Description("Reset all parameters, including operation counters, to default values")]
		PARAM_RESET_ALL_DEFAULT
	}

	public enum PREFLIGHT_STORAGE_MISSION_ACTION
	{
		[Description("Read current mission data from persistent storage")]
		MISSION_READ_PERSISTENT,
		[Description("Write current mission data to persistent storage")]
		MISSION_WRITE_PERSISTENT,
		[Description("Erase all mission data stored on the vehicle (both persistent and volatile storage)")]
		MISSION_RESET_DEFAULT
	}

	public enum REBOOT_SHUTDOWN_CONDITIONS
	{
		[Description("Reboot/Shutdown only if allowed by safety checks, such as being landed.")]
		SAFETY_INTERLOCKED = 0,
		[Description("Force reboot/shutdown of the autopilot/component regardless of system state.")]
		FORCE = 20190226
	}

	public enum MAV_CMD : ushort
	{
		[Description("Navigate to waypoint. This is intended for use in missions (for guided commands outside of missions use MAV_CMD_DO_REPOSITION).")]
		WAYPOINT = 16,
		[Description("Loiter around this waypoint an unlimited amount of time")]
		LOITER_UNLIM = 17,
		[Description("Loiter around this waypoint for X turns")]
		LOITER_TURNS = 18,
		[Description("Loiter at the specified latitude, longitude and altitude for a certain amount of time. Multicopter vehicles stop at the point (within a vehicle-specific acceptance radius). Forward-only moving vehicles (e.g. fixed-wing) circle the point with the specified radius/direction. If the Heading Required parameter (2) is non-zero forward moving aircraft will only leave the loiter circle once heading towards the next waypoint.")]
		LOITER_TIME = 19,
		[Description("Return to launch location")]
		RETURN_TO_LAUNCH = 20,
		[Description("Land at location.")]
		LAND = 21,
		[Description("Takeoff from ground / hand. Vehicles that support multiple takeoff modes (e.g. VTOL quadplane) should take off using the currently configured mode.")]
		TAKEOFF = 22,
		[Description("Land at local position (local frame only)")]
		LAND_LOCAL = 23,
		[Description("Takeoff from local position (local frame only)")]
		TAKEOFF_LOCAL = 24,
		[Description("Vehicle following, i.e. this waypoint represents the position of a moving vehicle")]
		FOLLOW = 25,
		[Description("Continue on the current course and climb/descend to specified altitude.  When the altitude is reached continue to the next command (i.e., don't proceed to the next command until the desired altitude is reached.")]
		CONTINUE_AND_CHANGE_ALT = 30,
		[Description("Begin loiter at the specified Latitude and Longitude.  If Lat=Lon=0, then loiter at the current position.  Don't consider the navigation command complete (don't leave loiter) until the altitude has been reached. Additionally, if the Heading Required parameter is non-zero the aircraft will not leave the loiter until heading toward the next waypoint.")]
		LOITER_TO_ALT = 31,
		[Description("Begin following a target")]
		DO_FOLLOW = 32,
		[Description("Reposition the MAV after a follow target command has been sent")]
		DO_FOLLOW_REPOSITION = 33,
		[Description("Start orbiting on the circumference of a circle defined by the parameters. Setting values to NaN/INT32_MAX (as appropriate) results in using defaults.")]
		DO_ORBIT = 34,
		[Description("Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicle's control system to control the vehicle attitude and the attitude of various sensors such as cameras.")]
		ROI = 80,
		[Description("Control autonomous path planning on the MAV.")]
		PATHPLANNING = 81,
		[Description("Navigate to waypoint using a spline path.")]
		SPLINE_WAYPOINT = 82,
		[Description("Takeoff from ground using VTOL mode, and transition to forward flight with specified heading. The command should be ignored by vehicles that dont support both VTOL and fixed-wing flight (multicopters, boats,etc.).")]
		VTOL_TAKEOFF = 84,
		[Description("Land using VTOL mode")]
		VTOL_LAND = 85,
		[Description("Hand control over to an external controller")]
		GUIDED_ENABLE = 92,
		[Description("Delay the next navigation command a number of seconds or until a specified time")]
		DELAY = 93,
		[Description("Descend and place payload. Vehicle moves to specified location, descends until it detects a hanging payload has reached the ground, and then releases the payload. If ground is not detected before the reaching the maximum descent value (param1), the command will complete without releasing the payload.")]
		PAYLOAD_PLACE = 94,
		[Description("NOP - This command is only used to mark the upper limit of the NAV/ACTION commands in the enumeration")]
		LAST = 95,
		[Description("Delay mission state machine.")]
		CONDITION_DELAY = 112,
		[Description("Ascend/descend to target altitude at specified rate. Delay mission state machine until desired altitude reached.")]
		CONDITION_CHANGE_ALT = 113,
		[Description("Delay mission state machine until within desired distance of next NAV point.")]
		CONDITION_DISTANCE = 114,
		[Description("Reach a certain target angle.")]
		CONDITION_YAW = 115,
		[Description("NOP - This command is only used to mark the upper limit of the CONDITION commands in the enumeration")]
		CONDITION_LAST = 159,
		[Description("Set system mode.")]
		DO_SET_MODE = 176,
		[Description("Jump to the desired command in the mission list.  Repeat this action only the specified number of times")]
		DO_JUMP = 177,
		[Description("Change speed and/or throttle set points. The value persists until it is overridden or there is a mode change")]
		DO_CHANGE_SPEED = 178,
		[Description("           Sets the home position to either to the current position or a specified position.           The home position is the default position that the system will return to and land on.           The position is set automatically by the system during the takeoff (and may also be set using this command).           Note: the current home position may be emitted in a HOME_POSITION message on request (using MAV_CMD_REQUEST_MESSAGE with param1=242).         ")]
		DO_SET_HOME = 179,
		[Description("Set a system parameter.  Caution!  Use of this command requires knowledge of the numeric enumeration value of the parameter.")]
		DO_SET_PARAMETER = 180,
		[Description("Set a relay to a condition.")]
		DO_SET_RELAY = 181,
		[Description("Cycle a relay on and off for a desired number of cycles with a desired period.")]
		DO_REPEAT_RELAY = 182,
		[Description("Set a servo to a desired PWM value.")]
		DO_SET_SERVO = 183,
		[Description("Cycle a between its nominal setting and a desired PWM for a desired number of cycles with a desired period.")]
		DO_REPEAT_SERVO = 184,
		[Description("Terminate flight immediately.           Flight termination immediately and irreversibly terminates the current flight, returning the vehicle to ground.           The vehicle will ignore RC or other input until it has been power-cycled.           Termination may trigger safety measures, including: disabling motors and deployment of parachute on multicopters, and setting flight surfaces to initiate a landing pattern on fixed-wing).           On multicopters without a parachute it may trigger a crash landing.           Support for this command can be tested using the protocol bit: MAV_PROTOCOL_CAPABILITY_FLIGHT_TERMINATION.           Support for this command can also be tested by sending the command with param1=0 (< 0.5); the ACK should be either MAV_RESULT_FAILED or MAV_RESULT_UNSUPPORTED.         ")]
		DO_FLIGHTTERMINATION = 185,
		[Description("Change altitude set point.")]
		DO_CHANGE_ALTITUDE = 186,
		[Description("Sets actuators (e.g. servos) to a desired value. The actuator numbers are mapped to specific outputs (e.g. on any MAIN or AUX PWM or UAVCAN) using a flight-stack specific mechanism (i.e. a parameter).")]
		DO_SET_ACTUATOR = 187,
		[Description("Mission item to specify the start of a failsafe/landing return-path segment (the end of the segment is the next MAV_CMD_DO_LAND_START item).           A vehicle that is using missions for landing (e.g. in a return mode) will join the mission on the closest path of the return-path segment (instead of MAV_CMD_DO_LAND_START or the nearest waypoint).           The main use case is to minimize the failsafe flight path in corridor missions, where the inbound/outbound paths are constrained (by geofences) to the same particular path.           The MAV_CMD_NAV_RETURN_PATH_START would be placed at the start of the return path.           If a failsafe occurs on the outbound path the vehicle will move to the nearest point on the return path (which is parallel for this kind of mission), effectively turning round and following the shortest path to landing.           If a failsafe occurs on the inbound path the vehicle is already on the return segment and will continue to landing.           The Latitude/Longitude/Altitude are optional, and may be set to 0 if not needed.           If specified, the item defines the waypoint at which the return segment starts.           If sent using as a command, the vehicle will perform a mission landing (using the land segment if defined) or reject the command if mission landings are not supported, or no mission landing is defined. When used as a command any position information in the command is ignored.         ")]
		DO_RETURN_PATH_START = 188,
		[Description("Mission item to mark the start of a mission landing pattern, or a command to land with a mission landing pattern.          When used in a mission, this is a marker for the start of a sequence of mission items that represent a landing pattern.         It should be followed by a navigation item that defines the first waypoint of the landing sequence.         The start marker positional params are used only for selecting what landing pattern to use if several are defined in the mission (the selected pattern will be the one with the marker position that is closest to the vehicle when a landing is commanded).         If the marker item position has zero-values for latitude, longitude, and altitude, then landing pattern selection is instead based on the position of the first waypoint in the landing sequence.  \t      When sent as a command it triggers a landing using a mission landing pattern. \t      The location parameters are not used in this case, and should be set to 0. \t")]
		DO_LAND_START = 189,
		[Description("Mission command to perform a landing from a rally point.")]
		DO_RALLY_LAND = 190,
		[Description("Mission command to safely abort an autonomous landing.")]
		DO_GO_AROUND = 191,
		[Description("Reposition the vehicle to a specific WGS84 global position. This command is intended for guided commands (for missions use MAV_CMD_NAV_WAYPOINT instead).")]
		DO_REPOSITION = 192,
		[Description("If in a GPS controlled position mode, hold the current position or continue.")]
		DO_PAUSE_CONTINUE = 193,
		[Description("Set moving direction to forward or reverse.")]
		DO_SET_REVERSE = 194,
		[Description("Sets the region of interest (ROI) to a location. This can then be used by the vehicle's control system to control the vehicle attitude and the attitude of various sensors such as cameras. This command can be sent to a gimbal manager but not to a gimbal device. A gimbal is not to react to this message.")]
		DO_SET_ROI_LOCATION = 195,
		[Description("Sets the region of interest (ROI) to be toward next waypoint, with optional pitch/roll/yaw offset. This can then be used by the vehicle's control system to control the vehicle attitude and the attitude of various sensors such as cameras. This command can be sent to a gimbal manager but not to a gimbal device. A gimbal device is not to react to this message.")]
		DO_SET_ROI_WPNEXT_OFFSET = 196,
		[Description("Cancels any previous ROI command returning the vehicle/sensors to default flight characteristics. This can then be used by the vehicle's control system to control the vehicle attitude and the attitude of various sensors such as cameras. This command can be sent to a gimbal manager but not to a gimbal device. A gimbal device is not to react to this message. After this command the gimbal manager should go back to manual input if available, and otherwise assume a neutral position.")]
		DO_SET_ROI_NONE = 197,
		[Description("Mount tracks system with specified system ID. Determination of target vehicle position may be done with GLOBAL_POSITION_INT or any other means. This command can be sent to a gimbal manager but not to a gimbal device. A gimbal device is not to react to this message.")]
		DO_SET_ROI_SYSID = 198,
		[Description("Control onboard camera system.")]
		DO_CONTROL_VIDEO = 200,
		[Description("Sets the region of interest (ROI) for a sensor set or the vehicle itself. This can then be used by the vehicle's control system to control the vehicle attitude and the attitude of various sensors such as cameras.")]
		DO_SET_ROI = 201,
		[Description("Configure digital camera. This is a fallback message for systems that have not yet implemented PARAM_EXT_XXX messages and camera definition files (see https://mavlink.io/en/services/camera_def.html ).")]
		DO_DIGICAM_CONFIGURE = 202,
		[Description("Control digital camera. This is a fallback message for systems that have not yet implemented PARAM_EXT_XXX messages and camera definition files (see https://mavlink.io/en/services/camera_def.html ).")]
		DO_DIGICAM_CONTROL = 203,
		[Description("Mission command to configure a camera or antenna mount")]
		DO_MOUNT_CONFIGURE = 204,
		[Description("Mission command to control a camera or antenna mount")]
		DO_MOUNT_CONTROL = 205,
		[Description("Mission command to set camera trigger distance for this flight. The camera is triggered each time this distance is exceeded. This command can also be used to set the shutter integration time for the camera.")]
		DO_SET_CAM_TRIGG_DIST = 206,
		[Description("           Enable the geofence.           This can be used in a mission or via the command protocol.           The persistence/lifetime of the setting is undefined.           Depending on flight stack implementation it may persist until superseded, or it may revert to a system default at the end of a mission.           Flight stacks typically reset the setting to system defaults on reboot. \t")]
		DO_FENCE_ENABLE = 207,
		[Description("Mission item/command to release a parachute or enable/disable auto release.")]
		DO_PARACHUTE = 208,
		[Description("Command to perform motor test.")]
		DO_MOTOR_TEST = 209,
		[Description("Change to/from inverted flight.")]
		DO_INVERTED_FLIGHT = 210,
		[Description("Mission command to operate a gripper.")]
		DO_GRIPPER = 211,
		[Description("Enable/disable autotune.")]
		DO_AUTOTUNE_ENABLE = 212,
		[Description("Sets a desired vehicle turn angle and speed change.")]
		SET_YAW_SPEED = 213,
		[Description("Mission command to set camera trigger interval for this flight. If triggering is enabled, the camera is triggered each time this interval expires. This command can also be used to set the shutter integration time for the camera.")]
		DO_SET_CAM_TRIGG_INTERVAL = 214,
		[Description("Mission command to control a camera or antenna mount, using a quaternion as reference.")]
		DO_MOUNT_CONTROL_QUAT = 220,
		[Description("set id of master controller")]
		DO_GUIDED_MASTER = 221,
		[Description("Set limits for external control")]
		DO_GUIDED_LIMITS = 222,
		[Description("Control vehicle engine. This is interpreted by the vehicles engine controller to change the target engine state. It is intended for vehicles with internal combustion engines")]
		DO_ENGINE_CONTROL = 223,
		[Description("           Set the mission item with sequence number seq as the current item and emit MISSION_CURRENT (whether or not the mission number changed).           If a mission is currently being executed, the system will continue to this new mission item on the shortest path, skipping any intermediate mission items. \t  Note that mission jump repeat counters are not reset unless param2 is set (see MAV_CMD_DO_JUMP param2).            This command may trigger a mission state-machine change on some systems: for example from MISSION_STATE_NOT_STARTED or MISSION_STATE_PAUSED to MISSION_STATE_ACTIVE.           If the system is in mission mode, on those systems this command might therefore start, restart or resume the mission.           If the system is not in mission mode this command must not trigger a switch to mission mode.            The mission may be 'reset' using param2.           Resetting sets jump counters to initial values (to reset counters without changing the current mission item set the param1 to `-1`).           Resetting also explicitly changes a mission state of MISSION_STATE_COMPLETE to MISSION_STATE_PAUSED or MISSION_STATE_ACTIVE, potentially allowing it to resume when it is (next) in a mission mode.  \t  The command will ACK with MAV_RESULT_FAILED if the sequence number is out of range (including if there is no mission item).         ")]
		DO_SET_MISSION_CURRENT = 224,
		[Description("NOP - This command is only used to mark the upper limit of the DO commands in the enumeration")]
		DO_LAST = 240,
		[Description("Trigger calibration. This command will be only accepted if in pre-flight mode. Except for Temperature Calibration, only one sensor should be set in a single message and all others should be zero.")]
		PREFLIGHT_CALIBRATION = 241,
		[Description("Set sensor offsets. This command will be only accepted if in pre-flight mode.")]
		PREFLIGHT_SET_SENSOR_OFFSETS = 242,
		[Description("Trigger UAVCAN configuration (actuator ID assignment and direction mapping). Note that this maps to the legacy UAVCAN v0 function UAVCAN_ENUMERATE, which is intended to be executed just once during initial vehicle configuration (it is not a normal pre-flight command and has been poorly named).")]
		PREFLIGHT_UAVCAN = 243,
		[Description("Request storage of different parameter values and logs. This command will be only accepted if in pre-flight mode.")]
		PREFLIGHT_STORAGE = 245,
		[Description("Request the reboot or shutdown of system components.")]
		PREFLIGHT_REBOOT_SHUTDOWN = 246,
		[Description("Override current mission with command to pause mission, pause mission and move to position, continue/resume mission. When param 1 indicates that the mission is paused (MAV_GOTO_DO_HOLD), param 2 defines whether it holds in place or moves to another position.")]
		OVERRIDE_GOTO = 252,
		[Description("Mission command to set a Camera Auto Mount Pivoting Oblique Survey (Replaces CAM_TRIGG_DIST for this purpose). The camera is triggered each time this distance is exceeded, then the mount moves to the next position. Params 4~6 set-up the angle limits and number of positions for oblique survey, where mount-enabled vehicles automatically roll the camera between shots to emulate an oblique camera setup (providing an increased HFOV). This command can also be used to set the shutter integration time for the camera.")]
		OBLIQUE_SURVEY = 260,
		[Description("Enable the specified standard MAVLink mode.           If the specified mode is not supported, the vehicle should ACK with MAV_RESULT_FAILED.           See https://mavlink.io/en/services/standard_modes.html         ")]
		DO_SET_STANDARD_MODE = 262,
		[Description("start running a mission")]
		MISSION_START = 300,
		[Description("Actuator testing command. This is similar to MAV_CMD_DO_MOTOR_TEST but operates on the level of output functions, i.e. it is possible to test Motor1 independent from which output it is configured on. Autopilots must NACK this command with MAV_RESULT_TEMPORARILY_REJECTED while armed.")]
		ACTUATOR_TEST = 310,
		[Description("Actuator configuration command.")]
		CONFIGURE_ACTUATOR = 311,
		[Description("Arms / Disarms a component")]
		COMPONENT_ARM_DISARM = 400,
		[Description("Instructs a target system to run pre-arm checks.           This allows preflight checks to be run on demand, which may be useful on systems that normally run them at low rate, or which do not trigger checks when the armable state might have changed.           This command should return MAV_RESULT_ACCEPTED if it will run the checks.           The results of the checks are usually then reported in SYS_STATUS messages (this is system-specific).           The command should return MAV_RESULT_TEMPORARILY_REJECTED if the system is already armed.         ")]
		RUN_PREARM_CHECKS = 401,
		[Description("Turns illuminators ON/OFF. An illuminator is a light source that is used for lighting up dark areas external to the system: e.g. a torch or searchlight (as opposed to a light source for illuminating the system itself, e.g. an indicator light).")]
		ILLUMINATOR_ON_OFF = 405,
		[Description("Configures illuminator settings. An illuminator is a light source that is used for lighting up dark areas external to the system: e.g. a torch or searchlight (as opposed to a light source for illuminating the system itself, e.g. an indicator light).")]
		DO_ILLUMINATOR_CONFIGURE = 406,
		[Description("Request the home position from the vehicle. \t  The vehicle will ACK the command and then emit the HOME_POSITION message.")]
		GET_HOME_POSITION = 410,
		[Description("Inject artificial failure for testing purposes. Note that autopilots should implement an additional protection before accepting this command such as a specific param setting.")]
		INJECT_FAILURE = 420,
		[Description("Starts receiver pairing.")]
		START_RX_PAIR = 500,
		[Description("           Request the interval between messages for a particular MAVLink message ID.           The receiver should ACK the command and then emit its response in a MESSAGE_INTERVAL message.         ")]
		GET_MESSAGE_INTERVAL = 510,
		[Description("Set the interval between messages for a particular MAVLink message ID. This interface replaces REQUEST_DATA_STREAM.")]
		SET_MESSAGE_INTERVAL = 511,
		[Description("Request the target system(s) emit a single instance of a specified message (i.e. a 'one-shot' version of MAV_CMD_SET_MESSAGE_INTERVAL).")]
		REQUEST_MESSAGE = 512,
		[Description("Request MAVLink protocol version compatibility. All receivers should ACK the command and then emit their capabilities in an PROTOCOL_VERSION message")]
		REQUEST_PROTOCOL_VERSION = 519,
		[Description("Request autopilot capabilities. The receiver should ACK the command and then emit its capabilities in an AUTOPILOT_VERSION message")]
		REQUEST_AUTOPILOT_CAPABILITIES = 520,
		[Description("Request camera information (CAMERA_INFORMATION).")]
		REQUEST_CAMERA_INFORMATION = 521,
		[Description("Request camera settings (CAMERA_SETTINGS).")]
		REQUEST_CAMERA_SETTINGS = 522,
		[Description("Request storage information (STORAGE_INFORMATION). Use the command's target_component to target a specific component's storage.")]
		REQUEST_STORAGE_INFORMATION = 525,
		[Description("Format a storage medium. Once format is complete, a STORAGE_INFORMATION message is sent. Use the command's target_component to target a specific component's storage.")]
		STORAGE_FORMAT = 526,
		[Description("Request camera capture status (CAMERA_CAPTURE_STATUS)")]
		REQUEST_CAMERA_CAPTURE_STATUS = 527,
		[Description("Request flight information (FLIGHT_INFORMATION)")]
		REQUEST_FLIGHT_INFORMATION = 528,
		[Description("Reset all camera settings to Factory Default")]
		RESET_CAMERA_SETTINGS = 529,
		[Description("Set camera running mode. Use NaN for reserved values. GCS will send a MAV_CMD_REQUEST_VIDEO_STREAM_STATUS command after a mode change if the camera supports video streaming.")]
		SET_CAMERA_MODE = 530,
		[Description("Set camera zoom. Camera must respond with a CAMERA_SETTINGS message (on success).")]
		SET_CAMERA_ZOOM = 531,
		[Description("Set camera focus. Camera must respond with a CAMERA_SETTINGS message (on success).")]
		SET_CAMERA_FOCUS = 532,
		[Description("Set that a particular storage is the preferred location for saving photos, videos, and/or other media (e.g. to set that an SD card is used for storing videos).           There can only be one preferred save location for each particular media type: setting a media usage flag will clear/reset that same flag if set on any other storage.           If no flag is set the system should use its default storage.           A target system can choose to always use default storage, in which case it should ACK the command with MAV_RESULT_UNSUPPORTED.           A target system can choose to not allow a particular storage to be set as preferred storage, in which case it should ACK the command with MAV_RESULT_DENIED.")]
		SET_STORAGE_USAGE = 533,
		[Description("Set camera source. Changes the camera's active sources on cameras with multiple image sensors.")]
		SET_CAMERA_SOURCE = 534,
		[Description("Tagged jump target. Can be jumped to with MAV_CMD_DO_JUMP_TAG.")]
		JUMP_TAG = 600,
		[Description("Jump to the matching tag in the mission list. Repeat this action for the specified number of times. A mission should contain a single matching tag for each jump. If this is not the case then a jump to a missing tag should complete the mission, and a jump where there are multiple matching tags should always select the one with the lowest mission sequence number.")]
		DO_JUMP_TAG = 601,
		[Description("Set gimbal manager pitch/yaw setpoints (low rate command). It is possible to set combinations of the values below. E.g. an angle as well as a desired angular rate can be used to get to this angle at a certain angular rate, or an angular rate only will result in continuous turning. NaN is to be used to signal unset. Note: only the gimbal manager will react to this command - it will be ignored by a gimbal device. Use GIMBAL_MANAGER_SET_PITCHYAW if you need to stream pitch/yaw setpoints at higher rate. ")]
		DO_GIMBAL_MANAGER_PITCHYAW = 1000,
		[Description("Gimbal configuration to set which sysid/compid is in primary and secondary control.")]
		DO_GIMBAL_MANAGER_CONFIGURE = 1001,
		[Description("Start image capture sequence. CAMERA_IMAGE_CAPTURED must be emitted after each capture.            Param1 (id) may be used to specify the target camera: 0: all cameras, 1 to 6: autopilot-connected cameras, 7-255: MAVLink camera component ID.           It is needed in order to target specific cameras connected to the autopilot, or specific sensors in a multi-sensor camera (neither of which have a distinct MAVLink component ID).           It is also needed to specify the target camera in missions.            When used in a mission, an autopilot should execute the MAV_CMD for a specified local camera (param1 = 1-6), or resend it as a command if it is intended for a MAVLink camera (param1 = 7 - 255), setting the command's target_component as the param1 value (and setting param1 in the command to zero).           If the param1 is 0 the autopilot should do both.            When sent in a command the target MAVLink address is set using target_component.           If addressed specifically to an autopilot: param1 should be used in the same way as it is for missions (though command should NACK with MAV_RESULT_DENIED if a specified local camera does not exist).           If addressed to a MAVLink camera, param 1 can be used to address all cameras (0), or to separately address 1 to 7 individual sensors. Other values should be NACKed with MAV_RESULT_DENIED.           If the command is broadcast (target_component is 0) then param 1 should be set to 0 (any other value should be NACKED with MAV_RESULT_DENIED). An autopilot would trigger any local cameras and forward the command to all channels.         ")]
		IMAGE_START_CAPTURE = 2000,
		[Description("Stop image capture sequence.            Param1 (id) may be used to specify the target camera: 0: all cameras, 1 to 6: autopilot-connected cameras, 7-255: MAVLink camera component ID.           It is needed in order to target specific cameras connected to the autopilot, or specific sensors in a multi-sensor camera (neither of which have a distinct MAVLink component ID).           It is also needed to specify the target camera in missions.            When used in a mission, an autopilot should execute the MAV_CMD for a specified local camera (param1 = 1-6), or resend it as a command if it is intended for a MAVLink camera (param1 = 7 - 255), setting the command's target_component as the param1 value (and setting param1 in the command to zero).           If the param1 is 0 the autopilot should do both.            When sent in a command the target MAVLink address is set using target_component.           If addressed specifically to an autopilot: param1 should be used in the same way as it is for missions (though command should NACK with MAV_RESULT_DENIED if a specified local camera does not exist).           If addressed to a MAVLink camera, param1 can be used to address all cameras (0), or to separately address 1 to 7 individual sensors. Other values should be NACKed with MAV_RESULT_DENIED.           If the command is broadcast (target_component is 0) then param 1 should be set to 0 (any other value should be NACKED with MAV_RESULT_DENIED). An autopilot would trigger any local cameras and forward the command to all channels.         ")]
		IMAGE_STOP_CAPTURE = 2001,
		[Description("Re-request a CAMERA_IMAGE_CAPTURED message.")]
		REQUEST_CAMERA_IMAGE_CAPTURE = 2002,
		[Description("Enable or disable on-board camera triggering system.")]
		DO_TRIGGER_CONTROL = 2003,
		[Description("If the camera supports point visual tracking (CAMERA_CAP_FLAGS_HAS_TRACKING_POINT is set), this command allows to initiate the tracking.")]
		CAMERA_TRACK_POINT = 2004,
		[Description("If the camera supports rectangle visual tracking (CAMERA_CAP_FLAGS_HAS_TRACKING_RECTANGLE is set), this command allows to initiate the tracking.")]
		CAMERA_TRACK_RECTANGLE = 2005,
		[Description("Stops ongoing tracking.")]
		CAMERA_STOP_TRACKING = 2010,
		[Description("Starts video capture (recording).")]
		VIDEO_START_CAPTURE = 2500,
		[Description("Stop the current video capture (recording).")]
		VIDEO_STOP_CAPTURE = 2501,
		[Description("Start video streaming")]
		VIDEO_START_STREAMING = 2502,
		[Description("Stop the given video stream")]
		VIDEO_STOP_STREAMING = 2503,
		[Description("Request video stream information (VIDEO_STREAM_INFORMATION)")]
		REQUEST_VIDEO_STREAM_INFORMATION = 2504,
		[Description("Request video stream status (VIDEO_STREAM_STATUS)")]
		REQUEST_VIDEO_STREAM_STATUS = 2505,
		[Description("Request to start streaming logging data over MAVLink (see also LOGGING_DATA message)")]
		LOGGING_START = 2510,
		[Description("Request to stop streaming log data over MAVLink")]
		LOGGING_STOP = 2511,
		[Description("")]
		AIRFRAME_CONFIGURATION = 2520,
		[Description("Request to start/stop transmitting over the high latency telemetry")]
		CONTROL_HIGH_LATENCY = 2600,
		[Description("Create a panorama at the current position")]
		PANORAMA_CREATE = 2800,
		[Description("Request VTOL transition")]
		DO_VTOL_TRANSITION = 3000,
		[Description("Request authorization to arm the vehicle to a external entity, the arm authorizer is responsible to request all data that is needs from the vehicle before authorize or deny the request. \t\tIf approved the COMMAND_ACK message progress field should be set with period of time that this authorization is valid in seconds. \t\tIf the authorization is denied COMMAND_ACK.result_param2 should be set with one of the reasons in ARM_AUTH_DENIED_REASON.         ")]
		ARM_AUTHORIZATION_REQUEST = 3001,
		[Description("This command sets the submode to standard guided when vehicle is in guided mode. The vehicle holds position and altitude and the user can input the desired velocities along all three axes.                   ")]
		SET_GUIDED_SUBMODE_STANDARD = 4000,
		[Description("This command sets submode circle when vehicle is in guided mode. Vehicle flies along a circle facing the center of the circle. The user can input the velocity along the circle and change the radius. If no input is given the vehicle will hold position.                   ")]
		SET_GUIDED_SUBMODE_CIRCLE = 4001,
		[Description("Delay mission state machine until gate has been reached.")]
		CONDITION_GATE = 4501,
		[Description("Fence return point (there can only be one such point in a geofence definition). If rally points are supported they should be used instead.")]
		FENCE_RETURN_POINT = 5000,
		[Description("Fence vertex for an inclusion polygon (the polygon must not be self-intersecting). The vehicle must stay within this area. Minimum of 3 vertices required.           The vertices for a polygon must be sent sequentially, each with param1 set to the total number of vertices in the polygon.         ")]
		FENCE_POLYGON_VERTEX_INCLUSION = 5001,
		[Description("Fence vertex for an exclusion polygon (the polygon must not be self-intersecting). The vehicle must stay outside this area. Minimum of 3 vertices required.           The vertices for a polygon must be sent sequentially, each with param1 set to the total number of vertices in the polygon.         ")]
		FENCE_POLYGON_VERTEX_EXCLUSION = 5002,
		[Description("Circular fence area. The vehicle must stay inside this area.         ")]
		FENCE_CIRCLE_INCLUSION = 5003,
		[Description("Circular fence area. The vehicle must stay outside this area.         ")]
		FENCE_CIRCLE_EXCLUSION = 5004,
		[Description("Rally point. You can have multiple rally points defined.         ")]
		RALLY_POINT = 5100,
		[Description("Commands the vehicle to respond with a sequence of messages UAVCAN_NODE_INFO, one message per every UAVCAN node that is online. Note that some of the response messages can be lost, which the receiver can detect easily by checking whether every received UAVCAN_NODE_STATUS has a matching message UAVCAN_NODE_INFO received earlier; if not, this command should be sent again in order to request re-transmission of the node information messages.")]
		UAVCAN_GET_NODE_INFO = 5200,
		[Description("Change state of safety switch.")]
		DO_SET_SAFETY_SWITCH_STATE = 5300,
		[Description("Trigger the start of an ADSB-out IDENT. This should only be used when requested to do so by an Air Traffic Controller in controlled airspace. This starts the IDENT which is then typically held for 18 seconds by the hardware per the Mode A, C, and S transponder spec.")]
		DO_ADSB_OUT_IDENT = 10001,
		[Description("Deploy payload on a Lat / Lon / Alt position. This includes the navigation to reach the required release position and velocity.")]
		PAYLOAD_PREPARE_DEPLOY = 30001,
		[Description("Control the payload deployment.")]
		PAYLOAD_CONTROL_DEPLOY = 30002,
		[Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
		WAYPOINT_USER_1 = 31000,
		[Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
		WAYPOINT_USER_2 = 31001,
		[Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
		WAYPOINT_USER_3 = 31002,
		[Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
		WAYPOINT_USER_4 = 31003,
		[Description("User defined waypoint item. Ground Station will show the Vehicle as flying through this item.")]
		WAYPOINT_USER_5 = 31004,
		[Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
		SPATIAL_USER_1 = 31005,
		[Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
		SPATIAL_USER_2 = 31006,
		[Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
		SPATIAL_USER_3 = 31007,
		[Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
		SPATIAL_USER_4 = 31008,
		[Description("User defined spatial item. Ground Station will not show the Vehicle as flying through this item. Example: ROI item.")]
		SPATIAL_USER_5 = 31009,
		[Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
		USER_1 = 31010,
		[Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
		USER_2 = 31011,
		[Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
		USER_3 = 31012,
		[Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
		USER_4 = 31013,
		[Description("User defined command. Ground Station will not show the Vehicle as flying through this item. Example: MAV_CMD_DO_SET_PARAMETER item.")]
		USER_5 = 31014,
		[Description("Request forwarding of CAN packets from the given CAN bus to this component. CAN Frames are sent using CAN_FRAME and CANFD_FRAME messages")]
		CAN_FORWARD = 32000,
		[Description("Magnetometer calibration based on provided known yaw. This allows for fast calibration using WMM field tables in the vehicle, given only the known yaw of the vehicle. If Latitude and longitude are both zero then use the current vehicle location.")]
		FIXED_MAG_CAL_YAW = 42006,
		[Description("Command to operate winch.")]
		DO_WINCH = 42600,
		[Description("Provide an external position estimate for use when dead-reckoning. This is meant to be used for occasional position resets that may be provided by a external system such as a remote pilot using landmarks over a video link.")]
		EXTERNAL_POSITION_ESTIMATE = 43003
	}

	public enum MAV_DATA_STREAM
	{
		[Description("Enable all data streams")]
		ALL = 0,
		[Description("Enable IMU_RAW, GPS_RAW, GPS_STATUS packets.")]
		RAW_SENSORS = 1,
		[Description("Enable GPS_STATUS, CONTROL_STATUS, AUX_STATUS")]
		EXTENDED_STATUS = 2,
		[Description("Enable RC_CHANNELS_SCALED, RC_CHANNELS_RAW, SERVO_OUTPUT_RAW")]
		RC_CHANNELS = 3,
		[Description("Enable ATTITUDE_CONTROLLER_OUTPUT, POSITION_CONTROLLER_OUTPUT, NAV_CONTROLLER_OUTPUT.")]
		RAW_CONTROLLER = 4,
		[Description("Enable LOCAL_POSITION, GLOBAL_POSITION_INT messages.")]
		POSITION = 6,
		[Description("Dependent on the autopilot")]
		EXTRA1 = 10,
		[Description("Dependent on the autopilot")]
		EXTRA2 = 11,
		[Description("Dependent on the autopilot")]
		EXTRA3 = 12
	}

	public enum MAV_ROI
	{
		[Description("No region of interest.")]
		NONE,
		[Description("Point toward next waypoint, with optional pitch/roll/yaw offset.")]
		WPNEXT,
		[Description("Point toward given waypoint.")]
		WPINDEX,
		[Description("Point toward fixed location.")]
		LOCATION,
		[Description("Point toward of given id.")]
		TARGET
	}

	public enum MAV_PARAM_TYPE : byte
	{
		[Description("8-bit unsigned integer")]
		UINT8 = 1,
		[Description("8-bit signed integer")]
		INT8,
		[Description("16-bit unsigned integer")]
		UINT16,
		[Description("16-bit signed integer")]
		INT16,
		[Description("32-bit unsigned integer")]
		UINT32,
		[Description("32-bit signed integer")]
		INT32,
		[Description("64-bit unsigned integer")]
		UINT64,
		[Description("64-bit signed integer")]
		INT64,
		[Description("32-bit floating-point")]
		REAL32,
		[Description("64-bit floating-point")]
		REAL64
	}

	public enum MAV_PARAM_EXT_TYPE : byte
	{
		[Description("8-bit unsigned integer")]
		UINT8 = 1,
		[Description("8-bit signed integer")]
		INT8,
		[Description("16-bit unsigned integer")]
		UINT16,
		[Description("16-bit signed integer")]
		INT16,
		[Description("32-bit unsigned integer")]
		UINT32,
		[Description("32-bit signed integer")]
		INT32,
		[Description("64-bit unsigned integer")]
		UINT64,
		[Description("64-bit signed integer")]
		INT64,
		[Description("32-bit floating-point")]
		REAL32,
		[Description("64-bit floating-point")]
		REAL64,
		[Description("Custom Type")]
		CUSTOM
	}

	public enum MAV_RESULT : byte
	{
		[Description("Command is valid (is supported and has valid parameters), and was executed.")]
		ACCEPTED,
		[Description("Command is valid, but cannot be executed at this time. This is used to indicate a problem that should be fixed just by waiting (e.g. a state machine is busy, can't arm because have not got GPS lock, etc.). Retrying later should work.")]
		TEMPORARILY_REJECTED,
		[Description("Command is invalid (is supported but has invalid parameters). Retrying same command and parameters will not work.")]
		DENIED,
		[Description("Command is not supported (unknown).")]
		UNSUPPORTED,
		[Description("Command is valid, but execution has failed. This is used to indicate any non-temporary or unexpected problem, i.e. any problem that must be fixed before the command can succeed/be retried. For example, attempting to write a file when out of memory, attempting to arm when sensors are not calibrated, etc.")]
		FAILED,
		[Description("Command is valid and is being executed. This will be followed by further progress updates, i.e. the component may send further COMMAND_ACK messages with result MAV_RESULT_IN_PROGRESS (at a rate decided by the implementation), and must terminate by sending a COMMAND_ACK message with final result of the operation. The COMMAND_ACK.progress field can be used to indicate the progress of the operation.")]
		IN_PROGRESS,
		[Description("Command has been cancelled (as a result of receiving a COMMAND_CANCEL message).")]
		CANCELLED,
		[Description("Command is only accepted when sent as a COMMAND_LONG.")]
		COMMAND_LONG_ONLY,
		[Description("Command is only accepted when sent as a COMMAND_INT.")]
		COMMAND_INT_ONLY,
		[Description("Command is invalid because a frame is required and the specified frame is not supported.")]
		COMMAND_UNSUPPORTED_MAV_FRAME
	}

	public enum MAV_MISSION_RESULT : byte
	{
		[Description("mission accepted OK")]
		MAV_MISSION_ACCEPTED,
		[Description("Generic error / not accepting mission commands at all right now.")]
		MAV_MISSION_ERROR,
		[Description("Coordinate frame is not supported.")]
		MAV_MISSION_UNSUPPORTED_FRAME,
		[Description("Command is not supported.")]
		MAV_MISSION_UNSUPPORTED,
		[Description("Mission items exceed storage space.")]
		MAV_MISSION_NO_SPACE,
		[Description("One of the parameters has an invalid value.")]
		MAV_MISSION_INVALID,
		[Description("param1 has an invalid value.")]
		MAV_MISSION_INVALID_PARAM1,
		[Description("param2 has an invalid value.")]
		MAV_MISSION_INVALID_PARAM2,
		[Description("param3 has an invalid value.")]
		MAV_MISSION_INVALID_PARAM3,
		[Description("param4 has an invalid value.")]
		MAV_MISSION_INVALID_PARAM4,
		[Description("x / param5 has an invalid value.")]
		MAV_MISSION_INVALID_PARAM5_X,
		[Description("y / param6 has an invalid value.")]
		MAV_MISSION_INVALID_PARAM6_Y,
		[Description("z / param7 has an invalid value.")]
		MAV_MISSION_INVALID_PARAM7,
		[Description("Mission item received out of sequence")]
		MAV_MISSION_INVALID_SEQUENCE,
		[Description("Not accepting any mission commands from this communication partner.")]
		MAV_MISSION_DENIED,
		[Description("Current mission operation cancelled (e.g. mission upload, mission download).")]
		MAV_MISSION_OPERATION_CANCELLED
	}

	public enum MAV_SEVERITY : byte
	{
		[Description("System is unusable. This is a 'panic' condition.")]
		EMERGENCY,
		[Description("Action should be taken immediately. Indicates error in non-critical systems.")]
		ALERT,
		[Description("Action must be taken immediately. Indicates failure in a primary system.")]
		CRITICAL,
		[Description("Indicates an error in secondary/redundant systems.")]
		ERROR,
		[Description("Indicates about a possible future error if this is not resolved within a given timeframe. Example would be a low battery warning.")]
		WARNING,
		[Description("An unusual event has occurred, though not an error condition. This should be investigated for the root cause.")]
		NOTICE,
		[Description("Normal operational messages. Useful for logging. No action is required for these messages.")]
		INFO,
		[Description("Useful non-operational messages that can assist in debugging. These should not occur during normal operation.")]
		DEBUG
	}

	[Flags]
	public enum MAV_POWER_STATUS : ushort
	{
		[Description("main brick power supply valid")]
		BRICK_VALID = 1,
		[Description("main servo power supply valid for FMU")]
		SERVO_VALID = 2,
		[Description("USB power is connected")]
		USB_CONNECTED = 4,
		[Description("peripheral supply is in over-current state")]
		PERIPH_OVERCURRENT = 8,
		[Description("hi-power peripheral supply is in over-current state")]
		PERIPH_HIPOWER_OVERCURRENT = 0x10,
		[Description("Power status has changed since boot")]
		CHANGED = 0x20
	}

	public enum SERIAL_CONTROL_DEV : byte
	{
		[Description("First telemetry port")]
		TELEM1 = 0,
		[Description("Second telemetry port")]
		TELEM2 = 1,
		[Description("First GPS port")]
		GPS1 = 2,
		[Description("Second GPS port")]
		GPS2 = 3,
		[Description("system shell")]
		SHELL = 10,
		[Description("SERIAL0")]
		SERIAL_CONTROL_SERIAL0 = 100,
		[Description("SERIAL1")]
		SERIAL_CONTROL_SERIAL1 = 101,
		[Description("SERIAL2")]
		SERIAL_CONTROL_SERIAL2 = 102,
		[Description("SERIAL3")]
		SERIAL_CONTROL_SERIAL3 = 103,
		[Description("SERIAL4")]
		SERIAL_CONTROL_SERIAL4 = 104,
		[Description("SERIAL5")]
		SERIAL_CONTROL_SERIAL5 = 105,
		[Description("SERIAL6")]
		SERIAL_CONTROL_SERIAL6 = 106,
		[Description("SERIAL7")]
		SERIAL_CONTROL_SERIAL7 = 107,
		[Description("SERIAL8")]
		SERIAL_CONTROL_SERIAL8 = 108,
		[Description("SERIAL9")]
		SERIAL_CONTROL_SERIAL9 = 109
	}

	[Flags]
	public enum SERIAL_CONTROL_FLAG : byte
	{
		[Description("Set if this is a reply")]
		REPLY = 1,
		[Description("Set if the sender wants the receiver to send a response as another SERIAL_CONTROL message")]
		RESPOND = 2,
		[Description("Set if access to the serial port should be removed from whatever driver is currently using it, giving exclusive access to the SERIAL_CONTROL protocol. The port can be handed back by sending a request without this flag set")]
		EXCLUSIVE = 4,
		[Description("Block on writes to the serial port")]
		BLOCKING = 8,
		[Description("Send multiple replies until port is drained")]
		MULTI = 0x10
	}

	public enum MAV_DISTANCE_SENSOR : byte
	{
		[Description("Laser rangefinder, e.g. LightWare SF02/F or PulsedLight units")]
		LASER,
		[Description("Ultrasound rangefinder, e.g. MaxBotix units")]
		ULTRASOUND,
		[Description("Infrared rangefinder, e.g. Sharp units")]
		INFRARED,
		[Description("Radar type, e.g. uLanding units")]
		RADAR,
		[Description("Broken or unknown type, e.g. analog units")]
		UNKNOWN
	}

	public enum MAV_SENSOR_ORIENTATION : byte
	{
		[Description("Roll: 0, Pitch: 0, Yaw: 0")]
		MAV_SENSOR_ROTATION_NONE = 0,
		[Description("Roll: 0, Pitch: 0, Yaw: 45")]
		MAV_SENSOR_ROTATION_YAW_45 = 1,
		[Description("Roll: 0, Pitch: 0, Yaw: 90")]
		MAV_SENSOR_ROTATION_YAW_90 = 2,
		[Description("Roll: 0, Pitch: 0, Yaw: 135")]
		MAV_SENSOR_ROTATION_YAW_135 = 3,
		[Description("Roll: 0, Pitch: 0, Yaw: 180")]
		MAV_SENSOR_ROTATION_YAW_180 = 4,
		[Description("Roll: 0, Pitch: 0, Yaw: 225")]
		MAV_SENSOR_ROTATION_YAW_225 = 5,
		[Description("Roll: 0, Pitch: 0, Yaw: 270")]
		MAV_SENSOR_ROTATION_YAW_270 = 6,
		[Description("Roll: 0, Pitch: 0, Yaw: 315")]
		MAV_SENSOR_ROTATION_YAW_315 = 7,
		[Description("Roll: 180, Pitch: 0, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_180 = 8,
		[Description("Roll: 180, Pitch: 0, Yaw: 45")]
		MAV_SENSOR_ROTATION_ROLL_180_YAW_45 = 9,
		[Description("Roll: 180, Pitch: 0, Yaw: 90")]
		MAV_SENSOR_ROTATION_ROLL_180_YAW_90 = 10,
		[Description("Roll: 180, Pitch: 0, Yaw: 135")]
		MAV_SENSOR_ROTATION_ROLL_180_YAW_135 = 11,
		[Description("Roll: 0, Pitch: 180, Yaw: 0")]
		MAV_SENSOR_ROTATION_PITCH_180 = 12,
		[Description("Roll: 180, Pitch: 0, Yaw: 225")]
		MAV_SENSOR_ROTATION_ROLL_180_YAW_225 = 13,
		[Description("Roll: 180, Pitch: 0, Yaw: 270")]
		MAV_SENSOR_ROTATION_ROLL_180_YAW_270 = 14,
		[Description("Roll: 180, Pitch: 0, Yaw: 315")]
		MAV_SENSOR_ROTATION_ROLL_180_YAW_315 = 15,
		[Description("Roll: 90, Pitch: 0, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_90 = 16,
		[Description("Roll: 90, Pitch: 0, Yaw: 45")]
		MAV_SENSOR_ROTATION_ROLL_90_YAW_45 = 17,
		[Description("Roll: 90, Pitch: 0, Yaw: 90")]
		MAV_SENSOR_ROTATION_ROLL_90_YAW_90 = 18,
		[Description("Roll: 90, Pitch: 0, Yaw: 135")]
		MAV_SENSOR_ROTATION_ROLL_90_YAW_135 = 19,
		[Description("Roll: 270, Pitch: 0, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_270 = 20,
		[Description("Roll: 270, Pitch: 0, Yaw: 45")]
		MAV_SENSOR_ROTATION_ROLL_270_YAW_45 = 21,
		[Description("Roll: 270, Pitch: 0, Yaw: 90")]
		MAV_SENSOR_ROTATION_ROLL_270_YAW_90 = 22,
		[Description("Roll: 270, Pitch: 0, Yaw: 135")]
		MAV_SENSOR_ROTATION_ROLL_270_YAW_135 = 23,
		[Description("Roll: 0, Pitch: 90, Yaw: 0")]
		MAV_SENSOR_ROTATION_PITCH_90 = 24,
		[Description("Roll: 0, Pitch: 270, Yaw: 0")]
		MAV_SENSOR_ROTATION_PITCH_270 = 25,
		[Description("Roll: 0, Pitch: 180, Yaw: 90")]
		MAV_SENSOR_ROTATION_PITCH_180_YAW_90 = 26,
		[Description("Roll: 0, Pitch: 180, Yaw: 270")]
		MAV_SENSOR_ROTATION_PITCH_180_YAW_270 = 27,
		[Description("Roll: 90, Pitch: 90, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_90_PITCH_90 = 28,
		[Description("Roll: 180, Pitch: 90, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_180_PITCH_90 = 29,
		[Description("Roll: 270, Pitch: 90, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_270_PITCH_90 = 30,
		[Description("Roll: 90, Pitch: 180, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_90_PITCH_180 = 31,
		[Description("Roll: 270, Pitch: 180, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_270_PITCH_180 = 32,
		[Description("Roll: 90, Pitch: 270, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_90_PITCH_270 = 33,
		[Description("Roll: 180, Pitch: 270, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_180_PITCH_270 = 34,
		[Description("Roll: 270, Pitch: 270, Yaw: 0")]
		MAV_SENSOR_ROTATION_ROLL_270_PITCH_270 = 35,
		[Description("Roll: 90, Pitch: 180, Yaw: 90")]
		MAV_SENSOR_ROTATION_ROLL_90_PITCH_180_YAW_90 = 36,
		[Description("Roll: 90, Pitch: 0, Yaw: 270")]
		MAV_SENSOR_ROTATION_ROLL_90_YAW_270 = 37,
		[Description("Roll: 90, Pitch: 68, Yaw: 293")]
		MAV_SENSOR_ROTATION_ROLL_90_PITCH_68_YAW_293 = 38,
		[Description("Pitch: 315")]
		MAV_SENSOR_ROTATION_PITCH_315 = 39,
		[Description("Roll: 90, Pitch: 315")]
		MAV_SENSOR_ROTATION_ROLL_90_PITCH_315 = 40,
		[Description("Custom orientation")]
		MAV_SENSOR_ROTATION_CUSTOM = 100
	}

	public enum MAV_MISSION_TYPE : byte
	{
		[Description("Items are mission commands for main mission.")]
		MISSION = 0,
		[Description("Specifies GeoFence area(s). Items are MAV_CMD_NAV_FENCE_ GeoFence items.")]
		FENCE = 1,
		[Description("Specifies the rally points for the vehicle. Rally points are alternative RTL points. Items are MAV_CMD_NAV_RALLY_POINT rally point items.")]
		RALLY = 2,
		[Description("Only used in MISSION_CLEAR_ALL to clear all mission types.")]
		ALL = byte.MaxValue
	}

	public enum MAV_ESTIMATOR_TYPE : byte
	{
		[Description("Unknown type of the estimator.")]
		UNKNOWN,
		[Description("This is a naive estimator without any real covariance feedback.")]
		NAIVE,
		[Description("Computer vision based estimate. Might be up to scale.")]
		VISION,
		[Description("Visual-inertial estimate.")]
		VIO,
		[Description("Plain GPS estimate.")]
		GPS,
		[Description("Estimator integrating GPS and inertial sensing.")]
		GPS_INS,
		[Description("Estimate from external motion capturing system.")]
		MOCAP,
		[Description("Estimator based on lidar sensor input.")]
		LIDAR,
		[Description("Estimator on autopilot.")]
		AUTOPILOT
	}

	public enum MAV_BATTERY_TYPE : byte
	{
		[Description("Not specified.")]
		UNKNOWN,
		[Description("Lithium polymer battery")]
		LIPO,
		[Description("Lithium-iron-phosphate battery")]
		LIFE,
		[Description("Lithium-ION battery")]
		LION,
		[Description("Nickel metal hydride battery")]
		NIMH
	}

	public enum MAV_BATTERY_FUNCTION : byte
	{
		[Description("Battery function is unknown")]
		UNKNOWN,
		[Description("Battery supports all flight systems")]
		ALL,
		[Description("Battery for the propulsion system")]
		PROPULSION,
		[Description("Avionics battery")]
		AVIONICS,
		[Description("Payload battery")]
		PAYLOAD
	}

	public enum MAV_BATTERY_CHARGE_STATE : byte
	{
		[Description("Low battery state is not provided")]
		UNDEFINED,
		[Description("Battery is not in low state. Normal operation.")]
		OK,
		[Description("Battery state is low, warn and monitor close.")]
		LOW,
		[Description("Battery state is critical, return or abort immediately.")]
		CRITICAL,
		[Description("Battery state is too low for ordinary abort sequence. Perform fastest possible emergency stop to prevent damage.")]
		EMERGENCY,
		[Description("Battery failed, damage unavoidable. Possible causes (faults) are listed in MAV_BATTERY_FAULT.")]
		FAILED,
		[Description("Battery is diagnosed to be defective or an error occurred, usage is discouraged / prohibited. Possible causes (faults) are listed in MAV_BATTERY_FAULT.")]
		UNHEALTHY,
		[Description("Battery is charging.")]
		CHARGING
	}

	public enum MAV_BATTERY_MODE : byte
	{
		[Description("Battery mode not supported/unknown battery mode/normal operation.")]
		UNKNOWN,
		[Description("Battery is auto discharging (towards storage level).")]
		AUTO_DISCHARGING,
		[Description("Battery in hot-swap mode (current limited to prevent spikes that might damage sensitive electrical circuits).")]
		HOT_SWAP
	}

	[Flags]
	public enum MAV_BATTERY_FAULT : uint
	{
		[Description("Battery has deep discharged.")]
		DEEP_DISCHARGE = 1u,
		[Description("Voltage spikes.")]
		SPIKES = 2u,
		[Description("One or more cells have failed. Battery should also report MAV_BATTERY_CHARGE_STATE_FAILE (and should not be used).")]
		CELL_FAIL = 4u,
		[Description("Over-current fault.")]
		OVER_CURRENT = 8u,
		[Description("Over-temperature fault.")]
		OVER_TEMPERATURE = 0x10u,
		[Description("Under-temperature fault.")]
		UNDER_TEMPERATURE = 0x20u,
		[Description("Vehicle voltage is not compatible with this battery (batteries on same power rail should have similar voltage).")]
		INCOMPATIBLE_VOLTAGE = 0x40u,
		[Description("Battery firmware is not compatible with current autopilot firmware.")]
		INCOMPATIBLE_FIRMWARE = 0x80u,
		[Description("Battery is not compatible due to cell configuration (e.g. 5s1p when vehicle requires 6s).")]
		BATTERY_FAULT_INCOMPATIBLE_CELLS_CONFIGURATION = 0x100u
	}

	public enum MAV_FUEL_TYPE : uint
	{
		[Description("Not specified. Fuel levels are normalized (i.e. maximum is 1, and other levels are relative to 1).")]
		UNKNOWN,
		[Description("A generic liquid fuel. Fuel levels are in millilitres (ml). Fuel rates are in millilitres/second.")]
		LIQUID,
		[Description("A gas tank. Fuel levels are in kilo-Pascal (kPa), and flow rates are in milliliters per second (ml/s).")]
		GAS
	}

	public enum MAV_GENERATOR_STATUS_FLAG : ulong
	{
		[Description("Generator is off.")]
		OFF = 1uL,
		[Description("Generator is ready to start generating power.")]
		READY = 2uL,
		[Description("Generator is generating power.")]
		GENERATING = 4uL,
		[Description("Generator is charging the batteries (generating enough power to charge and provide the load).")]
		CHARGING = 8uL,
		[Description("Generator is operating at a reduced maximum power.")]
		REDUCED_POWER = 0x10uL,
		[Description("Generator is providing the maximum output.")]
		MAXPOWER = 0x20uL,
		[Description("Generator is near the maximum operating temperature, cooling is insufficient.")]
		OVERTEMP_WARNING = 0x40uL,
		[Description("Generator hit the maximum operating temperature and shutdown.")]
		OVERTEMP_FAULT = 0x80uL,
		[Description("Power electronics are near the maximum operating temperature, cooling is insufficient.")]
		ELECTRONICS_OVERTEMP_WARNING = 0x100uL,
		[Description("Power electronics hit the maximum operating temperature and shutdown.")]
		ELECTRONICS_OVERTEMP_FAULT = 0x200uL,
		[Description("Power electronics experienced a fault and shutdown.")]
		ELECTRONICS_FAULT = 0x400uL,
		[Description("The power source supplying the generator failed e.g. mechanical generator stopped, tether is no longer providing power, solar cell is in shade, hydrogen reaction no longer happening.")]
		POWERSOURCE_FAULT = 0x800uL,
		[Description("Generator controller having communication problems.")]
		COMMUNICATION_WARNING = 0x1000uL,
		[Description("Power electronic or generator cooling system error.")]
		COOLING_WARNING = 0x2000uL,
		[Description("Generator controller power rail experienced a fault.")]
		POWER_RAIL_FAULT = 0x4000uL,
		[Description("Generator controller exceeded the overcurrent threshold and shutdown to prevent damage.")]
		OVERCURRENT_FAULT = 0x8000uL,
		[Description("Generator controller detected a high current going into the batteries and shutdown to prevent battery damage.")]
		BATTERY_OVERCHARGE_CURRENT_FAULT = 0x10000uL,
		[Description("Generator controller exceeded it's overvoltage threshold and shutdown to prevent it exceeding the voltage rating.")]
		OVERVOLTAGE_FAULT = 0x20000uL,
		[Description("Batteries are under voltage (generator will not start).")]
		BATTERY_UNDERVOLT_FAULT = 0x40000uL,
		[Description("Generator start is inhibited by e.g. a safety switch.")]
		START_INHIBITED = 0x80000uL,
		[Description("Generator requires maintenance.")]
		MAINTENANCE_REQUIRED = 0x100000uL,
		[Description("Generator is not ready to generate yet.")]
		WARMING_UP = 0x200000uL,
		[Description("Generator is idle.")]
		IDLE = 0x400000uL
	}

	public enum MAV_VTOL_STATE : byte
	{
		[Description("MAV is not configured as VTOL")]
		UNDEFINED,
		[Description("VTOL is in transition from multicopter to fixed-wing")]
		TRANSITION_TO_FW,
		[Description("VTOL is in transition from fixed-wing to multicopter")]
		TRANSITION_TO_MC,
		[Description("VTOL is in multicopter state")]
		MC,
		[Description("VTOL is in fixed-wing state")]
		FW
	}

	public enum MAV_LANDED_STATE : byte
	{
		[Description("MAV landed state is unknown")]
		UNDEFINED,
		[Description("MAV is landed (on ground)")]
		ON_GROUND,
		[Description("MAV is in air")]
		IN_AIR,
		[Description("MAV currently taking off")]
		TAKEOFF,
		[Description("MAV currently landing")]
		LANDING
	}

	public enum ADSB_ALTITUDE_TYPE : byte
	{
		[Description("Altitude reported from a Baro source using QNH reference")]
		PRESSURE_QNH,
		[Description("Altitude reported from a GNSS source")]
		GEOMETRIC
	}

	public enum ADSB_EMITTER_TYPE : byte
	{
		[Description("")]
		NO_INFO,
		[Description("")]
		LIGHT,
		[Description("")]
		SMALL,
		[Description("")]
		LARGE,
		[Description("")]
		HIGH_VORTEX_LARGE,
		[Description("")]
		HEAVY,
		[Description("")]
		HIGHLY_MANUV,
		[Description("")]
		ROTOCRAFT,
		[Description("")]
		UNASSIGNED,
		[Description("")]
		GLIDER,
		[Description("")]
		LIGHTER_AIR,
		[Description("")]
		PARACHUTE,
		[Description("")]
		ULTRA_LIGHT,
		[Description("")]
		UNASSIGNED2,
		[Description("")]
		UAV,
		[Description("")]
		SPACE,
		[Description("")]
		UNASSGINED3,
		[Description("")]
		EMERGENCY_SURFACE,
		[Description("")]
		SERVICE_SURFACE,
		[Description("")]
		POINT_OBSTACLE
	}

	[Flags]
	public enum ADSB_FLAGS : ushort
	{
		[Description("")]
		VALID_COORDS = 1,
		[Description("")]
		VALID_ALTITUDE = 2,
		[Description("")]
		VALID_HEADING = 4,
		[Description("")]
		VALID_VELOCITY = 8,
		[Description("")]
		VALID_CALLSIGN = 0x10,
		[Description("")]
		VALID_SQUAWK = 0x20,
		[Description("")]
		SIMULATED = 0x40,
		[Description("")]
		VERTICAL_VELOCITY_VALID = 0x80,
		[Description("")]
		BARO_VALID = 0x100,
		[Description("")]
		SOURCE_UAT = 0x8000
	}

	[Flags]
	public enum MAV_DO_REPOSITION_FLAGS
	{
		[Description("The aircraft should immediately transition into guided. This should not be set for follow me applications")]
		CHANGE_MODE = 1,
		[Description("Yaw relative to the vehicle current heading (if not set, relative to North).")]
		RELATIVE_YAW = 2
	}

	public enum SPEED_TYPE
	{
		[Description("Airspeed")]
		AIRSPEED,
		[Description("Groundspeed")]
		GROUNDSPEED,
		[Description("Climb speed")]
		CLIMB_SPEED,
		[Description("Descent speed")]
		DESCENT_SPEED
	}

	[Flags]
	public enum ESTIMATOR_STATUS_FLAGS : ushort
	{
		[Description("True if the attitude estimate is good")]
		ESTIMATOR_ATTITUDE = 1,
		[Description("True if the horizontal velocity estimate is good")]
		ESTIMATOR_VELOCITY_HORIZ = 2,
		[Description("True if the  vertical velocity estimate is good")]
		ESTIMATOR_VELOCITY_VERT = 4,
		[Description("True if the horizontal position (relative) estimate is good")]
		ESTIMATOR_POS_HORIZ_REL = 8,
		[Description("True if the horizontal position (absolute) estimate is good")]
		ESTIMATOR_POS_HORIZ_ABS = 0x10,
		[Description("True if the vertical position (absolute) estimate is good")]
		ESTIMATOR_POS_VERT_ABS = 0x20,
		[Description("True if the vertical position (above ground) estimate is good")]
		ESTIMATOR_POS_VERT_AGL = 0x40,
		[Description("True if the EKF is in a constant position mode and is not using external measurements (eg GPS or optical flow)")]
		ESTIMATOR_CONST_POS_MODE = 0x80,
		[Description("True if the EKF has sufficient data to enter a mode that will provide a (relative) position estimate")]
		ESTIMATOR_PRED_POS_HORIZ_REL = 0x100,
		[Description("True if the EKF has sufficient data to enter a mode that will provide a (absolute) position estimate")]
		ESTIMATOR_PRED_POS_HORIZ_ABS = 0x200,
		[Description("True if the EKF has detected a GPS glitch")]
		ESTIMATOR_GPS_GLITCH = 0x400,
		[Description("True if the EKF has detected bad accelerometer data")]
		ESTIMATOR_ACCEL_ERROR = 0x800
	}

	public enum MOTOR_TEST_ORDER
	{
		[Description("Default autopilot motor test method.")]
		DEFAULT,
		[Description("Motor numbers are specified as their index in a predefined vehicle-specific sequence.")]
		SEQUENCE,
		[Description("Motor numbers are specified as the output as labeled on the board.")]
		BOARD
	}

	public enum MOTOR_TEST_THROTTLE_TYPE
	{
		[Description("Throttle as a percentage (0 ~ 100)")]
		MOTOR_TEST_THROTTLE_PERCENT,
		[Description("Throttle as an absolute PWM value (normally in range of 1000~2000).")]
		MOTOR_TEST_THROTTLE_PWM,
		[Description("Throttle pass-through from pilot's transmitter.")]
		MOTOR_TEST_THROTTLE_PILOT,
		[Description("Per-motor compass calibration test.")]
		MOTOR_TEST_COMPASS_CAL
	}

	[Flags]
	public enum GPS_INPUT_IGNORE_FLAGS : ushort
	{
		[Description("ignore altitude field")]
		GPS_INPUT_IGNORE_FLAG_ALT = 1,
		[Description("ignore hdop field")]
		GPS_INPUT_IGNORE_FLAG_HDOP = 2,
		[Description("ignore vdop field")]
		GPS_INPUT_IGNORE_FLAG_VDOP = 4,
		[Description("ignore horizontal velocity field (vn and ve)")]
		GPS_INPUT_IGNORE_FLAG_VEL_HORIZ = 8,
		[Description("ignore vertical velocity field (vd)")]
		GPS_INPUT_IGNORE_FLAG_VEL_VERT = 0x10,
		[Description("ignore speed accuracy field")]
		GPS_INPUT_IGNORE_FLAG_SPEED_ACCURACY = 0x20,
		[Description("ignore horizontal accuracy field")]
		GPS_INPUT_IGNORE_FLAG_HORIZONTAL_ACCURACY = 0x40,
		[Description("ignore vertical accuracy field")]
		GPS_INPUT_IGNORE_FLAG_VERTICAL_ACCURACY = 0x80
	}

	public enum MAV_COLLISION_ACTION : byte
	{
		[Description("Ignore any potential collisions")]
		NONE,
		[Description("Report potential collision")]
		REPORT,
		[Description("Ascend or Descend to avoid threat")]
		ASCEND_OR_DESCEND,
		[Description("Move horizontally to avoid threat")]
		MOVE_HORIZONTALLY,
		[Description("Aircraft to move perpendicular to the collision's velocity vector")]
		MOVE_PERPENDICULAR,
		[Description("Aircraft to fly directly back to its launch point")]
		RTL,
		[Description("Aircraft to stop in place")]
		HOVER
	}

	public enum MAV_COLLISION_THREAT_LEVEL : byte
	{
		[Description("Not a threat")]
		NONE,
		[Description("Craft is mildly concerned about this threat")]
		LOW,
		[Description("Craft is panicking, and may take actions to avoid threat")]
		HIGH
	}

	public enum MAV_COLLISION_SRC : byte
	{
		[Description("ID field references ADSB_VEHICLE packets")]
		ADSB,
		[Description("ID field references MAVLink SRC ID")]
		MAVLINK_GPS_GLOBAL_INT
	}

	public enum GPS_FIX_TYPE : byte
	{
		[Description("No GPS connected")]
		NO_GPS,
		[Description("No position information, GPS is connected")]
		NO_FIX,
		[Description("2D position")]
		_2D_FIX,
		[Description("3D position")]
		_3D_FIX,
		[Description("DGPS/SBAS aided 3D position")]
		DGPS,
		[Description("RTK float, 3D position")]
		RTK_FLOAT,
		[Description("RTK Fixed, 3D position")]
		RTK_FIXED,
		[Description("Static fixed, typically used for base stations")]
		STATIC,
		[Description("PPP, 3D position.")]
		PPP
	}

	public enum RTK_BASELINE_COORDINATE_SYSTEM : byte
	{
		[Description("Earth-centered, Earth-fixed")]
		ECEF,
		[Description("RTK basestation centered, north, east, down")]
		NED
	}

	public enum LANDING_TARGET_TYPE : byte
	{
		[Description("Landing target signaled by light beacon (ex: IR-LOCK)")]
		LIGHT_BEACON,
		[Description("Landing target signaled by radio beacon (ex: ILS, NDB)")]
		RADIO_BEACON,
		[Description("Landing target represented by a fiducial marker (ex: ARTag)")]
		VISION_FIDUCIAL,
		[Description("Landing target represented by a pre-defined visual shape/feature (ex: X-marker, H-marker, square)")]
		VISION_OTHER
	}

	public enum VTOL_TRANSITION_HEADING
	{
		[Description("Respect the heading configuration of the vehicle.")]
		VEHICLE_DEFAULT,
		[Description("Use the heading pointing towards the next waypoint.")]
		NEXT_WAYPOINT,
		[Description("Use the heading on takeoff (while sitting on the ground).")]
		TAKEOFF,
		[Description("Use the specified heading in parameter 4.")]
		SPECIFIED,
		[Description("Use the current heading when reaching takeoff altitude (potentially facing the wind when weather-vaning is active).")]
		ANY
	}

	[Flags]
	public enum CAMERA_CAP_FLAGS : uint
	{
		[Description("Camera is able to record video")]
		CAPTURE_VIDEO = 1u,
		[Description("Camera is able to capture images")]
		CAPTURE_IMAGE = 2u,
		[Description("Camera has separate Video and Image/Photo modes (MAV_CMD_SET_CAMERA_MODE)")]
		HAS_MODES = 4u,
		[Description("Camera can capture images while in video mode")]
		CAN_CAPTURE_IMAGE_IN_VIDEO_MODE = 8u,
		[Description("Camera can capture videos while in Photo/Image mode")]
		CAN_CAPTURE_VIDEO_IN_IMAGE_MODE = 0x10u,
		[Description("Camera has image survey mode (MAV_CMD_SET_CAMERA_MODE)")]
		HAS_IMAGE_SURVEY_MODE = 0x20u,
		[Description("Camera has basic zoom control (MAV_CMD_SET_CAMERA_ZOOM)")]
		HAS_BASIC_ZOOM = 0x40u,
		[Description("Camera has basic focus control (MAV_CMD_SET_CAMERA_FOCUS)")]
		HAS_BASIC_FOCUS = 0x80u,
		[Description("Camera has video streaming capabilities (request VIDEO_STREAM_INFORMATION with MAV_CMD_REQUEST_MESSAGE for video streaming info)")]
		HAS_VIDEO_STREAM = 0x100u,
		[Description("Camera supports tracking of a point on the camera view.")]
		HAS_TRACKING_POINT = 0x200u,
		[Description("Camera supports tracking of a selection rectangle on the camera view.")]
		HAS_TRACKING_RECTANGLE = 0x400u,
		[Description("Camera supports tracking geo status (CAMERA_TRACKING_GEO_STATUS).")]
		HAS_TRACKING_GEO_STATUS = 0x800u,
		[Description("Camera supports absolute thermal range (request CAMERA_THERMAL_RANGE with MAV_CMD_REQUEST_MESSAGE).")]
		HAS_THERMAL_RANGE = 0x1000u
	}

	[Flags]
	public enum VIDEO_STREAM_STATUS_FLAGS : ushort
	{
		[Description("Stream is active (running)")]
		RUNNING = 1,
		[Description("Stream is thermal imaging")]
		THERMAL = 2,
		[Description("Stream can report absolute thermal range (see CAMERA_THERMAL_RANGE).")]
		THERMAL_RANGE_ENABLED = 4
	}

	public enum VIDEO_STREAM_TYPE : byte
	{
		[Description("Stream is RTSP")]
		RTSP,
		[Description("Stream is RTP UDP (URI gives the port number)")]
		RTPUDP,
		[Description("Stream is MPEG on TCP")]
		TCP_MPEG,
		[Description("Stream is MPEG TS (URI gives the port number)")]
		MPEG_TS
	}

	public enum VIDEO_STREAM_ENCODING : byte
	{
		[Description("Stream encoding is unknown")]
		UNKNOWN,
		[Description("Stream encoding is H.264")]
		H264,
		[Description("Stream encoding is H.265")]
		H265
	}

	[Flags]
	public enum CAMERA_TRACKING_STATUS_FLAGS : byte
	{
		[Description("Camera is not tracking")]
		IDLE = 0,
		[Description("Camera is tracking")]
		ACTIVE = 1,
		[Description("Camera tracking in error state")]
		ERROR = 2
	}

	public enum CAMERA_TRACKING_MODE : byte
	{
		[Description("Not tracking")]
		NONE,
		[Description("Target is a point")]
		POINT,
		[Description("Target is a rectangle")]
		RECTANGLE
	}

	public enum CAMERA_TRACKING_TARGET_DATA : byte
	{
		[Description("Target data embedded in image data (proprietary)")]
		EMBEDDED = 1,
		[Description("Target data rendered in image")]
		RENDERED = 2,
		[Description("Target data within status message (Point or Rectangle)")]
		IN_STATUS = 4
	}

	public enum CAMERA_ZOOM_TYPE
	{
		[Description("Zoom one step increment (-1 for wide, 1 for tele)")]
		ZOOM_TYPE_STEP,
		[Description("Continuous normalized zoom in/out rate until stopped. Range -1..1, negative: wide, positive: narrow/tele, 0 to stop zooming. Other values should be clipped to the range.")]
		ZOOM_TYPE_CONTINUOUS,
		[Description("Zoom value as proportion of full camera range (a percentage value between 0.0 and 100.0)")]
		ZOOM_TYPE_RANGE,
		[Description("Zoom value/variable focal length in millimetres. Note that there is no message to get the valid zoom range of the camera, so this can type can only be used for cameras where the zoom range is known (implying that this cannot reliably be used in a GCS for an arbitrary camera)")]
		ZOOM_TYPE_FOCAL_LENGTH,
		[Description("Zoom value as horizontal field of view in degrees.")]
		ZOOM_TYPE_HORIZONTAL_FOV
	}

	public enum SET_FOCUS_TYPE
	{
		[Description("Focus one step increment (-1 for focusing in, 1 for focusing out towards infinity).")]
		FOCUS_TYPE_STEP,
		[Description("Continuous normalized focus in/out rate until stopped. Range -1..1, negative: in, positive: out towards infinity, 0 to stop focusing. Other values should be clipped to the range.")]
		FOCUS_TYPE_CONTINUOUS,
		[Description("Focus value as proportion of full camera focus range (a value between 0.0 and 100.0)")]
		FOCUS_TYPE_RANGE,
		[Description("Focus value in metres. Note that there is no message to get the valid focus range of the camera, so this can type can only be used for cameras where the range is known (implying that this cannot reliably be used in a GCS for an arbitrary camera).")]
		FOCUS_TYPE_METERS,
		[Description("Focus automatically.")]
		FOCUS_TYPE_AUTO,
		[Description("Single auto focus. Mainly used for still pictures. Usually abbreviated as AF-S.")]
		FOCUS_TYPE_AUTO_SINGLE,
		[Description("Continuous auto focus. Mainly used for dynamic scenes. Abbreviated as AF-C.")]
		FOCUS_TYPE_AUTO_CONTINUOUS
	}

	public enum CAMERA_SOURCE
	{
		[Description("Default camera source.")]
		DEFAULT,
		[Description("RGB camera source.")]
		RGB,
		[Description("IR camera source.")]
		IR,
		[Description("NDVI camera source.")]
		NDVI
	}

	public enum PARAM_ACK : byte
	{
		[Description("Parameter value ACCEPTED and SET")]
		ACCEPTED,
		[Description("Parameter value UNKNOWN/UNSUPPORTED")]
		VALUE_UNSUPPORTED,
		[Description("Parameter failed to set")]
		FAILED,
		[Description("Parameter value received but not yet set/accepted. A subsequent PARAM_EXT_ACK with the final result will follow once operation is completed. This is returned immediately for parameters that take longer to set, indicating that the the parameter was received and does not need to be resent.")]
		IN_PROGRESS
	}

	public enum CAMERA_MODE : byte
	{
		[Description("Camera is in image/photo capture mode.")]
		IMAGE,
		[Description("Camera is in video capture mode.")]
		VIDEO,
		[Description("Camera is in image survey capture mode. It allows for camera controller to do specific settings for surveys.")]
		IMAGE_SURVEY
	}

	public enum MAV_ARM_AUTH_DENIED_REASON
	{
		[Description("Not a specific reason")]
		GENERIC,
		[Description("Authorizer will send the error as string to GCS")]
		NONE,
		[Description("At least one waypoint have a invalid value")]
		INVALID_WAYPOINT,
		[Description("Timeout in the authorizer process(in case it depends on network)")]
		TIMEOUT,
		[Description("Airspace of the mission in use by another vehicle, second result parameter can have the waypoint id that caused it to be denied.")]
		AIRSPACE_IN_USE,
		[Description("Weather is not good to fly")]
		BAD_WEATHER
	}

	public enum RC_TYPE
	{
		[Description("Spektrum")]
		SPEKTRUM,
		[Description("CRSF")]
		CRSF
	}

	public enum RC_SUB_TYPE
	{
		[Description("Spektrum DSM2")]
		SPEKTRUM_DSM2,
		[Description("Spektrum DSMX")]
		SPEKTRUM_DSMX,
		[Description("Spektrum DSMX8")]
		SPEKTRUM_DSMX8
	}

	public enum POSITION_TARGET_TYPEMASK : ushort
	{
		[Description("Ignore position x")]
		X_IGNORE = 1,
		[Description("Ignore position y")]
		Y_IGNORE = 2,
		[Description("Ignore position z")]
		Z_IGNORE = 4,
		[Description("Ignore velocity x")]
		VX_IGNORE = 8,
		[Description("Ignore velocity y")]
		VY_IGNORE = 0x10,
		[Description("Ignore velocity z")]
		VZ_IGNORE = 0x20,
		[Description("Ignore acceleration x")]
		AX_IGNORE = 0x40,
		[Description("Ignore acceleration y")]
		AY_IGNORE = 0x80,
		[Description("Ignore acceleration z")]
		AZ_IGNORE = 0x100,
		[Description("Use force instead of acceleration")]
		FORCE_SET = 0x200,
		[Description("Ignore yaw")]
		YAW_IGNORE = 0x400,
		[Description("Ignore yaw rate")]
		YAW_RATE_IGNORE = 0x800
	}

	public enum ATTITUDE_TARGET_TYPEMASK : byte
	{
		[Description("Ignore body roll rate")]
		BODY_ROLL_RATE_IGNORE = 1,
		[Description("Ignore body pitch rate")]
		BODY_PITCH_RATE_IGNORE = 2,
		[Description("Ignore body yaw rate")]
		BODY_YAW_RATE_IGNORE = 4,
		[Description("Use 3D body thrust setpoint instead of throttle")]
		THRUST_BODY_SET = 0x20,
		[Description("Ignore throttle")]
		THROTTLE_IGNORE = 0x40,
		[Description("Ignore attitude")]
		ATTITUDE_IGNORE = 0x80
	}

	public enum UTM_FLIGHT_STATE : byte
	{
		[Description("The flight state can't be determined.")]
		UNKNOWN = 1,
		[Description("UAS on ground.")]
		GROUND = 2,
		[Description("UAS airborne.")]
		AIRBORNE = 3,
		[Description("UAS is in an emergency flight state.")]
		EMERGENCY = 16,
		[Description("UAS has no active controls.")]
		NOCTRL = 32
	}

	[Flags]
	public enum UTM_DATA_AVAIL_FLAGS : byte
	{
		[Description("The field time contains valid data.")]
		TIME_VALID = 1,
		[Description("The field uas_id contains valid data.")]
		UAS_ID_AVAILABLE = 2,
		[Description("The fields lat, lon and h_acc contain valid data.")]
		POSITION_AVAILABLE = 4,
		[Description("The fields alt and v_acc contain valid data.")]
		ALTITUDE_AVAILABLE = 8,
		[Description("The field relative_alt contains valid data.")]
		RELATIVE_ALTITUDE_AVAILABLE = 0x10,
		[Description("The fields vx and vy contain valid data.")]
		HORIZONTAL_VELO_AVAILABLE = 0x20,
		[Description("The field vz contains valid data.")]
		VERTICAL_VELO_AVAILABLE = 0x40,
		[Description("The fields next_lat, next_lon and next_alt contain valid data.")]
		NEXT_WAYPOINT_AVAILABLE = 0x80
	}

	public enum CELLULAR_STATUS_FLAG : byte
	{
		[Description("State unknown or not reportable.")]
		UNKNOWN,
		[Description("Modem is unusable")]
		FAILED,
		[Description("Modem is being initialized")]
		INITIALIZING,
		[Description("Modem is locked")]
		LOCKED,
		[Description("Modem is not enabled and is powered down")]
		DISABLED,
		[Description("Modem is currently transitioning to the CELLULAR_STATUS_FLAG_DISABLED state")]
		DISABLING,
		[Description("Modem is currently transitioning to the CELLULAR_STATUS_FLAG_ENABLED state")]
		ENABLING,
		[Description("Modem is enabled and powered on but not registered with a network provider and not available for data connections")]
		ENABLED,
		[Description("Modem is searching for a network provider to register")]
		SEARCHING,
		[Description("Modem is registered with a network provider, and data connections and messaging may be available for use")]
		REGISTERED,
		[Description("Modem is disconnecting and deactivating the last active packet data bearer. This state will not be entered if more than one packet data bearer is active and one of the active bearers is deactivated")]
		DISCONNECTING,
		[Description("Modem is activating and connecting the first packet data bearer. Subsequent bearer activations when another bearer is already active do not cause this state to be entered")]
		CONNECTING,
		[Description("One or more packet data bearers is active and connected")]
		CONNECTED
	}

	public enum CELLULAR_NETWORK_FAILED_REASON : byte
	{
		[Description("No error")]
		NONE,
		[Description("Error state is unknown")]
		UNKNOWN,
		[Description("SIM is required for the modem but missing")]
		SIM_MISSING,
		[Description("SIM is available, but not usable for connection")]
		SIM_ERROR
	}

	public enum CELLULAR_NETWORK_RADIO_TYPE : byte
	{
		[Description("")]
		NONE,
		[Description("")]
		GSM,
		[Description("")]
		CDMA,
		[Description("")]
		WCDMA,
		[Description("")]
		LTE
	}

	public enum PRECISION_LAND_MODE
	{
		[Description("Normal (non-precision) landing.")]
		DISABLED,
		[Description("Use precision landing if beacon detected when land command accepted, otherwise land normally.")]
		OPPORTUNISTIC,
		[Description("Use precision landing, searching for beacon if not found when land command accepted (land normally if beacon cannot be found).")]
		REQUIRED
	}

	public enum PARACHUTE_ACTION
	{
		[Description("Disable auto-release of parachute (i.e. release triggered by crash detectors).")]
		PARACHUTE_DISABLE,
		[Description("Enable auto-release of parachute.")]
		PARACHUTE_ENABLE,
		[Description("Release parachute and kill motors.")]
		PARACHUTE_RELEASE
	}

	public enum MAV_TUNNEL_PAYLOAD_TYPE : ushort
	{
		[Description("Encoding of payload unknown.")]
		UNKNOWN = 0,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED0 = 200,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED1 = 201,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED2 = 202,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED3 = 203,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED4 = 204,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED5 = 205,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED6 = 206,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED7 = 207,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED8 = 208,
		[Description("Registered for STorM32 gimbal controller.")]
		STORM32_RESERVED9 = 209,
		[Description("Registered for ModalAI remote OSD protocol.")]
		MODALAI_REMOTE_OSD = 210,
		[Description("Registered for ModalAI ESC UART passthru protocol.")]
		MODALAI_ESC_UART_PASSTHRU = 211,
		[Description("Registered for ModalAI vendor use.")]
		MODALAI_IO_UART_PASSTHRU = 212
	}

	public enum MAV_ODID_ID_TYPE : byte
	{
		[Description("No type defined.")]
		NONE,
		[Description("Manufacturer Serial Number (ANSI/CTA-2063 format).")]
		SERIAL_NUMBER,
		[Description("CAA (Civil Aviation Authority) registered ID. Format: [ICAO Country Code].[CAA Assigned ID].")]
		CAA_REGISTRATION_ID,
		[Description("UTM (Unmanned Traffic Management) assigned UUID (RFC4122).")]
		UTM_ASSIGNED_UUID,
		[Description("A 20 byte ID for a specific flight/session. The exact ID type is indicated by the first byte of uas_id and these type values are managed by ICAO.")]
		SPECIFIC_SESSION_ID
	}

	public enum MAV_ODID_UA_TYPE : byte
	{
		[Description("No UA (Unmanned Aircraft) type defined.")]
		NONE,
		[Description("Aeroplane/Airplane. Fixed wing.")]
		AEROPLANE,
		[Description("Helicopter or multirotor.")]
		HELICOPTER_OR_MULTIROTOR,
		[Description("Gyroplane.")]
		GYROPLANE,
		[Description("VTOL (Vertical Take-Off and Landing). Fixed wing aircraft that can take off vertically.")]
		HYBRID_LIFT,
		[Description("Ornithopter.")]
		ORNITHOPTER,
		[Description("Glider.")]
		GLIDER,
		[Description("Kite.")]
		KITE,
		[Description("Free Balloon.")]
		FREE_BALLOON,
		[Description("Captive Balloon.")]
		CAPTIVE_BALLOON,
		[Description("Airship. E.g. a blimp.")]
		AIRSHIP,
		[Description("Free Fall/Parachute (unpowered).")]
		FREE_FALL_PARACHUTE,
		[Description("Rocket.")]
		ROCKET,
		[Description("Tethered powered aircraft.")]
		TETHERED_POWERED_AIRCRAFT,
		[Description("Ground Obstacle.")]
		GROUND_OBSTACLE,
		[Description("Other type of aircraft not listed earlier.")]
		OTHER
	}

	public enum MAV_ODID_STATUS : byte
	{
		[Description("The status of the (UA) Unmanned Aircraft is undefined.")]
		UNDECLARED,
		[Description("The UA is on the ground.")]
		GROUND,
		[Description("The UA is in the air.")]
		AIRBORNE,
		[Description("The UA is having an emergency.")]
		EMERGENCY,
		[Description("The remote ID system is failing or unreliable in some way.")]
		REMOTE_ID_SYSTEM_FAILURE
	}

	public enum MAV_ODID_HEIGHT_REF : byte
	{
		[Description("The height field is relative to the take-off location.")]
		OVER_TAKEOFF,
		[Description("The height field is relative to ground.")]
		OVER_GROUND
	}

	public enum MAV_ODID_HOR_ACC : byte
	{
		[Description("The horizontal accuracy is unknown.")]
		UNKNOWN,
		[Description("The horizontal accuracy is smaller than 10 Nautical Miles. 18.52 km.")]
		_10NM,
		[Description("The horizontal accuracy is smaller than 4 Nautical Miles. 7.408 km.")]
		_4NM,
		[Description("The horizontal accuracy is smaller than 2 Nautical Miles. 3.704 km.")]
		_2NM,
		[Description("The horizontal accuracy is smaller than 1 Nautical Miles. 1.852 km.")]
		_1NM,
		[Description("The horizontal accuracy is smaller than 0.5 Nautical Miles. 926 m.")]
		_0_5NM,
		[Description("The horizontal accuracy is smaller than 0.3 Nautical Miles. 555.6 m.")]
		_0_3NM,
		[Description("The horizontal accuracy is smaller than 0.1 Nautical Miles. 185.2 m.")]
		_0_1NM,
		[Description("The horizontal accuracy is smaller than 0.05 Nautical Miles. 92.6 m.")]
		_0_05NM,
		[Description("The horizontal accuracy is smaller than 30 meter.")]
		_30_METER,
		[Description("The horizontal accuracy is smaller than 10 meter.")]
		_10_METER,
		[Description("The horizontal accuracy is smaller than 3 meter.")]
		_3_METER,
		[Description("The horizontal accuracy is smaller than 1 meter.")]
		_1_METER
	}

	public enum MAV_ODID_VER_ACC : byte
	{
		[Description("The vertical accuracy is unknown.")]
		UNKNOWN,
		[Description("The vertical accuracy is smaller than 150 meter.")]
		_150_METER,
		[Description("The vertical accuracy is smaller than 45 meter.")]
		_45_METER,
		[Description("The vertical accuracy is smaller than 25 meter.")]
		_25_METER,
		[Description("The vertical accuracy is smaller than 10 meter.")]
		_10_METER,
		[Description("The vertical accuracy is smaller than 3 meter.")]
		_3_METER,
		[Description("The vertical accuracy is smaller than 1 meter.")]
		_1_METER
	}

	public enum MAV_ODID_SPEED_ACC : byte
	{
		[Description("The speed accuracy is unknown.")]
		UNKNOWN,
		[Description("The speed accuracy is smaller than 10 meters per second.")]
		_10_METERS_PER_SECOND,
		[Description("The speed accuracy is smaller than 3 meters per second.")]
		_3_METERS_PER_SECOND,
		[Description("The speed accuracy is smaller than 1 meters per second.")]
		_1_METERS_PER_SECOND,
		[Description("The speed accuracy is smaller than 0.3 meters per second.")]
		_0_3_METERS_PER_SECOND
	}

	public enum MAV_ODID_TIME_ACC : byte
	{
		[Description("The timestamp accuracy is unknown.")]
		UNKNOWN,
		[Description("The timestamp accuracy is smaller than or equal to 0.1 second.")]
		_0_1_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 0.2 second.")]
		_0_2_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 0.3 second.")]
		_0_3_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 0.4 second.")]
		_0_4_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 0.5 second.")]
		_0_5_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 0.6 second.")]
		_0_6_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 0.7 second.")]
		_0_7_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 0.8 second.")]
		_0_8_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 0.9 second.")]
		_0_9_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 1.0 second.")]
		_1_0_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 1.1 second.")]
		_1_1_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 1.2 second.")]
		_1_2_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 1.3 second.")]
		_1_3_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 1.4 second.")]
		_1_4_SECOND,
		[Description("The timestamp accuracy is smaller than or equal to 1.5 second.")]
		_1_5_SECOND
	}

	public enum MAV_ODID_AUTH_TYPE : byte
	{
		[Description("No authentication type is specified.")]
		NONE,
		[Description("Signature for the UAS (Unmanned Aircraft System) ID.")]
		UAS_ID_SIGNATURE,
		[Description("Signature for the Operator ID.")]
		OPERATOR_ID_SIGNATURE,
		[Description("Signature for the entire message set.")]
		MESSAGE_SET_SIGNATURE,
		[Description("Authentication is provided by Network Remote ID.")]
		NETWORK_REMOTE_ID,
		[Description("The exact authentication type is indicated by the first byte of authentication_data and these type values are managed by ICAO.")]
		SPECIFIC_AUTHENTICATION
	}

	public enum MAV_ODID_DESC_TYPE : byte
	{
		[Description("Optional free-form text description of the purpose of the flight.")]
		TEXT,
		[Description("Optional additional clarification when status == MAV_ODID_STATUS_EMERGENCY.")]
		EMERGENCY,
		[Description("Optional additional clarification when status != MAV_ODID_STATUS_EMERGENCY.")]
		EXTENDED_STATUS
	}

	public enum MAV_ODID_OPERATOR_LOCATION_TYPE : byte
	{
		[Description("The location/altitude of the operator is the same as the take-off location.")]
		TAKEOFF,
		[Description("The location/altitude of the operator is dynamic. E.g. based on live GNSS data.")]
		LIVE_GNSS,
		[Description("The location/altitude of the operator are fixed values.")]
		FIXED
	}

	public enum MAV_ODID_CLASSIFICATION_TYPE : byte
	{
		[Description("The classification type for the UA is undeclared.")]
		UNDECLARED,
		[Description("The classification type for the UA follows EU (European Union) specifications.")]
		EU
	}

	public enum MAV_ODID_CATEGORY_EU : byte
	{
		[Description("The category for the UA, according to the EU specification, is undeclared.")]
		UNDECLARED,
		[Description("The category for the UA, according to the EU specification, is the Open category.")]
		OPEN,
		[Description("The category for the UA, according to the EU specification, is the Specific category.")]
		SPECIFIC,
		[Description("The category for the UA, according to the EU specification, is the Certified category.")]
		CERTIFIED
	}

	public enum MAV_ODID_CLASS_EU : byte
	{
		[Description("The class for the UA, according to the EU specification, is undeclared.")]
		UNDECLARED,
		[Description("The class for the UA, according to the EU specification, is Class 0.")]
		CLASS_0,
		[Description("The class for the UA, according to the EU specification, is Class 1.")]
		CLASS_1,
		[Description("The class for the UA, according to the EU specification, is Class 2.")]
		CLASS_2,
		[Description("The class for the UA, according to the EU specification, is Class 3.")]
		CLASS_3,
		[Description("The class for the UA, according to the EU specification, is Class 4.")]
		CLASS_4,
		[Description("The class for the UA, according to the EU specification, is Class 5.")]
		CLASS_5,
		[Description("The class for the UA, according to the EU specification, is Class 6.")]
		CLASS_6
	}

	public enum MAV_ODID_OPERATOR_ID_TYPE : byte
	{
		[Description("CAA (Civil Aviation Authority) registered operator ID.")]
		CAA
	}

	public enum MAV_ODID_ARM_STATUS : byte
	{
		[Description("Passing arming checks.")]
		GOOD_TO_ARM,
		[Description("Generic arming failure, see error string for details.")]
		PRE_ARM_FAIL_GENERIC
	}

	public enum TUNE_FORMAT : uint
	{
		[Description("Format is QBasic 1.1 Play: https://www.qbasic.net/en/reference/qb11/Statement/PLAY-006.htm.")]
		QBASIC1_1 = 1u,
		[Description("Format is Modern Music Markup Language (MML): https://en.wikipedia.org/wiki/Music_Macro_Language#Modern_MML.")]
		MML_MODERN
	}

	public enum AIS_TYPE : byte
	{
		[Description("Not available (default).")]
		UNKNOWN,
		[Description("")]
		RESERVED_1,
		[Description("")]
		RESERVED_2,
		[Description("")]
		RESERVED_3,
		[Description("")]
		RESERVED_4,
		[Description("")]
		RESERVED_5,
		[Description("")]
		RESERVED_6,
		[Description("")]
		RESERVED_7,
		[Description("")]
		RESERVED_8,
		[Description("")]
		RESERVED_9,
		[Description("")]
		RESERVED_10,
		[Description("")]
		RESERVED_11,
		[Description("")]
		RESERVED_12,
		[Description("")]
		RESERVED_13,
		[Description("")]
		RESERVED_14,
		[Description("")]
		RESERVED_15,
		[Description("")]
		RESERVED_16,
		[Description("")]
		RESERVED_17,
		[Description("")]
		RESERVED_18,
		[Description("")]
		RESERVED_19,
		[Description("Wing In Ground effect.")]
		WIG,
		[Description("")]
		WIG_HAZARDOUS_A,
		[Description("")]
		WIG_HAZARDOUS_B,
		[Description("")]
		WIG_HAZARDOUS_C,
		[Description("")]
		WIG_HAZARDOUS_D,
		[Description("")]
		WIG_RESERVED_1,
		[Description("")]
		WIG_RESERVED_2,
		[Description("")]
		WIG_RESERVED_3,
		[Description("")]
		WIG_RESERVED_4,
		[Description("")]
		WIG_RESERVED_5,
		[Description("")]
		FISHING,
		[Description("")]
		TOWING,
		[Description("Towing: length exceeds 200m or breadth exceeds 25m.")]
		TOWING_LARGE,
		[Description("Dredging or other underwater ops.")]
		DREDGING,
		[Description("")]
		DIVING,
		[Description("")]
		MILITARY,
		[Description("")]
		SAILING,
		[Description("")]
		PLEASURE,
		[Description("")]
		RESERVED_20,
		[Description("")]
		RESERVED_21,
		[Description("High Speed Craft.")]
		HSC,
		[Description("")]
		HSC_HAZARDOUS_A,
		[Description("")]
		HSC_HAZARDOUS_B,
		[Description("")]
		HSC_HAZARDOUS_C,
		[Description("")]
		HSC_HAZARDOUS_D,
		[Description("")]
		HSC_RESERVED_1,
		[Description("")]
		HSC_RESERVED_2,
		[Description("")]
		HSC_RESERVED_3,
		[Description("")]
		HSC_RESERVED_4,
		[Description("")]
		HSC_UNKNOWN,
		[Description("")]
		PILOT,
		[Description("Search And Rescue vessel.")]
		SAR,
		[Description("")]
		TUG,
		[Description("")]
		PORT_TENDER,
		[Description("Anti-pollution equipment.")]
		ANTI_POLLUTION,
		[Description("")]
		LAW_ENFORCEMENT,
		[Description("")]
		SPARE_LOCAL_1,
		[Description("")]
		SPARE_LOCAL_2,
		[Description("")]
		MEDICAL_TRANSPORT,
		[Description("Noncombatant ship according to RR Resolution No. 18.")]
		NONECOMBATANT,
		[Description("")]
		PASSENGER,
		[Description("")]
		PASSENGER_HAZARDOUS_A,
		[Description("")]
		PASSENGER_HAZARDOUS_B,
		[Description("")]
		PASSENGER_HAZARDOUS_C,
		[Description("")]
		PASSENGER_HAZARDOUS_D,
		[Description("")]
		PASSENGER_RESERVED_1,
		[Description("")]
		PASSENGER_RESERVED_2,
		[Description("")]
		PASSENGER_RESERVED_3,
		[Description("")]
		PASSENGER_RESERVED_4,
		[Description("")]
		PASSENGER_UNKNOWN,
		[Description("")]
		CARGO,
		[Description("")]
		CARGO_HAZARDOUS_A,
		[Description("")]
		CARGO_HAZARDOUS_B,
		[Description("")]
		CARGO_HAZARDOUS_C,
		[Description("")]
		CARGO_HAZARDOUS_D,
		[Description("")]
		CARGO_RESERVED_1,
		[Description("")]
		CARGO_RESERVED_2,
		[Description("")]
		CARGO_RESERVED_3,
		[Description("")]
		CARGO_RESERVED_4,
		[Description("")]
		CARGO_UNKNOWN,
		[Description("")]
		TANKER,
		[Description("")]
		TANKER_HAZARDOUS_A,
		[Description("")]
		TANKER_HAZARDOUS_B,
		[Description("")]
		TANKER_HAZARDOUS_C,
		[Description("")]
		TANKER_HAZARDOUS_D,
		[Description("")]
		TANKER_RESERVED_1,
		[Description("")]
		TANKER_RESERVED_2,
		[Description("")]
		TANKER_RESERVED_3,
		[Description("")]
		TANKER_RESERVED_4,
		[Description("")]
		TANKER_UNKNOWN,
		[Description("")]
		OTHER,
		[Description("")]
		OTHER_HAZARDOUS_A,
		[Description("")]
		OTHER_HAZARDOUS_B,
		[Description("")]
		OTHER_HAZARDOUS_C,
		[Description("")]
		OTHER_HAZARDOUS_D,
		[Description("")]
		OTHER_RESERVED_1,
		[Description("")]
		OTHER_RESERVED_2,
		[Description("")]
		OTHER_RESERVED_3,
		[Description("")]
		OTHER_RESERVED_4,
		[Description("")]
		OTHER_UNKNOWN
	}

	public enum AIS_NAV_STATUS : byte
	{
		[Description("Under way using engine.")]
		AIS_STATUS_UNDER_WAY,
		[Description("")]
		AIS_STATUS_ANCHORED,
		[Description("")]
		AIS_STATUS_UN_COMMANDED,
		[Description("")]
		AIS_STATUS_RESTRICTED_MANOEUVERABILITY,
		[Description("")]
		AIS_STATUS_DRAUGHT_CONSTRAINED,
		[Description("")]
		AIS_STATUS_MOORED,
		[Description("")]
		AIS_STATUS_AGROUND,
		[Description("")]
		AIS_STATUS_FISHING,
		[Description("")]
		AIS_STATUS_SAILING,
		[Description("")]
		AIS_STATUS_RESERVED_HSC,
		[Description("")]
		AIS_STATUS_RESERVED_WIG,
		[Description("")]
		AIS_STATUS_RESERVED_1,
		[Description("")]
		AIS_STATUS_RESERVED_2,
		[Description("")]
		AIS_STATUS_RESERVED_3,
		[Description("Search And Rescue Transponder.")]
		AIS_STATUS_AIS_SART,
		[Description("Not available (default).")]
		AIS_STATUS_UNKNOWN
	}

	[Flags]
	public enum AIS_FLAGS : ushort
	{
		[Description("1 = Position accuracy less than 10m, 0 = position accuracy greater than 10m.")]
		POSITION_ACCURACY = 1,
		[Description("")]
		VALID_COG = 2,
		[Description("")]
		VALID_VELOCITY = 4,
		[Description("1 = Velocity over 52.5765m/s (102.2 knots)")]
		HIGH_VELOCITY = 8,
		[Description("")]
		VALID_TURN_RATE = 0x10,
		[Description("Only the sign of the returned turn rate value is valid, either greater than 5deg/30s or less than -5deg/30s")]
		TURN_RATE_SIGN_ONLY = 0x20,
		[Description("")]
		VALID_DIMENSIONS = 0x40,
		[Description("Distance to bow is larger than 511m")]
		LARGE_BOW_DIMENSION = 0x80,
		[Description("Distance to stern is larger than 511m")]
		LARGE_STERN_DIMENSION = 0x100,
		[Description("Distance to port side is larger than 63m")]
		LARGE_PORT_DIMENSION = 0x200,
		[Description("Distance to starboard side is larger than 63m")]
		LARGE_STARBOARD_DIMENSION = 0x400,
		[Description("")]
		VALID_CALLSIGN = 0x800,
		[Description("")]
		VALID_NAME = 0x1000
	}

	public enum FAILURE_UNIT
	{
		[Description("")]
		SENSOR_GYRO = 0,
		[Description("")]
		SENSOR_ACCEL = 1,
		[Description("")]
		SENSOR_MAG = 2,
		[Description("")]
		SENSOR_BARO = 3,
		[Description("")]
		SENSOR_GPS = 4,
		[Description("")]
		SENSOR_OPTICAL_FLOW = 5,
		[Description("")]
		SENSOR_VIO = 6,
		[Description("")]
		SENSOR_DISTANCE_SENSOR = 7,
		[Description("")]
		SENSOR_AIRSPEED = 8,
		[Description("")]
		SYSTEM_BATTERY = 100,
		[Description("")]
		SYSTEM_MOTOR = 101,
		[Description("")]
		SYSTEM_SERVO = 102,
		[Description("")]
		SYSTEM_AVOIDANCE = 103,
		[Description("")]
		SYSTEM_RC_SIGNAL = 104,
		[Description("")]
		SYSTEM_MAVLINK_SIGNAL = 105
	}

	public enum FAILURE_TYPE
	{
		[Description("No failure injected, used to reset a previous failure.")]
		OK,
		[Description("Sets unit off, so completely non-responsive.")]
		OFF,
		[Description("Unit is stuck e.g. keeps reporting the same value.")]
		STUCK,
		[Description("Unit is reporting complete garbage.")]
		GARBAGE,
		[Description("Unit is consistently wrong.")]
		WRONG,
		[Description("Unit is slow, so e.g. reporting at slower than expected rate.")]
		SLOW,
		[Description("Data of unit is delayed in time.")]
		DELAYED,
		[Description("Unit is sometimes working, sometimes not.")]
		INTERMITTENT
	}

	public enum NAV_VTOL_LAND_OPTIONS
	{
		[Description("Default autopilot landing behaviour.")]
		VTOL_LAND_OPTIONS_DEFAULT,
		[Description("Descend in fixed wing mode, transitioning to multicopter mode for vertical landing when close to the ground.           The fixed wing descent pattern is at the discretion of the vehicle (e.g. transition altitude, loiter direction, radius, and speed, etc.).         ")]
		VTOL_LAND_OPTIONS_FW_DESCENT,
		[Description("Land in multicopter mode on reaching the landing coordinates (the whole landing is by 'hover descent').")]
		VTOL_LAND_OPTIONS_HOVER_DESCENT
	}

	public enum MAV_WINCH_STATUS_FLAG : uint
	{
		[Description("Winch is healthy")]
		MAV_WINCH_STATUS_HEALTHY = 1u,
		[Description("Winch line is fully retracted")]
		MAV_WINCH_STATUS_FULLY_RETRACTED = 2u,
		[Description("Winch motor is moving")]
		MAV_WINCH_STATUS_MOVING = 4u,
		[Description("Winch clutch is engaged allowing motor to move freely.")]
		MAV_WINCH_STATUS_CLUTCH_ENGAGED = 8u,
		[Description("Winch is locked by locking mechanism.")]
		MAV_WINCH_STATUS_LOCKED = 0x10u,
		[Description("Winch is gravity dropping payload.")]
		MAV_WINCH_STATUS_DROPPING = 0x20u,
		[Description("Winch is arresting payload descent.")]
		MAV_WINCH_STATUS_ARRESTING = 0x40u,
		[Description("Winch is using torque measurements to sense the ground.")]
		MAV_WINCH_STATUS_GROUND_SENSE = 0x80u,
		[Description("Winch is returning to the fully retracted position.")]
		MAV_WINCH_STATUS_RETRACTING = 0x100u,
		[Description("Winch is redelivering the payload. This is a failover state if the line tension goes above a threshold during RETRACTING.")]
		MAV_WINCH_STATUS_REDELIVER = 0x200u,
		[Description("Winch is abandoning the line and possibly payload. Winch unspools the entire calculated line length. This is a failover state from REDELIVER if the number of attempts exceeds a threshold.")]
		MAV_WINCH_STATUS_ABANDON_LINE = 0x400u,
		[Description("Winch is engaging the locking mechanism.")]
		MAV_WINCH_STATUS_LOCKING = 0x800u,
		[Description("Winch is spooling on line.")]
		MAV_WINCH_STATUS_LOAD_LINE = 0x1000u,
		[Description("Winch is loading a payload.")]
		MAV_WINCH_STATUS_LOAD_PAYLOAD = 0x2000u
	}

	public enum MAG_CAL_STATUS : byte
	{
		[Description("")]
		MAG_CAL_NOT_STARTED,
		[Description("")]
		MAG_CAL_WAITING_TO_START,
		[Description("")]
		MAG_CAL_RUNNING_STEP_ONE,
		[Description("")]
		MAG_CAL_RUNNING_STEP_TWO,
		[Description("")]
		MAG_CAL_SUCCESS,
		[Description("")]
		MAG_CAL_FAILED,
		[Description("")]
		MAG_CAL_BAD_ORIENTATION,
		[Description("")]
		MAG_CAL_BAD_RADIUS
	}

	public enum MAV_EVENT_ERROR_REASON : byte
	{
		[Description("The requested event is not available (anymore).")]
		UNAVAILABLE
	}

	[Flags]
	public enum MAV_EVENT_CURRENT_SEQUENCE_FLAGS : byte
	{
		[Description("A sequence reset has happened (e.g. vehicle reboot).")]
		RESET = 1
	}

	[Flags]
	public enum HIL_SENSOR_UPDATED_FLAGS : uint
	{
		[Description("The value in the xacc field has been updated")]
		HIL_SENSOR_UPDATED_XACC = 1u,
		[Description("The value in the yacc field has been updated")]
		HIL_SENSOR_UPDATED_YACC = 2u,
		[Description("The value in the zacc field has been updated")]
		HIL_SENSOR_UPDATED_ZACC = 4u,
		[Description("The value in the xgyro field has been updated")]
		HIL_SENSOR_UPDATED_XGYRO = 8u,
		[Description("The value in the ygyro field has been updated")]
		HIL_SENSOR_UPDATED_YGYRO = 0x10u,
		[Description("The value in the zgyro field has been updated")]
		HIL_SENSOR_UPDATED_ZGYRO = 0x20u,
		[Description("The value in the xmag field has been updated")]
		HIL_SENSOR_UPDATED_XMAG = 0x40u,
		[Description("The value in the ymag field has been updated")]
		HIL_SENSOR_UPDATED_YMAG = 0x80u,
		[Description("The value in the zmag field has been updated")]
		HIL_SENSOR_UPDATED_ZMAG = 0x100u,
		[Description("The value in the abs_pressure field has been updated")]
		HIL_SENSOR_UPDATED_ABS_PRESSURE = 0x200u,
		[Description("The value in the diff_pressure field has been updated")]
		HIL_SENSOR_UPDATED_DIFF_PRESSURE = 0x400u,
		[Description("The value in the pressure_alt field has been updated")]
		HIL_SENSOR_UPDATED_PRESSURE_ALT = 0x800u,
		[Description("The value in the temperature field has been updated")]
		HIL_SENSOR_UPDATED_TEMPERATURE = 0x1000u,
		[Description("Full reset of attitude/position/velocities/etc was performed in sim (Bit 31).")]
		HIL_SENSOR_UPDATED_RESET = 0x80000000u
	}

	[Flags]
	public enum HIGHRES_IMU_UPDATED_FLAGS : ushort
	{
		[Description("The value in the xacc field has been updated")]
		HIGHRES_IMU_UPDATED_XACC = 1,
		[Description("The value in the yacc field has been updated")]
		HIGHRES_IMU_UPDATED_YACC = 2,
		[Description("The value in the zacc field has been updated since")]
		HIGHRES_IMU_UPDATED_ZACC = 4,
		[Description("The value in the xgyro field has been updated")]
		HIGHRES_IMU_UPDATED_XGYRO = 8,
		[Description("The value in the ygyro field has been updated")]
		HIGHRES_IMU_UPDATED_YGYRO = 0x10,
		[Description("The value in the zgyro field has been updated")]
		HIGHRES_IMU_UPDATED_ZGYRO = 0x20,
		[Description("The value in the xmag field has been updated")]
		HIGHRES_IMU_UPDATED_XMAG = 0x40,
		[Description("The value in the ymag field has been updated")]
		HIGHRES_IMU_UPDATED_YMAG = 0x80,
		[Description("The value in the zmag field has been updated")]
		HIGHRES_IMU_UPDATED_ZMAG = 0x100,
		[Description("The value in the abs_pressure field has been updated")]
		HIGHRES_IMU_UPDATED_ABS_PRESSURE = 0x200,
		[Description("The value in the diff_pressure field has been updated")]
		HIGHRES_IMU_UPDATED_DIFF_PRESSURE = 0x400,
		[Description("The value in the pressure_alt field has been updated")]
		HIGHRES_IMU_UPDATED_PRESSURE_ALT = 0x800,
		[Description("The value in the temperature field has been updated")]
		HIGHRES_IMU_UPDATED_TEMPERATURE = 0x1000
	}

	public enum CAN_FILTER_OP : byte
	{
		[Description("")]
		CAN_FILTER_REPLACE,
		[Description("")]
		CAN_FILTER_ADD,
		[Description("")]
		CAN_FILTER_REMOVE
	}

	public enum MAV_FTP_ERR
	{
		[Description("None: No error")]
		NONE,
		[Description("Fail: Unknown failure")]
		FAIL,
		[Description("FailErrno: Command failed, Err number sent back in PayloadHeader.data[1]. \t\tThis is a file-system error number understood by the server operating system.")]
		FAILERRNO,
		[Description("InvalidDataSize: Payload size is invalid")]
		INVALIDDATASIZE,
		[Description("InvalidSession: Session is not currently open")]
		INVALIDSESSION,
		[Description("NoSessionsAvailable: All available sessions are already in use")]
		NOSESSIONSAVAILABLE,
		[Description("EOF: Offset past end of file for ListDirectory and ReadFile commands")]
		EOF,
		[Description("UnknownCommand: Unknown command / opcode")]
		UNKNOWNCOMMAND,
		[Description("FileExists: File/directory already exists")]
		FILEEXISTS,
		[Description("FileProtected: File/directory is write protected")]
		FILEPROTECTED,
		[Description("FileNotFound: File/directory not found")]
		FILENOTFOUND
	}

	public enum MAV_FTP_OPCODE
	{
		[Description("None. Ignored, always ACKed")]
		NONE = 0,
		[Description("TerminateSession: Terminates open Read session")]
		TERMINATESESSION = 1,
		[Description("ResetSessions: Terminates all open read sessions")]
		RESETSESSION = 2,
		[Description("ListDirectory. List files and directories in path from offset")]
		LISTDIRECTORY = 3,
		[Description("OpenFileRO: Opens file at path for reading, returns session")]
		OPENFILERO = 4,
		[Description("ReadFile: Reads size bytes from offset in session")]
		READFILE = 5,
		[Description("CreateFile: Creates file at path for writing, returns session")]
		CREATEFILE = 6,
		[Description("WriteFile: Writes size bytes to offset in session")]
		WRITEFILE = 7,
		[Description("RemoveFile: Remove file at path")]
		REMOVEFILE = 8,
		[Description("CreateDirectory: Creates directory at path")]
		CREATEDIRECTORY = 9,
		[Description("RemoveDirectory: Removes directory at path. The directory must be empty.")]
		REMOVEDIRECTORY = 10,
		[Description("OpenFileWO: Opens file at path for writing, returns session")]
		OPENFILEWO = 11,
		[Description("TruncateFile: Truncate file at path to offset length")]
		TRUNCATEFILE = 12,
		[Description("Rename: Rename path1 to path2")]
		RENAME = 13,
		[Description("CalcFileCRC32: Calculate CRC32 for file at path")]
		CALCFILECRC = 14,
		[Description("BurstReadFile: Burst download session file")]
		BURSTREADFILE = 15,
		[Description("ACK: ACK response")]
		ACK = 128,
		[Description("NAK: NAK response")]
		NAK = 129
	}

	public enum MISSION_STATE : byte
	{
		[Description("The mission status reporting is not supported.")]
		UNKNOWN,
		[Description("No mission on the vehicle.")]
		NO_MISSION,
		[Description("Mission has not started. This is the case after a mission has uploaded but not yet started executing.")]
		NOT_STARTED,
		[Description("Mission is active, and will execute mission items when in auto mode.")]
		ACTIVE,
		[Description("Mission is paused when in auto mode.")]
		PAUSED,
		[Description("Mission has executed all mission items.")]
		COMPLETE
	}

	public enum SAFETY_SWITCH_STATE
	{
		[Description("Safety switch is engaged and vehicle should be safe to approach.")]
		SAFE,
		[Description("Safety switch is NOT engaged and motors, propellers and other actuators should be considered active.")]
		DANGEROUS
	}

	public enum ILLUMINATOR_MODE : byte
	{
		[Description("Illuminator mode is not specified/unknown")]
		UNKNOWN,
		[Description("Illuminator behavior is controlled by MAV_CMD_DO_ILLUMINATOR_CONFIGURE settings")]
		INTERNAL_CONTROL,
		[Description("Illuminator behavior is controlled by external factors: e.g. an external hardware signal")]
		EXTERNAL_SYNC
	}

	[Flags]
	public enum ILLUMINATOR_ERROR_FLAGS : uint
	{
		[Description("Illuminator thermal throttling error.")]
		THERMAL_THROTTLING = 1u,
		[Description("Illuminator over temperature shutdown error.")]
		OVER_TEMPERATURE_SHUTDOWN = 2u,
		[Description("Illuminator thermistor failure.")]
		THERMISTOR_FAILURE = 4u
	}

	public enum MAV_STANDARD_MODE : byte
	{
		[Description("Non standard mode.           This may be used when reporting the mode if the current flight mode is not a standard mode.         ")]
		NON_STANDARD,
		[Description("Position mode (manual).           Position-controlled and stabilized manual mode.           When sticks are released vehicles return to their level-flight orientation and hold both position and altitude against wind and external forces.           This mode can only be set by vehicles that can hold a fixed position.           Multicopter (MC) vehicles actively brake and hold both position and altitude against wind and external forces.           Hybrid MC/FW ('VTOL') vehicles first transition to multicopter mode (if needed) but otherwise behave in the same way as MC vehicles.           Fixed-wing (FW) vehicles must not support this mode.           Other vehicle types must not support this mode (this may be revisited through the PR process).         ")]
		POSITION_HOLD,
		[Description("Orbit (manual).           Position-controlled and stabilized manual mode.           The vehicle circles around a fixed setpoint in the horizontal plane at a particular radius, altitude, and direction.           Flight stacks may further allow manual control over the setpoint position, radius, direction, speed, and/or altitude of the circle, but this is not mandated.           Flight stacks may support the [MAV_CMD_DO_ORBIT](https://mavlink.io/en/messages/common.html#MAV_CMD_DO_ORBIT) for changing the orbit parameters.           MC and FW vehicles may support this mode.           Hybrid MC/FW ('VTOL') vehicles may support this mode in MC/FW or both modes; if the mode is not supported by the current configuration the vehicle should transition to the supported configuration.           Other vehicle types must not support this mode (this may be revisited through the PR process).         ")]
		ORBIT,
		[Description("Cruise mode (manual).           Position-controlled and stabilized manual mode.           When sticks are released vehicles return to their level-flight orientation and hold their original track against wind and external forces.           Fixed-wing (FW) vehicles level orientation and maintain current track and altitude against wind and external forces.           Hybrid MC/FW ('VTOL') vehicles first transition to FW mode (if needed) but otherwise behave in the same way as MC vehicles.           Multicopter (MC) vehicles must not support this mode.           Other vehicle types must not support this mode (this may be revisited through the PR process).         ")]
		CRUISE,
		[Description("Altitude hold (manual).           Altitude-controlled and stabilized manual mode.           When sticks are released vehicles return to their level-flight orientation and hold their altitude.           MC vehicles continue with existing momentum and may move with wind (or other external forces).           FW vehicles continue with current heading, but may be moved off-track by wind.           Hybrid MC/FW ('VTOL') vehicles behave according to their current configuration/mode (FW or MC).           Other vehicle types must not support this mode (this may be revisited through the PR process).         ")]
		ALTITUDE_HOLD,
		[Description("Safe recovery mode (auto).           Automatic mode that takes vehicle to a predefined safe location via a safe flight path, and may also automatically land the vehicle.           This mode is more commonly referred to as RTL and/or or Smart RTL.           The precise return location, flight path, and landing behaviour depend on vehicle configuration and type.           For example, the vehicle might return to the home/launch location, a rally point, or the start of a mission landing, it might follow a direct path, mission path, or breadcrumb path, and land using a mission landing pattern or some other kind of descent.         ")]
		SAFE_RECOVERY,
		[Description("Mission mode (automatic).           Automatic mode that executes MAVLink missions.           Missions are executed from the current waypoint as soon as the mode is enabled.         ")]
		MISSION,
		[Description("Land mode (auto).           Automatic mode that lands the vehicle at the current location.           The precise landing behaviour depends on vehicle configuration and type.         ")]
		LAND,
		[Description("Takeoff mode (auto).           Automatic takeoff mode.           The precise takeoff behaviour depends on vehicle configuration and type.         ")]
		TAKEOFF
	}

	public enum MAV_MODE_PROPERTY : uint
	{
		[Description("If set, this mode is an advanced mode.           For example a rate-controlled manual mode might be advanced, whereas a position-controlled manual mode is not.           A GCS can optionally use this flag to configure the UI for its intended users.         ")]
		ADVANCED = 1u,
		[Description("If set, this mode should not be added to the list of selectable modes.           The mode might still be selected by the FC directly (for example as part of a failsafe).         ")]
		NOT_USER_SELECTABLE = 2u,
		[Description("If set, this mode is automatically controlled (it may use but does not require a manual controller).           If unset the mode is a assumed to require user input (be a manual mode).         ")]
		AUTO_MODE = 4u
	}

	[Flags]
	public enum HIL_ACTUATOR_CONTROLS_FLAGS : ulong
	{
		[Description("Simulation is using lockstep")]
		LOCKSTEP = 1uL
	}

	public enum MAV_BOOL : sbyte
	{
		[Description("False.")]
		FALSE,
		[Description("True.")]
		TRUE
	}

	[Flags]
	public enum MAV_PROTOCOL_CAPABILITY : ulong
	{
		[Description("Autopilot supports the MISSION_ITEM float message type.           Note that MISSION_ITEM is deprecated, and autopilots should use MISSION_INT instead.         ")]
		MISSION_FLOAT = 1uL,
		[Description("Autopilot supports the new param float message type.")]
		PARAM_FLOAT = 2uL,
		[Description("Autopilot supports MISSION_ITEM_INT scaled integer message type.           Note that this flag must always be set if missions are supported, because missions must always use MISSION_ITEM_INT (rather than MISSION_ITEM, which is deprecated).         ")]
		MISSION_INT = 4uL,
		[Description("Autopilot supports COMMAND_INT scaled integer message type.")]
		COMMAND_INT = 8uL,
		[Description("Parameter protocol uses byte-wise encoding of parameter values into param_value (float) fields: https://mavlink.io/en/services/parameter.html#parameter-encoding.           Note that either this flag or MAV_PROTOCOL_CAPABILITY_PARAM_ENCODE_C_CAST should be set if the parameter protocol is supported.         ")]
		PARAM_ENCODE_BYTEWISE = 0x10uL,
		[Description("Autopilot supports the File Transfer Protocol v1: https://mavlink.io/en/services/ftp.html.")]
		FTP = 0x20uL,
		[Description("Autopilot supports commanding attitude offboard.")]
		SET_ATTITUDE_TARGET = 0x40uL,
		[Description("Autopilot supports commanding position and velocity targets in local NED frame.")]
		SET_POSITION_TARGET_LOCAL_NED = 0x80uL,
		[Description("Autopilot supports commanding position and velocity targets in global scaled integers.")]
		SET_POSITION_TARGET_GLOBAL_INT = 0x100uL,
		[Description("Autopilot supports terrain protocol / data handling.")]
		TERRAIN = 0x200uL,
		[Description("Reserved for future use.")]
		RESERVED3 = 0x400uL,
		[Description("Autopilot supports the MAV_CMD_DO_FLIGHTTERMINATION command (flight termination).")]
		FLIGHT_TERMINATION = 0x800uL,
		[Description("Autopilot supports onboard compass calibration.")]
		COMPASS_CALIBRATION = 0x1000uL,
		[Description("Autopilot supports MAVLink version 2.")]
		MAVLINK2 = 0x2000uL,
		[Description("Autopilot supports mission fence protocol.")]
		MISSION_FENCE = 0x4000uL,
		[Description("Autopilot supports mission rally point protocol.")]
		MISSION_RALLY = 0x8000uL,
		[Description("Reserved for future use.")]
		RESERVED2 = 0x10000uL,
		[Description("Parameter protocol uses C-cast of parameter values to set the param_value (float) fields: https://mavlink.io/en/services/parameter.html#parameter-encoding.           Note that either this flag or MAV_PROTOCOL_CAPABILITY_PARAM_ENCODE_BYTEWISE should be set if the parameter protocol is supported.         ")]
		PARAM_ENCODE_C_CAST = 0x20000uL,
		[Description("This component implements/is a gimbal manager. This means the GIMBAL_MANAGER_INFORMATION, and other messages can be requested.         ")]
		COMPONENT_IMPLEMENTS_GIMBAL_MANAGER = 0x40000uL,
		[Description("Component supports locking control to a particular GCS independent of its system (via MAV_CMD_REQUEST_OPERATOR_CONTROL).")]
		COMPONENT_ACCEPTS_GCS_CONTROL = 0x80000uL,
		[Description("Autopilot has a connected gripper. MAVLink Grippers would set MAV_TYPE_GRIPPER instead.")]
		GRIPPER = 0x100000uL
	}

	public enum FIRMWARE_VERSION_TYPE
	{
		[Description("development release")]
		DEV = 0,
		[Description("alpha release")]
		ALPHA = 64,
		[Description("beta release")]
		BETA = 128,
		[Description("release candidate")]
		RC = 192,
		[Description("official stable release")]
		OFFICIAL = 255
	}

	public enum MAV_AUTOPILOT : byte
	{
		[Description("Generic autopilot, full support for everything")]
		GENERIC,
		[Description("Reserved for future use.")]
		RESERVED,
		[Description("SLUGS autopilot, http://slugsuav.soe.ucsc.edu")]
		SLUGS,
		[Description("ArduPilot - Plane/Copter/Rover/Sub/Tracker, https://ardupilot.org")]
		ARDUPILOTMEGA,
		[Description("OpenPilot, http://openpilot.org")]
		OPENPILOT,
		[Description("Generic autopilot only supporting simple waypoints")]
		GENERIC_WAYPOINTS_ONLY,
		[Description("Generic autopilot supporting waypoints and other simple navigation commands")]
		GENERIC_WAYPOINTS_AND_SIMPLE_NAVIGATION_ONLY,
		[Description("Generic autopilot supporting the full mission command set")]
		GENERIC_MISSION_FULL,
		[Description("No valid autopilot, e.g. a GCS or other MAVLink component")]
		INVALID,
		[Description("PPZ UAV - http://nongnu.org/paparazzi")]
		PPZ,
		[Description("UAV Dev Board")]
		UDB,
		[Description("FlexiPilot")]
		FP,
		[Description("PX4 Autopilot - http://px4.io/")]
		PX4,
		[Description("SMACCMPilot - http://smaccmpilot.org")]
		SMACCMPILOT,
		[Description("AutoQuad -- http://autoquad.org")]
		AUTOQUAD,
		[Description("Armazila -- http://armazila.com")]
		ARMAZILA,
		[Description("Aerob -- http://aerob.ru")]
		AEROB,
		[Description("ASLUAV autopilot -- http://www.asl.ethz.ch")]
		ASLUAV,
		[Description("SmartAP Autopilot - http://sky-drones.com")]
		SMARTAP,
		[Description("AirRails - http://uaventure.com")]
		AIRRAILS,
		[Description("Fusion Reflex - https://fusion.engineering")]
		REFLEX
	}

	public enum MAV_TYPE : byte
	{
		[Description("Generic micro air vehicle")]
		GENERIC,
		[Description("Fixed wing aircraft.")]
		FIXED_WING,
		[Description("Quadrotor")]
		QUADROTOR,
		[Description("Coaxial helicopter")]
		COAXIAL,
		[Description("Normal helicopter with tail rotor.")]
		HELICOPTER,
		[Description("Ground installation")]
		ANTENNA_TRACKER,
		[Description("Operator control unit / ground control station")]
		GCS,
		[Description("Airship, controlled")]
		AIRSHIP,
		[Description("Free balloon, uncontrolled")]
		FREE_BALLOON,
		[Description("Rocket")]
		ROCKET,
		[Description("Ground rover")]
		GROUND_ROVER,
		[Description("Surface vessel, boat, ship")]
		SURFACE_BOAT,
		[Description("Submarine")]
		SUBMARINE,
		[Description("Hexarotor")]
		HEXAROTOR,
		[Description("Octorotor")]
		OCTOROTOR,
		[Description("Tricopter")]
		TRICOPTER,
		[Description("Flapping wing")]
		FLAPPING_WING,
		[Description("Kite")]
		KITE,
		[Description("Onboard companion controller")]
		ONBOARD_CONTROLLER,
		[Description("Two-rotor Tailsitter VTOL that additionally uses control surfaces in vertical operation. Note, value previously named MAV_TYPE_VTOL_DUOROTOR.")]
		VTOL_TAILSITTER_DUOROTOR,
		[Description("Quad-rotor Tailsitter VTOL using a V-shaped quad config in vertical operation. Note: value previously named MAV_TYPE_VTOL_QUADROTOR.")]
		VTOL_TAILSITTER_QUADROTOR,
		[Description("Tiltrotor VTOL. Fuselage and wings stay (nominally) horizontal in all flight phases. It able to tilt (some) rotors to provide thrust in cruise flight.")]
		VTOL_TILTROTOR,
		[Description("VTOL with separate fixed rotors for hover and cruise flight. Fuselage and wings stay (nominally) horizontal in all flight phases.")]
		VTOL_FIXEDROTOR,
		[Description("Tailsitter VTOL. Fuselage and wings orientation changes depending on flight phase: vertical for hover, horizontal for cruise. Use more specific VTOL MAV_TYPE_VTOL_TAILSITTER_DUOROTOR or MAV_TYPE_VTOL_TAILSITTER_QUADROTOR if appropriate.")]
		VTOL_TAILSITTER,
		[Description("Tiltwing VTOL. Fuselage stays horizontal in all flight phases. The whole wing, along with any attached engine, can tilt between vertical and horizontal mode.")]
		VTOL_TILTWING,
		[Description("VTOL reserved 5")]
		VTOL_RESERVED5,
		[Description("Gimbal")]
		GIMBAL,
		[Description("ADSB system")]
		ADSB,
		[Description("Steerable, nonrigid airfoil")]
		PARAFOIL,
		[Description("Dodecarotor")]
		DODECAROTOR,
		[Description("Camera")]
		CAMERA,
		[Description("Charging station")]
		CHARGING_STATION,
		[Description("FLARM collision avoidance system")]
		FLARM,
		[Description("Servo")]
		SERVO,
		[Description("Open Drone ID. See https://mavlink.io/en/services/opendroneid.html.")]
		ODID,
		[Description("Decarotor")]
		DECAROTOR,
		[Description("Battery")]
		BATTERY,
		[Description("Parachute")]
		PARACHUTE,
		[Description("Log")]
		LOG,
		[Description("OSD")]
		OSD,
		[Description("IMU")]
		IMU,
		[Description("GPS")]
		GPS,
		[Description("Winch")]
		WINCH,
		[Description("Generic multirotor that does not fit into a specific type or whose type is unknown")]
		GENERIC_MULTIROTOR,
		[Description("Illuminator. An illuminator is a light source that is used for lighting up dark areas external to the system: e.g. a torch or searchlight (as opposed to a light source for illuminating the system itself, e.g. an indicator light).")]
		ILLUMINATOR,
		[Description("Orbiter spacecraft. Includes satellites orbiting terrestrial and extra-terrestrial bodies. Follows NASA Spacecraft Classification.")]
		SPACECRAFT_ORBITER,
		[Description("A generic four-legged ground vehicle (e.g., a robot dog).")]
		GROUND_QUADRUPED,
		[Description("VTOL hybrid of helicopter and autogyro. It has a main rotor for lift and separate propellers for forward flight. The rotor must be powered for hover but can autorotate in cruise flight. See: https://en.wikipedia.org/wiki/Gyrodyne")]
		VTOL_GYRODYNE,
		[Description("Gripper")]
		GRIPPER
	}

	public enum MAV_MODE_FLAG : byte
	{
		[Description("0b00000001 system-specific custom mode is enabled. When using this flag to enable a custom mode all other flags should be ignored.")]
		CUSTOM_MODE_ENABLED = 1,
		[Description("0b00000010 system has a test mode enabled. This flag is intended for temporary system tests and should not be used for stable implementations.")]
		TEST_ENABLED = 2,
		[Description("0b00000100 autonomous mode enabled, system finds its own goal positions. Guided flag can be set or not, depends on the actual implementation.")]
		AUTO_ENABLED = 4,
		[Description("0b00001000 guided mode enabled, system flies waypoints / mission items.")]
		GUIDED_ENABLED = 8,
		[Description("0b00010000 system stabilizes electronically its attitude (and optionally position). It needs however further control inputs to move around.")]
		STABILIZE_ENABLED = 0x10,
		[Description("0b00100000 hardware in the loop simulation. All motors / actuators are blocked, but internal software is full operational.")]
		HIL_ENABLED = 0x20,
		[Description("0b01000000 remote control input is enabled.")]
		MANUAL_INPUT_ENABLED = 0x40,
		[Description("0b10000000 MAV safety set to armed. Motors are enabled / running / can start. Ready to fly. Additional note: this flag is to be ignore when sent in the command MAV_CMD_DO_SET_MODE and MAV_CMD_COMPONENT_ARM_DISARM shall be used instead. The flag can still be used to report the armed state.")]
		SAFETY_ARMED = 0x80
	}

	public enum MAV_MODE_FLAG_DECODE_POSITION
	{
		[Description("Eighth bit: 00000001")]
		CUSTOM_MODE = 1,
		[Description("Seventh bit: 00000010")]
		TEST = 2,
		[Description("Sixth bit:   00000100")]
		AUTO = 4,
		[Description("Fifth bit:  00001000")]
		GUIDED = 8,
		[Description("Fourth bit: 00010000")]
		STABILIZE = 0x10,
		[Description("Third bit:  00100000")]
		HIL = 0x20,
		[Description("Second bit: 01000000")]
		MANUAL = 0x40,
		[Description("First bit:  10000000")]
		SAFETY = 0x80
	}

	public enum MAV_STATE : byte
	{
		[Description("Uninitialized system, state is unknown.")]
		UNINIT,
		[Description("System is booting up.")]
		BOOT,
		[Description("System is calibrating and not flight-ready.")]
		CALIBRATING,
		[Description("System is grounded and on standby. It can be launched any time.")]
		STANDBY,
		[Description("System is active and might be already airborne. Motors are engaged.")]
		ACTIVE,
		[Description("System is in a non-normal flight mode (failsafe). It can however still navigate.")]
		CRITICAL,
		[Description("System is in a non-normal flight mode (failsafe). It lost control over parts or over the whole airframe. It is in mayday and going down.")]
		EMERGENCY,
		[Description("System just initialized its power-down sequence, will shut down now.")]
		POWEROFF,
		[Description("System is terminating itself (failsafe or commanded).")]
		FLIGHT_TERMINATION
	}

	public enum MAV_COMPONENT
	{
		[Description("Target id (target_component) used to broadcast messages to all components of the receiving system. Components should attempt to process messages with this component ID and forward to components on any other interfaces. Note: This is not a valid *source* component id for a message.")]
		MAV_COMP_ID_ALL = 0,
		[Description("System flight controller component ('autopilot'). Only one autopilot is expected in a particular system.")]
		MAV_COMP_ID_AUTOPILOT1 = 1,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER1 = 25,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER2 = 26,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER3 = 27,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER4 = 28,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER5 = 29,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER6 = 30,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER7 = 31,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER8 = 32,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER9 = 33,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER10 = 34,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER11 = 35,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER12 = 36,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER13 = 37,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER14 = 38,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER15 = 39,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER16 = 40,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER17 = 41,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER18 = 42,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER19 = 43,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER20 = 44,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER21 = 45,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER22 = 46,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER23 = 47,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER24 = 48,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER25 = 49,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER26 = 50,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER27 = 51,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER28 = 52,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER29 = 53,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER30 = 54,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER31 = 55,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER32 = 56,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER33 = 57,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER34 = 58,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER35 = 59,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER36 = 60,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER37 = 61,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER38 = 62,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER39 = 63,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER40 = 64,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER41 = 65,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER42 = 66,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER43 = 67,
		[Description("Telemetry radio (e.g. SiK radio, or other component that emits RADIO_STATUS messages).")]
		MAV_COMP_ID_TELEMETRY_RADIO = 68,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER45 = 69,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER46 = 70,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER47 = 71,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER48 = 72,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER49 = 73,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER50 = 74,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER51 = 75,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER52 = 76,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER53 = 77,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER54 = 78,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER55 = 79,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER56 = 80,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER57 = 81,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER58 = 82,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER59 = 83,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER60 = 84,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER61 = 85,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER62 = 86,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER63 = 87,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER64 = 88,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER65 = 89,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER66 = 90,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER67 = 91,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER68 = 92,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER69 = 93,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER70 = 94,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER71 = 95,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER72 = 96,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER73 = 97,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER74 = 98,
		[Description("Id for a component on privately managed MAVLink network. Can be used for any purpose but may not be published by components outside of the private network.")]
		MAV_COMP_ID_USER75 = 99,
		[Description("Camera #1.")]
		MAV_COMP_ID_CAMERA = 100,
		[Description("Camera #2.")]
		MAV_COMP_ID_CAMERA2 = 101,
		[Description("Camera #3.")]
		MAV_COMP_ID_CAMERA3 = 102,
		[Description("Camera #4.")]
		MAV_COMP_ID_CAMERA4 = 103,
		[Description("Camera #5.")]
		MAV_COMP_ID_CAMERA5 = 104,
		[Description("Camera #6.")]
		MAV_COMP_ID_CAMERA6 = 105,
		[Description("Servo #1.")]
		MAV_COMP_ID_SERVO1 = 140,
		[Description("Servo #2.")]
		MAV_COMP_ID_SERVO2 = 141,
		[Description("Servo #3.")]
		MAV_COMP_ID_SERVO3 = 142,
		[Description("Servo #4.")]
		MAV_COMP_ID_SERVO4 = 143,
		[Description("Servo #5.")]
		MAV_COMP_ID_SERVO5 = 144,
		[Description("Servo #6.")]
		MAV_COMP_ID_SERVO6 = 145,
		[Description("Servo #7.")]
		MAV_COMP_ID_SERVO7 = 146,
		[Description("Servo #8.")]
		MAV_COMP_ID_SERVO8 = 147,
		[Description("Servo #9.")]
		MAV_COMP_ID_SERVO9 = 148,
		[Description("Servo #10.")]
		MAV_COMP_ID_SERVO10 = 149,
		[Description("Servo #11.")]
		MAV_COMP_ID_SERVO11 = 150,
		[Description("Servo #12.")]
		MAV_COMP_ID_SERVO12 = 151,
		[Description("Servo #13.")]
		MAV_COMP_ID_SERVO13 = 152,
		[Description("Servo #14.")]
		MAV_COMP_ID_SERVO14 = 153,
		[Description("Gimbal #1.")]
		MAV_COMP_ID_GIMBAL = 154,
		[Description("Logging component.")]
		MAV_COMP_ID_LOG = 155,
		[Description("Automatic Dependent Surveillance-Broadcast (ADS-B) component.")]
		MAV_COMP_ID_ADSB = 156,
		[Description("On Screen Display (OSD) devices for video links.")]
		MAV_COMP_ID_OSD = 157,
		[Description("Generic autopilot peripheral component ID. Meant for devices that do not implement the parameter microservice.")]
		MAV_COMP_ID_PERIPHERAL = 158,
		[Description("Gimbal ID for QX1.")]
		MAV_COMP_ID_QX1_GIMBAL = 159,
		[Description("FLARM collision alert component.")]
		MAV_COMP_ID_FLARM = 160,
		[Description("Parachute component.")]
		MAV_COMP_ID_PARACHUTE = 161,
		[Description("Winch component.")]
		MAV_COMP_ID_WINCH = 169,
		[Description("Gimbal #2.")]
		MAV_COMP_ID_GIMBAL2 = 171,
		[Description("Gimbal #3.")]
		MAV_COMP_ID_GIMBAL3 = 172,
		[Description("Gimbal #4")]
		MAV_COMP_ID_GIMBAL4 = 173,
		[Description("Gimbal #5.")]
		MAV_COMP_ID_GIMBAL5 = 174,
		[Description("Gimbal #6.")]
		MAV_COMP_ID_GIMBAL6 = 175,
		[Description("Battery #1.")]
		MAV_COMP_ID_BATTERY = 180,
		[Description("Battery #2.")]
		MAV_COMP_ID_BATTERY2 = 181,
		[Description("CAN over MAVLink client.")]
		MAV_COMP_ID_MAVCAN = 189,
		[Description("Component that can generate/supply a mission flight plan (e.g. GCS or developer API).")]
		MAV_COMP_ID_MISSIONPLANNER = 190,
		[Description("Component that lives on the onboard computer (companion computer) and has some generic functionalities, such as settings system parameters and monitoring the status of some processes that don't directly speak mavlink and so on.")]
		MAV_COMP_ID_ONBOARD_COMPUTER = 191,
		[Description("Component that lives on the onboard computer (companion computer) and has some generic functionalities, such as settings system parameters and monitoring the status of some processes that don't directly speak mavlink and so on.")]
		MAV_COMP_ID_ONBOARD_COMPUTER2 = 192,
		[Description("Component that lives on the onboard computer (companion computer) and has some generic functionalities, such as settings system parameters and monitoring the status of some processes that don't directly speak mavlink and so on.")]
		MAV_COMP_ID_ONBOARD_COMPUTER3 = 193,
		[Description("Component that lives on the onboard computer (companion computer) and has some generic functionalities, such as settings system parameters and monitoring the status of some processes that don't directly speak mavlink and so on.")]
		MAV_COMP_ID_ONBOARD_COMPUTER4 = 194,
		[Description("Component that finds an optimal path between points based on a certain constraint (e.g. minimum snap, shortest path, cost, etc.).")]
		MAV_COMP_ID_PATHPLANNER = 195,
		[Description("Component that plans a collision free path between two points.")]
		MAV_COMP_ID_OBSTACLE_AVOIDANCE = 196,
		[Description("Component that provides position estimates using VIO techniques.")]
		MAV_COMP_ID_VISUAL_INERTIAL_ODOMETRY = 197,
		[Description("Component that manages pairing of vehicle and GCS.")]
		MAV_COMP_ID_PAIRING_MANAGER = 198,
		[Description("Inertial Measurement Unit (IMU) #1.")]
		MAV_COMP_ID_IMU = 200,
		[Description("Inertial Measurement Unit (IMU) #2.")]
		MAV_COMP_ID_IMU_2 = 201,
		[Description("Inertial Measurement Unit (IMU) #3.")]
		MAV_COMP_ID_IMU_3 = 202,
		[Description("GPS #1.")]
		MAV_COMP_ID_GPS = 220,
		[Description("GPS #2.")]
		MAV_COMP_ID_GPS2 = 221,
		[Description("Open Drone ID transmitter/receiver (Bluetooth/WiFi/Internet).")]
		MAV_COMP_ID_ODID_TXRX_1 = 236,
		[Description("Open Drone ID transmitter/receiver (Bluetooth/WiFi/Internet).")]
		MAV_COMP_ID_ODID_TXRX_2 = 237,
		[Description("Open Drone ID transmitter/receiver (Bluetooth/WiFi/Internet).")]
		MAV_COMP_ID_ODID_TXRX_3 = 238,
		[Description("Component to bridge MAVLink to UDP (i.e. from a UART).")]
		MAV_COMP_ID_UDP_BRIDGE = 240,
		[Description("Component to bridge to UART (i.e. from UDP).")]
		MAV_COMP_ID_UART_BRIDGE = 241,
		[Description("Component handling TUNNEL messages (e.g. vendor specific GUI of a component).")]
		MAV_COMP_ID_TUNNEL_NODE = 242,
		[Description("Illuminator")]
		MAV_COMP_ID_ILLUMINATOR = 243,
		[Description("Deprecated, don't use. Component for handling system messages (e.g. to ARM, takeoff, etc.).")]
		MAV_COMP_ID_SYSTEM_CONTROL = 250
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 43)]
	public struct mavlink_sys_status_t(uint onboard_control_sensors_present, uint onboard_control_sensors_enabled, uint onboard_control_sensors_health, ushort load, ushort voltage_battery, short current_battery, ushort drop_rate_comm, ushort errors_comm, ushort errors_count1, ushort errors_count2, ushort errors_count3, ushort errors_count4, sbyte battery_remaining, uint onboard_control_sensors_present_extended, uint onboard_control_sensors_enabled_extended, uint onboard_control_sensors_health_extended)
	{
		[Units("")]
		[Description("Bitmap showing which onboard controllers and sensors are present. Value of 0: not present. Value of 1: present.")]
		public uint onboard_control_sensors_present = onboard_control_sensors_present;

		[Units("")]
		[Description("Bitmap showing which onboard controllers and sensors are enabled:  Value of 0: not enabled. Value of 1: enabled.")]
		public uint onboard_control_sensors_enabled = onboard_control_sensors_enabled;

		[Units("")]
		[Description("Bitmap showing which onboard controllers and sensors have an error (or are operational). Value of 0: error. Value of 1: healthy.")]
		public uint onboard_control_sensors_health = onboard_control_sensors_health;

		[Units("[d%]")]
		[Description("Maximum usage in percent of the mainloop time. Values: [0-1000] - should always be below 1000")]
		public ushort load = load;

		[Units("[mV]")]
		[Description("Battery voltage, UINT16_MAX: Voltage not sent by autopilot")]
		public ushort voltage_battery = voltage_battery;

		[Units("[cA]")]
		[Description("Battery current, -1: Current not sent by autopilot")]
		public short current_battery = current_battery;

		[Units("[c%]")]
		[Description("Communication drop rate, (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV)")]
		public ushort drop_rate_comm = drop_rate_comm;

		[Units("")]
		[Description("Communication errors (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV)")]
		public ushort errors_comm = errors_comm;

		[Units("")]
		[Description("Autopilot-specific errors")]
		public ushort errors_count1 = errors_count1;

		[Units("")]
		[Description("Autopilot-specific errors")]
		public ushort errors_count2 = errors_count2;

		[Units("")]
		[Description("Autopilot-specific errors")]
		public ushort errors_count3 = errors_count3;

		[Units("")]
		[Description("Autopilot-specific errors")]
		public ushort errors_count4 = errors_count4;

		[Units("[%]")]
		[Description("Battery energy remaining, -1: Battery remaining energy not sent by autopilot")]
		public sbyte battery_remaining = battery_remaining;

		[Units("")]
		[Description("Bitmap showing which onboard controllers and sensors are present. Value of 0: not present. Value of 1: present.")]
		public uint onboard_control_sensors_present_extended = onboard_control_sensors_present_extended;

		[Units("")]
		[Description("Bitmap showing which onboard controllers and sensors are enabled:  Value of 0: not enabled. Value of 1: enabled.")]
		public uint onboard_control_sensors_enabled_extended = onboard_control_sensors_enabled_extended;

		[Units("")]
		[Description("Bitmap showing which onboard controllers and sensors have an error (or are operational). Value of 0: error. Value of 1: healthy.")]
		public uint onboard_control_sensors_health_extended = onboard_control_sensors_health_extended;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
	public struct mavlink_system_time_t(ulong time_unix_usec, uint time_boot_ms)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX epoch time).")]
		public ulong time_unix_usec = time_unix_usec;

		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 14)]
	public struct mavlink_ping_t(ulong time_usec, uint seq, byte target_system, byte target_component)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("PING sequence")]
		public uint seq = seq;

		[Units("")]
		[Description("0: request ping from all receiving systems. If greater than 0: message is a ping response and number is the system id of the requesting system")]
		public byte target_system = target_system;

		[Units("")]
		[Description("0: request ping from all receiving components. If greater than 0: message is a ping response and number is the component id of the requesting component.")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
	public struct mavlink_change_operator_control_t(byte target_system, byte control_request, byte version, byte[] passkey)
	{
		[Units("")]
		[Description("System the GCS requests control for")]
		public byte target_system = target_system;

		[Units("")]
		[Description("0: request control of this MAV, 1: Release control of this MAV")]
		public byte control_request = control_request;

		[Units("[rad]")]
		[Description("0: key as plaintext, 1-255: future, different hashing/encryption variants. The GCS should in general use the safest mode possible initially and then gradually move down the encryption level if it gets a NACK message indicating an encryption mismatch.")]
		public byte version = version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
		[Units("")]
		[Description("Password / Key, depending on version plaintext or encrypted. 25 or less characters, NULL terminated. The characters may involve A-Z, a-z, 0-9, and '!?,.-'")]
		public byte[] passkey = passkey;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 3)]
	public struct mavlink_change_operator_control_ack_t(byte gcs_system_id, byte control_request, byte ack)
	{
		[Units("")]
		[Description("ID of the GCS this message ")]
		public byte gcs_system_id = gcs_system_id;

		[Units("")]
		[Description("0: request control of this MAV, 1: Release control of this MAV")]
		public byte control_request = control_request;

		[Units("")]
		[Description("0: ACK, 1: NACK: Wrong passkey, 2: NACK: Unsupported passkey encryption method, 3: NACK: Already under control")]
		public byte ack = ack;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
	public struct mavlink_auth_key_t(byte[] key)
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("key")]
		public byte[] key = key;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 36)]
	public struct mavlink_link_node_status_t(ulong timestamp, uint tx_rate, uint rx_rate, uint messages_sent, uint messages_received, uint messages_lost, ushort rx_parse_err, ushort tx_overflows, ushort rx_overflows, byte tx_buf, byte rx_buf)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public ulong timestamp = timestamp;

		[Units("[bytes/s]")]
		[Description("Transmit rate")]
		public uint tx_rate = tx_rate;

		[Units("[bytes/s]")]
		[Description("Receive rate")]
		public uint rx_rate = rx_rate;

		[Units("")]
		[Description("Messages sent")]
		public uint messages_sent = messages_sent;

		[Units("")]
		[Description("Messages received (estimated from counting seq)")]
		public uint messages_received = messages_received;

		[Units("")]
		[Description("Messages lost (estimated from counting seq)")]
		public uint messages_lost = messages_lost;

		[Units("[bytes]")]
		[Description("Number of bytes that could not be parsed correctly.")]
		public ushort rx_parse_err = rx_parse_err;

		[Units("[bytes]")]
		[Description("Transmit buffer overflows. This number wraps around as it reaches UINT16_MAX")]
		public ushort tx_overflows = tx_overflows;

		[Units("[bytes]")]
		[Description("Receive buffer overflows. This number wraps around as it reaches UINT16_MAX")]
		public ushort rx_overflows = rx_overflows;

		[Units("[%]")]
		[Description("Remaining free transmit buffer space")]
		public byte tx_buf = tx_buf;

		[Units("[%]")]
		[Description("Remaining free receive buffer space")]
		public byte rx_buf = rx_buf;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
	public struct mavlink_set_mode_t(uint custom_mode, byte target_system, byte base_mode)
	{
		[Units("")]
		[Description("The new autopilot-specific mode. This field can be ignored by an autopilot.")]
		public uint custom_mode = custom_mode;

		[Units("")]
		[Description("The system setting the mode")]
		public byte target_system = target_system;

		[Units("")]
		[Description("The new base mode.")]
		public byte base_mode = base_mode;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
	public struct mavlink_param_request_read_t(short param_index, byte target_system, byte target_component, byte[] param_id)
	{
		[Units("")]
		[Description("Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored)")]
		public short param_index = param_index;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
		public byte[] param_id = param_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
	public struct mavlink_param_request_list_t(byte target_system, byte target_component)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 25)]
	public struct mavlink_param_value_t(float param_value, ushort param_count, ushort param_index, byte[] param_id, byte param_type)
	{
		[Units("")]
		[Description("Onboard parameter value")]
		public float param_value = param_value;

		[Units("")]
		[Description("Total number of onboard parameters")]
		public ushort param_count = param_count;

		[Units("")]
		[Description("Index of this onboard parameter")]
		public ushort param_index = param_index;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
		public byte[] param_id = param_id;

		[Units("")]
		[Description("Onboard parameter type.")]
		public byte param_type = param_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 23)]
	public struct mavlink_param_set_t(float param_value, byte target_system, byte target_component, byte[] param_id, byte param_type)
	{
		[Units("")]
		[Description("Onboard parameter value")]
		public float param_value = param_value;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
		public byte[] param_id = param_id;

		[Units("")]
		[Description("Onboard parameter type.")]
		public byte param_type = param_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 52)]
	public struct mavlink_gps_raw_int_t(ulong time_usec, int lat, int lon, int alt, ushort eph, ushort epv, ushort vel, ushort cog, byte fix_type, byte satellites_visible, int alt_ellipsoid, uint h_acc, uint v_acc, uint vel_acc, uint hdg_acc, ushort yaw)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[degE7]")]
		[Description("Latitude (WGS84, EGM96 ellipsoid)")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude (WGS84, EGM96 ellipsoid)")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude (MSL). Positive for up. Note that virtually all GPS modules provide the MSL altitude in addition to the WGS84 altitude.")]
		public int alt = alt;

		[Units("")]
		[Description("GPS HDOP horizontal dilution of position (unitless * 100). If unknown, set to: UINT16_MAX")]
		public ushort eph = eph;

		[Units("")]
		[Description("GPS VDOP vertical dilution of position (unitless * 100). If unknown, set to: UINT16_MAX")]
		public ushort epv = epv;

		[Units("[cm/s]")]
		[Description("GPS ground speed. If unknown, set to: UINT16_MAX")]
		public ushort vel = vel;

		[Units("[cdeg]")]
		[Description("Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX")]
		public ushort cog = cog;

		[Units("")]
		[Description("GPS fix type.")]
		public byte fix_type = fix_type;

		[Units("")]
		[Description("Number of satellites visible. If unknown, set to UINT8_MAX")]
		public byte satellites_visible = satellites_visible;

		[Units("[mm]")]
		[Description("Altitude (above WGS84, EGM96 ellipsoid). Positive for up.")]
		public int alt_ellipsoid = alt_ellipsoid;

		[Units("[mm]")]
		[Description("Position uncertainty.")]
		public uint h_acc = h_acc;

		[Units("[mm]")]
		[Description("Altitude uncertainty.")]
		public uint v_acc = v_acc;

		[Units("[mm/s]")]
		[Description("Speed uncertainty.")]
		public uint vel_acc = vel_acc;

		[Units("[degE5]")]
		[Description("Heading / track uncertainty")]
		public uint hdg_acc = hdg_acc;

		[Units("[cdeg]")]
		[Description("Yaw in earth frame from north. Use 0 if this GPS does not provide yaw. Use UINT16_MAX if this GPS is configured to provide yaw and is currently unable to provide it. Use 36000 for north.")]
		public ushort yaw = yaw;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 101)]
	public struct mavlink_gps_status_t(byte satellites_visible, byte[] satellite_prn, byte[] satellite_used, byte[] satellite_elevation, byte[] satellite_azimuth, byte[] satellite_snr)
	{
		[Units("")]
		[Description("Number of satellites visible")]
		public byte satellites_visible = satellites_visible;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("Global satellite ID")]
		public byte[] satellite_prn = satellite_prn;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("0: Satellite not used, 1: used for localization")]
		public byte[] satellite_used = satellite_used;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("[deg]")]
		[Description("Elevation (0: right on top of receiver, 90: on the horizon) of satellite")]
		public byte[] satellite_elevation = satellite_elevation;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("[deg]")]
		[Description("Direction of satellite, 0: 0 deg, 255: 360 deg.")]
		public byte[] satellite_azimuth = satellite_azimuth;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("[dB]")]
		[Description("Signal to noise ratio of satellite")]
		public byte[] satellite_snr = satellite_snr;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
	public struct mavlink_scaled_imu_t(uint time_boot_ms, short xacc, short yacc, short zacc, short xgyro, short ygyro, short zgyro, short xmag, short ymag, short zmag, short temperature)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[mG]")]
		[Description("X acceleration")]
		public short xacc = xacc;

		[Units("[mG]")]
		[Description("Y acceleration")]
		public short yacc = yacc;

		[Units("[mG]")]
		[Description("Z acceleration")]
		public short zacc = zacc;

		[Units("[mrad/s]")]
		[Description("Angular speed around X axis")]
		public short xgyro = xgyro;

		[Units("[mrad/s]")]
		[Description("Angular speed around Y axis")]
		public short ygyro = ygyro;

		[Units("[mrad/s]")]
		[Description("Angular speed around Z axis")]
		public short zgyro = zgyro;

		[Units("[mgauss]")]
		[Description("X Magnetic field")]
		public short xmag = xmag;

		[Units("[mgauss]")]
		[Description("Y Magnetic field")]
		public short ymag = ymag;

		[Units("[mgauss]")]
		[Description("Z Magnetic field")]
		public short zmag = zmag;

		[Units("[cdegC]")]
		[Description("Temperature, 0: IMU does not provide temperature values. If the IMU is at 0C it must send 1 (0.01C).")]
		public short temperature = temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 29)]
	public struct mavlink_raw_imu_t(ulong time_usec, short xacc, short yacc, short zacc, short xgyro, short ygyro, short zgyro, short xmag, short ymag, short zmag, byte id, short temperature)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("X acceleration (raw)")]
		public short xacc = xacc;

		[Units("")]
		[Description("Y acceleration (raw)")]
		public short yacc = yacc;

		[Units("")]
		[Description("Z acceleration (raw)")]
		public short zacc = zacc;

		[Units("")]
		[Description("Angular speed around X axis (raw)")]
		public short xgyro = xgyro;

		[Units("")]
		[Description("Angular speed around Y axis (raw)")]
		public short ygyro = ygyro;

		[Units("")]
		[Description("Angular speed around Z axis (raw)")]
		public short zgyro = zgyro;

		[Units("")]
		[Description("X Magnetic field (raw)")]
		public short xmag = xmag;

		[Units("")]
		[Description("Y Magnetic field (raw)")]
		public short ymag = ymag;

		[Units("")]
		[Description("Z Magnetic field (raw)")]
		public short zmag = zmag;

		[Units("")]
		[Description("Id. Ids are numbered from 0 and map to IMUs numbered from 1 (e.g. IMU1 will have a message with id=0)")]
		public byte id = id;

		[Units("[cdegC]")]
		[Description("Temperature, 0: IMU does not provide temperature values. If the IMU is at 0C it must send 1 (0.01C).")]
		public short temperature = temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct mavlink_raw_pressure_t(ulong time_usec, short press_abs, short press_diff1, short press_diff2, short temperature)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("Absolute pressure (raw)")]
		public short press_abs = press_abs;

		[Units("")]
		[Description("Differential pressure 1 (raw, 0 if nonexistent)")]
		public short press_diff1 = press_diff1;

		[Units("")]
		[Description("Differential pressure 2 (raw, 0 if nonexistent)")]
		public short press_diff2 = press_diff2;

		[Units("")]
		[Description("Raw Temperature measurement (raw)")]
		public short temperature = temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct mavlink_scaled_pressure_t(uint time_boot_ms, float press_abs, float press_diff, short temperature, short temperature_press_diff)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[hPa]")]
		[Description("Absolute pressure")]
		public float press_abs = press_abs;

		[Units("[hPa]")]
		[Description("Differential pressure 1")]
		public float press_diff = press_diff;

		[Units("[cdegC]")]
		[Description("Absolute pressure temperature")]
		public short temperature = temperature;

		[Units("[cdegC]")]
		[Description("Differential pressure temperature (0, if not available). Report values of 0 (or 1) as 1 cdegC.")]
		public short temperature_press_diff = temperature_press_diff;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
	public struct mavlink_attitude_t(uint time_boot_ms, float roll, float pitch, float yaw, float rollspeed, float pitchspeed, float yawspeed)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[rad]")]
		[Description("Roll angle (-pi..+pi)")]
		public float roll = roll;

		[Units("[rad]")]
		[Description("Pitch angle (-pi..+pi)")]
		public float pitch = pitch;

		[Units("[rad]")]
		[Description("Yaw angle (-pi..+pi)")]
		public float yaw = yaw;

		[Units("[rad/s]")]
		[Description("Roll angular speed")]
		public float rollspeed = rollspeed;

		[Units("[rad/s]")]
		[Description("Pitch angular speed")]
		public float pitchspeed = pitchspeed;

		[Units("[rad/s]")]
		[Description("Yaw angular speed")]
		public float yawspeed = yawspeed;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 48)]
	public struct mavlink_attitude_quaternion_t(uint time_boot_ms, float q1, float q2, float q3, float q4, float rollspeed, float pitchspeed, float yawspeed, float[] repr_offset_q)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("Quaternion component 1, w (1 in null-rotation)")]
		public float q1 = q1;

		[Units("")]
		[Description("Quaternion component 2, x (0 in null-rotation)")]
		public float q2 = q2;

		[Units("")]
		[Description("Quaternion component 3, y (0 in null-rotation)")]
		public float q3 = q3;

		[Units("")]
		[Description("Quaternion component 4, z (0 in null-rotation)")]
		public float q4 = q4;

		[Units("[rad/s]")]
		[Description("Roll angular speed")]
		public float rollspeed = rollspeed;

		[Units("[rad/s]")]
		[Description("Pitch angular speed")]
		public float pitchspeed = pitchspeed;

		[Units("[rad/s]")]
		[Description("Yaw angular speed")]
		public float yawspeed = yawspeed;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Rotation offset by which the attitude quaternion and angular speed vector should be rotated for user display (quaternion with [w, x, y, z] order, zero-rotation is [1, 0, 0, 0], send [0, 0, 0, 0] if field not supported). This field is intended for systems in which the reference attitude may change during flight. For example, tailsitters VTOLs rotate their reference attitude by 90 degrees between hover mode and fixed wing mode, thus repr_offset_q is equal to [1, 0, 0, 0] in hover mode and equal to [0.7071, 0, 0.7071, 0] in fixed wing mode.")]
		public float[] repr_offset_q = repr_offset_q;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
	public struct mavlink_local_position_ned_t(uint time_boot_ms, float x, float y, float z, float vx, float vy, float vz)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[m]")]
		[Description("X Position")]
		public float x = x;

		[Units("[m]")]
		[Description("Y Position")]
		public float y = y;

		[Units("[m]")]
		[Description("Z Position")]
		public float z = z;

		[Units("[m/s]")]
		[Description("X Speed")]
		public float vx = vx;

		[Units("[m/s]")]
		[Description("Y Speed")]
		public float vy = vy;

		[Units("[m/s]")]
		[Description("Z Speed")]
		public float vz = vz;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
	public struct mavlink_global_position_int_t(uint time_boot_ms, int lat, int lon, int alt, int relative_alt, short vx, short vy, short vz, ushort hdg)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[degE7]")]
		[Description("Latitude, expressed")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude, expressed")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude (MSL). Note that virtually all GPS modules provide both WGS84 and MSL.")]
		public int alt = alt;

		[Units("[mm]")]
		[Description("Altitude above home")]
		public int relative_alt = relative_alt;

		[Units("[cm/s]")]
		[Description("Ground X Speed (Latitude, positive north)")]
		public short vx = vx;

		[Units("[cm/s]")]
		[Description("Ground Y Speed (Longitude, positive east)")]
		public short vy = vy;

		[Units("[cm/s]")]
		[Description("Ground Z Speed (Altitude, positive down)")]
		public short vz = vz;

		[Units("[cdeg]")]
		[Description("Vehicle heading (yaw angle), 0.0..359.99 degrees. If unknown, set to: UINT16_MAX")]
		public ushort hdg = hdg;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 22)]
	public struct mavlink_rc_channels_scaled_t(uint time_boot_ms, short chan1_scaled, short chan2_scaled, short chan3_scaled, short chan4_scaled, short chan5_scaled, short chan6_scaled, short chan7_scaled, short chan8_scaled, byte port, byte rssi)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("RC channel 1 value scaled.")]
		public short chan1_scaled = chan1_scaled;

		[Units("")]
		[Description("RC channel 2 value scaled.")]
		public short chan2_scaled = chan2_scaled;

		[Units("")]
		[Description("RC channel 3 value scaled.")]
		public short chan3_scaled = chan3_scaled;

		[Units("")]
		[Description("RC channel 4 value scaled.")]
		public short chan4_scaled = chan4_scaled;

		[Units("")]
		[Description("RC channel 5 value scaled.")]
		public short chan5_scaled = chan5_scaled;

		[Units("")]
		[Description("RC channel 6 value scaled.")]
		public short chan6_scaled = chan6_scaled;

		[Units("")]
		[Description("RC channel 7 value scaled.")]
		public short chan7_scaled = chan7_scaled;

		[Units("")]
		[Description("RC channel 8 value scaled.")]
		public short chan8_scaled = chan8_scaled;

		[Units("")]
		[Description("Servo output port (set of 8 outputs = 1 port). Flight stacks running on Pixhawk should use: 0 = MAIN, 1 = AUX.")]
		public byte port = port;

		[Units("")]
		[Description("Receive signal strength indicator in device-dependent units/scale. Values: [0-254], UINT8_MAX: invalid/unknown.")]
		public byte rssi = rssi;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 22)]
	public struct mavlink_rc_channels_raw_t(uint time_boot_ms, ushort chan1_raw, ushort chan2_raw, ushort chan3_raw, ushort chan4_raw, ushort chan5_raw, ushort chan6_raw, ushort chan7_raw, ushort chan8_raw, byte port, byte rssi)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[us]")]
		[Description("RC channel 1 value.")]
		public ushort chan1_raw = chan1_raw;

		[Units("[us]")]
		[Description("RC channel 2 value.")]
		public ushort chan2_raw = chan2_raw;

		[Units("[us]")]
		[Description("RC channel 3 value.")]
		public ushort chan3_raw = chan3_raw;

		[Units("[us]")]
		[Description("RC channel 4 value.")]
		public ushort chan4_raw = chan4_raw;

		[Units("[us]")]
		[Description("RC channel 5 value.")]
		public ushort chan5_raw = chan5_raw;

		[Units("[us]")]
		[Description("RC channel 6 value.")]
		public ushort chan6_raw = chan6_raw;

		[Units("[us]")]
		[Description("RC channel 7 value.")]
		public ushort chan7_raw = chan7_raw;

		[Units("[us]")]
		[Description("RC channel 8 value.")]
		public ushort chan8_raw = chan8_raw;

		[Units("")]
		[Description("Servo output port (set of 8 outputs = 1 port). Flight stacks running on Pixhawk should use: 0 = MAIN, 1 = AUX.")]
		public byte port = port;

		[Units("")]
		[Description("Receive signal strength indicator in device-dependent units/scale. Values: [0-254], UINT8_MAX: invalid/unknown.")]
		public byte rssi = rssi;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 37)]
	public struct mavlink_servo_output_raw_t(uint time_usec, ushort servo1_raw, ushort servo2_raw, ushort servo3_raw, ushort servo4_raw, ushort servo5_raw, ushort servo6_raw, ushort servo7_raw, ushort servo8_raw, byte port, ushort servo9_raw, ushort servo10_raw, ushort servo11_raw, ushort servo12_raw, ushort servo13_raw, ushort servo14_raw, ushort servo15_raw, ushort servo16_raw)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public uint time_usec = time_usec;

		[Units("[us]")]
		[Description("Servo output 1 value")]
		public ushort servo1_raw = servo1_raw;

		[Units("[us]")]
		[Description("Servo output 2 value")]
		public ushort servo2_raw = servo2_raw;

		[Units("[us]")]
		[Description("Servo output 3 value")]
		public ushort servo3_raw = servo3_raw;

		[Units("[us]")]
		[Description("Servo output 4 value")]
		public ushort servo4_raw = servo4_raw;

		[Units("[us]")]
		[Description("Servo output 5 value")]
		public ushort servo5_raw = servo5_raw;

		[Units("[us]")]
		[Description("Servo output 6 value")]
		public ushort servo6_raw = servo6_raw;

		[Units("[us]")]
		[Description("Servo output 7 value")]
		public ushort servo7_raw = servo7_raw;

		[Units("[us]")]
		[Description("Servo output 8 value")]
		public ushort servo8_raw = servo8_raw;

		[Units("")]
		[Description("Servo output port (set of 8 outputs = 1 port). Flight stacks running on Pixhawk should use: 0 = MAIN, 1 = AUX.")]
		public byte port = port;

		[Units("[us]")]
		[Description("Servo output 9 value")]
		public ushort servo9_raw = servo9_raw;

		[Units("[us]")]
		[Description("Servo output 10 value")]
		public ushort servo10_raw = servo10_raw;

		[Units("[us]")]
		[Description("Servo output 11 value")]
		public ushort servo11_raw = servo11_raw;

		[Units("[us]")]
		[Description("Servo output 12 value")]
		public ushort servo12_raw = servo12_raw;

		[Units("[us]")]
		[Description("Servo output 13 value")]
		public ushort servo13_raw = servo13_raw;

		[Units("[us]")]
		[Description("Servo output 14 value")]
		public ushort servo14_raw = servo14_raw;

		[Units("[us]")]
		[Description("Servo output 15 value")]
		public ushort servo15_raw = servo15_raw;

		[Units("[us]")]
		[Description("Servo output 16 value")]
		public ushort servo16_raw = servo16_raw;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 7)]
	public struct mavlink_mission_request_partial_list_t(short start_index, short end_index, byte target_system, byte target_component, byte mission_type)
	{
		[Units("")]
		[Description("Start index")]
		public short start_index = start_index;

		[Units("")]
		[Description("End index, -1 by default (-1: send list to end). Else a valid index of the list")]
		public short end_index = end_index;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 7)]
	public struct mavlink_mission_write_partial_list_t(short start_index, short end_index, byte target_system, byte target_component, byte mission_type)
	{
		[Units("")]
		[Description("Start index. Must be smaller / equal to the largest index of the current onboard list.")]
		public short start_index = start_index;

		[Units("")]
		[Description("End index, equal or greater than start index.")]
		public short end_index = end_index;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 38)]
	public struct mavlink_mission_item_t(float param1, float param2, float param3, float param4, float x, float y, float z, ushort seq, ushort command, byte target_system, byte target_component, byte frame, byte current, byte autocontinue, byte mission_type)
	{
		[Units("")]
		[Description("PARAM1, see MAV_CMD enum")]
		public float param1 = param1;

		[Units("")]
		[Description("PARAM2, see MAV_CMD enum")]
		public float param2 = param2;

		[Units("")]
		[Description("PARAM3, see MAV_CMD enum")]
		public float param3 = param3;

		[Units("")]
		[Description("PARAM4, see MAV_CMD enum")]
		public float param4 = param4;

		[Units("")]
		[Description("PARAM5 / local: X coordinate, global: latitude")]
		public float x = x;

		[Units("")]
		[Description("PARAM6 / local: Y coordinate, global: longitude")]
		public float y = y;

		[Units("")]
		[Description("PARAM7 / local: Z coordinate, global: altitude (relative or absolute, depending on frame).")]
		public float z = z;

		[Units("")]
		[Description("Sequence")]
		public ushort seq = seq;

		[Units("")]
		[Description("The scheduled action for the waypoint.")]
		public ushort command = command;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("The coordinate system of the waypoint.")]
		public byte frame = frame;

		[Units("")]
		[Description("false:0, true:1")]
		public byte current = current;

		[Units("")]
		[Description("Autocontinue to next waypoint. 0: false, 1: true. Set false to pause mission after the item completes.")]
		public byte autocontinue = autocontinue;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 5)]
	public struct mavlink_mission_request_t(ushort seq, byte target_system, byte target_component, byte mission_type)
	{
		[Units("")]
		[Description("Sequence")]
		public ushort seq = seq;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
	public struct mavlink_mission_set_current_t(ushort seq, byte target_system, byte target_component)
	{
		[Units("")]
		[Description("Sequence")]
		public ushort seq = seq;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 18)]
	public struct mavlink_mission_current_t(ushort seq, ushort total, byte mission_state, byte mission_mode, uint mission_id, uint fence_id, uint rally_points_id)
	{
		[Units("")]
		[Description("Sequence")]
		public ushort seq = seq;

		[Units("")]
		[Description("Total number of mission items on vehicle (on last item, sequence == total). If the autopilot stores its home location as part of the mission this will be excluded from the total. 0: Not supported, UINT16_MAX if no mission is present on the vehicle.")]
		public ushort total = total;

		[Units("")]
		[Description("Mission state machine state. MISSION_STATE_UNKNOWN if state reporting not supported.")]
		public byte mission_state = mission_state;

		[Units("")]
		[Description("Vehicle is in a mode that can execute mission items or suspended. 0: Unknown, 1: In mission mode, 2: Suspended (not in mission mode).")]
		public byte mission_mode = mission_mode;

		[Units("")]
		[Description("Id of current on-vehicle mission plan, or 0 if IDs are not supported or there is no mission loaded. GCS can use this to track changes to the mission plan type. The same value is returned on mission upload (in the MISSION_ACK).")]
		public uint mission_id = mission_id;

		[Units("")]
		[Description("Id of current on-vehicle fence plan, or 0 if IDs are not supported or there is no fence loaded. GCS can use this to track changes to the fence plan type. The same value is returned on fence upload (in the MISSION_ACK).")]
		public uint fence_id = fence_id;

		[Units("")]
		[Description("Id of current on-vehicle rally point plan, or 0 if IDs are not supported or there are no rally points loaded. GCS can use this to track changes to the rally point plan type. The same value is returned on rally point upload (in the MISSION_ACK).")]
		public uint rally_points_id = rally_points_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 3)]
	public struct mavlink_mission_request_list_t(byte target_system, byte target_component, byte mission_type)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 9)]
	public struct mavlink_mission_count_t(ushort count, byte target_system, byte target_component, byte mission_type, uint opaque_id)
	{
		[Units("")]
		[Description("Number of mission items in the sequence")]
		public ushort count = count;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;

		[Units("")]
		[Description("Id of current on-vehicle mission, fence, or rally point plan (on download from vehicle).         This field is used when downloading a plan from a vehicle to a GCS.         0 on upload to the vehicle from GCS.         0 if plan ids are not supported.         The current on-vehicle plan ids are streamed in `MISSION_CURRENT`, allowing a GCS to determine if any part of the plan has changed and needs to be re-uploaded.         The ids are recalculated by the vehicle when any part of the on-vehicle plan changes (when a new plan is uploaded, the vehicle returns the new id to the GCS in MISSION_ACK).       ")]
		public uint opaque_id = opaque_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 3)]
	public struct mavlink_mission_clear_all_t(byte target_system, byte target_component, byte mission_type)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
	public struct mavlink_mission_item_reached_t(ushort seq)
	{
		[Units("")]
		[Description("Sequence")]
		public ushort seq = seq;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
	public struct mavlink_mission_ack_t(byte target_system, byte target_component, byte type, byte mission_type, uint opaque_id)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Mission result.")]
		public byte type = type;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;

		[Units("")]
		[Description("Id of new on-vehicle mission, fence, or rally point plan (on upload to vehicle).         The id is calculated and returned by a vehicle when a new plan is uploaded by a GCS.         The only requirement on the id is that it must change when there is any change to the on-vehicle plan type (there is no requirement that the id be globally unique).         0 on download from the vehicle to the GCS (on download the ID is set in MISSION_COUNT).         0 if plan ids are not supported.         The current on-vehicle plan ids are streamed in `MISSION_CURRENT`, allowing a GCS to determine if any part of the plan has changed and needs to be re-uploaded.       ")]
		public uint opaque_id = opaque_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 21)]
	public struct mavlink_set_gps_global_origin_t(int latitude, int longitude, int altitude, byte target_system, ulong time_usec)
	{
		[Units("[degE7]")]
		[Description("Latitude (WGS84)")]
		public int latitude = latitude;

		[Units("[degE7]")]
		[Description("Longitude (WGS84)")]
		public int longitude = longitude;

		[Units("[mm]")]
		[Description("Altitude (MSL). Positive for up.")]
		public int altitude = altitude;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
	public struct mavlink_gps_global_origin_t(int latitude, int longitude, int altitude, ulong time_usec)
	{
		[Units("[degE7]")]
		[Description("Latitude (WGS84)")]
		public int latitude = latitude;

		[Units("[degE7]")]
		[Description("Longitude (WGS84)")]
		public int longitude = longitude;

		[Units("[mm]")]
		[Description("Altitude (MSL). Positive for up.")]
		public int altitude = altitude;

		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 37)]
	public struct mavlink_param_map_rc_t(float param_value0, float scale, float param_value_min, float param_value_max, short param_index, byte target_system, byte target_component, byte[] param_id, byte parameter_rc_channel_index)
	{
		[Units("")]
		[Description("Initial parameter value")]
		public float param_value0 = param_value0;

		[Units("")]
		[Description("Scale, maps the RC range [-1, 1] to a parameter value")]
		public float scale = scale;

		[Units("")]
		[Description("Minimum param value. The protocol does not define if this overwrites an onboard minimum value. (Depends on implementation)")]
		public float param_value_min = param_value_min;

		[Units("")]
		[Description("Maximum param value. The protocol does not define if this overwrites an onboard maximum value. (Depends on implementation)")]
		public float param_value_max = param_value_max;

		[Units("")]
		[Description("Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored), send -2 to disable any existing map for this rc_channel_index.")]
		public short param_index = param_index;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
		public byte[] param_id = param_id;

		[Units("")]
		[Description("Index of parameter RC channel. Not equal to the RC channel id. Typically corresponds to a potentiometer-knob on the RC.")]
		public byte parameter_rc_channel_index = parameter_rc_channel_index;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 5)]
	public struct mavlink_mission_request_int_t(ushort seq, byte target_system, byte target_component, byte mission_type)
	{
		[Units("")]
		[Description("Sequence")]
		public ushort seq = seq;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 27)]
	public struct mavlink_safety_set_allowed_area_t(float p1x, float p1y, float p1z, float p2x, float p2y, float p2z, byte target_system, byte target_component, byte frame)
	{
		[Units("[m]")]
		[Description("x position 1 / Latitude 1")]
		public float p1x = p1x;

		[Units("[m]")]
		[Description("y position 1 / Longitude 1")]
		public float p1y = p1y;

		[Units("[m]")]
		[Description("z position 1 / Altitude 1")]
		public float p1z = p1z;

		[Units("[m]")]
		[Description("x position 2 / Latitude 2")]
		public float p2x = p2x;

		[Units("[m]")]
		[Description("y position 2 / Longitude 2")]
		public float p2y = p2y;

		[Units("[m]")]
		[Description("z position 2 / Altitude 2")]
		public float p2z = p2z;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Coordinate frame. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down.")]
		public byte frame = frame;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 25)]
	public struct mavlink_safety_allowed_area_t(float p1x, float p1y, float p1z, float p2x, float p2y, float p2z, byte frame)
	{
		[Units("[m]")]
		[Description("x position 1 / Latitude 1")]
		public float p1x = p1x;

		[Units("[m]")]
		[Description("y position 1 / Longitude 1")]
		public float p1y = p1y;

		[Units("[m]")]
		[Description("z position 1 / Altitude 1")]
		public float p1z = p1z;

		[Units("[m]")]
		[Description("x position 2 / Latitude 2")]
		public float p2x = p2x;

		[Units("[m]")]
		[Description("y position 2 / Longitude 2")]
		public float p2y = p2y;

		[Units("[m]")]
		[Description("z position 2 / Altitude 2")]
		public float p2z = p2z;

		[Units("")]
		[Description("Coordinate frame. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down.")]
		public byte frame = frame;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 72)]
	public struct mavlink_attitude_quaternion_cov_t(ulong time_usec, float[] q, float rollspeed, float pitchspeed, float yawspeed, float[] covariance)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation)")]
		public float[] q = q;

		[Units("[rad/s]")]
		[Description("Roll angular speed")]
		public float rollspeed = rollspeed;

		[Units("[rad/s]")]
		[Description("Pitch angular speed")]
		public float pitchspeed = pitchspeed;

		[Units("[rad/s]")]
		[Description("Yaw angular speed")]
		public float yawspeed = yawspeed;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
		[Units("")]
		[Description("Row-major representation of a 3x3 attitude covariance matrix (states: roll, pitch, yaw; first three entries are the first ROW, next three entries are the second row, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] covariance = covariance;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 26)]
	public struct mavlink_nav_controller_output_t(float nav_roll, float nav_pitch, float alt_error, float aspd_error, float xtrack_error, short nav_bearing, short target_bearing, ushort wp_dist)
	{
		[Units("[deg]")]
		[Description("Current desired roll")]
		public float nav_roll = nav_roll;

		[Units("[deg]")]
		[Description("Current desired pitch")]
		public float nav_pitch = nav_pitch;

		[Units("[m]")]
		[Description("Current altitude error")]
		public float alt_error = alt_error;

		[Units("[m/s]")]
		[Description("Current airspeed error")]
		public float aspd_error = aspd_error;

		[Units("[m]")]
		[Description("Current crosstrack error on x-y plane")]
		public float xtrack_error = xtrack_error;

		[Units("[deg]")]
		[Description("Current desired heading")]
		public short nav_bearing = nav_bearing;

		[Units("[deg]")]
		[Description("Bearing to current waypoint/target")]
		public short target_bearing = target_bearing;

		[Units("[m]")]
		[Description("Distance to active waypoint")]
		public ushort wp_dist = wp_dist;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 181)]
	public struct mavlink_global_position_int_cov_t(ulong time_usec, int lat, int lon, int alt, int relative_alt, float vx, float vy, float vz, float[] covariance, byte estimator_type)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[degE7]")]
		[Description("Latitude")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude in meters above MSL")]
		public int alt = alt;

		[Units("[mm]")]
		[Description("Altitude above ground")]
		public int relative_alt = relative_alt;

		[Units("[m/s]")]
		[Description("Ground X Speed (Latitude)")]
		public float vx = vx;

		[Units("[m/s]")]
		[Description("Ground Y Speed (Longitude)")]
		public float vy = vy;

		[Units("[m/s]")]
		[Description("Ground Z Speed (Altitude)")]
		public float vz = vz;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
		[Units("")]
		[Description("Row-major representation of a 6x6 position and velocity 6x6 cross-covariance matrix (states: lat, lon, alt, vx, vy, vz; first six entries are the first ROW, next six entries are the second row, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] covariance = covariance;

		[Units("")]
		[Description("Class id of the estimator this estimate originated from.")]
		public byte estimator_type = estimator_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 225)]
	public struct mavlink_local_position_ned_cov_t(ulong time_usec, float x, float y, float z, float vx, float vy, float vz, float ax, float ay, float az, float[] covariance, byte estimator_type)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[m]")]
		[Description("X Position")]
		public float x = x;

		[Units("[m]")]
		[Description("Y Position")]
		public float y = y;

		[Units("[m]")]
		[Description("Z Position")]
		public float z = z;

		[Units("[m/s]")]
		[Description("X Speed")]
		public float vx = vx;

		[Units("[m/s]")]
		[Description("Y Speed")]
		public float vy = vy;

		[Units("[m/s]")]
		[Description("Z Speed")]
		public float vz = vz;

		[Units("[m/s/s]")]
		[Description("X Acceleration")]
		public float ax = ax;

		[Units("[m/s/s]")]
		[Description("Y Acceleration")]
		public float ay = ay;

		[Units("[m/s/s]")]
		[Description("Z Acceleration")]
		public float az = az;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 45)]
		[Units("")]
		[Description("Row-major representation of position, velocity and acceleration 9x9 cross-covariance matrix upper right triangle (states: x, y, z, vx, vy, vz, ax, ay, az; first nine entries are the first ROW, next eight entries are the second row, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] covariance = covariance;

		[Units("")]
		[Description("Class id of the estimator this estimate originated from.")]
		public byte estimator_type = estimator_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 42)]
	public struct mavlink_rc_channels_t(uint time_boot_ms, ushort chan1_raw, ushort chan2_raw, ushort chan3_raw, ushort chan4_raw, ushort chan5_raw, ushort chan6_raw, ushort chan7_raw, ushort chan8_raw, ushort chan9_raw, ushort chan10_raw, ushort chan11_raw, ushort chan12_raw, ushort chan13_raw, ushort chan14_raw, ushort chan15_raw, ushort chan16_raw, ushort chan17_raw, ushort chan18_raw, byte chancount, byte rssi)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[us]")]
		[Description("RC channel 1 value.")]
		public ushort chan1_raw = chan1_raw;

		[Units("[us]")]
		[Description("RC channel 2 value.")]
		public ushort chan2_raw = chan2_raw;

		[Units("[us]")]
		[Description("RC channel 3 value.")]
		public ushort chan3_raw = chan3_raw;

		[Units("[us]")]
		[Description("RC channel 4 value.")]
		public ushort chan4_raw = chan4_raw;

		[Units("[us]")]
		[Description("RC channel 5 value.")]
		public ushort chan5_raw = chan5_raw;

		[Units("[us]")]
		[Description("RC channel 6 value.")]
		public ushort chan6_raw = chan6_raw;

		[Units("[us]")]
		[Description("RC channel 7 value.")]
		public ushort chan7_raw = chan7_raw;

		[Units("[us]")]
		[Description("RC channel 8 value.")]
		public ushort chan8_raw = chan8_raw;

		[Units("[us]")]
		[Description("RC channel 9 value.")]
		public ushort chan9_raw = chan9_raw;

		[Units("[us]")]
		[Description("RC channel 10 value.")]
		public ushort chan10_raw = chan10_raw;

		[Units("[us]")]
		[Description("RC channel 11 value.")]
		public ushort chan11_raw = chan11_raw;

		[Units("[us]")]
		[Description("RC channel 12 value.")]
		public ushort chan12_raw = chan12_raw;

		[Units("[us]")]
		[Description("RC channel 13 value.")]
		public ushort chan13_raw = chan13_raw;

		[Units("[us]")]
		[Description("RC channel 14 value.")]
		public ushort chan14_raw = chan14_raw;

		[Units("[us]")]
		[Description("RC channel 15 value.")]
		public ushort chan15_raw = chan15_raw;

		[Units("[us]")]
		[Description("RC channel 16 value.")]
		public ushort chan16_raw = chan16_raw;

		[Units("[us]")]
		[Description("RC channel 17 value.")]
		public ushort chan17_raw = chan17_raw;

		[Units("[us]")]
		[Description("RC channel 18 value.")]
		public ushort chan18_raw = chan18_raw;

		[Units("")]
		[Description("Total number of RC channels being received. This can be larger than 18, indicating that more channels are available but not given in this message. This value should be 0 when no RC channels are available.")]
		public byte chancount = chancount;

		[Units("")]
		[Description("Receive signal strength indicator in device-dependent units/scale. Values: [0-254], UINT8_MAX: invalid/unknown.")]
		public byte rssi = rssi;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
	public struct mavlink_request_data_stream_t(ushort req_message_rate, byte target_system, byte target_component, byte req_stream_id, byte start_stop)
	{
		[Units("[Hz]")]
		[Description("The requested message rate")]
		public ushort req_message_rate = req_message_rate;

		[Units("")]
		[Description("The target requested to send the message stream.")]
		public byte target_system = target_system;

		[Units("")]
		[Description("The target requested to send the message stream.")]
		public byte target_component = target_component;

		[Units("")]
		[Description("The ID of the requested data stream")]
		public byte req_stream_id = req_stream_id;

		[Units("")]
		[Description("1 to start sending, 0 to stop sending.")]
		public byte start_stop = start_stop;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
	public struct mavlink_data_stream_t(ushort message_rate, byte stream_id, byte on_off)
	{
		[Units("[Hz]")]
		[Description("The message rate")]
		public ushort message_rate = message_rate;

		[Units("")]
		[Description("The ID of the requested data stream")]
		public byte stream_id = stream_id;

		[Units("")]
		[Description("1 stream is enabled, 0 stream is stopped.")]
		public byte on_off = on_off;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 30)]
	public struct mavlink_manual_control_t(short x, short y, short z, short r, ushort buttons, byte target, ushort buttons2, byte enabled_extensions, short s, short t, short aux1, short aux2, short aux3, short aux4, short aux5, short aux6)
	{
		[Units("")]
		[Description("X-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to forward(1000)-backward(-1000) movement on a joystick and the pitch of a vehicle.")]
		public short x = x;

		[Units("")]
		[Description("Y-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to left(-1000)-right(1000) movement on a joystick and the roll of a vehicle.")]
		public short y = y;

		[Units("")]
		[Description("Z-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a separate slider movement with maximum being 1000 and minimum being -1000 on a joystick and the thrust of a vehicle. Positive values are positive thrust, negative values are negative thrust.")]
		public short z = z;

		[Units("")]
		[Description("R-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a twisting of the joystick, with counter-clockwise being 1000 and clockwise being -1000, and the yaw of a vehicle.")]
		public short r = r;

		[Units("")]
		[Description("A bitfield corresponding to the joystick buttons' 0-15 current state, 1 for pressed, 0 for released. The lowest bit corresponds to Button 1.")]
		public ushort buttons = buttons;

		[Units("")]
		[Description("The system to be controlled.")]
		public byte target = target;

		[Units("")]
		[Description("A bitfield corresponding to the joystick buttons' 16-31 current state, 1 for pressed, 0 for released. The lowest bit corresponds to Button 16.")]
		public ushort buttons2 = buttons2;

		[Units("")]
		[Description("Set bits to 1 to indicate which of the following extension fields contain valid data: bit 0: pitch, bit 1: roll, bit 2: aux1, bit 3: aux2, bit 4: aux3, bit 5: aux4, bit 6: aux5, bit 7: aux6")]
		public byte enabled_extensions = enabled_extensions;

		[Units("")]
		[Description("Pitch-only-axis, normalized to the range [-1000,1000]. Generally corresponds to pitch on vehicles with additional degrees of freedom. Valid if bit 0 of enabled_extensions field is set. Set to 0 if invalid.")]
		public short s = s;

		[Units("")]
		[Description("Roll-only-axis, normalized to the range [-1000,1000]. Generally corresponds to roll on vehicles with additional degrees of freedom. Valid if bit 1 of enabled_extensions field is set. Set to 0 if invalid.")]
		public short t = t;

		[Units("")]
		[Description("Aux continuous input field 1. Normalized in the range [-1000,1000]. Purpose defined by recipient. Valid data if bit 2 of enabled_extensions field is set. 0 if bit 2 is unset.")]
		public short aux1 = aux1;

		[Units("")]
		[Description("Aux continuous input field 2. Normalized in the range [-1000,1000]. Purpose defined by recipient. Valid data if bit 3 of enabled_extensions field is set. 0 if bit 3 is unset.")]
		public short aux2 = aux2;

		[Units("")]
		[Description("Aux continuous input field 3. Normalized in the range [-1000,1000]. Purpose defined by recipient. Valid data if bit 4 of enabled_extensions field is set. 0 if bit 4 is unset.")]
		public short aux3 = aux3;

		[Units("")]
		[Description("Aux continuous input field 4. Normalized in the range [-1000,1000]. Purpose defined by recipient. Valid data if bit 5 of enabled_extensions field is set. 0 if bit 5 is unset.")]
		public short aux4 = aux4;

		[Units("")]
		[Description("Aux continuous input field 5. Normalized in the range [-1000,1000]. Purpose defined by recipient. Valid data if bit 6 of enabled_extensions field is set. 0 if bit 6 is unset.")]
		public short aux5 = aux5;

		[Units("")]
		[Description("Aux continuous input field 6. Normalized in the range [-1000,1000]. Purpose defined by recipient. Valid data if bit 7 of enabled_extensions field is set. 0 if bit 7 is unset.")]
		public short aux6 = aux6;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 38)]
	public struct mavlink_rc_channels_override_t(ushort chan1_raw, ushort chan2_raw, ushort chan3_raw, ushort chan4_raw, ushort chan5_raw, ushort chan6_raw, ushort chan7_raw, ushort chan8_raw, byte target_system, byte target_component, ushort chan9_raw, ushort chan10_raw, ushort chan11_raw, ushort chan12_raw, ushort chan13_raw, ushort chan14_raw, ushort chan15_raw, ushort chan16_raw, ushort chan17_raw, ushort chan18_raw)
	{
		[Units("[us]")]
		[Description("RC channel 1 value. A value of UINT16_MAX means to ignore this field. A value of 0 means to release this channel back to the RC radio.")]
		public ushort chan1_raw = chan1_raw;

		[Units("[us]")]
		[Description("RC channel 2 value. A value of UINT16_MAX means to ignore this field. A value of 0 means to release this channel back to the RC radio.")]
		public ushort chan2_raw = chan2_raw;

		[Units("[us]")]
		[Description("RC channel 3 value. A value of UINT16_MAX means to ignore this field. A value of 0 means to release this channel back to the RC radio.")]
		public ushort chan3_raw = chan3_raw;

		[Units("[us]")]
		[Description("RC channel 4 value. A value of UINT16_MAX means to ignore this field. A value of 0 means to release this channel back to the RC radio.")]
		public ushort chan4_raw = chan4_raw;

		[Units("[us]")]
		[Description("RC channel 5 value. A value of UINT16_MAX means to ignore this field. A value of 0 means to release this channel back to the RC radio.")]
		public ushort chan5_raw = chan5_raw;

		[Units("[us]")]
		[Description("RC channel 6 value. A value of UINT16_MAX means to ignore this field. A value of 0 means to release this channel back to the RC radio.")]
		public ushort chan6_raw = chan6_raw;

		[Units("[us]")]
		[Description("RC channel 7 value. A value of UINT16_MAX means to ignore this field. A value of 0 means to release this channel back to the RC radio.")]
		public ushort chan7_raw = chan7_raw;

		[Units("[us]")]
		[Description("RC channel 8 value. A value of UINT16_MAX means to ignore this field. A value of 0 means to release this channel back to the RC radio.")]
		public ushort chan8_raw = chan8_raw;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("[us]")]
		[Description("RC channel 9 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan9_raw = chan9_raw;

		[Units("[us]")]
		[Description("RC channel 10 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan10_raw = chan10_raw;

		[Units("[us]")]
		[Description("RC channel 11 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan11_raw = chan11_raw;

		[Units("[us]")]
		[Description("RC channel 12 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan12_raw = chan12_raw;

		[Units("[us]")]
		[Description("RC channel 13 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan13_raw = chan13_raw;

		[Units("[us]")]
		[Description("RC channel 14 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan14_raw = chan14_raw;

		[Units("[us]")]
		[Description("RC channel 15 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan15_raw = chan15_raw;

		[Units("[us]")]
		[Description("RC channel 16 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan16_raw = chan16_raw;

		[Units("[us]")]
		[Description("RC channel 17 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan17_raw = chan17_raw;

		[Units("[us]")]
		[Description("RC channel 18 value. A value of 0 or UINT16_MAX means to ignore this field. A value of UINT16_MAX-1 means to release this channel back to the RC radio.")]
		public ushort chan18_raw = chan18_raw;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 38)]
	public struct mavlink_mission_item_int_t(float param1, float param2, float param3, float param4, int x, int y, float z, ushort seq, ushort command, byte target_system, byte target_component, byte frame, byte current, byte autocontinue, byte mission_type)
	{
		[Units("")]
		[Description("PARAM1, see MAV_CMD enum")]
		public float param1 = param1;

		[Units("")]
		[Description("PARAM2, see MAV_CMD enum")]
		public float param2 = param2;

		[Units("")]
		[Description("PARAM3, see MAV_CMD enum")]
		public float param3 = param3;

		[Units("")]
		[Description("PARAM4, see MAV_CMD enum")]
		public float param4 = param4;

		[Units("")]
		[Description("PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7")]
		public int x = x;

		[Units("")]
		[Description("PARAM6 / y position: local: x position in meters * 1e4, global: longitude in degrees *10^7")]
		public int y = y;

		[Units("")]
		[Description("PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame.")]
		public float z = z;

		[Units("")]
		[Description("Waypoint ID (sequence number). Starts at zero. Increases monotonically for each waypoint, no gaps in the sequence (0,1,2,3,4).")]
		public ushort seq = seq;

		[Units("")]
		[Description("The scheduled action for the waypoint.")]
		public ushort command = command;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("The coordinate system of the waypoint.")]
		public byte frame = frame;

		[Units("")]
		[Description("false:0, true:1")]
		public byte current = current;

		[Units("")]
		[Description("Autocontinue to next waypoint. 0: false, 1: true. Set false to pause mission after the item completes.")]
		public byte autocontinue = autocontinue;

		[Units("")]
		[Description("Mission type.")]
		public byte mission_type = mission_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
	public struct mavlink_vfr_hud_t(float airspeed, float groundspeed, float alt, float climb, short heading, ushort throttle)
	{
		[Units("[m/s]")]
		[Description("Vehicle speed in form appropriate for vehicle type. For standard aircraft this is typically calibrated airspeed (CAS) or indicated airspeed (IAS) - either of which can be used by a pilot to estimate stall speed.")]
		public float airspeed = airspeed;

		[Units("[m/s]")]
		[Description("Current ground speed.")]
		public float groundspeed = groundspeed;

		[Units("[m]")]
		[Description("Current altitude (MSL).")]
		public float alt = alt;

		[Units("[m/s]")]
		[Description("Current climb rate.")]
		public float climb = climb;

		[Units("[deg]")]
		[Description("Current heading in compass units (0-360, 0=north).")]
		public short heading = heading;

		[Units("[%]")]
		[Description("Current throttle setting (0 to 100).")]
		public ushort throttle = throttle;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 35)]
	public struct mavlink_command_int_t(float param1, float param2, float param3, float param4, int x, int y, float z, ushort command, byte target_system, byte target_component, byte frame, byte current, byte autocontinue)
	{
		[Units("")]
		[Description("PARAM1, see MAV_CMD enum")]
		public float param1 = param1;

		[Units("")]
		[Description("PARAM2, see MAV_CMD enum")]
		public float param2 = param2;

		[Units("")]
		[Description("PARAM3, see MAV_CMD enum")]
		public float param3 = param3;

		[Units("")]
		[Description("PARAM4, see MAV_CMD enum")]
		public float param4 = param4;

		[Units("")]
		[Description("PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7")]
		public int x = x;

		[Units("")]
		[Description("PARAM6 / local: y position in meters * 1e4, global: longitude in degrees * 10^7")]
		public int y = y;

		[Units("")]
		[Description("PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame).")]
		public float z = z;

		[Units("")]
		[Description("The scheduled action for the mission item.")]
		public ushort command = command;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("The coordinate system of the COMMAND.")]
		public byte frame = frame;

		[Units("")]
		[Description("Not used.")]
		public byte current = current;

		[Units("")]
		[Description("Not used (set 0).")]
		public byte autocontinue = autocontinue;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 33)]
	public struct mavlink_command_long_t(float param1, float param2, float param3, float param4, float param5, float param6, float param7, ushort command, byte target_system, byte target_component, byte confirmation)
	{
		[Units("")]
		[Description("Parameter 1 (for the specific command).")]
		public float param1 = param1;

		[Units("")]
		[Description("Parameter 2 (for the specific command).")]
		public float param2 = param2;

		[Units("")]
		[Description("Parameter 3 (for the specific command).")]
		public float param3 = param3;

		[Units("")]
		[Description("Parameter 4 (for the specific command).")]
		public float param4 = param4;

		[Units("")]
		[Description("Parameter 5 (for the specific command).")]
		public float param5 = param5;

		[Units("")]
		[Description("Parameter 6 (for the specific command).")]
		public float param6 = param6;

		[Units("")]
		[Description("Parameter 7 (for the specific command).")]
		public float param7 = param7;

		[Units("")]
		[Description("Command ID (of command to send).")]
		public ushort command = command;

		[Units("")]
		[Description("System which should execute the command")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component which should execute the command, 0 for all components")]
		public byte target_component = target_component;

		[Units("")]
		[Description("0: First transmission of this command. 1-255: Confirmation transmissions (e.g. for kill command)")]
		public byte confirmation = confirmation;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 10)]
	public struct mavlink_command_ack_t(ushort command, byte result, byte progress, int result_param2, byte target_system, byte target_component)
	{
		[Units("")]
		[Description("Command ID (of acknowledged command).")]
		public ushort command = command;

		[Units("")]
		[Description("Result of command.")]
		public byte result = result;

		[Units("[%]")]
		[Description("The progress percentage when result is MAV_RESULT_IN_PROGRESS. Values: [0-100], or UINT8_MAX if the progress is unknown.")]
		public byte progress = progress;

		[Units("")]
		[Description("Additional result information. Can be set with a command-specific enum containing command-specific error reasons for why the command might be denied. If used, the associated enum must be documented in the corresponding MAV_CMD (this enum should have a 0 value to indicate 'unused' or 'unknown').")]
		public int result_param2 = result_param2;

		[Units("")]
		[Description("System ID of the target recipient. This is the ID of the system that sent the command for which this COMMAND_ACK is an acknowledgement.")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID of the target recipient. This is the ID of the system that sent the command for which this COMMAND_ACK is an acknowledgement.")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
	public struct mavlink_command_cancel_t(ushort command, byte target_system, byte target_component)
	{
		[Units("")]
		[Description("Command ID (of command to cancel).")]
		public ushort command = command;

		[Units("")]
		[Description("System executing long running command. Should not be broadcast (0).")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component executing long running command.")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 22)]
	public struct mavlink_manual_setpoint_t(uint time_boot_ms, float roll, float pitch, float yaw, float thrust, byte mode_switch, byte manual_override_switch)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[rad/s]")]
		[Description("Desired roll rate")]
		public float roll = roll;

		[Units("[rad/s]")]
		[Description("Desired pitch rate")]
		public float pitch = pitch;

		[Units("[rad/s]")]
		[Description("Desired yaw rate")]
		public float yaw = yaw;

		[Units("")]
		[Description("Collective thrust, normalized to 0 .. 1")]
		public float thrust = thrust;

		[Units("")]
		[Description("Flight mode switch position, 0.. 255")]
		public byte mode_switch = mode_switch;

		[Units("")]
		[Description("Override mode switch position, 0.. 255")]
		public byte manual_override_switch = manual_override_switch;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 51)]
	public struct mavlink_set_attitude_target_t(uint time_boot_ms, float[] q, float body_roll_rate, float body_pitch_rate, float body_yaw_rate, float thrust, byte target_system, byte target_component, byte type_mask, float[] thrust_body)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0) from MAV_FRAME_LOCAL_NED to MAV_FRAME_BODY_FRD")]
		public float[] q = q;

		[Units("[rad/s]")]
		[Description("Body roll rate")]
		public float body_roll_rate = body_roll_rate;

		[Units("[rad/s]")]
		[Description("Body pitch rate")]
		public float body_pitch_rate = body_pitch_rate;

		[Units("[rad/s]")]
		[Description("Body yaw rate")]
		public float body_yaw_rate = body_yaw_rate;

		[Units("")]
		[Description("Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust)")]
		public float thrust = thrust;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
		public byte type_mask = type_mask;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		[Units("")]
		[Description("3D thrust setpoint in the body NED frame, normalized to -1 .. 1")]
		public float[] thrust_body = thrust_body;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 37)]
	public struct mavlink_attitude_target_t(uint time_boot_ms, float[] q, float body_roll_rate, float body_pitch_rate, float body_yaw_rate, float thrust, byte type_mask)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)")]
		public float[] q = q;

		[Units("[rad/s]")]
		[Description("Body roll rate")]
		public float body_roll_rate = body_roll_rate;

		[Units("[rad/s]")]
		[Description("Body pitch rate")]
		public float body_pitch_rate = body_pitch_rate;

		[Units("[rad/s]")]
		[Description("Body yaw rate")]
		public float body_yaw_rate = body_yaw_rate;

		[Units("")]
		[Description("Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust)")]
		public float thrust = thrust;

		[Units("")]
		[Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
		public byte type_mask = type_mask;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 53)]
	public struct mavlink_set_position_target_local_ned_t(uint time_boot_ms, float x, float y, float z, float vx, float vy, float vz, float afx, float afy, float afz, float yaw, float yaw_rate, ushort type_mask, byte target_system, byte target_component, byte coordinate_frame)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[m]")]
		[Description("X Position in NED frame")]
		public float x = x;

		[Units("[m]")]
		[Description("Y Position in NED frame")]
		public float y = y;

		[Units("[m]")]
		[Description("Z Position in NED frame (note, altitude is negative in NED)")]
		public float z = z;

		[Units("[m/s]")]
		[Description("X velocity in NED frame")]
		public float vx = vx;

		[Units("[m/s]")]
		[Description("Y velocity in NED frame")]
		public float vy = vy;

		[Units("[m/s]")]
		[Description("Z velocity in NED frame")]
		public float vz = vz;

		[Units("[m/s/s]")]
		[Description("X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afx = afx;

		[Units("[m/s/s]")]
		[Description("Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afy = afy;

		[Units("[m/s/s]")]
		[Description("Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afz = afz;

		[Units("[rad]")]
		[Description("yaw setpoint")]
		public float yaw = yaw;

		[Units("[rad/s]")]
		[Description("yaw rate setpoint")]
		public float yaw_rate = yaw_rate;

		[Units("")]
		[Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
		public ushort type_mask = type_mask;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9")]
		public byte coordinate_frame = coordinate_frame;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 51)]
	public struct mavlink_position_target_local_ned_t(uint time_boot_ms, float x, float y, float z, float vx, float vy, float vz, float afx, float afy, float afz, float yaw, float yaw_rate, ushort type_mask, byte coordinate_frame)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[m]")]
		[Description("X Position in NED frame")]
		public float x = x;

		[Units("[m]")]
		[Description("Y Position in NED frame")]
		public float y = y;

		[Units("[m]")]
		[Description("Z Position in NED frame (note, altitude is negative in NED)")]
		public float z = z;

		[Units("[m/s]")]
		[Description("X velocity in NED frame")]
		public float vx = vx;

		[Units("[m/s]")]
		[Description("Y velocity in NED frame")]
		public float vy = vy;

		[Units("[m/s]")]
		[Description("Z velocity in NED frame")]
		public float vz = vz;

		[Units("[m/s/s]")]
		[Description("X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afx = afx;

		[Units("[m/s/s]")]
		[Description("Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afy = afy;

		[Units("[m/s/s]")]
		[Description("Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afz = afz;

		[Units("[rad]")]
		[Description("yaw setpoint")]
		public float yaw = yaw;

		[Units("[rad/s]")]
		[Description("yaw rate setpoint")]
		public float yaw_rate = yaw_rate;

		[Units("")]
		[Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
		public ushort type_mask = type_mask;

		[Units("")]
		[Description("Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9")]
		public byte coordinate_frame = coordinate_frame;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 53)]
	public struct mavlink_set_position_target_global_int_t(uint time_boot_ms, int lat_int, int lon_int, float alt, float vx, float vy, float vz, float afx, float afy, float afz, float yaw, float yaw_rate, ushort type_mask, byte target_system, byte target_component, byte coordinate_frame)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot). The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency.")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[degE7]")]
		[Description("Latitude in WGS84 frame")]
		public int lat_int = lat_int;

		[Units("[degE7]")]
		[Description("Longitude in WGS84 frame")]
		public int lon_int = lon_int;

		[Units("[m]")]
		[Description("Altitude (MSL, Relative to home, or AGL - depending on frame)")]
		public float alt = alt;

		[Units("[m/s]")]
		[Description("X velocity in NED frame")]
		public float vx = vx;

		[Units("[m/s]")]
		[Description("Y velocity in NED frame")]
		public float vy = vy;

		[Units("[m/s]")]
		[Description("Z velocity in NED frame")]
		public float vz = vz;

		[Units("[m/s/s]")]
		[Description("X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afx = afx;

		[Units("[m/s/s]")]
		[Description("Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afy = afy;

		[Units("[m/s/s]")]
		[Description("Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afz = afz;

		[Units("[rad]")]
		[Description("yaw setpoint")]
		public float yaw = yaw;

		[Units("[rad/s]")]
		[Description("yaw rate setpoint")]
		public float yaw_rate = yaw_rate;

		[Units("")]
		[Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
		public ushort type_mask = type_mask;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Valid options are: MAV_FRAME_GLOBAL = 0, MAV_FRAME_GLOBAL_RELATIVE_ALT = 3, MAV_FRAME_GLOBAL_TERRAIN_ALT = 10 (MAV_FRAME_GLOBAL_INT, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT are allowed synonyms, but have been deprecated)")]
		public byte coordinate_frame = coordinate_frame;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 51)]
	public struct mavlink_position_target_global_int_t(uint time_boot_ms, int lat_int, int lon_int, float alt, float vx, float vy, float vz, float afx, float afy, float afz, float yaw, float yaw_rate, ushort type_mask, byte coordinate_frame)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot). The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency.")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[degE7]")]
		[Description("Latitude in WGS84 frame")]
		public int lat_int = lat_int;

		[Units("[degE7]")]
		[Description("Longitude in WGS84 frame")]
		public int lon_int = lon_int;

		[Units("[m]")]
		[Description("Altitude (MSL, AGL or relative to home altitude, depending on frame)")]
		public float alt = alt;

		[Units("[m/s]")]
		[Description("X velocity in NED frame")]
		public float vx = vx;

		[Units("[m/s]")]
		[Description("Y velocity in NED frame")]
		public float vy = vy;

		[Units("[m/s]")]
		[Description("Z velocity in NED frame")]
		public float vz = vz;

		[Units("[m/s/s]")]
		[Description("X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afx = afx;

		[Units("[m/s/s]")]
		[Description("Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afy = afy;

		[Units("[m/s/s]")]
		[Description("Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N")]
		public float afz = afz;

		[Units("[rad]")]
		[Description("yaw setpoint")]
		public float yaw = yaw;

		[Units("[rad/s]")]
		[Description("yaw rate setpoint")]
		public float yaw_rate = yaw_rate;

		[Units("")]
		[Description("Bitmap to indicate which dimensions should be ignored by the vehicle.")]
		public ushort type_mask = type_mask;

		[Units("")]
		[Description("Valid options are: MAV_FRAME_GLOBAL = 0, MAV_FRAME_GLOBAL_RELATIVE_ALT = 3, MAV_FRAME_GLOBAL_TERRAIN_ALT = 10 (MAV_FRAME_GLOBAL_INT, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT are allowed synonyms, but have been deprecated)")]
		public byte coordinate_frame = coordinate_frame;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
	public struct mavlink_local_position_ned_system_global_offset_t(uint time_boot_ms, float x, float y, float z, float roll, float pitch, float yaw)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[m]")]
		[Description("X Position")]
		public float x = x;

		[Units("[m]")]
		[Description("Y Position")]
		public float y = y;

		[Units("[m]")]
		[Description("Z Position")]
		public float z = z;

		[Units("[rad]")]
		[Description("Roll")]
		public float roll = roll;

		[Units("[rad]")]
		[Description("Pitch")]
		public float pitch = pitch;

		[Units("[rad]")]
		[Description("Yaw")]
		public float yaw = yaw;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 56)]
	public struct mavlink_hil_state_t(ulong time_usec, float roll, float pitch, float yaw, float rollspeed, float pitchspeed, float yawspeed, int lat, int lon, int alt, short vx, short vy, short vz, short xacc, short yacc, short zacc)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[rad]")]
		[Description("Roll angle")]
		public float roll = roll;

		[Units("[rad]")]
		[Description("Pitch angle")]
		public float pitch = pitch;

		[Units("[rad]")]
		[Description("Yaw angle")]
		public float yaw = yaw;

		[Units("[rad/s]")]
		[Description("Body frame roll / phi angular speed")]
		public float rollspeed = rollspeed;

		[Units("[rad/s]")]
		[Description("Body frame pitch / theta angular speed")]
		public float pitchspeed = pitchspeed;

		[Units("[rad/s]")]
		[Description("Body frame yaw / psi angular speed")]
		public float yawspeed = yawspeed;

		[Units("[degE7]")]
		[Description("Latitude")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude")]
		public int alt = alt;

		[Units("[cm/s]")]
		[Description("Ground X Speed (Latitude)")]
		public short vx = vx;

		[Units("[cm/s]")]
		[Description("Ground Y Speed (Longitude)")]
		public short vy = vy;

		[Units("[cm/s]")]
		[Description("Ground Z Speed (Altitude)")]
		public short vz = vz;

		[Units("[mG]")]
		[Description("X acceleration")]
		public short xacc = xacc;

		[Units("[mG]")]
		[Description("Y acceleration")]
		public short yacc = yacc;

		[Units("[mG]")]
		[Description("Z acceleration")]
		public short zacc = zacc;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 42)]
	public struct mavlink_hil_controls_t(ulong time_usec, float roll_ailerons, float pitch_elevator, float yaw_rudder, float throttle, float aux1, float aux2, float aux3, float aux4, byte mode, byte nav_mode)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("Control output -1 .. 1")]
		public float roll_ailerons = roll_ailerons;

		[Units("")]
		[Description("Control output -1 .. 1")]
		public float pitch_elevator = pitch_elevator;

		[Units("")]
		[Description("Control output -1 .. 1")]
		public float yaw_rudder = yaw_rudder;

		[Units("")]
		[Description("Throttle 0 .. 1")]
		public float throttle = throttle;

		[Units("")]
		[Description("Aux 1, -1 .. 1")]
		public float aux1 = aux1;

		[Units("")]
		[Description("Aux 2, -1 .. 1")]
		public float aux2 = aux2;

		[Units("")]
		[Description("Aux 3, -1 .. 1")]
		public float aux3 = aux3;

		[Units("")]
		[Description("Aux 4, -1 .. 1")]
		public float aux4 = aux4;

		[Units("")]
		[Description("System mode.")]
		public byte mode = mode;

		[Units("")]
		[Description("Navigation mode (MAV_NAV_MODE)")]
		public byte nav_mode = nav_mode;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 33)]
	public struct mavlink_hil_rc_inputs_raw_t(ulong time_usec, ushort chan1_raw, ushort chan2_raw, ushort chan3_raw, ushort chan4_raw, ushort chan5_raw, ushort chan6_raw, ushort chan7_raw, ushort chan8_raw, ushort chan9_raw, ushort chan10_raw, ushort chan11_raw, ushort chan12_raw, byte rssi)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[us]")]
		[Description("RC channel 1 value")]
		public ushort chan1_raw = chan1_raw;

		[Units("[us]")]
		[Description("RC channel 2 value")]
		public ushort chan2_raw = chan2_raw;

		[Units("[us]")]
		[Description("RC channel 3 value")]
		public ushort chan3_raw = chan3_raw;

		[Units("[us]")]
		[Description("RC channel 4 value")]
		public ushort chan4_raw = chan4_raw;

		[Units("[us]")]
		[Description("RC channel 5 value")]
		public ushort chan5_raw = chan5_raw;

		[Units("[us]")]
		[Description("RC channel 6 value")]
		public ushort chan6_raw = chan6_raw;

		[Units("[us]")]
		[Description("RC channel 7 value")]
		public ushort chan7_raw = chan7_raw;

		[Units("[us]")]
		[Description("RC channel 8 value")]
		public ushort chan8_raw = chan8_raw;

		[Units("[us]")]
		[Description("RC channel 9 value")]
		public ushort chan9_raw = chan9_raw;

		[Units("[us]")]
		[Description("RC channel 10 value")]
		public ushort chan10_raw = chan10_raw;

		[Units("[us]")]
		[Description("RC channel 11 value")]
		public ushort chan11_raw = chan11_raw;

		[Units("[us]")]
		[Description("RC channel 12 value")]
		public ushort chan12_raw = chan12_raw;

		[Units("")]
		[Description("Receive signal strength indicator in device-dependent units/scale. Values: [0-254], UINT8_MAX: invalid/unknown.")]
		public byte rssi = rssi;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 81)]
	public struct mavlink_hil_actuator_controls_t(ulong time_usec, ulong flags, float[] controls, byte mode)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("Flags bitmask.")]
		public ulong flags = flags;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Control outputs -1 .. 1. Channel assignment depends on the simulated hardware.")]
		public float[] controls = controls;

		[Units("")]
		[Description("System mode. Includes arming state.")]
		public byte mode = mode;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 34)]
	public struct mavlink_optical_flow_t(ulong time_usec, float flow_comp_m_x, float flow_comp_m_y, float ground_distance, short flow_x, short flow_y, byte sensor_id, byte quality, float flow_rate_x, float flow_rate_y)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[m/s]")]
		[Description("Flow in x-sensor direction, angular-speed compensated")]
		public float flow_comp_m_x = flow_comp_m_x;

		[Units("[m/s]")]
		[Description("Flow in y-sensor direction, angular-speed compensated")]
		public float flow_comp_m_y = flow_comp_m_y;

		[Units("[m]")]
		[Description("Ground distance. Positive value: distance known. Negative value: Unknown distance")]
		public float ground_distance = ground_distance;

		[Units("[dpix]")]
		[Description("Flow in x-sensor direction")]
		public short flow_x = flow_x;

		[Units("[dpix]")]
		[Description("Flow in y-sensor direction")]
		public short flow_y = flow_y;

		[Units("")]
		[Description("Sensor ID")]
		public byte sensor_id = sensor_id;

		[Units("")]
		[Description("Optical flow quality / confidence. 0: bad, 255: maximum quality")]
		public byte quality = quality;

		[Units("[rad/s]")]
		[Description("Flow rate about X axis")]
		public float flow_rate_x = flow_rate_x;

		[Units("[rad/s]")]
		[Description("Flow rate about Y axis")]
		public float flow_rate_y = flow_rate_y;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 117)]
	public struct mavlink_global_vision_position_estimate_t(ulong usec, float x, float y, float z, float roll, float pitch, float yaw, float[] covariance, byte reset_counter)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX time or since system boot)")]
		public ulong usec = usec;

		[Units("[m]")]
		[Description("Global X position")]
		public float x = x;

		[Units("[m]")]
		[Description("Global Y position")]
		public float y = y;

		[Units("[m]")]
		[Description("Global Z position")]
		public float z = z;

		[Units("[rad]")]
		[Description("Roll angle")]
		public float roll = roll;

		[Units("[rad]")]
		[Description("Pitch angle")]
		public float pitch = pitch;

		[Units("[rad]")]
		[Description("Yaw angle")]
		public float yaw = yaw;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
		[Units("")]
		[Description("Row-major representation of pose 6x6 cross-covariance matrix upper right triangle (states: x_global, y_global, z_global, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] covariance = covariance;

		[Units("")]
		[Description("Estimate reset counter. This should be incremented when the estimate resets in any of the dimensions (position, velocity, attitude, angular speed). This is designed to be used when e.g an external SLAM system detects a loop-closure and the estimate jumps.")]
		public byte reset_counter = reset_counter;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 117)]
	public struct mavlink_vision_position_estimate_t(ulong usec, float x, float y, float z, float roll, float pitch, float yaw, float[] covariance, byte reset_counter)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX time or time since system boot)")]
		public ulong usec = usec;

		[Units("[m]")]
		[Description("Local X position")]
		public float x = x;

		[Units("[m]")]
		[Description("Local Y position")]
		public float y = y;

		[Units("[m]")]
		[Description("Local Z position")]
		public float z = z;

		[Units("[rad]")]
		[Description("Roll angle")]
		public float roll = roll;

		[Units("[rad]")]
		[Description("Pitch angle")]
		public float pitch = pitch;

		[Units("[rad]")]
		[Description("Yaw angle")]
		public float yaw = yaw;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
		[Units("")]
		[Description("Row-major representation of pose 6x6 cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] covariance = covariance;

		[Units("")]
		[Description("Estimate reset counter. This should be incremented when the estimate resets in any of the dimensions (position, velocity, attitude, angular speed). This is designed to be used when e.g an external SLAM system detects a loop-closure and the estimate jumps.")]
		public byte reset_counter = reset_counter;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 57)]
	public struct mavlink_vision_speed_estimate_t(ulong usec, float x, float y, float z, float[] covariance, byte reset_counter)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX time or time since system boot)")]
		public ulong usec = usec;

		[Units("[m/s]")]
		[Description("Global X speed")]
		public float x = x;

		[Units("[m/s]")]
		[Description("Global Y speed")]
		public float y = y;

		[Units("[m/s]")]
		[Description("Global Z speed")]
		public float z = z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
		[Units("")]
		[Description("Row-major representation of 3x3 linear velocity covariance matrix (states: vx, vy, vz; 1st three entries - 1st row, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] covariance = covariance;

		[Units("")]
		[Description("Estimate reset counter. This should be incremented when the estimate resets in any of the dimensions (position, velocity, attitude, angular speed). This is designed to be used when e.g an external SLAM system detects a loop-closure and the estimate jumps.")]
		public byte reset_counter = reset_counter;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 116)]
	public struct mavlink_vicon_position_estimate_t(ulong usec, float x, float y, float z, float roll, float pitch, float yaw, float[] covariance)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX time or time since system boot)")]
		public ulong usec = usec;

		[Units("[m]")]
		[Description("Global X position")]
		public float x = x;

		[Units("[m]")]
		[Description("Global Y position")]
		public float y = y;

		[Units("[m]")]
		[Description("Global Z position")]
		public float z = z;

		[Units("[rad]")]
		[Description("Roll angle")]
		public float roll = roll;

		[Units("[rad]")]
		[Description("Pitch angle")]
		public float pitch = pitch;

		[Units("[rad]")]
		[Description("Yaw angle")]
		public float yaw = yaw;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
		[Units("")]
		[Description("Row-major representation of 6x6 pose cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] covariance = covariance;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 63)]
	public struct mavlink_highres_imu_t(ulong time_usec, float xacc, float yacc, float zacc, float xgyro, float ygyro, float zgyro, float xmag, float ymag, float zmag, float abs_pressure, float diff_pressure, float pressure_alt, float temperature, ushort fields_updated, byte id)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[m/s/s]")]
		[Description("X acceleration")]
		public float xacc = xacc;

		[Units("[m/s/s]")]
		[Description("Y acceleration")]
		public float yacc = yacc;

		[Units("[m/s/s]")]
		[Description("Z acceleration")]
		public float zacc = zacc;

		[Units("[rad/s]")]
		[Description("Angular speed around X axis")]
		public float xgyro = xgyro;

		[Units("[rad/s]")]
		[Description("Angular speed around Y axis")]
		public float ygyro = ygyro;

		[Units("[rad/s]")]
		[Description("Angular speed around Z axis")]
		public float zgyro = zgyro;

		[Units("[gauss]")]
		[Description("X Magnetic field")]
		public float xmag = xmag;

		[Units("[gauss]")]
		[Description("Y Magnetic field")]
		public float ymag = ymag;

		[Units("[gauss]")]
		[Description("Z Magnetic field")]
		public float zmag = zmag;

		[Units("[hPa]")]
		[Description("Absolute pressure")]
		public float abs_pressure = abs_pressure;

		[Units("[hPa]")]
		[Description("Differential pressure")]
		public float diff_pressure = diff_pressure;

		[Units("")]
		[Description("Altitude calculated from pressure")]
		public float pressure_alt = pressure_alt;

		[Units("[degC]")]
		[Description("Temperature")]
		public float temperature = temperature;

		[Units("")]
		[Description("Bitmap for fields that have updated since last message")]
		public ushort fields_updated = fields_updated;

		[Units("")]
		[Description("Id. Ids are numbered from 0 and map to IMUs numbered from 1 (e.g. IMU1 will have a message with id=0)")]
		public byte id = id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 44)]
	public struct mavlink_optical_flow_rad_t(ulong time_usec, uint integration_time_us, float integrated_x, float integrated_y, float integrated_xgyro, float integrated_ygyro, float integrated_zgyro, uint time_delta_distance_us, float distance, short temperature, byte sensor_id, byte quality)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[us]")]
		[Description("Integration time. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the.")]
		public uint integration_time_us = integration_time_us;

		[Units("[rad]")]
		[Description("Flow around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.)")]
		public float integrated_x = integrated_x;

		[Units("[rad]")]
		[Description("Flow around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.)")]
		public float integrated_y = integrated_y;

		[Units("[rad]")]
		[Description("RH rotation around X axis")]
		public float integrated_xgyro = integrated_xgyro;

		[Units("[rad]")]
		[Description("RH rotation around Y axis")]
		public float integrated_ygyro = integrated_ygyro;

		[Units("[rad]")]
		[Description("RH rotation around Z axis")]
		public float integrated_zgyro = integrated_zgyro;

		[Units("[us]")]
		[Description("Time since the distance was sampled.")]
		public uint time_delta_distance_us = time_delta_distance_us;

		[Units("[m]")]
		[Description("Distance to the center of the flow field. Positive value (including zero): distance known. Negative value: Unknown distance.")]
		public float distance = distance;

		[Units("[cdegC]")]
		[Description("Temperature")]
		public short temperature = temperature;

		[Units("")]
		[Description("Sensor ID")]
		public byte sensor_id = sensor_id;

		[Units("")]
		[Description("Optical flow quality / confidence. 0: no valid flow, 255: maximum quality")]
		public byte quality = quality;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 65)]
	public struct mavlink_hil_sensor_t(ulong time_usec, float xacc, float yacc, float zacc, float xgyro, float ygyro, float zgyro, float xmag, float ymag, float zmag, float abs_pressure, float diff_pressure, float pressure_alt, float temperature, uint fields_updated, byte id)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[m/s/s]")]
		[Description("X acceleration")]
		public float xacc = xacc;

		[Units("[m/s/s]")]
		[Description("Y acceleration")]
		public float yacc = yacc;

		[Units("[m/s/s]")]
		[Description("Z acceleration")]
		public float zacc = zacc;

		[Units("[rad/s]")]
		[Description("Angular speed around X axis in body frame")]
		public float xgyro = xgyro;

		[Units("[rad/s]")]
		[Description("Angular speed around Y axis in body frame")]
		public float ygyro = ygyro;

		[Units("[rad/s]")]
		[Description("Angular speed around Z axis in body frame")]
		public float zgyro = zgyro;

		[Units("[gauss]")]
		[Description("X Magnetic field")]
		public float xmag = xmag;

		[Units("[gauss]")]
		[Description("Y Magnetic field")]
		public float ymag = ymag;

		[Units("[gauss]")]
		[Description("Z Magnetic field")]
		public float zmag = zmag;

		[Units("[hPa]")]
		[Description("Absolute pressure")]
		public float abs_pressure = abs_pressure;

		[Units("[hPa]")]
		[Description("Differential pressure (airspeed)")]
		public float diff_pressure = diff_pressure;

		[Units("")]
		[Description("Altitude calculated from pressure")]
		public float pressure_alt = pressure_alt;

		[Units("[degC]")]
		[Description("Temperature")]
		public float temperature = temperature;

		[Units("")]
		[Description("Bitmap for fields that have updated since last message")]
		public uint fields_updated = fields_updated;

		[Units("")]
		[Description("Sensor ID (zero indexed). Used for multiple sensor inputs")]
		public byte id = id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 92)]
	public struct mavlink_sim_state_t(float q1, float q2, float q3, float q4, float roll, float pitch, float yaw, float xacc, float yacc, float zacc, float xgyro, float ygyro, float zgyro, float lat, float lon, float alt, float std_dev_horz, float std_dev_vert, float vn, float ve, float vd, int lat_int, int lon_int)
	{
		[Units("")]
		[Description("True attitude quaternion component 1, w (1 in null-rotation)")]
		public float q1 = q1;

		[Units("")]
		[Description("True attitude quaternion component 2, x (0 in null-rotation)")]
		public float q2 = q2;

		[Units("")]
		[Description("True attitude quaternion component 3, y (0 in null-rotation)")]
		public float q3 = q3;

		[Units("")]
		[Description("True attitude quaternion component 4, z (0 in null-rotation)")]
		public float q4 = q4;

		[Units("[rad]")]
		[Description("Attitude roll expressed as Euler angles, not recommended except for human-readable outputs")]
		public float roll = roll;

		[Units("[rad]")]
		[Description("Attitude pitch expressed as Euler angles, not recommended except for human-readable outputs")]
		public float pitch = pitch;

		[Units("[rad]")]
		[Description("Attitude yaw expressed as Euler angles, not recommended except for human-readable outputs")]
		public float yaw = yaw;

		[Units("[m/s/s]")]
		[Description("X acceleration")]
		public float xacc = xacc;

		[Units("[m/s/s]")]
		[Description("Y acceleration")]
		public float yacc = yacc;

		[Units("[m/s/s]")]
		[Description("Z acceleration")]
		public float zacc = zacc;

		[Units("[rad/s]")]
		[Description("Angular speed around X axis")]
		public float xgyro = xgyro;

		[Units("[rad/s]")]
		[Description("Angular speed around Y axis")]
		public float ygyro = ygyro;

		[Units("[rad/s]")]
		[Description("Angular speed around Z axis")]
		public float zgyro = zgyro;

		[Units("[deg]")]
		[Description("Latitude (lower precision). Both this and the lat_int field should be set.")]
		public float lat = lat;

		[Units("[deg]")]
		[Description("Longitude (lower precision). Both this and the lon_int field should be set.")]
		public float lon = lon;

		[Units("[m]")]
		[Description("Altitude")]
		public float alt = alt;

		[Units("")]
		[Description("Horizontal position standard deviation")]
		public float std_dev_horz = std_dev_horz;

		[Units("")]
		[Description("Vertical position standard deviation")]
		public float std_dev_vert = std_dev_vert;

		[Units("[m/s]")]
		[Description("True velocity in north direction in earth-fixed NED frame")]
		public float vn = vn;

		[Units("[m/s]")]
		[Description("True velocity in east direction in earth-fixed NED frame")]
		public float ve = ve;

		[Units("[m/s]")]
		[Description("True velocity in down direction in earth-fixed NED frame")]
		public float vd = vd;

		[Units("[degE7]")]
		[Description("Latitude (higher precision). If 0, recipients should use the lat field value (otherwise this field is preferred).")]
		public int lat_int = lat_int;

		[Units("[degE7]")]
		[Description("Longitude (higher precision). If 0, recipients should use the lon field value (otherwise this field is preferred).")]
		public int lon_int = lon_int;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 9)]
	public struct mavlink_radio_status_t(ushort rxerrors, ushort @fixed, byte rssi, byte remrssi, byte txbuf, byte noise, byte remnoise)
	{
		[Units("")]
		[Description("Count of radio packet receive errors (since boot).")]
		public ushort rxerrors = rxerrors;

		[Units("")]
		[Description("Count of error corrected radio packets (since boot).")]
		public ushort @fixed = @fixed;

		[Units("")]
		[Description("Local (message sender) received signal strength indication in device-dependent units/scale. Values: [0-254], UINT8_MAX: invalid/unknown.")]
		public byte rssi = rssi;

		[Units("")]
		[Description("Remote (message receiver) signal strength indication in device-dependent units/scale. Values: [0-254], UINT8_MAX: invalid/unknown.")]
		public byte remrssi = remrssi;

		[Units("[%]")]
		[Description("Remaining free transmitter buffer space.")]
		public byte txbuf = txbuf;

		[Units("")]
		[Description("Local background noise level. These are device dependent RSSI values (scale as approx 2x dB on SiK radios). Values: [0-254], UINT8_MAX: invalid/unknown.")]
		public byte noise = noise;

		[Units("")]
		[Description("Remote background noise level. These are device dependent RSSI values (scale as approx 2x dB on SiK radios). Values: [0-254], UINT8_MAX: invalid/unknown.")]
		public byte remnoise = remnoise;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 254)]
	public struct mavlink_file_transfer_protocol_t(byte target_network, byte target_system, byte target_component, byte[] payload)
	{
		[Units("")]
		[Description("Network ID (0 for broadcast)")]
		public byte target_network = target_network;

		[Units("")]
		[Description("System ID (0 for broadcast)")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast)")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 251)]
		[Units("")]
		[Description("Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields. The content/format of this block is defined in https://mavlink.io/en/services/ftp.html.")]
		public byte[] payload = payload;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 18)]
	public struct mavlink_timesync_t(long tc1, long ts1, byte target_system, byte target_component)
	{
		[Units("[ns]")]
		[Description("Time sync timestamp 1. Syncing: 0. Responding: Timestamp of responding component.")]
		public long tc1 = tc1;

		[Units("[ns]")]
		[Description("Time sync timestamp 2. Timestamp of syncing component (mirrored in response).")]
		public long ts1 = ts1;

		[Units("")]
		[Description("Target system id. Request: 0 (broadcast) or id of specific system. Response must contain system id of the requesting component.")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Target component id. Request: 0 (broadcast) or id of specific component. Response must contain component id of the requesting component.")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
	public struct mavlink_camera_trigger_t(ulong time_usec, uint seq)
	{
		[Units("[us]")]
		[Description("Timestamp for image frame (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("Image frame sequence")]
		public uint seq = seq;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 39)]
	public struct mavlink_hil_gps_t(ulong time_usec, int lat, int lon, int alt, ushort eph, ushort epv, ushort vel, short vn, short ve, short vd, ushort cog, byte fix_type, byte satellites_visible, byte id, ushort yaw)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[degE7]")]
		[Description("Latitude (WGS84)")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude (WGS84)")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude (MSL). Positive for up.")]
		public int alt = alt;

		[Units("")]
		[Description("GPS HDOP horizontal dilution of position (unitless * 100). If unknown, set to: UINT16_MAX")]
		public ushort eph = eph;

		[Units("")]
		[Description("GPS VDOP vertical dilution of position (unitless * 100). If unknown, set to: UINT16_MAX")]
		public ushort epv = epv;

		[Units("[cm/s]")]
		[Description("GPS ground speed. If unknown, set to: UINT16_MAX")]
		public ushort vel = vel;

		[Units("[cm/s]")]
		[Description("GPS velocity in north direction in earth-fixed NED frame")]
		public short vn = vn;

		[Units("[cm/s]")]
		[Description("GPS velocity in east direction in earth-fixed NED frame")]
		public short ve = ve;

		[Units("[cm/s]")]
		[Description("GPS velocity in down direction in earth-fixed NED frame")]
		public short vd = vd;

		[Units("[cdeg]")]
		[Description("Course over ground (NOT heading, but direction of movement), 0.0..359.99 degrees. If unknown, set to: UINT16_MAX")]
		public ushort cog = cog;

		[Units("")]
		[Description("0-1: no fix, 2: 2D fix, 3: 3D fix. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix.")]
		public byte fix_type = fix_type;

		[Units("")]
		[Description("Number of satellites visible. If unknown, set to UINT8_MAX")]
		public byte satellites_visible = satellites_visible;

		[Units("")]
		[Description("GPS ID (zero indexed). Used for multiple GPS inputs")]
		public byte id = id;

		[Units("[cdeg]")]
		[Description("Yaw of vehicle relative to Earth's North, zero means not available, use 36000 for north")]
		public ushort yaw = yaw;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 44)]
	public struct mavlink_hil_optical_flow_t(ulong time_usec, uint integration_time_us, float integrated_x, float integrated_y, float integrated_xgyro, float integrated_ygyro, float integrated_zgyro, uint time_delta_distance_us, float distance, short temperature, byte sensor_id, byte quality)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[us]")]
		[Description("Integration time. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the.")]
		public uint integration_time_us = integration_time_us;

		[Units("[rad]")]
		[Description("Flow in radians around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.)")]
		public float integrated_x = integrated_x;

		[Units("[rad]")]
		[Description("Flow in radians around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.)")]
		public float integrated_y = integrated_y;

		[Units("[rad]")]
		[Description("RH rotation around X axis")]
		public float integrated_xgyro = integrated_xgyro;

		[Units("[rad]")]
		[Description("RH rotation around Y axis")]
		public float integrated_ygyro = integrated_ygyro;

		[Units("[rad]")]
		[Description("RH rotation around Z axis")]
		public float integrated_zgyro = integrated_zgyro;

		[Units("[us]")]
		[Description("Time since the distance was sampled.")]
		public uint time_delta_distance_us = time_delta_distance_us;

		[Units("[m]")]
		[Description("Distance to the center of the flow field. Positive value (including zero): distance known. Negative value: Unknown distance.")]
		public float distance = distance;

		[Units("[cdegC]")]
		[Description("Temperature")]
		public short temperature = temperature;

		[Units("")]
		[Description("Sensor ID")]
		public byte sensor_id = sensor_id;

		[Units("")]
		[Description("Optical flow quality / confidence. 0: no valid flow, 255: maximum quality")]
		public byte quality = quality;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 64)]
	public struct mavlink_hil_state_quaternion_t(ulong time_usec, float[] attitude_quaternion, float rollspeed, float pitchspeed, float yawspeed, int lat, int lon, int alt, short vx, short vy, short vz, ushort ind_airspeed, ushort true_airspeed, short xacc, short yacc, short zacc)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Vehicle attitude expressed as normalized quaternion in w, x, y, z order (with 1 0 0 0 being the null-rotation)")]
		public float[] attitude_quaternion = attitude_quaternion;

		[Units("[rad/s]")]
		[Description("Body frame roll / phi angular speed")]
		public float rollspeed = rollspeed;

		[Units("[rad/s]")]
		[Description("Body frame pitch / theta angular speed")]
		public float pitchspeed = pitchspeed;

		[Units("[rad/s]")]
		[Description("Body frame yaw / psi angular speed")]
		public float yawspeed = yawspeed;

		[Units("[degE7]")]
		[Description("Latitude")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude")]
		public int alt = alt;

		[Units("[cm/s]")]
		[Description("Ground X Speed (Latitude)")]
		public short vx = vx;

		[Units("[cm/s]")]
		[Description("Ground Y Speed (Longitude)")]
		public short vy = vy;

		[Units("[cm/s]")]
		[Description("Ground Z Speed (Altitude)")]
		public short vz = vz;

		[Units("[cm/s]")]
		[Description("Indicated airspeed")]
		public ushort ind_airspeed = ind_airspeed;

		[Units("[cm/s]")]
		[Description("True airspeed")]
		public ushort true_airspeed = true_airspeed;

		[Units("[mG]")]
		[Description("X acceleration")]
		public short xacc = xacc;

		[Units("[mG]")]
		[Description("Y acceleration")]
		public short yacc = yacc;

		[Units("[mG]")]
		[Description("Z acceleration")]
		public short zacc = zacc;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
	public struct mavlink_scaled_imu2_t(uint time_boot_ms, short xacc, short yacc, short zacc, short xgyro, short ygyro, short zgyro, short xmag, short ymag, short zmag, short temperature)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[mG]")]
		[Description("X acceleration")]
		public short xacc = xacc;

		[Units("[mG]")]
		[Description("Y acceleration")]
		public short yacc = yacc;

		[Units("[mG]")]
		[Description("Z acceleration")]
		public short zacc = zacc;

		[Units("[mrad/s]")]
		[Description("Angular speed around X axis")]
		public short xgyro = xgyro;

		[Units("[mrad/s]")]
		[Description("Angular speed around Y axis")]
		public short ygyro = ygyro;

		[Units("[mrad/s]")]
		[Description("Angular speed around Z axis")]
		public short zgyro = zgyro;

		[Units("[mgauss]")]
		[Description("X Magnetic field")]
		public short xmag = xmag;

		[Units("[mgauss]")]
		[Description("Y Magnetic field")]
		public short ymag = ymag;

		[Units("[mgauss]")]
		[Description("Z Magnetic field")]
		public short zmag = zmag;

		[Units("[cdegC]")]
		[Description("Temperature, 0: IMU does not provide temperature values. If the IMU is at 0C it must send 1 (0.01C).")]
		public short temperature = temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
	public struct mavlink_log_request_list_t(ushort start, ushort end, byte target_system, byte target_component)
	{
		[Units("")]
		[Description("First log id (0 for first available)")]
		public ushort start = start;

		[Units("")]
		[Description("Last log id (0xffff for last available)")]
		public ushort end = end;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 14)]
	public struct mavlink_log_entry_t(uint time_utc, uint size, ushort id, ushort num_logs, ushort last_log_num)
	{
		[Units("[s]")]
		[Description("UTC timestamp of log since 1970, or 0 if not available")]
		public uint time_utc = time_utc;

		[Units("[bytes]")]
		[Description("Size of the log (may be approximate)")]
		public uint size = size;

		[Units("")]
		[Description("Log id")]
		public ushort id = id;

		[Units("")]
		[Description("Total number of logs")]
		public ushort num_logs = num_logs;

		[Units("")]
		[Description("High log number")]
		public ushort last_log_num = last_log_num;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
	public struct mavlink_log_request_data_t(uint ofs, uint count, ushort id, byte target_system, byte target_component)
	{
		[Units("")]
		[Description("Offset into the log")]
		public uint ofs = ofs;

		[Units("[bytes]")]
		[Description("Number of bytes")]
		public uint count = count;

		[Units("")]
		[Description("Log id (from LOG_ENTRY reply)")]
		public ushort id = id;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 97)]
	public struct mavlink_log_data_t(uint ofs, ushort id, byte count, byte[] data)
	{
		[Units("")]
		[Description("Offset into the log")]
		public uint ofs = ofs;

		[Units("")]
		[Description("Log id (from LOG_ENTRY reply)")]
		public ushort id = id;

		[Units("[bytes]")]
		[Description("Number of bytes (zero for end of log)")]
		public byte count = count;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 90)]
		[Units("")]
		[Description("log data")]
		public byte[] data = data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
	public struct mavlink_log_erase_t(byte target_system, byte target_component)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
	public struct mavlink_log_request_end_t(byte target_system, byte target_component)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 113)]
	public struct mavlink_gps_inject_data_t(byte target_system, byte target_component, byte len, byte[] data)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("[bytes]")]
		[Description("Data length")]
		public byte len = len;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 110)]
		[Units("")]
		[Description("Raw data (110 is enough for 12 satellites of RTCMv2)")]
		public byte[] data = data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 57)]
	public struct mavlink_gps2_raw_t(ulong time_usec, int lat, int lon, int alt, uint dgps_age, ushort eph, ushort epv, ushort vel, ushort cog, byte fix_type, byte satellites_visible, byte dgps_numch, ushort yaw, int alt_ellipsoid, uint h_acc, uint v_acc, uint vel_acc, uint hdg_acc)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[degE7]")]
		[Description("Latitude (WGS84)")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude (WGS84)")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude (MSL). Positive for up.")]
		public int alt = alt;

		[Units("[ms]")]
		[Description("Age of DGPS info")]
		public uint dgps_age = dgps_age;

		[Units("")]
		[Description("GPS HDOP horizontal dilution of position (unitless * 100). If unknown, set to: UINT16_MAX")]
		public ushort eph = eph;

		[Units("")]
		[Description("GPS VDOP vertical dilution of position (unitless * 100). If unknown, set to: UINT16_MAX")]
		public ushort epv = epv;

		[Units("[cm/s]")]
		[Description("GPS ground speed. If unknown, set to: UINT16_MAX")]
		public ushort vel = vel;

		[Units("[cdeg]")]
		[Description("Course over ground (NOT heading, but direction of movement): 0.0..359.99 degrees. If unknown, set to: UINT16_MAX")]
		public ushort cog = cog;

		[Units("")]
		[Description("GPS fix type.")]
		public byte fix_type = fix_type;

		[Units("")]
		[Description("Number of satellites visible. If unknown, set to UINT8_MAX")]
		public byte satellites_visible = satellites_visible;

		[Units("")]
		[Description("Number of DGPS satellites")]
		public byte dgps_numch = dgps_numch;

		[Units("[cdeg]")]
		[Description("Yaw in earth frame from north. Use 0 if this GPS does not provide yaw. Use UINT16_MAX if this GPS is configured to provide yaw and is currently unable to provide it. Use 36000 for north.")]
		public ushort yaw = yaw;

		[Units("[mm]")]
		[Description("Altitude (above WGS84, EGM96 ellipsoid). Positive for up.")]
		public int alt_ellipsoid = alt_ellipsoid;

		[Units("[mm]")]
		[Description("Position uncertainty.")]
		public uint h_acc = h_acc;

		[Units("[mm]")]
		[Description("Altitude uncertainty.")]
		public uint v_acc = v_acc;

		[Units("[mm/s]")]
		[Description("Speed uncertainty.")]
		public uint vel_acc = vel_acc;

		[Units("[degE5]")]
		[Description("Heading / track uncertainty")]
		public uint hdg_acc = hdg_acc;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
	public struct mavlink_power_status_t(ushort Vcc, ushort Vservo, ushort flags)
	{
		[Units("[mV]")]
		[Description("5V rail voltage.")]
		public ushort Vcc = Vcc;

		[Units("[mV]")]
		[Description("Servo rail voltage.")]
		public ushort Vservo = Vservo;

		[Units("")]
		[Description("Bitmap of power supply status flags.")]
		public ushort flags = flags;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 81)]
	public struct mavlink_serial_control_t(uint baudrate, ushort timeout, byte device, byte flags, byte count, byte[] data, byte target_system, byte target_component)
	{
		[Units("[bits/s]")]
		[Description("Baudrate of transfer. Zero means no change.")]
		public uint baudrate = baudrate;

		[Units("[ms]")]
		[Description("Timeout for reply data")]
		public ushort timeout = timeout;

		[Units("")]
		[Description("Serial control device type.")]
		public byte device = device;

		[Units("")]
		[Description("Bitmap of serial control flags.")]
		public byte flags = flags;

		[Units("[bytes]")]
		[Description("how many bytes in this transfer")]
		public byte count = count;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 70)]
		[Units("")]
		[Description("serial data")]
		public byte[] data = data;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 35)]
	public struct mavlink_gps_rtk_t(uint time_last_baseline_ms, uint tow, int baseline_a_mm, int baseline_b_mm, int baseline_c_mm, uint accuracy, int iar_num_hypotheses, ushort wn, byte rtk_receiver_id, byte rtk_health, byte rtk_rate, byte nsats, byte baseline_coords_type)
	{
		[Units("[ms]")]
		[Description("Time since boot of last baseline message received.")]
		public uint time_last_baseline_ms = time_last_baseline_ms;

		[Units("[ms]")]
		[Description("GPS Time of Week of last baseline")]
		public uint tow = tow;

		[Units("[mm]")]
		[Description("Current baseline in ECEF x or NED north component.")]
		public int baseline_a_mm = baseline_a_mm;

		[Units("[mm]")]
		[Description("Current baseline in ECEF y or NED east component.")]
		public int baseline_b_mm = baseline_b_mm;

		[Units("[mm]")]
		[Description("Current baseline in ECEF z or NED down component.")]
		public int baseline_c_mm = baseline_c_mm;

		[Units("")]
		[Description("Current estimate of baseline accuracy.")]
		public uint accuracy = accuracy;

		[Units("")]
		[Description("Current number of integer ambiguity hypotheses.")]
		public int iar_num_hypotheses = iar_num_hypotheses;

		[Units("")]
		[Description("GPS Week Number of last baseline")]
		public ushort wn = wn;

		[Units("")]
		[Description("Identification of connected RTK receiver.")]
		public byte rtk_receiver_id = rtk_receiver_id;

		[Units("")]
		[Description("GPS-specific health report for RTK data.")]
		public byte rtk_health = rtk_health;

		[Units("[Hz]")]
		[Description("Rate of baseline messages being received by GPS")]
		public byte rtk_rate = rtk_rate;

		[Units("")]
		[Description("Current number of sats used for RTK calculation.")]
		public byte nsats = nsats;

		[Units("")]
		[Description("Coordinate system of baseline")]
		public byte baseline_coords_type = baseline_coords_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 35)]
	public struct mavlink_gps2_rtk_t(uint time_last_baseline_ms, uint tow, int baseline_a_mm, int baseline_b_mm, int baseline_c_mm, uint accuracy, int iar_num_hypotheses, ushort wn, byte rtk_receiver_id, byte rtk_health, byte rtk_rate, byte nsats, byte baseline_coords_type)
	{
		[Units("[ms]")]
		[Description("Time since boot of last baseline message received.")]
		public uint time_last_baseline_ms = time_last_baseline_ms;

		[Units("[ms]")]
		[Description("GPS Time of Week of last baseline")]
		public uint tow = tow;

		[Units("[mm]")]
		[Description("Current baseline in ECEF x or NED north component.")]
		public int baseline_a_mm = baseline_a_mm;

		[Units("[mm]")]
		[Description("Current baseline in ECEF y or NED east component.")]
		public int baseline_b_mm = baseline_b_mm;

		[Units("[mm]")]
		[Description("Current baseline in ECEF z or NED down component.")]
		public int baseline_c_mm = baseline_c_mm;

		[Units("")]
		[Description("Current estimate of baseline accuracy.")]
		public uint accuracy = accuracy;

		[Units("")]
		[Description("Current number of integer ambiguity hypotheses.")]
		public int iar_num_hypotheses = iar_num_hypotheses;

		[Units("")]
		[Description("GPS Week Number of last baseline")]
		public ushort wn = wn;

		[Units("")]
		[Description("Identification of connected RTK receiver.")]
		public byte rtk_receiver_id = rtk_receiver_id;

		[Units("")]
		[Description("GPS-specific health report for RTK data.")]
		public byte rtk_health = rtk_health;

		[Units("[Hz]")]
		[Description("Rate of baseline messages being received by GPS")]
		public byte rtk_rate = rtk_rate;

		[Units("")]
		[Description("Current number of sats used for RTK calculation.")]
		public byte nsats = nsats;

		[Units("")]
		[Description("Coordinate system of baseline")]
		public byte baseline_coords_type = baseline_coords_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
	public struct mavlink_scaled_imu3_t(uint time_boot_ms, short xacc, short yacc, short zacc, short xgyro, short ygyro, short zgyro, short xmag, short ymag, short zmag, short temperature)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[mG]")]
		[Description("X acceleration")]
		public short xacc = xacc;

		[Units("[mG]")]
		[Description("Y acceleration")]
		public short yacc = yacc;

		[Units("[mG]")]
		[Description("Z acceleration")]
		public short zacc = zacc;

		[Units("[mrad/s]")]
		[Description("Angular speed around X axis")]
		public short xgyro = xgyro;

		[Units("[mrad/s]")]
		[Description("Angular speed around Y axis")]
		public short ygyro = ygyro;

		[Units("[mrad/s]")]
		[Description("Angular speed around Z axis")]
		public short zgyro = zgyro;

		[Units("[mgauss]")]
		[Description("X Magnetic field")]
		public short xmag = xmag;

		[Units("[mgauss]")]
		[Description("Y Magnetic field")]
		public short ymag = ymag;

		[Units("[mgauss]")]
		[Description("Z Magnetic field")]
		public short zmag = zmag;

		[Units("[cdegC]")]
		[Description("Temperature, 0: IMU does not provide temperature values. If the IMU is at 0C it must send 1 (0.01C).")]
		public short temperature = temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 13)]
	public struct mavlink_data_transmission_handshake_t(uint size, ushort width, ushort height, ushort packets, byte type, byte payload, byte jpg_quality)
	{
		[Units("[bytes]")]
		[Description("total data size (set on ACK only).")]
		public uint size = size;

		[Units("")]
		[Description("Width of a matrix or image.")]
		public ushort width = width;

		[Units("")]
		[Description("Height of a matrix or image.")]
		public ushort height = height;

		[Units("")]
		[Description("Number of packets being sent (set on ACK only).")]
		public ushort packets = packets;

		[Units("")]
		[Description("Type of requested/acknowledged data.")]
		public byte type = type;

		[Units("[bytes]")]
		[Description("Payload size per packet (normally 253 byte, see DATA field size in message ENCAPSULATED_DATA) (set on ACK only).")]
		public byte payload = payload;

		[Units("[%]")]
		[Description("JPEG quality. Values: [1-100].")]
		public byte jpg_quality = jpg_quality;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 255)]
	public struct mavlink_encapsulated_data_t(ushort seqnr, byte[] data)
	{
		[Units("")]
		[Description("sequence number (starting with 0 on every transmission)")]
		public ushort seqnr = seqnr;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 253)]
		[Units("")]
		[Description("image data bytes")]
		public byte[] data = data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 39)]
	public struct mavlink_distance_sensor_t(uint time_boot_ms, ushort min_distance, ushort max_distance, ushort current_distance, byte type, byte id, byte orientation, byte covariance, float horizontal_fov, float vertical_fov, float[] quaternion, byte signal_quality)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[cm]")]
		[Description("Minimum distance the sensor can measure")]
		public ushort min_distance = min_distance;

		[Units("[cm]")]
		[Description("Maximum distance the sensor can measure")]
		public ushort max_distance = max_distance;

		[Units("[cm]")]
		[Description("Current distance reading")]
		public ushort current_distance = current_distance;

		[Units("")]
		[Description("Type of distance sensor.")]
		public byte type = type;

		[Units("")]
		[Description("Onboard ID of the sensor")]
		public byte id = id;

		[Units("")]
		[Description("Direction the sensor faces. downward-facing: ROTATION_PITCH_270, upward-facing: ROTATION_PITCH_90, backward-facing: ROTATION_PITCH_180, forward-facing: ROTATION_NONE, left-facing: ROTATION_YAW_90, right-facing: ROTATION_YAW_270")]
		public byte orientation = orientation;

		[Units("[cm^2]")]
		[Description("Measurement variance. Max standard deviation is 6cm. UINT8_MAX if unknown.")]
		public byte covariance = covariance;

		[Units("[rad]")]
		[Description("Horizontal Field of View (angle) where the distance measurement is valid and the field of view is known. Otherwise this is set to 0.")]
		public float horizontal_fov = horizontal_fov;

		[Units("[rad]")]
		[Description("Vertical Field of View (angle) where the distance measurement is valid and the field of view is known. Otherwise this is set to 0.")]
		public float vertical_fov = vertical_fov;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion of the sensor orientation in vehicle body frame (w, x, y, z order, zero-rotation is 1, 0, 0, 0). Zero-rotation is along the vehicle body x-axis. This field is required if the orientation is set to MAV_SENSOR_ROTATION_CUSTOM. Set it to 0 if invalid.'")]
		public float[] quaternion = quaternion;

		[Units("[%]")]
		[Description("Signal quality of the sensor. Specific to each sensor type, representing the relation of the signal strength with the target reflectivity, distance, size or aspect, but normalised as a percentage. 0 = unknown/unset signal quality, 1 = invalid signal, 100 = perfect signal.")]
		public byte signal_quality = signal_quality;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 18)]
	public struct mavlink_terrain_request_t(ulong mask, int lat, int lon, ushort grid_spacing)
	{
		[Units("")]
		[Description("Bitmask of requested 4x4 grids (row major 8x7 array of grids, 56 bits)")]
		public ulong mask = mask;

		[Units("[degE7]")]
		[Description("Latitude of SW corner of first grid")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude of SW corner of first grid")]
		public int lon = lon;

		[Units("[m]")]
		[Description("Grid spacing")]
		public ushort grid_spacing = grid_spacing;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 43)]
	public struct mavlink_terrain_data_t(int lat, int lon, ushort grid_spacing, short[] data, byte gridbit)
	{
		[Units("[degE7]")]
		[Description("Latitude of SW corner of first grid")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude of SW corner of first grid")]
		public int lon = lon;

		[Units("[m]")]
		[Description("Grid spacing")]
		public ushort grid_spacing = grid_spacing;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("[m]")]
		[Description("Terrain data MSL")]
		public short[] data = data;

		[Units("")]
		[Description("bit within the terrain request mask")]
		public byte gridbit = gridbit;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
	public struct mavlink_terrain_check_t(int lat, int lon)
	{
		[Units("[degE7]")]
		[Description("Latitude")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude")]
		public int lon = lon;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 22)]
	public struct mavlink_terrain_report_t(int lat, int lon, float terrain_height, float current_height, ushort spacing, ushort pending, ushort loaded)
	{
		[Units("[degE7]")]
		[Description("Latitude")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude")]
		public int lon = lon;

		[Units("[m]")]
		[Description("Terrain height MSL")]
		public float terrain_height = terrain_height;

		[Units("[m]")]
		[Description("Current vehicle height above lat/lon terrain height")]
		public float current_height = current_height;

		[Units("")]
		[Description("grid spacing (zero if terrain at this location unavailable)")]
		public ushort spacing = spacing;

		[Units("")]
		[Description("Number of 4x4 terrain blocks waiting to be received or read from disk")]
		public ushort pending = pending;

		[Units("")]
		[Description("Number of 4x4 terrain blocks in memory")]
		public ushort loaded = loaded;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct mavlink_scaled_pressure2_t(uint time_boot_ms, float press_abs, float press_diff, short temperature, short temperature_press_diff)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[hPa]")]
		[Description("Absolute pressure")]
		public float press_abs = press_abs;

		[Units("[hPa]")]
		[Description("Differential pressure")]
		public float press_diff = press_diff;

		[Units("[cdegC]")]
		[Description("Absolute pressure temperature")]
		public short temperature = temperature;

		[Units("[cdegC]")]
		[Description("Differential pressure temperature (0, if not available). Report values of 0 (or 1) as 1 cdegC.")]
		public short temperature_press_diff = temperature_press_diff;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 120)]
	public struct mavlink_att_pos_mocap_t(ulong time_usec, float[] q, float x, float y, float z, float[] covariance)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0)")]
		public float[] q = q;

		[Units("[m]")]
		[Description("X position (NED)")]
		public float x = x;

		[Units("[m]")]
		[Description("Y position (NED)")]
		public float y = y;

		[Units("[m]")]
		[Description("Z position (NED)")]
		public float z = z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
		[Units("")]
		[Description("Row-major representation of a pose 6x6 cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] covariance = covariance;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 43)]
	public struct mavlink_set_actuator_control_target_t(ulong time_usec, float[] controls, byte group_mlx, byte target_system, byte target_component)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("")]
		[Description("Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs.")]
		public float[] controls = controls;

		[Units("")]
		[Description("Actuator group. The '_mlx' indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances.")]
		public byte group_mlx = group_mlx;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 41)]
	public struct mavlink_actuator_control_target_t(ulong time_usec, float[] controls, byte group_mlx)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("")]
		[Description("Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs.")]
		public float[] controls = controls;

		[Units("")]
		[Description("Actuator group. The '_mlx' indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances.")]
		public byte group_mlx = group_mlx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
	public struct mavlink_altitude_t(ulong time_usec, float altitude_monotonic, float altitude_amsl, float altitude_local, float altitude_relative, float altitude_terrain, float bottom_clearance)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[m]")]
		[Description("This altitude measure is initialized on system boot and monotonic (it is never reset, but represents the local altitude change). The only guarantee on this field is that it will never be reset and is consistent within a flight. The recommended value for this field is the uncorrected barometric altitude at boot time. This altitude will also drift and vary between flights.")]
		public float altitude_monotonic = altitude_monotonic;

		[Units("[m]")]
		[Description("This altitude measure is strictly above mean sea level and might be non-monotonic (it might reset on events like GPS lock or when a new QNH value is set). It should be the altitude to which global altitude waypoints are compared to. Note that it is *not* the GPS altitude, however, most GPS modules already output MSL by default and not the WGS84 altitude.")]
		public float altitude_amsl = altitude_amsl;

		[Units("[m]")]
		[Description("This is the local altitude in the local coordinate frame. It is not the altitude above home, but in reference to the coordinate origin (0, 0, 0). It is up-positive.")]
		public float altitude_local = altitude_local;

		[Units("[m]")]
		[Description("This is the altitude above the home position. It resets on each change of the current home position.")]
		public float altitude_relative = altitude_relative;

		[Units("[m]")]
		[Description("This is the altitude above terrain. It might be fed by a terrain database or an altimeter. Values smaller than -1000 should be interpreted as unknown.")]
		public float altitude_terrain = altitude_terrain;

		[Units("[m]")]
		[Description("This is not the altitude, but the clear space below the system according to the fused clearance estimate. It generally should max out at the maximum range of e.g. the laser altimeter. It is generally a moving target. A negative value indicates no measurement available.")]
		public float bottom_clearance = bottom_clearance;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 243)]
	public struct mavlink_resource_request_t(byte request_id, byte uri_type, byte[] uri, byte transfer_type, byte[] storage)
	{
		[Units("")]
		[Description("Request ID. This ID should be reused when sending back URI contents")]
		public byte request_id = request_id;

		[Units("")]
		[Description("The type of requested URI. 0 = a file via URL. 1 = a UAVCAN binary")]
		public byte uri_type = uri_type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 120)]
		[Units("")]
		[Description("The requested unique resource identifier (URI). It is not necessarily a straight domain name (depends on the URI type enum)")]
		public byte[] uri = uri;

		[Units("")]
		[Description("The way the autopilot wants to receive the URI. 0 = MAVLink FTP. 1 = binary stream.")]
		public byte transfer_type = transfer_type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 120)]
		[Units("")]
		[Description("The storage path the autopilot wants the URI to be stored in. Will only be valid if the transfer_type has a storage associated (e.g. MAVLink FTP).")]
		public byte[] storage = storage;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct mavlink_scaled_pressure3_t(uint time_boot_ms, float press_abs, float press_diff, short temperature, short temperature_press_diff)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[hPa]")]
		[Description("Absolute pressure")]
		public float press_abs = press_abs;

		[Units("[hPa]")]
		[Description("Differential pressure")]
		public float press_diff = press_diff;

		[Units("[cdegC]")]
		[Description("Absolute pressure temperature")]
		public short temperature = temperature;

		[Units("[cdegC]")]
		[Description("Differential pressure temperature (0, if not available). Report values of 0 (or 1) as 1 cdegC.")]
		public short temperature_press_diff = temperature_press_diff;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 93)]
	public struct mavlink_follow_target_t(ulong timestamp, ulong custom_state, int lat, int lon, float alt, float[] vel, float[] acc, float[] attitude_q, float[] rates, float[] position_cov, byte est_capabilities)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public ulong timestamp = timestamp;

		[Units("")]
		[Description("button states or switches of a tracker device")]
		public ulong custom_state = custom_state;

		[Units("[degE7]")]
		[Description("Latitude (WGS84)")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude (WGS84)")]
		public int lon = lon;

		[Units("[m]")]
		[Description("Altitude (MSL)")]
		public float alt = alt;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		[Units("[m/s]")]
		[Description("target velocity (0,0,0) for unknown")]
		public float[] vel = vel;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		[Units("[m/s/s]")]
		[Description("linear target acceleration (0,0,0) for unknown")]
		public float[] acc = acc;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("(0 0 0 0 for unknown)")]
		public float[] attitude_q = attitude_q;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		[Units("")]
		[Description("(0 0 0 for unknown)")]
		public float[] rates = rates;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		[Units("")]
		[Description("eph epv")]
		public float[] position_cov = position_cov;

		[Units("")]
		[Description("bit positions for tracker reporting capabilities (POS = 0, VEL = 1, ACCEL = 2, ATT + RATES = 3)")]
		public byte est_capabilities = est_capabilities;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 100)]
	public struct mavlink_control_system_state_t(ulong time_usec, float x_acc, float y_acc, float z_acc, float x_vel, float y_vel, float z_vel, float x_pos, float y_pos, float z_pos, float airspeed, float[] vel_variance, float[] pos_variance, float[] q, float roll_rate, float pitch_rate, float yaw_rate)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[m/s/s]")]
		[Description("X acceleration in body frame")]
		public float x_acc = x_acc;

		[Units("[m/s/s]")]
		[Description("Y acceleration in body frame")]
		public float y_acc = y_acc;

		[Units("[m/s/s]")]
		[Description("Z acceleration in body frame")]
		public float z_acc = z_acc;

		[Units("[m/s]")]
		[Description("X velocity in body frame")]
		public float x_vel = x_vel;

		[Units("[m/s]")]
		[Description("Y velocity in body frame")]
		public float y_vel = y_vel;

		[Units("[m/s]")]
		[Description("Z velocity in body frame")]
		public float z_vel = z_vel;

		[Units("[m]")]
		[Description("X position in local frame")]
		public float x_pos = x_pos;

		[Units("[m]")]
		[Description("Y position in local frame")]
		public float y_pos = y_pos;

		[Units("[m]")]
		[Description("Z position in local frame")]
		public float z_pos = z_pos;

		[Units("[m/s]")]
		[Description("Airspeed, set to -1 if unknown")]
		public float airspeed = airspeed;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		[Units("")]
		[Description("Variance of body velocity estimate")]
		public float[] vel_variance = vel_variance;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		[Units("")]
		[Description("Variance in local position")]
		public float[] pos_variance = pos_variance;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("The attitude, represented as Quaternion")]
		public float[] q = q;

		[Units("[rad/s]")]
		[Description("Angular rate in roll axis")]
		public float roll_rate = roll_rate;

		[Units("[rad/s]")]
		[Description("Angular rate in pitch axis")]
		public float pitch_rate = pitch_rate;

		[Units("[rad/s]")]
		[Description("Angular rate in yaw axis")]
		public float yaw_rate = yaw_rate;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 54)]
	public struct mavlink_battery_status_t(int current_consumed, int energy_consumed, short temperature, ushort[] voltages, short current_battery, byte id, byte battery_function, byte type, sbyte battery_remaining, int time_remaining, byte charge_state, ushort[] voltages_ext, byte mode, uint fault_bitmask)
	{
		[Units("[mAh]")]
		[Description("Consumed charge, -1: autopilot does not provide consumption estimate")]
		public int current_consumed = current_consumed;

		[Units("[hJ]")]
		[Description("Consumed energy, -1: autopilot does not provide energy consumption estimate")]
		public int energy_consumed = energy_consumed;

		[Units("[cdegC]")]
		[Description("Temperature of the battery. INT16_MAX for unknown temperature.")]
		public short temperature = temperature;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		[Units("[mV]")]
		[Description("Battery voltage of cells 1 to 10 (see voltages_ext for cells 11-14). Cells in this field above the valid cell count for this battery should have the UINT16_MAX value. If individual cell voltages are unknown or not measured for this battery, then the overall battery voltage should be filled in cell 0, with all others set to UINT16_MAX. If the voltage of the battery is greater than (UINT16_MAX - 1), then cell 0 should be set to (UINT16_MAX - 1), and cell 1 to the remaining voltage. This can be extended to multiple cells if the total voltage is greater than 2 * (UINT16_MAX - 1).")]
		public ushort[] voltages = voltages;

		[Units("[cA]")]
		[Description("Battery current, -1: autopilot does not measure the current")]
		public short current_battery = current_battery;

		[Units("")]
		[Description("Battery ID")]
		public byte id = id;

		[Units("")]
		[Description("Function of the battery")]
		public byte battery_function = battery_function;

		[Units("")]
		[Description("Type (chemistry) of the battery")]
		public byte type = type;

		[Units("[%]")]
		[Description("Remaining battery energy. Values: [0-100], -1: autopilot does not estimate the remaining battery.")]
		public sbyte battery_remaining = battery_remaining;

		[Units("[s]")]
		[Description("Remaining battery time, 0: autopilot does not provide remaining battery time estimate")]
		public int time_remaining = time_remaining;

		[Units("")]
		[Description("State for extent of discharge, provided by autopilot for warning or external reactions")]
		public byte charge_state = charge_state;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("[mV]")]
		[Description("Battery voltages for cells 11 to 14. Cells above the valid cell count for this battery should have a value of 0, where zero indicates not supported (note, this is different than for the voltages field and allows empty byte truncation). If the measured value is 0 then 1 should be sent instead.")]
		public ushort[] voltages_ext = voltages_ext;

		[Units("")]
		[Description("Battery mode. Default (0) is that battery mode reporting is not supported or battery is in normal-use mode.")]
		public byte mode = mode;

		[Units("")]
		[Description("Fault/health indications. These should be set when charge_state is MAV_BATTERY_CHARGE_STATE_FAILED or MAV_BATTERY_CHARGE_STATE_UNHEALTHY (if not, fault reporting is not supported).")]
		public uint fault_bitmask = fault_bitmask;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 60)]
	public struct mavlink_landing_target_t(ulong time_usec, float angle_x, float angle_y, float distance, float size_x, float size_y, byte target_num, byte frame, float x, float y, float z, float[] q, byte type, byte position_valid)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[rad]")]
		[Description("X-axis angular offset of the target from the center of the image")]
		public float angle_x = angle_x;

		[Units("[rad]")]
		[Description("Y-axis angular offset of the target from the center of the image")]
		public float angle_y = angle_y;

		[Units("[m]")]
		[Description("Distance to the target from the vehicle")]
		public float distance = distance;

		[Units("[rad]")]
		[Description("Size of target along x-axis")]
		public float size_x = size_x;

		[Units("[rad]")]
		[Description("Size of target along y-axis")]
		public float size_y = size_y;

		[Units("")]
		[Description("The ID of the target if multiple targets are present")]
		public byte target_num = target_num;

		[Units("")]
		[Description("Coordinate frame used for following fields.")]
		public byte frame = frame;

		[Units("[m]")]
		[Description("X Position of the landing target in MAV_FRAME")]
		public float x = x;

		[Units("[m]")]
		[Description("Y Position of the landing target in MAV_FRAME")]
		public float y = y;

		[Units("[m]")]
		[Description("Z Position of the landing target in MAV_FRAME")]
		public float z = z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion of landing target orientation (w, x, y, z order, zero-rotation is 1, 0, 0, 0)")]
		public float[] q = q;

		[Units("")]
		[Description("Type of landing target")]
		public byte type = type;

		[Units("")]
		[Description("Position fields (x, y, z, q, type) contain valid target position information (MAV_BOOL_FALSE: invalid values). Values not equal to 0 or 1 are invalid.")]
		public byte position_valid = position_valid;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 9)]
	public struct mavlink_fence_status_t(uint breach_time, ushort breach_count, byte breach_status, byte breach_type, byte breach_mitigation)
	{
		[Units("[ms]")]
		[Description("Time (since boot) of last breach.")]
		public uint breach_time = breach_time;

		[Units("")]
		[Description("Number of fence breaches.")]
		public ushort breach_count = breach_count;

		[Units("")]
		[Description("Breach status (0 if currently inside fence, 1 if outside).")]
		public byte breach_status = breach_status;

		[Units("")]
		[Description("Last breach type.")]
		public byte breach_type = breach_type;

		[Units("")]
		[Description("Active action to prevent fence breach")]
		public byte breach_mitigation = breach_mitigation;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 54)]
	public struct mavlink_mag_cal_report_t(float fitness, float ofs_x, float ofs_y, float ofs_z, float diag_x, float diag_y, float diag_z, float offdiag_x, float offdiag_y, float offdiag_z, byte compass_id, byte cal_mask, byte cal_status, byte autosaved, float orientation_confidence, byte old_orientation, byte new_orientation, float scale_factor)
	{
		[Units("[mgauss]")]
		[Description("RMS milligauss residuals.")]
		public float fitness = fitness;

		[Units("")]
		[Description("X offset.")]
		public float ofs_x = ofs_x;

		[Units("")]
		[Description("Y offset.")]
		public float ofs_y = ofs_y;

		[Units("")]
		[Description("Z offset.")]
		public float ofs_z = ofs_z;

		[Units("")]
		[Description("X diagonal (matrix 11).")]
		public float diag_x = diag_x;

		[Units("")]
		[Description("Y diagonal (matrix 22).")]
		public float diag_y = diag_y;

		[Units("")]
		[Description("Z diagonal (matrix 33).")]
		public float diag_z = diag_z;

		[Units("")]
		[Description("X off-diagonal (matrix 12 and 21).")]
		public float offdiag_x = offdiag_x;

		[Units("")]
		[Description("Y off-diagonal (matrix 13 and 31).")]
		public float offdiag_y = offdiag_y;

		[Units("")]
		[Description("Z off-diagonal (matrix 32 and 23).")]
		public float offdiag_z = offdiag_z;

		[Units("")]
		[Description("Compass being calibrated.")]
		public byte compass_id = compass_id;

		[Units("")]
		[Description("Bitmask of compasses being calibrated.")]
		public byte cal_mask = cal_mask;

		[Units("")]
		[Description("Calibration Status.")]
		public byte cal_status = cal_status;

		[Units("")]
		[Description("0=requires a MAV_CMD_DO_ACCEPT_MAG_CAL, 1=saved to parameters.")]
		public byte autosaved = autosaved;

		[Units("")]
		[Description("Confidence in orientation (higher is better).")]
		public float orientation_confidence = orientation_confidence;

		[Units("")]
		[Description("orientation before calibration.")]
		public byte old_orientation = old_orientation;

		[Units("")]
		[Description("orientation after calibration.")]
		public byte new_orientation = new_orientation;

		[Units("")]
		[Description("field radius correction factor")]
		public float scale_factor = scale_factor;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 73)]
	public struct mavlink_efi_status_t(float ecu_index, float rpm, float fuel_consumed, float fuel_flow, float engine_load, float throttle_position, float spark_dwell_time, float barometric_pressure, float intake_manifold_pressure, float intake_manifold_temperature, float cylinder_head_temperature, float ignition_timing, float injection_time, float exhaust_gas_temperature, float throttle_out, float pt_compensation, byte health, float ignition_voltage, float fuel_pressure)
	{
		[Units("")]
		[Description("ECU index")]
		public float ecu_index = ecu_index;

		[Units("")]
		[Description("RPM")]
		public float rpm = rpm;

		[Units("[cm^3]")]
		[Description("Fuel consumed")]
		public float fuel_consumed = fuel_consumed;

		[Units("[cm^3/min]")]
		[Description("Fuel flow rate")]
		public float fuel_flow = fuel_flow;

		[Units("[%]")]
		[Description("Engine load")]
		public float engine_load = engine_load;

		[Units("[%]")]
		[Description("Throttle position")]
		public float throttle_position = throttle_position;

		[Units("[ms]")]
		[Description("Spark dwell time")]
		public float spark_dwell_time = spark_dwell_time;

		[Units("[kPa]")]
		[Description("Barometric pressure")]
		public float barometric_pressure = barometric_pressure;

		[Units("[kPa]")]
		[Description("Intake manifold pressure(")]
		public float intake_manifold_pressure = intake_manifold_pressure;

		[Units("[degC]")]
		[Description("Intake manifold temperature")]
		public float intake_manifold_temperature = intake_manifold_temperature;

		[Units("[degC]")]
		[Description("Cylinder head temperature")]
		public float cylinder_head_temperature = cylinder_head_temperature;

		[Units("[deg]")]
		[Description("Ignition timing (Crank angle degrees)")]
		public float ignition_timing = ignition_timing;

		[Units("[ms]")]
		[Description("Injection time")]
		public float injection_time = injection_time;

		[Units("[degC]")]
		[Description("Exhaust gas temperature")]
		public float exhaust_gas_temperature = exhaust_gas_temperature;

		[Units("[%]")]
		[Description("Output throttle")]
		public float throttle_out = throttle_out;

		[Units("")]
		[Description("Pressure/temperature compensation")]
		public float pt_compensation = pt_compensation;

		[Units("")]
		[Description("EFI health status")]
		public byte health = health;

		[Units("[V]")]
		[Description("Supply voltage to EFI sparking system.  Zero in this value means 'unknown', so if the supply voltage really is zero volts use 0.0001 instead.")]
		public float ignition_voltage = ignition_voltage;

		[Units("[kPa]")]
		[Description("Fuel pressure. Zero in this value means 'unknown', so if the fuel pressure really is zero kPa use 0.0001 instead.")]
		public float fuel_pressure = fuel_pressure;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 42)]
	public struct mavlink_estimator_status_t(ulong time_usec, float vel_ratio, float pos_horiz_ratio, float pos_vert_ratio, float mag_ratio, float hagl_ratio, float tas_ratio, float pos_horiz_accuracy, float pos_vert_accuracy, ushort flags)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("Velocity innovation test ratio")]
		public float vel_ratio = vel_ratio;

		[Units("")]
		[Description("Horizontal position innovation test ratio")]
		public float pos_horiz_ratio = pos_horiz_ratio;

		[Units("")]
		[Description("Vertical position innovation test ratio")]
		public float pos_vert_ratio = pos_vert_ratio;

		[Units("")]
		[Description("Magnetometer innovation test ratio")]
		public float mag_ratio = mag_ratio;

		[Units("")]
		[Description("Height above terrain innovation test ratio")]
		public float hagl_ratio = hagl_ratio;

		[Units("")]
		[Description("True airspeed innovation test ratio")]
		public float tas_ratio = tas_ratio;

		[Units("[m]")]
		[Description("Horizontal position 1-STD accuracy relative to the EKF local origin")]
		public float pos_horiz_accuracy = pos_horiz_accuracy;

		[Units("[m]")]
		[Description("Vertical position 1-STD accuracy relative to the EKF local origin")]
		public float pos_vert_accuracy = pos_vert_accuracy;

		[Units("")]
		[Description("Bitmap indicating which EKF outputs are valid.")]
		public ushort flags = flags;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 40)]
	public struct mavlink_wind_cov_t(ulong time_usec, float wind_x, float wind_y, float wind_z, float var_horiz, float var_vert, float wind_alt, float horiz_accuracy, float vert_accuracy)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[m/s]")]
		[Description("Wind in North (NED) direction (NAN if unknown)")]
		public float wind_x = wind_x;

		[Units("[m/s]")]
		[Description("Wind in East (NED) direction (NAN if unknown)")]
		public float wind_y = wind_y;

		[Units("[m/s]")]
		[Description("Wind in down (NED) direction (NAN if unknown)")]
		public float wind_z = wind_z;

		[Units("[m/s]")]
		[Description("Variability of wind in XY, 1-STD estimated from a 1 Hz lowpassed wind estimate (NAN if unknown)")]
		public float var_horiz = var_horiz;

		[Units("[m/s]")]
		[Description("Variability of wind in Z, 1-STD estimated from a 1 Hz lowpassed wind estimate (NAN if unknown)")]
		public float var_vert = var_vert;

		[Units("[m]")]
		[Description("Altitude (MSL) that this measurement was taken at (NAN if unknown)")]
		public float wind_alt = wind_alt;

		[Units("[m/s]")]
		[Description("Horizontal speed 1-STD accuracy (0 if unknown)")]
		public float horiz_accuracy = horiz_accuracy;

		[Units("[m/s]")]
		[Description("Vertical speed 1-STD accuracy (0 if unknown)")]
		public float vert_accuracy = vert_accuracy;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 65)]
	public struct mavlink_gps_input_t(ulong time_usec, uint time_week_ms, int lat, int lon, float alt, float hdop, float vdop, float vn, float ve, float vd, float speed_accuracy, float horiz_accuracy, float vert_accuracy, ushort ignore_flags, ushort time_week, byte gps_id, byte fix_type, byte satellites_visible, ushort yaw)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[ms]")]
		[Description("GPS time (from start of GPS week)")]
		public uint time_week_ms = time_week_ms;

		[Units("[degE7]")]
		[Description("Latitude (WGS84)")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude (WGS84)")]
		public int lon = lon;

		[Units("[m]")]
		[Description("Altitude (MSL). Positive for up.")]
		public float alt = alt;

		[Units("")]
		[Description("GPS HDOP horizontal dilution of position (unitless). If unknown, set to: UINT16_MAX")]
		public float hdop = hdop;

		[Units("")]
		[Description("GPS VDOP vertical dilution of position (unitless). If unknown, set to: UINT16_MAX")]
		public float vdop = vdop;

		[Units("[m/s]")]
		[Description("GPS velocity in north direction in earth-fixed NED frame")]
		public float vn = vn;

		[Units("[m/s]")]
		[Description("GPS velocity in east direction in earth-fixed NED frame")]
		public float ve = ve;

		[Units("[m/s]")]
		[Description("GPS velocity in down direction in earth-fixed NED frame")]
		public float vd = vd;

		[Units("[m/s]")]
		[Description("GPS speed accuracy")]
		public float speed_accuracy = speed_accuracy;

		[Units("[m]")]
		[Description("GPS horizontal accuracy")]
		public float horiz_accuracy = horiz_accuracy;

		[Units("[m]")]
		[Description("GPS vertical accuracy")]
		public float vert_accuracy = vert_accuracy;

		[Units("")]
		[Description("Bitmap indicating which GPS input flags fields to ignore.  All other fields must be provided.")]
		public ushort ignore_flags = ignore_flags;

		[Units("")]
		[Description("GPS week number")]
		public ushort time_week = time_week;

		[Units("")]
		[Description("ID of the GPS for multiple GPS inputs")]
		public byte gps_id = gps_id;

		[Units("")]
		[Description("0-1: no fix, 2: 2D fix, 3: 3D fix. 4: 3D with DGPS. 5: 3D with RTK")]
		public byte fix_type = fix_type;

		[Units("")]
		[Description("Number of satellites visible.")]
		public byte satellites_visible = satellites_visible;

		[Units("[cdeg]")]
		[Description("Yaw of vehicle relative to Earth's North, zero means not available, use 36000 for north")]
		public ushort yaw = yaw;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 182)]
	public struct mavlink_gps_rtcm_data_t(byte flags, byte len, byte[] data)
	{
		[Units("")]
		[Description("LSB: 1 means message is fragmented, next 2 bits are the fragment ID, the remaining 5 bits are used for the sequence ID. Messages are only to be flushed to the GPS when the entire message has been reconstructed on the autopilot. The fragment ID specifies which order the fragments should be assembled into a buffer, while the sequence ID is used to detect a mismatch between different buffers. The buffer is considered fully reconstructed when either all 4 fragments are present, or all the fragments before the first fragment with a non full payload is received. This management is used to ensure that normal GPS operation doesn't corrupt RTCM data, and to recover from a unreliable transport delivery order.")]
		public byte flags = flags;

		[Units("[bytes]")]
		[Description("data length")]
		public byte len = len;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 180)]
		[Units("")]
		[Description("RTCM message (may be fragmented)")]
		public byte[] data = data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 40)]
	public struct mavlink_high_latency_t(uint custom_mode, int latitude, int longitude, short roll, short pitch, ushort heading, short heading_sp, short altitude_amsl, short altitude_sp, ushort wp_distance, byte base_mode, byte landed_state, sbyte throttle, byte airspeed, byte airspeed_sp, byte groundspeed, sbyte climb_rate, byte gps_nsat, byte gps_fix_type, byte battery_remaining, sbyte temperature, sbyte temperature_air, byte failsafe, byte wp_num)
	{
		[Units("")]
		[Description("A bitfield for use for autopilot-specific flags.")]
		public uint custom_mode = custom_mode;

		[Units("[degE7]")]
		[Description("Latitude")]
		public int latitude = latitude;

		[Units("[degE7]")]
		[Description("Longitude")]
		public int longitude = longitude;

		[Units("[cdeg]")]
		[Description("roll")]
		public short roll = roll;

		[Units("[cdeg]")]
		[Description("pitch")]
		public short pitch = pitch;

		[Units("[cdeg]")]
		[Description("heading")]
		public ushort heading = heading;

		[Units("[cdeg]")]
		[Description("heading setpoint")]
		public short heading_sp = heading_sp;

		[Units("[m]")]
		[Description("Altitude above mean sea level")]
		public short altitude_amsl = altitude_amsl;

		[Units("[m]")]
		[Description("Altitude setpoint relative to the home position")]
		public short altitude_sp = altitude_sp;

		[Units("[m]")]
		[Description("distance to target")]
		public ushort wp_distance = wp_distance;

		[Units("")]
		[Description("Bitmap of enabled system modes.")]
		public byte base_mode = base_mode;

		[Units("")]
		[Description("The landed state. Is set to MAV_LANDED_STATE_UNDEFINED if landed state is unknown.")]
		public byte landed_state = landed_state;

		[Units("[%]")]
		[Description("throttle (percentage)")]
		public sbyte throttle = throttle;

		[Units("[m/s]")]
		[Description("airspeed")]
		public byte airspeed = airspeed;

		[Units("[m/s]")]
		[Description("airspeed setpoint")]
		public byte airspeed_sp = airspeed_sp;

		[Units("[m/s]")]
		[Description("groundspeed")]
		public byte groundspeed = groundspeed;

		[Units("[m/s]")]
		[Description("climb rate")]
		public sbyte climb_rate = climb_rate;

		[Units("")]
		[Description("Number of satellites visible. If unknown, set to UINT8_MAX")]
		public byte gps_nsat = gps_nsat;

		[Units("")]
		[Description("GPS Fix type.")]
		public byte gps_fix_type = gps_fix_type;

		[Units("[%]")]
		[Description("Remaining battery (percentage)")]
		public byte battery_remaining = battery_remaining;

		[Units("[degC]")]
		[Description("Autopilot temperature (degrees C)")]
		public sbyte temperature = temperature;

		[Units("[degC]")]
		[Description("Air temperature (degrees C) from airspeed sensor")]
		public sbyte temperature_air = temperature_air;

		[Units("")]
		[Description("failsafe (each bit represents a failsafe where 0=ok, 1=failsafe active (bit0:RC, bit1:batt, bit2:GPS, bit3:GCS, bit4:fence)")]
		public byte failsafe = failsafe;

		[Units("")]
		[Description("current waypoint number")]
		public byte wp_num = wp_num;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 42)]
	public struct mavlink_high_latency2_t(uint timestamp, int latitude, int longitude, ushort custom_mode, short altitude, short target_altitude, ushort target_distance, ushort wp_num, ushort failure_flags, byte type, byte autopilot, byte heading, byte target_heading, byte throttle, byte airspeed, byte airspeed_sp, byte groundspeed, byte windspeed, byte wind_heading, byte eph, byte epv, sbyte temperature_air, sbyte climb_rate, sbyte battery, sbyte custom0, sbyte custom1, sbyte custom2)
	{
		[Units("[ms]")]
		[Description("Timestamp (milliseconds since boot or Unix epoch)")]
		public uint timestamp = timestamp;

		[Units("[degE7]")]
		[Description("Latitude")]
		public int latitude = latitude;

		[Units("[degE7]")]
		[Description("Longitude")]
		public int longitude = longitude;

		[Units("")]
		[Description("A bitfield for use for autopilot-specific flags (2 byte version).")]
		public ushort custom_mode = custom_mode;

		[Units("[m]")]
		[Description("Altitude above mean sea level")]
		public short altitude = altitude;

		[Units("[m]")]
		[Description("Altitude setpoint")]
		public short target_altitude = target_altitude;

		[Units("[dam]")]
		[Description("Distance to target waypoint or position")]
		public ushort target_distance = target_distance;

		[Units("")]
		[Description("Current waypoint number")]
		public ushort wp_num = wp_num;

		[Units("")]
		[Description("Bitmap of failure flags.")]
		public ushort failure_flags = failure_flags;

		[Units("")]
		[Description("Type of the MAV (quadrotor, helicopter, etc.)")]
		public byte type = type;

		[Units("")]
		[Description("Autopilot type / class. Use MAV_AUTOPILOT_INVALID for components that are not flight controllers.")]
		public byte autopilot = autopilot;

		[Units("[deg/2]")]
		[Description("Heading")]
		public byte heading = heading;

		[Units("[deg/2]")]
		[Description("Heading setpoint")]
		public byte target_heading = target_heading;

		[Units("[%]")]
		[Description("Throttle")]
		public byte throttle = throttle;

		[Units("[m/s*5]")]
		[Description("Airspeed")]
		public byte airspeed = airspeed;

		[Units("[m/s*5]")]
		[Description("Airspeed setpoint")]
		public byte airspeed_sp = airspeed_sp;

		[Units("[m/s*5]")]
		[Description("Groundspeed")]
		public byte groundspeed = groundspeed;

		[Units("[m/s*5]")]
		[Description("Windspeed")]
		public byte windspeed = windspeed;

		[Units("[deg/2]")]
		[Description("Wind heading")]
		public byte wind_heading = wind_heading;

		[Units("[dm]")]
		[Description("Maximum error horizontal position since last message")]
		public byte eph = eph;

		[Units("[dm]")]
		[Description("Maximum error vertical position since last message")]
		public byte epv = epv;

		[Units("[degC]")]
		[Description("Air temperature")]
		public sbyte temperature_air = temperature_air;

		[Units("[dm/s]")]
		[Description("Maximum climb rate magnitude since last message")]
		public sbyte climb_rate = climb_rate;

		[Units("[%]")]
		[Description("Battery level (-1 if field not provided).")]
		public sbyte battery = battery;

		[Units("")]
		[Description("Field for custom payload.")]
		public sbyte custom0 = custom0;

		[Units("")]
		[Description("Field for custom payload.")]
		public sbyte custom1 = custom1;

		[Units("")]
		[Description("Field for custom payload.")]
		public sbyte custom2 = custom2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
	public struct mavlink_vibration_t(ulong time_usec, float vibration_x, float vibration_y, float vibration_z, uint clipping_0, uint clipping_1, uint clipping_2)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("Vibration levels on X-axis")]
		public float vibration_x = vibration_x;

		[Units("")]
		[Description("Vibration levels on Y-axis")]
		public float vibration_y = vibration_y;

		[Units("")]
		[Description("Vibration levels on Z-axis")]
		public float vibration_z = vibration_z;

		[Units("")]
		[Description("first accelerometer clipping count")]
		public uint clipping_0 = clipping_0;

		[Units("")]
		[Description("second accelerometer clipping count")]
		public uint clipping_1 = clipping_1;

		[Units("")]
		[Description("third accelerometer clipping count")]
		public uint clipping_2 = clipping_2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 60)]
	public struct mavlink_home_position_t(int latitude, int longitude, int altitude, float x, float y, float z, float[] q, float approach_x, float approach_y, float approach_z, ulong time_usec)
	{
		[Units("[degE7]")]
		[Description("Latitude (WGS84)")]
		public int latitude = latitude;

		[Units("[degE7]")]
		[Description("Longitude (WGS84)")]
		public int longitude = longitude;

		[Units("[mm]")]
		[Description("Altitude (MSL). Positive for up.")]
		public int altitude = altitude;

		[Units("[m]")]
		[Description("Local X position of this position in the local coordinate frame (NED)")]
		public float x = x;

		[Units("[m]")]
		[Description("Local Y position of this position in the local coordinate frame (NED)")]
		public float y = y;

		[Units("[m]")]
		[Description("Local Z position of this position in the local coordinate frame (NED: positive 'down')")]
		public float z = z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("         Quaternion indicating world-to-surface-normal and heading transformation of the takeoff position.         Used to indicate the heading and slope of the ground.         All fields should be set to NaN if an accurate quaternion for both heading and surface slope cannot be supplied.       ")]
		public float[] q = q;

		[Units("[m]")]
		[Description("Local X position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
		public float approach_x = approach_x;

		[Units("[m]")]
		[Description("Local Y position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
		public float approach_y = approach_y;

		[Units("[m]")]
		[Description("Local Z position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
		public float approach_z = approach_z;

		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 61)]
	public struct mavlink_set_home_position_t(int latitude, int longitude, int altitude, float x, float y, float z, float[] q, float approach_x, float approach_y, float approach_z, byte target_system, ulong time_usec)
	{
		[Units("[degE7]")]
		[Description("Latitude (WGS84)")]
		public int latitude = latitude;

		[Units("[degE7]")]
		[Description("Longitude (WGS84)")]
		public int longitude = longitude;

		[Units("[mm]")]
		[Description("Altitude (MSL). Positive for up.")]
		public int altitude = altitude;

		[Units("[m]")]
		[Description("Local X position of this position in the local coordinate frame (NED)")]
		public float x = x;

		[Units("[m]")]
		[Description("Local Y position of this position in the local coordinate frame (NED)")]
		public float y = y;

		[Units("[m]")]
		[Description("Local Z position of this position in the local coordinate frame (NED: positive 'down')")]
		public float z = z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("World to surface normal and heading transformation of the takeoff position. Used to indicate the heading and slope of the ground")]
		public float[] q = q;

		[Units("[m]")]
		[Description("Local X position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
		public float approach_x = approach_x;

		[Units("[m]")]
		[Description("Local Y position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
		public float approach_y = approach_y;

		[Units("[m]")]
		[Description("Local Z position of the end of the approach vector. Multicopters should set this position based on their takeoff path. Grass-landing fixed wing aircraft should set it the same way as multicopters. Runway-landing fixed wing aircraft should set it to the opposite direction of the takeoff, assuming the takeoff happened from the threshold / touchdown zone.")]
		public float approach_z = approach_z;

		[Units("")]
		[Description("System ID.")]
		public byte target_system = target_system;

		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
	public struct mavlink_message_interval_t(int interval_us, ushort message_id)
	{
		[Units("[us]")]
		[Description("The interval between two messages. A value of -1 indicates this stream is disabled, 0 indicates it is not available, > 0 indicates the interval at which it is sent.")]
		public int interval_us = interval_us;

		[Units("")]
		[Description("The ID of the requested MAVLink message. v1.0 is limited to 254 messages.")]
		public ushort message_id = message_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
	public struct mavlink_extended_sys_state_t(byte vtol_state, byte landed_state)
	{
		[Units("")]
		[Description("The VTOL state if applicable. Is set to MAV_VTOL_STATE_UNDEFINED if UAV is not in VTOL configuration.")]
		public byte vtol_state = vtol_state;

		[Units("")]
		[Description("The landed state. Is set to MAV_LANDED_STATE_UNDEFINED if landed state is unknown.")]
		public byte landed_state = landed_state;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 38)]
	public struct mavlink_adsb_vehicle_t(uint ICAO_address, int lat, int lon, int altitude, ushort heading, ushort hor_velocity, short ver_velocity, ushort flags, ushort squawk, byte altitude_type, byte[] callsign, byte emitter_type, byte tslc)
	{
		[Units("")]
		[Description("ICAO address")]
		public uint ICAO_address = ICAO_address;

		[Units("[degE7]")]
		[Description("Latitude")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude(ASL)")]
		public int altitude = altitude;

		[Units("[cdeg]")]
		[Description("Course over ground")]
		public ushort heading = heading;

		[Units("[cm/s]")]
		[Description("The horizontal velocity")]
		public ushort hor_velocity = hor_velocity;

		[Units("[cm/s]")]
		[Description("The vertical velocity. Positive is up")]
		public short ver_velocity = ver_velocity;

		[Units("")]
		[Description("Bitmap to indicate various statuses including valid data fields")]
		public ushort flags = flags;

		[Units("")]
		[Description("Squawk code. Note that the code is in decimal: e.g. 7700 (general emergency) is encoded as binary 0b0001_1110_0001_0100, not(!) as 0b0000_111_111_000_000")]
		public ushort squawk = squawk;

		[Units("")]
		[Description("ADSB altitude type.")]
		public byte altitude_type = altitude_type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
		[Units("")]
		[Description("The callsign, 8+null")]
		public byte[] callsign = callsign;

		[Units("")]
		[Description("ADSB emitter type.")]
		public byte emitter_type = emitter_type;

		[Units("[s]")]
		[Description("Time since last communication in seconds")]
		public byte tslc = tslc;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 19)]
	public struct mavlink_collision_t(uint id, float time_to_minimum_delta, float altitude_minimum_delta, float horizontal_minimum_delta, byte src, byte action, byte threat_level)
	{
		[Units("")]
		[Description("Unique identifier, domain based on src field")]
		public uint id = id;

		[Units("[s]")]
		[Description("Estimated time until collision occurs")]
		public float time_to_minimum_delta = time_to_minimum_delta;

		[Units("[m]")]
		[Description("Closest vertical distance between vehicle and object")]
		public float altitude_minimum_delta = altitude_minimum_delta;

		[Units("[m]")]
		[Description("Closest horizontal distance between vehicle and object")]
		public float horizontal_minimum_delta = horizontal_minimum_delta;

		[Units("")]
		[Description("Collision data source")]
		public byte src = src;

		[Units("")]
		[Description("Action that is being taken to avoid this collision")]
		public byte action = action;

		[Units("")]
		[Description("How concerned the aircraft is about this collision")]
		public byte threat_level = threat_level;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 254)]
	public struct mavlink_v2_extension_t(ushort message_type, byte target_network, byte target_system, byte target_component, byte[] payload)
	{
		[Units("")]
		[Description("A code that identifies the software component that understands this message (analogous to USB device classes or mime type strings). If this code is less than 32768, it is considered a 'registered' protocol extension and the corresponding entry should be added to https://github.com/mavlink/mavlink/definition_files/extension_message_ids.xml. Software creators can register blocks of message IDs as needed (useful for GCS specific metadata, etc...). Message_types greater than 32767 are considered local experiments and should not be checked in to any widely distributed codebase.")]
		public ushort message_type = message_type;

		[Units("")]
		[Description("Network ID (0 for broadcast)")]
		public byte target_network = target_network;

		[Units("")]
		[Description("System ID (0 for broadcast)")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast)")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 249)]
		[Units("")]
		[Description("Variable length payload. The length must be encoded in the payload as part of the message_type protocol, e.g. by including the length as payload data, or by terminating the payload data with a non-zero marker. This is required in order to reconstruct zero-terminated payloads that are (or otherwise would be) trimmed by MAVLink 2 empty-byte truncation. The entire content of the payload block is opaque unless you understand the encoding message_type. The particular encoding used can be extension specific and might not always be documented as part of the MAVLink specification.")]
		public byte[] payload = payload;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 36)]
	public struct mavlink_memory_vect_t(ushort address, byte ver, byte type, sbyte[] value)
	{
		[Units("")]
		[Description("Starting address of the debug variables")]
		public ushort address = address;

		[Units("")]
		[Description("Version code of the type variable. 0=unknown, type ignored and assumed int16_t. 1=as below")]
		public byte ver = ver;

		[Units("")]
		[Description("Type code of the memory variables. for ver = 1: 0=16 x int16_t, 1=16 x uint16_t, 2=16 x Q15, 3=16 x 1Q14")]
		public byte type = type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Memory contents at specified address")]
		public sbyte[] value = value;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 30)]
	public struct mavlink_debug_vect_t(ulong time_usec, float x, float y, float z, byte[] name)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("x")]
		public float x = x;

		[Units("")]
		[Description("y")]
		public float y = y;

		[Units("")]
		[Description("z")]
		public float z = z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		[Units("")]
		[Description("Name")]
		public byte[] name = name;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 18)]
	public struct mavlink_named_value_float_t(uint time_boot_ms, float value, byte[] name)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("Floating point value")]
		public float value = value;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		[Units("")]
		[Description("Name of the debug variable")]
		public byte[] name = name;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 18)]
	public struct mavlink_named_value_int_t(uint time_boot_ms, int value, byte[] name)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("Signed integer value")]
		public int value = value;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		[Units("")]
		[Description("Name of the debug variable")]
		public byte[] name = name;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 54)]
	public struct mavlink_statustext_t(byte severity, byte[] text, ushort id, byte chunk_seq)
	{
		[Units("")]
		[Description("Severity of status. Relies on the definitions within RFC-5424.")]
		public byte severity = severity;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
		[Units("")]
		[Description("Status text message, without null termination character")]
		public byte[] text = text;

		[Units("")]
		[Description("Unique (opaque) identifier for this statustext message.  May be used to reassemble a logical long-statustext message from a sequence of chunks.  A value of zero indicates this is the only chunk in the sequence and the message can be emitted immediately.")]
		public ushort id = id;

		[Units("")]
		[Description("This chunk's sequence number; indexing is from zero.  Any null character in the text field is taken to mean this was the last chunk.")]
		public byte chunk_seq = chunk_seq;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 9)]
	public struct mavlink_debug_t(uint time_boot_ms, float value, byte ind)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("DEBUG value")]
		public float value = value;

		[Units("")]
		[Description("index of debug variable")]
		public byte ind = ind;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 42)]
	public struct mavlink_setup_signing_t(ulong initial_timestamp, byte target_system, byte target_component, byte[] secret_key)
	{
		[Units("")]
		[Description("initial timestamp")]
		public ulong initial_timestamp = initial_timestamp;

		[Units("")]
		[Description("system id of the target")]
		public byte target_system = target_system;

		[Units("")]
		[Description("component ID of the target")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("signing key")]
		public byte[] secret_key = secret_key;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 9)]
	public struct mavlink_button_change_t(uint time_boot_ms, uint last_change_ms, byte state)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[ms]")]
		[Description("Time of last change of button state.")]
		public uint last_change_ms = last_change_ms;

		[Units("")]
		[Description("Bitmap for state of buttons.")]
		public byte state = state;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 232)]
	public struct mavlink_play_tune_t(byte target_system, byte target_component, byte[] tune, byte[] tune2)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
		[Units("")]
		[Description("tune in board specific format")]
		public byte[] tune = tune;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
		[Units("")]
		[Description("tune extension (appended to tune)")]
		public byte[] tune2 = tune2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 237)]
	public struct mavlink_camera_information_t(uint time_boot_ms, uint firmware_version, float focal_length, float sensor_size_h, float sensor_size_v, uint flags, ushort resolution_h, ushort resolution_v, ushort cam_definition_version, byte[] vendor_name, byte[] model_name, byte lens_id, byte[] cam_definition_uri, byte gimbal_device_id, byte camera_device_id)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("Version of the camera firmware, encoded as: `(Dev & 0xff) << 24 | (Patch & 0xff) << 16 | (Minor & 0xff) << 8 | (Major & 0xff)`. Use 0 if not known.")]
		public uint firmware_version = firmware_version;

		[Units("[mm]")]
		[Description("Focal length. Use NaN if not known.")]
		public float focal_length = focal_length;

		[Units("[mm]")]
		[Description("Image sensor size horizontal. Use NaN if not known.")]
		public float sensor_size_h = sensor_size_h;

		[Units("[mm]")]
		[Description("Image sensor size vertical. Use NaN if not known.")]
		public float sensor_size_v = sensor_size_v;

		[Units("")]
		[Description("Bitmap of camera capability flags.")]
		public uint flags = flags;

		[Units("[pix]")]
		[Description("Horizontal image resolution. Use 0 if not known.")]
		public ushort resolution_h = resolution_h;

		[Units("[pix]")]
		[Description("Vertical image resolution. Use 0 if not known.")]
		public ushort resolution_v = resolution_v;

		[Units("")]
		[Description("Camera definition version (iteration).  Use 0 if not known.")]
		public ushort cam_definition_version = cam_definition_version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Name of the camera vendor")]
		public byte[] vendor_name = vendor_name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Name of the camera model")]
		public byte[] model_name = model_name;

		[Units("")]
		[Description("Reserved for a lens ID.  Use 0 if not known.")]
		public byte lens_id = lens_id;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 140)]
		[Units("")]
		[Description("Camera definition URI (if any, otherwise only basic functions will be available). HTTP- (http://) and MAVLink FTP- (mavlinkftp://) formatted URIs are allowed (and both must be supported by any GCS that implements the Camera Protocol). The definition file may be xz compressed, which will be indicated by the file extension .xml.xz (a GCS that implements the protocol must support decompressing the file). The string needs to be zero terminated.  Use a zero-length string if not known.")]
		public byte[] cam_definition_uri = cam_definition_uri;

		[Units("")]
		[Description("Gimbal id of a gimbal associated with this camera. This is the component id of the gimbal device, or 1-6 for non mavlink gimbals. Use 0 if no gimbal is associated with the camera.")]
		public byte gimbal_device_id = gimbal_device_id;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id).")]
		public byte camera_device_id = camera_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 14)]
	public struct mavlink_camera_settings_t(uint time_boot_ms, byte mode_id, float zoomLevel, float focusLevel, byte camera_device_id)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("Camera mode")]
		public byte mode_id = mode_id;

		[Units("")]
		[Description("Current zoom level as a percentage of the full range (0.0 to 100.0, NaN if not known)")]
		public float zoomLevel = zoomLevel;

		[Units("")]
		[Description("Current focus level as a percentage of the full range (0.0 to 100.0, NaN if not known)")]
		public float focusLevel = focusLevel;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id).")]
		public byte camera_device_id = camera_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 61)]
	public struct mavlink_storage_information_t(uint time_boot_ms, float total_capacity, float used_capacity, float available_capacity, float read_speed, float write_speed, byte storage_id, byte storage_count, byte status, byte type, byte[] name, byte storage_usage)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[MiB]")]
		[Description("Total capacity. If storage is not ready (STORAGE_STATUS_READY) value will be ignored.")]
		public float total_capacity = total_capacity;

		[Units("[MiB]")]
		[Description("Used capacity. If storage is not ready (STORAGE_STATUS_READY) value will be ignored.")]
		public float used_capacity = used_capacity;

		[Units("[MiB]")]
		[Description("Available storage capacity. If storage is not ready (STORAGE_STATUS_READY) value will be ignored.")]
		public float available_capacity = available_capacity;

		[Units("[MiB/s]")]
		[Description("Read speed.")]
		public float read_speed = read_speed;

		[Units("[MiB/s]")]
		[Description("Write speed.")]
		public float write_speed = write_speed;

		[Units("")]
		[Description("Storage ID (1 for first, 2 for second, etc.)")]
		public byte storage_id = storage_id;

		[Units("")]
		[Description("Number of storage devices")]
		public byte storage_count = storage_count;

		[Units("")]
		[Description("Status of storage")]
		public byte status = status;

		[Units("")]
		[Description("Type of storage")]
		public byte type = type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Textual storage name to be used in UI (microSD 1, Internal Memory, etc.) This is a NULL terminated string. If it is exactly 32 characters long, add a terminating NULL. If this string is empty, the generic type is shown to the user.")]
		public byte[] name = name;

		[Units("")]
		[Description("Flags indicating whether this instance is preferred storage for photos, videos, etc.         Note: Implementations should initially set the flags on the system-default storage id used for saving media (if possible/supported).         This setting can then be overridden using MAV_CMD_SET_STORAGE_USAGE.         If the media usage flags are not set, a GCS may assume storage ID 1 is the default storage for all media types.")]
		public byte storage_usage = storage_usage;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 23)]
	public struct mavlink_camera_capture_status_t(uint time_boot_ms, float image_interval, uint recording_time_ms, float available_capacity, byte image_status, byte video_status, int image_count, byte camera_device_id)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[s]")]
		[Description("Image capture interval")]
		public float image_interval = image_interval;

		[Units("[ms]")]
		[Description("Elapsed time since recording started (0: Not supported/available). A GCS should compute recording time and use non-zero values of this field to correct any discrepancy.")]
		public uint recording_time_ms = recording_time_ms;

		[Units("[MiB]")]
		[Description("Available storage capacity.")]
		public float available_capacity = available_capacity;

		[Units("")]
		[Description("Current status of image capturing (0: idle, 1: capture in progress, 2: interval set but idle, 3: interval set and capture in progress)")]
		public byte image_status = image_status;

		[Units("")]
		[Description("Current status of video capturing (0: idle, 1: capture in progress)")]
		public byte video_status = video_status;

		[Units("")]
		[Description("Total number of images captured ('forever', or until reset using MAV_CMD_STORAGE_FORMAT).")]
		public int image_count = image_count;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id).")]
		public byte camera_device_id = camera_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 255)]
	public struct mavlink_camera_image_captured_t(ulong time_utc, uint time_boot_ms, int lat, int lon, int alt, int relative_alt, float[] q, int image_index, byte camera_id, sbyte capture_result, byte[] file_url)
	{
		[Units("[us]")]
		[Description("Timestamp (time since UNIX epoch) in UTC. 0 for unknown.")]
		public ulong time_utc = time_utc;

		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[degE7]")]
		[Description("Latitude where image was taken")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude where capture was taken")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude (MSL) where image was taken")]
		public int alt = alt;

		[Units("[mm]")]
		[Description("Altitude above ground")]
		public int relative_alt = relative_alt;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion of camera orientation (w, x, y, z order, zero-rotation is 1, 0, 0, 0)")]
		public float[] q = q;

		[Units("")]
		[Description("Zero based index of this image (i.e. a new image will have index CAMERA_CAPTURE_STATUS.image count -1)")]
		public int image_index = image_index;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id). Field name is usually camera_device_id.")]
		public byte camera_id = camera_id;

		[Units("")]
		[Description("Image was captured successfully (MAV_BOOL_TRUE). Values not equal to 0 or 1 are invalid.")]
		public sbyte capture_result = capture_result;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 205)]
		[Units("")]
		[Description("URL of image taken. Either local storage or http://foo.jpg if camera provides an HTTP interface.")]
		public byte[] file_url = file_url;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
	public struct mavlink_flight_information_t(ulong arming_time_utc, ulong takeoff_time_utc, ulong flight_uuid, uint time_boot_ms, uint landing_time)
	{
		[Units("[us]")]
		[Description("Timestamp at arming (since system boot). Set to 0 on boot. Set value on arming. Note, field is misnamed UTC.")]
		public ulong arming_time_utc = arming_time_utc;

		[Units("[us]")]
		[Description("Timestamp at takeoff (since system boot). Set to 0 at boot and on arming. Note, field is misnamed UTC.")]
		public ulong takeoff_time_utc = takeoff_time_utc;

		[Units("")]
		[Description("Flight number. Note, field is misnamed UUID.")]
		public ulong flight_uuid = flight_uuid;

		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[ms]")]
		[Description("Timestamp at landing (in ms since system boot). Set to 0 at boot and on arming.")]
		public uint landing_time = landing_time;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
	public struct mavlink_mount_orientation_t(uint time_boot_ms, float roll, float pitch, float yaw, float yaw_absolute)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[deg]")]
		[Description("Roll in global frame (set to NaN for invalid).")]
		public float roll = roll;

		[Units("[deg]")]
		[Description("Pitch in global frame (set to NaN for invalid).")]
		public float pitch = pitch;

		[Units("[deg]")]
		[Description("Yaw relative to vehicle (set to NaN for invalid).")]
		public float yaw = yaw;

		[Units("[deg]")]
		[Description("Yaw in absolute frame relative to Earth's North, north is 0 (set to NaN for invalid).")]
		public float yaw_absolute = yaw_absolute;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 255)]
	public struct mavlink_logging_data_t(ushort sequence, byte target_system, byte target_component, byte length, byte first_message_offset, byte[] data)
	{
		[Units("")]
		[Description("sequence number (can wrap)")]
		public ushort sequence = sequence;

		[Units("")]
		[Description("system ID of the target")]
		public byte target_system = target_system;

		[Units("")]
		[Description("component ID of the target")]
		public byte target_component = target_component;

		[Units("[bytes]")]
		[Description("data length")]
		public byte length = length;

		[Units("[bytes]")]
		[Description("offset into data where first message starts. This can be used for recovery, when a previous message got lost (set to UINT8_MAX if no start exists).")]
		public byte first_message_offset = first_message_offset;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 249)]
		[Units("")]
		[Description("logged data")]
		public byte[] data = data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 255)]
	public struct mavlink_logging_data_acked_t(ushort sequence, byte target_system, byte target_component, byte length, byte first_message_offset, byte[] data)
	{
		[Units("")]
		[Description("sequence number (can wrap)")]
		public ushort sequence = sequence;

		[Units("")]
		[Description("system ID of the target")]
		public byte target_system = target_system;

		[Units("")]
		[Description("component ID of the target")]
		public byte target_component = target_component;

		[Units("[bytes]")]
		[Description("data length")]
		public byte length = length;

		[Units("[bytes]")]
		[Description("offset into data where first message starts. This can be used for recovery, when a previous message got lost (set to UINT8_MAX if no start exists).")]
		public byte first_message_offset = first_message_offset;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 249)]
		[Units("")]
		[Description("logged data")]
		public byte[] data = data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
	public struct mavlink_logging_ack_t(ushort sequence, byte target_system, byte target_component)
	{
		[Units("")]
		[Description("sequence number (must match the one in LOGGING_DATA_ACKED)")]
		public ushort sequence = sequence;

		[Units("")]
		[Description("system ID of the target")]
		public byte target_system = target_system;

		[Units("")]
		[Description("component ID of the target")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 215)]
	public struct mavlink_video_stream_information_t(float framerate, uint bitrate, ushort flags, ushort resolution_h, ushort resolution_v, ushort rotation, ushort hfov, byte stream_id, byte count, byte type, byte[] name, byte[] uri, byte encoding, byte camera_device_id)
	{
		[Units("[Hz]")]
		[Description("Frame rate.")]
		public float framerate = framerate;

		[Units("[bits/s]")]
		[Description("Bit rate.")]
		public uint bitrate = bitrate;

		[Units("")]
		[Description("Bitmap of stream status flags.")]
		public ushort flags = flags;

		[Units("[pix]")]
		[Description("Horizontal resolution.")]
		public ushort resolution_h = resolution_h;

		[Units("[pix]")]
		[Description("Vertical resolution.")]
		public ushort resolution_v = resolution_v;

		[Units("[deg]")]
		[Description("Video image rotation clockwise.")]
		public ushort rotation = rotation;

		[Units("[deg]")]
		[Description("Horizontal Field of view.")]
		public ushort hfov = hfov;

		[Units("")]
		[Description("Video Stream ID (1 for first, 2 for second, etc.)")]
		public byte stream_id = stream_id;

		[Units("")]
		[Description("Number of streams available.")]
		public byte count = count;

		[Units("")]
		[Description("Type of stream.")]
		public byte type = type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Stream name.")]
		public byte[] name = name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 160)]
		[Units("")]
		[Description("Video stream URI (TCP or RTSP URI ground station should connect to) or port number (UDP port ground station should listen to).")]
		public byte[] uri = uri;

		[Units("")]
		[Description("Encoding of stream.")]
		public byte encoding = encoding;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id).")]
		public byte camera_device_id = camera_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
	public struct mavlink_video_stream_status_t(float framerate, uint bitrate, ushort flags, ushort resolution_h, ushort resolution_v, ushort rotation, ushort hfov, byte stream_id, byte camera_device_id)
	{
		[Units("[Hz]")]
		[Description("Frame rate")]
		public float framerate = framerate;

		[Units("[bits/s]")]
		[Description("Bit rate")]
		public uint bitrate = bitrate;

		[Units("")]
		[Description("Bitmap of stream status flags")]
		public ushort flags = flags;

		[Units("[pix]")]
		[Description("Horizontal resolution")]
		public ushort resolution_h = resolution_h;

		[Units("[pix]")]
		[Description("Vertical resolution")]
		public ushort resolution_v = resolution_v;

		[Units("[deg]")]
		[Description("Video image rotation clockwise")]
		public ushort rotation = rotation;

		[Units("[deg]")]
		[Description("Horizontal Field of view")]
		public ushort hfov = hfov;

		[Units("")]
		[Description("Video Stream ID (1 for first, 2 for second, etc.)")]
		public byte stream_id = stream_id;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id).")]
		public byte camera_device_id = camera_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 53)]
	public struct mavlink_camera_fov_status_t(uint time_boot_ms, int lat_camera, int lon_camera, int alt_camera, int lat_image, int lon_image, int alt_image, float[] q, float hfov, float vfov, byte camera_device_id)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[degE7]")]
		[Description("Latitude of camera (INT32_MAX if unknown).")]
		public int lat_camera = lat_camera;

		[Units("[degE7]")]
		[Description("Longitude of camera (INT32_MAX if unknown).")]
		public int lon_camera = lon_camera;

		[Units("[mm]")]
		[Description("Altitude (MSL) of camera (INT32_MAX if unknown).")]
		public int alt_camera = alt_camera;

		[Units("[degE7]")]
		[Description("Latitude of center of image (INT32_MAX if unknown, INT32_MIN if at infinity, not intersecting with horizon).")]
		public int lat_image = lat_image;

		[Units("[degE7]")]
		[Description("Longitude of center of image (INT32_MAX if unknown, INT32_MIN if at infinity, not intersecting with horizon).")]
		public int lon_image = lon_image;

		[Units("[mm]")]
		[Description("Altitude (MSL) of center of image (INT32_MAX if unknown, INT32_MIN if at infinity, not intersecting with horizon).")]
		public int alt_image = alt_image;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion of camera orientation (w, x, y, z order, zero-rotation is 1, 0, 0, 0)")]
		public float[] q = q;

		[Units("[deg]")]
		[Description("Horizontal field of view (NaN if unknown).")]
		public float hfov = hfov;

		[Units("[deg]")]
		[Description("Vertical field of view (NaN if unknown).")]
		public float vfov = vfov;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id).")]
		public byte camera_device_id = camera_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
	public struct mavlink_camera_tracking_image_status_t(float point_x, float point_y, float radius, float rec_top_x, float rec_top_y, float rec_bottom_x, float rec_bottom_y, byte tracking_status, byte tracking_mode, byte target_data, byte camera_device_id)
	{
		[Units("")]
		[Description("Current tracked point x value if CAMERA_TRACKING_MODE_POINT (normalized 0..1, 0 is left, 1 is right), NAN if unknown")]
		public float point_x = point_x;

		[Units("")]
		[Description("Current tracked point y value if CAMERA_TRACKING_MODE_POINT (normalized 0..1, 0 is top, 1 is bottom), NAN if unknown")]
		public float point_y = point_y;

		[Units("")]
		[Description("Current tracked radius if CAMERA_TRACKING_MODE_POINT (normalized 0..1, 0 is image left, 1 is image right), NAN if unknown")]
		public float radius = radius;

		[Units("")]
		[Description("Current tracked rectangle top x value if CAMERA_TRACKING_MODE_RECTANGLE (normalized 0..1, 0 is left, 1 is right), NAN if unknown")]
		public float rec_top_x = rec_top_x;

		[Units("")]
		[Description("Current tracked rectangle top y value if CAMERA_TRACKING_MODE_RECTANGLE (normalized 0..1, 0 is top, 1 is bottom), NAN if unknown")]
		public float rec_top_y = rec_top_y;

		[Units("")]
		[Description("Current tracked rectangle bottom x value if CAMERA_TRACKING_MODE_RECTANGLE (normalized 0..1, 0 is left, 1 is right), NAN if unknown")]
		public float rec_bottom_x = rec_bottom_x;

		[Units("")]
		[Description("Current tracked rectangle bottom y value if CAMERA_TRACKING_MODE_RECTANGLE (normalized 0..1, 0 is top, 1 is bottom), NAN if unknown")]
		public float rec_bottom_y = rec_bottom_y;

		[Units("")]
		[Description("Current tracking status")]
		public byte tracking_status = tracking_status;

		[Units("")]
		[Description("Current tracking mode")]
		public byte tracking_mode = tracking_mode;

		[Units("")]
		[Description("Defines location of target data")]
		public byte target_data = target_data;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id).")]
		public byte camera_device_id = camera_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 50)]
	public struct mavlink_camera_tracking_geo_status_t(int lat, int lon, float alt, float h_acc, float v_acc, float vel_n, float vel_e, float vel_d, float vel_acc, float dist, float hdg, float hdg_acc, byte tracking_status, byte camera_device_id)
	{
		[Units("[degE7]")]
		[Description("Latitude of tracked object")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude of tracked object")]
		public int lon = lon;

		[Units("[m]")]
		[Description("Altitude of tracked object(AMSL, WGS84)")]
		public float alt = alt;

		[Units("[m]")]
		[Description("Horizontal accuracy. NAN if unknown")]
		public float h_acc = h_acc;

		[Units("[m]")]
		[Description("Vertical accuracy. NAN if unknown")]
		public float v_acc = v_acc;

		[Units("[m/s]")]
		[Description("North velocity of tracked object. NAN if unknown")]
		public float vel_n = vel_n;

		[Units("[m/s]")]
		[Description("East velocity of tracked object. NAN if unknown")]
		public float vel_e = vel_e;

		[Units("[m/s]")]
		[Description("Down velocity of tracked object. NAN if unknown")]
		public float vel_d = vel_d;

		[Units("[m/s]")]
		[Description("Velocity accuracy. NAN if unknown")]
		public float vel_acc = vel_acc;

		[Units("[m]")]
		[Description("Distance between camera and tracked object. NAN if unknown")]
		public float dist = dist;

		[Units("[rad]")]
		[Description("Heading in radians, in NED. NAN if unknown")]
		public float hdg = hdg;

		[Units("[rad]")]
		[Description("Accuracy of heading, in NED. NAN if unknown")]
		public float hdg_acc = hdg_acc;

		[Units("")]
		[Description("Current tracking status")]
		public byte tracking_status = tracking_status;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id).")]
		public byte camera_device_id = camera_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 30)]
	public struct mavlink_camera_thermal_range_t(uint time_boot_ms, float max, float max_point_x, float max_point_y, float min, float min_point_x, float min_point_y, byte stream_id, byte camera_device_id)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[degC]")]
		[Description("Temperature max.")]
		public float max = max;

		[Units("")]
		[Description("Temperature max point x value (normalized 0..1, 0 is left, 1 is right), NAN if unknown.")]
		public float max_point_x = max_point_x;

		[Units("")]
		[Description("Temperature max point y value (normalized 0..1, 0 is top, 1 is bottom), NAN if unknown.")]
		public float max_point_y = max_point_y;

		[Units("[degC]")]
		[Description("Temperature min.")]
		public float min = min;

		[Units("")]
		[Description("Temperature min point x value (normalized 0..1, 0 is left, 1 is right), NAN if unknown.")]
		public float min_point_x = min_point_x;

		[Units("")]
		[Description("Temperature min point y value (normalized 0..1, 0 is top, 1 is bottom), NAN if unknown.")]
		public float min_point_y = min_point_y;

		[Units("")]
		[Description("Video Stream ID (1 for first, 2 for second, etc.)")]
		public byte stream_id = stream_id;

		[Units("")]
		[Description("Camera id of a non-MAVLink camera attached to an autopilot (1-6).  0 if the component is a MAVLink camera (with its own component id).")]
		public byte camera_device_id = camera_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 33)]
	public struct mavlink_gimbal_manager_information_t(uint time_boot_ms, uint cap_flags, float roll_min, float roll_max, float pitch_min, float pitch_max, float yaw_min, float yaw_max, byte gimbal_device_id)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("Bitmap of gimbal capability flags.")]
		public uint cap_flags = cap_flags;

		[Units("[rad]")]
		[Description("Minimum hardware roll angle (positive: rolling to the right, negative: rolling to the left)")]
		public float roll_min = roll_min;

		[Units("[rad]")]
		[Description("Maximum hardware roll angle (positive: rolling to the right, negative: rolling to the left)")]
		public float roll_max = roll_max;

		[Units("[rad]")]
		[Description("Minimum pitch angle (positive: up, negative: down)")]
		public float pitch_min = pitch_min;

		[Units("[rad]")]
		[Description("Maximum pitch angle (positive: up, negative: down)")]
		public float pitch_max = pitch_max;

		[Units("[rad]")]
		[Description("Minimum yaw angle (positive: to the right, negative: to the left)")]
		public float yaw_min = yaw_min;

		[Units("[rad]")]
		[Description("Maximum yaw angle (positive: to the right, negative: to the left)")]
		public float yaw_max = yaw_max;

		[Units("")]
		[Description("Gimbal device ID that this gimbal manager is responsible for. Component ID of gimbal device (or 1-6 for non-MAVLink gimbal).")]
		public byte gimbal_device_id = gimbal_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 13)]
	public struct mavlink_gimbal_manager_status_t(uint time_boot_ms, uint flags, byte gimbal_device_id, byte primary_control_sysid, byte primary_control_compid, byte secondary_control_sysid, byte secondary_control_compid)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("High level gimbal manager flags currently applied.")]
		public uint flags = flags;

		[Units("")]
		[Description("Gimbal device ID that this gimbal manager is responsible for. Component ID of gimbal device (or 1-6 for non-MAVLink gimbal).")]
		public byte gimbal_device_id = gimbal_device_id;

		[Units("")]
		[Description("System ID of MAVLink component with primary control, 0 for none.")]
		public byte primary_control_sysid = primary_control_sysid;

		[Units("")]
		[Description("Component ID of MAVLink component with primary control, 0 for none.")]
		public byte primary_control_compid = primary_control_compid;

		[Units("")]
		[Description("System ID of MAVLink component with secondary control, 0 for none.")]
		public byte secondary_control_sysid = secondary_control_sysid;

		[Units("")]
		[Description("Component ID of MAVLink component with secondary control, 0 for none.")]
		public byte secondary_control_compid = secondary_control_compid;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 35)]
	public struct mavlink_gimbal_manager_set_attitude_t(uint flags, float[] q, float angular_velocity_x, float angular_velocity_y, float angular_velocity_z, byte target_system, byte target_component, byte gimbal_device_id)
	{
		[Units("")]
		[Description("High level gimbal manager flags to use.")]
		public uint flags = flags;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation, the frame is depends on whether the flag GIMBAL_MANAGER_FLAGS_YAW_LOCK is set)")]
		public float[] q = q;

		[Units("[rad/s]")]
		[Description("X component of angular velocity, positive is rolling to the right, NaN to be ignored.")]
		public float angular_velocity_x = angular_velocity_x;

		[Units("[rad/s]")]
		[Description("Y component of angular velocity, positive is pitching up, NaN to be ignored.")]
		public float angular_velocity_y = angular_velocity_y;

		[Units("[rad/s]")]
		[Description("Z component of angular velocity, positive is yawing to the right, NaN to be ignored.")]
		public float angular_velocity_z = angular_velocity_z;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Component ID of gimbal device to address (or 1-6 for non-MAVLink gimbal), 0 for all gimbal device components. Send command multiple times for more than one gimbal (but not all gimbals).")]
		public byte gimbal_device_id = gimbal_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 145)]
	public struct mavlink_gimbal_device_information_t(ulong uid, uint time_boot_ms, uint firmware_version, uint hardware_version, float roll_min, float roll_max, float pitch_min, float pitch_max, float yaw_min, float yaw_max, ushort cap_flags, ushort custom_cap_flags, byte[] vendor_name, byte[] model_name, byte[] custom_name, byte gimbal_device_id)
	{
		[Units("")]
		[Description("UID of gimbal hardware (0 if unknown).")]
		public ulong uid = uid;

		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("Version of the gimbal firmware, encoded as: `(Dev & 0xff) << 24 | (Patch & 0xff) << 16 | (Minor & 0xff) << 8 | (Major & 0xff)`.")]
		public uint firmware_version = firmware_version;

		[Units("")]
		[Description("Version of the gimbal hardware, encoded as: `(Dev & 0xff) << 24 | (Patch & 0xff) << 16 | (Minor & 0xff) << 8 | (Major & 0xff)`.")]
		public uint hardware_version = hardware_version;

		[Units("[rad]")]
		[Description("Minimum hardware roll angle (positive: rolling to the right, negative: rolling to the left). NAN if unknown.")]
		public float roll_min = roll_min;

		[Units("[rad]")]
		[Description("Maximum hardware roll angle (positive: rolling to the right, negative: rolling to the left). NAN if unknown.")]
		public float roll_max = roll_max;

		[Units("[rad]")]
		[Description("Minimum hardware pitch angle (positive: up, negative: down). NAN if unknown.")]
		public float pitch_min = pitch_min;

		[Units("[rad]")]
		[Description("Maximum hardware pitch angle (positive: up, negative: down). NAN if unknown.")]
		public float pitch_max = pitch_max;

		[Units("[rad]")]
		[Description("Minimum hardware yaw angle (positive: to the right, negative: to the left). NAN if unknown.")]
		public float yaw_min = yaw_min;

		[Units("[rad]")]
		[Description("Maximum hardware yaw angle (positive: to the right, negative: to the left). NAN if unknown.")]
		public float yaw_max = yaw_max;

		[Units("")]
		[Description("Bitmap of gimbal capability flags.")]
		public ushort cap_flags = cap_flags;

		[Units("")]
		[Description("Bitmap for use for gimbal-specific capability flags.")]
		public ushort custom_cap_flags = custom_cap_flags;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Name of the gimbal vendor.")]
		public byte[] vendor_name = vendor_name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Name of the gimbal model.")]
		public byte[] model_name = model_name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Custom name of the gimbal given to it by the user.")]
		public byte[] custom_name = custom_name;

		[Units("")]
		[Description("This field is to be used if the gimbal manager and the gimbal device are the same component and hence have the same component ID. This field is then set to a number between 1-6. If the component ID is separate, this field is not required and must be set to 0.")]
		public byte gimbal_device_id = gimbal_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
	public struct mavlink_gimbal_device_set_attitude_t(float[] q, float angular_velocity_x, float angular_velocity_y, float angular_velocity_z, ushort flags, byte target_system, byte target_component)
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation). The frame is described in the message description. Set fields to NaN to be ignored.")]
		public float[] q = q;

		[Units("[rad/s]")]
		[Description("X component of angular velocity (positive: rolling to the right). The frame is described in the message description. NaN to be ignored.")]
		public float angular_velocity_x = angular_velocity_x;

		[Units("[rad/s]")]
		[Description("Y component of angular velocity (positive: pitching up). The frame is described in the message description. NaN to be ignored.")]
		public float angular_velocity_y = angular_velocity_y;

		[Units("[rad/s]")]
		[Description("Z component of angular velocity (positive: yawing to the right). The frame is described in the message description. NaN to be ignored.")]
		public float angular_velocity_z = angular_velocity_z;

		[Units("")]
		[Description("Low level gimbal flags.")]
		public ushort flags = flags;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 49)]
	public struct mavlink_gimbal_device_attitude_status_t(uint time_boot_ms, float[] q, float angular_velocity_x, float angular_velocity_y, float angular_velocity_z, uint failure_flags, ushort flags, byte target_system, byte target_component, float delta_yaw, float delta_yaw_velocity, byte gimbal_device_id)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation). The frame is described in the message description.")]
		public float[] q = q;

		[Units("[rad/s]")]
		[Description("X component of angular velocity (positive: rolling to the right). The frame is described in the message description. NaN if unknown.")]
		public float angular_velocity_x = angular_velocity_x;

		[Units("[rad/s]")]
		[Description("Y component of angular velocity (positive: pitching up). The frame is described in the message description. NaN if unknown.")]
		public float angular_velocity_y = angular_velocity_y;

		[Units("[rad/s]")]
		[Description("Z component of angular velocity (positive: yawing to the right). The frame is described in the message description. NaN if unknown.")]
		public float angular_velocity_z = angular_velocity_z;

		[Units("")]
		[Description("Failure flags (0 for no failure)")]
		public uint failure_flags = failure_flags;

		[Units("")]
		[Description("Current gimbal flags set.")]
		public ushort flags = flags;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("[rad]")]
		[Description("Yaw angle relating the quaternions in earth and body frames (see message description). NaN if unknown.")]
		public float delta_yaw = delta_yaw;

		[Units("[rad/s]")]
		[Description("Yaw angular velocity relating the angular velocities in earth and body frames (see message description). NaN if unknown.")]
		public float delta_yaw_velocity = delta_yaw_velocity;

		[Units("")]
		[Description("This field is to be used if the gimbal manager and the gimbal device are the same component and hence have the same component ID. This field is then set a number between 1-6. If the component ID is separate, this field is not required and must be set to 0.")]
		public byte gimbal_device_id = gimbal_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 57)]
	public struct mavlink_autopilot_state_for_gimbal_device_t(ulong time_boot_us, float[] q, uint q_estimated_delay_us, float vx, float vy, float vz, uint v_estimated_delay_us, float feed_forward_angular_velocity_z, ushort estimator_status, byte target_system, byte target_component, byte landed_state, float angular_velocity_z)
	{
		[Units("[us]")]
		[Description("Timestamp (time since system boot).")]
		public ulong time_boot_us = time_boot_us;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion components of autopilot attitude: w, x, y, z (1 0 0 0 is the null-rotation, Hamilton convention).")]
		public float[] q = q;

		[Units("[us]")]
		[Description("Estimated delay of the attitude data. 0 if unknown.")]
		public uint q_estimated_delay_us = q_estimated_delay_us;

		[Units("[m/s]")]
		[Description("X Speed in NED (North, East, Down). NAN if unknown.")]
		public float vx = vx;

		[Units("[m/s]")]
		[Description("Y Speed in NED (North, East, Down). NAN if unknown.")]
		public float vy = vy;

		[Units("[m/s]")]
		[Description("Z Speed in NED (North, East, Down). NAN if unknown.")]
		public float vz = vz;

		[Units("[us]")]
		[Description("Estimated delay of the speed data. 0 if unknown.")]
		public uint v_estimated_delay_us = v_estimated_delay_us;

		[Units("[rad/s]")]
		[Description("Feed forward Z component of angular velocity (positive: yawing to the right). NaN to be ignored. This is to indicate if the autopilot is actively yawing.")]
		public float feed_forward_angular_velocity_z = feed_forward_angular_velocity_z;

		[Units("")]
		[Description("Bitmap indicating which estimator outputs are valid.")]
		public ushort estimator_status = estimator_status;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("The landed state. Is set to MAV_LANDED_STATE_UNDEFINED if landed state is unknown.")]
		public byte landed_state = landed_state;

		[Units("[rad/s]")]
		[Description("Z component of angular velocity in NED (North, East, Down). NaN if unknown.")]
		public float angular_velocity_z = angular_velocity_z;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 23)]
	public struct mavlink_gimbal_manager_set_pitchyaw_t(uint flags, float pitch, float yaw, float pitch_rate, float yaw_rate, byte target_system, byte target_component, byte gimbal_device_id)
	{
		[Units("")]
		[Description("High level gimbal manager flags to use.")]
		public uint flags = flags;

		[Units("[rad]")]
		[Description("Pitch angle (positive: up, negative: down, NaN to be ignored).")]
		public float pitch = pitch;

		[Units("[rad]")]
		[Description("Yaw angle (positive: to the right, negative: to the left, NaN to be ignored).")]
		public float yaw = yaw;

		[Units("[rad/s]")]
		[Description("Pitch angular rate (positive: up, negative: down, NaN to be ignored).")]
		public float pitch_rate = pitch_rate;

		[Units("[rad/s]")]
		[Description("Yaw angular rate (positive: to the right, negative: to the left, NaN to be ignored).")]
		public float yaw_rate = yaw_rate;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Component ID of gimbal device to address (or 1-6 for non-MAVLink gimbal), 0 for all gimbal device components. Send command multiple times for more than one gimbal (but not all gimbals).")]
		public byte gimbal_device_id = gimbal_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 23)]
	public struct mavlink_gimbal_manager_set_manual_control_t(uint flags, float pitch, float yaw, float pitch_rate, float yaw_rate, byte target_system, byte target_component, byte gimbal_device_id)
	{
		[Units("")]
		[Description("High level gimbal manager flags.")]
		public uint flags = flags;

		[Units("")]
		[Description("Pitch angle unitless (-1..1, positive: up, negative: down, NaN to be ignored).")]
		public float pitch = pitch;

		[Units("")]
		[Description("Yaw angle unitless (-1..1, positive: to the right, negative: to the left, NaN to be ignored).")]
		public float yaw = yaw;

		[Units("")]
		[Description("Pitch angular rate unitless (-1..1, positive: up, negative: down, NaN to be ignored).")]
		public float pitch_rate = pitch_rate;

		[Units("")]
		[Description("Yaw angular rate unitless (-1..1, positive: to the right, negative: to the left, NaN to be ignored).")]
		public float yaw_rate = yaw_rate;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Component ID of gimbal device to address (or 1-6 for non-MAVLink gimbal), 0 for all gimbal device components. Send command multiple times for more than one gimbal (but not all gimbals).")]
		public byte gimbal_device_id = gimbal_device_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 46)]
	public struct mavlink_esc_info_t(ulong time_usec, uint[] error_count, ushort counter, ushort[] failure_flags, short[] temperature, byte index, byte count, byte connection_type, byte info)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Number of reported errors by each ESC since boot.")]
		public uint[] error_count = error_count;

		[Units("")]
		[Description("Counter of data packets received.")]
		public ushort counter = counter;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Bitmap of ESC failure flags.")]
		public ushort[] failure_flags = failure_flags;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("[cdegC]")]
		[Description("Temperature of each ESC. INT16_MAX: if data not supplied by ESC.")]
		public short[] temperature = temperature;

		[Units("")]
		[Description("Index of the first ESC in this message. minValue = 0, maxValue = 60, increment = 4.")]
		public byte index = index;

		[Units("")]
		[Description("Total number of ESCs in all messages of this type. Message fields with an index higher than this should be ignored because they contain invalid data.")]
		public byte count = count;

		[Units("")]
		[Description("Connection type protocol for all ESC.")]
		public byte connection_type = connection_type;

		[Units("")]
		[Description("Information regarding online/offline status of each ESC.")]
		public byte info = info;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 57)]
	public struct mavlink_esc_status_t(ulong time_usec, int[] rpm, float[] voltage, float[] current, byte index)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("[rpm]")]
		[Description("Reported motor RPM from each ESC (negative for reverse rotation).")]
		public int[] rpm = rpm;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("[V]")]
		[Description("Voltage measured from each ESC.")]
		public float[] voltage = voltage;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("[A]")]
		[Description("Current measured from each ESC.")]
		public float[] current = current;

		[Units("")]
		[Description("Index of the first ESC in this message. minValue = 0, maxValue = 60, increment = 4.")]
		public byte index = index;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 98)]
	public struct mavlink_wifi_config_ap_t(byte[] ssid, byte[] password, sbyte mode, sbyte response)
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Name of Wi-Fi network (SSID). Blank to leave it unchanged when setting. Current SSID when sent back as a response.")]
		public byte[] ssid = ssid;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		[Units("")]
		[Description("Password. Blank for an open AP. MD5 hash when message is sent back as a response.")]
		public byte[] password = password;

		[Units("")]
		[Description("WiFi Mode.")]
		public sbyte mode = mode;

		[Units("")]
		[Description("Message acceptance response (sent back to GS).")]
		public sbyte response = response;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 58)]
	public struct mavlink_ais_vessel_t(uint MMSI, int lat, int lon, ushort COG, ushort heading, ushort velocity, ushort dimension_bow, ushort dimension_stern, ushort tslc, ushort flags, sbyte turn_rate, byte navigational_status, byte type, byte dimension_port, byte dimension_starboard, byte[] callsign, byte[] name)
	{
		[Units("")]
		[Description("Mobile Marine Service Identifier, 9 decimal digits")]
		public uint MMSI = MMSI;

		[Units("[degE7]")]
		[Description("Latitude")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude")]
		public int lon = lon;

		[Units("[cdeg]")]
		[Description("Course over ground")]
		public ushort COG = COG;

		[Units("[cdeg]")]
		[Description("True heading")]
		public ushort heading = heading;

		[Units("[cm/s]")]
		[Description("Speed over ground")]
		public ushort velocity = velocity;

		[Units("[m]")]
		[Description("Distance from lat/lon location to bow")]
		public ushort dimension_bow = dimension_bow;

		[Units("[m]")]
		[Description("Distance from lat/lon location to stern")]
		public ushort dimension_stern = dimension_stern;

		[Units("[s]")]
		[Description("Time since last communication in seconds")]
		public ushort tslc = tslc;

		[Units("")]
		[Description("Bitmask to indicate various statuses including valid data fields")]
		public ushort flags = flags;

		[Units("[ddeg/s]")]
		[Description("Turn rate, 0.1 degrees per second")]
		public sbyte turn_rate = turn_rate;

		[Units("")]
		[Description("Navigational status")]
		public byte navigational_status = navigational_status;

		[Units("")]
		[Description("Type of vessels")]
		public byte type = type;

		[Units("[m]")]
		[Description("Distance from lat/lon location to port side")]
		public byte dimension_port = dimension_port;

		[Units("[m]")]
		[Description("Distance from lat/lon location to starboard side")]
		public byte dimension_starboard = dimension_starboard;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		[Units("")]
		[Description("The vessel callsign")]
		public byte[] callsign = callsign;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("The vessel name")]
		public byte[] name = name;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 17)]
	public struct mavlink_uavcan_node_status_t(ulong time_usec, uint uptime_sec, ushort vendor_specific_status_code, byte health, byte mode, byte sub_mode)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[s]")]
		[Description("Time since the start-up of the node.")]
		public uint uptime_sec = uptime_sec;

		[Units("")]
		[Description("Vendor-specific status information.")]
		public ushort vendor_specific_status_code = vendor_specific_status_code;

		[Units("")]
		[Description("Generalized node health status.")]
		public byte health = health;

		[Units("")]
		[Description("Generalized operating mode.")]
		public byte mode = mode;

		[Units("")]
		[Description("Not used currently.")]
		public byte sub_mode = sub_mode;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 116)]
	public struct mavlink_uavcan_node_info_t(ulong time_usec, uint uptime_sec, uint sw_vcs_commit, byte[] name, byte hw_version_major, byte hw_version_minor, byte[] hw_unique_id, byte sw_version_major, byte sw_version_minor)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[s]")]
		[Description("Time since the start-up of the node.")]
		public uint uptime_sec = uptime_sec;

		[Units("")]
		[Description("Version control system (VCS) revision identifier (e.g. git short commit hash). 0 if unknown.")]
		public uint sw_vcs_commit = sw_vcs_commit;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
		[Units("")]
		[Description("Node name string. For example, 'sapog.px4.io'.")]
		public byte[] name = name;

		[Units("")]
		[Description("Hardware major version number.")]
		public byte hw_version_major = hw_version_major;

		[Units("")]
		[Description("Hardware minor version number.")]
		public byte hw_version_minor = hw_version_minor;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Hardware unique 128-bit ID.")]
		public byte[] hw_unique_id = hw_unique_id;

		[Units("")]
		[Description("Software major version number.")]
		public byte sw_version_major = sw_version_major;

		[Units("")]
		[Description("Software minor version number.")]
		public byte sw_version_minor = sw_version_minor;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
	public struct mavlink_param_ext_request_read_t(short param_index, byte target_system, byte target_component, byte[] param_id)
	{
		[Units("")]
		[Description("Parameter index. Set to -1 to use the Parameter ID field as identifier (else param_id will be ignored)")]
		public short param_index = param_index;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
		public byte[] param_id = param_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
	public struct mavlink_param_ext_request_list_t(byte target_system, byte target_component)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 149)]
	public struct mavlink_param_ext_value_t(ushort param_count, ushort param_index, byte[] param_id, byte[] param_value, byte param_type)
	{
		[Units("")]
		[Description("Total number of parameters")]
		public ushort param_count = param_count;

		[Units("")]
		[Description("Index of this parameter")]
		public ushort param_index = param_index;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
		public byte[] param_id = param_id;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		[Units("")]
		[Description("Parameter value")]
		public byte[] param_value = param_value;

		[Units("")]
		[Description("Parameter type.")]
		public byte param_type = param_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 147)]
	public struct mavlink_param_ext_set_t(byte target_system, byte target_component, byte[] param_id, byte[] param_value, byte param_type)
	{
		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
		public byte[] param_id = param_id;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		[Units("")]
		[Description("Parameter value")]
		public byte[] param_value = param_value;

		[Units("")]
		[Description("Parameter type.")]
		public byte param_type = param_type;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 146)]
	public struct mavlink_param_ext_ack_t(byte[] param_id, byte[] param_value, byte param_type, byte param_result)
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string")]
		public byte[] param_id = param_id;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		[Units("")]
		[Description("Parameter value (new value if PARAM_ACK_ACCEPTED, current value otherwise)")]
		public byte[] param_value = param_value;

		[Units("")]
		[Description("Parameter type.")]
		public byte param_type = param_type;

		[Units("")]
		[Description("Result code.")]
		public byte param_result = param_result;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 167)]
	public struct mavlink_obstacle_distance_t(ulong time_usec, ushort[] distances, ushort min_distance, ushort max_distance, byte sensor_type, byte increment, float increment_f, float angle_offset, byte frame)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 72)]
		[Units("[cm]")]
		[Description("Distance of obstacles around the vehicle with index 0 corresponding to north + angle_offset, unless otherwise specified in the frame. A value of 0 is valid and means that the obstacle is practically touching the sensor. A value of max_distance +1 means no obstacle is present. A value of UINT16_MAX for unknown/not used. In a array element, one unit corresponds to 1cm.")]
		public ushort[] distances = distances;

		[Units("[cm]")]
		[Description("Minimum distance the sensor can measure.")]
		public ushort min_distance = min_distance;

		[Units("[cm]")]
		[Description("Maximum distance the sensor can measure.")]
		public ushort max_distance = max_distance;

		[Units("")]
		[Description("Class id of the distance sensor type.")]
		public byte sensor_type = sensor_type;

		[Units("[deg]")]
		[Description("Angular width in degrees of each array element. Increment direction is clockwise. This field is ignored if increment_f is non-zero.")]
		public byte increment = increment;

		[Units("[deg]")]
		[Description("Angular width in degrees of each array element as a float. If non-zero then this value is used instead of the uint8_t increment field. Positive is clockwise direction, negative is counter-clockwise.")]
		public float increment_f = increment_f;

		[Units("[deg]")]
		[Description("Relative angle offset of the 0-index element in the distances array. Value of 0 corresponds to forward. Positive is clockwise direction, negative is counter-clockwise.")]
		public float angle_offset = angle_offset;

		[Units("")]
		[Description("Coordinate frame of reference for the yaw rotation and offset of the sensor data. Defaults to MAV_FRAME_GLOBAL, which is north aligned. For body-mounted sensors use MAV_FRAME_BODY_FRD, which is vehicle front aligned.")]
		public byte frame = frame;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 233)]
	public struct mavlink_odometry_t(ulong time_usec, float x, float y, float z, float[] q, float vx, float vy, float vz, float rollspeed, float pitchspeed, float yawspeed, float[] pose_covariance, float[] velocity_covariance, byte frame_id, byte child_frame_id, byte reset_counter, byte estimator_type, sbyte quality)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[m]")]
		[Description("X Position")]
		public float x = x;

		[Units("[m]")]
		[Description("Y Position")]
		public float y = y;

		[Units("[m]")]
		[Description("Z Position")]
		public float z = z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation)")]
		public float[] q = q;

		[Units("[m/s]")]
		[Description("X linear speed")]
		public float vx = vx;

		[Units("[m/s]")]
		[Description("Y linear speed")]
		public float vy = vy;

		[Units("[m/s]")]
		[Description("Z linear speed")]
		public float vz = vz;

		[Units("[rad/s]")]
		[Description("Roll angular speed")]
		public float rollspeed = rollspeed;

		[Units("[rad/s]")]
		[Description("Pitch angular speed")]
		public float pitchspeed = pitchspeed;

		[Units("[rad/s]")]
		[Description("Yaw angular speed")]
		public float yawspeed = yawspeed;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
		[Units("")]
		[Description("Row-major representation of a 6x6 pose cross-covariance matrix upper right triangle (states: x, y, z, roll, pitch, yaw; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] pose_covariance = pose_covariance;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
		[Units("")]
		[Description("Row-major representation of a 6x6 velocity cross-covariance matrix upper right triangle (states: vx, vy, vz, rollspeed, pitchspeed, yawspeed; first six entries are the first ROW, next five entries are the second ROW, etc.). If unknown, assign NaN value to first element in the array.")]
		public float[] velocity_covariance = velocity_covariance;

		[Units("")]
		[Description("Coordinate frame of reference for the pose data.")]
		public byte frame_id = frame_id;

		[Units("")]
		[Description("Coordinate frame of reference for the velocity in free space (twist) data.")]
		public byte child_frame_id = child_frame_id;

		[Units("")]
		[Description("Estimate reset counter. This should be incremented when the estimate resets in any of the dimensions (position, velocity, attitude, angular speed). This is designed to be used when e.g an external SLAM system detects a loop-closure and the estimate jumps.")]
		public byte reset_counter = reset_counter;

		[Units("")]
		[Description("Type of estimator that is providing the odometry.")]
		public byte estimator_type = estimator_type;

		[Units("[%]")]
		[Description("Optional odometry quality metric as a percentage. -1 = odometry has failed, 0 = unknown/unset quality, 1 = worst quality, 100 = best quality")]
		public sbyte quality = quality;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 239)]
	public struct mavlink_trajectory_representation_waypoints_t(ulong time_usec, float[] pos_x, float[] pos_y, float[] pos_z, float[] vel_x, float[] vel_y, float[] vel_z, float[] acc_x, float[] acc_y, float[] acc_z, float[] pos_yaw, float[] vel_yaw, ushort[] command, byte valid_points)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m]")]
		[Description("X-coordinate of waypoint, set to NaN if not being used")]
		public float[] pos_x = pos_x;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m]")]
		[Description("Y-coordinate of waypoint, set to NaN if not being used")]
		public float[] pos_y = pos_y;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m]")]
		[Description("Z-coordinate of waypoint, set to NaN if not being used")]
		public float[] pos_z = pos_z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m/s]")]
		[Description("X-velocity of waypoint, set to NaN if not being used")]
		public float[] vel_x = vel_x;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m/s]")]
		[Description("Y-velocity of waypoint, set to NaN if not being used")]
		public float[] vel_y = vel_y;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m/s]")]
		[Description("Z-velocity of waypoint, set to NaN if not being used")]
		public float[] vel_z = vel_z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m/s/s]")]
		[Description("X-acceleration of waypoint, set to NaN if not being used")]
		public float[] acc_x = acc_x;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m/s/s]")]
		[Description("Y-acceleration of waypoint, set to NaN if not being used")]
		public float[] acc_y = acc_y;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m/s/s]")]
		[Description("Z-acceleration of waypoint, set to NaN if not being used")]
		public float[] acc_z = acc_z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[rad]")]
		[Description("Yaw angle, set to NaN if not being used")]
		public float[] pos_yaw = pos_yaw;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[rad/s]")]
		[Description("Yaw rate, set to NaN if not being used")]
		public float[] vel_yaw = vel_yaw;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("")]
		[Description("MAV_CMD command id of waypoint, set to UINT16_MAX if not being used.")]
		public ushort[] command = command;

		[Units("")]
		[Description("Number of valid points (up-to 5 waypoints are possible)")]
		public byte valid_points = valid_points;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 109)]
	public struct mavlink_trajectory_representation_bezier_t(ulong time_usec, float[] pos_x, float[] pos_y, float[] pos_z, float[] delta, float[] pos_yaw, byte valid_points)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m]")]
		[Description("X-coordinate of bezier control points. Set to NaN if not being used")]
		public float[] pos_x = pos_x;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m]")]
		[Description("Y-coordinate of bezier control points. Set to NaN if not being used")]
		public float[] pos_y = pos_y;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[m]")]
		[Description("Z-coordinate of bezier control points. Set to NaN if not being used")]
		public float[] pos_z = pos_z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[s]")]
		[Description("Bezier time horizon. Set to NaN if velocity/acceleration should not be incorporated")]
		public float[] delta = delta;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		[Units("[rad]")]
		[Description("Yaw. Set to NaN for unchanged")]
		public float[] pos_yaw = pos_yaw;

		[Units("")]
		[Description("Number of valid control points (up-to 5 points are possible)")]
		public byte valid_points = valid_points;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 10)]
	public struct mavlink_cellular_status_t(ushort mcc, ushort mnc, ushort lac, byte status, byte failure_reason, byte type, byte quality)
	{
		[Units("")]
		[Description("Mobile country code. If unknown, set to UINT16_MAX")]
		public ushort mcc = mcc;

		[Units("")]
		[Description("Mobile network code. If unknown, set to UINT16_MAX")]
		public ushort mnc = mnc;

		[Units("")]
		[Description("Location area code. If unknown, set to 0")]
		public ushort lac = lac;

		[Units("")]
		[Description("Cellular modem status")]
		public byte status = status;

		[Units("")]
		[Description("Failure reason when status in in CELLULAR_STATUS_FLAG_FAILED")]
		public byte failure_reason = failure_reason;

		[Units("")]
		[Description("Cellular network radio type: gsm, cdma, lte...")]
		public byte type = type;

		[Units("")]
		[Description("Signal quality in percent. If unknown, set to UINT8_MAX")]
		public byte quality = quality;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
	public struct mavlink_isbd_link_status_t(ulong timestamp, ulong last_heartbeat, ushort failed_sessions, ushort successful_sessions, byte signal_quality, byte ring_pending, byte tx_session_pending, byte rx_session_pending)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong timestamp = timestamp;

		[Units("[us]")]
		[Description("Timestamp of the last successful sbd session. The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong last_heartbeat = last_heartbeat;

		[Units("")]
		[Description("Number of failed SBD sessions.")]
		public ushort failed_sessions = failed_sessions;

		[Units("")]
		[Description("Number of successful SBD sessions.")]
		public ushort successful_sessions = successful_sessions;

		[Units("")]
		[Description("Signal quality equal to the number of bars displayed on the ISU signal strength indicator. Range is 0 to 5, where 0 indicates no signal and 5 indicates maximum signal strength.")]
		public byte signal_quality = signal_quality;

		[Units("")]
		[Description("1: Ring call pending, 0: No call pending.")]
		public byte ring_pending = ring_pending;

		[Units("")]
		[Description("1: Transmission session pending, 0: No transmission session pending.")]
		public byte tx_session_pending = tx_session_pending;

		[Units("")]
		[Description("1: Receiving session pending, 0: No receiving session pending.")]
		public byte rx_session_pending = rx_session_pending;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 84)]
	public struct mavlink_cellular_config_t(byte enable_lte, byte enable_pin, byte[] pin, byte[] new_pin, byte[] apn, byte[] puk, byte roaming, byte response)
	{
		[Units("")]
		[Description("Enable/disable LTE. 0: setting unchanged, 1: disabled, 2: enabled. Current setting when sent back as a response.")]
		public byte enable_lte = enable_lte;

		[Units("")]
		[Description("Enable/disable PIN on the SIM card. 0: setting unchanged, 1: disabled, 2: enabled. Current setting when sent back as a response.")]
		public byte enable_pin = enable_pin;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("PIN sent to the SIM card. Blank when PIN is disabled. Empty when message is sent back as a response.")]
		public byte[] pin = pin;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("New PIN when changing the PIN. Blank to leave it unchanged. Empty when message is sent back as a response.")]
		public byte[] new_pin = new_pin;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Name of the cellular APN. Blank to leave it unchanged. Current APN when sent back as a response.")]
		public byte[] apn = apn;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Required PUK code in case the user failed to authenticate 3 times with the PIN. Empty when message is sent back as a response.")]
		public byte[] puk = puk;

		[Units("")]
		[Description("Enable/disable roaming. 0: setting unchanged, 1: disabled, 2: enabled. Current setting when sent back as a response.")]
		public byte roaming = roaming;

		[Units("")]
		[Description("Message acceptance response (sent back to GS).")]
		public byte response = response;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 5)]
	public struct mavlink_raw_rpm_t(float frequency, byte index)
	{
		[Units("[rpm]")]
		[Description("Indicated rate")]
		public float frequency = frequency;

		[Units("")]
		[Description("Index of this RPM sensor (0-indexed)")]
		public byte index = index;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 70)]
	public struct mavlink_utm_global_position_t(ulong time, int lat, int lon, int alt, int relative_alt, int next_lat, int next_lon, int next_alt, short vx, short vy, short vz, ushort h_acc, ushort v_acc, ushort vel_acc, ushort update_rate, byte[] uas_id, byte flight_state, byte flags)
	{
		[Units("[us]")]
		[Description("Time of applicability of position (microseconds since UNIX epoch).")]
		public ulong time = time;

		[Units("[degE7]")]
		[Description("Latitude (WGS84)")]
		public int lat = lat;

		[Units("[degE7]")]
		[Description("Longitude (WGS84)")]
		public int lon = lon;

		[Units("[mm]")]
		[Description("Altitude (WGS84)")]
		public int alt = alt;

		[Units("[mm]")]
		[Description("Altitude above ground")]
		public int relative_alt = relative_alt;

		[Units("[degE7]")]
		[Description("Next waypoint, latitude (WGS84)")]
		public int next_lat = next_lat;

		[Units("[degE7]")]
		[Description("Next waypoint, longitude (WGS84)")]
		public int next_lon = next_lon;

		[Units("[mm]")]
		[Description("Next waypoint, altitude (WGS84)")]
		public int next_alt = next_alt;

		[Units("[cm/s]")]
		[Description("Ground X speed (latitude, positive north)")]
		public short vx = vx;

		[Units("[cm/s]")]
		[Description("Ground Y speed (longitude, positive east)")]
		public short vy = vy;

		[Units("[cm/s]")]
		[Description("Ground Z speed (altitude, positive down)")]
		public short vz = vz;

		[Units("[mm]")]
		[Description("Horizontal position uncertainty (standard deviation)")]
		public ushort h_acc = h_acc;

		[Units("[mm]")]
		[Description("Altitude uncertainty (standard deviation)")]
		public ushort v_acc = v_acc;

		[Units("[cm/s]")]
		[Description("Speed uncertainty (standard deviation)")]
		public ushort vel_acc = vel_acc;

		[Units("[cs]")]
		[Description("Time until next update. Set to 0 if unknown or in data driven mode.")]
		public ushort update_rate = update_rate;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
		[Units("")]
		[Description("Unique UAS ID.")]
		public byte[] uas_id = uas_id;

		[Units("")]
		[Description("Flight state")]
		public byte flight_state = flight_state;

		[Units("")]
		[Description("Bitwise OR combination of the data available flags.")]
		public byte flags = flags;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 252)]
	public struct mavlink_debug_float_array_t(ulong time_usec, ushort array_id, byte[] name, float[] data)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("Unique ID used to discriminate between arrays")]
		public ushort array_id = array_id;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		[Units("")]
		[Description("Name, for human-friendly display in a Ground Control Station")]
		public byte[] name = name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 58)]
		[Units("")]
		[Description("data")]
		public float[] data = data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 25)]
	public struct mavlink_orbit_execution_status_t(ulong time_usec, float radius, int x, int y, float z, byte frame)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[m]")]
		[Description("Radius of the orbit circle. Positive values orbit clockwise, negative values orbit counter-clockwise.")]
		public float radius = radius;

		[Units("")]
		[Description("X coordinate of center point. Coordinate system depends on frame field: local = x position in meters * 1e4, global = latitude in degrees * 1e7.")]
		public int x = x;

		[Units("")]
		[Description("Y coordinate of center point.  Coordinate system depends on frame field: local = x position in meters * 1e4, global = latitude in degrees * 1e7.")]
		public int y = y;

		[Units("[m]")]
		[Description("Altitude of center point. Coordinate system depends on frame field.")]
		public float z = z;

		[Units("")]
		[Description("The coordinate system of the fields: x, y, z.")]
		public byte frame = frame;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 109)]
	public struct mavlink_smart_battery_info_t(int capacity_full_specification, int capacity_full, ushort cycle_count, ushort weight, ushort discharge_minimum_voltage, ushort charging_minimum_voltage, ushort resting_minimum_voltage, byte id, byte battery_function, byte type, byte[] serial_number, byte[] device_name, ushort charging_maximum_voltage, byte cells_in_series, uint discharge_maximum_current, uint discharge_maximum_burst_current, byte[] manufacture_date)
	{
		[Units("[mAh]")]
		[Description("Capacity when full according to manufacturer, -1: field not provided.")]
		public int capacity_full_specification = capacity_full_specification;

		[Units("[mAh]")]
		[Description("Capacity when full (accounting for battery degradation), -1: field not provided.")]
		public int capacity_full = capacity_full;

		[Units("")]
		[Description("Charge/discharge cycle count. UINT16_MAX: field not provided.")]
		public ushort cycle_count = cycle_count;

		[Units("[g]")]
		[Description("Battery weight. 0: field not provided.")]
		public ushort weight = weight;

		[Units("[mV]")]
		[Description("Minimum per-cell voltage when discharging. If not supplied set to UINT16_MAX value.")]
		public ushort discharge_minimum_voltage = discharge_minimum_voltage;

		[Units("[mV]")]
		[Description("Minimum per-cell voltage when charging. If not supplied set to UINT16_MAX value.")]
		public ushort charging_minimum_voltage = charging_minimum_voltage;

		[Units("[mV]")]
		[Description("Minimum per-cell voltage when resting. If not supplied set to UINT16_MAX value.")]
		public ushort resting_minimum_voltage = resting_minimum_voltage;

		[Units("")]
		[Description("Battery ID")]
		public byte id = id;

		[Units("")]
		[Description("Function of the battery")]
		public byte battery_function = battery_function;

		[Units("")]
		[Description("Type (chemistry) of the battery")]
		public byte type = type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("Serial number in ASCII characters, 0 terminated. All 0: field not provided.")]
		public byte[] serial_number = serial_number;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
		[Units("")]
		[Description("Static device name in ASCII characters, 0 terminated. All 0: field not provided. Encode as manufacturer name then product name separated using an underscore.")]
		public byte[] device_name = device_name;

		[Units("[mV]")]
		[Description("Maximum per-cell voltage when charged. 0: field not provided.")]
		public ushort charging_maximum_voltage = charging_maximum_voltage;

		[Units("")]
		[Description("Number of battery cells in series. 0: field not provided.")]
		public byte cells_in_series = cells_in_series;

		[Units("[mA]")]
		[Description("Maximum pack discharge current. 0: field not provided.")]
		public uint discharge_maximum_current = discharge_maximum_current;

		[Units("[mA]")]
		[Description("Maximum pack discharge burst current. 0: field not provided.")]
		public uint discharge_maximum_burst_current = discharge_maximum_burst_current;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
		[Units("")]
		[Description("Manufacture date (DD/MM/YYYY) in ASCII characters, 0 terminated. All 0: field not provided.")]
		public byte[] manufacture_date = manufacture_date;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 26)]
	public struct mavlink_fuel_status_t(float maximum_fuel, float consumed_fuel, float remaining_fuel, float flow_rate, float temperature, uint fuel_type, byte id, byte percent_remaining)
	{
		[Units("")]
		[Description("Capacity when full. Must be provided.")]
		public float maximum_fuel = maximum_fuel;

		[Units("")]
		[Description("Consumed fuel (measured). This value should not be inferred: if not measured set to NaN. NaN: field not provided.")]
		public float consumed_fuel = consumed_fuel;

		[Units("")]
		[Description("Remaining fuel until empty (measured). The value should not be inferred: if not measured set to NaN. NaN: field not provided.")]
		public float remaining_fuel = remaining_fuel;

		[Units("")]
		[Description("Positive value when emptying/using, and negative if filling/replacing. NaN: field not provided.")]
		public float flow_rate = flow_rate;

		[Units("[K]")]
		[Description("Fuel temperature. NaN: field not provided.")]
		public float temperature = temperature;

		[Units("")]
		[Description("Fuel type. Defines units for fuel capacity and consumption fields above.")]
		public uint fuel_type = fuel_type;

		[Units("")]
		[Description("Fuel ID. Must match ID of other messages for same fuel system, such as BATTERY_STATUS_V2.")]
		public byte id = id;

		[Units("[%]")]
		[Description("Percentage of remaining fuel, relative to full. Values: [0-100], UINT8_MAX: field not provided.")]
		public byte percent_remaining = percent_remaining;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 140)]
	public struct mavlink_battery_info_t(float discharge_minimum_voltage, float charging_minimum_voltage, float resting_minimum_voltage, float charging_maximum_voltage, float charging_maximum_current, float nominal_voltage, float discharge_maximum_current, float discharge_maximum_burst_current, float design_capacity, float full_charge_capacity, ushort cycle_count, ushort weight, byte id, byte battery_function, byte type, byte state_of_health, byte cells_in_series, byte[] manufacture_date, byte[] serial_number, byte[] name)
	{
		[Units("[V]")]
		[Description("Minimum per-cell voltage when discharging. 0: field not provided.")]
		public float discharge_minimum_voltage = discharge_minimum_voltage;

		[Units("[V]")]
		[Description("Minimum per-cell voltage when charging. 0: field not provided.")]
		public float charging_minimum_voltage = charging_minimum_voltage;

		[Units("[V]")]
		[Description("Minimum per-cell voltage when resting. 0: field not provided.")]
		public float resting_minimum_voltage = resting_minimum_voltage;

		[Units("[V]")]
		[Description("Maximum per-cell voltage when charged. 0: field not provided.")]
		public float charging_maximum_voltage = charging_maximum_voltage;

		[Units("[A]")]
		[Description("Maximum pack continuous charge current. 0: field not provided.")]
		public float charging_maximum_current = charging_maximum_current;

		[Units("[V]")]
		[Description("Battery nominal voltage. Used for conversion between Wh and Ah. 0: field not provided.")]
		public float nominal_voltage = nominal_voltage;

		[Units("[A]")]
		[Description("Maximum pack discharge current. 0: field not provided.")]
		public float discharge_maximum_current = discharge_maximum_current;

		[Units("[A]")]
		[Description("Maximum pack discharge burst current. 0: field not provided.")]
		public float discharge_maximum_burst_current = discharge_maximum_burst_current;

		[Units("[Ah]")]
		[Description("Fully charged design capacity. 0: field not provided.")]
		public float design_capacity = design_capacity;

		[Units("[Ah]")]
		[Description("Predicted battery capacity when fully charged (accounting for battery degradation). NAN: field not provided.")]
		public float full_charge_capacity = full_charge_capacity;

		[Units("")]
		[Description("Lifetime count of the number of charge/discharge cycles (https://en.wikipedia.org/wiki/Charge_cycle). UINT16_MAX: field not provided.")]
		public ushort cycle_count = cycle_count;

		[Units("[g]")]
		[Description("Battery weight. 0: field not provided.")]
		public ushort weight = weight;

		[Units("")]
		[Description("Battery ID")]
		public byte id = id;

		[Units("")]
		[Description("Function of the battery.")]
		public byte battery_function = battery_function;

		[Units("")]
		[Description("Type (chemistry) of the battery.")]
		public byte type = type;

		[Units("[%]")]
		[Description("State of Health (SOH) estimate. Typically 100% at the time of manufacture and will decrease over time and use. -1: field not provided.")]
		public byte state_of_health = state_of_health;

		[Units("")]
		[Description("Number of battery cells in series. 0: field not provided.")]
		public byte cells_in_series = cells_in_series;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
		[Units("")]
		[Description("Manufacture date (DDMMYYYY) in ASCII characters, 0 terminated. All 0: field not provided.")]
		public byte[] manufacture_date = manufacture_date;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Serial number in ASCII characters, 0 terminated. All 0: field not provided.")]
		public byte[] serial_number = serial_number;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
		[Units("")]
		[Description("Battery device name. Formatted as manufacturer name then product name, separated with an underscore (in ASCII characters), 0 terminated. All 0: field not provided.")]
		public byte[] name = name;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 42)]
	public struct mavlink_generator_status_t(ulong status, float battery_current, float load_current, float power_generated, float bus_voltage, float bat_current_setpoint, uint runtime, int time_until_maintenance, ushort generator_speed, short rectifier_temperature, short generator_temperature)
	{
		[Units("")]
		[Description("Status flags.")]
		public ulong status = status;

		[Units("[A]")]
		[Description("Current into/out of battery. Positive for out. Negative for in. NaN: field not provided.")]
		public float battery_current = battery_current;

		[Units("[A]")]
		[Description("Current going to the UAV. If battery current not available this is the DC current from the generator. Positive for out. Negative for in. NaN: field not provided")]
		public float load_current = load_current;

		[Units("[W]")]
		[Description("The power being generated. NaN: field not provided")]
		public float power_generated = power_generated;

		[Units("[V]")]
		[Description("Voltage of the bus seen at the generator, or battery bus if battery bus is controlled by generator and at a different voltage to main bus.")]
		public float bus_voltage = bus_voltage;

		[Units("[A]")]
		[Description("The target battery current. Positive for out. Negative for in. NaN: field not provided")]
		public float bat_current_setpoint = bat_current_setpoint;

		[Units("[s]")]
		[Description("Seconds this generator has run since it was rebooted. UINT32_MAX: field not provided.")]
		public uint runtime = runtime;

		[Units("[s]")]
		[Description("Seconds until this generator requires maintenance.  A negative value indicates maintenance is past-due. INT32_MAX: field not provided.")]
		public int time_until_maintenance = time_until_maintenance;

		[Units("[rpm]")]
		[Description("Speed of electrical generator or alternator. UINT16_MAX: field not provided.")]
		public ushort generator_speed = generator_speed;

		[Units("[degC]")]
		[Description("The temperature of the rectifier or power converter. INT16_MAX: field not provided.")]
		public short rectifier_temperature = rectifier_temperature;

		[Units("[degC]")]
		[Description("The temperature of the mechanical motor, fuel cell core or generator. INT16_MAX: field not provided.")]
		public short generator_temperature = generator_temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 140)]
	public struct mavlink_actuator_output_status_t(ulong time_usec, uint active, float[] actuator)
	{
		[Units("[us]")]
		[Description("Timestamp (since system boot).")]
		public ulong time_usec = time_usec;

		[Units("")]
		[Description("Active outputs")]
		public uint active = active;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Servo / motor output array values. Zero values indicate unused channels.")]
		public float[] actuator = actuator;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
	public struct mavlink_time_estimate_to_target_t(int safe_return, int land, int mission_next_item, int mission_end, int commanded_action)
	{
		[Units("[s]")]
		[Description("Estimated time to complete the vehicle's configured 'safe return' action from its current position (e.g. RTL, Smart RTL, etc.). -1 indicates that the vehicle is landed, or that no time estimate available.")]
		public int safe_return = safe_return;

		[Units("[s]")]
		[Description("Estimated time for vehicle to complete the LAND action from its current position. -1 indicates that the vehicle is landed, or that no time estimate available.")]
		public int land = land;

		[Units("[s]")]
		[Description("Estimated time for reaching/completing the currently active mission item. -1 means no time estimate available.")]
		public int mission_next_item = mission_next_item;

		[Units("[s]")]
		[Description("Estimated time for completing the current mission. -1 means no mission active and/or no estimate available.")]
		public int mission_end = mission_end;

		[Units("[s]")]
		[Description("Estimated time for completing the current commanded action (i.e. Go To, Takeoff, Land, etc.). -1 means no action active and/or no estimate available.")]
		public int commanded_action = commanded_action;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 133)]
	public struct mavlink_tunnel_t(ushort payload_type, byte target_system, byte target_component, byte payload_length, byte[] payload)
	{
		[Units("")]
		[Description("A code that identifies the content of the payload (0 for unknown, which is the default). If this code is less than 32768, it is a 'registered' payload type and the corresponding code should be added to the MAV_TUNNEL_PAYLOAD_TYPE enum. Software creators can register blocks of types as needed. Codes greater than 32767 are considered local experiments and should not be checked in to any widely distributed codebase.")]
		public ushort payload_type = payload_type;

		[Units("")]
		[Description("System ID (can be 0 for broadcast, but this is discouraged)")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (can be 0 for broadcast, but this is discouraged)")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Length of the data transported in payload")]
		public byte payload_length = payload_length;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		[Units("")]
		[Description("Variable length payload. The payload length is defined by payload_length. The entire content of this block is opaque unless you understand the encoding specified by payload_type.")]
		public byte[] payload = payload;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct mavlink_can_frame_t(uint id, byte target_system, byte target_component, byte bus, byte len, byte[] data)
	{
		[Units("")]
		[Description("Frame ID")]
		public uint id = id;

		[Units("")]
		[Description("System ID.")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID.")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Bus number")]
		public byte bus = bus;

		[Units("")]
		[Description("Frame length")]
		public byte len = len;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("")]
		[Description("Frame data")]
		public byte[] data = data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 238)]
	public struct mavlink_onboard_computer_status_t(ulong time_usec, uint uptime, uint ram_usage, uint ram_total, uint[] storage_type, uint[] storage_usage, uint[] storage_total, uint[] link_type, uint[] link_tx_rate, uint[] link_rx_rate, uint[] link_tx_max, uint[] link_rx_max, short[] fan_speed, byte type, byte[] cpu_cores, byte[] cpu_combined, byte[] gpu_cores, byte[] gpu_combined, sbyte temperature_board, sbyte[] temperature_core)
	{
		[Units("[us]")]
		[Description("Timestamp (UNIX Epoch time or time since system boot). The receiving end can infer timestamp format (since 1.1.1970 or since system boot) by checking for the magnitude of the number.")]
		public ulong time_usec = time_usec;

		[Units("[ms]")]
		[Description("Time since system boot.")]
		public uint uptime = uptime;

		[Units("[MiB]")]
		[Description("Amount of used RAM on the component system. A value of UINT32_MAX implies the field is unused.")]
		public uint ram_usage = ram_usage;

		[Units("[MiB]")]
		[Description("Total amount of RAM on the component system. A value of UINT32_MAX implies the field is unused.")]
		public uint ram_total = ram_total;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("Storage type: 0: HDD, 1: SSD, 2: EMMC, 3: SD card (non-removable), 4: SD card (removable). A value of UINT32_MAX implies the field is unused.")]
		public uint[] storage_type = storage_type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("[MiB]")]
		[Description("Amount of used storage space on the component system. A value of UINT32_MAX implies the field is unused.")]
		public uint[] storage_usage = storage_usage;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("[MiB]")]
		[Description("Total amount of storage space on the component system. A value of UINT32_MAX implies the field is unused.")]
		public uint[] storage_total = storage_total;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		[Units("")]
		[Description("Link type: 0-9: UART, 10-19: Wired network, 20-29: Wifi, 30-39: Point-to-point proprietary, 40-49: Mesh proprietary")]
		public uint[] link_type = link_type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		[Units("[KiB/s]")]
		[Description("Network traffic from the component system. A value of UINT32_MAX implies the field is unused.")]
		public uint[] link_tx_rate = link_tx_rate;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		[Units("[KiB/s]")]
		[Description("Network traffic to the component system. A value of UINT32_MAX implies the field is unused.")]
		public uint[] link_rx_rate = link_rx_rate;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		[Units("[KiB/s]")]
		[Description("Network capacity from the component system. A value of UINT32_MAX implies the field is unused.")]
		public uint[] link_tx_max = link_tx_max;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		[Units("[KiB/s]")]
		[Description("Network capacity to the component system. A value of UINT32_MAX implies the field is unused.")]
		public uint[] link_rx_max = link_rx_max;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("[rpm]")]
		[Description("Fan speeds. A value of INT16_MAX implies the field is unused.")]
		public short[] fan_speed = fan_speed;

		[Units("")]
		[Description("Type of the onboard computer: 0: Mission computer primary, 1: Mission computer backup 1, 2: Mission computer backup 2, 3: Compute node, 4-5: Compute spares, 6-9: Payload computers.")]
		public byte type = type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("")]
		[Description("CPU usage on the component in percent (100 - idle). A value of UINT8_MAX implies the field is unused.")]
		public byte[] cpu_cores = cpu_cores;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		[Units("")]
		[Description("Combined CPU usage as the last 10 slices of 100 MS (a histogram). This allows to identify spikes in load that max out the system, but only for a short amount of time. A value of UINT8_MAX implies the field is unused.")]
		public byte[] cpu_combined = cpu_combined;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		[Units("")]
		[Description("GPU usage on the component in percent (100 - idle). A value of UINT8_MAX implies the field is unused.")]
		public byte[] gpu_cores = gpu_cores;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		[Units("")]
		[Description("Combined GPU usage as the last 10 slices of 100 MS (a histogram). This allows to identify spikes in load that max out the system, but only for a short amount of time. A value of UINT8_MAX implies the field is unused.")]
		public byte[] gpu_combined = gpu_combined;

		[Units("[degC]")]
		[Description("Temperature of the board. A value of INT8_MAX implies the field is unused.")]
		public sbyte temperature_board = temperature_board;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("[degC]")]
		[Description("Temperature of the CPU core. A value of INT8_MAX implies the field is unused.")]
		public sbyte[] temperature_core = temperature_core;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 212)]
	public struct mavlink_component_information_t(uint time_boot_ms, uint general_metadata_file_crc, uint peripherals_metadata_file_crc, byte[] general_metadata_uri, byte[] peripherals_metadata_uri)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("CRC32 of the general metadata file (general_metadata_uri).")]
		public uint general_metadata_file_crc = general_metadata_file_crc;

		[Units("")]
		[Description("CRC32 of peripherals metadata file (peripherals_metadata_uri).")]
		public uint peripherals_metadata_file_crc = peripherals_metadata_file_crc;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
		[Units("")]
		[Description("MAVLink FTP URI for the general metadata file (COMP_METADATA_TYPE_GENERAL), which may be compressed with xz. The file contains general component metadata, and may contain URI links for additional metadata (see COMP_METADATA_TYPE). The information is static from boot, and may be generated at compile time. The string needs to be zero terminated.")]
		public byte[] general_metadata_uri = general_metadata_uri;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
		[Units("")]
		[Description("(Optional) MAVLink FTP URI for the peripherals metadata file (COMP_METADATA_TYPE_PERIPHERALS), which may be compressed with xz. This contains data about 'attached components' such as UAVCAN nodes. The peripherals are in a separate file because the information must be generated dynamically at runtime. The string needs to be zero terminated.")]
		public byte[] peripherals_metadata_uri = peripherals_metadata_uri;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 160)]
	public struct mavlink_component_information_basic_t(ulong capabilities, uint time_boot_ms, uint time_manufacture_s, byte[] vendor_name, byte[] model_name, byte[] software_version, byte[] hardware_version, byte[] serial_number)
	{
		[Units("")]
		[Description("Component capability flags")]
		public ulong capabilities = capabilities;

		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("[s]")]
		[Description("Date of manufacture as a UNIX Epoch time (since 1.1.1970) in seconds.")]
		public uint time_manufacture_s = time_manufacture_s;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Name of the component vendor. Needs to be zero terminated. The field is optional and can be empty/all zeros.")]
		public byte[] vendor_name = vendor_name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Name of the component model. Needs to be zero terminated. The field is optional and can be empty/all zeros.")]
		public byte[] model_name = model_name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
		[Units("")]
		[Description("Software version. The recommended format is SEMVER: 'major.minor.patch'  (any format may be used). The field must be zero terminated if it has a value. The field is optional and can be empty/all zeros.")]
		public byte[] software_version = software_version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
		[Units("")]
		[Description("Hardware version. The recommended format is SEMVER: 'major.minor.patch'  (any format may be used). The field must be zero terminated if it has a value. The field is optional and can be empty/all zeros.")]
		public byte[] hardware_version = hardware_version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		[Units("")]
		[Description("Hardware serial number. The field must be zero terminated if it has a value. The field is optional and can be empty/all zeros.")]
		public byte[] serial_number = serial_number;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 108)]
	public struct mavlink_component_metadata_t(uint time_boot_ms, uint file_crc, byte[] uri)
	{
		[Units("[ms]")]
		[Description("Timestamp (time since system boot).")]
		public uint time_boot_ms = time_boot_ms;

		[Units("")]
		[Description("CRC32 of the general metadata file.")]
		public uint file_crc = file_crc;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
		[Units("")]
		[Description("MAVLink FTP URI for the general metadata file (COMP_METADATA_TYPE_GENERAL), which may be compressed with xz. The file contains general component metadata, and may contain URI links for additional metadata (see COMP_METADATA_TYPE). The information is static from boot, and may be generated at compile time. The string needs to be zero terminated.")]
		public byte[] uri = uri;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 254)]
	public struct mavlink_play_tune_v2_t(uint format, byte target_system, byte target_component, byte[] tune)
	{
		[Units("")]
		[Description("Tune format")]
		public uint format = format;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 248)]
		[Units("")]
		[Description("Tune definition as a NULL-terminated string.")]
		public byte[] tune = tune;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
	public struct mavlink_supported_tunes_t(uint format, byte target_system, byte target_component)
	{
		[Units("")]
		[Description("Bitfield of supported tune formats.")]
		public uint format = format;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 53)]
	public struct mavlink_event_t(uint id, uint event_time_boot_ms, ushort sequence, byte destination_component, byte destination_system, byte log_levels, byte[] arguments)
	{
		[Units("")]
		[Description("Event ID (as defined in the component metadata)")]
		public uint id = id;

		[Units("[ms]")]
		[Description("Timestamp (time since system boot when the event happened).")]
		public uint event_time_boot_ms = event_time_boot_ms;

		[Units("")]
		[Description("Sequence number.")]
		public ushort sequence = sequence;

		[Units("")]
		[Description("Component ID")]
		public byte destination_component = destination_component;

		[Units("")]
		[Description("System ID")]
		public byte destination_system = destination_system;

		[Units("")]
		[Description("Log levels: 4 bits MSB: internal (for logging purposes), 4 bits LSB: external. Levels: Emergency = 0, Alert = 1, Critical = 2, Error = 3, Warning = 4, Notice = 5, Info = 6, Debug = 7, Protocol = 8, Disabled = 9")]
		public byte log_levels = log_levels;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
		[Units("")]
		[Description("Arguments (depend on event ID).")]
		public byte[] arguments = arguments;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 3)]
	public struct mavlink_current_event_sequence_t(ushort sequence, byte flags)
	{
		[Units("")]
		[Description("Sequence number.")]
		public ushort sequence = sequence;

		[Units("")]
		[Description("Flag bitset.")]
		public byte flags = flags;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
	public struct mavlink_request_event_t(ushort first_sequence, ushort last_sequence, byte target_system, byte target_component)
	{
		[Units("")]
		[Description("First sequence number of the requested event.")]
		public ushort first_sequence = first_sequence;

		[Units("")]
		[Description("Last sequence number of the requested event.")]
		public ushort last_sequence = last_sequence;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 7)]
	public struct mavlink_response_event_error_t(ushort sequence, ushort sequence_oldest_available, byte target_system, byte target_component, byte reason)
	{
		[Units("")]
		[Description("Sequence number.")]
		public ushort sequence = sequence;

		[Units("")]
		[Description("Oldest Sequence number that is still available after the sequence set in REQUEST_EVENT.")]
		public ushort sequence_oldest_available = sequence_oldest_available;

		[Units("")]
		[Description("System ID")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID")]
		public byte target_component = target_component;

		[Units("")]
		[Description("Error reason.")]
		public byte reason = reason;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 46)]
	public struct mavlink_available_modes_t(uint custom_mode, uint properties, byte number_modes, byte mode_index, byte standard_mode, byte[] mode_name)
	{
		[Units("")]
		[Description("A bitfield for use for autopilot-specific flags")]
		public uint custom_mode = custom_mode;

		[Units("")]
		[Description("Mode properties.")]
		public uint properties = properties;

		[Units("")]
		[Description("The total number of available modes for the current vehicle type.")]
		public byte number_modes = number_modes;

		[Units("")]
		[Description("The current mode index within number_modes, indexed from 1. The index is not guaranteed to be persistent, and may change between reboots or if the set of modes change.")]
		public byte mode_index = mode_index;

		[Units("")]
		[Description("Standard mode.")]
		public byte standard_mode = standard_mode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 35)]
		[Units("")]
		[Description("Name of custom mode, with null termination character. Should be omitted for standard modes.")]
		public byte[] mode_name = mode_name;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 9)]
	public struct mavlink_current_mode_t(uint custom_mode, uint intended_custom_mode, byte standard_mode)
	{
		[Units("")]
		[Description("A bitfield for use for autopilot-specific flags")]
		public uint custom_mode = custom_mode;

		[Units("")]
		[Description("The custom_mode of the mode that was last commanded by the user (for example, with MAV_CMD_DO_SET_STANDARD_MODE, MAV_CMD_DO_SET_MODE or via RC). This should usually be the same as custom_mode. It will be different if the vehicle is unable to enter the intended mode, or has left that mode due to a failsafe condition. 0 indicates the intended custom mode is unknown/not supplied")]
		public uint intended_custom_mode = intended_custom_mode;

		[Units("")]
		[Description("Standard mode.")]
		public byte standard_mode = standard_mode;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 1)]
	public struct mavlink_available_modes_monitor_t(byte seq)
	{
		[Units("")]
		[Description("Sequence number. The value iterates sequentially whenever AVAILABLE_MODES changes (e.g. support for a new mode is added/removed dynamically).")]
		public byte seq = seq;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 35)]
	public struct mavlink_illuminator_status_t(uint uptime_ms, uint error_status, float brightness, float strobe_period, float strobe_duty_cycle, float temp_c, float min_strobe_period, float max_strobe_period, byte enable, byte mode_bitmask, byte mode)
	{
		[Units("[ms]")]
		[Description("Time since the start-up of the illuminator in ms")]
		public uint uptime_ms = uptime_ms;

		[Units("")]
		[Description("Errors")]
		public uint error_status = error_status;

		[Units("[%]")]
		[Description("Illuminator brightness")]
		public float brightness = brightness;

		[Units("[s]")]
		[Description("Illuminator strobing period in seconds")]
		public float strobe_period = strobe_period;

		[Units("[%]")]
		[Description("Illuminator strobing duty cycle")]
		public float strobe_duty_cycle = strobe_duty_cycle;

		[Units("")]
		[Description("Temperature in Celsius")]
		public float temp_c = temp_c;

		[Units("[s]")]
		[Description("Minimum strobing period in seconds")]
		public float min_strobe_period = min_strobe_period;

		[Units("[s]")]
		[Description("Maximum strobing period in seconds")]
		public float max_strobe_period = max_strobe_period;

		[Units("")]
		[Description("0: Illuminators OFF, 1: Illuminators ON")]
		public byte enable = enable;

		[Units("")]
		[Description("Supported illuminator modes")]
		public byte mode_bitmask = mode_bitmask;

		[Units("")]
		[Description("Illuminator mode")]
		public byte mode = mode;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 72)]
	public struct mavlink_canfd_frame_t(uint id, byte target_system, byte target_component, byte bus, byte len, byte[] data)
	{
		[Units("")]
		[Description("Frame ID")]
		public uint id = id;

		[Units("")]
		[Description("System ID.")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID.")]
		public byte target_component = target_component;

		[Units("")]
		[Description("bus number")]
		public byte bus = bus;

		[Units("")]
		[Description("Frame length")]
		public byte len = len;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		[Units("")]
		[Description("Frame data")]
		public byte[] data = data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 37)]
	public struct mavlink_can_filter_modify_t(ushort[] ids, byte target_system, byte target_component, byte bus, byte operation, byte num_ids)
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("")]
		[Description("filter IDs, length num_ids")]
		public ushort[] ids = ids;

		[Units("")]
		[Description("System ID.")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID.")]
		public byte target_component = target_component;

		[Units("")]
		[Description("bus number")]
		public byte bus = bus;

		[Units("")]
		[Description("what operation to perform on the filter list. See CAN_FILTER_OP enum.")]
		public byte operation = operation;

		[Units("")]
		[Description("number of IDs in filter list")]
		public byte num_ids = num_ids;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 137)]
	public struct mavlink_wheel_distance_t(ulong time_usec, double[] distance, byte count)
	{
		[Units("[us]")]
		[Description("Timestamp (synced to UNIX time or since system boot).")]
		public ulong time_usec = time_usec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[Units("[m]")]
		[Description("Distance reported by individual wheel encoders. Forward rotations increase values, reverse rotations decrease them. Not all wheels will necessarily have wheel encoders; the mapping of encoders to wheel positions must be agreed/understood by the endpoints.")]
		public double[] distance = distance;

		[Units("")]
		[Description("Number of wheels reported.")]
		public byte count = count;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 34)]
	public struct mavlink_winch_status_t(ulong time_usec, float line_length, float speed, float tension, float voltage, float current, uint status, short temperature)
	{
		[Units("[us]")]
		[Description("Timestamp (synced to UNIX time or since system boot).")]
		public ulong time_usec = time_usec;

		[Units("[m]")]
		[Description("Length of line released. NaN if unknown")]
		public float line_length = line_length;

		[Units("[m/s]")]
		[Description("Speed line is being released or retracted. Positive values if being released, negative values if being retracted, NaN if unknown")]
		public float speed = speed;

		[Units("[kg]")]
		[Description("Tension on the line. NaN if unknown")]
		public float tension = tension;

		[Units("[V]")]
		[Description("Voltage of the battery supplying the winch. NaN if unknown")]
		public float voltage = voltage;

		[Units("[A]")]
		[Description("Current draw from the winch. NaN if unknown")]
		public float current = current;

		[Units("")]
		[Description("Status flags")]
		public uint status = status;

		[Units("[degC]")]
		[Description("Temperature of the motor. INT16_MAX if unknown")]
		public short temperature = temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 44)]
	public struct mavlink_open_drone_id_basic_id_t(byte target_system, byte target_component, byte[] id_or_mac, byte id_type, byte ua_type, byte[] uas_id)
	{
		[Units("")]
		[Description("System ID (0 for broadcast).")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast).")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("Only used for drone ID data received from other UAs. See detailed description at https://mavlink.io/en/services/opendroneid.html. ")]
		public byte[] id_or_mac = id_or_mac;

		[Units("")]
		[Description("Indicates the format for the uas_id field of this message.")]
		public byte id_type = id_type;

		[Units("")]
		[Description("Indicates the type of UA (Unmanned Aircraft).")]
		public byte ua_type = ua_type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("UAS (Unmanned Aircraft System) ID following the format specified by id_type. Shall be filled with nulls in the unused portion of the field.")]
		public byte[] uas_id = uas_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 59)]
	public struct mavlink_open_drone_id_location_t(int latitude, int longitude, float altitude_barometric, float altitude_geodetic, float height, float timestamp, ushort direction, ushort speed_horizontal, short speed_vertical, byte target_system, byte target_component, byte[] id_or_mac, byte status, byte height_reference, byte horizontal_accuracy, byte vertical_accuracy, byte barometer_accuracy, byte speed_accuracy, byte timestamp_accuracy)
	{
		[Units("[degE7]")]
		[Description("Current latitude of the unmanned aircraft. If unknown: 0 (both Lat/Lon).")]
		public int latitude = latitude;

		[Units("[degE7]")]
		[Description("Current longitude of the unmanned aircraft. If unknown: 0 (both Lat/Lon).")]
		public int longitude = longitude;

		[Units("[m]")]
		[Description("The altitude calculated from the barometric pressure. Reference is against 29.92inHg or 1013.2mb. If unknown: -1000 m.")]
		public float altitude_barometric = altitude_barometric;

		[Units("[m]")]
		[Description("The geodetic altitude as defined by WGS84. If unknown: -1000 m.")]
		public float altitude_geodetic = altitude_geodetic;

		[Units("[m]")]
		[Description("The current height of the unmanned aircraft above the take-off location or the ground as indicated by height_reference. If unknown: -1000 m.")]
		public float height = height;

		[Units("[s]")]
		[Description("Seconds after the full hour with reference to UTC time. Typically the GPS outputs a time-of-week value in milliseconds. First convert that to UTC and then convert for this field using ((float) (time_week_ms % (60*60*1000))) / 1000. If unknown: 0xFFFF.")]
		public float timestamp = timestamp;

		[Units("[cdeg]")]
		[Description("Direction over ground (not heading, but direction of movement) measured clockwise from true North: 0 - 35999 centi-degrees. If unknown: 36100 centi-degrees.")]
		public ushort direction = direction;

		[Units("[cm/s]")]
		[Description("Ground speed. Positive only. If unknown: 25500 cm/s. If speed is larger than 25425 cm/s, use 25425 cm/s.")]
		public ushort speed_horizontal = speed_horizontal;

		[Units("[cm/s]")]
		[Description("The vertical speed. Up is positive. If unknown: 6300 cm/s. If speed is larger than 6200 cm/s, use 6200 cm/s. If lower than -6200 cm/s, use -6200 cm/s.")]
		public short speed_vertical = speed_vertical;

		[Units("")]
		[Description("System ID (0 for broadcast).")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast).")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("Only used for drone ID data received from other UAs. See detailed description at https://mavlink.io/en/services/opendroneid.html. ")]
		public byte[] id_or_mac = id_or_mac;

		[Units("")]
		[Description("Indicates whether the unmanned aircraft is on the ground or in the air.")]
		public byte status = status;

		[Units("")]
		[Description("Indicates the reference point for the height field.")]
		public byte height_reference = height_reference;

		[Units("")]
		[Description("The accuracy of the horizontal position.")]
		public byte horizontal_accuracy = horizontal_accuracy;

		[Units("")]
		[Description("The accuracy of the vertical position.")]
		public byte vertical_accuracy = vertical_accuracy;

		[Units("")]
		[Description("The accuracy of the barometric altitude.")]
		public byte barometer_accuracy = barometer_accuracy;

		[Units("")]
		[Description("The accuracy of the horizontal and vertical speed.")]
		public byte speed_accuracy = speed_accuracy;

		[Units("")]
		[Description("The accuracy of the timestamps.")]
		public byte timestamp_accuracy = timestamp_accuracy;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 53)]
	public struct mavlink_open_drone_id_authentication_t(uint timestamp, byte target_system, byte target_component, byte[] id_or_mac, byte authentication_type, byte data_page, byte last_page_index, byte length, byte[] authentication_data)
	{
		[Units("[s]")]
		[Description("This field is only present for page 0. 32 bit Unix Timestamp in seconds since 00:00:00 01/01/2019.")]
		public uint timestamp = timestamp;

		[Units("")]
		[Description("System ID (0 for broadcast).")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast).")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("Only used for drone ID data received from other UAs. See detailed description at https://mavlink.io/en/services/opendroneid.html. ")]
		public byte[] id_or_mac = id_or_mac;

		[Units("")]
		[Description("Indicates the type of authentication.")]
		public byte authentication_type = authentication_type;

		[Units("")]
		[Description("Allowed range is 0 - 15.")]
		public byte data_page = data_page;

		[Units("")]
		[Description("This field is only present for page 0. Allowed range is 0 - 15. See the description of struct ODID_Auth_data at https://github.com/opendroneid/opendroneid-core-c/blob/master/libopendroneid/opendroneid.h.")]
		public byte last_page_index = last_page_index;

		[Units("[bytes]")]
		[Description("This field is only present for page 0. Total bytes of authentication_data from all data pages. See the description of struct ODID_Auth_data at https://github.com/opendroneid/opendroneid-core-c/blob/master/libopendroneid/opendroneid.h.")]
		public byte length = length;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 23)]
		[Units("")]
		[Description("Opaque authentication data. For page 0, the size is only 17 bytes. For other pages, the size is 23 bytes. Shall be filled with nulls in the unused portion of the field.")]
		public byte[] authentication_data = authentication_data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 46)]
	public struct mavlink_open_drone_id_self_id_t(byte target_system, byte target_component, byte[] id_or_mac, byte description_type, byte[] description)
	{
		[Units("")]
		[Description("System ID (0 for broadcast).")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast).")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("Only used for drone ID data received from other UAs. See detailed description at https://mavlink.io/en/services/opendroneid.html. ")]
		public byte[] id_or_mac = id_or_mac;

		[Units("")]
		[Description("Indicates the type of the description field.")]
		public byte description_type = description_type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 23)]
		[Units("")]
		[Description("Text description or numeric value expressed as ASCII characters. Shall be filled with nulls in the unused portion of the field.")]
		public byte[] description = description;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 54)]
	public struct mavlink_open_drone_id_system_t(int operator_latitude, int operator_longitude, float area_ceiling, float area_floor, float operator_altitude_geo, uint timestamp, ushort area_count, ushort area_radius, byte target_system, byte target_component, byte[] id_or_mac, byte operator_location_type, byte classification_type, byte category_eu, byte class_eu)
	{
		[Units("[degE7]")]
		[Description("Latitude of the operator. If unknown: 0 (both Lat/Lon).")]
		public int operator_latitude = operator_latitude;

		[Units("[degE7]")]
		[Description("Longitude of the operator. If unknown: 0 (both Lat/Lon).")]
		public int operator_longitude = operator_longitude;

		[Units("[m]")]
		[Description("Area Operations Ceiling relative to WGS84. If unknown: -1000 m. Used only for swarms/multiple UA.")]
		public float area_ceiling = area_ceiling;

		[Units("[m]")]
		[Description("Area Operations Floor relative to WGS84. If unknown: -1000 m. Used only for swarms/multiple UA.")]
		public float area_floor = area_floor;

		[Units("[m]")]
		[Description("Geodetic altitude of the operator relative to WGS84. If unknown: -1000 m.")]
		public float operator_altitude_geo = operator_altitude_geo;

		[Units("[s]")]
		[Description("32 bit Unix Timestamp in seconds since 00:00:00 01/01/2019.")]
		public uint timestamp = timestamp;

		[Units("")]
		[Description("Number of aircraft in the area, group or formation (default 1). Used only for swarms/multiple UA.")]
		public ushort area_count = area_count;

		[Units("[m]")]
		[Description("Radius of the cylindrical area of the group or formation (default 0). Used only for swarms/multiple UA.")]
		public ushort area_radius = area_radius;

		[Units("")]
		[Description("System ID (0 for broadcast).")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast).")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("Only used for drone ID data received from other UAs. See detailed description at https://mavlink.io/en/services/opendroneid.html. ")]
		public byte[] id_or_mac = id_or_mac;

		[Units("")]
		[Description("Specifies the operator location type.")]
		public byte operator_location_type = operator_location_type;

		[Units("")]
		[Description("Specifies the classification type of the UA.")]
		public byte classification_type = classification_type;

		[Units("")]
		[Description("When classification_type is MAV_ODID_CLASSIFICATION_TYPE_EU, specifies the category of the UA.")]
		public byte category_eu = category_eu;

		[Units("")]
		[Description("When classification_type is MAV_ODID_CLASSIFICATION_TYPE_EU, specifies the class of the UA.")]
		public byte class_eu = class_eu;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 43)]
	public struct mavlink_open_drone_id_operator_id_t(byte target_system, byte target_component, byte[] id_or_mac, byte operator_id_type, byte[] operator_id)
	{
		[Units("")]
		[Description("System ID (0 for broadcast).")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast).")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("Only used for drone ID data received from other UAs. See detailed description at https://mavlink.io/en/services/opendroneid.html. ")]
		public byte[] id_or_mac = id_or_mac;

		[Units("")]
		[Description("Indicates the type of the operator_id field.")]
		public byte operator_id_type = operator_id_type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("Text description or numeric value expressed as ASCII characters. Shall be filled with nulls in the unused portion of the field.")]
		public byte[] operator_id = operator_id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 249)]
	public struct mavlink_open_drone_id_message_pack_t(byte target_system, byte target_component, byte[] id_or_mac, byte single_message_size, byte msg_pack_size, byte[] messages)
	{
		[Units("")]
		[Description("System ID (0 for broadcast).")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast).")]
		public byte target_component = target_component;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		[Units("")]
		[Description("Only used for drone ID data received from other UAs. See detailed description at https://mavlink.io/en/services/opendroneid.html. ")]
		public byte[] id_or_mac = id_or_mac;

		[Units("[bytes]")]
		[Description("This field must currently always be equal to 25 (bytes), since all encoded OpenDroneID messages are specified to have this length.")]
		public byte single_message_size = single_message_size;

		[Units("")]
		[Description("Number of encoded messages in the pack (not the number of bytes). Allowed range is 1 - 9.")]
		public byte msg_pack_size = msg_pack_size;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 225)]
		[Units("")]
		[Description("Concatenation of encoded OpenDroneID messages. Shall be filled with nulls in the unused portion of the field.")]
		public byte[] messages = messages;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 51)]
	public struct mavlink_open_drone_id_arm_status_t(byte status, byte[] error)
	{
		[Units("")]
		[Description("Status level indicating if arming is allowed.")]
		public byte status = status;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
		[Units("")]
		[Description("Text error message, should be empty if status is good to arm. Fill with nulls in unused portion.")]
		public byte[] error = error;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 18)]
	public struct mavlink_open_drone_id_system_update_t(int operator_latitude, int operator_longitude, float operator_altitude_geo, uint timestamp, byte target_system, byte target_component)
	{
		[Units("[degE7]")]
		[Description("Latitude of the operator. If unknown: 0 (both Lat/Lon).")]
		public int operator_latitude = operator_latitude;

		[Units("[degE7]")]
		[Description("Longitude of the operator. If unknown: 0 (both Lat/Lon).")]
		public int operator_longitude = operator_longitude;

		[Units("[m]")]
		[Description("Geodetic altitude of the operator relative to WGS84. If unknown: -1000 m.")]
		public float operator_altitude_geo = operator_altitude_geo;

		[Units("[s]")]
		[Description("32 bit Unix Timestamp in seconds since 00:00:00 01/01/2019.")]
		public uint timestamp = timestamp;

		[Units("")]
		[Description("System ID (0 for broadcast).")]
		public byte target_system = target_system;

		[Units("")]
		[Description("Component ID (0 for broadcast).")]
		public byte target_component = target_component;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 5)]
	public struct mavlink_hygrometer_sensor_t(short temperature, ushort humidity, byte id)
	{
		[Units("[cdegC]")]
		[Description("Temperature")]
		public short temperature = temperature;

		[Units("[c%]")]
		[Description("Humidity")]
		public ushort humidity = humidity;

		[Units("")]
		[Description("Hygrometer ID")]
		public byte id = id;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 78)]
	public struct mavlink_autopilot_version_t(ulong capabilities, ulong uid, uint flight_sw_version, uint middleware_sw_version, uint os_sw_version, uint board_version, ushort vendor_id, ushort product_id, byte[] flight_custom_version, byte[] middleware_custom_version, byte[] os_custom_version, byte[] uid2)
	{
		[Units("")]
		[Description("Bitmap of capabilities")]
		public ulong capabilities = capabilities;

		[Units("")]
		[Description("UID if provided by hardware (see uid2)")]
		public ulong uid = uid;

		[Units("")]
		[Description("Firmware version number.         The field must be encoded as 4 bytes, where each byte (shown from MSB to LSB) is part of a semantic version: (major) (minor) (patch) (FIRMWARE_VERSION_TYPE).       ")]
		public uint flight_sw_version = flight_sw_version;

		[Units("")]
		[Description("Middleware version number")]
		public uint middleware_sw_version = middleware_sw_version;

		[Units("")]
		[Description("Operating system version number")]
		public uint os_sw_version = os_sw_version;

		[Units("")]
		[Description("HW / board version (last 8 bits should be silicon ID, if any). The first 16 bits of this field specify a board type from an enumeration stored at https://github.com/PX4/PX4-Bootloader/blob/master/board_types.txt and with extensive additions at https://github.com/ArduPilot/ardupilot/blob/master/Tools/AP_Bootloader/board_types.txt")]
		public uint board_version = board_version;

		[Units("")]
		[Description("ID of the board vendor")]
		public ushort vendor_id = vendor_id;

		[Units("")]
		[Description("ID of the product")]
		public ushort product_id = product_id;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("")]
		[Description("Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.")]
		public byte[] flight_custom_version = flight_custom_version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("")]
		[Description("Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.")]
		public byte[] middleware_custom_version = middleware_custom_version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("")]
		[Description("Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases.")]
		public byte[] os_custom_version = os_custom_version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
		[Units("")]
		[Description("UID if provided by hardware (supersedes the uid field. If this is non-zero, use this field, otherwise use uid)")]
		public byte[] uid2 = uid2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 9)]
	public struct mavlink_heartbeat_t(uint custom_mode, byte type, byte autopilot, byte base_mode, byte system_status, byte mavlink_version)
	{
		[Units("")]
		[Description("A bitfield for use for autopilot-specific flags")]
		public uint custom_mode = custom_mode;

		[Units("")]
		[Description("Vehicle or component type. For a flight controller component the vehicle type (quadrotor, helicopter, etc.). For other components the component type (e.g. camera, gimbal, etc.). This should be used in preference to component id for identifying the component type.")]
		public byte type = type;

		[Units("")]
		[Description("Autopilot type / class. Use MAV_AUTOPILOT_INVALID for components that are not flight controllers.")]
		public byte autopilot = autopilot;

		[Units("")]
		[Description("System mode bitmap.")]
		public byte base_mode = base_mode;

		[Units("")]
		[Description("System status flag.")]
		public byte system_status = system_status;

		[Units("")]
		[Description("MAVLink version, not writable by user, gets added by protocol because of magic data type: uint8_t_mavlink_version")]
		public byte mavlink_version = mavlink_version;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 22)]
	public struct mavlink_protocol_version_t(ushort version, ushort min_version, ushort max_version, byte[] spec_version_hash, byte[] library_version_hash)
	{
		[Units("")]
		[Description("Currently active MAVLink version number * 100: v1.0 is 100, v2.0 is 200, etc.")]
		public ushort version = version;

		[Units("")]
		[Description("Minimum MAVLink version supported")]
		public ushort min_version = min_version;

		[Units("")]
		[Description("Maximum MAVLink version supported (set to the same value as version by default)")]
		public ushort max_version = max_version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("")]
		[Description("The first 8 bytes (not characters printed in hex!) of the git hash.")]
		public byte[] spec_version_hash = spec_version_hash;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[Units("")]
		[Description("The first 8 bytes (not characters printed in hex!) of the git hash.")]
		public byte[] library_version_hash = library_version_hash;
	}

	public class MavlinkCRC
	{
		private const int X25_INIT_CRC = 65535;

		private const int X25_VALIDATE_CRC = 61624;

		public static ushort crc_accumulate(byte b, ushort crc)
		{
			byte b2 = (byte)(b ^ (byte)(crc & 0xFF));
			b2 = (byte)(b2 ^ (b2 << 4));
			return (ushort)((crc >> 8) ^ (b2 << 8) ^ (b2 << 3) ^ (b2 >> 4));
		}

		public static ushort crc_calculate(byte[] pBuffer, int length)
		{
			if (length < 1)
			{
				return ushort.MaxValue;
			}
			ushort num = ushort.MaxValue;
			for (int i = 1; i < length; i++)
			{
				num = crc_accumulate(pBuffer[i], num);
			}
			return num;
		}
	}

	public class MAVLinkMessage
	{
		public static readonly MAVLinkMessage Invalid = new MAVLinkMessage();

		private object _locker = new object();

		private byte[] _buffer;

		private object _data;

		public byte[] buffer
		{
			get
			{
				return _buffer;
			}
			set
			{
				_buffer = value;
				processBuffer(_buffer);
			}
		}

		public DateTime rxtime { get; set; }

		public byte header { get; internal set; }

		public byte payloadlength { get; internal set; }

		public byte incompat_flags { get; internal set; }

		public byte compat_flags { get; internal set; }

		public byte seq { get; internal set; }

		public byte sysid { get; internal set; }

		public byte compid { get; internal set; }

		public uint msgid { get; internal set; }

		public bool ismavlink2
		{
			get
			{
				if (buffer != null && buffer.Length != 0)
				{
					return buffer[0] == 253;
				}
				return false;
			}
		}

		public string msgtypename => MAVLINK_MESSAGE_INFOS.GetMessageInfo(msgid).name;

		public object data
		{
			get
			{
				lock (_locker)
				{
					if (_data != null)
					{
						return _data;
					}
					message_info messageInfo = MAVLINK_MESSAGE_INFOS.GetMessageInfo(msgid);
					if (messageInfo.type == null)
					{
						return null;
					}
					_data = Activator.CreateInstance(messageInfo.type);
					try
					{
						if (payloadlength == 0)
						{
							return _data;
						}
						if (ismavlink2)
						{
							MavlinkUtil.ByteArrayToStructure(buffer, ref _data, 10, payloadlength);
						}
						else
						{
							MavlinkUtil.ByteArrayToStructure(buffer, ref _data, 6, payloadlength);
						}
					}
					catch (Exception)
					{
					}
				}
				return _data;
			}
		}

		public ushort crc16 { get; internal set; }

		public byte[] sig { get; internal set; }

		public byte sigLinkid
		{
			get
			{
				if (sig != null)
				{
					return sig[0];
				}
				return 0;
			}
		}

		public ulong sigTimestamp
		{
			get
			{
				if (sig != null)
				{
					byte[] array = new byte[8];
					Array.Copy(sig, 1, array, 0, 6);
					return BitConverter.ToUInt64(array, 0);
				}
				return 0uL;
			}
		}

		public int Length
		{
			get
			{
				if (buffer == null)
				{
					return 0;
				}
				return buffer.Length;
			}
		}

		public T ToStructure<T>()
		{
			return (T)data;
		}

		public MAVLinkMessage()
		{
			rxtime = DateTime.MinValue;
		}

		public MAVLinkMessage(byte[] buffer)
			: this(buffer, DateTime.UtcNow)
		{
		}

		public MAVLinkMessage(byte[] buffer, DateTime rxTime)
		{
			this.buffer = buffer;
			rxtime = rxTime;
			processBuffer(buffer);
		}

		internal void processBuffer(byte[] buffer)
		{
			_data = null;
			if (buffer[0] == 253)
			{
				if (buffer.Length >= 10)
				{
					header = buffer[0];
					payloadlength = buffer[1];
					incompat_flags = buffer[2];
					compat_flags = buffer[3];
					seq = buffer[4];
					sysid = buffer[5];
					compid = buffer[6];
					msgid = (uint)((buffer[9] << 16) + (buffer[8] << 8) + buffer[7]);
					int num = 9 + payloadlength + 1;
					int num2 = 9 + payloadlength + 2;
					crc16 = (ushort)((buffer[num2] << 8) + buffer[num]);
					if ((incompat_flags & 1) > 0)
					{
						sig = new byte[13];
						Array.ConstrainedCopy(buffer, buffer.Length - 13, sig, 0, 13);
					}
				}
			}
			else if (buffer.Length >= 6)
			{
				header = buffer[0];
				payloadlength = buffer[1];
				seq = buffer[2];
				sysid = buffer[3];
				compid = buffer[4];
				msgid = buffer[5];
				int num3 = 5 + payloadlength + 1;
				int num4 = 5 + payloadlength + 2;
				crc16 = (ushort)((buffer[num4] << 8) + buffer[num3]);
			}
		}

		public override string ToString()
		{
			return string.Format("{5},{4},{0},{1},{2},{3}", new object[6] { sysid, compid, msgid, msgtypename, ismavlink2, rxtime });
		}
	}

	public class Units : Attribute
	{
		public string Unit { get; set; }

		public Units(string unit)
		{
			Unit = unit;
		}
	}

	public class Description : Attribute
	{
		public string Text { get; set; }

		public Description(string desc)
		{
			Text = desc;
		}
	}

	public class MavlinkParse
	{
		public int packetcount = 0;

		public int badCRC = 0;

		public int badLength = 0;

		public bool hasTimestamp = false;

		public byte sendlinkid { get; set; }

		public ulong lasttimestamp { get; set; }

		public byte[] signingKey { get; set; }

		public MavlinkParse(bool hasTimestamp = false)
		{
			this.hasTimestamp = hasTimestamp;
		}

		public static void ReadWithTimeout(Stream BaseStream, byte[] buffer, int offset, int count)
		{
			int num = 500;
			if (BaseStream.CanSeek)
			{
				num = 0;
				if (BaseStream.Position + count > BaseStream.Length)
				{
					throw new EndOfStreamException("End of data");
				}
			}
			if (BaseStream.CanTimeout)
			{
				num = BaseStream.ReadTimeout;
				if (num == -1)
				{
					num = 60000;
				}
			}
			DateTime dateTime = DateTime.Now.AddMilliseconds(num);
			int num2 = count;
			int num3 = offset;
			while (true)
			{
				int num4 = BaseStream.Read(buffer, num3, num2);
				num2 -= num4;
				num3 += num4;
				if (num4 > 0)
				{
					dateTime = DateTime.Now.AddMilliseconds(num);
				}
				if (num2 == 0)
				{
					return;
				}
				if (DateTime.Now > dateTime)
				{
					break;
				}
				Thread.Sleep(1);
			}
			throw new TimeoutException("Timeout waiting for data");
		}

		public MAVLinkMessage ReadPacket(Stream BaseStream)
		{
			byte[] array = new byte[280];
			DateTime rxTime = DateTime.MinValue;
			if (hasTimestamp)
			{
				byte[] array2 = new byte[8];
				int num = BaseStream.Read(array2, 0, array2.Length);
				Array.Reverse((Array)array2);
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				ulong num2 = BitConverter.ToUInt64(array2, 0);
				if (num2 / 1000 / 1000 / 60 / 60 < 9999999)
				{
					rxTime = dateTime.AddMilliseconds(num2 / 1000).ToLocalTime();
				}
			}
			int i;
			for (i = 0; i <= 280; i++)
			{
				ReadWithTimeout(BaseStream, array, 0, 1);
				if (array[0] == 253 || array[0] == 254)
				{
					break;
				}
			}
			if (i >= 280)
			{
				return null;
			}
			byte b = (byte)((array[0] == 253) ? 9 : 5);
			int num3 = b + 1;
			try
			{
				ReadWithTimeout(BaseStream, array, 1, b);
			}
			catch (EndOfStreamException)
			{
				return null;
			}
			int num4 = 0;
			if (array[0] == 253)
			{
				num4 = array[1] + num3 + 2 - 2;
				if ((array[2] & 1) > 0)
				{
					num4 += 13;
				}
			}
			else
			{
				num4 = array[1] + num3 + 2 - 2;
			}
			try
			{
				ReadWithTimeout(BaseStream, array, num3, num4 - (num3 - 2));
			}
			catch (EndOfStreamException)
			{
				return null;
			}
			Array.Resize(ref array, num4 + 2);
			MAVLinkMessage mAVLinkMessage = new MAVLinkMessage(array, rxTime);
			ushort num5 = MavlinkCRC.crc_calculate(array, array.Length - 2);
			if (mAVLinkMessage.header == 253 || mAVLinkMessage.header == 254)
			{
				num5 = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_INFOS.GetMessageInfo(mAVLinkMessage.msgid).crc, num5);
			}
			if (mAVLinkMessage.crc16 >> 8 != num5 >> 8 || (mAVLinkMessage.crc16 & 0xFF) != (num5 & 0xFF))
			{
				badCRC++;
				return null;
			}
			return mAVLinkMessage;
		}

		public byte[] GenerateMAVLinkPacket10(MAVLINK_MSG_ID messageType, object indata, byte sysid = byte.MaxValue, byte compid = 190, int sequence = -1)
		{
			byte[] array = MavlinkUtil.StructureToByteArray(indata);
			byte[] array2 = new byte[array.Length + 6 + 2];
			array2[0] = 254;
			array2[1] = (byte)array.Length;
			array2[2] = (byte)packetcount;
			if (sequence != -1)
			{
				array2[2] = (byte)sequence;
			}
			packetcount++;
			array2[3] = sysid;
			array2[4] = compid;
			array2[5] = (byte)messageType;
			int num = 6;
			byte[] array3 = array;
			foreach (byte b in array3)
			{
				array2[num] = b;
				num++;
			}
			ushort crc = MavlinkCRC.crc_calculate(array2, array2[1] + 6);
			crc = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_INFOS.GetMessageInfo((uint)messageType).crc, crc);
			byte b2 = (byte)(crc & 0xFF);
			byte b3 = (byte)(crc >> 8);
			array2[num] = b2;
			num++;
			array2[num] = b3;
			num++;
			return array2;
		}

		public byte[] GenerateMAVLinkPacket20(MAVLINK_MSG_ID messageType, object indata, bool sign = false, byte sysid = byte.MaxValue, byte compid = 190, int sequence = -1)
		{
			byte[] payload = MavlinkUtil.StructureToByteArray(indata);
			MavlinkUtil.trim_payload(ref payload);
			int num = 0;
			if (sign)
			{
				num = 13;
			}
			byte[] array = new byte[payload.Length + 12 + num];
			array[0] = 253;
			array[1] = (byte)payload.Length;
			array[2] = 0;
			if (sign)
			{
				array[2] |= 1;
			}
			array[3] = 0;
			array[4] = (byte)packetcount;
			if (sequence != -1)
			{
				array[4] = (byte)sequence;
			}
			packetcount++;
			array[5] = sysid;
			array[6] = compid;
			array[7] = (byte)messageType;
			array[8] = (byte)((uint)messageType >> 8);
			array[9] = (byte)((uint)messageType >> 16);
			int num2 = 10;
			byte[] array2 = payload;
			foreach (byte b in array2)
			{
				array[num2] = b;
				num2++;
			}
			ushort crc = MavlinkCRC.crc_calculate(array, payload.Length + 10);
			crc = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_INFOS.GetMessageInfo((uint)messageType).crc, crc);
			byte b2 = (byte)(crc & 0xFF);
			byte b3 = (byte)(crc >> 8);
			array[num2] = b2;
			num2++;
			array[num2] = b3;
			num2++;
			if (sign)
			{
				ulong num3 = (ulong)((DateTime.UtcNow - new DateTime(2015, 1, 1)).TotalMilliseconds * 100.0);
				if (num3 == lasttimestamp)
				{
					num3++;
				}
				lasttimestamp = num3;
				byte[] bytes = BitConverter.GetBytes(num3);
				byte[] array3 = new byte[7] { sendlinkid, 0, 0, 0, 0, 0, 0 };
				Array.Copy(bytes, 0, array3, 1, 6);
				if (signingKey == null || signingKey.Length != 32)
				{
					signingKey = new byte[32];
				}
				using SHA256CryptoServiceProvider sHA256CryptoServiceProvider = new SHA256CryptoServiceProvider();
				MemoryStream memoryStream = new MemoryStream();
				memoryStream.Write(signingKey, 0, signingKey.Length);
				memoryStream.Write(array, 0, num2);
				memoryStream.Write(array3, 0, array3.Length);
				byte[] array4 = sHA256CryptoServiceProvider.ComputeHash(memoryStream.GetBuffer());
				Array.Resize(ref array4, 6);
				byte[] array5 = array3;
				foreach (byte b4 in array5)
				{
					array[num2] = b4;
					num2++;
				}
				byte[] array6 = array4;
				foreach (byte b5 in array6)
				{
					array[num2] = b5;
					num2++;
				}
			}
			return array;
		}
	}

	public const string MAVLINK_BUILD_DATE = "Thu Aug 07 2025";

	public const string MAVLINK_WIRE_PROTOCOL_VERSION = "2.0";

	public const int MAVLINK_MAX_PAYLOAD_LEN = 255;

	public const byte MAVLINK_CORE_HEADER_LEN = 9;

	public const byte MAVLINK_CORE_HEADER_MAVLINK1_LEN = 5;

	public const byte MAVLINK_NUM_HEADER_BYTES = 10;

	public const byte MAVLINK_NUM_CHECKSUM_BYTES = 2;

	public const byte MAVLINK_NUM_NON_PAYLOAD_BYTES = 12;

	public const int MAVLINK_MAX_PACKET_LEN = 280;

	public const byte MAVLINK_SIGNATURE_BLOCK_LEN = 13;

	public const int MAVLINK_LITTLE_ENDIAN = 1;

	public const int MAVLINK_BIG_ENDIAN = 0;

	public const byte MAVLINK_STX = 253;

	public const byte MAVLINK_STX_MAVLINK1 = 254;

	public const byte MAVLINK_ENDIAN = 1;

	public const bool MAVLINK_ALIGNED_FIELDS = true;

	public const byte MAVLINK_CRC_EXTRA = 1;

	public const byte MAVLINK_COMMAND_24BIT = 1;

	public const bool MAVLINK_NEED_BYTE_SWAP = true;

	public static message_info[] MAVLINK_MESSAGE_INFOS = new message_info[229]
	{
		new message_info(0u, "HEARTBEAT", 50, 9u, 9u, typeof(mavlink_heartbeat_t)),
		new message_info(1u, "SYS_STATUS", 124, 31u, 43u, typeof(mavlink_sys_status_t)),
		new message_info(2u, "SYSTEM_TIME", 137, 12u, 12u, typeof(mavlink_system_time_t)),
		new message_info(4u, "PING", 237, 14u, 14u, typeof(mavlink_ping_t)),
		new message_info(5u, "CHANGE_OPERATOR_CONTROL", 217, 28u, 28u, typeof(mavlink_change_operator_control_t)),
		new message_info(6u, "CHANGE_OPERATOR_CONTROL_ACK", 104, 3u, 3u, typeof(mavlink_change_operator_control_ack_t)),
		new message_info(7u, "AUTH_KEY", 119, 32u, 32u, typeof(mavlink_auth_key_t)),
		new message_info(8u, "LINK_NODE_STATUS", 117, 36u, 36u, typeof(mavlink_link_node_status_t)),
		new message_info(11u, "SET_MODE", 89, 6u, 6u, typeof(mavlink_set_mode_t)),
		new message_info(20u, "PARAM_REQUEST_READ", 214, 20u, 20u, typeof(mavlink_param_request_read_t)),
		new message_info(21u, "PARAM_REQUEST_LIST", 159, 2u, 2u, typeof(mavlink_param_request_list_t)),
		new message_info(22u, "PARAM_VALUE", 220, 25u, 25u, typeof(mavlink_param_value_t)),
		new message_info(23u, "PARAM_SET", 168, 23u, 23u, typeof(mavlink_param_set_t)),
		new message_info(24u, "GPS_RAW_INT", 24, 30u, 52u, typeof(mavlink_gps_raw_int_t)),
		new message_info(25u, "GPS_STATUS", 23, 101u, 101u, typeof(mavlink_gps_status_t)),
		new message_info(26u, "SCALED_IMU", 170, 22u, 24u, typeof(mavlink_scaled_imu_t)),
		new message_info(27u, "RAW_IMU", 144, 26u, 29u, typeof(mavlink_raw_imu_t)),
		new message_info(28u, "RAW_PRESSURE", 67, 16u, 16u, typeof(mavlink_raw_pressure_t)),
		new message_info(29u, "SCALED_PRESSURE", 115, 14u, 16u, typeof(mavlink_scaled_pressure_t)),
		new message_info(30u, "ATTITUDE", 39, 28u, 28u, typeof(mavlink_attitude_t)),
		new message_info(31u, "ATTITUDE_QUATERNION", 246, 32u, 48u, typeof(mavlink_attitude_quaternion_t)),
		new message_info(32u, "LOCAL_POSITION_NED", 185, 28u, 28u, typeof(mavlink_local_position_ned_t)),
		new message_info(33u, "GLOBAL_POSITION_INT", 104, 28u, 28u, typeof(mavlink_global_position_int_t)),
		new message_info(34u, "RC_CHANNELS_SCALED", 237, 22u, 22u, typeof(mavlink_rc_channels_scaled_t)),
		new message_info(35u, "RC_CHANNELS_RAW", 244, 22u, 22u, typeof(mavlink_rc_channels_raw_t)),
		new message_info(36u, "SERVO_OUTPUT_RAW", 222, 21u, 37u, typeof(mavlink_servo_output_raw_t)),
		new message_info(37u, "MISSION_REQUEST_PARTIAL_LIST", 212, 6u, 7u, typeof(mavlink_mission_request_partial_list_t)),
		new message_info(38u, "MISSION_WRITE_PARTIAL_LIST", 9, 6u, 7u, typeof(mavlink_mission_write_partial_list_t)),
		new message_info(39u, "MISSION_ITEM", 254, 37u, 38u, typeof(mavlink_mission_item_t)),
		new message_info(40u, "MISSION_REQUEST", 230, 4u, 5u, typeof(mavlink_mission_request_t)),
		new message_info(41u, "MISSION_SET_CURRENT", 28, 4u, 4u, typeof(mavlink_mission_set_current_t)),
		new message_info(42u, "MISSION_CURRENT", 28, 2u, 18u, typeof(mavlink_mission_current_t)),
		new message_info(43u, "MISSION_REQUEST_LIST", 132, 2u, 3u, typeof(mavlink_mission_request_list_t)),
		new message_info(44u, "MISSION_COUNT", 221, 4u, 9u, typeof(mavlink_mission_count_t)),
		new message_info(45u, "MISSION_CLEAR_ALL", 232, 2u, 3u, typeof(mavlink_mission_clear_all_t)),
		new message_info(46u, "MISSION_ITEM_REACHED", 11, 2u, 2u, typeof(mavlink_mission_item_reached_t)),
		new message_info(47u, "MISSION_ACK", 153, 3u, 8u, typeof(mavlink_mission_ack_t)),
		new message_info(48u, "SET_GPS_GLOBAL_ORIGIN", 41, 13u, 21u, typeof(mavlink_set_gps_global_origin_t)),
		new message_info(49u, "GPS_GLOBAL_ORIGIN", 39, 12u, 20u, typeof(mavlink_gps_global_origin_t)),
		new message_info(50u, "PARAM_MAP_RC", 78, 37u, 37u, typeof(mavlink_param_map_rc_t)),
		new message_info(51u, "MISSION_REQUEST_INT", 196, 4u, 5u, typeof(mavlink_mission_request_int_t)),
		new message_info(54u, "SAFETY_SET_ALLOWED_AREA", 15, 27u, 27u, typeof(mavlink_safety_set_allowed_area_t)),
		new message_info(55u, "SAFETY_ALLOWED_AREA", 3, 25u, 25u, typeof(mavlink_safety_allowed_area_t)),
		new message_info(61u, "ATTITUDE_QUATERNION_COV", 167, 72u, 72u, typeof(mavlink_attitude_quaternion_cov_t)),
		new message_info(62u, "NAV_CONTROLLER_OUTPUT", 183, 26u, 26u, typeof(mavlink_nav_controller_output_t)),
		new message_info(63u, "GLOBAL_POSITION_INT_COV", 119, 181u, 181u, typeof(mavlink_global_position_int_cov_t)),
		new message_info(64u, "LOCAL_POSITION_NED_COV", 191, 225u, 225u, typeof(mavlink_local_position_ned_cov_t)),
		new message_info(65u, "RC_CHANNELS", 118, 42u, 42u, typeof(mavlink_rc_channels_t)),
		new message_info(66u, "REQUEST_DATA_STREAM", 148, 6u, 6u, typeof(mavlink_request_data_stream_t)),
		new message_info(67u, "DATA_STREAM", 21, 4u, 4u, typeof(mavlink_data_stream_t)),
		new message_info(69u, "MANUAL_CONTROL", 243, 11u, 30u, typeof(mavlink_manual_control_t)),
		new message_info(70u, "RC_CHANNELS_OVERRIDE", 124, 18u, 38u, typeof(mavlink_rc_channels_override_t)),
		new message_info(73u, "MISSION_ITEM_INT", 38, 37u, 38u, typeof(mavlink_mission_item_int_t)),
		new message_info(74u, "VFR_HUD", 20, 20u, 20u, typeof(mavlink_vfr_hud_t)),
		new message_info(75u, "COMMAND_INT", 158, 35u, 35u, typeof(mavlink_command_int_t)),
		new message_info(76u, "COMMAND_LONG", 152, 33u, 33u, typeof(mavlink_command_long_t)),
		new message_info(77u, "COMMAND_ACK", 143, 3u, 10u, typeof(mavlink_command_ack_t)),
		new message_info(80u, "COMMAND_CANCEL", 14, 4u, 4u, typeof(mavlink_command_cancel_t)),
		new message_info(81u, "MANUAL_SETPOINT", 106, 22u, 22u, typeof(mavlink_manual_setpoint_t)),
		new message_info(82u, "SET_ATTITUDE_TARGET", 49, 39u, 51u, typeof(mavlink_set_attitude_target_t)),
		new message_info(83u, "ATTITUDE_TARGET", 22, 37u, 37u, typeof(mavlink_attitude_target_t)),
		new message_info(84u, "SET_POSITION_TARGET_LOCAL_NED", 143, 53u, 53u, typeof(mavlink_set_position_target_local_ned_t)),
		new message_info(85u, "POSITION_TARGET_LOCAL_NED", 140, 51u, 51u, typeof(mavlink_position_target_local_ned_t)),
		new message_info(86u, "SET_POSITION_TARGET_GLOBAL_INT", 5, 53u, 53u, typeof(mavlink_set_position_target_global_int_t)),
		new message_info(87u, "POSITION_TARGET_GLOBAL_INT", 150, 51u, 51u, typeof(mavlink_position_target_global_int_t)),
		new message_info(89u, "LOCAL_POSITION_NED_SYSTEM_GLOBAL_OFFSET", 231, 28u, 28u, typeof(mavlink_local_position_ned_system_global_offset_t)),
		new message_info(90u, "HIL_STATE", 183, 56u, 56u, typeof(mavlink_hil_state_t)),
		new message_info(91u, "HIL_CONTROLS", 63, 42u, 42u, typeof(mavlink_hil_controls_t)),
		new message_info(92u, "HIL_RC_INPUTS_RAW", 54, 33u, 33u, typeof(mavlink_hil_rc_inputs_raw_t)),
		new message_info(93u, "HIL_ACTUATOR_CONTROLS", 47, 81u, 81u, typeof(mavlink_hil_actuator_controls_t)),
		new message_info(100u, "OPTICAL_FLOW", 175, 26u, 34u, typeof(mavlink_optical_flow_t)),
		new message_info(101u, "GLOBAL_VISION_POSITION_ESTIMATE", 102, 32u, 117u, typeof(mavlink_global_vision_position_estimate_t)),
		new message_info(102u, "VISION_POSITION_ESTIMATE", 158, 32u, 117u, typeof(mavlink_vision_position_estimate_t)),
		new message_info(103u, "VISION_SPEED_ESTIMATE", 208, 20u, 57u, typeof(mavlink_vision_speed_estimate_t)),
		new message_info(104u, "VICON_POSITION_ESTIMATE", 56, 32u, 116u, typeof(mavlink_vicon_position_estimate_t)),
		new message_info(105u, "HIGHRES_IMU", 93, 62u, 63u, typeof(mavlink_highres_imu_t)),
		new message_info(106u, "OPTICAL_FLOW_RAD", 138, 44u, 44u, typeof(mavlink_optical_flow_rad_t)),
		new message_info(107u, "HIL_SENSOR", 108, 64u, 65u, typeof(mavlink_hil_sensor_t)),
		new message_info(108u, "SIM_STATE", 32, 84u, 92u, typeof(mavlink_sim_state_t)),
		new message_info(109u, "RADIO_STATUS", 185, 9u, 9u, typeof(mavlink_radio_status_t)),
		new message_info(110u, "FILE_TRANSFER_PROTOCOL", 84, 254u, 254u, typeof(mavlink_file_transfer_protocol_t)),
		new message_info(111u, "TIMESYNC", 34, 16u, 18u, typeof(mavlink_timesync_t)),
		new message_info(112u, "CAMERA_TRIGGER", 174, 12u, 12u, typeof(mavlink_camera_trigger_t)),
		new message_info(113u, "HIL_GPS", 124, 36u, 39u, typeof(mavlink_hil_gps_t)),
		new message_info(114u, "HIL_OPTICAL_FLOW", 237, 44u, 44u, typeof(mavlink_hil_optical_flow_t)),
		new message_info(115u, "HIL_STATE_QUATERNION", 4, 64u, 64u, typeof(mavlink_hil_state_quaternion_t)),
		new message_info(116u, "SCALED_IMU2", 76, 22u, 24u, typeof(mavlink_scaled_imu2_t)),
		new message_info(117u, "LOG_REQUEST_LIST", 128, 6u, 6u, typeof(mavlink_log_request_list_t)),
		new message_info(118u, "LOG_ENTRY", 56, 14u, 14u, typeof(mavlink_log_entry_t)),
		new message_info(119u, "LOG_REQUEST_DATA", 116, 12u, 12u, typeof(mavlink_log_request_data_t)),
		new message_info(120u, "LOG_DATA", 134, 97u, 97u, typeof(mavlink_log_data_t)),
		new message_info(121u, "LOG_ERASE", 237, 2u, 2u, typeof(mavlink_log_erase_t)),
		new message_info(122u, "LOG_REQUEST_END", 203, 2u, 2u, typeof(mavlink_log_request_end_t)),
		new message_info(123u, "GPS_INJECT_DATA", 250, 113u, 113u, typeof(mavlink_gps_inject_data_t)),
		new message_info(124u, "GPS2_RAW", 87, 35u, 57u, typeof(mavlink_gps2_raw_t)),
		new message_info(125u, "POWER_STATUS", 203, 6u, 6u, typeof(mavlink_power_status_t)),
		new message_info(126u, "SERIAL_CONTROL", 220, 79u, 81u, typeof(mavlink_serial_control_t)),
		new message_info(127u, "GPS_RTK", 25, 35u, 35u, typeof(mavlink_gps_rtk_t)),
		new message_info(128u, "GPS2_RTK", 226, 35u, 35u, typeof(mavlink_gps2_rtk_t)),
		new message_info(129u, "SCALED_IMU3", 46, 22u, 24u, typeof(mavlink_scaled_imu3_t)),
		new message_info(130u, "DATA_TRANSMISSION_HANDSHAKE", 29, 13u, 13u, typeof(mavlink_data_transmission_handshake_t)),
		new message_info(131u, "ENCAPSULATED_DATA", 223, 255u, 255u, typeof(mavlink_encapsulated_data_t)),
		new message_info(132u, "DISTANCE_SENSOR", 85, 14u, 39u, typeof(mavlink_distance_sensor_t)),
		new message_info(133u, "TERRAIN_REQUEST", 6, 18u, 18u, typeof(mavlink_terrain_request_t)),
		new message_info(134u, "TERRAIN_DATA", 229, 43u, 43u, typeof(mavlink_terrain_data_t)),
		new message_info(135u, "TERRAIN_CHECK", 203, 8u, 8u, typeof(mavlink_terrain_check_t)),
		new message_info(136u, "TERRAIN_REPORT", 1, 22u, 22u, typeof(mavlink_terrain_report_t)),
		new message_info(137u, "SCALED_PRESSURE2", 195, 14u, 16u, typeof(mavlink_scaled_pressure2_t)),
		new message_info(138u, "ATT_POS_MOCAP", 109, 36u, 120u, typeof(mavlink_att_pos_mocap_t)),
		new message_info(139u, "SET_ACTUATOR_CONTROL_TARGET", 168, 43u, 43u, typeof(mavlink_set_actuator_control_target_t)),
		new message_info(140u, "ACTUATOR_CONTROL_TARGET", 181, 41u, 41u, typeof(mavlink_actuator_control_target_t)),
		new message_info(141u, "ALTITUDE", 47, 32u, 32u, typeof(mavlink_altitude_t)),
		new message_info(142u, "RESOURCE_REQUEST", 72, 243u, 243u, typeof(mavlink_resource_request_t)),
		new message_info(143u, "SCALED_PRESSURE3", 131, 14u, 16u, typeof(mavlink_scaled_pressure3_t)),
		new message_info(144u, "FOLLOW_TARGET", 127, 93u, 93u, typeof(mavlink_follow_target_t)),
		new message_info(146u, "CONTROL_SYSTEM_STATE", 103, 100u, 100u, typeof(mavlink_control_system_state_t)),
		new message_info(147u, "BATTERY_STATUS", 154, 36u, 54u, typeof(mavlink_battery_status_t)),
		new message_info(148u, "AUTOPILOT_VERSION", 178, 60u, 78u, typeof(mavlink_autopilot_version_t)),
		new message_info(149u, "LANDING_TARGET", 200, 30u, 60u, typeof(mavlink_landing_target_t)),
		new message_info(162u, "FENCE_STATUS", 189, 8u, 9u, typeof(mavlink_fence_status_t)),
		new message_info(192u, "MAG_CAL_REPORT", 36, 44u, 54u, typeof(mavlink_mag_cal_report_t)),
		new message_info(225u, "EFI_STATUS", 208, 65u, 73u, typeof(mavlink_efi_status_t)),
		new message_info(230u, "ESTIMATOR_STATUS", 163, 42u, 42u, typeof(mavlink_estimator_status_t)),
		new message_info(231u, "WIND_COV", 105, 40u, 40u, typeof(mavlink_wind_cov_t)),
		new message_info(232u, "GPS_INPUT", 151, 63u, 65u, typeof(mavlink_gps_input_t)),
		new message_info(233u, "GPS_RTCM_DATA", 35, 182u, 182u, typeof(mavlink_gps_rtcm_data_t)),
		new message_info(234u, "HIGH_LATENCY", 150, 40u, 40u, typeof(mavlink_high_latency_t)),
		new message_info(235u, "HIGH_LATENCY2", 179, 42u, 42u, typeof(mavlink_high_latency2_t)),
		new message_info(241u, "VIBRATION", 90, 32u, 32u, typeof(mavlink_vibration_t)),
		new message_info(242u, "HOME_POSITION", 104, 52u, 60u, typeof(mavlink_home_position_t)),
		new message_info(243u, "SET_HOME_POSITION", 85, 53u, 61u, typeof(mavlink_set_home_position_t)),
		new message_info(244u, "MESSAGE_INTERVAL", 95, 6u, 6u, typeof(mavlink_message_interval_t)),
		new message_info(245u, "EXTENDED_SYS_STATE", 130, 2u, 2u, typeof(mavlink_extended_sys_state_t)),
		new message_info(246u, "ADSB_VEHICLE", 184, 38u, 38u, typeof(mavlink_adsb_vehicle_t)),
		new message_info(247u, "COLLISION", 81, 19u, 19u, typeof(mavlink_collision_t)),
		new message_info(248u, "V2_EXTENSION", 8, 254u, 254u, typeof(mavlink_v2_extension_t)),
		new message_info(249u, "MEMORY_VECT", 204, 36u, 36u, typeof(mavlink_memory_vect_t)),
		new message_info(250u, "DEBUG_VECT", 49, 30u, 30u, typeof(mavlink_debug_vect_t)),
		new message_info(251u, "NAMED_VALUE_FLOAT", 170, 18u, 18u, typeof(mavlink_named_value_float_t)),
		new message_info(252u, "NAMED_VALUE_INT", 44, 18u, 18u, typeof(mavlink_named_value_int_t)),
		new message_info(253u, "STATUSTEXT", 83, 51u, 54u, typeof(mavlink_statustext_t)),
		new message_info(254u, "DEBUG", 46, 9u, 9u, typeof(mavlink_debug_t)),
		new message_info(256u, "SETUP_SIGNING", 71, 42u, 42u, typeof(mavlink_setup_signing_t)),
		new message_info(257u, "BUTTON_CHANGE", 131, 9u, 9u, typeof(mavlink_button_change_t)),
		new message_info(258u, "PLAY_TUNE", 187, 32u, 232u, typeof(mavlink_play_tune_t)),
		new message_info(259u, "CAMERA_INFORMATION", 92, 235u, 237u, typeof(mavlink_camera_information_t)),
		new message_info(260u, "CAMERA_SETTINGS", 146, 5u, 14u, typeof(mavlink_camera_settings_t)),
		new message_info(261u, "STORAGE_INFORMATION", 179, 27u, 61u, typeof(mavlink_storage_information_t)),
		new message_info(262u, "CAMERA_CAPTURE_STATUS", 12, 18u, 23u, typeof(mavlink_camera_capture_status_t)),
		new message_info(263u, "CAMERA_IMAGE_CAPTURED", 133, 255u, 255u, typeof(mavlink_camera_image_captured_t)),
		new message_info(264u, "FLIGHT_INFORMATION", 49, 28u, 32u, typeof(mavlink_flight_information_t)),
		new message_info(265u, "MOUNT_ORIENTATION", 26, 16u, 20u, typeof(mavlink_mount_orientation_t)),
		new message_info(266u, "LOGGING_DATA", 193, 255u, 255u, typeof(mavlink_logging_data_t)),
		new message_info(267u, "LOGGING_DATA_ACKED", 35, 255u, 255u, typeof(mavlink_logging_data_acked_t)),
		new message_info(268u, "LOGGING_ACK", 14, 4u, 4u, typeof(mavlink_logging_ack_t)),
		new message_info(269u, "VIDEO_STREAM_INFORMATION", 109, 213u, 215u, typeof(mavlink_video_stream_information_t)),
		new message_info(270u, "VIDEO_STREAM_STATUS", 59, 19u, 20u, typeof(mavlink_video_stream_status_t)),
		new message_info(271u, "CAMERA_FOV_STATUS", 22, 52u, 53u, typeof(mavlink_camera_fov_status_t)),
		new message_info(275u, "CAMERA_TRACKING_IMAGE_STATUS", 126, 31u, 32u, typeof(mavlink_camera_tracking_image_status_t)),
		new message_info(276u, "CAMERA_TRACKING_GEO_STATUS", 18, 49u, 50u, typeof(mavlink_camera_tracking_geo_status_t)),
		new message_info(277u, "CAMERA_THERMAL_RANGE", 62, 30u, 30u, typeof(mavlink_camera_thermal_range_t)),
		new message_info(280u, "GIMBAL_MANAGER_INFORMATION", 70, 33u, 33u, typeof(mavlink_gimbal_manager_information_t)),
		new message_info(281u, "GIMBAL_MANAGER_STATUS", 48, 13u, 13u, typeof(mavlink_gimbal_manager_status_t)),
		new message_info(282u, "GIMBAL_MANAGER_SET_ATTITUDE", 123, 35u, 35u, typeof(mavlink_gimbal_manager_set_attitude_t)),
		new message_info(283u, "GIMBAL_DEVICE_INFORMATION", 74, 144u, 145u, typeof(mavlink_gimbal_device_information_t)),
		new message_info(284u, "GIMBAL_DEVICE_SET_ATTITUDE", 99, 32u, 32u, typeof(mavlink_gimbal_device_set_attitude_t)),
		new message_info(285u, "GIMBAL_DEVICE_ATTITUDE_STATUS", 137, 40u, 49u, typeof(mavlink_gimbal_device_attitude_status_t)),
		new message_info(286u, "AUTOPILOT_STATE_FOR_GIMBAL_DEVICE", 210, 53u, 57u, typeof(mavlink_autopilot_state_for_gimbal_device_t)),
		new message_info(287u, "GIMBAL_MANAGER_SET_PITCHYAW", 1, 23u, 23u, typeof(mavlink_gimbal_manager_set_pitchyaw_t)),
		new message_info(288u, "GIMBAL_MANAGER_SET_MANUAL_CONTROL", 20, 23u, 23u, typeof(mavlink_gimbal_manager_set_manual_control_t)),
		new message_info(290u, "ESC_INFO", 251, 46u, 46u, typeof(mavlink_esc_info_t)),
		new message_info(291u, "ESC_STATUS", 10, 57u, 57u, typeof(mavlink_esc_status_t)),
		new message_info(299u, "WIFI_CONFIG_AP", 19, 96u, 98u, typeof(mavlink_wifi_config_ap_t)),
		new message_info(300u, "PROTOCOL_VERSION", 217, 22u, 22u, typeof(mavlink_protocol_version_t)),
		new message_info(301u, "AIS_VESSEL", 243, 58u, 58u, typeof(mavlink_ais_vessel_t)),
		new message_info(310u, "UAVCAN_NODE_STATUS", 28, 17u, 17u, typeof(mavlink_uavcan_node_status_t)),
		new message_info(311u, "UAVCAN_NODE_INFO", 95, 116u, 116u, typeof(mavlink_uavcan_node_info_t)),
		new message_info(320u, "PARAM_EXT_REQUEST_READ", 243, 20u, 20u, typeof(mavlink_param_ext_request_read_t)),
		new message_info(321u, "PARAM_EXT_REQUEST_LIST", 88, 2u, 2u, typeof(mavlink_param_ext_request_list_t)),
		new message_info(322u, "PARAM_EXT_VALUE", 243, 149u, 149u, typeof(mavlink_param_ext_value_t)),
		new message_info(323u, "PARAM_EXT_SET", 78, 147u, 147u, typeof(mavlink_param_ext_set_t)),
		new message_info(324u, "PARAM_EXT_ACK", 132, 146u, 146u, typeof(mavlink_param_ext_ack_t)),
		new message_info(330u, "OBSTACLE_DISTANCE", 23, 158u, 167u, typeof(mavlink_obstacle_distance_t)),
		new message_info(331u, "ODOMETRY", 91, 230u, 233u, typeof(mavlink_odometry_t)),
		new message_info(332u, "TRAJECTORY_REPRESENTATION_WAYPOINTS", 236, 239u, 239u, typeof(mavlink_trajectory_representation_waypoints_t)),
		new message_info(333u, "TRAJECTORY_REPRESENTATION_BEZIER", 231, 109u, 109u, typeof(mavlink_trajectory_representation_bezier_t)),
		new message_info(334u, "CELLULAR_STATUS", 72, 10u, 10u, typeof(mavlink_cellular_status_t)),
		new message_info(335u, "ISBD_LINK_STATUS", 225, 24u, 24u, typeof(mavlink_isbd_link_status_t)),
		new message_info(336u, "CELLULAR_CONFIG", 245, 84u, 84u, typeof(mavlink_cellular_config_t)),
		new message_info(339u, "RAW_RPM", 199, 5u, 5u, typeof(mavlink_raw_rpm_t)),
		new message_info(340u, "UTM_GLOBAL_POSITION", 99, 70u, 70u, typeof(mavlink_utm_global_position_t)),
		new message_info(350u, "DEBUG_FLOAT_ARRAY", 232, 20u, 252u, typeof(mavlink_debug_float_array_t)),
		new message_info(360u, "ORBIT_EXECUTION_STATUS", 11, 25u, 25u, typeof(mavlink_orbit_execution_status_t)),
		new message_info(370u, "SMART_BATTERY_INFO", 75, 87u, 109u, typeof(mavlink_smart_battery_info_t)),
		new message_info(371u, "FUEL_STATUS", 10, 26u, 26u, typeof(mavlink_fuel_status_t)),
		new message_info(372u, "BATTERY_INFO", 26, 140u, 140u, typeof(mavlink_battery_info_t)),
		new message_info(373u, "GENERATOR_STATUS", 117, 42u, 42u, typeof(mavlink_generator_status_t)),
		new message_info(375u, "ACTUATOR_OUTPUT_STATUS", 251, 140u, 140u, typeof(mavlink_actuator_output_status_t)),
		new message_info(380u, "TIME_ESTIMATE_TO_TARGET", 232, 20u, 20u, typeof(mavlink_time_estimate_to_target_t)),
		new message_info(385u, "TUNNEL", 147, 133u, 133u, typeof(mavlink_tunnel_t)),
		new message_info(386u, "CAN_FRAME", 132, 16u, 16u, typeof(mavlink_can_frame_t)),
		new message_info(387u, "CANFD_FRAME", 4, 72u, 72u, typeof(mavlink_canfd_frame_t)),
		new message_info(388u, "CAN_FILTER_MODIFY", 8, 37u, 37u, typeof(mavlink_can_filter_modify_t)),
		new message_info(390u, "ONBOARD_COMPUTER_STATUS", 156, 238u, 238u, typeof(mavlink_onboard_computer_status_t)),
		new message_info(395u, "COMPONENT_INFORMATION", 0, 212u, 212u, typeof(mavlink_component_information_t)),
		new message_info(396u, "COMPONENT_INFORMATION_BASIC", 50, 160u, 160u, typeof(mavlink_component_information_basic_t)),
		new message_info(397u, "COMPONENT_METADATA", 182, 108u, 108u, typeof(mavlink_component_metadata_t)),
		new message_info(400u, "PLAY_TUNE_V2", 110, 254u, 254u, typeof(mavlink_play_tune_v2_t)),
		new message_info(401u, "SUPPORTED_TUNES", 183, 6u, 6u, typeof(mavlink_supported_tunes_t)),
		new message_info(410u, "EVENT", 160, 53u, 53u, typeof(mavlink_event_t)),
		new message_info(411u, "CURRENT_EVENT_SEQUENCE", 106, 3u, 3u, typeof(mavlink_current_event_sequence_t)),
		new message_info(412u, "REQUEST_EVENT", 33, 6u, 6u, typeof(mavlink_request_event_t)),
		new message_info(413u, "RESPONSE_EVENT_ERROR", 77, 7u, 7u, typeof(mavlink_response_event_error_t)),
		new message_info(435u, "AVAILABLE_MODES", 134, 46u, 46u, typeof(mavlink_available_modes_t)),
		new message_info(436u, "CURRENT_MODE", 193, 9u, 9u, typeof(mavlink_current_mode_t)),
		new message_info(437u, "AVAILABLE_MODES_MONITOR", 30, 1u, 1u, typeof(mavlink_available_modes_monitor_t)),
		new message_info(440u, "ILLUMINATOR_STATUS", 66, 35u, 35u, typeof(mavlink_illuminator_status_t)),
		new message_info(9000u, "WHEEL_DISTANCE", 113, 137u, 137u, typeof(mavlink_wheel_distance_t)),
		new message_info(9005u, "WINCH_STATUS", 117, 34u, 34u, typeof(mavlink_winch_status_t)),
		new message_info(12900u, "OPEN_DRONE_ID_BASIC_ID", 114, 44u, 44u, typeof(mavlink_open_drone_id_basic_id_t)),
		new message_info(12901u, "OPEN_DRONE_ID_LOCATION", 254, 59u, 59u, typeof(mavlink_open_drone_id_location_t)),
		new message_info(12902u, "OPEN_DRONE_ID_AUTHENTICATION", 140, 53u, 53u, typeof(mavlink_open_drone_id_authentication_t)),
		new message_info(12903u, "OPEN_DRONE_ID_SELF_ID", 249, 46u, 46u, typeof(mavlink_open_drone_id_self_id_t)),
		new message_info(12904u, "OPEN_DRONE_ID_SYSTEM", 77, 54u, 54u, typeof(mavlink_open_drone_id_system_t)),
		new message_info(12905u, "OPEN_DRONE_ID_OPERATOR_ID", 49, 43u, 43u, typeof(mavlink_open_drone_id_operator_id_t)),
		new message_info(12915u, "OPEN_DRONE_ID_MESSAGE_PACK", 94, 249u, 249u, typeof(mavlink_open_drone_id_message_pack_t)),
		new message_info(12918u, "OPEN_DRONE_ID_ARM_STATUS", 139, 51u, 51u, typeof(mavlink_open_drone_id_arm_status_t)),
		new message_info(12919u, "OPEN_DRONE_ID_SYSTEM_UPDATE", 7, 18u, 18u, typeof(mavlink_open_drone_id_system_update_t)),
		new message_info(12920u, "HYGROMETER_SENSOR", 20, 5u, 5u, typeof(mavlink_hygrometer_sensor_t))
	};

	public const byte MAVLINK_VERSION = 3;

	public const byte MAVLINK_IFLAG_SIGNED = 1;

	public const byte MAVLINK_IFLAG_MASK = 1;

	public static string GetUnit(string fieldname, Type packetype = null, string name = "", uint msgid = uint.MaxValue)
	{
		try
		{
			message_info message_info2 = default(message_info);
			if (packetype != null)
			{
				message_info2 = MAVLINK_MESSAGE_INFOS.First((message_info a) => a.type == packetype);
			}
			if (msgid != uint.MaxValue)
			{
				message_info2 = MAVLINK_MESSAGE_INFOS.First((message_info a) => a.msgid == msgid);
			}
			if (!string.IsNullOrEmpty(name))
			{
				message_info2 = MAVLINK_MESSAGE_INFOS.First((message_info a) => a.name == name);
			}
			if (message_info2.name == "")
			{
				return "";
			}
			FieldInfo field = message_info2.type.GetField(fieldname);
			if (field != null)
			{
				object[] customAttributes = field.GetCustomAttributes(inherit: false);
				if (customAttributes.Length != 0)
				{
					return customAttributes.OfType<Units>().First().Unit;
				}
			}
		}
		catch
		{
		}
		return "";
	}
}
