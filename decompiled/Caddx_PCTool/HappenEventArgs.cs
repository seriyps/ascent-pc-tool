using System;

namespace Caddx_PCTool;

public class HappenEventArgs : EventArgs
{
	public EventType eventType { get; set; }

	public InfoType infoType { get; set; }

	public string className { get; set; }

	public string msg { get; set; }

	public string eventDesc { get; set; }

	public int Index { get; set; }

	public float processVal { get; set; }

	public string processDesc { get; set; }

	public string DevName { get; set; }
}
