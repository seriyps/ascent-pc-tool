using System;

namespace Caddx_PCTool.Update;

public class UpdateServiceException : Exception
{
	public UpdateFailureKind Kind { get; }

	public UpdateServiceException(UpdateFailureKind kind, string message, Exception inner = null)
		: base(message, inner)
	{
		Kind = kind;
	}
}
