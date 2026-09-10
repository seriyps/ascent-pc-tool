using System.Runtime.InteropServices;
using System.Text;

namespace Caddx_PCTool;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ResDeviceInfoV2
{
	public int receiveMaxSize;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	public byte[] sdkversion;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	public byte[] devicename;

	public int cputemp;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	public byte[] firmwareInfo;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	public byte[] serialNumber;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	public byte[] hardwareVersion;

	public int status;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	public byte[] detail;

	public ResDeviceInfoV2()
	{
		sdkversion = new byte[32];
		devicename = new byte[64];
		firmwareInfo = new byte[64];
		serialNumber = new byte[32];
		hardwareVersion = new byte[32];
		detail = new byte[64];
	}

	public override string ToString()
	{
		return "Fw: " + Encoding.ASCII.GetString(firmwareInfo).TrimEnd(new char[1]) + " | SN: " + Encoding.ASCII.GetString(serialNumber).TrimEnd(new char[1]) + " | HW: " + Encoding.ASCII.GetString(hardwareVersion).TrimEnd(new char[1]) + " | " + $"CPU Temp: {cputemp}℃ | MaxRecv: {receiveMaxSize}kb";
	}
}
