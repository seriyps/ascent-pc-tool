using System;

namespace Caddx_PCTool;

public class DevCardEventArgs : EventArgs
{
	public string Name { get; set; }

	public string Desc { get; set; }

	public EventType EveType { get; set; }

	public string DeviceName { get; set; }

	public string PortName { get; set; }

	public string BtnName { get; set; }

	public UsbDevInfo UsbInfo { get; set; }

	public ResAscentInfo AscentInfo { get; set; }

	public SerialPortSessionHandle SessionHandle { get; set; }
}
