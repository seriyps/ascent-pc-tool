using System;

namespace Caddx_PCTool;

public class FlowProgressEventArgsV2 : EventArgs
{
	public string StepName { get; set; }

	public float Percent { get; set; }

	public string Desc { get; set; }
}
