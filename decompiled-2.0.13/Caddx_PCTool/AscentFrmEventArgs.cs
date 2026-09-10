using System;

namespace Caddx_PCTool;

public class AscentFrmEventArgs : EventArgs
{
	public string Desc { get; set; }

	public string PortName { get; set; }

	public bool IsOpen { get; set; }
}
