using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Caddx_PCTool;

public class Serialport_Gimbal : IDisposable
{
	private BlockQueue<SPEventArgs> _sendQue;

	private CancellationTokenSource _sendCts;

	private CancellationTokenSource _recvCts;

	private CancellationTokenSource _parseCts;

	private string _portName;

	private SerialPort _serial;

	private readonly object _writeLock = new object();

	private readonly RingBuffer _recvBuffer = new RingBuffer(65536);

	public int SegLength = 1024;

	public volatile bool IsUpgrading;

	private readonly byte[] _headerBuf = new byte[8];

	private readonly byte[] _headerBuf_Ascent = new byte[36];

	private readonly byte[] _frameBuf = new byte[5120];

	private readonly byte[] _crcBuf = new byte[128];

	private readonly byte[] _peekAheadBuf = new byte[4];

	private const int DATALENGTH = 125;

	private const int DATALENGTH2 = 142;

	private bool _isfirst = false;

	private DateTime _lastUpgradeDataTime = DateTime.MinValue;

	private CancellationTokenSource _monitorCts;

	private Task _monitorTask;

	private const int SerialCheckInterval = 50;

	private const int SerialStableThreshold = 150;

	private bool _lastStableState = false;

	public bool IsComOpened
	{
		get
		{
			SerialPort serial = _serial;
			return serial != null && serial.IsOpen;
		}
	}

	public SerialPort SPobj => _serial;

	public bool IsRunAutoUpgrade { get; set; }

	public bool IsFirst => _isfirst;

	public DateTime LastUpgradeDataTime => _lastUpgradeDataTime;

	public event Action<byte[], object, object> ResFrameInfo;

	public event Action<byte[], object, object> ResUpgradeInfo;

	public event Action<InfoType, string, object> OnSpGimLogEvent;

	public event Action<bool, string, int> SerialConnectedStateChange;

	public event Action<byte[], object, object> ResUpgradeInfo_Asce;

	private void StartSerialMonitor()
	{
		StopSerialMonitor();
		_monitorCts = new CancellationTokenSource();
		CancellationToken token = _monitorCts.Token;
		_monitorTask = Task.Run(async delegate
		{
			bool lastRawState = false;
			int stableTime = 0;
			while (!token.IsCancellationRequested)
			{
				bool currentState;
				try
				{
					currentState = _serial != null && _serial.IsOpen;
				}
				catch
				{
					currentState = false;
				}
				if (currentState == lastRawState)
				{
					stableTime += 50;
				}
				else
				{
					lastRawState = currentState;
					stableTime = 0;
				}
				if (stableTime >= 150 && _lastStableState != currentState)
				{
					_lastStableState = currentState;
					SerialConnectedStateChange?.Invoke(_serial.IsOpen, _serial.PortName, _serial.BaudRate);
				}
				try
				{
					await Task.Delay(50, token);
				}
				catch
				{
					break;
				}
			}
		}, token);
	}

	private void StopSerialMonitor()
	{
		try
		{
			_monitorCts?.Cancel();
		}
		catch
		{
		}
		try
		{
			_monitorTask?.Wait(100);
		}
		catch
		{
		}
		_monitorTask = null;
		_monitorCts = null;
	}

	public Serialport_Gimbal(UsbDevInfo info)
	{
		_sendCts = new CancellationTokenSource();
		_recvCts = new CancellationTokenSource();
		_parseCts = new CancellationTokenSource();
		_sendQue = new BlockQueue<SPEventArgs>(200);
		_monitorCts = new CancellationTokenSource();
	}

