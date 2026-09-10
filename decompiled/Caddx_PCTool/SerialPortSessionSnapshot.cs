using System;

namespace Caddx_PCTool;

public sealed class SerialPortSessionSnapshot
{
	public Guid SessionId { get; internal set; }

	public long Generation { get; internal set; }

	public SerialPortOwner Owner { get; internal set; }

	public SerialPortSessionState State { get; internal set; }

	public string PortName { get; internal set; }

	public string Vid { get; internal set; }

	public string Pid { get; internal set; }

	public DateTime OpenedAt { get; internal set; }
}
