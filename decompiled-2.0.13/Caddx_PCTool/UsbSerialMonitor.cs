using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Caddx_PCTool;

public class UsbSerialMonitor : IDisposable
{
	private ManagementEventWatcher _deviceInsertWatcher;

	private ManagementEventWatcher _deviceRemoveWatcher;

	private readonly ConcurrentDictionary<string, UsbDevInfo> _connectedDevices = new ConcurrentDictionary<string, UsbDevInfo>();

	private readonly ConcurrentDictionary<string, string> _portToDeviceIdMap = new ConcurrentDictionary<string, string>();

	private bool _isMonitoring;

	private readonly ConcurrentDictionary<string, UsbSerialportFSM> _spFSMDict = new ConcurrentDictionary<string, UsbSerialportFSM>();

	private readonly ConcurrentDictionary<string, ResAscentInfo> _resAscentInfoDict = new ConcurrentDictionary<string, ResAscentInfo>();

	private int _plusing = 0;

	public bool IsMonitoring => _isMonitoring;

	public int CurrDevCount => _connectedDevices.Count;

	public event EventHandler<UsbDevInfo> DeviceConnected;

	public event EventHandler<UsbDevInfo> DeviceDisconnected;

	public event Action<List<UsbDevInfo>, object, object> OnManualSearchEvent;

	public UsbSerialMonitor()
	{
		InitializeWmiWatchers();
	}

