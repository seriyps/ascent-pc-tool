using System.Threading;
using System.Threading.Tasks;

namespace Caddx_PCTool;

public interface ISerialPortSessionManager
{
	Task<SerialPortAcquireResult> AcquireAsync(UsbDevInfo device, SerialPortOwner owner, SerialPortSessionOpenOptions options, CancellationToken cancellationToken);

	SerialPortTransferResult TryTransfer(SerialPortSessionHandle source, SerialPortOwner targetOwner);

	Task<SerialPortReconnectResult> ReconnectByVidPidAsync(SerialPortSessionHandle source, SerialPortReconnectOptions options, CancellationToken cancellationToken);

	Task<SerialPortReleaseResult> ReleaseAsync(SerialPortSessionHandle source, SerialPortReleaseReason reason);

	void HandleDeviceRemoved(string portName);

	bool TryGetSnapshot(string portName, out SerialPortSessionSnapshot snapshot);

	bool TryGetTransport(SerialPortSessionHandle handle, out ISerialPortSessionTransport transport);

	Task StopAsync();
}
