using System;

namespace Caddx_PCTool;

public class FlowResultEventArgsV2 : EventArgs
{
	public bool Success { get; set; }

	public string Message { get; set; }

	public string ErrorDetail { get; set; }
}
