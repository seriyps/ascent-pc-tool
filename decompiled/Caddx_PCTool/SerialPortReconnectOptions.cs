namespace Caddx_PCTool;

public sealed class SerialPortReconnectOptions
{
	public int TimeoutMs { get; set; }

	public int RetryIntervalMs { get; set; }

	public SerialPortSessionOpenOptions OpenOptions { get; set; }
}
