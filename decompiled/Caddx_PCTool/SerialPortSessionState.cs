namespace Caddx_PCTool;

public enum SerialPortSessionState
{
	Reserved,
	Opening,
	Open,
	Reconnecting,
	Closing,
	CloseTimedOut,
	Closed,
	Faulted
}
