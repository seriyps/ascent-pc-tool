using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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

	public string FilePath;

	private FileStream _fs;

	private long _fileLen;

	private long _sent;

	private int _chunkSize = 1048576;

	private int _plusing = 0;

	private uint TimeoutPeriod = 8000u;

	private bool _isFirst = false;

	private int QueryUpgradeStatustimes = 0;

	private readonly int QueryUpgradeStatustimesLimit = 200;

	private bool _isSendRCModeData = false;

	private bool _isAutoSendJson = false;

	private byte[] _fileDataArr;

	public Action<uint, ResAscentInfo, object> OnAsceDevRecInfo;

	public bool IsFirst => _isFirst;

	public bool IsAutoUpgrade { get; set; }

	public bool IsSendJson_RCMode { get; set; }

	public event EventHandler<UpgProcHappenEventArgs> OnUpgProcHappenEvent;

	public UpgradeProcessFSM(UsbSerialportFSM usb)
	{
		_isFirst = true;
		_usb = usb;
		UsbSerialportFSM usb2 = _usb;
		usb2.OnAckReceived = (Action<uint, object, object>)Delegate.Combine(usb2.OnAckReceived, new Action<uint, object, object>(OnAck));
	}

	public void SetFile(string path)
	{
		FilePath = path;
		_fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
		_fileLen = _fs.Length;
		_sent = 0L;
	}

	public void SetFilePath_Json(string path)
	{
		_isAutoSendJson = true;
		IsAutoUpgrade = (_isSendRCModeData = false);
		FilePath = path;
		_sent = 0L;
		_fileDataArr = File.ReadAllBytes(path);
		_fileLen = _fileDataArr.Length;
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

	public void Start()
	{
		if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
		{
			UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
			{
				infoType = InfoType.fail,
				Desc = ((GD.Inst.CurrLang == 1) ? "固件路径有误" : "firmware path error")
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
			return;
		}
		if (_fs == null)
		{
			_fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
			_fileLen = _fs.Length;
		}
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
			IsAutoUpgrade = false;
			Send_ReopenCOM();
			break;
		case UpgState.Failed:
			IsAutoUpgrade = false;
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
		}
	}

	private void Fail(string msg)
	{
		QueryUpgradeStatustimes = 0;
		StopTimeout();
		IsAutoUpgrade = false;
		ReleaseFileResources();
		OnUpgProcHappenEvent?.Invoke(null, new UpgProcHappenEventArgs
		{
			infoType = InfoType.fail,
			Desc = msg,
			errMsg = CurrState.ToString() + " timeout," + msg,
			upgState = CurrState
		});
		CurrState = UpgState.Failed;
	}

	private void Fail(string msg, string desc)
	{
		QueryUpgradeStatustimes = 0;
		StopTimeout();
		IsAutoUpgrade = false;
		ReleaseFileResources();
		OnUpgProcHappenEvent?.Invoke(null, new UpgProcHappenEventArgs
		{
			infoType = InfoType.fail,
			Desc = msg,
			errMsg = CurrState.ToString() + " timeout," + msg,
			upgState = CurrState
		});
		CurrState = UpgState.Failed;
	}

	private void ReleaseFileResources()
	{
		try
		{
			if (_fs != null)
			{
				_fs.Dispose();
				_fs = null;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("释放文件资源异常: " + ex.Message, Color.Red);
		}
	}

	public void Send_FindDevice()
	{
		CurrState = UpgState.FindDevice_Normal;
		StartTimeout(TimeoutPeriod);
		if (_usb.CommandSerialSend(60u, 1, 2, 0, null, 0u, 0u, 2000) != 0)
		{
			Fail("发送 FIND_DEVICE 失败");
		}
	}

	public void Send_RebootClean()
	{
		StartTimeout(2 * TimeoutPeriod);
		byte[] array = Encoding.ASCII.GetBytes("clean");
		Array.Resize(ref array, 32);
		if (_usb.CommandSerialSend(3u, 1, 2, 0, array, (uint)array.Length, 0u, 3000) != 0)
		{
			Fail("发送 REBOOT 失败");
		}
	}

	private async Task Send_ReopenCOM()
	{
		if (!_usb.Reopen(GD.Inst.ReOpenTimeout_Asce))
		{
			Fail("重连超时");
			return;
		}
		Thread.Sleep(1000);
		if (IsAutoUpgrade)
		{
			NextState(UpgState.RemoteUpgrade);
			UpgProcHappenEventArgs upe = new UpgProcHappenEventArgs
			{
				infoType = InfoType.debugInfo,
				Desc = ((GD.Inst.CurrLang == 1) ? "进入clean模式" : "Enter clean mode"),
				Percent = 0.1f
			};
			OnUpgProcHappenEvent?.Invoke(null, upe);
		}
		else
		{
			UpgProcHappenEventArgs upe2 = new UpgProcHappenEventArgs
			{
				infoType = InfoType.debugInfo,
				Desc = ((GD.Inst.CurrLang == 1) ? "升级完成" : "upgrade is complete"),
				Percent = 1f
			};
			OnUpgProcHappenEvent?.Invoke(null, upe2);
		}
	}

	private void Send_ReopenJson()
	{
		if (!_usb.Reopen_Json(GD.Inst.ReOpenTimeout_Asce))
		{
			Fail("重连超时");
			return;
		}
		Thread.Sleep(500);
		UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
		{
			infoType = InfoType.info,
			Desc = ((GD.Inst.CurrLang == 1) ? "重启完成。" : "restart complete."),
			Percent = 1f
		};
		OnUpgProcHappenEvent?.Invoke(null, e);
	}

	private void Send_RemoteUpgrade()
	{
		StartTimeout(TimeoutPeriod);
		QueryUpgradeStatustimes = 0;
		if (_usb.CommandSerialSend(114u, 1, 2, 0, null, 0u, 0u, 3000) != 0)
		{
			Fail("REMOTE_UPGRADE 失败");
			return;
		}
		UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
		{
			infoType = InfoType.debugInfo,
			Desc = ((GD.Inst.CurrLang == 1) ? "执行文件传输" : "perform file transfers"),
			Percent = 0.15f
		};
		OnUpgProcHappenEvent?.Invoke(null, e);
	}

	private void Send_FileStart()
	{
		StartTimeout(TimeoutPeriod);
		BulidFileDataStruct(FilePath, out var fileinfo);
		byte[] array = StructToBytes(fileinfo);
		if (_usb.CommandSerialSend(115u, 1, 2, 0, array, (uint)array.Length, 0u, 3000) != 0)
		{
			Fail("SENDFILE_START 失败");
			return;
		}
		Thread.Sleep(100);
		_fs.Seek(0L, SeekOrigin.Begin);
		_plusing = 0;
		UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
		{
			infoType = InfoType.debugInfo,
			Desc = ((GD.Inst.CurrLang == 1) ? "开始传输文件" : "Start transferring files"),
			Percent = 0.17f
		};
		OnUpgProcHappenEvent?.Invoke(null, e);
	}

	public void Send_FileStart_Json()
	{
		CurrState = UpgState.SendFileStart_Json;
		BulidFileDataStruct_Hub(FilePath, out var fileinfo);
		byte[] array = StructToBytes(fileinfo);
		if (_usb.CommandSerialSend(115u, 1, 2, 0, array, (uint)array.Length, 0u, 3000) != 0)
		{
			Fail("SENDFILE_START 失败");
			return;
		}
		Thread.Sleep(100);
		UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
		{
			infoType = InfoType.debugInfo,
			Desc = ((GD.Inst.CurrLang == 1) ? "开始传输文件" : "Start transferring files"),
			Percent = 0.25f
		};
		OnUpgProcHappenEvent?.Invoke(null, e);
	}

	private void Send_FileData()
	{
		try
		{
			StartTimeout(TimeoutPeriod / 2);
			if (_sent >= _fileLen)
			{
				NextState(UpgState.SendFileEnd);
				return;
			}
			long val = _fileLen - _sent;
			int num = (int)Math.Min(_chunkSize, val);
			byte[] array = new byte[num];
			int num2 = _fs.Read(array, 0, num);
			int num3 = 1;
			if (_usb.CommandSerialSend(116u, 1, 2, 0, array, (uint)num, 0u, 8000) != 0)
			{
				Fail("发送文件分片失败");
			}
			_sent += num2;
			_plusing++;
			UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
			{
				infoType = InfoType.procesInfo,
				Desc = ((GD.Inst.CurrLang == 1) ? "文件传输中..." : "File transfer in progress..."),
				upgState = UpgState.SendFileData
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
			WriteLog.WriteLogFileToUI($"发送{_plusing}文件数据包", Color.Black);
		}
		catch (Exception ex)
		{
			Fail("Ascent upg error,desc=" + ex.Message);
		}
	}

	private void Send_FileData_Hub()
	{
		if (_sent >= _fileLen)
		{
			NextState(UpgState.SendFileEnd_Json);
			return;
		}
		long val = _fileLen - _sent;
		int len = (int)Math.Min(_chunkSize, val);
		if (_usb.CommandSerialSend(116u, 1, 2, 0, _fileDataArr, (uint)len, 0u, 8000) != 0)
		{
			Fail("发送文件分片失败");
		}
		_sent += _fileDataArr.Length;
		_plusing++;
	}

	private void Send_FileEnd()
	{
		StartTimeout(8000u);
		if (_usb.CommandSerialSend(117u, 1, 2, 0, null, 0u, 0u, 3000) != 0)
		{
			Fail("SENDFILE_END 失败");
		}
		_fs?.Dispose();
		_fs = null;
	}

	private void Send_FileEnd_Hub()
	{
		StartTimeout(8000u);
		if (_usb.CommandSerialSend(117u, 1, 2, 0, null, 0u, 0u, 3000) != 0)
		{
			Fail("SENDFILE_END 失败");
		}
		_fs?.Dispose();
		_fs = null;
	}

	private void Send_UpgradeStatus()
	{
		StartTimeout(TimeoutPeriod);
		Thread.Sleep(500);
		QueryUpgradeStatustimes++;
		if (_usb.CommandSerialSend(118u, 1, 2, 0, null, 0u, 0u, 3000) != 0)
		{
			Fail("UPGRADE_STATUS 查询失败");
		}
	}

	private void OnAck(uint cmd, object data, object data2)
	{
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		if (cmd == 0)
		{
			string text = StaticMethod.BytesToHexString((byte[])data2);
			string desc = Encoding.ASCII.GetString((byte[])data2).TrimEnd(new char[1]);
			ShowUnkonwnCMD_PromptWindows(CurrState, cmd, desc);
		}
		switch (CurrState)
		{
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
				ResAscentInfo resAscentInfo = (ResAscentInfo)data;
				_chunkSize = resAscentInfo.RecMaxSize;
				OnAsceDevRecInfo?.Invoke(cmd, resAscentInfo, data2);
			}
			break;
		case UpgState.Reboot_Clean:
			if (cmd == 3)
			{
				NextState(UpgState.WaitCleanOnline);
			}
			break;
		case UpgState.WaitCleanOnline:
			if (IsAutoUpgrade)
			{
				NextState(UpgState.RemoteUpgrade);
			}
			break;
		case UpgState.RemoteUpgrade:
			if (cmd == 114 && IsAutoUpgrade)
			{
				NextState(UpgState.SendFileStart);
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
				ResFileDataInfo resFileDataInfo2 = (ResFileDataInfo)data;
				NextState(UpgState.SendFileData);
			}
			break;
		case UpgState.SendFileEnd:
			if (cmd == 117)
			{
				ResFileDataInfo resFileDataInfo3 = (ResFileDataInfo)data;
				string title = ((GD.Inst.CurrLang == 1) ? "提示" : "Prompt");
				string desc3 = ((GD.Inst.CurrLang == 1) ? "文件上传成功" : "File upload successfully");
				CommModalFrm commModalFrm = new CommModalFrm();
				string text4 = Encoding.ASCII.GetString(resFileDataInfo3.Detail).TrimEnd(new char[1]);
				if (resFileDataInfo3.Status != 0 || text4 != "OK")
				{
					desc3 = Encoding.ASCII.GetString(resFileDataInfo3.Detail).TrimEnd(new char[1]);
					IsSendJson_RCMode = false;
					Fail(desc3);
				}
				else if (IsSendJson_RCMode)
				{
					UpgProcHappenEventArgs e4 = new UpgProcHappenEventArgs
					{
						infoType = InfoType.info,
						Desc = desc3
					};
					OnUpgProcHappenEvent?.Invoke(null, e4);
					IsSendJson_RCMode = false;
					commModalFrm.SetAllTxt(title, desc3, isshowBtnOK: true, isshowBtnCan: false);
					((Form)commModalFrm).ShowDialog();
					NextState(UpgState.UpgradeStatus);
				}
				else if (_isAutoSendJson)
				{
					UpgProcHappenEventArgs e5 = new UpgProcHappenEventArgs
					{
						infoType = InfoType.info,
						Desc = desc3
					};
					OnUpgProcHappenEvent?.Invoke(null, e5);
					_isAutoSendJson = false;
					commModalFrm.SetAllTxt(title, desc3, isshowBtnOK: true, isshowBtnCan: false);
					((Form)commModalFrm).ShowDialog();
					NextState(UpgState.UpgradeStatus);
				}
				else
				{
					NextState(UpgState.UpgradeStatus);
					UpgProcHappenEventArgs e6 = new UpgProcHappenEventArgs
					{
						infoType = InfoType.debugInfo,
						Desc = ((GD.Inst.CurrLang == 1) ? "检测升级状态" : "Check upgrade status"),
						Percent = 0.65f
					};
					OnUpgProcHappenEvent?.Invoke(null, e6);
				}
			}
			break;
		case UpgState.UpgradeStatus:
			if (cmd == 118 && IsAutoUpgrade)
			{
				float num = 0f;
				ResUpgradeStatus resUpgradeStatus = (ResUpgradeStatus)data;
				if (QueryUpgradeStatustimes > QueryUpgradeStatustimesLimit)
				{
					Fail("查询升级状态次数超限");
				}
				else if (resUpgradeStatus.Percent > 99)
				{
					UpgProcHappenEventArgs e2 = new UpgProcHappenEventArgs
					{
						infoType = InfoType.debugInfo,
						Desc = ((GD.Inst.CurrLang == 1) ? "升级成功,设备自动重启中" : "Upgrade successful, device is auto restarting"),
						Percent = 1f
					};
					OnUpgProcHappenEvent?.Invoke(null, e2);
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
					UpgProcHappenEventArgs e3 = new UpgProcHappenEventArgs
					{
						infoType = InfoType.procesInfo,
						Desc = ((GD.Inst.CurrLang == 1) ? "载入固件中..." : "Loading firmware"),
						upgState = UpgState.UpgradeStatus
					};
					OnUpgProcHappenEvent?.Invoke(null, e3);
				}
			}
			break;
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
				ResFileDataInfo resFileDataInfo4 = (ResFileDataInfo)data;
				NextState(UpgState.SendFileData_Json);
			}
			break;
		case UpgState.SendFileEnd_Json:
			if (cmd == 117)
			{
				ResFileDataInfo resFileDataInfo = (ResFileDataInfo)data;
				string text2 = ((GD.Inst.CurrLang == 1) ? "提示" : "Prompt");
				string desc2 = ((GD.Inst.CurrLang == 1) ? "上传完成，设备重启中" : "Upload complete, device restarting");
				string text3 = Encoding.ASCII.GetString(resFileDataInfo.Detail).TrimEnd(new char[1]);
				UpgProcHappenEventArgs e = new UpgProcHappenEventArgs
				{
					infoType = InfoType.info,
					Desc = desc2,
					Percent = 0.85f
				};
				OnUpgProcHappenEvent?.Invoke(null, e);
				_isAutoSendJson = false;
				NextState(UpgState.Completed_Json);
			}
			break;
		case UpgState.Completed:
		case UpgState.Failed:
			break;
		}
	}

	private void ShowUnkonwnCMD_PromptWindows(UpgState state, uint cmd, string desc)
	{
		WriteLog.WriteLogFileToUI(desc, Color.Red);
	}

	private ArFileInfo BuildFileInfo()
	{
		ArFileInfo result = new ArFileInfo
		{
			MD5 = new byte[64],
			filePath = new byte[128],
			fileDir = new byte[128],
			length = (int)_fileLen,
			saveAsFile = 1
		};
		string fileName = Path.GetFileName(FilePath);
		byte[] bytes = Encoding.ASCII.GetBytes("/tmp/pc/" + fileName);
		Array.Copy(bytes, result.filePath, bytes.Length);
		return result;
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
		_fs?.Close();
	}

	public int BulidFileDataStruct(string path, out ArFileInfo fileinfo)
	{
		int result = -1;
		fileinfo = default(ArFileInfo);
		fileinfo.MD5 = new byte[64];
		fileinfo.filePath = new byte[128];
		fileinfo.fileDir = new byte[128];
		try
		{
			int length = (int)_fileLen;
			MD5 mD = MD5.Create();
			using (FileStream inputStream = File.OpenRead(path))
			{
				byte[] array = mD.ComputeHash(inputStream);
				string s = BitConverter.ToString(array).Replace("-", "").ToLower();
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				Array.Copy(bytes, fileinfo.MD5, bytes.Length);
			}
			fileinfo.length = length;
			string s2 = "/tmp/pc/" + Path.GetFileName(path);
			byte[] bytes2 = Encoding.ASCII.GetBytes(s2);
			Array.Copy(bytes2, fileinfo.filePath, bytes2.Length);
			string text = path.Replace('\\', '/');
			string s3 = text;
			byte[] bytes3 = Encoding.ASCII.GetBytes(s3);
			string text2 = StaticMethod.BytesToHexString(bytes3);
			Array.Copy(bytes3, fileinfo.fileDir, bytes3.Length);
			fileinfo.saveAsFile = 1;
			return 1;
		}
		catch (Exception)
		{
			return result;
		}
	}

	public int BulidFileDataStruct_RCMode(string path, out ArFileInfo fileinfo)
	{
		int result = -1;
		fileinfo = default(ArFileInfo);
		fileinfo.MD5 = new byte[64];
		fileinfo.filePath = new byte[128];
		fileinfo.fileDir = new byte[128];
		try
		{
			int length = (int)_fileLen;
			MD5 mD = MD5.Create();
			FileStream inputStream = File.OpenRead(path);
			byte[] array = mD.ComputeHash(inputStream);
			string s = BitConverter.ToString(array).Replace("-", "").ToLower();
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			Array.Copy(bytes, fileinfo.MD5, bytes.Length);
			fileinfo.length = length;
			string s2 = "/factory/" + Path.GetFileName(path);
			byte[] bytes2 = Encoding.ASCII.GetBytes(s2);
			Array.Copy(bytes2, fileinfo.filePath, bytes2.Length);
			string text = path.Replace('\\', '/');
			string s3 = text;
			byte[] bytes3 = Encoding.ASCII.GetBytes(s3);
			string text2 = StaticMethod.BytesToHexString(bytes3);
			Array.Copy(bytes3, fileinfo.fileDir, bytes3.Length);
			fileinfo.saveAsFile = 1;
			return 1;
		}
		catch (Exception)
		{
			return result;
		}
	}

	public int BulidFileDataStruct_Hub(string path, out ArFileInfo fileinfo)
	{
		int result = -1;
		fileinfo = default(ArFileInfo);
		fileinfo.MD5 = new byte[64];
		fileinfo.filePath = new byte[128];
		fileinfo.fileDir = new byte[128];
		try
		{
			int length = (int)_fileLen;
			MD5 mD = MD5.Create();
			FileStream inputStream = File.OpenRead(path);
			byte[] array = mD.ComputeHash(inputStream);
			string s = BitConverter.ToString(array).Replace("-", "").ToLower();
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			Array.Copy(bytes, fileinfo.MD5, bytes.Length);
			fileinfo.length = length;
			string s2 = "/factory/" + Path.GetFileName(path);
			byte[] bytes2 = Encoding.ASCII.GetBytes(s2);
			Array.Copy(bytes2, fileinfo.filePath, bytes2.Length);
			string text = path.Replace('\\', '/');
			string s3 = text;
			byte[] bytes3 = Encoding.ASCII.GetBytes(s3);
			string text2 = StaticMethod.BytesToHexString(bytes3);
			Array.Copy(bytes3, fileinfo.fileDir, bytes3.Length);
			fileinfo.saveAsFile = 1;
			return 1;
		}
		catch (Exception)
		{
			return result;
		}
	}
}
