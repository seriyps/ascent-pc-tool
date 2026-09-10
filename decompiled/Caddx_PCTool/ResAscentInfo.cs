using System;

namespace Caddx_PCTool;

public class ResAscentInfo
{
	public string SN { get; set; }

	public string FWVers { get; set; }

	public string HWVers { get; set; }

	public int MCUTemp { get; set; }

	public string DevName { get; set; }

	public string SDKVers { get; set; }

	public string Details { get; set; }

	public int RecMaxSize { get; set; }

	public UsbDevInfo UsbInfo { get; set; }

	public DateTime UsbTime { get; set; }

	public override string ToString()
	{
		return "串口名=" + UsbInfo.PortName + ",固件版本号=" + FWVers + ", 硬件版本号=" + HWVers + ",设备名=" + DevName + string.Format("\r\nSDKV={0}, SN= {1},温度= {2},最大接收{3}kb", new object[4] { SDKVers, SN, MCUTemp, RecMaxSize });
	}
}
