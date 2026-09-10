using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaddxTool.Protocol;

public record FlowProgress(string StepName, double Percent, string Message);

public record FlowResult(bool Success, string Message)
{
    public static FlowResult Ok(string message) => new(true, message);
    public static FlowResult Fail(string message) => new(false, message);
}

// Mechanical port of Caddx_PCTool.FirmwareUpgradeFlowV2.RunAsync — see
// src/PROJECT.md "Safety principle" for why this stays close to the original
// step order/commands/timeouts rather than being redesigned. Owns the
// transport across the flow's two device reboots: closes it, waits the
// device-specific reboot delay (ArConstantsV2.RebootWaitByDeviceName), then
// polls device discovery until the device reappears and reopens.
public class FirmwareUpgradeFlow
{
    private readonly string _vid;
    private readonly string _deviceName;
    private readonly Func<string, IAscentTransport> _transportFactory;
    private readonly Func<IReadOnlyList<CandidatePort>> _scanCandidates;

    private IAscentTransport _transport;

    // Test seams — real callers leave these at their defaults.
    public Func<int, CancellationToken, Task> DelayFunc { get; set; } = (ms, ct) => Task.Delay(ms, ct);
    public Func<DateTime> Now { get; set; } = () => DateTime.Now;

    public FirmwareUpgradeFlow(
        IAscentTransport initialTransport,
        string vid,
        string deviceName,
        Func<string, IAscentTransport>? transportFactory = null,
        Func<IReadOnlyList<CandidatePort>>? scanCandidates = null)
    {
        _transport = initialTransport;
        _vid = vid;
        _deviceName = deviceName;
        _transportFactory = transportFactory ?? (portName => new SerialPortAscentTransport(portName));
        _scanCandidates = scanCandidates ?? AscentDeviceFinder.FindCandidates;
    }

    public async Task<FlowResult> RunAsync(string filePath, IProgress<FlowProgress>? progress, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return FlowResult.Fail($"firmware file not found: {filePath}");
        }

        try
        {
            var client = new AscentClient(_transport, Now);

            Report(progress, "FindDevice", 0.02, "Querying device info");
            ResDeviceInfoV2 deviceInfo = client.GetDeviceInfo();
            int chunkSize = deviceInfo.ReceiveMaxSize > 0 ? deviceInfo.ReceiveMaxSize : 1_048_576;

            Report(progress, "RebootClean", 0.08, "Rebooting into clean mode");
            client.SendNoAck(ArConstantsV2.CMD_REBOOT, BuildRebootModeBytes("clean"));
            client = await ReconnectAsync(ct);

            Report(progress, "RemoteUpgrade", 0.13, "Entering upgrade mode");
            EnsureSuccess(client.SendWithAck(ArConstantsV2.CMD_REMOTE_UPGRADE, null, ct: ct), "REMOTE_UPGRADE failed");

            Report(progress, "SendFileStart", 0.17, "Starting file transfer");
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath, ct);
            string md5Hex = Convert.ToHexString(MD5.HashData(fileBytes)).ToLowerInvariant();
            string remotePath = "/tmp/pc/" + Path.GetFileName(filePath);
            byte[] fileInfoPayload = ArFileInfoCodec.Build(md5Hex, fileBytes.Length, remotePath, filePath);
            EnsureSuccess(client.SendWithAck(ArConstantsV2.CMD_SENDFILE_START, fileInfoPayload, ct: ct), "SENDFILE_START failed");

            Report(progress, "SendFileData", 0.17, "Transferring firmware");
            int sent = 0;
            while (sent < fileBytes.Length)
            {
                ct.ThrowIfCancellationRequested();
                int len = Math.Min(chunkSize, fileBytes.Length - sent);
                byte[] chunk = fileBytes.AsSpan(sent, len).ToArray();
                EnsureSuccess(client.SendWithAck(ArConstantsV2.CMD_SENDFILE_DATA, chunk, ct: ct), "SENDFILE_DATA failed");
                sent += len;
                Report(progress, "SendFileData", 0.17 + 0.43 * sent / fileBytes.Length, "Transferring firmware");
            }

            Report(progress, "SendFileEnd", 0.62, "Finishing transfer");
            AckResult endAck = client.SendWithAck(ArConstantsV2.CMD_SENDFILE_END, null, ct: ct);
            EnsureSuccess(endAck, "SENDFILE_END failed");
            ResFileDataInfoV2 fileEndInfo = ResFileDataInfoV2.Parse(endAck.RawPayload ?? Array.Empty<byte>());
            if (fileEndInfo.Status != 0 || fileEndInfo.DetailText != "OK")
            {
                throw new InvalidOperationException(
                    string.IsNullOrEmpty(fileEndInfo.DetailText) ? "SENDFILE_END reported failure" : fileEndInfo.DetailText);
            }

            Report(progress, "UpgradeStatus", 0.65, "Polling upgrade status");
            await PollUpgradeStatusAsync(client, progress, ct);

            Report(progress, "Reboot", 0.98, "Rebooting device");
            await ReconnectAsync(ct);

