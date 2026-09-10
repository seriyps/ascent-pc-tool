using System;

namespace Caddx_PCTool;

public class FrmEventArgs : EventArgs
{
	public string Desc { get; set; }

	public string PortName { get; set; }

	public bool IsOpen { get; set; }

	public InfoType InfoType { get; set; }

	public UserLevel CurrLevel { get; set; }
}
