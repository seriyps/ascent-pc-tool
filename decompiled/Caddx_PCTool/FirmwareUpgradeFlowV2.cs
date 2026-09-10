using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caddx_PCTool;

public class FirmwareUpgradeFlowV2 : IDisposable
{
	private readonly ArTransportV2 _transport;

	private CancellationTokenSource _cts;

	private FileTransferSession _fileSession;

	private int _chunkSize = 1048576;

	public event EventHandler<FlowProgressEventArgsV2> OnProgress;

	public event EventHandler<FlowDeviceEventArgsV2> OnDeviceInfo;

	public event EventHandler<FlowResultEventArgsV2> OnCompleted;

	public FirmwareUpgradeFlowV2(ArTransportV2 transport)
	{
		_transport = transport ?? throw new ArgumentNullException("transport");
	}

	public void Start(string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
		{
			OnCompleted?.Invoke(this, new FlowResultEventArgsV2
			{
				Success = false,
				Message = Lang.T("v2.firmware_path_error"),
				ErrorDetail = filePath
			});
			return;
		}
		Cancel();
		_cts = new CancellationTokenSource();
		_fileSession = new FileTransferSession(filePath, "/tmp/pc/" + Path.GetFileName(filePath), new FileStream(filePath, FileMode.Open, FileAccess.Read), _chunkSize);
		Task.Run(() => RunAsync(_cts.Token), _cts.Token);
	}

	public void Cancel()
	{
		_cts?.Cancel();
	}

	private async Task RunAsync(CancellationToken ct)
	{
		try
		{
			await RebootCleanAsync(ct);
			await RemoteUpgradeAsync(ct);
			await SendFileStartAsync(ct);
			await SendFileDataLoopAsync(ct);
			await SendFileEndAsync(ct);
			await PollUpgradeStatusAsync(ct);
			await FinalRebootAsync(ct);
			OnCompleted?.Invoke(this, new FlowResultEventArgsV2
			{
				Success = true,
				Message = Lang.T("v2.upgrade_complete")
			});
		}
		catch (OperationCanceledException)
		{
			OnCompleted?.Invoke(this, new FlowResultEventArgsV2
			{
				Success = false,
				Message = Lang.T("v2.cancelled")
			});
		}
		catch (Exception ex2)
		{
			Exception ex3 = ex2;
			OnCompleted?.Invoke(this, new FlowResultEventArgsV2
			{
				Success = false,
				Message = ex3.Message,
				ErrorDetail = ex3.ToString()
			});
		}
		finally
		{
			_fileSession?.Dispose();
			_fileSession = null;
		}
	}

	protected virtual Task DelayUpgradeStatusPollAsync(CancellationToken ct)
	{
		return Task.Delay(1000, ct);
	}

	private async Task FindDeviceAsync(CancellationToken ct)
	{
		ReportProgress("FindDevice", 0.02f, Lang.T("v2.query_device_info"));
		AckResultV2 ack = await _transport.SendWithAckAsync(60u, null, 0u, 0u, 0, ct);
		EnsureSuccess(ack, "FIND_DEVICE failed");
		ResDeviceInfoV2 dev = ArPacketCodecV2.ParseDeviceInfo(ack.RawPayload ?? Array.Empty<byte>());
		_chunkSize = Math.Max(1, dev.receiveMaxSize);
		ResAscentInfo info = new ResAscentInfo
		{
			SN = Encoding.ASCII.GetString(dev.serialNumber).TrimEnd(new char[1]),
			FWVers = Encoding.ASCII.GetString(dev.firmwareInfo).TrimEnd(new char[1]),
			HWVers = Encoding.ASCII.GetString(dev.hardwareVersion).TrimEnd(new char[1]),
			MCUTemp = dev.cputemp,
			DevName = Encoding.ASCII.GetString(dev.devicename).TrimEnd(new char[1]),
			SDKVers = Encoding.ASCII.GetString(dev.sdkversion).TrimEnd(new char[1]),
			Details = Encoding.ASCII.GetString(dev.detail).TrimEnd(new char[1]),
			RecMaxSize = dev.receiveMaxSize,
			UsbInfo = _transport.UsbInfo,
			UsbTime = (_transport.UsbInfo?.ConnectedTime ?? DateTime.Now)
		};
		OnDeviceInfo?.Invoke(this, new FlowDeviceEventArgsV2
		{
			DeviceInfo = info
		});
	}

	private async Task RebootCleanAsync(CancellationToken ct)
	{
		ReportProgress("RebootClean", 0.08f, Lang.T("v2.reboot_clean"));
		if (!(await _transport.ReconnectAfterRebootAsync("clean", GD.Inst.ReOpenTimeout_Asce, ct)))
		{
			throw new InvalidOperationException("reboot clean reconnect timeout");
		}
	}

	private async Task RemoteUpgradeAsync(CancellationToken ct)
	{
		ReportProgress("RemoteUpgrade", 0.13f, Lang.T("v2.enter_upgrade_mode"));
		EnsureSuccess(await _transport.SendWithAckAsync(114u, null, 0u, 0u, 0, ct), "REMOTE_UPGRADE failed");
	}

