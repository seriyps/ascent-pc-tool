using System.Collections.Generic;

namespace Caddx_PCTool;

public interface IUsbDeviceEnumerator
{
	IReadOnlyList<UsbDevInfo> GetCurrentDevices();
}
