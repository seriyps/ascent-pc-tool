using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI;

namespace Caddx_PCTool;

public class SerialPortHelper : IDisposable
{
	private BlockQueue<SPEventArgs> _sendQue;

	private bool _isRunSendThread;

	private Thread _sendThr;

	private BlockingCollection<byte[]> _recQue;

	private Thread _readerThread;

	private CancellationTokenSource _ctsRead;

	private CancellationTokenSource _ctsConsume;

	private string _portName;

	private int _baudRate;

	private int _readBufferSize;

	private int _pollSleepMs;

	private List<byte> _buffer;

	private byte[] _frameHeader;

	private SerialPort _sp;

	public bool UseBlockingRead { get; set; }

	public int ReadTimeoutMs { get; set; }

	public bool IsOpen
	{
		get
		{
			if (_sp != null)
			{
				return _sp.IsOpen;
			}
			return false;
		}
	}

	public string PortName
	{
		get
		{
			return _portName;
		}
		set
		{
			_portName = value;
		}
	}

	public int BaudRate
	{
		get
		{
			return _baudRate;
		}
		set
		{
			_baudRate = value;
		}
	}

	public SerialPort SPObj => _sp;

	public event EventHandler<SPEventArgs> OnSPReceivedEvnet;

	public event EventHandler<ErrorEventArgs> OnSPErrorHappenEvnet;

