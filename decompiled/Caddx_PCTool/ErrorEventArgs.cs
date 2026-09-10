using System;

namespace Caddx_PCTool;

public class ErrorEventArgs : EventArgs
{
	public ErrorCode ErrCode { get; set; }

	public ErrorType ErrorType { get; set; }

	public string ErrDesc { get; set; }

	public string ErrPosition { get; set; }
}
