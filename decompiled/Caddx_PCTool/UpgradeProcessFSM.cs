using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class UpgradeProcessFSM : IDisposable
{
	private readonly UsbSerialportFSM _usb;

	public UpgState CurrState = UpgState.Idle;

	private CancellationTokenSource _ctsTimeout;

	private const int AckRetryIntervalMs = 2000;

	private const int AckTotalTimeoutMs = 8000;

	public string FilePath;

	private FileTransferSession _fileSession;

	private int _chunkSize = 1048576;

	private uint TimeoutPeriod = 8000u;

	private bool _isAutoUpgrade;

	private bool _isFirst = false;

	private int QueryUpgradeStatustimes = 0;

	private readonly int QueryUpgradeStatustimesLimit = 200;

	private bool _isSendRCModeData = false;

	private bool _isAutoSendJson = false;

	private bool _isAutoSendBBFreq = false;

	private string _saveBBFreqPath;

	private ResAscentInfo _upgDevInfo;

	public Action<uint, ResAscentInfo, object> OnAsceDevRecInfo;

	public Action<string, string, Color> OnUpgradeFail;

	public bool IsFirst => _isFirst;

	public bool IsAutoUpgrade => _isAutoUpgrade;

	public bool IsSendJson_RCMode { get; set; }

	public event EventHandler<UpgProcHappenEventArgs> OnUpgProcHappenEvent;

	public UpgradeProcessFSM(UsbSerialportFSM usb, ResAscentInfo devInfo)
	{
		_isFirst = true;
		_usb = usb;
		UsbSerialportFSM usb2 = _usb;
		usb2.OnAckReceivedEx = (Action<uint, uint, object, object>)Delegate.Combine(usb2.OnAckReceivedEx, new Action<uint, uint, object, object>(OnAck));
		_upgDevInfo = devInfo;
	}

	public UpgradeProcessFSM(UsbSerialportFSM usb)
	{
		_isFirst = true;
		_usb = usb;
		UsbSerialportFSM usb2 = _usb;
		usb2.OnAckReceivedEx = (Action<uint, uint, object, object>)Delegate.Combine(usb2.OnAckReceivedEx, new Action<uint, uint, object, object>(OnAck));
	}

	public void SetFile(string path)
	{
		FilePath = path;
	}

	private static bool TryParseVid(string vid, out int value)
	{
		value = 0;
		if (string.IsNullOrWhiteSpace(vid))
		{
			return false;
		}
		string text = vid.Trim();
		if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			text = text.Substring(2);
		}
		return int.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
	}

	public void SetFilePath_Json(string path)
	{
		_isAutoSendJson = true;
		_isAutoUpgrade = (_isSendRCModeData = false);
		FilePath = path;
		_fileSession = new FileTransferSession(path, "/factory/" + Path.GetFileName(path));
	}

	public void SetFilePath_BBFreq(string path)
	{
		_isAutoSendBBFreq = (_isAutoSendJson = true);
		_isAutoUpgrade = (_isSendRCModeData = false);
		FilePath = path;
		_fileSession = new FileTransferSession(path, "/factory/" + Path.GetFileName(path));
	}

	private void StartTimeout(uint ms)
	{
		_ctsTimeout?.Cancel();
		_ctsTimeout = new CancellationTokenSource();
		CancellationToken token = _ctsTimeout.Token;
		Task.Run(async delegate
		{
			try
			{
				await Task.Delay((int)ms, token);
				if (!token.IsCancellationRequested)
				{
					ReleaseFileResources();
					Fail($"{CurrState} timeout", "Unplug the USB cable and try the upgrade again");
				}
			}
			catch
			{
			}
		});
	}

	private void StopTimeout()
	{
		_ctsTimeout?.Cancel();
	}

	private bool SendWithRetryGuard(uint cmd, byte[] payload, uint len, string sendFailMsg, string timeoutMsg, uint expectedAckCmd = 0u)
	{
		return _usb.SendWithAckGuard(cmd, payload, len, timeoutMsg, null, delegate(string msg)
		{
			Fail(msg);
		}, null, expectedAckCmd, 2000, 8000, 3000, null, null, delegate(uint _, uint retryCount, string msg)
		{
			Fail((retryCount == 0) ? sendFailMsg : msg);
		});
	}

	private bool SendWithRetryGuardDelay(uint cmd, byte[] payload, uint len, string sendFailMsg, string timeoutMsg, int delayTime, uint expectedAckCmd = 0u)
	{
		return _usb.SendWithAckGuardDelay(cmd, payload, len, timeoutMsg, null, delegate(string msg)
		{
			Fail(msg);
		}, null, delayTime, expectedAckCmd, 2000, 8000, 3000, null, null, delegate(uint _, uint retryCount, string msg)
		{
			Fail((retryCount == 0) ? sendFailMsg : msg);
		});
	}

	public void Start()
	{
		if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
		{
			UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
			{
				infoType = InfoType.fail,
				Desc = Lang.T("upg_fsm.firmware_path_error")
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
			return;
		}
		if (_fileSession == null)
		{
			_fileSession = new FileTransferSession(FilePath, "/tmp/pc/" + Path.GetFileName(FilePath), new FileStream(FilePath, FileMode.Open, FileAccess.Read), _chunkSize);
		}
		_isAutoUpgrade = true;
		NextState(UpgState.Reboot_Clean);
	}

	public void NextState(UpgState next)
	{
		CurrState = next;
		StopTimeout();
		switch (next)
		{
		case UpgState.FindDevice_Normal:
			Send_FindDevice();
			break;
		case UpgState.Reboot_Clean:
			Send_RebootClean();
			break;
		case UpgState.WaitCleanOnline:
			Send_ReopenCOM();
			break;
		case UpgState.RemoteUpgrade:
			Send_RemoteUpgrade();
			break;
		case UpgState.SendFileStart:
			Send_FileStart();
			break;
		case UpgState.SendFileData:
			Send_FileData();
			break;
		case UpgState.SendFileEnd:
			Send_FileEnd();
			break;
		case UpgState.UpgradeStatus:
			Send_UpgradeStatus();
			break;
		case UpgState.Completed:
			_isAutoUpgrade = false;
			Send_ReopenCOM();
			break;
		case UpgState.Failed:
			_isAutoUpgrade = false;
			break;
		case UpgState.SendFileStart_Json:
			Send_FileStart_Json();
			break;
		case UpgState.SendFileData_Json:
			Send_FileData_Hub();
			break;
		case UpgState.SendFileEnd_Json:
			Send_FileEnd_Hub();
			break;
		case UpgState.Completed_Json:
			Send_ReopenJson();
			break;
		case UpgState.SetBBFreq:
			Send_SetBBFreq();
			break;
		case UpgState.Completed_SetBBFreq:
			Send_ReopenBBFreq();
			break;
		case UpgState.SendFileStart_BBFreq:
			break;
		case UpgState.GetBBFreq:
		case UpgState.SendFileData_BBFreq:
		case UpgState.SendFileEnd_BBFreq:
			break;
		}
	}

	private void Fail(string msg)
	{
		QueryUpgradeStatustimes = 0;
		StopTimeout();
		_isAutoUpgrade = (_isAutoSendBBFreq = (_isAutoSendJson = false));
		ReleaseFileResources();
		OnUpgProcHappenEvent?.Invoke(null, new UpgProcHappenEventArgs
		{
			infoType = InfoType.fail,
			Desc = msg,
			ErrMsg = CurrState.ToString() + " timeout," + msg,
			upgState = CurrState
		});
		CurrState = UpgState.Failed;
		WriteLog.WriteLogFileToUI("UpgradeProcessFSM fail，desc=" + msg, Color.Red);
	}

	private void Fail(string msg, string desc)
	{
		QueryUpgradeStatustimes = 0;
		StopTimeout();
		_isAutoUpgrade = (_isAutoSendBBFreq = (_isAutoSendJson = false));
		ReleaseFileResources();
		OnUpgProcHappenEvent?.Invoke(null, new UpgProcHappenEventArgs
		{
			infoType = InfoType.fail,
			Desc = msg,
			ErrMsg = desc,
			upgState = CurrState
		});
		CurrState = UpgState.Failed;
		WriteLog.WriteLogFileToUI("UpgradeProcessFSM fail，desc=" + msg, Color.Red);
	}

	private void ReleaseFileResources()
	{
		try
		{
			if (_fileSession != null)
			{
				_fileSession.Dispose();
				_fileSession = null;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("释放文件资源异常: " + ex.Message, Color.Red);
		}
	}

	public void Send_FindDevice(bool suppressUiFail = false)
	{
		CurrState = UpgState.FindDevice_Normal;
		if (suppressUiFail)
		{
			_usb.SendWithAckGuard(60u, null, 0u, "FIND_DEVICE Ack Timeout", null, delegate(string msg)
			{
				IgnoreCompletedFindDeviceFail(msg);
			}, null, 60u, 2000, 8000, 3000, null, null, delegate(uint _, uint retryCount, string msg)
			{
				IgnoreCompletedFindDeviceFail((retryCount == 0) ? "FIND_DEVICE Fail" : msg);
			});
		}
		else
		{
			SendWithRetryGuard(60u, null, 0u, "FIND_DEVICE Fail", "FIND_DEVICE Ack Timeout");
		}
	}

	private void IgnoreCompletedFindDeviceFail(string msg)
	{
		CurrState = UpgState.Completed;
		WriteLog.WriteLogFileToUI("升级完成后FIND_DEVICE失败，已忽略UI错误，desc=" + msg, Color.Gray);
	}

	public void Send_RebootClean()
	{
		byte[] array = Encoding.ASCII.GetBytes("clean");
		Array.Resize(ref array, 32);
		if (_usb.CommandSerialSendImmediate(3u, 1, 2, 0, array, (uint)array.Length, 0u, 3000, out var _) != 0)
		{
			Fail("send REBOOT fail");
		}
		else
		{
			NextState(UpgState.WaitCleanOnline);
		}
	}

	public void Send_Reboot()
	{
		byte[] array = Encoding.ASCII.GetBytes("normal");
		Array.Resize(ref array, 32);
		if (_usb.CommandSerialSendImmediate(3u, 1, 2, 0, array, (uint)array.Length, 0u, 3000, out var _) != 0)
		{
			Fail("send REBOOT fail");
		}
	}

	private void Send_ReopenCOM()
	{
		Task.Run(delegate
		{
			if (IsAutoUpgrade)
			{
				OnUpgProcHappenEvent?.Invoke(null, new UpgProcHappenEventArgs
				{
					infoType = InfoType.debugInfo,
					Desc = Lang.T("upg_fsm.reconnecting"),
					Percent = 0.08f
				});
			}
			if (!_usb.Reopen(GD.Inst.ReOpenTimeout_Asce))
			{
				Fail("Reconnection timeout");
			}
			else if (IsAutoUpgrade)
			{
				OnUpgProcHappenEvent?.Invoke(null, new UpgProcHappenEventArgs
				{
					infoType = InfoType.debugInfo,
					Desc = Lang.T("upg_fsm.device_reconnected_ready"),
					Percent = 0.1f
				});
				NextState(UpgState.RemoteUpgrade);
			}
			else
			{
				UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
				{
					infoType = InfoType.debugInfo,
					Desc = Lang.T("upg_fsm.upgrade_complete"),
					Percent = 1f
				};
				OnUpgProcHappenEvent?.Invoke(null, e);
				Send_FindDevice(suppressUiFail: true);
			}
		});
	}

	private void Send_ReopenJson()
	{
		if (!_usb.Reopen_Json(GD.Inst.ReOpenTimeout_Asce))
		{
			Fail("Reconnection timeout");
			return;
		}
		UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
		{
			infoType = InfoType.info,
			Desc = Lang.T("upg_fsm.restart_complete"),
			SignName = "restart complete",
			Percent = 1f
		};
		OnUpgProcHappenEvent?.Invoke(null, e);
	}

	private void Send_ReopenBBFreq()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!_usb.Reopen_Json(GD.Inst.ReOpenTimeout_Asce))
		{
			Fail("Reconnection timeout");
			return;
		}
		UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
		{
			infoType = InfoType.info,
			Desc = Lang.T("upg_fsm.restart_complete"),
			SignName = "restart complete",
			Percent = 1f
		};
		OnUpgProcHappenEvent?.Invoke(null, e);
		CommModalFrm commModalFrm = new CommModalFrm();
		commModalFrm.SetAllTxt(Lang.T("common.title_prompt"), Lang.T("upg_fsm.device_restart_complete"), Color.Green);
		((Form)commModalFrm).ShowDialog();
	}

	private void Send_RemoteUpgrade()
	{
		QueryUpgradeStatustimes = 0;
		if (SendWithRetryGuard(114u, null, 0u, "REMOTE_UPGRADE Fail", "REMOTE_UPGRADE Ack Timeout"))
		{
			UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
			{
				infoType = InfoType.debugInfo,
				Desc = Lang.T("upg_fsm.device_ready_enter_upgrade"),
				Percent = 0.13f
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
		}
	}

	private void Send_FileStart()
	{
		ArFileInfo s = _fileSession.BuildFileInfo();
		byte[] array = StructToBytes(s);
		if (SendWithRetryGuard(115u, array, (uint)array.Length, "SENDFILE_START Fail", "SENDFILE_START Ack Timeout"))
		{
			Thread.Sleep(100);
			UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
			{
				infoType = InfoType.debugInfo,
				Desc = Lang.T("upg_fsm.start_transfer"),
				Percent = 0.17f
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
		}
	}

	public void Send_FileStart_Json()
	{
		CurrState = UpgState.SendFileStart_Json;
		ArFileInfo s = _fileSession.BuildFileInfo();
		byte[] array = StructToBytes(s);
		if (SendWithRetryGuard(115u, array, (uint)array.Length, "SENDFILE_START Fail", "SENDFILE_START Ack Timeout"))
		{
			Thread.Sleep(100);
			UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
			{
				infoType = InfoType.debugInfo,
				Desc = Lang.T("upg_fsm.start_transfer"),
				Percent = 0.25f
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
		}
	}

	public void Send_FileStart_BBFreq(string filePath, string fullPath)
	{
		_usb.SetSendBBFreqConfig(isAuto: true);
		_isAutoSendBBFreq = true;
		CurrState = UpgState.SendFileStart_Json;
		_fileSession = new FileTransferSession(filePath, fullPath);
		ArFileInfo s = _fileSession.BuildFileInfo();
		byte[] array = StructToBytes(s);
		if (SendWithRetryGuard(115u, array, (uint)array.Length, "SENDFILE_START 失败", "等待 SENDFILE_START 响应超时"))
		{
			Thread.Sleep(100);
			UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
			{
				infoType = InfoType.debugInfo,
				Desc = Lang.T("upg_fsm.start_transfer"),
				Percent = 0.25f
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
		}
	}

	private void Send_FileData()
	{
		try
		{
			if (_fileSession.IsComplete)
			{
				NextState(UpgState.SendFileEnd);
				return;
			}
			byte[] nextChunk = _fileSession.GetNextChunk(out var length);
			if (nextChunk == null)
			{
				NextState(UpgState.SendFileEnd);
			}
			else if (SendWithRetryGuard(116u, nextChunk, (uint)length, "Send file chunk Fail", "Send file chunk Ack Timeout"))
			{
				UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
				{
					infoType = InfoType.procesInfo,
					Desc = Lang.T("upg_fsm.transfer_in_progress"),
					upgState = UpgState.SendFileData
				};
				OnUpgProcHappenEvent?.Invoke(null, e);
			}
		}
		catch (Exception ex)
		{
			Fail("Ascent upg error,desc=" + ex.Message);
		}
	}

	private void Send_FileData_Hub()
	{
		if (_fileSession.IsComplete)
		{
			NextState(UpgState.SendFileEnd_Json);
			return;
		}
		byte[] nextChunk = _fileSession.GetNextChunk(out var length);
		if (nextChunk == null)
		{
			NextState(UpgState.SendFileEnd_Json);
		}
		else
		{
			SendWithRetryGuard(116u, nextChunk, (uint)length, "Send file chunk Fail", "Send file chunk Ack Timeout");
		}
	}

	private void Send_FileEnd()
	{
		if (SendWithRetryGuard(117u, null, 0u, "SENDFILE_END Fail", "SENDFILE_END Ack Timeout"))
		{
			_fileSession?.Dispose();
			_fileSession = null;
		}
	}

	private void Send_FileEnd_Hub()
	{
		if (SendWithRetryGuard(117u, null, 0u, "SENDFILE_END Fail", "SENDFILE_END Ack Timeout"))
		{
			_fileSession?.Dispose();
			_fileSession = null;
		}
	}

	private void Send_UpgradeStatus()
	{
		QueryUpgradeStatustimes++;
		SendWithRetryGuardDelay(118u, null, 0u, "UPGRADE_STATUS Fail", "UPGRADE_STATUS Ack Timeout", 1000);
	}

	public void Send_GetBBFreq(string path)
	{
		CurrState = UpgState.GetBBFreq;
		_isAutoSendJson = (_isAutoSendBBFreq = true);
		_isAutoUpgrade = (_isSendRCModeData = false);
		_saveBBFreqPath = path;
		SendWithRetryGuard(120u, null, 0u, "GET_BB_FREQ Fail", "GET_BB_FREQ Ack Timeout", 116u);
	}

	public void Send_SetBBFreq()
	{
		CurrState = UpgState.SetBBFreq;
		_isAutoSendJson = (_isAutoSendBBFreq = true);
		_isAutoUpgrade = (_isSendRCModeData = false);
		SendWithRetryGuard(121u, null, 0u, "SET_BB_FREQ Fail", "SET_BB_FREQ Ack Timeout");
	}

	public void Send_FactoryReset()
	{
		CurrState = UpgState.FactoryReset_BBFreq;
		SendWithRetryGuard(122u, null, 0u, "FACTORY_RESET Fail", "FACTORY_RESET Ack Timeout");
	}

	private void OnAck(uint cmd, uint seq, object data, object data2)
	{
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			switch (CurrState)
			{
			case UpgState.Idle:
				switch (cmd)
				{
				case 116u:
				{
					string contents2 = (string)data2;
					File.WriteAllText(_saveBBFreqPath, contents2);
					CommModalFrm commModalFrm5 = new CommModalFrm();
					commModalFrm5.SetAllTxt(Lang.T("common.title_success"), Lang.T("upg_fsm.bbfreq_saved_successfully"), Color.Green);
					((Form)commModalFrm5).ShowDialog();
					break;
				}
				case 117u:
					_isAutoSendBBFreq = (_isAutoSendJson = false);
					break;
				}
				break;
			case UpgState.FindDevice_Normal:
				if (cmd == 60)
				{
					if (IsAutoUpgrade)
					{
						NextState(UpgState.Reboot_Clean);
					}
					else
					{
						StopTimeout();
					}
					ResAscentInfo resAscentInfo2 = (ResAscentInfo)data;
					_chunkSize = resAscentInfo2.RecMaxSize;
					OnAsceDevRecInfo?.Invoke(cmd, resAscentInfo2, data2);
				}
				break;
			case UpgState.Reboot_Clean:
				if (cmd == 3)
				{
					NextState(UpgState.WaitCleanOnline);
				}
				break;
			case UpgState.WaitCleanOnline:
				if (cmd == 60)
				{
					if (data is ResAscentInfo resAscentInfo)
					{
						_chunkSize = resAscentInfo.RecMaxSize;
						OnAsceDevRecInfo?.Invoke(cmd, resAscentInfo, data2);
					}
					if (IsAutoUpgrade)
					{
						NextState(UpgState.RemoteUpgrade);
					}
				}
				break;
			case UpgState.RemoteUpgrade:
				if (cmd == 114)
				{
					OnUpgProcHappenEvent?.Invoke(null, new UpgProcHappenEventArgs
					{
						infoType = InfoType.debugInfo,
						Desc = Lang.T("upg_fsm.upgrade_mode_entered_start_transfer"),
						Percent = 0.15f
					});
					if (IsAutoUpgrade)
					{
						NextState(UpgState.SendFileStart);
					}
				}
				break;
			case UpgState.SendFileStart:
				if (cmd == 115)
				{
					if (IsAutoUpgrade)
					{
						NextState(UpgState.SendFileData);
					}
					else
					{
						StopTimeout();
					}
				}
				break;
			case UpgState.SendFileData:
				if (cmd == 116)
				{
					_fileSession.Advance();
					if (_fileSession.IsComplete)
					{
						NextState(UpgState.SendFileEnd);
					}
					else
					{
						NextState(UpgState.SendFileData);
					}
				}
				break;
			case UpgState.SendFileEnd:
				if (cmd == 117)
				{
					ResFileDataInfo resFileDataInfo = (ResFileDataInfo)data;
					string title = Lang.T("common.title_prompt");
					string desc = Lang.T("upg_fsm.file_upload_success");
					CommModalFrm commModalFrm4 = new CommModalFrm();
					string text2 = Encoding.ASCII.GetString(resFileDataInfo.Detail).TrimEnd(new char[1]);
					if (resFileDataInfo.Status != 0 || text2 != "OK")
					{
						desc = Encoding.ASCII.GetString(resFileDataInfo.Detail).TrimEnd(new char[1]);
						IsSendJson_RCMode = false;
						Fail(desc);
					}
					else if (IsSendJson_RCMode)
					{
						UpgProcHappenEventArgs e3 = new UpgProcHappenEventArgs
						{
							infoType = InfoType.info,
							Desc = desc
						};
						OnUpgProcHappenEvent?.Invoke(null, e3);
						IsSendJson_RCMode = false;
						commModalFrm4.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
						((Form)commModalFrm4).ShowDialog();
						NextState(UpgState.UpgradeStatus);
					}
					else if (_isAutoSendJson)
					{
						UpgProcHappenEventArgs e4 = new UpgProcHappenEventArgs
						{
							infoType = InfoType.info,
							Desc = desc
						};
						OnUpgProcHappenEvent?.Invoke(null, e4);
						_isAutoSendJson = false;
						commModalFrm4.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
						((Form)commModalFrm4).ShowDialog();
						NextState(UpgState.UpgradeStatus);
					}
					else
					{
						NextState(UpgState.UpgradeStatus);
						UpgProcHappenEventArgs e5 = new UpgProcHappenEventArgs
						{
							infoType = InfoType.debugInfo,
							Desc = Lang.T("upg_fsm.check_upgrade_status"),
							Percent = 0.65f
						};
						OnUpgProcHappenEvent?.Invoke(null, e5);
					}
				}
				break;
			case UpgState.UpgradeStatus:
			{
				if (cmd != 118 || !_isAutoUpgrade)
				{
					break;
				}
				ResUpgradeStatus resUpgradeStatus = (ResUpgradeStatus)data;
				WriteLog.WriteLogFileToUI($"Percent={resUpgradeStatus.Percent},Status={resUpgradeStatus.Status}", Color.Red);
				string s = "";
				JudgeUpgradeStatus(resUpgradeStatus, out s);
				if (resUpgradeStatus.Status < 0)
				{
					_isAutoUpgrade = false;
					int result = 0;
					int.TryParse(_upgDevInfo.UsbInfo.VID, NumberStyles.HexNumber, null, out result);
					if (result == 7542 || result == 7541)
					{
						WriteLog.WriteLogFileToUI("vrx upgrade fail,send reboot", Color.Red);
						Send_Reboot();
						Task.Run(delegate
						{
							if (!_usb.Reopen(GD.Inst.ReOpenTimeout_Asce))
							{
								WriteLog.WriteLogFileToUI("重启后重连超时，跳过 FindDevice", Color.Red);
							}
							else
							{
								Send_FindDevice(suppressUiFail: true);
							}
						});
					}
					OnUpgradeFail?.Invoke(Lang.T("common.error_upgrade"), s, Color.Red);
				}
				else if (QueryUpgradeStatustimes > QueryUpgradeStatustimesLimit)
				{
					Fail("Upgrade status query limit exceeded");
				}
				else if (resUpgradeStatus.Percent >= 100)
				{
					UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
					{
						infoType = InfoType.debugInfo,
						Desc = Lang.T("upg_fsm.upgrade_success_restarting"),
						Percent = 1f
					};
					OnUpgProcHappenEvent?.Invoke(null, e);
					Thread.Sleep(500);
					NextState(UpgState.Completed);
				}
				else if (_isSendRCModeData || _isAutoSendJson)
				{
					NextState(UpgState.Completed);
				}
				else
				{
					NextState(UpgState.UpgradeStatus);
					UpgProcHappenEventArgs e2 = new UpgProcHappenEventArgs
					{
						infoType = InfoType.procesInfo,
						Desc = Lang.T("upg_fsm.loading_firmware"),
						upgState = UpgState.UpgradeStatus
					};
					OnUpgProcHappenEvent?.Invoke(null, e2);
				}
				break;
			}
			case UpgState.SendFileStart_Json:
				if (cmd == 115)
				{
					if (_isAutoSendJson)
					{
						NextState(UpgState.SendFileData_Json);
					}
					else
					{
						StopTimeout();
					}
				}
				break;
			case UpgState.SendFileData_Json:
				if (cmd == 115 || cmd == 116)
				{
					_fileSession.Advance();
					if (_fileSession.IsComplete)
					{
						NextState(UpgState.SendFileEnd_Json);
					}
					else
					{
						NextState(UpgState.SendFileData_Json);
					}
				}
				break;
			case UpgState.SendFileEnd_Json:
				if (cmd == 117)
				{
					if (_isAutoSendBBFreq)
					{
						_isAutoSendBBFreq = (_isAutoSendJson = false);
						NextState(UpgState.SetBBFreq);
						break;
					}
					if (_isAutoSendJson)
					{
						_isAutoSendJson = false;
						NextState(UpgState.Completed_Json);
						break;
					}
					ResFileDataInfo resFileDataInfo2 = (ResFileDataInfo)data;
					string text4 = Lang.T("common.title_prompt");
					string desc2 = Lang.T("upg_fsm.upload_complete_restarting");
					string text5 = Encoding.ASCII.GetString(resFileDataInfo2.Detail).TrimEnd(new char[1]);
					UpgProcHappenEventArgs e7 = new UpgProcHappenEventArgs
					{
						infoType = InfoType.info,
						Desc = desc2,
						Percent = 0.85f
					};
					OnUpgProcHappenEvent?.Invoke(null, e7);
				}
				break;
			case UpgState.GetBBFreq:
				if (cmd == 116)
				{
					string text3 = (string)data2;
					UpgProcHappenEventArgs e6 = new UpgProcHappenEventArgs
					{
						infoType = InfoType.ctrlSign,
						Desc = text3,
						ErrMsg = text3
					};
					OnUpgProcHappenEvent?.Invoke(null, e6);
				}
				break;
			case UpgState.SetBBFreq:
				if (cmd == 121)
				{
					ResAckInfo resAckInfo2 = (ResAckInfo)data;
					CommModalFrm commModalFrm3 = new CommModalFrm();
					if (resAckInfo2.Status == 0)
					{
						commModalFrm3.SetAllTxt(Lang.T("common.title_prompt"), Lang.T("upg_fsm.bbfreq_upload_success_restarting"), Color.Green);
					}
					else
					{
						string text = Encoding.ASCII.GetString(resAckInfo2.Detail).TrimEnd(new char[1]);
						commModalFrm3.SetAllTxt(Lang.T("common.title_error"), "set BB_Freq error,desc=" + text, Color.Red);
					}
					((Form)commModalFrm3).ShowDialog();
					NextState(UpgState.Completed_SetBBFreq);
				}
				break;
			case UpgState.Completed_SetBBFreq:
				switch (cmd)
				{
				case 116u:
				{
					string contents = (string)data2;
					File.WriteAllText(_saveBBFreqPath, contents);
					CommModalFrm commModalFrm2 = new CommModalFrm();
					commModalFrm2.SetAllTxt(Lang.T("common.title_success"), Lang.T("upg_fsm.bbfreq_saved_successfully"), Color.Green);
					((Form)commModalFrm2).ShowDialog();
					break;
				}
				case 117u:
					_isAutoSendBBFreq = (_isAutoSendJson = false);
					CurrState = UpgState.Idle;
					break;
				}
				break;
			case UpgState.FactoryReset_BBFreq:
				if (cmd == 122)
				{
					ResAckInfo resAckInfo = (ResAckInfo)data;
					CommModalFrm commModalFrm = new CommModalFrm();
					if (resAckInfo.Status == 0)
					{
						commModalFrm.SetAllTxt(Lang.T("common.title_prompt"), data2.ToString(), Color.Green);
						NextState(UpgState.Completed_SetBBFreq);
					}
					else
					{
						commModalFrm.SetAllTxt(Lang.T("common.title_error"), data2.ToString(), Color.Red);
					}
					((Form)commModalFrm).ShowDialog();
				}
				break;
			case UpgState.Completed:
			case UpgState.Failed:
			case UpgState.Completed_Json:
			case UpgState.SendFileStart_BBFreq:
			case UpgState.SendFileData_BBFreq:
			case UpgState.SendFileEnd_BBFreq:
				break;
			}
		}
		catch (Exception ex)
		{
			CommModalFrm commModalFrm6 = new CommModalFrm();
			commModalFrm6.SetAllTxt(Lang.T("common.title_error"), "OnACK error,desc=" + ex.Message, Color.Red);
			((Form)commModalFrm6).ShowDialog();
		}
	}

	private bool JudgeUpgradeStatus(ResUpgradeStatus obj, out string s)
	{
		s = "upgrade success";
		string text = "Please power off the device (disconnect both the UART B and USB cables), then power it back on and try upgrading again.";
		try
		{
			switch ((UpgradeStatusErrorCode)obj.Status)
			{
			case UpgradeStatusErrorCode.STAT_VERIFY_IMAGE:
				return true;
			case UpgradeStatusErrorCode.UPGRATE_ERR_NO_SD:
				s = "Upgrade failed, no SD card detected," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_NO_PC:
				s = "Upgrade failed, no computer connected," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_NO_FILE:
				s = "Upgrade failed, no upgrade file found," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_BAD_FILE:
				s = "Upgrade failed, upgrade file is corrupted," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_BOARD_TYPE:
				s = "Upgrade failed, the device model does not match the upgraded firmware," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_APP_VERSION:
				s = "Upgrade failed, The firmware version number is too low," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_UPGRADE_MODE:
				s = "Upgrade failed, invalid upgrade mode," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_IMG_TYPE:
				s = "Upgrade failed, image type error," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_IMG_CRC:
				s = "Upgrade failed, image CRC error," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_MAX:
				s = "Upgrade failed, unknown error," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_FLASH_ERR:
				s = "Upgrade failed, flash error," + text;
				return false;
			case UpgradeStatusErrorCode.UPGRATE_ERR_CHIPID:
				s = "Upgrade failed, chip ID error," + text;
				return false;
			default:
				return true;
			}
		}
		catch (Exception ex)
		{
			s = "Upgrade failed, unknown error,desc=" + ex.Message;
			return false;
		}
	}

	private void ShowUnkonwnCMD_PromptWindows(UpgState state, uint cmd, string desc)
	{
		WriteLog.WriteLogFileToUI(desc, Color.Red);
	}

	private byte[] StructToBytes<T>(T s) where T : struct
	{
		int num = Marshal.SizeOf(s);
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(s, intPtr, fDeleteOld: false);
		Marshal.Copy(intPtr, array, 0, num);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	public void Dispose()
	{
		StopTimeout();
		if (_usb != null)
		{
			UsbSerialportFSM usb = _usb;
			usb.OnAckReceivedEx = (Action<uint, uint, object, object>)Delegate.Remove(usb.OnAckReceivedEx, new Action<uint, uint, object, object>(OnAck));
		}
		_fileSession?.Dispose();
	}
}
