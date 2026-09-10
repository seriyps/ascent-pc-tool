using System;

namespace Caddx_PCTool;

public interface ISerialPortSessionTransport : IDisposable
{
	SerialPortTransportKind Kind { get; }

	string PortName { get; }

	bool IsOpen { get; }

	bool Open(string portName, UsbDevInfo device, int baudRate);

	void Close();
}
