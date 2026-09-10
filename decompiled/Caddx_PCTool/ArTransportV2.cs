using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Caddx_PCTool;

public class ArTransportV2 : IDisposable
{
	private class PendingAckContext
	{
		public uint Cmd;

		public uint ExpectedCmd;

		public uint Seq;

		public byte[] Payload;

		public uint Length;

		public DateTime FirstSentAt;

		public DateTime LastSentAt;

		public uint RetryCount;

		public TaskCompletionSource<AckResultV2> Tcs;

		public int SendTimeoutMs;
	}

	private readonly object _ackLock = new object();

	private readonly object _writeLock = new object();

	private readonly RingBuffer _recvBuffer = new RingBuffer(262144);

	private readonly byte[] _headerBuf = new byte[36];

	private BlockQueue<SPEventArgs> _sendQue = new BlockQueue<SPEventArgs>(200);

	private CancellationTokenSource _sendCts;

	private CancellationTokenSource _recvCts;

	private CancellationTokenSource _ackCts;

	private SerialPort _serial;

	private string _portName;

	private uint _seq;

	private UsbDevInfo _usbInfo;

	private PendingAckContext _pendingAck;

	protected virtual int AckRetryIntervalMs => 2000;

	protected virtual int AckTotalTimeoutMs => 10000;

	protected virtual int DefaultSendTimeoutMs => 3000;

	protected virtual int RebootWaitBeforeCloseMs => 1000;

	protected virtual int RebootWaitBeforeReconnectMs => 10000;

	public bool IsOpen => _serial != null && _serial.IsOpen;

	public UsbDevInfo UsbInfo => _usbInfo;

	public event Action<ArPacketEnvelopeV2> OnPacketReceived;