	private async Task SendFileStartAsync(CancellationToken ct)
	{
		ReportProgress("SendFileStart", 0.17f, Lang.T("v2.start_transfer"));
		ArFileInfo fileInfo = _fileSession.BuildFileInfo();
		byte[] bytes = ArPacketCodecV2.StructToBytes(fileInfo);
		EnsureSuccess(await _transport.SendWithAckAsync(115u, bytes, (uint)bytes.Length, 0u, 0, ct), "SENDFILE_START failed");
	}

	private async Task SendFileDataLoopAsync(CancellationToken ct)
	{
		while (!_fileSession.IsComplete)
		{
			byte[] chunk = _fileSession.GetNextChunk(out var length);
			if (chunk == null)
			{
				break;
			}
			EnsureSuccess(await _transport.SendWithAckAsync(116u, chunk, (uint)length, 0u, 0, ct), "SENDFILE_DATA failed");
			_fileSession.Advance();
			ReportProgress("SendFileData", 0.17f + 0.43f * _fileSession.Progress, Lang.T("v2.transfer_in_progress"));
		}
	}

	private async Task SendFileEndAsync(CancellationToken ct)
	{
		ReportProgress("SendFileEnd", 0.62f, Lang.T("v2.finish_transfer"));
		AckResultV2 ack = await _transport.SendWithAckAsync(117u, null, 0u, 0u, 0, ct);
		EnsureSuccess(ack, "SENDFILE_END failed");
		ResFileDataInfoV2 res = ArPacketCodecV2.ParseFileDataInfo(ack.RawPayload ?? Array.Empty<byte>());
		string detail = Encoding.ASCII.GetString(res.Detail).TrimEnd(new char[1]);
		if (res.Status != 0 || detail != "OK")
		{
			throw new InvalidOperationException(detail);
		}
	}

	protected virtual async Task PollUpgradeStatusAsync(CancellationToken ct)
	{
		for (int i = 0; i < 200; i++)
		{
			await DelayUpgradeStatusPollAsync(ct);
			AckResultV2 ack = await _transport.SendWithAckAsync(118u, null, 0u, 0u, 0, ct);
			EnsureSuccess(ack, "UPGRADE_STATUS failed");
			ResUpgradeStatusV2 status = ArPacketCodecV2.ParseUpgradeStatus(ack.RawPayload ?? Array.Empty<byte>());
			if (!JudgeUpgradeStatus(status, out var errorDesc))
			{
				throw new InvalidOperationException(errorDesc);
			}
			float percent = Math.Max(0, Math.Min(99, status.Percent));
			ReportProgress("UpgradeStatus", 0.65f + percent * 0.0034f, Lang.T("v2.loading_firmware"));
			if (status.Percent > 99)
			{
				return;
			}
			errorDesc = null;
		}
		throw new TimeoutException("upgrade status query limit exceeded");
	}

	private async Task FinalRebootAsync(CancellationToken ct)
	{
		ReportProgress("Reboot", 0.98f, Lang.T("v2.reboot_device"));
		if (!(await _transport.ReconnectAsync(GD.Inst.ReOpenTimeout_Asce, ct)))
		{
			throw new InvalidOperationException("final reconnect timeout");
		}
	}

	private bool JudgeUpgradeStatus(ResUpgradeStatusV2 status, out string desc)
	{
		desc = "upgrade success";
		if (status == null)
		{
			desc = "Upgrade failed, unknown error";
			return false;
		}
		switch ((UpgradeStatusErrorCode)status.Status)
		{
		case UpgradeStatusErrorCode.STAT_VERIFY_IMAGE:
			return true;
		case UpgradeStatusErrorCode.UPGRATE_ERR_NO_SD:
			desc = "Upgrade failed, no SD card detected";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_NO_PC:
			desc = "Upgrade failed, no computer connected";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_NO_FILE:
			desc = "Upgrade failed, no upgrade file found";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_BAD_FILE:
			desc = "Upgrade failed, upgrade file is corrupted";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_BOARD_TYPE:
			desc = "Upgrade failed, the device model does not match the upgraded firmware";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_APP_VERSION:
			desc = "Upgrade failed, The firmware version number is too low";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_UPGRADE_MODE:
			desc = "Upgrade failed, invalid upgrade mode";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_IMG_TYPE:
			desc = "Upgrade failed, image type error";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_IMG_CRC:
			desc = "Upgrade failed, image CRC error";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_MAX:
			desc = "Upgrade failed, unknown error";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_FLASH_ERR:
			desc = "Upgrade failed, flash error";
			return false;
		case UpgradeStatusErrorCode.UPGRATE_ERR_CHIPID:
			desc = "Upgrade failed, chip ID error";
			return false;
		default:
			return status.Status >= 0;
		}
	}

	protected virtual void EnsureSuccess(AckResultV2 ack, string message)
	{
		if (ack == null)
		{
			throw new InvalidOperationException(message);
		}
		if (ack.Status == AckStatusV2.Cancelled)
		{
			throw new OperationCanceledException();
		}
		if (!ack.IsSuccess)
		{
			throw new InvalidOperationException(string.IsNullOrEmpty(ack.ErrorMessage) ? message : ack.ErrorMessage);
		}
	}

	private void ReportProgress(string stepName, float percent, string desc)
	{
		OnProgress?.Invoke(this, new FlowProgressEventArgsV2
		{
			StepName = stepName,
			Percent = percent,
			Desc = desc
		});
	}

	public void Dispose()
	{
		Cancel();
		_fileSession?.Dispose();
		_cts?.Dispose();
	}
}