            Report(progress, "Done", 1.0, "Upgrade complete");
            return FlowResult.Ok("Upgrade complete");
        }
        catch (OperationCanceledException)
        {
            return FlowResult.Fail("Cancelled");
        }
        catch (Exception ex)
        {
            return FlowResult.Fail(ex.Message);
        }
        finally
        {
            _transport.Dispose();
        }
    }

    private async Task PollUpgradeStatusAsync(AscentClient client, IProgress<FlowProgress>? progress, CancellationToken ct)
    {
        for (int i = 0; i < 200; i++)
        {
            await DelayFunc(1000, ct);
            AckResult ack = client.SendWithAck(ArConstantsV2.CMD_UPGRADE_STATUS, null, ct: ct);
            EnsureSuccess(ack, "UPGRADE_STATUS failed");
            ResUpgradeStatusV2 status = ResUpgradeStatusV2.Parse(ack.RawPayload ?? Array.Empty<byte>());
            string? errorDesc = JudgeUpgradeStatus(status);
            if (errorDesc != null)
            {
                throw new InvalidOperationException(errorDesc);
            }
            double percent = Math.Clamp(status.Percent, 0, 99);
            Report(progress, "UpgradeStatus", 0.65 + percent * 0.0034, "Flashing firmware");
            if (status.Percent > 99)
            {
                return;
            }
        }
        throw new TimeoutException("upgrade status query limit exceeded");
    }

    // Ported verbatim from FirmwareUpgradeFlowV2.JudgeUpgradeStatus. Returns
    // null when the upgrade is still progressing/succeeded, else the
    // user-facing error message.
    private static string? JudgeUpgradeStatus(ResUpgradeStatusV2 status) => (UpgradeStatusErrorCode)status.Status switch
    {
        UpgradeStatusErrorCode.STAT_VERIFY_IMAGE => null,
        UpgradeStatusErrorCode.UPGRATE_ERR_NO_SD => "Upgrade failed, no SD card detected",
        UpgradeStatusErrorCode.UPGRATE_ERR_NO_PC => "Upgrade failed, no computer connected",
        UpgradeStatusErrorCode.UPGRATE_ERR_NO_FILE => "Upgrade failed, no upgrade file found",
        UpgradeStatusErrorCode.UPGRATE_ERR_BAD_FILE => "Upgrade failed, upgrade file is corrupted",
        UpgradeStatusErrorCode.UPGRATE_ERR_BOARD_TYPE => "Upgrade failed, the device model does not match the upgraded firmware",
        UpgradeStatusErrorCode.UPGRATE_ERR_APP_VERSION => "Upgrade failed, The firmware version number is too low",
        UpgradeStatusErrorCode.UPGRATE_ERR_UPGRADE_MODE => "Upgrade failed, invalid upgrade mode",
        UpgradeStatusErrorCode.UPGRATE_ERR_IMG_TYPE => "Upgrade failed, image type error",
        UpgradeStatusErrorCode.UPGRATE_ERR_IMG_CRC => "Upgrade failed, image CRC error",
        UpgradeStatusErrorCode.UPGRATE_ERR_MAX => "Upgrade failed, unknown error",
        UpgradeStatusErrorCode.UPGRATE_ERR_FLASH_ERR => "Upgrade failed, flash error",
        UpgradeStatusErrorCode.UPGRATE_ERR_CHIPID => "Upgrade failed, chip ID error",
        _ => status.Status >= 0 ? null : "Upgrade failed, unknown error",
    };

    private async Task<AscentClient> ReconnectAsync(CancellationToken ct)
    {
        _transport.Dispose();

        int rebootWaitMs = ArConstantsV2.RebootWaitByDeviceName.TryGetValue(_deviceName, out int w)
            ? w
            : ArConstantsV2.REBOOT_WAIT_DEFAULT_MS;
        await DelayFunc(rebootWaitMs, ct);

        DateTime start = Now();
        while ((Now() - start).TotalMilliseconds < ArConstantsV2.RECONNECT_TIMEOUT_MS)
        {
            ct.ThrowIfCancellationRequested();
            CandidatePort? match = _scanCandidates()
                .FirstOrDefault(c => string.Equals(c.Vid, _vid, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                try
                {
                    _transport = _transportFactory(match.PortName);
                    return new AscentClient(_transport, Now);
                }
                catch
                {
                    // Port enumerated but not ready to open yet — keep polling.
                }
            }
            await DelayFunc(ArConstantsV2.RECONNECT_POLL_INTERVAL_MS, ct);
        }
        throw new TimeoutException($"device did not reappear within {ArConstantsV2.RECONNECT_TIMEOUT_MS}ms after reboot");
    }

    private static void EnsureSuccess(AckResult ack, string message)
    {
        if (!ack.IsSuccess)
        {
            throw new TimeoutException(message);
        }
    }

    private static byte[] BuildRebootModeBytes(string mode)
    {
        byte[] bytes = string.IsNullOrEmpty(mode) ? new byte[32] : Encoding.ASCII.GetBytes(mode);
        Array.Resize(ref bytes, 32);
        return bytes;
    }

    private static void Report(IProgress<FlowProgress>? progress, string stepName, double percent, string message)
        => progress?.Report(new FlowProgress(stepName, percent, message));
}
