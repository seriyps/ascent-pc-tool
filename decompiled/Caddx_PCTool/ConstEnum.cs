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

	public const int HEADLENG = 8;

	public const int FRAMELENGTH = 118;

	public const int FRAMELENGTH2 = 121;

	public const int DATALENGTH = 110;

	public const int DATALENGTH2 = 113;

	public const int DATALENGTH3 = 117;

	public const int DATALENGTH4 = 134;

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
