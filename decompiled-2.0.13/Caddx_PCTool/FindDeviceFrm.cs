using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class FindDeviceFrm : UserControl, IDisposable
{
	private List<DeviceInfoCard> _devCardList = new List<DeviceInfoCard>();

	private NulllDeviceCtrl _nullDevCtrl;

	private List<UsbDevInfo> _usbInfoList = new List<UsbDevInfo>();

	private string faq_en = "FAQ";

	private string downfw_en = "Download firmware";

	private string faq_cn = "常见问题";

	private string downfw_cn = "下载固件在本地";

	private Stopwatch _swReshow = new Stopwatch();

	private UsbSerialMonitor _usbMonitor;

	private readonly ConcurrentDictionary<string, UsbSerialportFSM> _spFSMDict = new ConcurrentDictionary<string, UsbSerialportFSM>();

	private readonly ConcurrentDictionary<string, ResAscentInfo> _resAscentInfoDict = new ConcurrentDictionary<string, ResAscentInfo>();

	private readonly ConcurrentDictionary<string, DeviceInfoCard> _devInfoCardDict = new ConcurrentDictionary<string, DeviceInfoCard>();

	private Stopwatch _swManualSearch = new Stopwatch();

	private BlockQueue<ResAscentInfo> _ctrlQue;

	private CancellationTokenSource _ctrlCts;

	private readonly Dictionary<string, CancellationTokenSource> _timeoutCtsDict = new Dictionary<string, CancellationTokenSource>();

	private int _insertplus = 0;

	private CancellationTokenSource _ctsTimeout;

	private int retryTime = 2;

	private IContainer components = null;

	private PageHeader pageHeader1;

	private Button button2;

	private Button button1;

	private TableLayoutPanel tableLayoutPanel1;

	public event EventHandler<DevCardEventArgs> OnFindDeviceFrmConnectEvent;

	public FindDeviceFrm()
	{
		InitializeComponent();
	}

	private void FindDeviceFrm_Load(object sender, EventArgs e)
	{
		_usbMonitor = new UsbSerialMonitor();
		_usbMonitor.DeviceConnected += _usbMonitor_DeviceConnected;
		_usbMonitor.DeviceDisconnected += _usbMonitor_DeviceDisconnected;
		_usbMonitor.OnManualSearchEvent += _usbMonitor_OnManualSearchEvent;
		_usbMonitor.StartMonitoring();
		_ctrlQue = new BlockQueue<ResAscentInfo>(30);
		_ctrlCts = new CancellationTokenSource();
		Task.Run(() => DevCardCtrlLoop(_ctrlCts.Token));
		ReloadLang();
	}

	private void OnDevCardHappen(object sender, DevCardEventArgs e)
	{
		try
		{
			if (!_resAscentInfoDict.TryGetValue(e.PortName, out var value))
			{
				WriteLog.WriteLogFileToUI("从AscentInfo字典取值失败，串口名=" + e.PortName, Color.Red);
				bool flag = _spFSMDict.TryGetValue(e.PortName, out var value2);
				e.AscentInfo = new ResAscentInfo
				{
					UsbInfo = value2.UsbInfo,
					DevName = "SimGM",
					SN = "1234567890"
				};
				OnFindDeviceFrmConnectEvent?.Invoke(value2, e);
				Thread.Sleep(200);
				((Control)this).Hide();
			}
			else
			{
				e.AscentInfo = value;
				_spFSMDict.TryGetValue(e.PortName, out var value3);
				OnFindDeviceFrmConnectEvent?.Invoke(value3, e);
				Thread.Sleep(200);
				((Control)this).Hide();
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.OnDevCardHappen error，desc=" + ex.Message, Color.Red);
		}
	}

	private async Task DevCardCtrlLoop(CancellationToken cts)
	{
		try
		{
			while (!cts.IsCancellationRequested)
			{
				if (_ctrlQue.TryDequeue(out var resInfo))
				{
					if (GD.Inst.IsSWFirstRun)
					{
						DevCardEventArgs dce = new DevCardEventArgs
						{
							Desc = "CloseSplashScreen"
						};
						OnFindDeviceFrmConnectEvent(null, dce);
					}
					if (resInfo.UsbInfo.IsInserted)
					{
						await Task.Delay(100);
						AddCtrlAsync(resInfo.UsbInfo.PortName, resInfo);
						WriteLog.WriteLogFileToUI("设备返回信息=" + resInfo.ToString(), Color.DarkGreen);
					}
				}
				await Task.Delay(10);
				resInfo = null;
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			WriteLog.WriteLogFileToUI("FindDeviceFrm.DevCardCtrlLoop   error，Desc=" + ex2.Message, Color.Red);
		}
	}

	private void _usbMonitor_DeviceDisconnected(object sender, UsbDevInfo e)
	{
		try
		{
			RemoveDevcard(e.PortName);
			int currDevCount = _usbMonitor.CurrDevCount;
			if (!_spFSMDict.TryGetValue(e.PortName, out var _))
			{
				WriteLog.WriteLogFileToUI("该设备未添加到字典,串口名=" + e.PortName, Color.DarkRed);
				return;
			}
			_spFSMDict[e.PortName]?.Close();
			_spFSMDict.TryRemove(e.PortName, out var _);
			WriteLog.WriteLogFileToUI("拔出设备,串口名=" + e.PortName, Color.DarkOrange);
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm._usbMonitor_DeviceDisconnected error,desc=" + ex.Message, Color.Red);
		}
	}

	private void _usbMonitor_DeviceConnected(object sender, UsbDevInfo e)
	{
		_insertplus++;
		CloseSplashScreen();
		string text = sender.ToString();
		string text2 = text;
		string text3 = text2;
		if (text3 == "Unsupport")
		{
			DisplayUnsupportCard(e);
			return;
		}
		DisplaySearchCard(e);
		SendAndDisplayCtrl(_usbMonitor.CurrDevCount, e);
	}

	private void _usbMonitor_OnManualSearchEvent(List<UsbDevInfo> arg1, object arg2, object arg3)
	{
		string text = arg2 as string;
		_usbInfoList = arg1;
		if (_usbInfoList.Count < 1)
		{
			DisplayNulllDeviceCtrl();
			if (text == "SearchComp" && GD.Inst.IsSWFirstRun)
			{
				DevCardEventArgs e = new DevCardEventArgs
				{
					Desc = "CloseSplashScreen"
				};
				OnFindDeviceFrmConnectEvent(null, e);
			}
		}
	}

	private void SendAndDisplayCtrl(int count, UsbDevInfo usbinfo)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			if (GD.Inst.IsSWFirstRun)
			{
				CloseSplashScreen();
			}
			if (count < 1)
			{
				DisplayNulllDeviceCtrl();
			}
			else if (tableLayoutPanel1.RowCount < 2)
			{
				InitTLPRowsCols(2, 4);
			}
		});
		int result = 0;
		int.TryParse(usbinfo.VID, NumberStyles.HexNumber, null, out result);
		UsbSerialportFSM usbSerialportFSM = new UsbSerialportFSM(usbinfo, 3);
		usbSerialportFSM.OnAckReceived = (Action<uint, object, object>)Delegate.Combine(usbSerialportFSM.OnAckReceived, new Action<uint, object, object>(OnAckReceived));
		bool flag = false;
		if (result <= 8711 && result >= 7531)
		{
			if (!usbSerialportFSM.Open(usbinfo.PortName))
			{
				WriteLog.WriteLogFileToUI("串口连接失败，串口名=" + usbinfo.PortName, Color.DarkRed);
				return;
			}
			StartTimeout((uint)GD.Inst.FindDevTimeout_Asce, "FindDevice", usbinfo);
			if (usbSerialportFSM.CommandSerialSend(60u, 1, 2, 0, null, 0u, 0u, 2000) != 0)
			{
				WriteLog.WriteLogFileToUI("发送失败，串口名=" + usbinfo.PortName, Color.DarkRed);
				return;
			}
			WriteLog.WriteLogFileToUI("发送设备信息请求帧，串口名=" + usbinfo.PortName, Color.Black);
		}
		else
		{
			if (result != 6790)
			{
				WriteLog.WriteLogFileToUI("不支持该设备，VID=" + usbinfo.VID + ",串口名=" + usbinfo.PortName, Color.DarkRed);
				DisplayUnsupportCard(usbinfo);
				return;
			}
			Thread.Sleep(500);
			flag = usbSerialportFSM.Open(usbinfo.PortName, 460800);
			StartTimeout((uint)GD.Inst.FindDevTimeout_Asce, "FindDevice", usbinfo);
			int num = usbSerialportFSM.CommandSerialSend(60u, 1, 2, 0, null, 0u, 0u, 2000);
		}
		flag = _spFSMDict.TryAdd(usbinfo.PortName, usbSerialportFSM);
	}

	private void DisplaySearchCard(UsbDevInfo info)
	{
		try
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				DeviceInfoCard deviceInfoCard = new DeviceInfoCard();
				((Control)deviceInfoCard).Show();
				((Control)deviceInfoCard).Name = info.PortName;
				((Control)deviceInfoCard).Dock = (DockStyle)5;
				((Control)deviceInfoCard).Margin = new Padding(10, 10, 10, 10);
				deviceInfoCard.OnDevCardHappenEvent += OnDevCardHappen;
				deviceInfoCard.ShowSearchDev();
				if (tableLayoutPanel1.RowCount < 2)
				{
					InitTLPRowsCols(2, 4);
				}
				((ControlCollection)tableLayoutPanel1.Controls).Add((Control)(object)deviceInfoCard);
				if (!_devInfoCardDict.TryAdd(info.PortName, deviceInfoCard))
				{
					WriteLog.WriteLogFileToUI("添加设备卡片失败,串口名=" + info.PortName, Color.Red);
				}
			});
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.DisplaySearchCard error,desc=" + ex.Message, Color.Red);
		}
	}

	private void DisplayUnsupportCard(UsbDevInfo info)
	{
		try
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				DeviceInfoCard deviceInfoCard = new DeviceInfoCard();
				((Control)deviceInfoCard).Show();
				((Control)deviceInfoCard).Name = info.PortName;
				((Control)deviceInfoCard).Dock = (DockStyle)5;
				((Control)deviceInfoCard).Margin = new Padding(10, 10, 10, 10);
				deviceInfoCard.OnDevCardHappenEvent += OnDevCardHappen;
				deviceInfoCard.ShowUnsupportDev(info.PortName);
				if (tableLayoutPanel1.RowCount < 2)
				{
					InitTLPRowsCols(2, 4);
				}
				((ControlCollection)tableLayoutPanel1.Controls).Add((Control)(object)deviceInfoCard);
				if (!_devInfoCardDict.TryAdd(info.PortName, deviceInfoCard))
				{
					WriteLog.WriteLogFileToUI("添加设备卡片失败,串口名=" + info.PortName, Color.Red);
				}
			});
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.DisplaySearchCard error,desc=" + ex.Message, Color.Red);
		}
	}

	private void DisplayGMCard(UsbDevInfo info)
	{
		try
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				DeviceInfoCard deviceInfoCard = new DeviceInfoCard();
				((Control)deviceInfoCard).Show();
				((Control)deviceInfoCard).Name = info.PortName;
				((Control)deviceInfoCard).Dock = (DockStyle)5;
				((Control)deviceInfoCard).Margin = new Padding(10, 10, 10, 10);
				deviceInfoCard.OnDevCardHappenEvent += OnDevCardHappen;
				deviceInfoCard.ShowSearchDev();
				if (tableLayoutPanel1.RowCount < 2)
				{
					InitTLPRowsCols(2, 4);
				}
				((ControlCollection)tableLayoutPanel1.Controls).Add((Control)(object)deviceInfoCard);
				if (!_devInfoCardDict.TryAdd(info.PortName, deviceInfoCard))
				{
					WriteLog.WriteLogFileToUI("添加设备卡片失败,串口名=" + info.PortName, Color.Red);
				}
				Thread.Sleep(200);
				deviceInfoCard.SetDeviceName("GM1-V2", info.PortName);
			});
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.DisplaySearchCard error,desc=" + ex.Message, Color.Red);
		}
	}

	private void RemoveDevcard(string portname)
	{
		try
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				if (_devInfoCardDict.TryGetValue(portname, out var _))
				{
					_devInfoCardDict.TryRemove(portname, out var value2);
					((ControlCollection)tableLayoutPanel1.Controls).Remove((Control)(object)value2);
					if (_devInfoCardDict.Count < 1)
					{
						DisplayNulllDeviceCtrl();
					}
				}
			});
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.RemoveDevcard error,desc=" + ex.Message, Color.Red);
		}
	}

	public void ManualSearchDev()
	{
		_swReshow.Restart();
		_swManualSearch.Restart();
		_usbInfoList = _usbMonitor.ManualSearchDevices();
		if (_usbInfoList.Count < 1)
		{
			DisplayNulllDeviceCtrl();
			_usbMonitor.StartMonitoring();
			DevCardEventArgs e = new DevCardEventArgs
			{
				Desc = "CloseSplashScreen"
			};
			OnFindDeviceFrmConnectEvent(null, e);
			return;
		}
		WriteLog.WriteLogFileToUI($"当前设备数={_usbInfoList.Count}", Color.Black);
		for (int i = 0; i < _usbInfoList.Count; i++)
		{
			UsbSerialportFSM usbSerialportFSM = new UsbSerialportFSM(_usbInfoList[i]);
			usbSerialportFSM.OnAckReceived = (Action<uint, object, object>)Delegate.Combine(usbSerialportFSM.OnAckReceived, new Action<uint, object, object>(OnAckReceived));
			if (!usbSerialportFSM.Open(_usbInfoList[i].PortName))
			{
				WriteLog.WriteLogFileToUI("连接失败，串口名=" + _usbInfoList[i].PortName, Color.DarkRed);
				break;
			}
			int num = usbSerialportFSM.CommandSerialSend(60u, 1, 2, 0, null, 0u, 0u, 2000);
			if (!_spFSMDict.TryAdd(_usbInfoList[i].PortName, usbSerialportFSM))
			{
				WriteLog.WriteLogFileToUI("添加到字典失败，串口名=" + _usbInfoList[i].PortName, Color.DarkRed);
				break;
			}
		}
	}

	private void OnAckReceived(uint arg1, object arg2, object arg3)
	{
		try
		{
			if (arg1 == 60)
			{
				UsbDevInfo usbDevInfo = arg3 as UsbDevInfo;
				ResAscentInfo resAscentInfo = arg2 as ResAscentInfo;
				_resAscentInfoDict.TryAdd(usbDevInfo.PortName, resAscentInfo);
				_ctrlQue.Enqueue(resAscentInfo);
				StopTimeout();
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.OnAckReceived error,desc=" + ex.Message, Color.Red);
		}
	}

	private void OnDevConn(int arg1, ResAscentInfo arg2, ConcurrentDictionary<string, ResAscentInfo> arg3, object arg4)
	{
	}

	private void button2_Click(object sender, EventArgs e)
	{
	}

	private void button1_Click(object sender, EventArgs e)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			((Control)pageHeader1).Padding = new Padding(0, 15, 130, 0);
		});
	}

	private void CloseFindDeviceFrm()
	{
		if (_usbMonitor != null)
		{
			_usbMonitor.DeviceConnected -= _usbMonitor_DeviceConnected;
			_usbMonitor.DeviceDisconnected -= _usbMonitor_DeviceDisconnected;
			_usbMonitor.OnManualSearchEvent -= _usbMonitor_OnManualSearchEvent;
			_usbMonitor.StopMonitoring();
			_usbMonitor.Dispose();
			_usbMonitor = null;
		}
	}

	public void ReloadFont()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
		Button obj = button2;
		Font font = (((Control)button1).Font = val);
		((Control)obj).Font = font;
	}

	public void ReloadLang(bool isReloadFont = false)
	{
		try
		{
			switch ((LangType)GD.Inst.CurrLang)
			{
			case LangType.zh_CN:
				((Control)this).Invoke((Delegate)(Action)delegate
				{
					((Control)button1).Text = downfw_cn;
					((Control)button2).Text = faq_cn;
				});
				break;
			case LangType.en_US:
				((Control)this).Invoke((Delegate)(Action)delegate
				{
					((Control)button1).Text = downfw_en;
					((Control)button2).Text = faq_en;
				});
				break;
			}
			if (_devInfoCardDict.Count > 0)
			{
				for (int num = 0; num < _devInfoCardDict.Count; num++)
				{
					_devInfoCardDict.ElementAt(num).Value.ReloadLang();
				}
			}
			else
			{
				_nullDevCtrl?.ReloadLang();
			}
			if (isReloadFont)
			{
				ReloadFont();
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.ReloadLang error ,desc=" + ex.Message, Color.Red);
		}
	}

	private void DisplayNulllDeviceCtrl()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		((ControlCollection)tableLayoutPanel1.Controls).Clear();
		_devCardList.Clear();
		((ControlCollection)tableLayoutPanel1.Controls).Clear();
		if (tableLayoutPanel1.RowCount != 1 || tableLayoutPanel1.ColumnCount != 1)
		{
			((TableLayoutStyleCollection)tableLayoutPanel1.ColumnStyles).Clear();
			((TableLayoutStyleCollection)tableLayoutPanel1.RowStyles).Clear();
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.ColumnCount = 1;
			for (int i = 0; i < 1; i++)
			{
				tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 100f));
				tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 100f));
			}
		}
		_nullDevCtrl = new NulllDeviceCtrl();
		_nullDevCtrl.OnNulllDeviceHappenEvent += OnDevCardHappen;
		((Control)_nullDevCtrl).Dock = (DockStyle)5;
		tableLayoutPanel1.Controls.Add((Control)(object)_nullDevCtrl, 0, 0);
		_nullDevCtrl.SetDeviceName(isshow: true, "", "请连接USB设备");
	}

	private void InitTLPRowsCols(int RowCount, int ColumnCount)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Expected O, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			((ControlCollection)tableLayoutPanel1.Controls).Clear();
			_devCardList.Clear();
			((TableLayoutStyleCollection)tableLayoutPanel1.ColumnStyles).Clear();
			((TableLayoutStyleCollection)tableLayoutPanel1.RowStyles).Clear();
			tableLayoutPanel1.RowCount = RowCount;
			tableLayoutPanel1.ColumnCount = ColumnCount;
			for (int i = 0; i < ColumnCount; i++)
			{
				tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 100f / (float)ColumnCount));
			}
			for (int j = 0; j < RowCount; j++)
			{
				tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 100f / (float)RowCount));
			}
		});
	}

	public void RemoveCtrl(string ctrlName)
	{
		Control[] array = ((ControlCollection)tableLayoutPanel1.Controls).Find(ctrlName, true);
		if (array.Length != 0)
		{
			((ControlCollection)tableLayoutPanel1.Controls).Remove(array[0]);
			((Component)(object)array[0]).Dispose();
		}
	}

	private void RemoveCtrl(int count, UsbDevInfo usb)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			if (count < 1)
			{
				DisplayNulllDeviceCtrl();
			}
		});
	}

	public void AddCtrl(ConcurrentDictionary<string, ResAscentInfo> resDict)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (tableLayoutPanel1.RowCount < 2)
		{
			InitTLPRowsCols(2, 4);
		}
		DeviceInfoCard deviceInfoCard = new DeviceInfoCard();
		((Control)deviceInfoCard).Name = resDict.ElementAt(0).Key;
		((Control)deviceInfoCard).Dock = (DockStyle)5;
		((Control)deviceInfoCard).Margin = new Padding(10, 10, 10, 10);
		deviceInfoCard.OnDevCardHappenEvent += OnDevCardHappen;
		((Control)deviceInfoCard).Show();
		deviceInfoCard.ShowSearchDev();
		((ControlCollection)tableLayoutPanel1.Controls).Add((Control)(object)deviceInfoCard);
	}

	private void AddCtrlAsync(string portname, ResAscentInfo resInfo)
	{
		try
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				DeviceInfoCard value = new DeviceInfoCard();
				if (!_devInfoCardDict.TryGetValue(portname, out value))
				{
					WriteLog.WriteLogFileToUI(",串口名=" + portname, Color.Red);
				}
				else
				{
					((ControlCollection)tableLayoutPanel1.Controls).Add((Control)(object)_devInfoCardDict[portname]);
					_devInfoCardDict[portname].SetDeviceInfo(resInfo);
					((Control)_devInfoCardDict[portname]).Show();
				}
			});
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.AddCtrlAsync error,desc=" + ex.Message, Color.Red);
		}
	}

	private void CloseSplashScreen()
	{
		DevCardEventArgs e = new DevCardEventArgs();
		e.Desc = "CloseSplashScreen";
		OnFindDeviceFrmConnectEvent?.Invoke(null, e);
	}

	public void DisplayCurrDev()
	{
	}

	private void StartTimeout(uint ms, string state, UsbDevInfo usbinfo)
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
					Fail(state, usbinfo);
				}
			}
			catch (OperationCanceledException)
			{
			}
		});
	}

	private void StopTimeout()
	{
		_ctsTimeout?.Cancel();
	}

	private void Fail(string msg, UsbDevInfo usbinfo)
	{
		StopTimeout();
		WriteLog.WriteLogFileToUI(msg + "超时,portname=" + usbinfo.PortName, Color.Red);
		if (!(msg == "FindDevice"))
		{
			return;
		}
		if (!_devInfoCardDict.TryGetValue(usbinfo.PortName, out var value))
		{
			WriteLog.WriteLogFileToUI("_devInfoCardDict取值失败", Color.Red);
			return;
		}
		int num = Convert.ToInt32(usbinfo.VID, 16);
		if (num == 6790)
		{
			ResAscentInfo deviceInfo = new ResAscentInfo
			{
				UsbInfo = usbinfo,
				DevName = "CaddxSimGM"
			};
			value.SetDeviceInfo(deviceInfo);
			((Control)value).Show();
		}
		else
		{
			value.ShowUnsupportDev(usbinfo.PortName);
		}
	}

	private void StartTimeout(uint ms, string state, UsbDevInfo usbinfo, int idx = 1)
	{
		if (_timeoutCtsDict.TryGetValue(usbinfo.PortName, out var value))
		{
			value.Cancel();
			value.Dispose();
		}
		CancellationTokenSource cts = new CancellationTokenSource();
		_timeoutCtsDict[usbinfo.PortName] = cts;
		CancellationToken token = cts.Token;
		Task.Run(async delegate
		{
			try
			{
				await Task.Delay((int)ms, token).ConfigureAwait(continueOnCapturedContext: false);
				if (!token.IsCancellationRequested)
				{
					Fail(state, usbinfo);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception)
			{
			}
			finally
			{
				lock (_timeoutCtsDict)
				{
					if (_timeoutCtsDict.TryGetValue(usbinfo.PortName, out var toRemove) && toRemove == cts)
					{
						_timeoutCtsDict.Remove(usbinfo.PortName);
						toRemove.Dispose();
					}
				}
			}
		});
	}

	private void StopTimeout(string portname, int idx = 1)
	{
		if (_timeoutCtsDict.TryGetValue(portname, out var value))
		{
			value.Cancel();
		}
	}

	private void Fail(string msg, UsbDevInfo usbinfo, int idx = 1)
	{
		StopTimeout(usbinfo.PortName);
		WriteLog.WriteLogFileToUI(msg + "超时,portname=" + usbinfo.PortName, Color.Red);
		if (!(msg == "FindDevice"))
		{
			return;
		}
		if (!_devInfoCardDict.TryGetValue(usbinfo.PortName, out var value))
		{
			WriteLog.WriteLogFileToUI("_devInfoCardDict取值失败", Color.Red);
			return;
		}
		int num = Convert.ToInt32(usbinfo.VID, 16);
		if (num == 6790)
		{
		}
		value.ShowUnsupportDev(usbinfo.PortName);
	}

	public void GetAllUsbDevInfo(out List<UsbDevInfo> usbList)
	{
		usbList = new List<UsbDevInfo>();
		for (int i = 0; i < _resAscentInfoDict.Count; i++)
		{
			usbList.Add(_resAscentInfoDict.ElementAt(i).Value.UsbInfo);
		}
	}

	public void ShowDevOccupy()
	{
		for (int i = 0; i < _devInfoCardDict.Count; i++)
		{
			_devInfoCardDict.ElementAt(i).Value.ShowDevOccupy();
		}
	}

	public void Dispose()
	{
		try
		{
			for (int i = 0; i < _spFSMDict.Count; i++)
			{
				_spFSMDict.ElementAt(i).Value.Close();
			}
			_ctrlCts?.Cancel();
			_ctrlQue?.ClearAndClose();
			CloseFindDeviceFrm();
			_spFSMDict.Clear();
			_resAscentInfoDict.Clear();
		}
		catch (Exception)
		{
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((ContainerControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Expected O, but got Unknown
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Expected O, but got Unknown
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Expected O, but got Unknown
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Expected O, but got Unknown
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Expected O, but got Unknown
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Expected O, but got Unknown
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Expected O, but got Unknown
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Expected O, but got Unknown
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		pageHeader1 = new PageHeader();
		button1 = new Button();
		button2 = new Button();
		tableLayoutPanel1 = new TableLayoutPanel();
		((Control)pageHeader1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)pageHeader1).BackColor = Color.FromArgb(38, 41, 43);
		((Control)pageHeader1).Controls.Add((Control)(object)button1);
		((Control)pageHeader1).Controls.Add((Control)(object)button2);
		pageHeader1.DividerThickness = 0f;
		((Control)pageHeader1).Dock = (DockStyle)1;
		((Control)pageHeader1).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((IControl)pageHeader1).HandDragFolder = false;
		((Control)pageHeader1).Location = new Point(0, 0);
		((Control)pageHeader1).Margin = new Padding(0);
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Size = new Size(1200, 50);
		((Control)pageHeader1).TabIndex = 1;
		((Control)pageHeader1).Text = "";
		pageHeader1.UseTextBold = false;
		((Control)button1).Anchor = (AnchorStyles)6;
		button1.BackColor = Color.FromArgb(233, 30, 99);
		button1.DefaultBack = Color.FromArgb(26, 66, 130, 248);
		((Control)button1).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button1.ForeColor = Color.FromArgb(66, 130, 248);
		button1.IconRatio = 0f;
		button1.IconSvg = "DownloadOutlined";
		((Control)button1).Location = new Point(1001, 5);
		((Control)button1).Margin = new Padding(0);
		((Control)button1).Name = "button1";
		button1.Radius = 10;
		((Control)button1).Size = new Size(172, 40);
		((Control)button1).TabIndex = 0;
		((Control)button1).Text = "下载固件到本地";
		((IControl)button1).Visible = false;
		((Control)button1).Click += button1_Click;
		((Control)button2).Anchor = (AnchorStyles)6;
		button2.BackColor = Color.White;
		button2.DefaultBack = Color.White;
		((Control)button2).Font = new Font("思源黑体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)128);
		button2.ForeColor = Color.White;
		button2.Ghost = true;
		button2.Icon = (Image)(object)Resources.语言选择;
		button2.IconGap = 0.5f;
		button2.IconRatio = 1f;
		button2.IconSvg = "";
		((Control)button2).Location = new Point(842, 5);
		((Control)button2).Margin = new Padding(0);
		((Control)button2).Name = "button2";
		((Control)button2).Size = new Size(150, 40);
		((Control)button2).TabIndex = 1;
		((Control)button2).Text = "常见问题";
		((IControl)button2).Visible = false;
		((Control)button2).Click += button2_Click;
		((Control)tableLayoutPanel1).BackColor = Color.FromArgb(38, 41, 43);
		tableLayoutPanel1.ColumnCount = 1;
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 25f));
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 25f));
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 25f));
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 25f));
		((Control)tableLayoutPanel1).Dock = (DockStyle)5;
		((Control)tableLayoutPanel1).Location = new Point(0, 50);
		((Control)tableLayoutPanel1).Margin = new Padding(0);
		((Control)tableLayoutPanel1).Name = "tableLayoutPanel1";
		tableLayoutPanel1.RowCount = 1;
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 50f));
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 50f));
		((Control)tableLayoutPanel1).Size = new Size(1200, 775);
		((Control)tableLayoutPanel1).TabIndex = 2;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(9f, 22f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.NavajoWhite;
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Controls.Add((Control)(object)pageHeader1);
		((Control)this).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)this).Margin = new Padding(4, 6, 4, 6);
		((Control)this).Name = "FindDeviceFrm";
		((Control)this).Size = new Size(1200, 825);
		((UserControl)this).Load += FindDeviceFrm_Load;
		((Control)pageHeader1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
