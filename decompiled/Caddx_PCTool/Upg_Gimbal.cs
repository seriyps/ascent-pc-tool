using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class Upg_Gimbal
{
	private const int CHUNK_SIZE = 256;

	private const byte FRAME_HEADER = 234;

	private const byte FRAME_TAIL = 85;

	private readonly string _className;

	private readonly Serialport_Gimbal _usb;

	private CancellationTokenSource _ctsTimeout;

	private CancellationTokenSource _ctsGryoCalibTimeout;

	private CancellationTokenSource _ctsUpgradeTimeout;

	private CancellationTokenSource _ctsDataStall;

	private DateTime _lastDataActivityTime;

	private uint _lastFwOffset = uint.MaxValue;

	private int _fwOffsetRepeatCount = 0;

	private FileStream _fs;

	private uint _fileLen;

	private int _sent;

	private int _chunkSize = 1048576;

	private int _plusing = 0;

	private uint _seq = 0u;

	private bool _isFirst = false;

	private byte _currCMD;

	private bool _isCalibGryo = false;

	private byte[] _fwData = new byte[0];

	private int _sendCommDelay = 20;

	private int _sendCalibDelay = 70;

	private int _findDeviceDelay = 2000;

	private string firmwareFilePath;

	private byte[] _firmwareData;

	private long fileSize;

	private long sentSize;

	private bool _isUpgrade;

	private int lastOffset;

	private int _calibPlusing = 0;

	private int _upgPlusing = 0;

	private int QueryUpgradeStatustimes = 0;

	private readonly int QueryUpgradeStatustimesLimit = 200;

	private uint TimeoutPeriod = 8000u;

	private uint[] _crcTable = new uint[256];

	public int UpgradeTotalTimeoutMs { get; set; } = 300000;

	public int UpgradeStallTimeoutMs { get; set; } = 30000;

	public int FwOffsetRepeatMaxCount { get; set; } = 300;

	public bool IsFirst => _isFirst;

	public bool IsAutoUpgrade => _isUpgrade;

	public event EventHandler<HappenEventArgs> OnUpgProcHappenEvent;

	public event Action<bool, RecvPacket_Gim, object> DeviceFrameInfo;

	public Upg_Gimbal(Serialport_Gimbal usb)
	{
		_isFirst = true;
		_usb = usb;
		_usb.ResFrameInfo += ResFrameInfo;
		_usb.ResUpgradeInfo += ResUpgradeInfo;
		_usb.ResUpgradeInfo_Asce += ResUpgradeInfo_Asce;
		InitCrcTable();
	}

	private void OnAck(uint cmd, object data, object data2)
	{
		switch ((AR_COMMAND)cmd)
		{
		case AR_COMMAND.AR_COMMAND_FIND_DEVICE:
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
			break;
		}
		case AR_COMMAND.AR_COMMAND_REBOOT:
			NextState(UpgState.WaitCleanOnline);
			break;
		case AR_COMMAND.AR_COMMAND_REMOTE_UPGRADE:
			if (IsAutoUpgrade)
			{
				NextState(UpgState.SendFileStart);
			}
			break;
		case AR_COMMAND.AR_CMD_SENDFILE_START:
			if (IsAutoUpgrade)
			{
				NextState(UpgState.SendFileData);
			}
			else
			{
				StopTimeout();
			}
			break;
		case AR_COMMAND.AR_CMD_SENDFILE_DATA:
		{
			ResFileDataInfo resFileDataInfo = (ResFileDataInfo)data;
			NextState(UpgState.SendFileData);
			break;
		}
		case AR_COMMAND.AR_CMD_SENDFILE_END:
		{
			ResFileDataInfo resFileDataInfo2 = (ResFileDataInfo)data;
			string text = Lang.T("common.title_prompt");
			string text2 = Lang.T("gimbal.file_upload_success");
			CommModalFrm commModalFrm = new CommModalFrm();
			string text3 = Encoding.ASCII.GetString(resFileDataInfo2.Detail).TrimEnd(new char[1]);
			if (resFileDataInfo2.Status != 0 || text3 != "OK")
			{
				text2 = Encoding.ASCII.GetString(resFileDataInfo2.Detail).TrimEnd(new char[1]);
				Fail(text2);
			}
			else
			{
				NextState(UpgState.UpgradeStatus);
			}
			break;
		}
		case AR_COMMAND.AR_CMD_UPGRADE_STATUS:
			if (IsAutoUpgrade)
			{
				float num = 0f;
				ResUpgradeStatus resUpgradeStatus = (ResUpgradeStatus)data;
				if (QueryUpgradeStatustimes > QueryUpgradeStatustimesLimit)
				{
					Fail("查询升级状态次数超限");
				}
				else if (resUpgradeStatus.Percent > 99)
				{
					Thread.Sleep(500);
					NextState(UpgState.Completed);
				}
				else
				{
					NextState(UpgState.UpgradeStatus);
				}
			}
			break;
		}
	}

	private void SetUpgradeMode(bool upgrading)
	{
		_isUpgrade = upgrading;
		if (_usb != null)
		{
			_usb.IsUpgrading = upgrading;
		}
	}

	private void Fail(string msg)
	{
		QueryUpgradeStatustimes = 0;
		StopTimeout();
		SetUpgradeMode(upgrading: false);
		WriteLog.WriteLogFileToUI(msg, Color.Red);
		_fs?.Dispose();
		_fs = null;
	}

	private void ResFrameInfo(byte[] arg1, object arg2, object arg3)
	{
		StopTimeout();
		WriteLog.WriteLogFileToUI($"收到返回帧，index={arg1[3]},_isCalibGryo={_isCalibGryo.ToString()}", Color.Black);
		JudgleIdx(arg1);
	}

	private async void JudgleIdx(byte[] frame)
	{
		RecvPacket_Gim arg2 = new RecvPacket_Gim();
		if (!ParseCommData(frame, out arg2))
		{
			WriteLog.WriteLogFileToUI("解析返回帧失败，数据无效", Color.DarkRed);
			return;
		}
		DeviceFrameInfo?.Invoke(GD.Inst.IsAutoRefreshGimData, arg2, null);
		switch ((GIM_CMD)_currCMD)
		{
		case GIM_CMD.CalibGyro:
		{
			StopGryoCalibTimeout();
			HappenEventArgs upe;
			if (arg2.index != 6 && _isCalibGryo)
			{
				_calibPlusing++;
				BuildSendPacket_Delay(GIM_CMD.CalibGyro, 100);
				upe = new HappenEventArgs
				{
					infoType = InfoType.procesInfo,
					msg = "CalibGyro",
					className = _className
				};
				OnUpgProcHappenEvent?.Invoke(null, upe);
				break;
			}
			GD.Inst.SW_Calib.Stop();
			StopTimeout();
			_isCalibGryo = false;
			for (int i = 0; i < 5; i++)
			{
				BuildSendPacket_Delay(GIM_CMD.StartGim, 100);
			}
			WriteLog.WriteLogFileToUI($"陀螺仪校准ct(ms)={GD.Inst.SW_Calib.ElapsedMilliseconds}", Color.OrangeRed);
			upe = new HappenEventArgs
			{
				infoType = InfoType.procesInfo,
				msg = "CalibGyroFinsh",
				className = _className
			};
			OnUpgProcHappenEvent?.Invoke(null, upe);
			break;
		}
		case GIM_CMD.StartGim:
			if (arg2.index != 4)
			{
				await Task.Delay(_sendCommDelay);
			}
			break;
		case GIM_CMD.StopGim:
			if (arg2.index != 5)
			{
				await Task.Delay(_sendCommDelay);
			}
			break;
		case GIM_CMD.WriteParam:
			if (arg2.index != 14)
			{
				await Task.Delay(_sendCommDelay);
			}
			break;
		case GIM_CMD.SetGMMode:
			if (arg2.index != 1)
			{
				await Task.Delay(_sendCommDelay);
			}
			break;
		case GIM_CMD.SetSensNum:
			if (arg2.index != 1)
			{
				await Task.Delay(_sendCommDelay);
			}
			break;
		case GIM_CMD.SetRollNum:
			if (arg2.index != 23)
			{
				await Task.Delay(_sendCommDelay);
			}
			break;
		case GIM_CMD.SetPitchNum:
			if (arg2.index != 24)
			{
				await Task.Delay(_sendCommDelay);
			}
			break;
		case GIM_CMD.SetYawNum:
			if (arg2.index != 25)
			{
				await Task.Delay(_sendCommDelay);
			}
			break;
		case GIM_CMD.PosCalib:
			PosCalibCommModal(arg2.index);
			break;
		}
	}

	private void PosCalibCommModal(byte reIdx)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		string title;
		string desc;
		Color color;
		switch (reIdx)
		{
		case 36:
			title = "Info";
			desc = "Position calibration successful, compensation saved to Flash.";
			color = Color.Green;
			break;
		case 37:
			title = "Error prompt";
			desc = "Position calibration failed: Self-test incomplete, or not in FPV mode.";
			color = Color.Red;
			break;
		case 38:
			title = "Info";
			desc = "Legacy firmware compatibility notice: Current firmware will no longer return this Euler angle limit.";
			color = Color.White;
			break;
		case 39:
			title = "Error prompt";
			desc = "Position calibration failed: Invalid offset, or exceeds allowable save range.";
			color = Color.Red;
			break;
		default:
			title = "Error prompt";
			desc = "Unknown error occurred.";
			color = Color.Red;
			break;
		}
		CommModalFrm commModalFrm = new CommModalFrm();
		commModalFrm.SetAllTxt(title, desc, color);
		((Form)commModalFrm).ShowDialog();
	}

	private bool ParseCommData(byte[] recBuffer, out RecvPacket_Gim packet)
	{
		bool result = false;
		packet = new RecvPacket_Gim();
		try
		{
			if (recBuffer[0] != 171 || recBuffer[1] != byte.MaxValue)
			{
				WriteLog.WriteLogFileToUI("无效的帧头", Color.Red);
				return false;
			}
			byte b = recBuffer[4];
			if (b != 110 && b != 113 && b != 117 && b != 134)
			{
				WriteLog.WriteLogFileToUI($"数据长度不匹配,length={b}", Color.Red);
				return false;
			}
			packet.version = (float)(int)recBuffer[5] * 0.1f;
			packet.index = recBuffer[3];
			packet.a = StaticMethod.ReadFloat(recBuffer, 6);
			packet.b = StaticMethod.ReadFloat(recBuffer, 10);
			packet.c = StaticMethod.ReadFloat(recBuffer, 14);
			packet.roll = StaticMethod.ReadFloat(recBuffer, 18);
			packet.pitch = StaticMethod.ReadFloat(recBuffer, 22);
			packet.yaw = StaticMethod.ReadFloat(recBuffer, 26);
			packet.x = StaticMethod.ReadFloat(recBuffer, 30);
			packet.y = StaticMethod.ReadFloat(recBuffer, 34);
			packet.z = StaticMethod.ReadFloat(recBuffer, 38);
			packet.accx = StaticMethod.ReadFloat(recBuffer, 42);
			packet.accy = StaticMethod.ReadFloat(recBuffer, 46);
			packet.accz = StaticMethod.ReadFloat(recBuffer, 50);
			for (int i = 0; i < 16; i++)
			{
				packet.channels[i] = StaticMethod.ReadUInt16(recBuffer, 54 + i * 2);
			}
			packet.modeChann = recBuffer[86];
			packet.sensChann = recBuffer[87];
			packet.rollChann = recBuffer[88];
			packet.pitchChann = recBuffer[89];
			packet.yawChann = recBuffer[90];
			packet.switchMode = recBuffer[91];
			packet.ID[0] = StaticMethod.ReadUInt32(recBuffer, 92);
			packet.ID[1] = StaticMethod.ReadUInt32(recBuffer, 96);
			packet.ID[2] = StaticMethod.ReadUInt32(recBuffer, 100);
			packet.rollgain = recBuffer[104];
			packet.pitchgain = recBuffer[105];
			packet.yawgain = recBuffer[106];
			packet.CMD = recBuffer[107];
			packet.sensitivity = StaticMethod.ReadFloat(recBuffer, 108);
			packet.temp = StaticMethod.ReadFloat(recBuffer, 112);
			packet.sumcheck = recBuffer[116];
			packet.addcheck = recBuffer[117];
			if (recBuffer.Length == 125)
			{
				packet.sensitivity = StaticMethod.ReadFloat(recBuffer, 111);
				packet.temp = StaticMethod.ReadFloat(recBuffer, 115);
				packet.currMode = recBuffer[110];
				packet.angleProtectEnable = recBuffer[119];
				packet.m0Mode = recBuffer[120];
				packet.m1Mode = recBuffer[121];
				packet.m2Mode = recBuffer[122];
				packet.sumcheck = recBuffer[123];
				packet.addcheck = recBuffer[124];
			}
			else if (recBuffer.Length == 142)
			{
				packet.sensitivity = StaticMethod.ReadFloat(recBuffer, 111);
				packet.temp = StaticMethod.ReadFloat(recBuffer, 115);
				packet.currMode = recBuffer[110];
				packet.angleProtectEnable = recBuffer[119];
				packet.m0Mode = recBuffer[120];
				packet.m1Mode = recBuffer[121];
				packet.m2Mode = recBuffer[122];
				packet.lensProfile = recBuffer[139];
				packet.sumcheck = recBuffer[140];
				packet.addcheck = recBuffer[141];
			}
			byte b2 = 0;
			byte b3 = 0;
			return true;
		}
		catch (Exception)
		{
			return result;
		}
	}

	private void ResUpgradeInfo(byte[] arg1, object arg2, object arg3)
	{
		try
		{
			ResetDataStallWatch();
			string text = arg2.ToString();
			if (!_isUpgrade)
			{
				return;
			}
			uint num = (uint)(arg1[11] | (arg1[12] << 8) | (arg1[13] << 16) | (arg1[14] << 24));
			uint num2 = arg1[15];
			if (num == _lastFwOffset)
			{
				_fwOffsetRepeatCount++;
			}
			else
			{
				_lastFwOffset = num;
				_fwOffsetRepeatCount = 1;
			}
			if (_fwOffsetRepeatCount > FwOffsetRepeatMaxCount)
			{
				SetUpgradeMode(upgrading: false);
				StopUpgradeTimeout();
				StopDataStallWatch();
				WriteLog.WriteLogFileToUI($"升级失败：fwOffset={num} 连续{_fwOffsetRepeatCount}次未更新，超过上限{FwOffsetRepeatMaxCount}次", Color.Red);
				HappenEventArgs e = new HappenEventArgs
				{
					infoType = InfoType.fail,
					msg = ((GD.Inst.CurrLang == 1) ? $"升级失败，偏移量{num}连续{_fwOffsetRepeatCount}次未前进" : $"Upgrade fail, offset {num} repeated {_fwOffsetRepeatCount} times"),
					className = "Offset repetitions exceeded limit",
					eventType = EventType.log
				};
				OnUpgProcHappenEvent?.Invoke(null, e);
			}
			else
			{
				SendFirmwareChunk((int)num, (int)num2);
				if (_upgPlusing % 10 == 1)
				{
					WriteLog.WriteLogFileToUI($"设备请求固件数据，offset={num},length={num2}", Color.DarkBlue);
				}
				_upgPlusing++;
			}
		}
		catch (Exception)
		{
		}
	}

	private void ResUpgradeInfo_Asce(byte[] arg1, object arg2, object arg3)
	{
		try
		{
			uint num = (uint)arg3;
			string text = "";
			switch ((AR_COMMAND)num)
			{
			case AR_COMMAND.AR_COMMAND_FIND_DEVICE:
			{
				byte[] frame = ExtractAscentPayload(arg1);
				ResDeviceInfo resDeviceInfo = ParseDeviceInfo(frame);
				_chunkSize = resDeviceInfo.receiveMaxSize;
				HappenEventArgs e = new HappenEventArgs();
				e.infoType = InfoType.upgProcInfo;
				e.msg = "UpgradeComplet";
				e.className = _className;
				e.DevName = Encoding.ASCII.GetString(resDeviceInfo.devicename).TrimEnd(new char[1]);
				e.eventDesc = resDeviceInfo.ToString();
				HappenEventArgs e2 = e;
				OnUpgProcHappenEvent?.Invoke(resDeviceInfo, e2);
				break;
			}
			case AR_COMMAND.AR_COMMAND_REBOOT:
			{
				ResAckInfo resAckInfo = ParseAckInfo(arg1);
				text = Encoding.ASCII.GetString(resAckInfo.Detail).Trim(new char[1]);
				break;
			}
			case AR_COMMAND.AR_COMMAND_REMOTE_UPGRADE:
			{
				ResAckInfo resAckInfo = ParseAckInfo(arg1);
				text = Encoding.ASCII.GetString(resAckInfo.Detail).Trim(new char[1]);
				break;
			}
			case AR_COMMAND.AR_CMD_SENDFILE_START:
			{
				ResAckInfo resAckInfo = ParseAckInfo(arg1);
				text = Encoding.ASCII.GetString(resAckInfo.Detail).Trim(new char[1]);
				if (resAckInfo.Status == 0)
				{
					NextState(UpgState.SendFileData);
				}
				else
				{
					WriteLog.WriteLogFileToUI("SENDFILE_START error,detail=" + text, Color.Red);
				}
				break;
			}
			case AR_COMMAND.AR_CMD_SENDFILE_DATA:
			{
				ResFileDataInfo resFileDataInfo2 = ParseDataInfo(arg1);
				text = Encoding.ASCII.GetString(resFileDataInfo2.Detail).Trim(new char[1]);
				if (resFileDataInfo2.Status == 0)
				{
					NextState(UpgState.SendFileData);
				}
				else
				{
					WriteLog.WriteLogFileToUI("SENDFILE_DATA error,detail=" + text, Color.Red);
				}
				break;
			}
			case AR_COMMAND.AR_CMD_SENDFILE_END:
			{
				ResFileDataInfo resFileDataInfo = ParseDataInfo(arg1);
				break;
			}
			case AR_COMMAND.AR_CMD_UPGRADE_STATUS:
				ParseUpgradeStatus(arg1);
				break;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI($"解析Ascent返回帧异常,cmd={arg3},desc={ex.Message}", Color.Red);
		}
	}

	public void SetFile(string path, out long filesize)
	{
		firmwareFilePath = path;
		using (_fs = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			_fileLen = (uint)_fs.Length;
			filesize = _fs.Length;
			_sent = 0;
			_firmwareData = new byte[_fs.Length];
			int num = _fs.Read(_firmwareData, 0, (int)_fs.Length);
			fileSize = _firmwareData.Length;
		}
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
					ACKTimeout($"状态 {_currCMD} 超时", _currCMD);
				}
			}
			catch
			{
			}
		});
	}

	private void StartGryoCalibTimeout(uint ms, string msg)
	{
		_ctsGryoCalibTimeout?.Cancel();
		_ctsGryoCalibTimeout = new CancellationTokenSource();
		CancellationToken token = _ctsGryoCalibTimeout.Token;
		Task.Run(async delegate
		{
			try
			{
				await Task.Delay((int)ms, token);
				if (!token.IsCancellationRequested)
				{
					GyroCalibTimeout("CalibGyro timeout", _currCMD, nouse: false);
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

	private void StopGryoCalibTimeout()
	{
		_ctsTimeout?.Cancel();
	}

	private void StartUpgradeTimeout()
	{
		StopUpgradeTimeout();
		_ctsUpgradeTimeout = new CancellationTokenSource();
		CancellationToken token = _ctsUpgradeTimeout.Token;
		int timeoutMs = UpgradeTotalTimeoutMs;
		Task.Run(async delegate
		{
			try
			{
				await Task.Delay(timeoutMs, token);
				if (!token.IsCancellationRequested)
				{
					SetUpgradeMode(upgrading: false);
					StopDataStallWatch();
					WriteLog.WriteLogFileToUI($"升级总超时（{timeoutMs / 1000}秒），升级失败", Color.Red);
					HappenEventArgs upe = new HappenEventArgs
					{
						infoType = InfoType.fail,
						msg = ((GD.Inst.CurrLang == 1) ? $"升级超时，已超过{timeoutMs / 1000}秒未完成" : $"Upgrade timeout, exceeded {timeoutMs / 1000}s"),
						className = " Total upgrade time timeout",
						eventType = EventType.log
					};
					OnUpgProcHappenEvent?.Invoke(null, upe);
				}
			}
			catch
			{
			}
		});
	}

	private void StopUpgradeTimeout()
	{
		_ctsUpgradeTimeout?.Cancel();
	}

	private void StartDataStallWatch()
	{
		StopDataStallWatch();
		_lastDataActivityTime = DateTime.Now;
		_ctsDataStall = new CancellationTokenSource();
		CancellationToken token = _ctsDataStall.Token;
		int stallMs = UpgradeStallTimeoutMs;
		Task.Run(async delegate
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					await Task.Delay(1000, token);
					if (token.IsCancellationRequested)
					{
						break;
					}
					double idleMs = (DateTime.Now - _lastDataActivityTime).TotalMilliseconds;
					if (idleMs > (double)stallMs)
					{
						SetUpgradeMode(upgrading: false);
						StopUpgradeTimeout();
						WriteLog.WriteLogFileToUI($"升级数据停滞（{idleMs / 1000.0:F1}秒无数据交互），升级失败", Color.Red);
						HappenEventArgs upe = new HappenEventArgs
						{
							infoType = InfoType.fail,
							msg = ((GD.Inst.CurrLang == 1) ? $"升级停滞，{stallMs / 1000}秒内无数据交互" : $"Upgrade stalled, no data for {stallMs / 1000}s"),
							className = "Return frame timeout",
							eventType = EventType.log
						};
						OnUpgProcHappenEvent?.Invoke(null, upe);
						break;
					}
				}
			}
			catch
			{
			}
		});
	}

	private void ResetDataStallWatch()
	{
		_lastDataActivityTime = DateTime.Now;
	}

	private void StopDataStallWatch()
	{
		_ctsDataStall?.Cancel();
	}

	public void StartUpgrade_Ascent(string path)
	{
		SetUpgradeMode(upgrading: true);
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			HappenEventArgs e = new HappenEventArgs
			{
				infoType = InfoType.fail,
				msg = "升级文件不存在,固件路径有误",
				className = _className,
				eventType = EventType.log
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
		}
		else
		{
			SetFile(path, out var _);
			NextState(UpgState.SendFileStart);
		}
	}

	public void StartUpgrade(string path)
	{
		SetUpgradeMode(upgrading: true);
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			HappenEventArgs e = new HappenEventArgs
			{
				infoType = InfoType.fail,
				msg = "升级文件不存在,固件路径有误",
				className = _className,
				eventType = EventType.log
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
		}
		else
		{
			SetFile(path, out var _);
			StartUpgradeTimeout();
			StartDataStallWatch();
			if (GD.Inst.IsOpenAscentUpg)
			{
				StartUpgrade_Ascent(path);
			}
			else
			{
				Send_StartUpgrade();
			}
		}
	}

	private void Send_StartUpgrade()
	{
		try
		{
			lastOffset = -1;
			List<byte> list = new List<byte>();
			list.Add(234);
			list.Add(85);
			list.Add(17);
			list.Add(30);
			list.Add(65);
			list.Add(0);
			byte[] bytes = BitConverter.GetBytes(_fileLen);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse((Array)bytes);
			}
			list.AddRange(bytes);
			uint value = CRC32_Gimbal.Calculate(_firmwareData);
			byte[] bytes2 = BitConverter.GetBytes(value);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse((Array)bytes2);
			}
			list.AddRange(bytes2);
			list.Add(0);
			ushort value2 = CRC16_Gimbal.Calculate(list.ToArray());
			byte[] bytes3 = BitConverter.GetBytes(value2);
			list.AddRange(bytes3);
			string arg = StaticMethod.BytesToHexString(list.ToArray());
			WriteLog.WriteLogFileToUI($"发送升级指令，文件大小={_fileLen}字节，byteArr={arg}", Color.Green);
			_usb.SendMsg(list.ToArray());
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI($"Send_StartUpgrade error，desc={ex.Message} 文件大小={_fileLen}byte", Color.Red);
		}
	}

	private void SendFirmwareChunk(int offset, int size)
	{
		if (!_isUpgrade)
		{
			WriteLog.WriteLogFileToUI("未发送启动升级帧", Color.OrangeRed);
			return;
		}
		if (offset + size > _firmwareData.Length)
		{
			WriteLog.WriteLogFileToUI("请求的固件数据超出文件大小!", Color.DarkRed);
			return;
		}
		List<byte> list = new List<byte>();
		byte[] array = new byte[size];
		Array.Copy(_firmwareData, offset, array, 0, size);
		list.Add(234);
		list.Add(85);
		if (array.Length < 48)
		{
			list.Add((byte)(65 - (48 - array.Length)));
		}
		else
		{
			list.Add(65);
		}
		list.Add(32);
		list.Add(65);
		list.Add(0);
		list.Add(1);
		list.AddRange(array);
		byte[] bytes = BitConverter.GetBytes(offset);
		if (!BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		list.AddRange(bytes);
		byte[] bytes2 = BitConverter.GetBytes(size);
		if (!BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes2);
		}
		list.AddRange(bytes2);
		ushort value = CRC16_Gimbal.Calculate(list.ToArray());
		byte[] bytes3 = BitConverter.GetBytes(value);
		if (!BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes3);
		}
		list.AddRange(bytes3);
		_usb.SendMsg(list.ToArray());
		if (offset > lastOffset)
		{
			sentSize += size;
			lastOffset = offset;
		}
		int num = Math.Min((int)(sentSize * 100 / fileSize), 100);
		float num2 = (float)num / 100f;
		if ((double)num2 > 0.02)
		{
			HappenEventArgs e = new HappenEventArgs
			{
				infoType = InfoType.upgProcInfo,
				msg = "process",
				className = _className,
				eventType = EventType.ackTrigger,
				processVal = num2
			};
			OnUpgProcHappenEvent?.Invoke(null, e);
		}
		if (num >= 100)
		{
			StopUpgradeTimeout();
			StopDataStallWatch();
			SendUpgradeFinish();
			sentSize = (_upgPlusing = (num = 0));
			HappenEventArgs e2 = new HappenEventArgs
			{
				infoType = InfoType.upgProcInfo,
				msg = "procFinish",
				className = _className,
				eventType = EventType.ackTrigger,
				processVal = 1f,
				processDesc = Lang.T("gimbal.upgrade_complete")
			};
			OnUpgProcHappenEvent?.Invoke(null, e2);
		}
	}

	private void SendUpgradeFinish()
	{
		SetUpgradeMode(upgrading: false);
	}

	public void NextState(UpgState next)
	{
		switch (next)
		{
		case UpgState.FindDevice_Normal:
			break;
		case UpgState.Reboot_Clean:
			break;
		case UpgState.WaitCleanOnline:
			break;
		case UpgState.RemoteUpgrade:
			Send_RemoteUpgrade();
			break;
		case UpgState.SendFileStart:
			Send_SENDFILE_START();
			break;
		case UpgState.SendFileData:
			Send_SENDFILE_DATA();
			break;
		case UpgState.SendFileEnd:
			Send_SENDFILE_END();
			break;
		case UpgState.UpgradeStatus:
			Send_UPGRADE_STATUS();
			break;
		case UpgState.Completed:
			SetUpgradeMode(upgrading: false);
			break;
		case UpgState.Failed:
			SetUpgradeMode(upgrading: false);
			break;
		}
	}

	public void Send_FindDevice()
	{
		Task.Run(async delegate
		{
			try
			{
				await Task.Delay(_findDeviceDelay);
				if (Ascent_SendPacket(60u, 1, 2, 0, null, 0u, 0u, 2000) != 0)
				{
					Fail("发送 FIND_DEVICE 失败");
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				Fail("发送 FIND_DEVICE 异常: " + ex2.Message);
			}
		});
	}

	public void Send_RebootClean()
	{
		byte[] array = Encoding.ASCII.GetBytes("clean");
		Array.Resize(ref array, 32);
		if (Ascent_SendPacket(3u, 1, 2, 0, array, (uint)array.Length) != 0)
		{
			Fail("发送 REBOOT 失败");
		}
	}

	public void Send_RemoteUpgrade()
	{
		if (Ascent_SendPacket(114u, 1, 2, 0, null, 0u) != 0)
		{
			Fail("发送 RemoteUpgrade 失败");
		}
	}

	public void Send_SENDFILE_START()
	{
		BulidFileDataStruct(firmwareFilePath, out var fileinfo);
		byte[] array = StaticMethod.StructToBytes(fileinfo);
		if (Ascent_SendPacket(115u, 1, 2, 0, array, (uint)array.Length) != 0)
		{
			Fail("发送 RemoteUpgrade 失败");
		}
	}

	public void Send_SENDFILE_DATA()
	{
		if (_sent >= _fileLen)
		{
			NextState(UpgState.SendFileEnd);
			return;
		}
		byte[] array = new byte[256];
		Buffer.BlockCopy(_firmwareData, _sent, array, 0, 256);
		if (Ascent_SendPacket(116u, 1, 2, 0, array, (uint)array.Length) != 0)
		{
			Fail("发送 RemoteUpgrade 失败");
		}
		_sent += 256;
	}

	public void Send_SENDFILE_END()
	{
		if (Ascent_SendPacket(117u, 1, 2, 0, null, 0u) != 0)
		{
			Fail("发送 RemoteUpgrade 失败");
		}
	}

	public void Send_UPGRADE_STATUS()
	{
		if (Ascent_SendPacket(118u, 1, 2, 0, null, 0u) != 0)
		{
			Fail("发送 RemoteUpgrade 失败");
		}
	}

	private void ACKTimeout(string msg, byte cmd)
	{
		StopTimeout();
		StopUpgradeTimeout();
		StopDataStallWatch();
		HappenEventArgs e = new HappenEventArgs
		{
			infoType = InfoType.fail,
			msg = msg,
			className = _className,
			eventType = EventType.log
		};
		OnUpgProcHappenEvent?.Invoke(this, e);
	}

	private void GyroCalibTimeout(string msg, byte cmd, bool nouse)
	{
		StopGryoCalibTimeout();
		if (_isCalibGryo && cmd == 2)
		{
			GD.Inst.SW_Calib.Stop();
			_isCalibGryo = false;
			for (int i = 0; i < 10; i++)
			{
				BuildSendPacket_Delay(GIM_CMD.CalibGyro, 100);
			}
			WriteLog.WriteLogFileToUI($"陀螺仪校准timeout，启动云台中,ct(ms)={GD.Inst.SW_Calib.ElapsedMilliseconds}", Color.Orange);
			if (!_usb.IsComOpened)
			{
			}
		}
		HappenEventArgs e = new HappenEventArgs
		{
			infoType = InfoType.fail,
			msg = msg,
			className = _className,
			eventType = EventType.log
		};
		OnUpgProcHappenEvent?.Invoke(this, e);
	}

	public int BuildSendPacket_Gimbal(GIM_CMD cmd, float rollcmd = 0f, float pitchcmd = 0f, float yawcmd = 0f)
	{
		try
		{
			_currCMD = (byte)cmd;
			if (cmd == GIM_CMD.CalibGyro)
			{
				_isCalibGryo = true;
				StartTimeout(3000u);
			}
			List<byte> list = new List<byte>();
			list.Add(126);
			list.Add(22);
			list.Add((byte)(cmd & (GIM_CMD)0xFF));
			short num = ClampToInt16(rollcmd * 100f);
			short num2 = ClampToInt16(pitchcmd * 100f);
			short num3 = ClampToInt16(yawcmd * 100f);
			list.Add((byte)((num >> 8) & 0xFF));
			list.Add((byte)(num & 0xFF));
			list.Add((byte)((num2 >> 8) & 0xFF));
			list.Add((byte)(num2 & 0xFF));
			list.Add((byte)((num3 >> 8) & 0xFF));
			list.Add((byte)(num3 & 0xFF));
			list.Add(90);
			list.Add(165);
			_usb.SendMsg(list.ToArray());
			byte b = (byte)cmd;
			if (b < 26 && b > 20)
			{
				string arg = StaticMethod.BytesToHexString(list.ToArray());
				WriteLog.WriteLogFileToUI($"cmd={b},bytes={arg}", Color.Black);
			}
			return 0;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("BuildSendPacket_Gimbal error,desc=" + ex.Message, Color.DarkRed);
			return -9;
		}
	}

	public int BuildSendPacket_Delay(GIM_CMD cmd, int delay)
	{
		try
		{
			if (cmd == GIM_CMD.CalibGyro)
			{
				_isCalibGryo = true;
				StartGryoCalibTimeout(3000u, "CalibGyro");
			}
			_currCMD = (byte)cmd;
			List<byte> list = new List<byte>();
			list.Add(126);
			list.Add(22);
			list.Add((byte)(cmd & (GIM_CMD)0xFF));
			short num = ClampToInt16(0f);
			short num2 = ClampToInt16(0f);
			short num3 = ClampToInt16(0f);
			list.Add((byte)((num >> 8) & 0xFF));
			list.Add((byte)(num & 0xFF));
			list.Add((byte)((num2 >> 8) & 0xFF));
			list.Add((byte)(num2 & 0xFF));
			list.Add((byte)((num3 >> 8) & 0xFF));
			list.Add((byte)(num3 & 0xFF));
			list.Add(90);
			list.Add(165);
			_usb.SendMsg(list.ToArray(), delay);
			byte b = (byte)cmd;
			if (b < 26 && b > 20)
			{
				string arg = StaticMethod.BytesToHexString(list.ToArray());
				WriteLog.WriteLogFileToUI($"cmd={b},bytes={arg}", Color.Black);
			}
			return 0;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("BuildSendPacket_Gimbal error,desc=" + ex.Message, Color.DarkRed);
			return -9;
		}
	}

	public int Ascent_SendPacket(uint cmd, ushort type, ushort format, ushort userId, byte[] payload, uint len, uint retryNumber = 0u, int timeoutMs = 0)
	{
		try
		{
			ArProtocolHeader obj = new ArProtocolHeader
			{
				magic = 1095914575u,
				version = 3292,
				type = type,
				msgid = 0,
				unused = 43981,
				command = cmd,
				format = format,
				userid = userId,
				length = len,
				seq = _seq,
				retry = 0u,
				crc32 = 0u
			};
			uint num = 0u;
			bool flag = false;
			obj.crc32 = 0u;
			byte[] headerBytes = StaticMethod.StructToBytes(obj);
			uint crc = Ascent_CrcCalc(headerBytes, payload);
			obj.crc32 = crc;
			byte[] collection = StaticMethod.StructToBytes(obj);
			try
			{
				List<byte> list = new List<byte>();
				list.AddRange(collection);
				if (payload != null)
				{
					list.AddRange(payload);
				}
				_usb.SendMsg(list.ToArray());
				flag = true;
			}
			catch (Exception ex)
			{
				Fail("Write failed: " + ex.Message);
				return -1;
			}
			_seq++;
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}

	private short ClampToInt16(float value)
	{
		if (value < -32768f)
		{
			return short.MinValue;
		}
		if (value > 32767f)
		{
			return short.MaxValue;
		}
		return (short)value;
	}

	private short ClampToInt16(double value)
	{
		if (value < -32768.0)
		{
			return short.MinValue;
		}
		if (value > 32767.0)
		{
			return short.MaxValue;
		}
		return (short)value;
	}

	private void OnAck(AR_COMMAND cmd, object data)
	{
	}

	public void Dispose()
	{
		StopTimeout();
		StopUpgradeTimeout();
		StopDataStallWatch();
		_fs?.Close();
		_usb?.Dispose();
		_usb.ResFrameInfo -= ResFrameInfo;
		_usb.ResUpgradeInfo -= ResUpgradeInfo;
	}

	private int BulidFileDataStruct(string path, out ArFileInfo fileinfo)
	{
		int result = -1;
		fileinfo = default(ArFileInfo);
		fileinfo.MD5 = new byte[64];
		fileinfo.filePath = new byte[128];
		fileinfo.fileDir = new byte[128];
		try
		{
			int fileLen = (int)_fileLen;
			MD5 mD = MD5.Create();
			using (FileStream inputStream = File.OpenRead(path))
			{
				byte[] array = mD.ComputeHash(inputStream);
				string s = BitConverter.ToString(array).Replace("-", "").ToLower();
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				Array.Copy(bytes, fileinfo.MD5, bytes.Length);
			}
			fileinfo.length = fileLen;
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

	private void InitCrcTable()
	{
		uint num = 3988292384u;
		for (uint num2 = 0u; num2 < 256; num2++)
		{
			uint num3 = num2;
			for (int i = 0; i < 8; i++)
			{
				num3 = (((num3 & 1) != 0) ? ((num3 >> 1) ^ num) : (num3 >> 1));
			}
			_crcTable[num2] = num3;
		}
	}

	private uint Ascent_CrcCalc(byte[] headerBytes, byte[] payload)
	{
		uint num = uint.MaxValue;
		if (headerBytes != null)
		{
			for (int i = 0; i < headerBytes.Length; i++)
			{
				num = _crcTable[(num ^ headerBytes[i]) & 0xFF] ^ (num >> 8);
			}
		}
		if (payload != null)
		{
			for (int j = 0; j < payload.Length; j++)
			{
				num = _crcTable[(num ^ payload[j]) & 0xFF] ^ (num >> 8);
			}
		}
		return num ^ 0xFFFFFFFFu;
	}

	private byte[] ExtractAscentPayload(byte[] frame)
	{
		if (frame == null || frame.Length <= 36)
		{
			return frame;
		}
		if (frame[0] != 79 || frame[1] != 84 || frame[2] != 82 || frame[3] != 65)
		{
			return frame;
		}
		byte[] array = new byte[frame.Length - 36];
		Buffer.BlockCopy(frame, 36, array, 0, array.Length);
		return array;
	}

	private ResDeviceInfo ParseDeviceInfo(byte[] frame)
	{
		ResDeviceInfo resDeviceInfo = new ResDeviceInfo();
		resDeviceInfo.receiveMaxSize = BitConverter.ToInt32(frame, 0);
		int sourceIndex = 4;
		Array.Copy(frame, sourceIndex, resDeviceInfo.sdkversion, 0, 32);
		int num = 36;
		Array.Copy(frame, num, resDeviceInfo.devicename, 0, 64);
		int num2 = num + 64;
		resDeviceInfo.cputemp = BitConverter.ToInt32(frame, num2);
		int num3 = num2 + 4;
		Array.Copy(frame, num3, resDeviceInfo.firmwareInfo, 0, 64);
		int num4 = num3 + 64;
		Array.Copy(frame, num4, resDeviceInfo.serialNumber, 0, 32);
		int num5 = num4 + 32;
		Array.Copy(frame, num5, resDeviceInfo.hardwareVersion, 0, 32);
		int num6 = num5 + 32;
		resDeviceInfo.status = BitConverter.ToInt32(frame, num6);
		int sourceIndex2 = num6 + 4;
		Array.Copy(frame, sourceIndex2, resDeviceInfo.detail, 0, 64);
		return resDeviceInfo;
	}

	private ResAckInfo ParseAckInfo(byte[] frame)
	{
		byte[] array = new byte[68];
		Array.Copy(frame, 36, array, 0, array.Length);
		ResAckInfo resAckInfo = new ResAckInfo();
		resAckInfo.Status = BitConverter.ToInt32(array, 0);
		Array.Copy(array, 4, resAckInfo.Detail, 0, 64);
		return resAckInfo;
	}

	private ResUpgradeStatus ParseUpgradeStatus(byte[] frame)
	{
		ResUpgradeStatus resUpgradeStatus = new ResUpgradeStatus();
		resUpgradeStatus.Percent = BitConverter.ToInt32(frame, 0);
		resUpgradeStatus.Status = BitConverter.ToInt32(frame, 4);
		int sourceIndex = 8;
		Array.Copy(frame, sourceIndex, resUpgradeStatus.Detail, 0, 64);
		return resUpgradeStatus;
	}

	private ResFileDataInfo ParseDataInfo(byte[] frame)
	{
		byte[] array = new byte[80];
		Array.Copy(frame, 36, array, 0, array.Length);
		ResFileDataInfo resFileDataInfo = new ResFileDataInfo();
		resFileDataInfo.Length = BitConverter.ToInt32(array, 0);
		int startIndex = 4;
		resFileDataInfo.Cursize = BitConverter.ToInt32(array, startIndex);
		int num = 8;
		resFileDataInfo.Totalsize = BitConverter.ToInt32(array, num);
		int num2 = num + 4;
		resFileDataInfo.Status = BitConverter.ToInt32(array, num2);
		int sourceIndex = num2 + 4;
		Array.Copy(array, sourceIndex, resFileDataInfo.Detail, 0, 64);
		return resFileDataInfo;
	}
}
