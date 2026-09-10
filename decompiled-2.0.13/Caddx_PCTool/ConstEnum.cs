using System.Runtime.InteropServices;

namespace Caddx_PCTool;

public class ConstEnum
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SendHeader_Gim
	{
		public byte CMD;

		public float roll;

		public float pitch;

		public float yaw;
	}

	public class RecvPacket_Gim
	{
		public float a;

		public float b;

		public float c;

		public float roll;

		public float pitch;

		public float yaw;

		public float x;

		public float y;

		public float z;

		public float accx;

		public float accy;

		public float accz;

		public float version;

		public float sensitivity;

		public float temp;

		public ushort[] channels;

		public byte modeChann;

		public byte sensChann;

		public byte rollChann;

		public byte pitchChann;

		public byte yawChann;

		public byte switchMode;

		public byte index;

		public byte rollgain;

		public byte pitchgain;

		public byte yawgain;

		public byte CMD;

		public byte sumcheck;

		public byte addcheck;

		public byte fileflag;

		public byte currMode;

		public byte m0Mode;

		public byte m1Mode;

		public byte m2Mode;

		public byte angleProtectEnable;

		public uint[] ID = new uint[3];

		public RecvPacket_Gim()
		{
			version = (sensitivity = (temp = (a = (b = (c = (roll = (pitch = (yaw = (x = (y = (z = (accx = (accy = (accz = 0f))))))))))))));
			modeChann = (sensChann = (rollChann = (pitchChann = (yawChann = (switchMode = (index = (rollgain = (pitchgain = (yawgain = (CMD = (sumcheck = (addcheck = (fileflag = 0)))))))))))));
			channels = new ushort[16];
		}
	}

	public enum GIM_CMD : byte
	{
		StartGim = 0,
		StopGim = 1,
		CalibGyro = 2,
		SetRollGain = 3,
		SetPitchGain = 4,
		SetYawGain = 5,
		SetModeChann = 6,
		SetSensChann = 7,
		SetRollChann = 8,
		SetPitchChann = 9,
		SetYawChann = 10,
		ConnModeSel = 11,
		PushParam = 12,
		DownParam = 13,
		WriteParam = 14,
		ssss = 15,
		SetGMMode = 21,
		SetSensNum = 22,
		SetRollNum = 23,
		SetPitchNum = 24,
		SetYawNum = 25,
		AngProtect = 31,
		ModeSlot = 33
	}

	public const int FRAMELENGTH = 118;

	public const int FRAMELENGTH2 = 121;

	public const int DATALENGTH = 110;

	public const int DATALENGTH2 = 113;

	public const int DATALENGTH3 = 117;

	public const byte HEAD1 = 171;

	public const byte HEAD2 = byte.MaxValue;

	public const int OFFSET_LENGTH = 4;

	public const int OFFSET_VERSION = 5;

	public const int OFFSET_INDEX = 3;

	public const int OFFSET_FLOATSTART = 6;

	public const int FLOATSIZE = 4;

	public const int OFFSET_CHANNELS = 54;

	public const int CHANNELSCOUNT = 16;

	public const int OFFSET_MODE = 86;

	public const int OFFSET_SEN = 87;

	public const int OFFSET_CHANNELR = 88;

	public const int OFFSET_CHANNELP = 89;

	public const int OFFSET_CHANNELY = 90;

	public const int OFFSET_SWITCHMODE = 91;

	public const int OFFSET_ID = 92;

	public const int OFFSET_ROLLGAIN = 104;

	public const int OFFSET_PITCHGAIN = 105;

	public const int OFFSET_YAWGAIN = 106;

	public const int OFFSET_CMD = 107;

	public const int OFFSET_SENSITIVITY = 108;

	public const int OFFSET_TEMP = 112;

	public const int OFFSET_SUMCHECK = 116;

	public const int OFFSET_ADDCHECK = 117;
}
