using System.Linq;
using System.Text;

namespace CaddxTool.Protocol.Tests;

public class FirmwareUpgradeFlowTests
{
    private const string Vid = "1d76";
    private const string DeviceName = "ascent_lite_plus";

    // Mirrors ResDeviceInfoV2's 300-byte layout — only ReceiveMaxSize matters here.
    private static byte[] BuildDeviceInfoPayload(int receiveMaxSize)
    {
        byte[] buf = new byte[300];
        BitConverter.GetBytes(receiveMaxSize).CopyTo(buf, 0);
        return buf;
    }

    // Mirrors ResFileDataInfoV2's layout: Length,Cursize,Totalsize,Status:int32x4, Detail[64].
    private static byte[] BuildFileEndPayload(int status, string detail)
    {
        byte[] buf = new byte[16 + 64];
        BitConverter.GetBytes(status).CopyTo(buf, 12);
        Encoding.ASCII.GetBytes(detail).CopyTo(buf, 16);
        return buf;
    }

    // Mirrors ResUpgradeStatusV2's layout: Percent,Status:int32x2, Detail[64].
    private static byte[] BuildUpgradeStatusPayload(int percent, int status)
    {
        byte[] buf = new byte[8 + 64];
        BitConverter.GetBytes(percent).CopyTo(buf, 0);
        BitConverter.GetBytes(status).CopyTo(buf, 4);
        return buf;
    }

    private static (FirmwareUpgradeFlow flow, ManualClock clock, FakeAscentTransport initial, FakeAscentTransport postFirstReboot, FakeAscentTransport final)
        BuildHarness(int receiveMaxSize, Func<SentPacket, byte[]?> postRebootHandler)
    {
        var clock = new ManualClock();

        var initial = new FakeAscentTransport { Clock = clock };
        initial.OnSend = sent => sent.Cmd == ArConstantsV2.CMD_FIND_DEVICE
            ? FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq, BuildDeviceInfoPayload(receiveMaxSize))
            : null; // CMD_REBOOT is fire-and-forget, no response expected

        var postFirstReboot = new FakeAscentTransport { Clock = clock, OnSend = postRebootHandler };
        var final = new FakeAscentTransport { Clock = clock };

        int transportFactoryCalls = 0;
        var flow = new FirmwareUpgradeFlow(
            initial,
            vid: Vid,
            deviceName: DeviceName,
            transportFactory: _ => transportFactoryCalls++ == 0 ? postFirstReboot : final,
            scanCandidates: () => new[] { new CandidatePort("/dev/ttyACM1", Vid, "0101") });

        // Advance the shared clock deterministically instead of really waiting —
        // keeps reboot-wait/reconnect-poll/ack-retry timeouts test-fast while still
        // exercising the real timeout arithmetic.
        flow.DelayFunc = (ms, _) => { clock.Advance(TimeSpan.FromMilliseconds(ms)); return Task.CompletedTask; };
        flow.Now = clock.Now;

