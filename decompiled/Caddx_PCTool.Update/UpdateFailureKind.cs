namespace Caddx_PCTool.Update;

public enum UpdateFailureKind
{
	NotInstalled,
	NotConfigured,
	NoUpdate,
	Cancelled,
	Network,
	RateLimited,
	Forbidden,
	Server,
	StaleResult,
	Unknown
}
