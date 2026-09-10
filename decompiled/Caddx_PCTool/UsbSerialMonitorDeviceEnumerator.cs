using System.Collections.Generic;

namespace Caddx_PCTool;

public sealed class UsbSerialMonitorDeviceEnumerator : IUsbDeviceEnumerator
{
	public IReadOnlyList<UsbDevInfo> GetCurrentDevices()
	{
		using UsbSerialMonitor usbSerialMonitor = new UsbSerialMonitor();
		List<UsbDevInfo> list = usbSerialMonitor.ManualSearchDevices();
		List<UsbDevInfo> list2 = new List<UsbDevInfo>(list.Count);
		foreach (UsbDevInfo item in list)
		{
			list2.Add(UsbDevInfoSnapshot.Clone(item));
		}
		return list2;
	}
}
