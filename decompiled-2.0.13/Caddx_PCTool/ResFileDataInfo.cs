using System.Runtime.InteropServices;

namespace Caddx_PCTool;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ResFileDataInfo
{
	public int Length;

	public int Cursize;

	public int Totalsize;

	public int Status;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	public byte[] Detail;

	public ResFileDataInfo()
	{
		Length = (Cursize = (Totalsize = 0));
		Status = -1;
		Detail = new byte[64];
	}
}
