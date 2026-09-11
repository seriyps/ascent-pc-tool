using CaddxTool.Protocol;

namespace CaddxTool.FakeDevice;

// Writes one line per frame in a format meant to be diffed between two capture
// runs (e.g. official Windows app vs. our native port talking to the same fake
// device) — no timestamps, so `diff official.log native.log` only shows real
// protocol differences. SENDFILE_DATA payloads are summarized (length only,
// not hex-dumped) since their content is just raw firmware-file bytes,
// identical by construction on both sides and not useful to diff byte-by-byte.
public sealed class FrameLogger : IDisposable
{
    private readonly StreamWriter? _file;

    public FrameLogger(string? logPath)
    {
        _file = logPath != null ? new StreamWriter(logPath, append: false) { AutoFlush = true } : null;
    }

    public void Log(string direction, ArProtocolHeaderV2 header, byte[] payload)
    {
        string name = CommandName(header.command);
        string payloadField = header.command == ArConstantsV2.CMD_SENDFILE_DATA
            ? $"<{payload.Length} bytes, crc32={ArPacketCodecV2.CrcCalc(null, payload):X8}>"
            : Convert.ToHexString(payload);

        Console.WriteLine($"{direction} cmd={header.command}({name}) seq={header.seq} retry={header.retry} len={header.length}");
        _file?.WriteLine($"{direction} cmd={header.command} seq={header.seq} retry={header.retry} len={header.length} payload={payloadField}");
    }

    public void Note(string message)
    {
        Console.WriteLine("  -> " + message);
        _file?.WriteLine("# " + message);
    }

    private static string CommandName(uint cmd) => cmd switch
    {
        ArConstantsV2.CMD_FIND_DEVICE => "FIND_DEVICE",
        ArConstantsV2.CMD_REBOOT => "REBOOT",
        ArConstantsV2.CMD_REMOTE_UPGRADE => "REMOTE_UPGRADE",
        ArConstantsV2.CMD_SENDFILE_START => "SENDFILE_START",
        ArConstantsV2.CMD_SENDFILE_DATA => "SENDFILE_DATA",
        ArConstantsV2.CMD_SENDFILE_END => "SENDFILE_END",
        ArConstantsV2.CMD_UPGRADE_STATUS => "UPGRADE_STATUS",
        _ => "?",
    };

    public void Dispose() => _file?.Dispose();
}