        return (flow, clock, initial, postFirstReboot, final);
    }

    private static string WriteTempFirmware(int size)
    {
        string path = Path.GetTempFileName();
        File.WriteAllBytes(path, new byte[size]);
        return path;
    }

    [Fact]
    public async Task RunAsync_HappyPath_CompletesAndReportsDone()
    {
        int uploadStatusPolls = 0;
        var (flow, _, _, postFirstReboot, _) = BuildHarness(receiveMaxSize: 65536, postRebootHandler: sent => sent.Cmd switch
        {
            ArConstantsV2.CMD_REMOTE_UPGRADE => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq),
            ArConstantsV2.CMD_SENDFILE_START => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq),
            ArConstantsV2.CMD_SENDFILE_DATA => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq),
            ArConstantsV2.CMD_SENDFILE_END => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq, BuildFileEndPayload(0, "OK")),
            ArConstantsV2.CMD_UPGRADE_STATUS => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq,
                BuildUpgradeStatusPayload(percent: ++uploadStatusPolls >= 3 ? 100 : uploadStatusPolls * 40, status: 0)),
            _ => null,
        });

        string firmwarePath = WriteTempFirmware(200_000); // > one 65536 chunk, exercises the chunk loop
        try
        {
            var progress = new SyncProgress<FlowProgress>();
            FlowResult result = await flow.RunAsync(firmwarePath, progress);

            Assert.True(result.Success, result.Message);
            Assert.Contains(progress.Reports, p => p.StepName == "Done" && p.Percent >= 1.0);

            // 200000 bytes / 65536-byte chunks = 4 chunks (3 full + 1 partial).
            int chunkCount = postFirstReboot.SentPackets.Count(p => p.Cmd == ArConstantsV2.CMD_SENDFILE_DATA);
            Assert.Equal(4, chunkCount);
        }
        finally
        {
            File.Delete(firmwarePath);
        }
    }

    [Fact]
    public async Task RunAsync_BoardTypeMismatch_FailsWithPortedMessage()
    {
        var (flow, _, _, _, _) = BuildHarness(receiveMaxSize: 65536, postRebootHandler: sent => sent.Cmd switch
        {
            ArConstantsV2.CMD_REMOTE_UPGRADE => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq),
            ArConstantsV2.CMD_SENDFILE_START => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq),
            ArConstantsV2.CMD_SENDFILE_DATA => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq),
            ArConstantsV2.CMD_SENDFILE_END => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq, BuildFileEndPayload(0, "OK")),
            ArConstantsV2.CMD_UPGRADE_STATUS => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq,
                BuildUpgradeStatusPayload(percent: 0, status: (int)UpgradeStatusErrorCode.UPGRATE_ERR_BOARD_TYPE)),
            _ => null,
        });

        string firmwarePath = WriteTempFirmware(1024);
        try
        {
            FlowResult result = await flow.RunAsync(firmwarePath, progress: null);

            Assert.False(result.Success);
            Assert.Equal("Upgrade failed, the device model does not match the upgraded firmware", result.Message);
        }
        finally
        {
            File.Delete(firmwarePath);
        }
    }

    [Fact]
    public async Task RunAsync_SendFileEndReportsFailureDetail_FailsWithThatDetail()
    {
        var (flow, _, _, _, _) = BuildHarness(receiveMaxSize: 65536, postRebootHandler: sent => sent.Cmd switch
        {
            ArConstantsV2.CMD_REMOTE_UPGRADE => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq),
            ArConstantsV2.CMD_SENDFILE_START => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq),
            ArConstantsV2.CMD_SENDFILE_DATA => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq),
            ArConstantsV2.CMD_SENDFILE_END => FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq, BuildFileEndPayload(1, "MD5_MISMATCH")),
            _ => null,
        });

        string firmwarePath = WriteTempFirmware(1024);
        try
        {
            FlowResult result = await flow.RunAsync(firmwarePath, progress: null);

            Assert.False(result.Success);
            Assert.Equal("MD5_MISMATCH", result.Message);
        }
        finally
        {
            File.Delete(firmwarePath);
        }
    }

    [Fact]
    public async Task RunAsync_DeviceNeverReappearsAfterReboot_FailsWithTimeout()
    {
        var clock = new ManualClock();
        var initial = new FakeAscentTransport { Clock = clock };
        initial.OnSend = sent => sent.Cmd == ArConstantsV2.CMD_FIND_DEVICE
            ? FakeAscentTransport.BuildResponse(sent.Cmd, sent.Seq, BuildDeviceInfoPayload(65536))
            : null;

        var flow = new FirmwareUpgradeFlow(
            initial,
            vid: Vid,
            deviceName: DeviceName,
            transportFactory: _ => throw new InvalidOperationException("should never be reached — device never reappears"),
            scanCandidates: () => Array.Empty<CandidatePort>());
        flow.DelayFunc = (ms, _) => { clock.Advance(TimeSpan.FromMilliseconds(ms)); return Task.CompletedTask; };
        flow.Now = clock.Now;

        string firmwarePath = WriteTempFirmware(1024);
        try
        {
            FlowResult result = await flow.RunAsync(firmwarePath, progress: null);

            Assert.False(result.Success);
            Assert.Contains("did not reappear", result.Message);
        }
        finally
        {
            File.Delete(firmwarePath);
        }
    }

    [Fact]
    public async Task RunAsync_MissingFile_FailsFastWithoutTouchingTransport()
    {
        var (flow, _, initial, _, _) = BuildHarness(receiveMaxSize: 65536, postRebootHandler: _ => null);

        FlowResult result = await flow.RunAsync("/nonexistent/firmware.bin", progress: null);

        Assert.False(result.Success);
        Assert.Empty(initial.SentPackets);
    }
}
