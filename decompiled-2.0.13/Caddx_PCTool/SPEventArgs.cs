using System;

namespace Caddx_PCTool;

public class SPEventArgs : EventArgs
{
	public string Data { get; set; }

	public string[] DataArr { get; set; }

	public string PortName { get; set; }

	public byte[] HeaderBuffer { get; set; }

	public byte[] DataBuffer { get; set; }

	public byte Cmd { get; set; }

	public int Index { get; set; }

	public int DelayTime { get; set; }

	public int SendType { get; set; }

	public byte VerType { get; set; }
}
