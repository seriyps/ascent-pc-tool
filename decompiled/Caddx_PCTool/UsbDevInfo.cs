using System;

namespace Caddx_PCTool;

public class UsbDevInfo
{
	public string DevName { get; set; }

	public string PortName { get; set; }

	public string VID { get; set; }

	public string PID { get; set; }

	public DateTime ConnectedTime { get; set; }

	public bool IsInserted { get; set; }

	public override string ToString()
	{
		return "设备名: " + DevName + ", 串口名: " + PortName + ", VID: " + VID + ", PID: " + PID;
	}
}
