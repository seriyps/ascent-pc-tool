using System;
using System.Threading;

namespace CaddxTool.Protocol;

// Synchronous client over IAscentTransport. Ported request/ack semantics from
// ArTransportV2: same seq resent on every read timeout, give up once the
// total ack timeout elapses. Deliberately synchronous (not the original's
// send-queue/recv-loop/event architecture) since this is a GUI tool driving
// one request at a time, not a background service — see AGENTS.md/PROJECT.md
// for why a mechanical-but-not-byte-identical port is fine here.
public class AscentClient : IDisposable
{
    private readonly IAscentTransport _transport;
    private readonly Func<DateTime> _now;
    private uint _seq = 1;

    public AscentClient(IAscentTransport transport, Func<DateTime>? now = null)
    {
        _transport = transport;
        _now = now ?? (() => DateTime.Now);
    }

    public static AscentClient OpenSerial(string portName, int baudRate = 115200)
        => new(new SerialPortAscentTransport(portName, baudRate));

    // Fire-and-forget send, no ack expected — used for the reboot command,
    // mirroring ArTransportV2.SendNoAck.
    public void SendNoAck(uint cmd, byte[]? payload)
    {
        uint len = (uint)(payload?.Length ?? 0);
        uint seq = _seq++;
        byte[] packet = ArPacketCodecV2.BuildPacket(cmd, ArConstantsV2.MSGTYPE_REQUEST,
            ArConstantsV2.FMT_BINARY, userId: 0, payload: payload, len: len, seq: seq, retry: 0);
        _transport.Write(packet, 0, packet.Length);
    }

    public ResDeviceInfoV2 GetDeviceInfo()
    {
        AckResult ack = SendWithAck(ArConstantsV2.CMD_FIND_DEVICE, payload: null);
        if (!ack.IsSuccess)
        {
            throw new TimeoutException("FIND_DEVICE timed out");
        }
        return ResDeviceInfoV2.Parse(ack.RawPayload ?? Array.Empty<byte>());
    }

    // Sends `cmd` and waits for a response whose header `command` field matches
    // `expectedAckCmd` (defaults to `cmd`). Resends the identical packet (same
    // seq, incrementing retry count) every time the transport's read times
    // out, until `totalTimeoutMs` has elapsed overall — mirrors
    // ArTransportV2.SendWithAckAsync/StartAckLoop, except the per-attempt
    // retry interval is whatever read timeout the transport itself is
    // configured with (SerialPortAscentTransport defaults to 2000ms, matching
    // ArConstantsV2.ACK_RETRY_INTERVAL_MS) rather than being independently
    // configurable here.
    public AckResult SendWithAck(uint cmd, byte[]? payload, uint expectedAckCmd = 0,
        int totalTimeoutMs = ArConstantsV2.ACK_TOTAL_TIMEOUT_MS, CancellationToken ct = default)
    {
        uint expected = expectedAckCmd == 0 ? cmd : expectedAckCmd;
        uint len = (uint)(payload?.Length ?? 0);
        uint seq = _seq++;
        uint retryCount = 0;

        byte[] packet = ArPacketCodecV2.BuildPacket(cmd, ArConstantsV2.MSGTYPE_REQUEST,
            ArConstantsV2.FMT_BINARY, userId: 0, payload: payload, len: len, seq: seq, retry: retryCount);
        _transport.Write(packet, 0, packet.Length);

        DateTime start = _now();
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            if ((_now() - start).TotalMilliseconds >= totalTimeoutMs)
            {
                return new AckResult { Status = AckStatus.Timeout, Cmd = expected, Seq = seq, RetryCount = retryCount };
            }

            byte[] header;
            try
            {
                header = ReadExact(ArConstantsV2.HEADER_LENGTH);
            }
            catch (TimeoutException)
            {
                retryCount++;
                byte[] resend = ArPacketCodecV2.BuildPacket(cmd, ArConstantsV2.MSGTYPE_REQUEST,
                    ArConstantsV2.FMT_BINARY, userId: 0, payload: payload, len: len, seq: seq, retry: retryCount);
                _transport.Write(resend, 0, resend.Length);
                continue;
            }

            var parsedHeader = ArPacketCodecV2.ParseHeader(header);
            if (parsedHeader.magic != ArConstantsV2.MAGIC)
            {
                throw new InvalidOperationException($"bad magic in response: 0x{parsedHeader.magic:X8}");
            }
            byte[] responsePayload = parsedHeader.length > 0 ? ReadExact((int)parsedHeader.length) : Array.Empty<byte>();

            if (parsedHeader.command != expected || parsedHeader.seq != seq)
            {
                // Unrelated/stale packet — keep waiting for the one we asked for.
                continue;
            }

            return new AckResult
            {
                Status = AckStatus.Matched,
                Cmd = parsedHeader.command,
                Seq = parsedHeader.seq,
                RetryCount = retryCount,
                RawPayload = responsePayload,
            };
        }
    }

    private byte[] ReadExact(int count)
    {
        byte[] buf = new byte[count];
        int offset = 0;
        while (offset < count)
        {
            int read = _transport.Read(buf, offset, count - offset);
            if (read <= 0)
            {
                throw new TimeoutException("transport read timed out / returned no data");
            }
            offset += read;
        }
        return buf;
    }

    public void Dispose() => _transport.Dispose();
}
