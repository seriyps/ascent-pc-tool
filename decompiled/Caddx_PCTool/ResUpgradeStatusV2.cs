using System.Runtime.InteropServices;

namespace Caddx_PCTool;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ResUpgradeStatusV2
{
	public int Percent;

	public int Status;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	public byte[] Detail;

	public ResUpgradeStatusV2()
	{
		Detail = new byte[64];
	}
}
