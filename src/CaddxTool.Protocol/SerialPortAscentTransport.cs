using System.IO.Ports;

namespace CaddxTool.Protocol;

public class SerialPortAscentTransport : IAscentTransport
{
    private readonly SerialPort _port;

    public SerialPortAscentTransport(string portName, int baudRate = 115200)
    {
        _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
        {
            // Generous rather than tight: real USB-CDC-ACM hardware transfers at
            // USB speed regardless of this nominal baud rate, so it never notices
            // a longer timeout. But some virtual serial transports genuinely
            // enforce baud-accurate throughput — confirmed with tty0tty (used to
            // test against the official Windows app under Wine, since it's the
            // only local option that implements TIOCMGET; plain socat PTYs don't
            // pace at all) — where a single 1MB SENDFILE_DATA chunk can take ~91s
            // to write at 115200 baud. 2000ms blew up mid-write on that transport
            // with an unhandled TimeoutException; see src/PROJECT.md.
            ReadTimeout = 150000,
            WriteTimeout = 150000,
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
