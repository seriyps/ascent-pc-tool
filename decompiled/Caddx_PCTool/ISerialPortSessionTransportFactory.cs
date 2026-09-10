namespace Caddx_PCTool;

public interface ISerialPortSessionTransportFactory
{
	ISerialPortSessionTransport Create(SerialPortTransportKind kind);
}
