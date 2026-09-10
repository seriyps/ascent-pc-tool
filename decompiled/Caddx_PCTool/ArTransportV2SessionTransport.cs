using System;

namespace Caddx_PCTool;

public sealed class ArTransportV2SessionTransport : ISerialPortSessionTransport, IDisposable
{
	private ArTransportV2 _transport;

	public SerialPortTransportKind Kind => SerialPortTransportKind.ArTransportV2;

	public string PortName { get; private set; }

	public bool IsOpen => _transport != null && _transport.IsOpen;

	public ArTransportV2 Transport => _transport;

	public bool Open(string portName, UsbDevInfo device, int baudRate)
	{
		Close();
		ArTransportV2 arTransportV = new ArTransportV2();
		if (!arTransportV.Open(portName, UsbDevInfoSnapshot.Clone(device), baudRate))
		{
			arTransportV.Dispose();
			return false;
		}
		_transport = arTransportV;
		PortName = portName;
		return true;
	}

	public void Close()
	{
		ArTransportV2 transport = _transport;
		_transport = null;
		PortName = null;
		transport?.Dispose();
	}

	public void Dispose()
	{
		Close();
	}
}
