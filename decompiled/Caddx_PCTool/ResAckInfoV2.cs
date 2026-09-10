using System.Runtime.InteropServices;

namespace Caddx_PCTool;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ResAckInfoV2
{
	public int Status;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	public byte[] Detail;

	public ResAckInfoV2()
	{
		Detail = new byte[64];
	}
}