	public virtual bool Open(string portName, UsbDevInfo usbInfo, int baudRate = 115200)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		try
		{
			Close();
			_serial = new SerialPort(portName, baudRate, (Parity)0, 8, (StopBits)1);
			_serial.WriteBufferSize = 1075200;
			_serial.ReadBufferSize = 1075200;
			_serial.ReadTimeout = -1;
			_serial.WriteTimeout = -1;
			_serial.Open();
			if (!_serial.IsOpen)
			{
				return false;
			}
			_portName = portName;
			_usbInfo = usbInfo;
			_recvBuffer.ClearAllBuffer();
			_sendQue = new BlockQueue<SPEventArgs>(200);
			StartSendLoop();
			StartRecvLoop();
			return true;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("ArTransportV2.Open failed: " + ex.Message, Color.Red);
			return false;
		}
	}

	public virtual void Close()
	{
		CancelPendingAck(AckStatusV2.Cancelled, "transport closed");
		_sendCts?.Cancel();
		_recvCts?.Cancel();
		_sendQue?.ClearAndClose();
		_recvBuffer.ClearAllBuffer();
		if (_serial == null)
		{
			return;
		}
		try
		{
			_serial.Close();
			((Component)(object)_serial).Dispose();
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("ArTransportV2.Close failed: " + ex.Message, Color.Red);
		}
		finally
		{
			_serial = null;
		}
	}

	public virtual int SendNoAck(uint cmd, byte[] payload, uint len, int delayBeforeSendMs = 0)
	{
		uint sentSeq;
		return EnqueueSend(cmd, payload, len, 0u, delayBeforeSendMs, out sentSeq, null);
	}

	public virtual Task<AckResultV2> SendWithAckAsync(uint cmd, byte[] payload, uint len, uint expectedAckCmd = 0u, int delayBeforeSendMs = 0, CancellationToken ct = default(CancellationToken))
	{
		if (ct.IsCancellationRequested)
		{
			return Task.FromCanceled<AckResultV2>(ct);
		}
		if (EnqueueSend(cmd, payload, len, 0u, delayBeforeSendMs, out var sentSeq, null) != 0)
		{
			return Task.FromResult(new AckResultV2
			{
				Status = AckStatusV2.SendFailed,
				Cmd = cmd,
				Seq = sentSeq,
				ErrorMessage = "send failed"
			});
		}
		TaskCompletionSource<AckResultV2> taskCompletionSource = new TaskCompletionSource<AckResultV2>(TaskCreationOptions.RunContinuationsAsynchronously);
		uint expectedCmd = ((expectedAckCmd == 0) ? cmd : expectedAckCmd);
		PendingAckContext pendingAck = new PendingAckContext
		{
			Cmd = cmd,
			ExpectedCmd = expectedCmd,
			Seq = sentSeq,
			Payload = payload,
			Length = len,
			FirstSentAt = DateTime.Now,
			LastSentAt = DateTime.Now,
			RetryCount = 0u,
			Tcs = taskCompletionSource,
			SendTimeoutMs = DefaultSendTimeoutMs
		};
		lock (_ackLock)
		{
			CancelPendingAck_NoLock(AckStatusV2.Cancelled, "superseded");
			_pendingAck = pendingAck;
			_ackCts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[1] { ct });
		}
		StartAckLoop(_ackCts.Token);
		ct.Register(delegate
		{
			CancelPendingAck(AckStatusV2.Cancelled, "cancelled");
		});
		return taskCompletionSource.Task;
	}

	public virtual async Task<bool> ReconnectAfterRebootAsync(string mode, int timeoutMs, CancellationToken ct = default(CancellationToken))
	{
		byte[] modeBytes = BuildRebootModeBytes(mode);
		if (SendNoAck(3u, modeBytes, (uint)modeBytes.Length) != 0)
		{
			return false;
		}
		return await ReconnectAsync(timeoutMs, ct);
	}

	public virtual async Task<bool> ReconnectAsync(int timeoutMs, CancellationToken ct = default(CancellationToken))
	{
		Close();
		await Task.Delay(GetReopenDelayMs(), ct);
		Stopwatch sw = Stopwatch.StartNew();
		int oldPortRetryCount = 0;
		while (sw.ElapsedMilliseconds < timeoutMs)
		{
			ct.ThrowIfCancellationRequested();
			bool connected;
			if (GD.Inst.ForceSearchMode == 1)
			{
				string tryPort = ResolveReconnectPortName();
				connected = TryReconnectPort(tryPort);
			}
			else if (GD.Inst.ForceSearchMode == 3)
			{
				if (oldPortRetryCount < GD.Inst.PortRetryTimeLimit)
				{
					if (TryReconnectPort(_portName))
					{
						return true;
					}
					oldPortRetryCount++;
					if (oldPortRetryCount < GD.Inst.PortRetryTimeLimit)
					{
						await Task.Delay(1000, ct);
					}
					continue;
				}
				string tryPort2 = ResolveReconnectPortName();
				connected = !string.IsNullOrEmpty(tryPort2) && TryReconnectPort(tryPort2);
			}
			else
			{
				connected = TryReconnectPort(_portName);
			}
			if (connected)
			{
				return true;
			}
			await Task.Delay(GetReconnectRetryDelayMs(), ct);
		}
		return false;
	}

	protected virtual int GetReconnectRetryDelayMs()
	{
		return Math.Max(1, GD.Inst.ReElectDelay);
	}

	protected virtual int GetReopenDelayMs()
	{
		string matchSource;
		if (GD.Inst != null)
		{
			return Math.Max(0, GD.Inst.ResolveReOpenDelay_Asce(_usbInfo, out matchSource));
		}
		return 0;
	}

	protected virtual bool TryReconnectPort(string tryPort)
	{
		return !string.IsNullOrEmpty(tryPort) && Open(tryPort, _usbInfo);
	}

	private void StartSendLoop()
	{
		_sendCts = new CancellationTokenSource();
		Task.Run(() => SendQueueLoop(_sendCts.Token));
	}

	private async Task SendQueueLoop(CancellationToken token)
	{
		try
		{
			while (!token.IsCancellationRequested)
			{
				if (_sendQue.TryDequeue(out var spe))
				{
					if (spe.DelayTime > 0)
					{
						await Task.Delay(spe.DelayTime, token);
					}
					if (_serial != null && _serial.IsOpen)
					{
						_serial.Write(spe.HeaderBuffer, 0, spe.HeaderBuffer.Length);
						if (spe.DataBuffer != null && spe.DataBuffer.Length != 0)
						{
							_serial.Write(spe.DataBuffer, 0, spe.DataBuffer.Length);
						}
					}
				}
				else
				{
					await Task.Delay(1, token);
				}
				spe = null;
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			Exception ex3 = ex2;
			WriteLog.WriteLogFileToUI("ArTransportV2.SendQueueLoop error: " + ex3.Message, Color.Red);
		}
	}

	private void StartRecvLoop()
	{
		_recvCts = new CancellationTokenSource();
		Task.Run(() => RecvLoop(_recvCts.Token));
	}

	private async Task RecvLoop(CancellationToken token)
	{
		int headerSize = 36;
		while (!token.IsCancellationRequested)
		{
			try
			{
				if (_serial != null && _serial.IsOpen)
				{
					int available = _serial.BytesToRead;
					if (available > 0)
					{
						byte[] buf = new byte[available];
						int read = _serial.Read(buf, 0, available);
						if (read > 0)
						{
							_recvBuffer.Write(buf, 0, read);
							while (true)
							{
								int buffered = _recvBuffer.PeekHeader(_headerBuf, headerSize);
								if (buffered < 0)
								{
									break;
								}
								ArProtocolHeaderV2 header = ArPacketCodecV2.ParseHeader(_headerBuf);
								int fullLen = headerSize + (int)header.length;
								if (fullLen > 5242880)
								{
									_recvBuffer.ClearAllBuffer();
									break;
								}
								if (buffered < fullLen)
								{
									break;
								}
								byte[] payload = null;
								if (header.length != 0)
								{
									payload = new byte[header.length];
									_recvBuffer.PeekAt(headerSize, payload, payload.Length);
								}
								_recvBuffer.Skip(fullLen);
								HandlePacket(header.command, header.seq, payload);
							}
						}
					}
				}
				await Task.Delay(5, token);
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex2)
			{
				WriteLog.WriteLogFileToUI("ArTransportV2.RecvLoop error: " + ex2.Message, Color.Red);
			}
		}
	}

	private void HandlePacket(uint cmd, uint seq, byte[] payload)
	{
		TaskCompletionSource<AckResultV2> taskCompletionSource = null;
		uint retryCount = 0u;
		lock (_ackLock)
		{
			if (_pendingAck != null && _pendingAck.ExpectedCmd == cmd && _pendingAck.Seq == seq)
			{
				taskCompletionSource = _pendingAck.Tcs;
				retryCount = _pendingAck.RetryCount;
				_pendingAck = null;
				_ackCts?.Cancel();
			}
		}
		if (taskCompletionSource != null)
		{
			taskCompletionSource.TrySetResult(new AckResultV2
			{
				Status = AckStatusV2.Matched,
				Cmd = cmd,
				Seq = seq,
				RetryCount = retryCount,
				RawPayload = payload
			});
		}
		else
		{
			OnPacketReceived?.Invoke(new ArPacketEnvelopeV2
			{
				Cmd = cmd,
				Seq = seq,
				RawPayload = payload
			});
		}
	}

	private void StartAckLoop(CancellationToken token)
	{
		Task.Run(async delegate
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					PendingAckContext snapshot;
					lock (_ackLock)
					{
						snapshot = _pendingAck;
					}
					if (snapshot == null)
					{
						break;
					}
					DateTime now = DateTime.Now;
					if ((now - snapshot.FirstSentAt).TotalMilliseconds >= (double)AckTotalTimeoutMs)
					{
						CancelPendingAck(AckStatusV2.Timeout, "ack timeout");
						break;
					}
					if ((now - snapshot.LastSentAt).TotalMilliseconds >= (double)AckRetryIntervalMs)
					{
						uint nextRetry = snapshot.RetryCount + 1;
						if (EnqueueSend(snapshot.Cmd, snapshot.Payload, snapshot.Length, nextRetry, 0, out var _, snapshot.Seq) != 0)
						{
							CancelPendingAck(AckStatusV2.SendFailed, "resend failed");
							break;
						}
						lock (_ackLock)
						{
							if (_pendingAck == null)
							{
								break;
							}
							_pendingAck.RetryCount = nextRetry;
							_pendingAck.LastSentAt = DateTime.Now;
						}
					}
					await Task.Delay(100, token);
					snapshot = null;
				}
			}
			catch (OperationCanceledException)
			{
			}
		}, token);
	}

	private void CancelPendingAck(AckStatusV2 status, string error)
	{
		lock (_ackLock)
		{
			CancelPendingAck_NoLock(status, error);
		}
	}

	private void CancelPendingAck_NoLock(AckStatusV2 status, string error)
	{
		if (_pendingAck == null)
		{
			_ackCts?.Cancel();
			return;
		}
		PendingAckContext pendingAck = _pendingAck;
		_pendingAck = null;
		_ackCts?.Cancel();
		pendingAck.Tcs.TrySetResult(new AckResultV2
		{
			Status = status,
			Cmd = pendingAck.ExpectedCmd,
			Seq = pendingAck.Seq,
			RetryCount = pendingAck.RetryCount,
			RawPayload = pendingAck.Payload,
			ErrorMessage = error
		});
	}

	protected virtual int EnqueueSend(uint cmd, byte[] payload, uint len, uint retryNumber, int delayBeforeSendMs, out uint sentSeq, uint? fixedSeq)
	{
		sentSeq = fixedSeq ?? _seq;
		if (_serial == null || !_serial.IsOpen)
		{
			return -1;
		}
		if (!fixedSeq.HasValue)
		{
			_seq++;
		}
		byte[] array = ArPacketCodecV2.BuildPacket(cmd, 1, 2, 0, payload, len, sentSeq, retryNumber);
		try
		{
			lock (_writeLock)
			{
				SPEventArgs e = new SPEventArgs();
				e.DelayTime = delayBeforeSendMs;
				e.Cmd = cmd;
				e.HeaderBuffer = new byte[36];
				SPEventArgs e2 = e;
				Buffer.BlockCopy(array, 0, e2.HeaderBuffer, 0, 36);
				if (array.Length > 36)
				{
					e2.DataBuffer = new byte[array.Length - 36];
					Buffer.BlockCopy(array, 36, e2.DataBuffer, 0, e2.DataBuffer.Length);
				}
				_sendQue.Enqueue(e2);
			}
			return 0;
		}
		catch
		{
			return -1;
		}
	}

	private byte[] BuildRebootModeBytes(string mode)
	{
		byte[] array = (string.IsNullOrEmpty(mode) ? new byte[32] : Encoding.ASCII.GetBytes(mode));
		Array.Resize(ref array, 32);
		return array;
	}

	private string ResolveReconnectPortName()
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		string text = _portName;
		if (_usbInfo == null || string.IsNullOrWhiteSpace(_usbInfo.VID) || string.IsNullOrWhiteSpace(_usbInfo.PID))
		{
			return text;
		}
		try
		{
			string text2 = _usbInfo.VID.Trim().ToUpperInvariant();
			string text3 = _usbInfo.PID.Trim().ToUpperInvariant();
			string[] portNames = SerialPort.GetPortNames();
			foreach (string text4 in portNames)
			{
				string text5 = "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%" + text4 + "%'";
				ManagementObjectSearcher val = new ManagementObjectSearcher(text5);
				try
				{
					ManagementObjectEnumerator enumerator = val.Get().GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ManagementObject val2 = (ManagementObject)enumerator.Current;
							string input = ((ManagementBaseObject)val2)["Name"]?.ToString() ?? string.Empty;
							string input2 = ((ManagementBaseObject)val2)["PNPDeviceID"]?.ToString() ?? string.Empty;
							Match match = Regex.Match(input, "\\(COM\\d+\\)");
							if (!match.Success)
							{
								continue;
							}
							Match match2 = Regex.Match(input2, "VID_([0-9A-Fa-f]{4})&PID_([0-9A-Fa-f]{4})", RegexOptions.IgnoreCase);
							if (match2.Success)
							{
								string text6 = match2.Groups[1].Value.ToUpperInvariant();
								string text7 = match2.Groups[2].Value.ToUpperInvariant();
								if (text6 == text2 && text7 == text3)
								{
									text = (_portName = match.Value.Trim(new char[2] { '(', ')' }));
									_usbInfo.PortName = text;
									return text;
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
		}
		catch
		{
		}
		return text;
	}

	public void Dispose()
	{
		Close();
	}
}
