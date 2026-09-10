namespace Caddx_PCTool;

public struct MSPV1FrameStruct
{
	public byte[] FrameHead_V1;

	public byte Direc;

	public byte PayLoadSize;

	public byte Function;

	public byte[] Payload;

	public byte CheckSum;
}
