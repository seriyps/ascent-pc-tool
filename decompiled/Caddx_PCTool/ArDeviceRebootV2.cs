using System.Runtime.InteropServices;

namespace Caddx_PCTool;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ArDeviceRebootV2
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	public byte[] mode;
}
