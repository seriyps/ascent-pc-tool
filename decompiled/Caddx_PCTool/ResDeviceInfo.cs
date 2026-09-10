using System.Runtime.InteropServices;
using System.Text;

namespace Caddx_PCTool;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ResDeviceInfo
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

	public override string ToString()
	{
		return "Fw: " + Encoding.ASCII.GetString(firmwareInfo).TrimEnd(new char[1]) + " | SN: " + Encoding.ASCII.GetString(serialNumber).TrimEnd(new char[1]) + " | HW: " + Encoding.ASCII.GetString(hardwareVersion).TrimEnd(new char[1]) + " | CPU Temp: " + cputemp + "℃ | MaxRecv: " + receiveMaxSize + "kb" + $"\r\nstatus={status} " + "\r\ndetail=" + Encoding.ASCII.GetString(detail).TrimEnd(new char[1]) + "\r\nsdkv=" + Encoding.ASCII.GetString(sdkversion).TrimEnd(new char[1]) + "\r\ndevName=" + Encoding.ASCII.GetString(devicename).TrimEnd(new char[1]);
	}

	public ResDeviceInfo()
	{
		receiveMaxSize = 0;
		sdkversion = new byte[32];
		devicename = new byte[64];
		cputemp = 0;
		firmwareInfo = new byte[64];
		serialNumber = new byte[32];
		hardwareVersion = new byte[32];
		detail = new byte[64];
		status = -1;
	}
}
