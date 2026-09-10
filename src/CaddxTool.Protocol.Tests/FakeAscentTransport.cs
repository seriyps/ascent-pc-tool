namespace CaddxTool.Protocol.Tests;

public record SentPacket(uint Cmd, uint Seq, uint Retry, byte[] Payload);

// In-memory IAscentTransport for exercising AscentClient/FirmwareUpgradeFlow
// without real hardware. OnSend is invoked for every packet written and
// decides what (if anything) comes back — return null to simulate "device
// didn't respond", driving AscentClient's retry path.
public class FakeAscentTransport : IAscentTransport
{
    private readonly Queue<byte> _inbound = new();

    public ManualClock? Clock { get; set; }
    public TimeSpan SimulatedReadTimeout { get; set; } = TimeSpan.FromMilliseconds(ArConstantsV2.ACK_RETRY_INTERVAL_MS);
    public Func<SentPacket, byte[]?>? OnSend { get; set; }
    public List<SentPacket> SentPackets { get; } = new();

    public void Write(byte[] buffer, int offset, int count)
    {
        byte[] raw = buffer.AsSpan(offset, count).ToArray();
        ArProtocolHeaderV2 header = ArPacketCodecV2.ParseHeader(raw);
        byte[] payload = raw.Length > ArConstantsV2.HEADER_LENGTH
            ? raw[ArConstantsV2.HEADER_LENGTH..]
            : Array.Empty<byte>();
        var sent = new SentPacket(header.command, header.seq, header.retry, payload);
        SentPackets.Add(sent);

        byte[]? response = OnSend?.Invoke(sent);
        if (response != null)
        {
            foreach (byte b in response)
            {
                _inbound.Enqueue(b);
            }
        }
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        if (_inbound.Count == 0)
        {
            // Model a real SerialPort blocking for its ReadTimeout before giving up.
            Clock?.Advance(SimulatedReadTimeout);
            throw new TimeoutException("fake transport: no data available");
        }
        int n = Math.Min(count, _inbound.Count);
        for (int i = 0; i < n; i++)
        {
            buffer[offset + i] = _inbound.Dequeue();
        }
        return n;
    }

    public void Dispose()
    {
    }

    public static byte[] BuildResponse(uint cmd, uint seq, byte[]? payload = null)
        => ArPacketCodecV2.BuildPacket(cmd, ArConstantsV2.MSGTYPE_ACK, ArConstantsV2.FMT_BINARY,
            userId: 0, payload: payload, len: (uint)(payload?.Length ?? 0), seq: seq, retry: 0);
}
