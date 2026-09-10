using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Caddx_PCTool;

public class UsbSerialportFSM : IDisposable
{
	private class PendingAckContext
	{
		public uint ExpectedCmd;

		public uint ExpectedSeq;

		public bool RequireMatchingSeq;

		public DateTime FirstSentAt;

		public DateTime LastSentAt;

		public uint RetryCount;

		public bool RetrySendInFlight;

		public uint Cmd;

		public byte[] Payload;

		public uint Length;

		public string TimeoutMsg;

		public Action<uint, object, object> OnMatchedAck;

		public Action<string> OnTimeout;

		public Action<string> OnSendFail;

		public Action<uint, uint> OnRetry;

		public Action<uint, uint, string> OnTimeoutEx;

		public Action<uint, uint, string> OnSendFailEx;

		public int RetryIntervalMs;

		public int TotalTimeoutMs;

		public int SendTimeoutMs;
	}

	private CancellationTokenSource _monitorCts;

	private Task _monitorTask;

	private const int SerialCheckInterval = 50;

	private const int SerialStableThreshold = 150;

	private bool _lastStableState = false;

	private BlockQueue<SPEventArgs> _sendQue;

	private CancellationTokenSource _sendCts;

	private string _portName;

	private SerialPort _serial;

	private readonly object _closeLock = new object();

	private bool _isClosed = true;

	private const int SerialPortCloseTimeoutMs = 1000;

	private const int SerialPortIoTimeoutMs = 25000;

	private const int BackgroundTaskStopTimeoutMs = 1500;

	private const int MaxReceiveFrameLength = 5120;

	private CancellationTokenSource _recvCts;

	private Task _sendTask;

	private Task _recvTask;

	private readonly object _writeLock = new object();

	private readonly RingBuffer _recvBuffer = new RingBuffer(262144);

	private readonly byte[] _headerBuf = new byte[36];

	private ConcurrentDictionary<uint, bool> _ackStatus = new ConcurrentDictionary<uint, bool>();

	private UsbDevInfo _devUsbInfo;

	private uint _seq = 0u;

	public int SegLength = 1024;

	public Action<uint, object, object> OnAckReceived;

	public Action<uint, uint, object, object> OnAckReceivedEx;

	private const int DefaultAckRetryIntervalMs = 1500;

	private const int DefaultAckTotalTimeoutMs = 10000;

	private readonly object _ackGuardLock = new object();

	private CancellationTokenSource _ackGuardCts;

	private CancellationTokenSource _ackDelayCts;

	private long _ackGuardVersion;

	private PendingAckContext _pendingAck;

	private readonly uint[] _crcTable = new uint[256];

	private bool _isfirst = false;

	private int retryTime = 2;

	private bool _isAutoSendBBFreqConfig = false;

	public uint LastReceivedSeq { get; private set; }

	public bool IsComOpened
	{
		get
		{
			if (_serial != null)
			{
				return _serial.IsOpen;
			}
			return false;
		}
	}

	public SerialPort SPobj => _serial;

	public bool IsRunAutoUpgrade { get; set; }

	public bool IsFirst => _isfirst;

	public UsbDevInfo UsbInfo => _devUsbInfo;

	public event Action<int> DeviceInfoReceived;

	public event Action<int> SigReceiveDateInfo;

	public event Action<string> LogInfo;

	public event Action<bool, string, int> SerialConnectedStateChange;

	public UsbSerialportFSM()
	{
		InitCrcTable();
		_monitorCts = new CancellationTokenSource();
		_sendCts = new CancellationTokenSource();
		_recvCts = new CancellationTokenSource();
		_sendQue = new BlockQueue<SPEventArgs>(200);
	}

	public UsbSerialportFSM(UsbDevInfo usbInfo, int reTime = 2)
	{
		InitCrcTable();
		_monitorCts = new CancellationTokenSource();
		_sendCts = new CancellationTokenSource();
		_recvCts = new CancellationTokenSource();
		_sendQue = new BlockQueue<SPEventArgs>(200);
		_devUsbInfo = usbInfo;
		retryTime = reTime;
	}

