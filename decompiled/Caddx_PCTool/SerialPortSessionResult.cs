namespace Caddx_PCTool;

public abstract class SerialPortSessionResult
{
	public bool Succeeded { get; internal set; }

	public SerialPortSessionFailureReason FailureReason { get; internal set; }

	public SerialPortOwner? CurrentOwner { get; internal set; }

	public SerialPortSessionState? CurrentState { get; internal set; }

	public SerialPortSessionHandle Handle { get; internal set; }

	public SerialPortSessionSnapshot Snapshot { get; internal set; }
}
