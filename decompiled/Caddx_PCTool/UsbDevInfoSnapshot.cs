namespace Caddx_PCTool;

public static class UsbDevInfoSnapshot
{
	public static UsbDevInfo Clone(UsbDevInfo source)
	{
		if (source == null)
		{
			return null;
		}
		return new UsbDevInfo
		{
			DevName = source.DevName,
			PortName = SerialPortSessionText.NormalizePortName(source.PortName),
			VID = source.VID,
			PID = source.PID,
			ConnectedTime = source.ConnectedTime,
			IsInserted = source.IsInserted
		};
	}
}