	public bool Open(string portName, int baudRate = 115200)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		try
		{
			_serial = new SerialPort(portName, baudRate, (Parity)0, 8, (StopBits)1);
			_serial.WriteBufferSize = 1075200;
			_serial.ReadBufferSize = 1075200;
			_serial.ReadTimeout = 25000;
			_serial.WriteTimeout = 25000;
			_serial.Open();
			if (!_serial.IsOpen)
			{
				((Component)(object)_serial).Dispose();
				_serial = null;
				return false;
			}
			lock (_closeLock)
			{
				_isClosed = false;
			}
			_portName = portName;
			_recvBuffer.ClearAllBuffer();
			StartSendThread();
			StartRecvThread();
			StartSerialMonitor();
			_isfirst = true;
			return true;
		}
		catch (Exception ex)
		{
			try
			{
				((Component)(object)_serial)?.Dispose();
			}
			catch
			{
			}
			_serial = null;
			Log("OpenSerial Failed: " + ex.Message);
			return false;
		}
	}

	public void Close()
	{
		SerialPort serial;
		lock (_closeLock)
		{
			if (_isClosed)
			{
				return;
			}
			_isClosed = true;
			serial = _serial;
		}
		CancelAckGuard();
		StopSerialMonitor();
		StopSendThread();
		StopRecvThread();
		lock (_closeLock)
		{
			if (_serial == serial)
			{
				_serial = null;
			}
		}
		CloseSerialPortWithTimeout(serial);
		_recvBuffer.ClearAllBuffer();
	}

	private void CloseSerialPortWithTimeout(SerialPort serial)
	{
		if (serial == null)
		{
			return;
		}
		Task task = Task.Run(delegate
		{
			try
			{
				if (serial.IsOpen)
				{
					serial.Close();
				}
				((Component)(object)serial).Dispose();
			}
			catch (Exception ex)
			{
				WriteLog.WriteLogFileToUI("关闭串口失败！desc=" + ex.Message, Color.Red);
			}
		});
		if (!task.Wait(1000))
		{
			WriteLog.WriteLogFileToUI("SerialPort.Close超时，串口名=" + serial.PortName, Color.DarkOrange);
		}
	}

	public bool Reopen()
	{
		try
		{
			Close();
			Thread.Sleep(200);
			return Open(_portName);
		}
		catch (Exception)
		{
			return false;
		}
	}

	private string GetDevicePortname()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		if (_devUsbInfo == null || string.IsNullOrWhiteSpace(_devUsbInfo.VID) || string.IsNullOrWhiteSpace(_devUsbInfo.PID))
		{
			return "";
		}
		string text = _devUsbInfo.VID.Trim().ToUpperInvariant();
		string text2 = _devUsbInfo.PID.Trim().ToUpperInvariant();
		string[] portNames = SerialPort.GetPortNames();
		string[] array = portNames;
		foreach (string text3 in array)
		{
			string text4 = "SELECT * FROM Win32_PnPEntity WHERE Name LIKE  '%" + text3 + "%'";
			ManagementObjectSearcher val = new ManagementObjectSearcher(text4);
			try
			{
				ManagementObjectCollection val2 = val.Get();
				ManagementObjectEnumerator enumerator = val2.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementObject device = (ManagementObject)enumerator.Current;
						UsbDevInfo usbDevInfo = ParseDeviceInformation((ManagementBaseObject)(object)device);
						if (usbDevInfo != null && !string.IsNullOrWhiteSpace(usbDevInfo.VID) && !string.IsNullOrWhiteSpace(usbDevInfo.PID))
						{
							string text5 = usbDevInfo.VID.Trim().ToUpperInvariant();
							string text6 = usbDevInfo.PID.Trim().ToUpperInvariant();
							if (text5 == text && text6 == text2)
							{
								return usbDevInfo.PortName;
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		return "";
	}

	private int GetCurrentReOpenDelayMs(out string matchSource)
	{
		int val = GD.Inst.ResolveReOpenDelay_Asce(_devUsbInfo, out matchSource);
		val = Math.Max(0, val);
		string text = _devUsbInfo?.DevName ?? "";
		string text2 = _devUsbInfo?.VID ?? "";
		string text3 = _devUsbInfo?.PID ?? "";
		string text4 = _devUsbInfo?.PortName ?? _portName ?? "";
		WriteLog.WriteLogFileToUI(string.Format("重连延时策略：DevName={0}, VID={1}, PID={2}, Port={3}, Delay={4}ms, Match={5}", new object[6] { text, text2, text3, text4, val, matchSource }), Color.DarkCyan);
		return val;
	}

	public bool Reopen(int timeout)
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Restart();
		try
		{
			Thread.Sleep(500);
			Close();
			int currentReOpenDelayMs = GetCurrentReOpenDelayMs(out var _);
			Thread.Sleep(currentReOpenDelayMs);
			DateTime now = DateTime.Now;
			int num = 0;
			while (true)
			{
				bool flag;
				if (GD.Inst.ForceSearchMode == 1)
				{
					string text = GetDevicePortname();
					if (string.IsNullOrEmpty(text))
					{
						text = _portName;
					}
					if (text != _portName)
					{
						WriteLog.WriteLogFileToUI("串口已变化，当前串口=" + text + ",旧串口=" + _portName, Color.DarkOrange);
					}
					flag = Open(text);
				}
				else if (GD.Inst.ForceSearchMode == 3)
				{
					if (num < GD.Inst.PortRetryTimeLimit)
					{
						if (Open(_portName))
						{
							stopwatch.Stop();
							WriteLog.WriteLogFileToUI("重连成功,重连耗时=" + stopwatch.ElapsedMilliseconds, Color.DarkGreen);
							return true;
						}
						num++;
						WriteLog.WriteLogFileToUI($"旧串口重连失败，第{num}次尝试，串口={_portName}", Color.DarkOrange);
						if ((DateTime.Now - now).TotalMilliseconds > (double)timeout)
						{
							WriteLog.WriteLogFileToUI("重连超时!", Color.Red);
							return false;
						}
						if (num >= GD.Inst.PortRetryTimeLimit)
						{
							WriteLog.WriteLogFileToUI($"旧串口连续重连{GD.Inst.PortRetryTimeLimit}次失败，开始搜索USB设备", Color.DarkOrange);
						}
						else
						{
							Thread.Sleep(1000);
						}
						continue;
					}
					string devicePortname = GetDevicePortname();
					if (!string.IsNullOrEmpty(devicePortname))
					{
						if (devicePortname != _portName)
						{
							WriteLog.WriteLogFileToUI("串口已变化，当前串口=" + devicePortname + ",旧串口=" + _portName, Color.DarkOrange);
						}
						if (Open(devicePortname))
						{
							if (_devUsbInfo != null)
							{
								_devUsbInfo.PortName = devicePortname;
							}
							stopwatch.Stop();
							WriteLog.WriteLogFileToUI("重连成功,重连耗时=" + stopwatch.ElapsedMilliseconds, Color.DarkGreen);
							return true;
						}
					}
					flag = false;
				}
				else
				{
					flag = Open(_portName);
				}
				if (flag)
				{
					stopwatch.Stop();
					WriteLog.WriteLogFileToUI("重连成功,重连耗时=" + stopwatch.ElapsedMilliseconds, Color.DarkGreen);
					return true;
				}
				if ((DateTime.Now - now).TotalMilliseconds > (double)timeout)
				{
					break;
				}
				Thread.Sleep(GD.Inst.ReElectDelay);
			}
			WriteLog.WriteLogFileToUI("重连超时!", Color.Red);
			return false;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("UsbSerialportFSM.Reopen err desc=" + ex.Message, Color.Red);
			return false;
		}
	}

	public bool ReopenClean_Json(int timeout)
	{
		bool result = false;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Restart();
		try
		{
			byte[] array = Encoding.ASCII.GetBytes("clean");
			Array.Resize(ref array, 32);
			int num = CommandSerialSend(3u, 1, 2, 0, array, (uint)array.Length, 0u, 3000);
			return result;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("UsbSerialportFSM.Reopen err desc=" + ex.Message, Color.Red);
			return false;
		}
	}

	public bool Reopen_Json(int timeout)
	{
		bool flag = false;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Restart();
		try
		{
			byte[] array = new byte[32];
			if (CommandSerialSend(3u, 1, 2, 0, array, (uint)array.Length, 0u, 3000) != 0)
			{
				WriteLog.WriteLogFileToUI("发送 REBOOT 失败", Color.DarkRed);
			}
			Thread.Sleep(500);
			Close();
			int currentReOpenDelayMs = GetCurrentReOpenDelayMs(out var _);
			Thread.Sleep(currentReOpenDelayMs);
			DateTime now = DateTime.Now;
			while (true)
			{
				if (Open(_portName))
				{
					stopwatch.Stop();
					WriteLog.WriteLogFileToUI("重连成功,重连耗时=" + stopwatch.ElapsedMilliseconds, Color.DarkGreen);
					return true;
				}
				if ((DateTime.Now - now).TotalMilliseconds > (double)timeout)
				{
					break;
				}
				Thread.Sleep(GD.Inst.ReElectDelay);
			}
			WriteLog.WriteLogFileToUI("重连超时!", Color.Red);
			return false;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("UsbSerialportFSM.Reopen err desc=" + ex.Message, Color.Red);
			return false;
		}
	}

	private void StartSendThread()
	{
		_sendQue = new BlockQueue<SPEventArgs>(200);
		_sendCts = new CancellationTokenSource();
		_sendTask = Task.Run(() => SendQueueLoop(_sendCts.Token));
	}

	private void StopSendThread()
	{
		_sendCts?.Cancel();
		_sendQue?.ClearAndClose();
		WaitForBackgroundTask(_sendTask, "_sendTask");
		_sendTask = null;
		_sendCts?.Dispose();
		_sendCts = null;
		_sendQue = null;
	}

	private async Task SendQueueLoop(CancellationToken cts)
	{
		while (!cts.IsCancellationRequested)
		{
			try
			{
				if (_sendQue.TryDequeue(out var spe))
				{
					if (spe.DelayTime > 0)
					{
						await Task.Delay(spe.DelayTime, cts);
					}
					SerialPort serial = _serial;
					if (!_isClosed && serial != null && serial.IsOpen)
					{
						try
						{
							spe.OnFrameSending?.Invoke();
							serial.Write(spe.HeaderBuffer, 0, spe.HeaderBuffer.Length);
							if (spe.DataBuffer != null && spe.DataBuffer.Length != 0)
							{
								serial.Write(spe.DataBuffer, 0, spe.DataBuffer.Length);
							}
							if (spe.Cmd != 116)
							{
								WriteLog.WriteLogFileToUI("请求帧发送完成，cmd=" + spe.Cmd.ToString("x2"), Color.Black);
							}
							spe.OnFrameSent?.Invoke();
						}
						catch (Exception ex)
						{
							Exception ex2 = ex;
							spe.OnFrameSendFailed?.Invoke(ex2);
							throw;
						}
					}
					else
					{
						InvalidOperationException ex3 = new InvalidOperationException("串口未打开！");
						WriteLog.WriteLogFileToUI(ex3.Message, Color.Red);
						spe.OnFrameSendFailed?.Invoke(ex3);
					}
				}
				spe = null;
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (InvalidOperationException) when (cts.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex6)
			{
				WriteLog.WriteLogFileToUI("SPSendThreadMethod error happen，Desc=" + ex6.Message, Color.Red);
				await Task.Delay(10);
			}
		}
	}

	private void SerialportSendMethod()
	{
		try
		{
			int num = 10;
			if (!_sendQue.TryDequeue(out var value))
			{
				return;
			}
			if (_serial != null && _serial.IsOpen)
			{
				_serial.Write(value.HeaderBuffer, 0, value.HeaderBuffer.Length);
				if (value.DataBuffer != null && value.DataBuffer.Length != 0)
				{
					_serial.Write(value.DataBuffer, 0, value.DataBuffer.Length);
				}
			}
			else
			{
				WriteLog.WriteLogFileToUI("串口未打开！", Color.Red);
			}
			num = ((value.DelayTime < 10) ? 10 : value.DelayTime);
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("SerialportSendMethod error, desc=" + ex.Message, Color.Red);
		}
	}

	private void StartRecvThread()
	{
		_recvCts = new CancellationTokenSource();
		_recvTask = Task.Run(delegate
		{
			RecvLoop(_recvCts.Token);
		});
	}

	private void StopRecvThread()
	{
		_recvCts?.Cancel();
		WaitForBackgroundTask(_recvTask, "_recvTask");
		_recvTask = null;
		_recvCts?.Dispose();
		_recvCts = null;
	}

	private void RecvLoop(CancellationToken cts)
	{
		int num = 36;
		while (!cts.IsCancellationRequested)
		{
			try
			{
				SerialPort serial = _serial;
				if (!_isClosed && serial != null && serial.IsOpen)
				{
					int bytesToRead = serial.BytesToRead;
					if (bytesToRead > 0)
					{
						byte[] array = new byte[bytesToRead];
						int num2 = serial.Read(array, 0, bytesToRead);
						if (num2 > 0)
						{
							_recvBuffer.Write(array, 0, num2);
							while (true)
							{
								int num3 = _recvBuffer.PeekHeader(_headerBuf, num);
								if (num3 < 0)
								{
									break;
								}
								ArProtocolHeader header = BytesToStruct<ArProtocolHeader>(_headerBuf);
								if (header.magic != 1095914575)
								{
									_recvBuffer.Skip(1);
									continue;
								}
								if (header.length > 5120 - num)
								{
									WriteLog.WriteLogFileToUI($"异常帧长度，跳过1字节重新同步，length={header.length},cmd={header.command}", Color.Red);
									_recvBuffer.Skip(1);
									continue;
								}
								int num4 = num + (int)header.length;
								if (num4 > 5120)
								{
									WriteLog.WriteLogFileToUI($"异常帧长度，跳过1字节重新同步，fullLen={num4},cmd={header.command}", Color.Red);
									_recvBuffer.Skip(1);
									continue;
								}
								if (header.command != 116)
								{
									WriteLog.WriteLogFileToUI(string.Format("返回帧长度={0},cmd={1}", _recvBuffer.Available, header.command.ToString("x2")), Color.IndianRed);
								}
								if (num3 < num4)
								{
									if (header.command == 60 && retryTime > 0)
									{
										WriteLog.WriteLogFileToUI($"重新发送设备信息帧，第{retryTime}次", Color.IndianRed);
									}
									break;
								}
								byte[] array2 = null;
								if (header.length != 0)
								{
									array2 = new byte[header.length];
									_recvBuffer.PeekAt(num, array2, array2.Length);
								}
								_recvBuffer.Skip(num4);
								if (header.command == 119 && !CliSendProtocol.TryValidateAckFrame(_headerBuf, array2, out var error))
								{
									WriteLog.WriteLogFileToUI($"AR_COMMAND_SEND_TEXT ACK 无效，原因={error}", Color.Red);
								}
								else
								{
									HandlePacket(header, array2, header.length);
								}
							}
						}
					}
				}
				if (cts.WaitHandle.WaitOne(5))
				{
					break;
				}
			}
			catch (Exception ex)
			{
				if (cts.IsCancellationRequested || _isClosed)
				{
					break;
				}
				Log("RecvLoop exception: " + ex.Message);
				if (cts.WaitHandle.WaitOne(10))
				{
					break;
				}
			}
		}
	}

	private void CancelAckGuard()
	{
		_ackGuardCts?.Cancel();
		_ackDelayCts?.Cancel();
		lock (_ackGuardLock)
		{
			_ackGuardVersion++;
			_pendingAck = null;
			_ackDelayCts = null;
		}
	}

	private bool TryCancelAckGuard(long guardVersion)
	{
		CancellationTokenSource ackGuardCts;
		lock (_ackGuardLock)
		{
			if (guardVersion != _ackGuardVersion)
			{
				return false;
			}
			_ackGuardVersion++;
			_pendingAck = null;
			ackGuardCts = _ackGuardCts;
			_ackGuardCts = null;
		}
		ackGuardCts?.Cancel();
		return true;
	}

	private bool TryConsumeMatchedAck(uint cmd, uint seq, object data, object data2)
	{
		PendingAckContext pendingAckContext = null;
		lock (_ackGuardLock)
		{
			if (_pendingAck == null)
			{
				return false;
			}
			if (_pendingAck.ExpectedCmd != cmd)
			{
				return false;
			}
			if (_pendingAck.ExpectedSeq != seq)
			{
				Log($"ACK seq mismatch, cmd=0x{cmd:X}, expectedSeq={_pendingAck.ExpectedSeq}, actualSeq={seq}");
				if (_pendingAck.RequireMatchingSeq)
				{
					return false;
				}
			}
			pendingAckContext = _pendingAck;
			_pendingAck = null;
			_ackGuardCts?.Cancel();
		}
		try
		{
			pendingAckContext?.OnMatchedAck?.Invoke(cmd, data, data2);
		}
		catch (Exception ex)
		{
			Log("OnMatchedAck 回调异常: " + ex.Message);
		}
		return true;
	}

	private async Task AckGuardLoop(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			PendingAckContext snapshot;
			lock (_ackGuardLock)
			{
				snapshot = _pendingAck;
			}
			if (snapshot == null)
			{
				break;
			}
			DateTime now = DateTime.Now;
			if ((now - snapshot.FirstSentAt).TotalMilliseconds >= (double)snapshot.TotalTimeoutMs)
			{
				lock (_ackGuardLock)
				{
					_pendingAck = null;
				}
				snapshot.OnTimeout?.Invoke(snapshot.TimeoutMsg);
				snapshot.OnTimeoutEx?.Invoke(snapshot.Cmd, snapshot.RetryCount, snapshot.TimeoutMsg);
				break;
			}
			if ((now - snapshot.LastSentAt).TotalMilliseconds >= (double)snapshot.RetryIntervalMs)
			{
				uint nextRetry = 0u;
				bool shouldRetry = false;
				lock (_ackGuardLock)
				{
					if (_pendingAck != snapshot)
					{
						break;
					}
					if (!_pendingAck.RetrySendInFlight)
					{
						nextRetry = _pendingAck.RetryCount + 1;
						_pendingAck.RetrySendInFlight = true;
						shouldRetry = true;
					}
				}
				if (shouldRetry && SendPacketForAckGuard(snapshot.Cmd, snapshot.Payload, snapshot.Length, nextRetry, snapshot.SendTimeoutMs, out var _, snapshot.ExpectedSeq, delegate
				{
					bool flag = false;
					lock (_ackGuardLock)
					{
						if (_pendingAck != snapshot)
						{
							return;
						}
						_pendingAck.RetryCount = nextRetry;
						_pendingAck.LastSentAt = DateTime.Now;
						_pendingAck.RetrySendInFlight = false;
						flag = true;
					}
					if (flag)
					{
						snapshot.OnRetry?.Invoke(snapshot.Cmd, nextRetry);
					}
				}, delegate(Exception ex)
				{
					bool flag = false;
					lock (_ackGuardLock)
					{
						if (_pendingAck != snapshot)
						{
							return;
						}
						_pendingAck = null;
						flag = true;
					}
					if (flag)
					{
						string text = snapshot.TimeoutMsg + ", resend failed: " + ex.Message;
						snapshot.OnSendFail?.Invoke(text);
						snapshot.OnSendFailEx?.Invoke(snapshot.Cmd, nextRetry, text);
					}
				}) != 0)
				{
					lock (_ackGuardLock)
					{
						if (_pendingAck == snapshot)
						{
							_pendingAck = null;
						}
					}
					string failMsg = snapshot.TimeoutMsg + ", resend failed";
					snapshot.OnSendFail?.Invoke(failMsg);
					snapshot.OnSendFailEx?.Invoke(snapshot.Cmd, nextRetry, failMsg);
					break;
				}
			}
			now = DateTime.Now;
			double elapsedSinceLastSend = (now - snapshot.LastSentAt).TotalMilliseconds;
			double elapsedSinceFirstSend = (now - snapshot.FirstSentAt).TotalMilliseconds;
			int untilRetryMs = (snapshot.RetrySendInFlight ? 20 : Math.Max(0, snapshot.RetryIntervalMs - (int)elapsedSinceLastSend));
			int untilTimeoutMs = Math.Max(0, snapshot.TotalTimeoutMs - (int)elapsedSinceFirstSend);
			int delayMs = Math.Max(1, Math.Min(Math.Min(untilRetryMs, untilTimeoutMs), 20));
			await Task.Delay(delayMs, token);
		}
	}

	private bool StartAckGuard(PendingAckContext pending, long guardVersion)
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		CancellationTokenSource ackGuardCts;
		lock (_ackGuardLock)
		{
			if (guardVersion != _ackGuardVersion)
			{
				cancellationTokenSource.Dispose();
				return false;
			}
			ackGuardCts = _ackGuardCts;
			_pendingAck = pending;
			_ackGuardCts = cancellationTokenSource;
		}
		ackGuardCts?.Cancel();
		CancellationToken token = cancellationTokenSource.Token;
		Task.Run(async delegate
		{
			try
			{
				await AckGuardLoop(token);
			}
			catch (TaskCanceledException)
			{
			}
			catch (Exception ex2)
			{
				Exception ex3 = ex2;
				Log("AckGuardLoop 异常: " + ex3.Message);
			}
		}, token);
		return true;
	}

	private void RaiseAck(uint cmd, uint seq, object data, object data2)
	{
		LastReceivedSeq = seq;
		TryConsumeMatchedAck(cmd, seq, data, data2);
		OnAckReceivedEx?.Invoke(cmd, seq, data, data2);
		OnAckReceived?.Invoke(cmd, data, data2);
	}

	private void HandlePacket(ArProtocolHeader header, byte[] payload, uint payloadLen)
	{
		uint command = header.command;
		uint seq = header.seq;
		string text = "";
		try
		{
			switch ((AR_COMMAND)command)
			{
			case AR_COMMAND.unkown_cmd:
				RaiseAck(command, seq, "UNKOWNCMD", payload);
				break;
			case AR_COMMAND.AR_COMMAND_FIND_DEVICE:
			{
				ResDeviceInfo resDeviceInfo = ParseDeviceInfo(payload);
				ResAscentInfo resAscentInfo = new ResAscentInfo();
				resAscentInfo.SN = Encoding.ASCII.GetString(resDeviceInfo.serialNumber).TrimEnd(new char[1]);
				resAscentInfo.FWVers = Encoding.ASCII.GetString(resDeviceInfo.firmwareInfo).TrimEnd(new char[1]);
				resAscentInfo.HWVers = Encoding.ASCII.GetString(resDeviceInfo.hardwareVersion).TrimEnd(new char[1]);
				resAscentInfo.MCUTemp = resDeviceInfo.cputemp;
				resAscentInfo.UsbInfo = _devUsbInfo;
				resAscentInfo.SDKVers = Encoding.ASCII.GetString(resDeviceInfo.sdkversion).TrimEnd(new char[1]);
				resAscentInfo.DevName = Encoding.ASCII.GetString(resDeviceInfo.devicename).TrimEnd(new char[1]);
				resAscentInfo.RecMaxSize = resDeviceInfo.receiveMaxSize;
				resAscentInfo.Details = Encoding.ASCII.GetString(resDeviceInfo.detail).TrimEnd(new char[1]);
				ResAscentInfo data3 = resAscentInfo;
				RaiseAck(60u, seq, data3, _devUsbInfo);
				break;
			}
			case AR_COMMAND.AR_COMMAND_REBOOT:
				RaiseAck(3u, seq, null, null);
				break;
			case AR_COMMAND.AR_COMMAND_REMOTE_UPGRADE:
				RaiseAck(114u, seq, null, null);
				break;
			case AR_COMMAND.AR_CMD_SENDFILE_START:
				RaiseAck(115u, seq, payload, payloadLen);
				break;
			case AR_COMMAND.AR_CMD_SENDFILE_DATA:
				if (_isAutoSendBBFreqConfig)
				{
					string text2 = ParseBBFreqConfig(payload);
					RaiseAck(116u, seq, text2, text2);
				}
				else
				{
					ResFileDataInfo resFileDataInfo2 = ParseDataInfo(payload);
					text = Encoding.ASCII.GetString(resFileDataInfo2.Detail).TrimEnd(new char[1]);
					RaiseAck(116u, seq, resFileDataInfo2, text);
				}
				break;
			case AR_COMMAND.AR_CMD_SENDFILE_END:
				if (_isAutoSendBBFreqConfig)
				{
					RaiseAck(117u, seq, null, "SendBBFreqEnd");
					_isAutoSendBBFreqConfig = false;
				}
				else
				{
					ResFileDataInfo resFileDataInfo = ParseDataInfo(payload);
					text = Encoding.ASCII.GetString(resFileDataInfo.Detail).TrimEnd(new char[1]);
					RaiseAck(117u, seq, resFileDataInfo, null);
				}
				break;
			case AR_COMMAND.AR_CMD_UPGRADE_STATUS:
			{
				ResUpgradeStatus data2 = ParseUpgradeStatus(payload);
				RaiseAck(118u, seq, data2, null);
				break;
			}
			case AR_COMMAND.AR_COMMAND_SEND_TEXT:
			{
				if (!CliSendProtocol.TryParseAck(payload, out var status, out var detail))
				{
					Log("AR_COMMAND_SEND_TEXT ACK 长度不足");
					break;
				}
				ResAckInfo resAckInfo3 = new ResAckInfo
				{
					Status = status
				};
				Buffer.BlockCopy(payload, 4, resAckInfo3.Detail, 0, 64);
				RaiseAck(119u, seq, resAckInfo3, detail);
				break;
			}
			case AR_COMMAND.AR_COMMAND_GET_BB_FREQ_CONFIG:
			{
				string data = ParseBBFreqConfig(payload);
				RaiseAck(120u, seq, data, null);
				break;
			}
			case AR_COMMAND.AR_COMMAND_SET_BB_FREQ_CONFIG:
			{
				ResAckInfo resAckInfo2 = ParseAckInfo(payload);
				text = Encoding.ASCII.GetString(resAckInfo2.Detail).TrimEnd(new char[1]);
				RaiseAck(121u, seq, resAckInfo2, text);
				break;
			}
			case AR_COMMAND.AR_COMMAND_FACTRESET_BB_FREQ_CONFIG:
			{
				ResAckInfo resAckInfo = ParseAckInfo(payload);
				text = Encoding.ASCII.GetString(resAckInfo.Detail).TrimEnd(new char[1]);
				RaiseAck(122u, seq, resAckInfo, text);
				break;
			}
			default:
				RaiseAck(command, seq, "UNKOWNCMD", null);
				break;
			}
		}
		catch (Exception ex)
		{
			Log("HandlePacket 异常: " + ex.Message);
		}
	}

	public int CommandSerialSend(uint cmd, ushort type, ushort format, ushort userId, byte[] payload, uint len, uint retryNumber, int timeoutMs)
	{
		uint sentSeq;
		return EnqueueSend(cmd, type, format, userId, payload, len, retryNumber, out sentSeq);
	}

	public int CommandSerialSend(uint cmd, ushort type, ushort format, ushort userId, byte[] payload, uint len, uint retryNumber, int timeoutMs, out uint sentSeq)
	{
		return EnqueueSend(cmd, type, format, userId, payload, len, retryNumber, out sentSeq);
	}

	public int CommandSerialSendDelay(uint cmd, byte[] payload, uint len, int delayTim, out uint sentSeq)
	{
		return EnqueueSendDelay(cmd, payload, len, delayTim, out sentSeq);
	}

	public int CommandSerialSend(uint cmd, ushort type, ushort format, ushort userId, byte[] payload, uint len, uint retryNumber, int timeoutMs, out uint sentSeq, uint? fixedSeq, Action<uint> afterFrameSent = null, Action<Exception> onFrameSendFail = null)
	{
		return EnqueueSend(cmd, type, format, userId, payload, len, retryNumber, out sentSeq, fixedSeq, afterFrameSent, onFrameSendFail);
	}

	public int CommandSerialSendImmediate(uint cmd, ushort type, ushort format, ushort userId, byte[] payload, uint len, uint retryNumber, int timeoutMs, out uint sentSeq)
	{
		sentSeq = 0u;
		SerialPort serial = _serial;
		if (_isClosed || serial == null || !serial.IsOpen)
		{
			Log("Serial not open");
			return -1;
		}
		uint seq = _seq;
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
			seq = seq,
			retry = retryNumber,
			crc32 = 0u
		};
		byte[] headerBytes = StructToBytes(obj);
		obj.crc32 = CrcCalc(headerBytes, payload, (int)len);
		byte[] array = StructToBytes(obj);
		try
		{
			lock (_writeLock)
			{
				if (_isClosed || serial == null || !serial.IsOpen)
				{
					Log("Serial not open");
					return -1;
				}
				serial.Write(array, 0, array.Length);
				if (payload != null && len != 0)
				{
					serial.Write(payload, 0, (int)len);
				}
			}
		}
		catch (Exception ex)
		{
			Log("Write failed: " + ex.Message);
			return -1;
		}
		sentSeq = obj.seq;
		_seq++;
		if (cmd != 116)
		{
			WriteLog.WriteLogFileToUI("请求帧同步发送完成，cmd=" + cmd.ToString("x2"), Color.Black);
		}
		return 0;
	}

	public int CommandSerialSend(uint cmd, ushort type, ushort format, ushort userId, byte[] payload, uint len)
	{
		uint sentSeq;
		return EnqueueSend(cmd, type, format, userId, payload, len, 0u, out sentSeq);
	}

	protected virtual int SendPacketForAckGuard(uint cmd, byte[] payload, uint len, uint retryNumber, int timeoutMs, out uint sentSeq, uint? fixedSeq = null, Action<uint> afterFrameSent = null, Action<Exception> onFrameSendFail = null)
	{
		return CommandSerialSend(cmd, 1, 2, 0, payload, len, retryNumber, timeoutMs, out sentSeq, fixedSeq, afterFrameSent, onFrameSendFail);
	}

	public bool SendWithAckGuard(uint cmd, byte[] payload, uint len, string timeoutMsg, Action<uint, object, object> onMatchedAck, Action<string> onTimeout, Action<string> onSendFail, uint expectedAckCmd = 0u, int retryIntervalMs = 1500, int totalTimeoutMs = 10000, int sendTimeoutMs = 3000, Action<uint, uint> onRetry = null, Action<uint, uint, string> onTimeoutEx = null, Action<uint, uint, string> onSendFailEx = null, bool requireMatchingSeq = false)
	{
		uint targetAckCmd = ((expectedAckCmd == 0) ? cmd : expectedAckCmd);
		CancelAckGuard();
		long guardVersion;
		lock (_ackGuardLock)
		{
			_ackGuardVersion++;
			guardVersion = _ackGuardVersion;
		}
		if (EnqueueSend(cmd, 1, 2, 0, payload, len, 0u, out var _, null, null, delegate(Exception ex)
		{
			if (TryCancelAckGuard(guardVersion))
			{
				string text2 = $"发送失败, cmd={cmd}, desc={ex.Message}";
				onSendFail?.Invoke(text2);
				onSendFailEx?.Invoke(cmd, 0u, text2);
			}
		}, delegate(uint seq)
		{
			DateTime now = DateTime.Now;
			PendingAckContext pending = new PendingAckContext
			{
				ExpectedCmd = targetAckCmd,
				ExpectedSeq = seq,
				RequireMatchingSeq = requireMatchingSeq,
				FirstSentAt = now,
				LastSentAt = now,
				RetryCount = 0u,
				Cmd = cmd,
				Payload = payload,
				Length = len,
				TimeoutMsg = timeoutMsg,
				OnMatchedAck = onMatchedAck,
				OnTimeout = onTimeout,
				OnSendFail = onSendFail,
				OnRetry = onRetry,
				OnTimeoutEx = onTimeoutEx,
				OnSendFailEx = onSendFailEx,
				RetryIntervalMs = retryIntervalMs,
				TotalTimeoutMs = totalTimeoutMs,
				SendTimeoutMs = sendTimeoutMs
			};
			StartAckGuard(pending, guardVersion);
		}) != 0)
		{
			if (TryCancelAckGuard(guardVersion))
			{
				string text = $"发送失败, cmd={cmd}";
				onSendFail?.Invoke(text);
				onSendFailEx?.Invoke(cmd, 0u, text);
			}
			return false;
		}
		return true;
	}

	public bool SendWithAckGuardDelay(uint cmd, byte[] payload, uint len, string timeoutMsg, Action<uint, object, object> onMatchedAck, Action<string> onTimeout, Action<string> onSendFail, int delayTime, uint expectedAckCmd = 0u, int retryIntervalMs = 1500, int totalTimeoutMs = 10000, int sendTimeoutMs = 3000, Action<uint, uint> onRetry = null, Action<uint, uint, string> onTimeoutEx = null, Action<uint, uint, string> onSendFailEx = null, bool requireMatchingSeq = false)
	{
		return SendWithAckGuardDelayAsync(cmd, payload, len, timeoutMsg, onMatchedAck, onTimeout, onSendFail, delayTime, expectedAckCmd, retryIntervalMs, totalTimeoutMs, sendTimeoutMs, onRetry, onTimeoutEx, onSendFailEx, requireMatchingSeq).GetAwaiter().GetResult();
	}

	public async Task<bool> SendWithAckGuardDelayAsync(uint cmd, byte[] payload, uint len, string timeoutMsg, Action<uint, object, object> onMatchedAck, Action<string> onTimeout, Action<string> onSendFail, int delayTime, uint expectedAckCmd = 0u, int retryIntervalMs = 1500, int totalTimeoutMs = 10000, int sendTimeoutMs = 3000, Action<uint, uint> onRetry = null, Action<uint, uint, string> onTimeoutEx = null, Action<uint, uint, string> onSendFailEx = null, bool requireMatchingSeq = false)
	{
		if (delayTime <= 0)
		{
			return SendWithAckGuard(cmd, payload, len, timeoutMsg, onMatchedAck, onTimeout, onSendFail, expectedAckCmd, retryIntervalMs, totalTimeoutMs, sendTimeoutMs, onRetry, onTimeoutEx, onSendFailEx, requireMatchingSeq);
		}
		CancellationTokenSource delayCts;
		lock (_ackGuardLock)
		{
			_ackDelayCts?.Cancel();
			_ackDelayCts?.Dispose();
			_ackDelayCts = new CancellationTokenSource();
			delayCts = _ackDelayCts;
		}
		try
		{
			await Task.Delay(delayTime, delayCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			if (delayCts.IsCancellationRequested)
			{
				return false;
			}
			return SendWithAckGuard(cmd, payload, len, timeoutMsg, onMatchedAck, onTimeout, onSendFail, expectedAckCmd, retryIntervalMs, totalTimeoutMs, sendTimeoutMs, onRetry, onTimeoutEx, onSendFailEx, requireMatchingSeq);
		}
		catch (TaskCanceledException)
		{
			return false;
		}
		catch (Exception ex2)
		{
			Exception ex3 = ex2;
			string failMsg = "AckGuardDelay error:" + ex3.Message;
			onSendFail?.Invoke(failMsg);
			onSendFailEx?.Invoke(cmd, 0u, failMsg);
			return false;
		}
		finally
		{
			lock (_ackGuardLock)
			{
				if (_ackDelayCts == delayCts)
				{
					_ackDelayCts = null;
				}
			}
			delayCts.Dispose();
		}
	}

	private int EnqueueSend(uint cmd, ushort type, ushort format, ushort userId, byte[] payload, uint len, uint retryNumber, out uint sentSeq, uint? fixedSeq = null, Action<uint> afterFrameSent = null, Action<Exception> onFrameSendFail = null, Action<uint> beforeFrameWrite = null)
	{
		sentSeq = 0u;
		SerialPort serial = _serial;
		if (_isClosed || serial == null || !serial.IsOpen || _sendQue == null)
		{
			Log("Serial not open");
			return -1;
		}
		uint seq = fixedSeq ?? _seq;
		ArProtocolHeader header = new ArProtocolHeader
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
			seq = seq,
			retry = retryNumber,
			crc32 = 0u
		};
		header.crc32 = 0u;
		byte[] array = StructToBytes(header);
		uint crc = CrcCalc(array, payload, (int)len);
		header.crc32 = crc;
		byte[] array2 = StructToBytes(header);
		try
		{
			lock (_writeLock)
			{
				SPEventArgs e = new SPEventArgs();
				e.HeaderBuffer = new byte[array.Length];
				Array.Copy(array2, e.HeaderBuffer, array2.Length);
				if (payload != null && payload.Length != 0)
				{
					e.DataBuffer = new byte[len];
					Array.Copy(payload, e.DataBuffer, (int)len);
				}
				if (cmd == 120 || cmd == 121)
				{
					_isAutoSendBBFreqConfig = true;
				}
				e.Cmd = cmd;
				e.OnFrameSending = ((beforeFrameWrite == null) ? null : ((Action)delegate
				{
					beforeFrameWrite(header.seq);
				}));
				e.OnFrameSent = ((afterFrameSent == null) ? null : ((Action)delegate
				{
					afterFrameSent(header.seq);
				}));
				e.OnFrameSendFailed = onFrameSendFail;
				_sendQue.Enqueue(e);
			}
		}
		catch (Exception ex)
		{
			Log("Write failed: " + ex.Message);
			return -1;
		}
		sentSeq = header.seq;
		if (!fixedSeq.HasValue)
		{
			_seq++;
		}
		return 0;
	}

	private int EnqueueSendDelay(uint cmd, byte[] payload, uint len, int delayTime, out uint sentSeq, uint? fixedSeq = null)
	{
		sentSeq = 0u;
		SerialPort serial = _serial;
		if (_isClosed || serial == null || !serial.IsOpen || _sendQue == null)
		{
			Log("Serial not open");
			return -1;
		}
		uint seq = fixedSeq ?? _seq;
		ArProtocolHeader obj = new ArProtocolHeader
		{
			magic = 1095914575u,
			version = 3292,
			type = 1,
			msgid = 0,
			unused = 43981,
			command = cmd,
			format = 2,
			userid = 0,
			length = len,
			seq = seq,
			retry = 0u,
			crc32 = 0u
		};
		obj.crc32 = 0u;
		byte[] array = StructToBytes(obj);
		uint crc = CrcCalc(array, payload, (int)len);
		obj.crc32 = crc;
		byte[] array2 = StructToBytes(obj);
		try
		{
			lock (_writeLock)
			{
				SPEventArgs e = new SPEventArgs();
				e.DelayTime = delayTime;
				e.HeaderBuffer = new byte[array.Length];
				Array.Copy(array2, e.HeaderBuffer, array2.Length);
				if (payload != null && payload.Length != 0)
				{
					e.DataBuffer = new byte[len];
					Array.Copy(payload, e.DataBuffer, (int)len);
				}
				if (cmd == 120 || cmd == 121)
				{
					_isAutoSendBBFreqConfig = true;
				}
				e.Cmd = cmd;
				_sendQue.Enqueue(e);
			}
		}
		catch (Exception ex)
		{
			Log("Write failed: " + ex.Message);
			return -1;
		}
		sentSeq = obj.seq;
		if (!fixedSeq.HasValue)
		{
			_seq++;
		}
		return 0;
	}

	public void SetSendBBFreqConfig(bool isAuto)
	{
		_isAutoSendBBFreqConfig = isAuto;
	}

	private bool WaitForAck(uint seq, int timeoutMs)
	{
		int i = 0;
		for (int num = 10; i < timeoutMs; i += num)
		{
			if (_ackStatus.TryRemove(seq, out var value))
			{
				if (value)
				{
					return true;
				}
				return false;
			}
			Thread.Sleep(num);
		}
		return false;
	}

	private static byte[] StructToBytes<T>(T obj) where T : struct
	{
		int num = Marshal.SizeOf(obj);
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			Marshal.StructureToPtr(obj, intPtr, fDeleteOld: false);
			Marshal.Copy(intPtr, array, 0, num);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return array;
	}

	private static T BytesToStruct<T>(byte[] bytes) where T : struct
	{
		GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		try
		{
			return Marshal.PtrToStructure<T>(gCHandle.AddrOfPinnedObject());
		}
		finally
		{
			gCHandle.Free();
		}
	}

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
					SerialPort serial = _serial;
					Action<bool, string, int> action = SerialConnectedStateChange;
					if (action != null)
					{
						action(currentState, ((serial != null) ? serial.PortName : null) ?? _portName ?? string.Empty, (serial != null) ? serial.BaudRate : 0);
					}
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

	private void WaitForBackgroundTask(Task task, string taskName)
	{
		if (task == null || (Task.CurrentId.HasValue && task.Id == Task.CurrentId.Value))
		{
			return;
		}
		try
		{
			if (!task.Wait(1500))
			{
				WriteLog.WriteLogFileToUI(taskName + " 停止超时", Color.DarkOrange);
			}
		}
		catch (AggregateException ex)
		{
			foreach (Exception innerException in ex.Flatten().InnerExceptions)
			{
				if (!(innerException is OperationCanceledException))
				{
					WriteLog.WriteLogFileToUI(taskName + " 停止异常，desc=" + innerException.Message, Color.Red);
				}
			}
		}
		catch (Exception ex2)
		{
			WriteLog.WriteLogFileToUI(taskName + " 停止异常，desc=" + ex2.Message, Color.Red);
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

	private uint CrcCalc(byte[] headerBytes, byte[] payload, int payloadLen = -1)
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
			int num2 = ((payloadLen >= 0) ? Math.Min(payloadLen, payload.Length) : payload.Length);
			for (int j = 0; j < num2; j++)
			{
				num = _crcTable[(num ^ payload[j]) & 0xFF] ^ (num >> 8);
			}
		}
		return num ^ 0xFFFFFFFFu;
	}

	private void Log(string s)
	{
		WriteLog.WriteLogFileToUI(s, Color.Brown);
	}

	public void Dispose()
	{
		Close();
	}

	private UsbDevInfo ParseDeviceInformation(ManagementBaseObject device)
	{
		try
		{
			string text = device["DeviceID"]?.ToString() ?? string.Empty;
			string text2 = device["Name"]?.ToString() ?? string.Empty;
			string text3 = device["Description"]?.ToString() ?? string.Empty;
			string input = device["PNPDeviceID"]?.ToString() ?? string.Empty;
			Match match = Regex.Match(text2, "\\(COM\\d+\\)");
			if (!match.Success)
			{
				return null;
			}
			string portName = match.Value.Trim(new char[2] { '(', ')' });
			Match match2 = Regex.Match(input, "VID_([0-9A-Fa-f]{4})&PID_([0-9A-Fa-f]{4})", RegexOptions.IgnoreCase);
			if (!match2.Success)
			{
				return null;
			}
			string value = match2.Groups[1].Value;
			string value2 = match2.Groups[2].Value;
			return new UsbDevInfo
			{
				PortName = portName,
				VID = value,
				PID = value2,
				DevName = text2,
				ConnectedTime = DateTime.Now
			};
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("解析设备信息时出错: " + ex.Message, Color.Red);
			return null;
		}
	}

	public ResDeviceInfo ParseDeviceInfo(byte[] frame)
	{
		ResDeviceInfo resDeviceInfo = new ResDeviceInfo();
		resDeviceInfo.receiveMaxSize = (SegLength = BitConverter.ToInt32(frame, 0));
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

	public ResAckInfo ParseAckInfo(byte[] frame)
	{
		ResAckInfo resAckInfo = new ResAckInfo();
		resAckInfo.Status = BitConverter.ToInt32(frame, 0);
		Array.Copy(frame, 4, resAckInfo.Detail, 0, 64);
		return resAckInfo;
	}

	public ResUpgradeStatus ParseUpgradeStatus(byte[] frame)
	{
		ResUpgradeStatus resUpgradeStatus = new ResUpgradeStatus();
		resUpgradeStatus.Percent = BitConverter.ToInt32(frame, 0);
		resUpgradeStatus.Status = BitConverter.ToInt32(frame, 4);
		int sourceIndex = 8;
		Array.Copy(frame, sourceIndex, resUpgradeStatus.Detail, 0, 64);
		return resUpgradeStatus;
	}

	public ResFileDataInfo ParseDataInfo(byte[] frame)
	{
		ResFileDataInfo resFileDataInfo = new ResFileDataInfo();
		resFileDataInfo.Length = BitConverter.ToInt32(frame, 0);
		int startIndex = 4;
		resFileDataInfo.Cursize = BitConverter.ToInt32(frame, startIndex);
		int num = 8;
		resFileDataInfo.Totalsize = BitConverter.ToInt32(frame, num);
		int num2 = num + 4;
		resFileDataInfo.Status = BitConverter.ToInt32(frame, num2);
		int sourceIndex = num2 + 4;
		Array.Copy(frame, sourceIndex, resFileDataInfo.Detail, 0, 64);
		return resFileDataInfo;
	}

	public string ParseBBFreqConfig(byte[] frame)
	{
		return Encoding.ASCII.GetString(frame).TrimEnd(new char[1]);
	}
}
