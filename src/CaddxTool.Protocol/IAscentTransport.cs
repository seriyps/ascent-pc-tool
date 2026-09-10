namespace CaddxTool.Protocol;

// Byte-stream abstraction between AscentClient/FirmwareUpgradeFlow and the
// underlying transport. SerialPort itself isn't practically mockable (thin
// wrapper over a native OS handle), so we abstract at this level instead and
// test against an in-memory fake — see CaddxTool.Protocol.Tests.
public interface IAscentTransport : IDisposable
{
    void Write(byte[] buffer, int offset, int count);

    // Mirrors SerialPort.Read semantics: blocks until at least one byte is
    // available or the transport's read timeout elapses, in which case it
    // throws TimeoutException. Never returns 0.
    int Read(byte[] buffer, int offset, int count);
}
