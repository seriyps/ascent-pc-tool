namespace Caddx_PCTool;

public enum SerialPortSessionFailureReason
{
	None,
	InvalidPortName,
	InvalidDevice,
	InvalidOptions,
	Busy,
	OpenFailed,
	StaleHandle,
	OwnerMismatch,
	InvalidState,
	ReconnectTimedOut,
	AmbiguousDevice,
	CloseTimedOut,
	ManagerStopping,
	Cancelled
}