	private void InitializeWmiWatchers()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		try
		{
			_deviceInsertWatcher = CreateWatcher("__InstanceCreationEvent");
			_deviceInsertWatcher.EventArrived += new EventArrivedEventHandler(OnDeviceInserted);
			_deviceRemoveWatcher = CreateWatcher("__InstanceDeletionEvent");
			_deviceRemoveWatcher.EventArrived += new EventArrivedEventHandler(OnDeviceRemoved);
		}
		catch (Exception ex)
		{
			Console.WriteLine("初始化WMI监听器失败: " + ex.Message);
			throw;
		}
	}

	private ManagementEventWatcher CreateWatcher(string type)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		return new ManagementEventWatcher((EventQuery)new WqlEventQuery("SELECT * FROM " + type + " WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity'"));
	}

	public List<UsbDevInfo> ManualSearchDevices()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		List<UsbDevInfo> list = new List<UsbDevInfo>();
		try
		{
			string[] portNames = SerialPort.GetPortNames();
			if (portNames.Length < 1)
			{
				return list;
			}
			HashSet<string> hashSet = new HashSet<string>(portNames.Select((string port) => port.TrimStart(new char[1] { '\\' })));
			foreach (string item in hashSet)
			{
				string text = "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%" + item + "%'";
				ManagementObjectSearcher val = new ManagementObjectSearcher(text);
				try
				{
					ManagementObjectCollection val2 = val.Get();
					ManagementObjectEnumerator enumerator2 = val2.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ManagementObject device = (ManagementObject)enumerator2.Current;
							UsbDevInfo usbDevInfo = ParseDeviceInformation((ManagementBaseObject)(object)device);
							if (usbDevInfo != null && Enumerable.Contains(portNames, usbDevInfo.PortName))
							{
								list.Add(usbDevInfo);
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				if (_isMonitoring)
				{
					UpdateDeviceSnapshot(list);
				}
			}
		}
		catch (Exception)
		{
		}
		return list;
	}

	public void StartMonitoring()
	{
		lock (this)
		{
			if (_isMonitoring)
			{
				return;
			}
			try
			{
				_deviceInsertWatcher.Start();
				_deviceRemoveWatcher.Start();
				_isMonitoring = true;
				List<UsbDevInfo> arg = ManualSearchDevices();
				OnManualSearchEvent?.Invoke(arg, "SearchComp", null);
				WriteLog.WriteLogFileToUI("启动wmi监听: ", Color.Blue);
			}
			catch (Exception ex)
			{
				WriteLog.WriteLogFileToUI("启动监听失败: " + ex.Message, Color.DarkRed);
			}
		}
	}

	public void StopMonitoring()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		lock (this)
		{
			if (!_isMonitoring)
			{
				return;
			}
			try
			{
				_deviceInsertWatcher.EventArrived -= new EventArrivedEventHandler(OnDeviceInserted);
				_deviceRemoveWatcher.EventArrived -= new EventArrivedEventHandler(OnDeviceRemoved);
				_deviceInsertWatcher.Stop();
				_deviceRemoveWatcher.Stop();
				_isMonitoring = false;
			}
			catch (Exception ex)
			{
				WriteLog.WriteLogFileToUI("停止WMI监听出错,desc=" + ex.Message, Color.Black);
			}
		}
	}

	public List<UsbDevInfo> GetCurrentDeviceSnapshot()
	{
		return _connectedDevices.Values.ToList();
	}

	public void ClearAllDevices()
	{
		_resAscentInfoDict.Clear();
		_spFSMDict.Clear();
		_connectedDevices.Clear();
		_portToDeviceIdMap.Clear();
	}

	private void OnDeviceInserted(object sender, EventArrivedEventArgs e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		try
		{
			ManagementBaseObject device = (ManagementBaseObject)e.NewEvent["TargetInstance"];
			UsbDevInfo deviceInfo = ParseDeviceInformation(device);
			if (deviceInfo == null)
			{
				WriteLog.WriteLogFileToUI("新设备解析失败,", Color.Red);
				return;
			}
			WriteLog.WriteLogFileToUI("发现新设备插入，串口名=" + deviceInfo.PortName + "，vid=" + deviceInfo.VID, Color.DarkBlue);
			if (!_connectedDevices.Values.Any((UsbDevInfo d) => d.PortName == deviceInfo.PortName && d.VID == deviceInfo.VID && d.PID == deviceInfo.PID))
			{
				AddDevice(deviceInfo);
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("处理设备插入事件时出错: " + ex.Message, Color.Red);
		}
	}

	private void OnDeviceRemoved(object sender, EventArrivedEventArgs e)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		try
		{
			ManagementBaseObject val = (ManagementBaseObject)e.NewEvent["TargetInstance"];
			string text = val["DeviceID"]?.ToString() ?? string.Empty;
			string text2 = val["Description"]?.ToString() ?? string.Empty;
			string input = val["Name"]?.ToString() ?? string.Empty;
			Match match = Regex.Match(input, "\\((COM\\d+)\\)");
			string text3 = (match.Success ? match.Groups[1].Value : string.Empty);
			RemoveDevice(text3, text3);
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("处理设备移除事件时出错: " + ex.Message, Color.Red);
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

	private void AddDevice(UsbDevInfo deviceInfo)
	{
		if (_connectedDevices.TryAdd(deviceInfo.PortName, deviceInfo))
		{
			_portToDeviceIdMap[deviceInfo.PortName] = deviceInfo.PortName;
			Task.Run(() => VerifyDeviceAvailability(deviceInfo));
		}
	}

	private async Task VerifyDeviceAvailability(UsbDevInfo deviceInfo)
	{
		await Task.Delay(300);
		try
		{
			string[] availablePorts = SerialPort.GetPortNames();
			if (!Enumerable.Contains(availablePorts, deviceInfo.PortName))
			{
				RemoveDevice(deviceInfo.PortName, deviceInfo.PortName);
				return;
			}
			int vidti = 0;
			int.TryParse(deviceInfo.VID, NumberStyles.HexNumber, null, out vidti);
			if (vidti <= 7542 && vidti >= 7531)
			{
				deviceInfo.IsInserted = true;
				DeviceConnected?.Invoke(this, deviceInfo);
				WriteLog.WriteLogFileToUI("新设备验证成功，设备usb信息= " + deviceInfo.ToString(), Color.DarkGreen);
			}
			else if (vidti == 7543 || vidti == 8711)
			{
				deviceInfo.IsInserted = true;
				DeviceConnected?.Invoke(this, deviceInfo);
				WriteLog.WriteLogFileToUI("vrx_pro验证成功，设备usb信息= " + deviceInfo.ToString(), Color.DarkBlue);
			}
			else if (vidti == 6790)
			{
				deviceInfo.IsInserted = true;
				DeviceConnected?.Invoke("gimbal", deviceInfo);
				WriteLog.WriteLogFileToUI("GM系列设备验证成功，设备usb信息= " + deviceInfo.ToString(), Color.DarkCyan);
			}
			else
			{
				WriteLog.WriteLogFileToUI("识别到vid范围外的usb设备,usb信息= " + deviceInfo.ToString(), Color.DarkOrange);
				DeviceConnected?.Invoke("Unsupport", deviceInfo);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			WriteLog.WriteLogFileToUI("验证设备可用性时出错: " + ex2.Message, Color.Red);
		}
	}

	private void Fsm_ResFrameInfo(UsbDevInfo arg1, int arg2, ArProtocolHeader arg3, byte[] arg4)
	{
		using UsbSerialportFSM usbSerialportFSM = new UsbSerialportFSM();
		uint command = arg3.command;
		uint num = command;
		if (num == 60)
		{
			ResAscentInfo resAscentInfo = new ResAscentInfo();
			ResDeviceInfo resDeviceInfo = usbSerialportFSM.ParseDeviceInfo(arg4);
			resAscentInfo.SN = Encoding.ASCII.GetString(resDeviceInfo.serialNumber).TrimEnd(new char[1]);
			resAscentInfo.FWVers = Encoding.ASCII.GetString(resDeviceInfo.firmwareInfo).TrimEnd(new char[1]);
			resAscentInfo.HWVers = Encoding.ASCII.GetString(resDeviceInfo.hardwareVersion).TrimEnd(new char[1]);
			resAscentInfo.MCUTemp = resDeviceInfo.cputemp;
			resAscentInfo.UsbInfo = arg1;
		}
	}

	private void OnAckReceived(AR_COMMAND arg1, object arg2, object arg3)
	{
		_plusing++;
		if (arg1 == AR_COMMAND.AR_COMMAND_FIND_DEVICE)
		{
			UsbDevInfo usbDevInfo = arg3 as UsbDevInfo;
			ResAscentInfo resAscentInfo = arg2 as ResAscentInfo;
			_spFSMDict[usbDevInfo.PortName].Close();
			WriteLog.WriteLogFileToUI("设备信息=" + resAscentInfo.ToString() + ",\r\nusb信息=" + usbDevInfo.ToString(), Color.DarkGreen);
			_resAscentInfoDict.TryAdd(usbDevInfo.PortName, resAscentInfo);
		}
		else
		{
			UsbDevInfo usbDevInfo = new UsbDevInfo();
			ResAscentInfo resAscentInfo = new ResAscentInfo();
		}
		if (_resAscentInfoDict.Count >= _connectedDevices.Count)
		{
			Thread.Sleep(100);
			_resAscentInfoDict.Clear();
			_spFSMDict.Clear();
			_plusing = 0;
		}
	}

	private void RemoveDevice(string deviceId, string portName)
	{
		string value2;
		if (_connectedDevices.TryRemove(deviceId, out var value))
		{
			_portToDeviceIdMap.TryRemove(portName, out value2);
			value.IsInserted = false;
			DeviceDisconnected?.Invoke(this, value);
			return;
		}
		if (_portToDeviceIdMap.TryRemove(portName, out var value3) && _connectedDevices.TryRemove(value3, out value))
		{
			value.IsInserted = false;
			DeviceDisconnected?.Invoke(this, value);
			return;
		}
		List<UsbDevInfo> list = _connectedDevices.Values.Where((UsbDevInfo d) => d.PortName == portName).ToList();
		foreach (UsbDevInfo item in list)
		{
			if (_connectedDevices.TryRemove(item.PortName, out value))
			{
				_portToDeviceIdMap.TryRemove(portName, out value2);
				value.IsInserted = false;
				DeviceDisconnected?.Invoke(this, value);
			}
		}
	}

	private void UpdateDeviceSnapshot(List<UsbDevInfo> currentDevices)
	{
		HashSet<string> hashSet = currentDevices.Select((UsbDevInfo d) => d.PortName).ToHashSet();
		HashSet<string> currentPortNames = currentDevices.Select((UsbDevInfo d) => d.PortName).ToHashSet();
		List<UsbDevInfo> list = _connectedDevices.Values.Where((UsbDevInfo d) => !currentPortNames.Contains(d.PortName)).ToList();
		foreach (UsbDevInfo item in list)
		{
			if (_connectedDevices.TryRemove(item.PortName, out var value))
			{
				_portToDeviceIdMap.TryRemove(item.PortName, out var _);
				value.IsInserted = false;
				DeviceDisconnected?.Invoke(this, value);
			}
		}
		foreach (UsbDevInfo currentDevice in currentDevices)
		{
			if (!_connectedDevices.ContainsKey(currentDevice.PortName))
			{
				AddDevice(currentDevice);
			}
		}
	}

	public void Dispose()
	{
		StopMonitoring();
		((Component)(object)_deviceInsertWatcher)?.Dispose();
		((Component)(object)_deviceRemoveWatcher)?.Dispose();
		ClearAllDevices();
	}
}
