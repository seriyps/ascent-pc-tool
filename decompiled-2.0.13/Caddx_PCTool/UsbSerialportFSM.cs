using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
	private CancellationTokenSource _monitorCts;

	private Task _monitorTask;

	private const int SerialCheckInterval = 50;

	private const int SerialStableThreshold = 150;

	private bool _lastStableState = false;

	private BlockQueue<SPEventArgs> _sendQue;

	private CancellationTokenSource _sendCts;

	private string _portName;

	private SerialPort _serial;

	private CancellationTokenSource _recvCts;

	private readonly object _writeLock = new object();

	private List<byte> _recvBuffer = new List<byte>();

	private ConcurrentDictionary<uint, bool> _ackStatus = new ConcurrentDictionary<uint, bool>();

	private UsbDevInfo _devUsbInfo;

	private uint _seq = 0u;

	public int SegLength = 1024;

	public Action<uint, object, object> OnAckReceived;

	private readonly uint[] _crcTable = new uint[256];

	private bool _isfirst = false;

	private int retryTime = 2;

	public bool IsComOpened => _serial.IsOpen;

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
			_serial.ReadTimeout = 5000;
			_serial.WriteTimeout = 5000;
			_serial.Open();
			if (!_serial.IsOpen)
			{
				return false;
			}
			_portName = portName;
			StartSendThread();
			StartRecvThread();
			StartSerialMonitor();
			_isfirst = true;
			return true;
		}
		catch (Exception ex)
		{
			Log("OpenSerial Failed: " + ex.Message);
			return false;
		}
	}

	public void Close()
	{
		StopSendThread();
		StopRecvThread();
		StopSerialMonitor();
		if (_serial == null)
		{
			return;
		}
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
			WriteLog.WriteLogFileToUI("关闭串口失败！desc=" + ex.Message, Color.Red);
		}
	}

	public bool Reopen()
	{
		try
		{
			Close();
			Thread.Sleep(150);
			return Open(_portName);
		}
		catch (Exception)
		{
			return false;
		}
	}

	private string GetDevicePortname()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		string result = "";
		string[] portNames = SerialPort.GetPortNames();
		string[] array = portNames;
		foreach (string text in array)
		{
			string text2 = "SELECT * FROM Win32_PnPEntity WHERE Name LIKE  '%" + text + "%'";
			ManagementObjectSearcher val = new ManagementObjectSearcher(text2);
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
						if (usbDevInfo != null && usbDevInfo.VID == _devUsbInfo.VID)
						{
							return usbDevInfo.PortName;
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
		return result;
	}

	public bool Reopen(int timeout)
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Restart();
		try
		{
			Thread.Sleep(100);
			Close();
			Thread.Sleep(GD.Inst.ReOpenDelay_Asce);
			DateTime now = DateTime.Now;
			while (true)
			{
				bool flag;
				if (GD.Inst.IsPortnameChanged)
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
			Thread.Sleep(GD.Inst.ReOpenDelay_Asce);
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
		if (_sendQue == null)
		{
			_sendQue = new BlockQueue<SPEventArgs>(200);
		}
		_sendCts = new CancellationTokenSource();
		Task.Run(delegate
		{
			SendQueueLoop(_sendCts.Token);
		});
	}

	private void StopSendThread()
	{
		_sendCts?.Cancel();
	}

	private void SendQueueLoop(CancellationToken cts)
	{
		try
		{
			while (!cts.IsCancellationRequested)
			{
				SerialportSendMethod();
				Thread.Sleep(5);
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("SPSendThreadMethod error happen，Desc=" + ex.Message, Color.Red);
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
		catch (Exception)
		{
		}
	}

	private void StartRecvThread()
	{
		_recvCts = new CancellationTokenSource();
		Task.Run(() => RecvLoop(_recvCts.Token));
	}

	private void StopRecvThread()
	{
		_recvCts?.Cancel();
	}

	private async Task RecvLoop(CancellationToken cts)
	{
		int headerSize = Marshal.SizeOf(typeof(ArProtocolHeader));
		while (!cts.IsCancellationRequested)
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
							lock (_recvBuffer)
							{
								_recvBuffer.AddRange(buf);
							}
							while (true)
							{
								byte[] headerBytes = null;
								lock (_recvBuffer)
								{
									if (_recvBuffer.Count < headerSize)
									{
										break;
									}
									headerBytes = _recvBuffer.GetRange(0, headerSize).ToArray();
									goto IL_01b7;
								}
								IL_01b7:
								ArProtocolHeader header = BytesToStruct<ArProtocolHeader>(headerBytes);
								int fullLen = headerSize + (int)header.length;
								if (header.command != 116 && header.command != 118)
								{
									WriteLog.WriteLogFileToUI($"返回帧长度={_recvBuffer.Count},cmd={header.command}", Color.IndianRed);
								}
								lock (_recvBuffer)
								{
									if (_recvBuffer.Count < fullLen)
									{
										if (header.command == 60 && retryTime > 0)
										{
											WriteLog.WriteLogFileToUI($"重新发送设备信息帧，第{retryTime}次", Color.IndianRed);
										}
										break;
									}
									byte[] fullPacket = _recvBuffer.GetRange(0, fullLen).ToArray();
									_recvBuffer.RemoveRange(0, fullLen);
									byte[] payload = new byte[header.length];
									if (header.length != 0)
									{
										Array.Copy(fullPacket, headerSize, payload, 0, payload.Length);
									}
									HandlePacket(header.command, payload, header.length);
								}
							}
						}
					}
				}
				Thread.Sleep(5);
			}
			catch (Exception ex)
			{
				Log("RecvLoop exception: " + ex.Message);
			}
		}
	}

	private void HandlePacket(uint cmd, byte[] payload, uint payloadLen)
	{
		string text = "";
		try
		{
			switch (cmd)
			{
			case 0u:
				OnAckReceived?.Invoke(cmd, "UNKOWNCMD", payload);
				break;
			case 60u:
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
				ResAscentInfo arg2 = resAscentInfo;
				OnAckReceived?.Invoke(60u, arg2, _devUsbInfo);
				break;
			}
			case 3u:
				OnAckReceived?.Invoke(3u, null, null);
				break;
			case 114u:
				OnAckReceived?.Invoke(114u, null, null);
				break;
			case 115u:
				if (cmd == 0)
				{
					string text4 = "";
				}
				OnAckReceived?.Invoke(115u, null, null);
				break;
			case 116u:
			{
				ResFileDataInfo resFileDataInfo = ParseDataInfo(payload);
				text = Encoding.ASCII.GetString(resFileDataInfo.Detail).TrimEnd(new char[1]);
				if (cmd == 0 || text != "OK")
				{
					string text2 = "";
				}
				OnAckReceived?.Invoke(116u, resFileDataInfo, null);
				break;
			}
			case 117u:
			{
				ResFileDataInfo resFileDataInfo2 = ParseDataInfo(payload);
				text = Encoding.ASCII.GetString(resFileDataInfo2.Detail).TrimEnd(new char[1]);
				if (cmd == 0 || text != "OK")
				{
					string text3 = "";
				}
				OnAckReceived?.Invoke(117u, resFileDataInfo2, null);
				break;
			}
			case 118u:
			{
				int num = BitConverter.ToInt32(payload, 0);
				ResUpgradeStatus arg = ParseUpgradeStatus(payload);
				OnAckReceived?.Invoke(118u, arg, null);
				break;
			}
			default:
				OnAckReceived?.Invoke(cmd, "UNKOWNCMD", null);
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
		if (_serial == null || !_serial.IsOpen)
		{
			Log("Serial not open");
			return -1;
		}
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
		do
		{
			obj.crc32 = 0u;
			byte[] array = StructToBytes(obj);
			uint crc = CrcCalc(array, payload);
			obj.crc32 = crc;
			byte[] array2 = StructToBytes(obj);
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
						if (obj.command == 116)
						{
						}
					}
					_sendQue.Enqueue(e);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Log("Write failed: " + ex.Message);
				return -1;
			}
		}
		while (!flag);
		_seq++;
		return 0;
	}

	public int CommandSerialSend(uint cmd, ushort type, ushort format, ushort userId, byte[] payload, uint len)
	{
		if (_serial == null || !_serial.IsOpen)
		{
			Log("Serial not open");
			return -1;
		}
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
		do
		{
			obj.crc32 = 0u;
			byte[] array = StructToBytes(obj);
			uint crc = CrcCalc(array, payload);
			obj.crc32 = crc;
			byte[] array2 = StructToBytes(obj);
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
						if (obj.command == 116)
						{
						}
					}
					_sendQue.Enqueue(e);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Log("Write failed: " + ex.Message);
				return -1;
			}
		}
		while (!flag);
		_seq++;
		return 0;
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

	private uint CrcCalc(byte[] headerBytes, byte[] payload)
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

	private void Log(string s)
	{
		LogInfo?.Invoke(s);
	}

	public void Dispose()
	{
		StopSendThread();
		StopRecvThread();
		StopSerialMonitor();
		if (_serial == null)
		{
			return;
		}
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
			WriteLog.WriteLogFileToUI("关闭串口失败！desc=" + ex.Message, Color.Red);
		}
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

	public void TestFindDevice()
	{
		string hex = "00 04 00 00 76 32 2E 31 2E 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 43 41 44 44 58 5F 47 4D 33 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 00 00 00 43 41 44 44 58 5F 47 4D 33 5F 46 57 5F 56 32 2E 31 2E 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 53 4E 30 30 33 41 30 30 34 39 33 32 33 35 35 31 30 36 33 34 33 31 33 37 33 30 00 00 00 00 00 00 48 57 5F 56 32 2E 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4F 4B 3A 44 65 76 69 63 65 20 69 6E 69 74 69 61 6C 69 7A 65 64 20 73 75 63 63 65 73 73 66 75 6C 6C 79 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
		byte[] frame = StaticMethod.HexStringToByteArray(hex);
		ResDeviceInfo resDeviceInfo = ParseDeviceInfo(frame);
		string info = resDeviceInfo.ToString();
		WriteLog.WriteLogFileToUI(info, Color.DarkBlue);
	}
}
