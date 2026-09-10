using System;

namespace Caddx_PCTool;

public class CLIModeFrmEventArgs : EventArgs
{
	public string Desc { get; set; }

	public byte[] Bytes { get; set; }

	public string PortName { get; set; }
}
