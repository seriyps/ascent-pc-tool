using System;

namespace Caddx_PCTool;

public sealed class SerialPortSessionHandle
{
	public Guid SessionId { get; private set; }

	public long Generation { get; private set; }

	public SerialPortOwner Owner { get; private set; }

	public string PortName { get; private set; }

	public string Vid { get; private set; }

	public string Pid { get; private set; }

	internal SerialPortSessionHandle(Guid sessionId, long generation, SerialPortOwner owner, string portName, string vid = null, string pid = null)
	{
		SessionId = sessionId;
		Generation = generation;
		Owner = owner;
		PortName = portName;
		Vid = vid;
		Pid = pid;
	}
}
