using System;

namespace Caddx_PCTool;

public sealed class SerialPortDeviceMatcher
{
	public string Vid { get; private set; }

	public string Pid { get; private set; }

	private SerialPortDeviceMatcher(string vid, string pid)
	{
		Vid = vid;
		Pid = pid;
	}

	public static bool TryCreate(UsbDevInfo device, out SerialPortDeviceMatcher matcher)
	{
		matcher = null;
		if (device == null || string.IsNullOrWhiteSpace(device.VID) || string.IsNullOrWhiteSpace(device.PID))
		{
			return false;
		}
		matcher = new SerialPortDeviceMatcher(device.VID.Trim().ToUpperInvariant(), device.PID.Trim().ToUpperInvariant());
		return true;
	}

	public bool Matches(UsbDevInfo device)
	{
		return device != null && string.Equals(Vid, device.VID?.Trim(), StringComparison.OrdinalIgnoreCase) && string.Equals(Pid, device.PID?.Trim(), StringComparison.OrdinalIgnoreCase);
	}
}
