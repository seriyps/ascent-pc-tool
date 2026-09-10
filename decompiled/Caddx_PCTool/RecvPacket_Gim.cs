namespace Caddx_PCTool;

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

	public byte lensProfile;

	public uint[] ID = new uint[3];

	public RecvPacket_Gim()
	{
		version = (sensitivity = (temp = (a = (b = (c = (roll = (pitch = (yaw = (x = (y = (z = (accx = (accy = (accz = 0f))))))))))))));
		modeChann = (sensChann = (rollChann = (pitchChann = (yawChann = (switchMode = (index = (rollgain = (pitchgain = (yawgain = (CMD = (sumcheck = (addcheck = (fileflag = (currMode = 0))))))))))))));
		channels = new ushort[16];
	}
}
