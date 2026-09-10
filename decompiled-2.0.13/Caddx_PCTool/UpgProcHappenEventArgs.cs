using System;

namespace Caddx_PCTool;

public class UpgProcHappenEventArgs : EventArgs
{
	public InfoType infoType { get; set; }

	public string Desc { get; set; }

	public float Percent { get; set; }

	public string errMsg { get; set; }

	public UpgState upgState { get; set; }
}
