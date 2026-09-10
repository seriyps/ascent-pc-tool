using System.IO.Ports;

namespace CaddxTool.Protocol;

public class SerialPortAscentTransport : IAscentTransport
{
    private readonly SerialPort _port;

    public SerialPortAscentTransport(string portName, int baudRate = 115200)
    {
        _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
        {
            ReadTimeout = 2000,
            WriteTimeout = 2000,
        };
        _port.Open();
    }

    public void Write(byte[] buffer, int offset, int count) => _port.Write(buffer, offset, count);

    public int Read(byte[] buffer, int offset, int count) => _port.Read(buffer, offset, count);

    public void Dispose()
    {
        try { _port.Close(); } catch { /* best-effort */ }
        _port.Dispose();
    }
}
