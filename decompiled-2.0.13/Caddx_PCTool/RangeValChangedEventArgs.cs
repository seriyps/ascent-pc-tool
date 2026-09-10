using System;

namespace Caddx_PCTool;

public class RangeValChangedEventArgs : EventArgs
{
	public int MinVal { get; }

	public int MaxVal { get; }

	public RangeValChangedEventArgs(int lower, int upper)
	{
		MinVal = lower;
		MaxVal = upper;
	}
}
