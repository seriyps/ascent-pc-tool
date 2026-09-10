using System;

namespace Caddx_PCTool;

public sealed class AscentSerialPortSessionTransportFactory : ISerialPortSessionTransportFactory
{
	public ISerialPortSessionTransport Create(SerialPortTransportKind kind)
	{
		return kind switch
		{
			SerialPortTransportKind.LegacyFsm => new LegacyFsmSessionTransport(), 
			SerialPortTransportKind.ArTransportV2 => new ArTransportV2SessionTransport(), 
			_ => throw new ArgumentOutOfRangeException("kind"), 
		};
	}
}
