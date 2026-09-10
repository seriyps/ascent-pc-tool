using System;

namespace Caddx_PCTool;

public sealed class LegacyFsmSessionTransport : ISerialPortSessionTransport, IDisposable
{
	private UsbSerialportFSM _fsm;

	public SerialPortTransportKind Kind => SerialPortTransportKind.LegacyFsm;

	public string PortName { get; private set; }

	public bool IsOpen => _fsm != null && _fsm.IsComOpened;

	public UsbSerialportFSM Fsm => _fsm;

	public bool Open(string portName, UsbDevInfo device, int baudRate)
	{
		Close();
		UsbSerialportFSM usbSerialportFSM = new UsbSerialportFSM(UsbDevInfoSnapshot.Clone(device));
		if (!usbSerialportFSM.Open(portName, baudRate))
		{
			usbSerialportFSM.Dispose();
			return false;
		}
		_fsm = usbSerialportFSM;
		PortName = portName;
		return true;
	}

	public void Close()
	{
		UsbSerialportFSM fsm = _fsm;
		_fsm = null;
		PortName = null;
		fsm?.Dispose();
	}

	public void Dispose()
	{
		Close();
	}
}