	public SerialPortHelper()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		_readBufferSize = 2048;
		_pollSleepMs = 5;
		_buffer = new List<byte>();
		_frameHeader = new byte[4] { 79, 84, 82, 65 };
		UseBlockingRead = false;
		ReadTimeoutMs = 500;
		base._002Ector();
		try
		{
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public SerialPortHelper(string portName, int BaudRate = 115200, int databit = 8, Handshake handshake = (Handshake)0)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		_readBufferSize = 2048;
		_pollSleepMs = 5;
		_buffer = new List<byte>();
		_frameHeader = new byte[4] { 79, 84, 82, 65 };
		UseBlockingRead = false;
		ReadTimeoutMs = 500;
		base._002Ector();
		try
		{
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void InitSendQueue()
	{
		_sendQue = new BlockQueue<SPEventArgs>(100);
		_isRunSendThread = true;
		_sendThr = new Thread(SPSendThreadMethod);
		_sendThr.Start(0);
		_recQue = new BlockingCollection<byte[]>();
		_ctsRead = new CancellationTokenSource();
		_ctsConsume = new CancellationTokenSource();
	}

	private void SPSendThreadMethod(object workIdx)
	{
		try
		{
			while (_isRunSendThread)
			{
				int idx = (int)workIdx;
				SerialportSend(idx);
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("SPSendThreadMethod error happen，Desc=" + ex.Message, Color.Red);
		}
	}

	private void SerialportSend(int idx)
	{
		int millisecondsTimeout = 10;
		try
		{
			if (_sendQue.TryDequeue(out var value))
			{
				if (_sp != null && _sp.IsOpen)
				{
					_sp.Write(value.DataBuffer, 0, value.DataBuffer.Length);
				}
				else
				{
					WriteLog.WriteLogFileToUI("串口未打开！", Color.Red);
				}
				millisecondsTimeout = ((value.DelayTime < 10) ? 10 : value.DelayTime);
			}
			Thread.Sleep(millisecondsTimeout);
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("SerialportSend error:" + ex.Message, Color.Red);
		}
	}

	private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		SerialPort val = (SerialPort)((sender is SerialPort) ? sender : null);
		int bytesToRead = val.BytesToRead;
		byte[] array = new byte[bytesToRead];
		try
		{
			val.Read(array, 0, bytesToRead);
			string text = "";
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				text = text + b.ToString("X2") + " ";
			}
			string text2 = Encoding.UTF8.GetString(array);
			string text3 = Encoding.ASCII.GetString(array);
			SPEventArgs e2 = new SPEventArgs();
			e2.Data = text;
			e2.DataBuffer = new byte[bytesToRead];
			array.CopyTo(e2.DataBuffer, 0);
			OnSPReceivedEvnet?.Invoke(sender, e2);
		}
		catch (Exception)
		{
		}
	}

	public int OpenSP(string portName, int BaudRate = 115200, int databit = 8, Handshake handshake = (Handshake)0)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			_portName = portName;
			_baudRate = BaudRate;
			_sp = new SerialPort(portName);
			_sp.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
			_sp.WriteBufferSize = 1048576;
			_sp.ReadBufferSize = 1048576;
			_sp.ReadTimeout = 2000;
			_sp.BaudRate = BaudRate;
			_sp.DataBits = databit;
			_sp.Parity = (Parity)0;
			_sp.Handshake = handshake;
			_sp.Open();
			Thread.Sleep(100);
			return _sp.IsOpen ? 1 : 2;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI(ex.Message, Color.Red);
			return -1;
		}
	}

	public int OpenSP(string portName, int BaudRate)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		try
		{
			_portName = portName;
			_baudRate = BaudRate;
			_sp = new SerialPort(portName);
			_sp.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
			_sp.WriteBufferSize = 4096;
			_sp.ReadBufferSize = 4096;
			_sp.ReadTimeout = 2000;
			_sp.BaudRate = BaudRate;
			_sp.DataBits = 8;
			_sp.Parity = (Parity)0;
			_sp.Handshake = (Handshake)0;
			_sp.Open();
			Thread.Sleep(100);
			return _sp.IsOpen ? 1 : 2;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI(ex.Message, Color.Red);
			return -1;
		}
	}

	public bool OpenSerialPortEx()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		try
		{
			if (_sp == null)
			{
				_sp = new SerialPort(_portName);
				_sp.WriteBufferSize = 4096;
				_sp.ReadBufferSize = 4096;
				_sp.ReadTimeout = 2000;
				_sp.BaudRate = 115200;
			}
			if (_sp.IsOpen)
			{
				_sp.Close();
				GD.Inst.DelayMethod(100);
			}
			_sp.Open();
			Thread.Sleep(100);
			Task.Run(delegate
			{
				ReaderLoop(_ctsRead.Token);
			});
			Task.Run(delegate
			{
				ConsumerLoop(_ctsConsume.Token);
			});
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public void SendCLIMsg(string msg)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SPEventArgs e = new SPEventArgs();
			byte[] bytes = Encoding.Default.GetBytes(msg);
			e.DataBuffer = new byte[bytes.Length + 1];
			Array.Copy(bytes, e.DataBuffer, bytes.Length);
			e.DataBuffer[bytes.Length] = 10;
			e.SendType = 2;
			_sendQue.Enqueue(e);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void SendCLIMsg(string[] msg)
	{
		SPEventArgs e = new SPEventArgs();
		e.DataArr = new string[msg.Length];
		Array.Copy(msg, e.DataArr, msg.Length);
		e.SendType = 4;
		_sendQue.Enqueue(e);
	}

	public void SendBytes(byte[] msg)
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!_sp.IsOpen)
			{
				_sp.Open();
				WriteLog.WriteLogFileToUI("重新打开串口，打开状态=" + _sp.IsOpen, Color.Black);
			}
			SPEventArgs e = new SPEventArgs();
			e.SendType = 2;
			e.DataBuffer = new byte[msg.Length];
			Array.Copy(msg, e.DataBuffer, msg.Length);
			_sendQue.Enqueue(e);
			string text = StaticMethod.BytesToHexString(msg);
			WriteLog.WriteLogFileToUI("发送帧=" + text, Color.Black);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void SendBytes(byte[] msg, int length)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SPEventArgs e = new SPEventArgs();
			e.SendType = 2;
			e.DataBuffer = new byte[length];
			Array.Copy(msg, e.DataBuffer, length);
			_sendQue.Enqueue(e);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void SendMSPPacket(byte cmd, byte VerType, byte[] msg)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SPEventArgs e = new SPEventArgs();
			e.Cmd = cmd;
			e.SendType = 0;
			e.VerType = VerType;
			if (msg == null)
			{
				Notification.error(Form.ActiveForm, "", "msg=null", (TAlignFrom)10, (Font)null, (int?)null);
				return;
			}
			e.DataBuffer = new byte[msg.Length];
			Array.Copy(msg, e.DataBuffer, msg.Length);
			_sendQue.Enqueue(e);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void SendMSPPacket(byte cmd, int delayms, byte[] msg)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SPEventArgs e = new SPEventArgs();
			e.Cmd = cmd;
			e.SendType = 0;
			e.VerType = 1;
			e.DelayTime = delayms;
			if (msg != null)
			{
				e.DataBuffer = new byte[msg.Length];
				Array.Copy(msg, e.DataBuffer, msg.Length);
			}
			_sendQue.Enqueue(e);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public bool SendMavPacket(byte[] msg)
	{
		if (_sp != null && _sp.IsOpen)
		{
			_sp.Write(msg, 0, msg.Length);
			return true;
		}
		return false;
	}

	private void ReaderLoop(CancellationToken token)
	{
		byte[] array = new byte[_readBufferSize];
		while (!token.IsCancellationRequested)
		{
			try
			{
				if (!_sp.IsOpen)
				{
					Thread.Sleep(2000);
					continue;
				}
				if (UseBlockingRead)
				{
					_sp.ReadTimeout = ReadTimeoutMs;
					try
					{
						int num = _sp.Read(array, 0, array.Length);
						if (num > 0)
						{
							_recQue.Add(CopyBuffer(array, num), token);
						}
					}
					catch (TimeoutException)
					{
					}
					continue;
				}
				int bytesToRead = _sp.BytesToRead;
				if (bytesToRead > 0)
				{
					int num2 = Math.Min(bytesToRead, array.Length);
					int num3 = _sp.Read(array, 0, num2);
					if (num3 > 0)
					{
						_recQue.Add(CopyBuffer(array, num3), token);
					}
				}
				else
				{
					Thread.Sleep(_pollSleepMs);
				}
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (InvalidOperationException) when (token.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex4)
			{
				ErrorEventArgs e = new ErrorEventArgs();
				e.ErrDesc = ex4.Message;
				OnSPErrorHappenEvnet?.Invoke(null, e);
				Thread.Sleep(500);
			}
		}
	}

	private static byte[] CopyBuffer(byte[] src, int len)
	{
		byte[] array = new byte[len];
		Buffer.BlockCopy(src, 0, array, 0, len);
		return array;
	}

	private void ConsumerLoop(CancellationToken token)
	{
		try
		{
			foreach (byte[] item in _recQue.GetConsumingEnumerable(token))
			{
				_buffer.AddRange(item);
				byte[] frame;
				while (TryExtractFrame(_buffer, out frame))
				{
					string text = BitConverter.ToString(frame).Replace("-", " ");
					WriteLog.WriteLogFileToUI("收到的返回帧=" + text + "\r\n长度=" + frame.Length, Color.Black);
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception)
		{
		}
	}

	private bool TryExtractFrame(List<byte> buffer, out byte[] frame)
	{
		frame = null;
		if (buffer.Count < 8)
		{
			return false;
		}
		int num = FindHeader(buffer);
		if (num < 0)
		{
			buffer.Clear();
			return false;
		}
		if (num > 0)
		{
			buffer.RemoveRange(0, num);
		}
		if (buffer.Count < 7)
		{
			return false;
		}
		int num2 = BitConverter.ToInt32(buffer.ToArray(), 20);
		int num3 = 7 + num2 + 1;
		if (buffer.Count < num3)
		{
			return false;
		}
		frame = buffer.GetRange(0, num3).ToArray();
		byte b = 0;
		for (int i = 7; i < 7 + num2; i++)
		{
			b += buffer[i];
		}
		byte b2 = buffer[7 + num2];
		buffer.RemoveRange(0, num3);
		return true;
	}

	private int FindHeader(List<byte> buffer)
	{
		for (int i = 0; i <= buffer.Count - _frameHeader.Length; i++)
		{
			bool flag = true;
			for (int j = 0; j < _frameHeader.Length; j++)
			{
				if (buffer[i + j] != _frameHeader[j])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return i;
			}
		}
		return -1;
	}

	public void CloseSP()
	{
		if (_sp != null && _sp.IsOpen)
		{
			_sp.Close();
		}
	}

	public int ReopenSP()
	{
		try
		{
			CloseSP();
			GD.Inst.DelayMethod(GD.Inst.ReOpenDelay);
			return _sp.IsOpen ? 1 : 2;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI(ex.Message, Color.Red);
			return -1;
		}
	}

	public void Dispose()
	{
		_ctsRead?.Cancel();
		_ctsConsume?.Cancel();
		Thread.Sleep(50);
		CloseSP();
		Thread.Sleep(50);
		_ctsRead.Cancel();
		_isRunSendThread = false;
		Thread.Sleep(50);
		_sendThr = null;
	}
}
