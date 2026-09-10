using System.Runtime.InteropServices;

namespace Caddx_PCTool;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ArFileInfoV2
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	public byte[] MD5;

	public int length;

	public int saveAsFile;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	public byte[] filePath;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	public byte[] fileDir;
}
