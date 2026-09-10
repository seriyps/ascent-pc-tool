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
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class FindDeviceFrm : UserControl, IDisposable
{
	private List<DeviceInfoCard> _devCardList = new List<DeviceInfoCard>();

	private NulllDeviceCtrl _nullDevCtrl;

	private List<UsbDevInfo> _usbInfoList = new List<UsbDevInfo>();

	private Stopwatch _swReshow = new Stopwatch();

	private UsbSerialMonitor _usbMonitor;

	private readonly ConcurrentDictionary<string, SerialPortSessionHandle> _discoverySessions = new ConcurrentDictionary<string, SerialPortSessionHandle>(StringComparer.OrdinalIgnoreCase);

	private readonly ConcurrentDictionary<string, ResAscentInfo> _resAscentInfoDict = new ConcurrentDictionary<string, ResAscentInfo>();

	private readonly ConcurrentDictionary<string, UsbDevInfo> _usbDevInfoDict = new ConcurrentDictionary<string, UsbDevInfo>(StringComparer.OrdinalIgnoreCase);

	private readonly ConcurrentDictionary<string, DeviceInfoCard> _devInfoCardDict = new ConcurrentDictionary<string, DeviceInfoCard>();

	private Stopwatch _swManualSearch = new Stopwatch();

	private BlockQueue<ResAscentInfo> _ctrlQue;

	private CancellationTokenSource _ctrlCts;

	private readonly Dictionary<string, CancellationTokenSource> _timeoutCtsDict = new Dictionary<string, CancellationTokenSource>();

	private int _insertplus = 0;

	private CancellationTokenSource _ctsTimeout;

	private int retryTime = 2;

	private static readonly byte MAX_ROW_COUNT = 3;

	private static readonly byte MAX_COL_COUNT = 2;

	private IContainer components = null;

	private PageHeader pageHeader1;

	private Button button2;

	private Button button1;

	private TableLayoutPanel tableLayoutPanel1;

	public event EventHandler<DevCardEventArgs> OnFindDeviceFrmConnectEvent;

	public FindDeviceFrm()
	{
		InitializeComponent();
		GD.Inst.FindDeviceFrm = this;
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
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		CommModalFrm commModalFrm = new CommModalFrm();
		try
		{
			string key = SerialPortSessionText.NormalizePortName(e.PortName);
			bool flag = _discoverySessions.TryGetValue(key, out var value) && IsCurrentDiscoverySession(value);
			_resAscentInfoDict.TryGetValue(key, out var value2);
			_usbDevInfoDict.TryGetValue(key, out var value3);
			string vid = value3?.VID ?? value2?.UsbInfo?.VID ?? value?.Vid;
			bool flag2 = IsGimbalVid(vid);
			if (!flag && (!flag2 || value2 == null))
			{
				commModalFrm.SetAllTxt("Error", "The device discovery session has expired,portname=" + e.PortName, Color.Red);
				((Form)commModalFrm).ShowDialog();
				WriteLog.WriteLogFileToUI("设备发现会话已失效，串口名=" + e.PortName, Color.DarkOrange);
				return;
			}
			e.Name = "connect";
			e.SessionHandle = (flag ? value : null);
			if (value2 == null)
			{
				if (!flag || !GD.Inst.SerialPortSessions.TryGetTransport(value, out var transport) || !(transport is LegacyFsmSessionTransport { Fsm: not null } legacyFsmSessionTransport))
				{
					commModalFrm.SetAllTxt("Error", "Failed to obtain the serial port from the discovery session.,portname=" + e.PortName, Color.Red);
					((Form)commModalFrm).ShowDialog();
					WriteLog.WriteLogFileToUI("从发现会话获取串口失败，串口名=" + e.PortName, Color.Red);
					return;
				}
				e.AscentInfo = new ResAscentInfo
				{
					UsbInfo = legacyFsmSessionTransport.Fsm.UsbInfo,
					DevName = "SimGM",
					SN = "1234567890"
				};
			}
			else
			{
				if (string.IsNullOrEmpty(value2.DevName))
				{
					string text = AscentDeviceNameResolver.ResolveDisplayName(value2);
					if (!string.IsNullOrEmpty(text))
					{
						value2.DevName = text;
					}
				}
				e.AscentInfo = value2;
			}
			OnFindDeviceFrmConnectEvent?.Invoke(null, e);
			if (GD.Inst.CurrSelFunType != FuntionType.findDevice && GD.Inst.CurrSelFunType != FuntionType.camhub)
			{
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
			GD.Inst.SerialPortSessions.HandleDeviceRemoved(e.PortName);
			string key = SerialPortSessionText.NormalizePortName(e.PortName);
			_discoverySessions.TryRemove(key, out var _);
			_resAscentInfoDict.TryRemove(key, out var _);
			_usbDevInfoDict.TryRemove(key, out var _);
			RemoveDevcard(e.PortName);
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
		SendAndDisplayCtrlAsync(_usbMonitor.CurrDevCount, e);
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

	private void EnsureDiscoveryLayout(int count)
	{
		if (((Control)this).InvokeRequired)
		{
			((Control)this).BeginInvoke((Delegate)(Action)delegate
			{
				EnsureDiscoveryLayout(count);
			});
			return;
		}
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
			InitTLPRowsCols(MAX_COL_COUNT, MAX_ROW_COUNT);
		}
	}

	private async Task SendAndDisplayCtrlAsync(int count, UsbDevInfo usbinfo)
	{
		EnsureDiscoveryLayout(count);
		if (count < 1)
		{
			return;
		}
		int.TryParse(usbinfo.VID, NumberStyles.HexNumber, null, out var vid);
		IEnumerable<int> values = Enum.GetValues(typeof(VIDConst)).Cast<int>();
		if (vid < values.Min() || vid > values.Max())
		{
			WriteLog.WriteLogFileToUI("不支持该设备，VID=" + usbinfo.VID + ",串口名=" + usbinfo.PortName, Color.DarkRed);
			DisplayUnsupportCard(usbinfo);
			return;
		}
		_usbDevInfoDict[SerialPortSessionText.NormalizePortName(usbinfo.PortName)] = usbinfo;
		int baudRate = GetDiscoveryBaudRate(usbinfo);
		SerialPortAcquireResult acquired = await GD.Inst.SerialPortSessions.AcquireAsync(usbinfo, SerialPortOwner.DeviceDiscovery, new SerialPortSessionOpenOptions
		{
			TransportKind = SerialPortTransportKind.LegacyFsm,
			BaudRate = baudRate
		}, CancellationToken.None);
		string portName = SerialPortSessionText.NormalizePortName(usbinfo.PortName);
		if (!acquired.Succeeded)
		{
			WriteLog.WriteLogFileToUI(string.Format("获取发现串口会话失败，串口名={0}, reason={1}, owner={2}, state={3}", new object[4] { usbinfo.PortName, acquired.FailureReason, acquired.CurrentOwner, acquired.CurrentState }), Color.DarkRed);
			ShowFindDeviceFailure(portName, usbinfo);
			return;
		}
		LegacyFsmSessionTransport legacy = default(LegacyFsmSessionTransport);
		int num;
		if (GD.Inst.SerialPortSessions.TryGetTransport(acquired.Handle, out var transport))
		{
			legacy = transport as LegacyFsmSessionTransport;
			if (legacy != null && legacy.Fsm != null)
			{
				num = ((!_discoverySessions.TryAdd(portName, acquired.Handle)) ? 1 : 0);
				goto IL_037f;
			}
		}
		num = 1;
		goto IL_037f;
		IL_037f:
		if (num != 0)
		{
			await GD.Inst.SerialPortSessions.ReleaseAsync(acquired.Handle, SerialPortReleaseReason.TransportUnavailable);
			WriteLog.WriteLogFileToUI("获取发现串口传输失败，串口名=" + usbinfo.PortName, Color.DarkRed);
			ShowFindDeviceFailure(portName, usbinfo);
			return;
		}
		UsbSerialportFSM fsm = legacy.Fsm;
		if (!fsm.SendWithAckGuard(60u, null, 0u, "FindDevice", delegate(uint cmd, object data, object data2)
		{
			HandleFindDeviceMatchedAck(acquired.Handle, cmd, data, data2);
		}, delegate
		{
			HandleFindDeviceTimeout(acquired.Handle, "FindDevice", usbinfo);
		}, delegate(string err)
		{
			HandleFindDeviceSendFail(acquired.Handle, usbinfo, err);
		}, 60u, GD.Inst.FindDevRetryInterval_Asce, GD.Inst.FindDevTimeout_Asce, 3000, delegate(uint retryCmd, uint retryCount)
		{
			WriteLog.WriteLogFileToUI($"重发第{retryCount}次, cmd=0x{retryCmd:X}, 串口名={usbinfo.PortName}", Color.Gray);
		}, delegate(uint timeoutCmd, uint retryCount, string msg)
		{
			WriteLog.WriteLogFileToUI(string.Format("超时, cmd=0x{0:X}, 已重发{1}次, 串口名={2}, msg={3}", new object[4] { timeoutCmd, retryCount, usbinfo.PortName, msg }), Color.DarkRed);
		}))
		{
			await ReleaseDiscoverySessionAsync(acquired.Handle, SerialPortReleaseReason.TransportUnavailable);
			ShowFindDeviceFailure(portName, usbinfo);
		}
		else
		{
			WriteLog.WriteLogFileToUI("发送设备信息请求帧(ACK守护已开启)，串口名=" + usbinfo.PortName, Color.Black);
		}
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
					InitTLPRowsCols(MAX_COL_COUNT, MAX_ROW_COUNT);
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
					InitTLPRowsCols(MAX_COL_COUNT, MAX_ROW_COUNT);
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
					InitTLPRowsCols(MAX_COL_COUNT, MAX_ROW_COUNT);
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

	private void HandleFindDeviceSendFail(SerialPortSessionHandle handle, UsbDevInfo usbinfo, string err)
	{
		try
		{
			string portName = SerialPortSessionText.NormalizePortName(usbinfo?.PortName);
			ReleaseDiscoverySessionAsync(handle, SerialPortReleaseReason.TransportUnavailable);
			if (((Control)this).IsHandleCreated)
			{
				((Control)this).BeginInvoke((Delegate)(Action)delegate
				{
					ShowFindDeviceFailure(portName, usbinfo);
				});
			}
			WriteLog.WriteLogFileToUI("发送失败，串口名=" + portName + ", err=" + err, Color.DarkRed);
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.HandleFindDeviceSendFail error,desc=" + ex.Message, Color.Red);
		}
	}

	private void HandleFindDeviceMatchedAck(SerialPortSessionHandle handle, uint cmd, object data, object data2)
	{
		if (cmd != 60 || !IsCurrentDiscoverySession(handle))
		{
			return;
		}
		UsbDevInfo usbDevInfo = data2 as UsbDevInfo;
		ResAscentInfo resAscentInfo = data as ResAscentInfo;
		if (usbDevInfo == null || resAscentInfo == null)
		{
			return;
		}
		string text = SerialPortSessionText.NormalizePortName(usbDevInfo.PortName);
		if (string.Equals(text, handle.PortName, StringComparison.OrdinalIgnoreCase))
		{
			_resAscentInfoDict[text] = resAscentInfo;
			_usbDevInfoDict[text] = usbDevInfo;
			_ctrlQue.Enqueue(resAscentInfo);
			if (IsGimbalVid(usbDevInfo.VID))
			{
				ReleaseDiscoverySessionAsync(handle, SerialPortReleaseReason.DiscoveryCompleted);
			}
		}
	}

	private void HandleFindDeviceTimeout(SerialPortSessionHandle handle, string state, UsbDevInfo usbinfo)
	{
		if (!IsCurrentDiscoverySession(handle))
		{
			return;
		}
		ReleaseDiscoverySessionAsync(handle, SerialPortReleaseReason.DiscoveryCompleted);
		try
		{
			if (((Control)this).IsHandleCreated)
			{
				((Control)this).BeginInvoke((Delegate)(Action)delegate
				{
					Fail(state, usbinfo, 1);
				});
			}
			else
			{
				Fail(state, usbinfo, 1);
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDevice超时处理异常,desc=" + ex.Message, Color.Red);
		}
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
			LangType currLang = (LangType)GD.Inst.CurrLang;
			LangType langType = currLang;
			if (langType - 1 <= LangType.zh_CN)
			{
				((Control)this).Invoke((Delegate)(Action)delegate
				{
					((Control)button1).Text = Lang.T("find_device.download_fw");
					((Control)button2).Text = Lang.T("find_device.faq");
				});
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
		((Control)this).Invoke((Delegate)(Action)delegate
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
			_nullDevCtrl.SetDeviceName(isshow: true, string.Empty, Lang.T("null_device.tip_connect_usb"));
		});
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
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (tableLayoutPanel1.RowCount < 2)
		{
			InitTLPRowsCols(MAX_COL_COUNT, MAX_ROW_COUNT);
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
		if (num == GD.Inst.GimbalVID)
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
			ResAscentInfo resAscentInfo = new ResAscentInfo
			{
				UsbInfo = usbinfo,
				DevName = "caddxsimgm",
				SN = "0000000000",
				Details = Lang.T("find_device.gm_v2_no_find_response")
			};
			_resAscentInfoDict[SerialPortSessionText.NormalizePortName(usbinfo.PortName)] = resAscentInfo;
			value.SetDeviceInfo(resAscentInfo);
			((Control)value).Show();
		}
		else
		{
			value.ShowUnsupportDev(usbinfo.PortName);
		}
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
			if (GD.Inst.FindDeviceFrm == this)
			{
				GD.Inst.FindDeviceFrm = null;
			}
			CloseFindDeviceFrm();
			foreach (SerialPortSessionHandle value in _discoverySessions.Values)
			{
				ReleaseDiscoverySessionAsync(value, SerialPortReleaseReason.DiscoveryCompleted);
			}
			_ctrlCts?.Cancel();
			_ctrlQue?.ClearAndClose();
			_discoverySessions.Clear();
			_resAscentInfoDict.Clear();
			_usbDevInfoDict.Clear();
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.Dispose error,desc=" + ex.Message, Color.Red);
		}
	}

	private bool IsCurrentDiscoverySession(SerialPortSessionHandle handle)
	{
		ISerialPortSessionTransport transport;
		return handle != null && handle.Owner == SerialPortOwner.DeviceDiscovery && GD.Inst.SerialPortSessions.TryGetTransport(handle, out transport);
	}

	private static bool IsGimbalVid(string vid)
	{
		int result;
		return int.TryParse(vid, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result) && result == GD.Inst.GimbalVID;
	}

	private async Task<bool> ReleaseDiscoverySessionAsync(SerialPortSessionHandle handle, SerialPortReleaseReason reason)
	{
		if (handle == null)
		{
			return false;
		}
		_discoverySessions.TryRemove(handle.PortName, out var _);
		return (await GD.Inst.SerialPortSessions.ReleaseAsync(handle, reason)).Succeeded;
	}

	public async Task<bool> ReleaseDiscoverySessionByPortAsync(string portName)
	{
		try
		{
			string normalized = SerialPortSessionText.NormalizePortName(portName);
			if (_discoverySessions.TryGetValue(normalized, out var handle))
			{
				return await ReleaseDiscoverySessionAsync(handle, SerialPortReleaseReason.CommandCompleted);
			}
			return false;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			WriteLog.WriteLogFileToUI("释放发现串口会话异常，串口名=" + portName + ", desc=" + ex2.Message, Color.Red);
			return false;
		}
	}

	public async Task<bool> RestoreDiscoverySessionByPortAsync(string portName)
	{
		if (((Control)this).Disposing || ((Control)this).IsDisposed)
		{
			return false;
		}
		string normalized = SerialPortSessionText.NormalizePortName(portName);
		if (_discoverySessions.TryGetValue(normalized, out var existing) && IsCurrentDiscoverySession(existing))
		{
			return true;
		}
		_discoverySessions.TryRemove(normalized, out var _);
		try
		{
			UsbDevInfo device = new UsbSerialMonitorDeviceEnumerator().GetCurrentDevices().FirstOrDefault((UsbDevInfo item) => string.Equals(SerialPortSessionText.NormalizePortName(item.PortName), normalized, StringComparison.OrdinalIgnoreCase));
			if (device == null)
			{
				WriteLog.WriteLogFileToUI("恢复发现会话失败，串口名=" + portName + "，未找到当前设备", Color.DarkOrange);
				return false;
			}
			SerialPortAcquireResult acquired = await GD.Inst.SerialPortSessions.AcquireAsync(device, SerialPortOwner.DeviceDiscovery, new SerialPortSessionOpenOptions
			{
				TransportKind = SerialPortTransportKind.LegacyFsm,
				BaudRate = GetDiscoveryBaudRate(device)
			}, CancellationToken.None);
			if (!acquired.Succeeded)
			{
				WriteLog.WriteLogFileToUI(string.Format("恢复发现会话失败，串口名={0}, reason={1}, owner={2}, state={3}", new object[4] { device.PortName, acquired.FailureReason, acquired.CurrentOwner, acquired.CurrentState }), Color.DarkOrange);
				return false;
			}
			if (!GD.Inst.SerialPortSessions.TryGetTransport(acquired.Handle, out var transport) || !(transport is LegacyFsmSessionTransport { Fsm: not null }) || !_discoverySessions.TryAdd(normalized, acquired.Handle))
			{
				await GD.Inst.SerialPortSessions.ReleaseAsync(acquired.Handle, SerialPortReleaseReason.TransportUnavailable);
				WriteLog.WriteLogFileToUI("恢复发现串口传输失败，串口名=" + device.PortName, Color.DarkOrange);
				return false;
			}
			WriteLog.WriteLogFileToUI("CLI 完成后已将 " + normalized + " 交还设备发现会话", Color.Gray);
			return true;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("恢复发现会话异常，串口名=" + portName + ", desc=" + ex.Message, Color.Red);
			return false;
		}
	}

	private static int GetDiscoveryBaudRate(UsbDevInfo device)
	{
		if (device != null && int.TryParse(device.VID, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result) && result == 6790)
		{
			return 460800;
		}
		return 115200;
	}

	private void ShowFindDeviceFailure(string portName, UsbDevInfo usbinfo)
	{
		if (_devInfoCardDict.TryGetValue(portName, out var value))
		{
			value.ShowUnRecviceDev(portName);
			((Control)value).Show();
		}
		else if (usbinfo != null)
		{
			DisplayUnsupportCard(usbinfo);
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
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Expected O, but got Unknown
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Expected O, but got Unknown
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Expected O, but got Unknown
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Expected O, but got Unknown
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Expected O, but got Unknown
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Expected O, but got Unknown
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Expected O, but got Unknown
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
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
