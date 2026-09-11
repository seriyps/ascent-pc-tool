using System.Text;
using CaddxTool.Protocol;
using CaddxTool.FakeDevice;

if (args.Length < 1 || args[0] is "-h" or "--help")
{
    Console.Error.WriteLine("Usage: CaddxTool.FakeDevice <serial-port-or-pty-path> [--device lite_plus|vrx_pro|gt_pro] [--log <path>]");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Simulates an Ascent device's wire protocol (FIND_DEVICE, REBOOT,");
    Console.Error.WriteLine("REMOTE_UPGRADE, SENDFILE_START/DATA/END, UPGRADE_STATUS) so a real client");
    Console.Error.WriteLine("(the official Windows app under Wine, or this project's own native app) can");
    Console.Error.WriteLine("run a full firmware-upgrade flow against it with no real hardware involved.");
    Console.Error.WriteLine("Intended use: pair with a socat PTY pair, point one end at this tool and the");
    Console.Error.WriteLine("other at the client under test, then compare --log output between runs.");
    return 1;
}

string portPath = args[0];
string profileName = "lite_plus";
string? logPath = null;
for (int i = 1; i < args.Length; i++)
{
    if (args[i] == "--device" && i + 1 < args.Length) { profileName = args[++i]; }
    else if (args[i] == "--log" && i + 1 < args.Length) { logPath = args[++i]; }
}

FakeDeviceProfile profile = FakeDeviceProfile.Named(profileName);
Console.WriteLine($"Fake Ascent device: {profile.DeviceName} (VID={profile.Vid}) listening on {portPath}");
Console.WriteLine("Waiting for requests... (Ctrl+C to stop)");

bool running = true;
Console.CancelKeyPress += (_, e) => { e.Cancel = true; running = false; };

using var transport = new SerialPortAscentTransport(portPath);
using var log = new FrameLogger(logPath);

int upgradeStatusPercent = 0;

try
{
    while (running)
    {
        byte[] header;
        try
        {
            header = ReadExact(transport, ArConstantsV2.HEADER_LENGTH, () => running);
        }
        catch (OperationCanceledException)
        {
            break;
        }

        var h = ArPacketCodecV2.ParseHeader(header);
        if (h.magic != ArConstantsV2.MAGIC)
        {
            log.Note($"bad magic 0x{h.magic:X8} — ignoring, resyncing on next header-sized read");
            continue;
        }

        byte[] payload = h.length > 0 ? ReadExact(transport, (int)h.length, () => running) : Array.Empty<byte>();
        log.Log("RX", h, payload);

        byte[]? responsePayload;
        bool sendResponse = true;

        switch (h.command)
        {
            case ArConstantsV2.CMD_REBOOT:
                string mode = Encoding.ASCII.GetString(payload).TrimEnd('\0');
                log.Note($"reboot requested (mode=\"{mode}\") — no ack expected; 'restarting' silently");
                upgradeStatusPercent = 0;
                sendResponse = false;
                responsePayload = null;
                break;

            case ArConstantsV2.CMD_FIND_DEVICE:
                responsePayload = ResDeviceInfoV2.Build(
                    profile.ReceiveMaxSize, profile.SdkVersion, profile.DeviceName, profile.CpuTemp,
                    profile.FirmwareInfo, profile.SerialNumber, profile.HardwareVersion);
                break;

            case ArConstantsV2.CMD_REMOTE_UPGRADE:
            case ArConstantsV2.CMD_SENDFILE_START:
                responsePayload = Array.Empty<byte>();
                break;

            case ArConstantsV2.CMD_SENDFILE_DATA:
                // Unlike REMOTE_UPGRADE/SENDFILE_START, the real app's ack handler
                // (UsbSerialportFSM.HandlePacket, AR_CMD_SENDFILE_DATA case) parses this
                // ack's payload via ParseDataInfo(payload) — same structured
                // ResFileDataInfo layout as SENDFILE_END. An empty payload makes that
                // parse throw (BitConverter.ToInt32 on a 0-length array), and since
                // HandlePacket swallows the exception in a catch-and-log, the ack never
                // reaches the app's ack-matching logic at all — it just silently
                // vanishes, and the sender retries/times out no matter how long it
                // waits. Confirmed empirically against the real official app under Wine.
                responsePayload = ResFileDataInfoV2.Build(payload.Length, payload.Length, payload.Length, status: 0, detail: "OK");
                break;

            case ArConstantsV2.CMD_SENDFILE_END:
                responsePayload = ResFileDataInfoV2.Build(payload.Length, payload.Length, payload.Length, status: 0, detail: "OK");
                break;

            case ArConstantsV2.CMD_UPGRADE_STATUS:
                upgradeStatusPercent = Math.Min(100, upgradeStatusPercent + 40);
                responsePayload = ResUpgradeStatusV2.Build(upgradeStatusPercent, status: 0);
                break;

            default:
                log.Note($"unhandled cmd {h.command} — not responding");
                sendResponse = false;
                responsePayload = null;
                break;
        }

        if (sendResponse)
        {
            byte[] response = ArPacketCodecV2.BuildPacket(h.command, ArConstantsV2.MSGTYPE_ACK, ArConstantsV2.FMT_BINARY,
                userId: 0, payload: responsePayload, len: (uint)(responsePayload?.Length ?? 0), seq: h.seq, retry: 0);
            transport.Write(response, 0, response.Length);
            log.Log("TX", ArPacketCodecV2.ParseHeader(response), responsePayload ?? Array.Empty<byte>());
        }
    }
}
finally
{
    Console.WriteLine("Fake device stopped.");
}

return 0;

static byte[] ReadExact(IAscentTransport transport, int count, Func<bool> shouldContinue)
{
    byte[] buf = new byte[count];
    int offset = 0;
    while (offset < count)
    {
        if (!shouldContinue())
        {
            throw new OperationCanceledException();
        }
        int n;
        try
        {
            n = transport.Read(buf, offset, count - offset);
        }
        catch (TimeoutException)
        {
            continue;
        }
        offset += n;
    }
    return buf;
}
