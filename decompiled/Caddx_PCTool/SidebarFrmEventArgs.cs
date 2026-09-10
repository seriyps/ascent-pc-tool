using System;

namespace Caddx_PCTool;

public class SidebarFrmEventArgs : EventArgs
{
	public string Text { get; set; }

	public string Name { get; set; }

	public string ID { get; set; }

	public int Idx { get; set; }
}