	public bool Open(string portName, int baudRate = 460800)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		try
		{
			_serial = new SerialPort(portName, baudRate, (Parity)0, 8, (StopBits)1);
			_serial.WriteBufferSize = 1075200;
			_serial.ReadBufferSize = 1075200;
			_serial.ReadTimeout = 5000;
			_serial.WriteTimeout = 5000;
			_serial.DataReceived -= null;
			_serial.Open();
			if (!_serial.IsOpen)
			{
				return false;
			}
			_portName = portName;
			StartSendThread();
			StartRecvThread();
			StartParseThread();
			StartSerialMonitor();
			if (!_isfirst)
			{
				_isfirst = true;
			}
			return true;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("Serialport_Gimbal.Open error,串口名=" + portName + ",desc=" + ex.Message, Color.Red);
			return false;
		}
	}

	public void Close()
	{
		StopSendThread();
		StopRecvThread();
		StopParseThread();
		StopSerialMonitor();
		if (_serial != null)
		{
			try
			{
				SerialPort serial = _serial;
				if (serial != null)
				{
					serial.Close();
				}
			}
			catch (Exception ex)
			{
				WriteLog.WriteLogFileToUI("串口关闭异常,desc=" + ex.Message, Color.Red);
			}
			_serial = null;
		}
		_recvBuffer.Clear();
	}

	private void StartSendThread()
	{
		if (_sendQue == null)
		{
			_sendQue = new BlockQueue<SPEventArgs>(200);
		}
		_sendCts = new CancellationTokenSource();
		Task.Run(() => SendQueueLoop(_sendCts.Token));
	}

	private void StopSendThread()
	{
		_sendCts?.Cancel();
	}

	public async Task SendQueueLoop(CancellationToken cts)
	{
		while (!cts.IsCancellationRequested)
		{
			try
			{
				if (!_sendQue.TryDequeue(out var spe))
				{
					await Task.Delay(1);
				}
				else if (_serial != null && _serial.IsOpen)
				{
					if (spe.DelayTime > 0)
					{
						await Task.Delay(spe.DelayTime);
					}
					_serial.Write(spe.HeaderBuffer, 0, spe.HeaderBuffer.Length);
					if (spe.DataBuffer != null && spe.DataBuffer.Length != 0)
					{
						_serial.Write(spe.DataBuffer, 0, spe.DataBuffer.Length);
					}
				}
				else
				{
					WriteLog.WriteLogFileToUI("串口未打开！", Color.Red);
				}
				spe = null;
			}
			catch (Exception ex)
			{
				WriteLog.WriteLogFileToUI("serialport send task error,desc=" + ex.Message, Color.Red);
			}
		}
	}

	private async Task SerialportSendMethod()
	{
		try
		{
			if (!_sendQue.TryDequeue(out var spe))
			{
				return;
			}
			if (_serial != null && _serial.IsOpen)
			{
				if (spe.DelayTime > 0)
				{
					await Task.Delay(spe.DelayTime);
				}
				_serial.Write(spe.HeaderBuffer, 0, spe.HeaderBuffer.Length);
				if (spe.DataBuffer != null && spe.DataBuffer.Length != 0)
				{
					_serial.Write(spe.DataBuffer, 0, spe.DataBuffer.Length);
				}
			}
			else
			{
				WriteLog.WriteLogFileToUI("串口未打开！", Color.Red);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			WriteLog.WriteLogFileToUI("串口发送异常,desc=" + ex2.Message, Color.Red);
		}
	}

	private void StartRecvThread()
	{
		_recvCts = new CancellationTokenSource();
		Task.Run(delegate
		{
			RecvLoop(_recvCts.Token);
		});
	}

	private void StopRecvThread()
	{
		_recvCts?.Cancel();
	}

	private void RecvLoop(CancellationToken token)
	{
		byte[] array = new byte[1024];
		while (!token.IsCancellationRequested)
		{
			try
			{
				if (_serial == null || !_serial.IsOpen)
				{
					Thread.Sleep(20);
					continue;
				}
				int bytesToRead = _serial.BytesToRead;
				if (bytesToRead <= 0)
				{
					Thread.Sleep(0);
					continue;
				}
				int num = Math.Min(bytesToRead, array.Length);
				int num2 = _serial.Read(array, 0, num);
				if (num2 > 0)
				{
					_recvBuffer.Write(array, 0, num2);
				}
				Thread.Sleep(0);
			}
			catch (Exception ex)
			{
				WriteLog.WriteLogFileToUI("云台串口接收异常,desc=" + ex.Message, Color.Red);
			}
		}
	}

	private void StartParseThread()
	{
		_parseCts = new CancellationTokenSource();
		Task.Run(delegate
		{
			ParseLoop(_parseCts.Token);
		});
	}

	private void StopParseThread()
	{
		_parseCts?.Cancel();
	}

	private void ParseLoop(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			try
			{
				if (_recvBuffer.Available < 4)
				{
					Thread.Sleep(0);
					continue;
				}
				ParseBuffer();
				Thread.Sleep(0);
			}
			catch (Exception ex)
			{
				WriteLog.WriteLogFileToUI("云台串口解析异常,desc=" + ex.Message, Color.Red);
			}
		}
	}

	private void ParseBuffer()
	{
		ArProtocolHeader arProtocolHeader = default(ArProtocolHeader);
		while (true)
		{
			int num = _recvBuffer.PeekHeader(_headerBuf, 8);
			if (num < 0)
			{
				break;
			}
			int num2 = 0;
			if (_headerBuf[0] == 234 && _headerBuf[1] == 85)
			{
				num2 = 17;
			}
			else if (_headerBuf[0] == 126 && _headerBuf[1] == 22)
			{
				num2 = 38;
			}
			else if (_headerBuf[0] == 171 && _headerBuf[1] == byte.MaxValue)
			{
				if (IsUpgrading)
				{
					if (num >= 142)
					{
						_recvBuffer.Skip(142);
						continue;
					}
					break;
				}
				num2 = _headerBuf[4] + 8;
			}
			else
			{
				if (_headerBuf[0] != 79 || _headerBuf[1] != 84 || _headerBuf[2] != 82 || _headerBuf[3] != 65)
				{
					_recvBuffer.Skip(1);
					continue;
				}
				int num3 = _recvBuffer.PeekHeader(_headerBuf_Ascent, 36);
				if (num3 < 36)
				{
					break;
				}
				arProtocolHeader = StaticMethod.BytesToStruct<ArProtocolHeader>(_headerBuf_Ascent);
				num2 = (int)(36 + arProtocolHeader.length);
			}
			if (num2 == 0)
			{
				_recvBuffer.Skip(1);
				continue;
			}
			if (num < num2)
			{
				break;
			}
			if (IsUpgrading && _headerBuf[0] == 126 && _headerBuf[1] == 22)
			{
				int num4 = 0;
				while (true)
				{
					int num5 = num4 + 38;
					if (num5 + 4 > num)
					{
						break;
					}
					int num6 = _recvBuffer.PeekAt(num5, _peekAheadBuf, 4);
					if (num6 >= 0 && _peekAheadBuf[0] == 126 && _peekAheadBuf[1] == 22)
					{
						num4 += 38;
						continue;
					}
					break;
				}
				if (num4 > 0)
				{
					_recvBuffer.Skip(num4);
					num = _recvBuffer.PeekHeader(_headerBuf, 4);
					if (num < 0 || num < 38)
					{
						break;
					}
				}
			}
			_recvBuffer.Peek(_frameBuf, num2);
			if (!ValidateFrame(_frameBuf, num2))
			{
				_recvBuffer.Skip(1);
				continue;
			}
			_recvBuffer.Skip(num2);
			byte[] array = new byte[num2];
			Buffer.BlockCopy(_frameBuf, 0, array, 0, num2);
			ProcessFrame(array, arProtocolHeader.command);
		}
	}

	private void ProcessFrame(byte[] frame, uint asce_cmd)
	{
		if (frame[0] == 126 && frame[1] == 22)
		{
			_lastUpgradeDataTime = DateTime.Now;
			ResUpgradeInfo?.Invoke(frame, "GMUpg", null);
		}
		else if (frame[0] != 234 || frame[1] != 85)
		{
			if (frame[0] == 171 && frame[1] == byte.MaxValue)
			{
				ResFrameInfo?.Invoke(frame, null, null);
			}
			else if (frame[0] == 79 && frame[1] == 84 && frame[2] == 82 && frame[3] == 65)
			{
				ResUpgradeInfo_Asce?.Invoke(frame, "AscentUpg", asce_cmd);
			}
		}
	}

	private bool ValidateFrame(byte[] frame, int length)
	{
		byte b = frame[0];
		byte b2 = frame[1];
		if (b == 126 && b2 == 22 && length == 38)
		{
			ushort num = (ushort)(frame[36] | (frame[37] << 8));
			Buffer.BlockCopy(frame, 0, _crcBuf, 0, 36);
			ushort num2 = CRC16_Gimbal.Calculate(_crcBuf, 36);
			return num == num2;
		}
		return true;
	}

	public bool HandlePacket(byte[] recBuffer, RecvPacket_Gim packet)
	{
		bool result = false;
		if (recBuffer.Length == 118 && recBuffer[0] == 171 && recBuffer[1] == byte.MaxValue)
		{
			result = ParseCommData(recBuffer, new RecvPacket_Gim());
		}
		else if (recBuffer.Length == 38 && recBuffer[0] == 126 && recBuffer[1] == 22)
		{
			_lastUpgradeDataTime = DateTime.Now;
			ushort num = (ushort)(recBuffer[36] | (recBuffer[37] << 8));
			byte[] array = new byte[36];
			Array.Copy(recBuffer, 0, array, 0, 36);
			ushort num2 = CRC16_Gimbal.Calculate(array);
			if (num == num2)
			{
				ResUpgradeInfo?.Invoke(recBuffer, "upgrade", null);
				result = true;
			}
			else
			{
				string text = StaticMethod.BytesToHexString(recBuffer);
				result = false;
				WriteLog.WriteLogFileToUI("云台升级帧解释失败," + text, Color.DarkRed);
			}
		}
		return result;
	}

	private bool ParseCommData(byte[] recBuffer, RecvPacket_Gim packet)
	{
		bool result = false;
		try
		{
			if (recBuffer[0] != 171 || recBuffer[1] != byte.MaxValue)
			{
				return false;
			}
			packet = new RecvPacket_Gim();
			byte b = recBuffer[4];
			if (b != 110)
			{
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
			byte b2 = 0;
			byte b3 = 0;
			for (int j = 0; j < 116; j++)
			{
				b2 += recBuffer[j];
				b3 += b2;
			}
			if (b2 != packet.sumcheck || b3 != packet.addcheck)
			{
				return false;
			}
			ResFrameInfo?.Invoke(recBuffer, null, null);
			return true;
		}
		catch (Exception)
		{
			return result;
		}
	}

	public int BuildSendPacket(GIM_CMD cmd, float roll = 0f, float picth = 0f, float yaw = 0f)
	{
		List<byte> list = new List<byte>(12);
		try
		{
			if (_serial == null || !_serial.IsOpen)
			{
				return -1;
			}
			lock (_writeLock)
			{
				list.Add(126);
				list.Add(22);
				list.Add((byte)(cmd & (GIM_CMD)0xFF));
				short num = ClampToInt16(roll * 100f);
				short num2 = ClampToInt16(picth * 100f);
				short num3 = ClampToInt16(yaw * 100f);
				list.Add((byte)((num >> 8) & 0xFF));
				list.Add((byte)(num & 0xFF));
				list.Add((byte)((num2 >> 8) & 0xFF));
				list.Add((byte)(num2 & 0xFF));
				list.Add((byte)((num3 >> 8) & 0xFF));
				list.Add((byte)(num3 & 0xFF));
				list.Add(90);
				list.Add(165);
				SPEventArgs e = new SPEventArgs();
				e.HeaderBuffer = list.ToArray();
				_sendQue.Enqueue(e);
			}
			return 0;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("构建发送帧失败,desc=" + ex.Message, Color.Red);
			return -1;
		}
	}

	public int BuildStartUpgradePacket(byte[] fileData)
	{
		try
		{
			return 0;
		}
		catch (Exception)
		{
			return -9;
		}
	}

	public bool SendMsg(byte[] data)
	{
		try
		{
			if (_serial != null && _serial.IsOpen)
			{
				SPEventArgs e = new SPEventArgs();
				e.HeaderBuffer = new byte[data.Length];
				Array.Copy(data, e.HeaderBuffer, data.Length);
				_sendQue.Enqueue(e);
				return true;
			}
			WriteLog.WriteLogFileToUI("串口未打开！", Color.Red);
			return false;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("串口发送失败，desc=" + ex.Message, Color.Red);
			return false;
		}
	}

	public bool SendMsg(byte[] data, int delayTime)
	{
		try
		{
			if (_serial != null && _serial.IsOpen)
			{
				SPEventArgs e = new SPEventArgs();
				e.HeaderBuffer = new byte[data.Length];
				Array.Copy(data, e.HeaderBuffer, data.Length);
				e.DelayTime = delayTime;
				_sendQue.Enqueue(e);
				return true;
			}
			WriteLog.WriteLogFileToUI("串口未打开！", Color.Red);
			return false;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("串口发送失败，desc=" + ex.Message, Color.Red);
			return false;
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

	public void Dispose()
	{
		StopSendThread();
		StopRecvThread();
		StopParseThread();
		StopSerialMonitor();
		_sendQue.ClearAndClose();
		if (_serial != null)
		{
			try
			{
				SerialPort serial = _serial;
				if (serial != null)
				{
					serial.Close();
				}
			}
			catch (Exception ex)
			{
				WriteLog.WriteLogFileToUI("串口关闭异常,desc=" + ex.Message, Color.Red);
			}
			_serial = null;
		}
		_recvBuffer.Clear();
	}
}
