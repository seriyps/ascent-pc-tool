using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;
using Newtonsoft.Json;

namespace Caddx_PCTool;

public class RCModeCtrl : UserControl, IDisposable
{
	private const ushort SLIDERMAX = 2100;

	private const ushort SLIDERMIN = 800;

	private SlotData _currSlotData = new SlotData();

	private string _currPortname = string.Empty;

	private string _filePath = string.Empty;

	private List<UsbDevInfo> _usbDevInfo;

	private CancellationTokenSource _ctsTimeout = new CancellationTokenSource();

	private IContainer components = null;

	private Button btn_refresh;

	private Button btn_connect;

	private Button btn_quit;

	private GridPanel grpan_Main;

	private GridPanel grpan_Btn;

	private GridPanel grpan_zoom;

	private GridPanel grpan_flyCtrl;

	private Label label1;

	private Select select4;

	private Label label4;

	private PageHeader pahead_Tips;

	private Button btn_closeTips;

	private Label lab_FWTips;

	private Button button2;

	private Button btn_RCUpload;

	private Input input1;

	private GridPanel gridPanel3;

	private Input inp_zoomMax;

	private Label lab_zoomMax;

	private Input inp_zoomMin;

	private Label lab_zoomMin;

	private Label_Input lab_Inp_zoom;

	private RangeSliderControl sliRan_zoom;

	private StackPanel stackPanel1;

	private GridPanel gridPanel1;

	private GridPanel gridPanel2;

	private Input inp_resetMax;

	private Label lab_resetMax;

	private Input inp_resetMin;

	private Label lab_resetMin;

	private Label_Input lab_Inp_reset;

	private GridPanel gridPanel6;

	private GridPanel gridPanel7;

	private Input inp_cam2Max;

	private Label lab_cam2Max;

	private Input inp_cam2Min;

	private Label lab_cam2Min;

	private Label_Input lab_Inp_cam2;

	private GridPanel gridPanel4;

	private GridPanel gridPanel5;

	private Input inp_cam1Max;

	private Label lab_cam1Max;

	private Input inp_cam1Min;

	private Label lab_cam1Min;

	private Label_Input lab_Inp_cam1;

	private StackPanel stackPanel2;

	private Label label2;

	private Switch switch1;

	private Label label12;

	private Select sel_portname;

	private Label label13;

	private GridPanel gridPanel8;

	private Label label14;

	private Button btn_HubUpload;

	private ProcessAndDesc processAndDesc1;

	private RangeSliderControl sliRan_cam2;

	private RangeSliderControl sliRan_cam1;

	private RangeSliderControl sliRan_reset;

	public event EventHandler<FrmEventArgs> OnRCModeCtrlEvnet;

	public RCModeCtrl()
	{
		InitializeComponent();
	}

	public RCModeCtrl(List<UsbDevInfo> info)
	{
		InitializeComponent();
		_usbDevInfo = info;
	}

	private void RCModeCtrl_Load(object sender, EventArgs e)
	{
		try
		{
			InitData();
			ResetCtrl(_currSlotData);
			sel_portname.Items.Clear();
			if (_usbDevInfo.Count == 0)
			{
				sel_portname.Items.Add((object)Lang.T("rcmode.no_com"));
				sel_portname.SelectedIndex = 0;
				string currPortname = (((Control)sel_portname).Text = Lang.T("rcmode.no_com"));
				_currPortname = currPortname;
			}
			else
			{
				for (int i = 0; i < _usbDevInfo.Count; i++)
				{
					sel_portname.Items.Add((object)_usbDevInfo[i].PortName);
				}
				sel_portname.SelectedIndex = 0;
				string currPortname = (((Control)sel_portname).Text = _usbDevInfo[0].PortName);
				_currPortname = currPortname;
			}
			ReloadFont();
			ReloadLang();
		}
		catch (Exception)
		{
		}
	}

	private void InitData()
	{
		if (File.Exists(ConstVal.RCMODE_JSON_Path))
		{
			string text = File.ReadAllText(ConstVal.RCMODE_JSON_Path);
			_currSlotData = JsonConvert.DeserializeObject<SlotData>(text);
			return;
		}
		_currSlotData = new SlotData
		{
			slot0 = 8,
			slot0_min = 1000,
			slot0_max = 1300,
			slot1 = 10,
			slot1_min = 1500,
			slot1_max = 2000
		};
	}

	private void ResetCtrl(SlotData data)
	{
		SlotData slotData = new SlotData(data);
		string labelText = Lang.T("rcmode.zoom_control");
		lab_Inp_zoom.SetLabelText(labelText);
		bool visible;
		if (slotData.slot0_min > 2100)
		{
			lab_Inp_zoom.Sel_Idx = 0;
			Label obj = lab_zoomMin;
			Label obj2 = lab_zoomMax;
			Input obj3 = inp_zoomMax;
			Input obj4 = inp_zoomMin;
			bool flag = (((Control)sliRan_zoom).Visible = false);
			bool flag3 = (((IControl)obj4).Visible = flag);
			bool flag5 = (((IControl)obj3).Visible = flag3);
			visible = (((Control)obj2).Visible = flag5);
			((Control)obj).Visible = visible;
			sliRan_zoom.LowerValue = 1200;
			sliRan_zoom.UpperValue = 1500;
			((Control)inp_zoomMin).Text = "1200";
			((Control)inp_zoomMax).Text = "1500";
		}
		else
		{
			lab_Inp_zoom.Sel_Idx = slotData.slot0;
			sliRan_zoom.LowerValue = slotData.slot0_min;
			sliRan_zoom.UpperValue = slotData.slot0_max;
			((Control)inp_zoomMin).Text = slotData.slot0_min.ToString();
			((Control)inp_zoomMax).Text = slotData.slot0_max.ToString();
		}
		labelText = Lang.T("rcmode.zoom_reset");
		lab_Inp_reset.SetLabelText(labelText);
		if (slotData.slot1_min > 2100)
		{
			lab_Inp_reset.Sel_Idx = 0;
			Label obj5 = lab_resetMin;
			Label obj6 = lab_resetMax;
			Input obj7 = inp_resetMax;
			Input obj8 = inp_resetMin;
			bool flag = (((Control)sliRan_reset).Visible = false);
			bool flag3 = (((IControl)obj8).Visible = flag);
			bool flag5 = (((IControl)obj7).Visible = flag3);
			visible = (((Control)obj6).Visible = flag5);
			((Control)obj5).Visible = visible;
			sliRan_reset.LowerValue = 1200;
			sliRan_reset.UpperValue = 1500;
			((Control)inp_resetMin).Text = "1200";
			((Control)inp_resetMax).Text = "1500";
		}
		else
		{
			lab_Inp_reset.Sel_Idx = slotData.slot1;
			sliRan_reset.LowerValue = slotData.slot1_min;
			sliRan_reset.UpperValue = slotData.slot1_max;
			((Control)inp_resetMin).Text = slotData.slot1_min.ToString();
			((Control)inp_resetMax).Text = slotData.slot1_max.ToString();
		}
		labelText = Lang.T("rcmode.cam_change");
		lab_Inp_cam1.SetLabelText(labelText);
		if (slotData.slot2_min > 2100)
		{
			lab_Inp_cam1.Sel_Idx = 0;
			Label obj9 = lab_cam1Min;
			Label obj10 = lab_cam1Max;
			Input obj11 = inp_cam1Max;
			Input obj12 = inp_cam1Min;
			bool flag = (((Control)sliRan_cam1).Visible = false);
			bool flag3 = (((IControl)obj12).Visible = flag);
			bool flag5 = (((IControl)obj11).Visible = flag3);
			visible = (((Control)obj10).Visible = flag5);
			((Control)obj9).Visible = visible;
			sliRan_cam1.LowerValue = 1200;
			sliRan_cam1.UpperValue = 1500;
			((Control)inp_cam1Min).Text = "1200";
			((Control)inp_cam1Max).Text = "1500";
		}
		else
		{
			lab_Inp_cam1.Sel_Idx = slotData.slot2;
			sliRan_cam1.LowerValue = slotData.slot2_min;
			sliRan_cam1.UpperValue = slotData.slot2_max;
			((Control)inp_cam1Min).Text = slotData.slot2_min.ToString();
			((Control)inp_cam1Max).Text = slotData.slot2_max.ToString();
		}
		labelText = Lang.T("rcmode.cam_reset");
		lab_Inp_cam2.SetLabelText(labelText);
		if (slotData.slot3_min > 2100)
		{
			lab_Inp_cam2.Sel_Idx = 0;
			Label obj13 = lab_cam2Min;
			Label obj14 = lab_cam2Max;
			Input obj15 = inp_cam2Max;
			Input obj16 = inp_cam2Min;
			bool flag = (((Control)sliRan_cam2).Visible = false);
			bool flag3 = (((IControl)obj16).Visible = flag);
			bool flag5 = (((IControl)obj15).Visible = flag3);
			visible = (((Control)obj14).Visible = flag5);
			((Control)obj13).Visible = visible;
			sliRan_cam2.LowerValue = 1200;
			sliRan_cam2.UpperValue = 1500;
			((Control)inp_cam2Min).Text = "1200";
			((Control)inp_cam2Max).Text = "1500";
		}
		else
		{
			lab_Inp_cam2.Sel_Idx = slotData.slot3;
			sliRan_cam2.LowerValue = slotData.slot3_min;
			sliRan_cam2.UpperValue = slotData.slot3_max;
			((Control)inp_cam2Min).Text = slotData.slot3_min.ToString();
			((Control)inp_cam2Max).Text = slotData.slot3_max.ToString();
		}
		Button obj17 = btn_RCUpload;
		visible = (((IControl)btn_HubUpload).Enabled = false);
		((IControl)obj17).Enabled = visible;
		processAndDesc1.HideInterCtrl();
		sliRan_zoom.OnRangeValChangedEvent += sliRan_zoomCtrl_OnRangeValChangedEvent;
		sliRan_reset.OnRangeValChangedEvent += sliRan_reset_OnRangeValChangedEvent;
		sliRan_cam1.OnRangeValChangedEvent += sliRan_cam1_OnRangeValChangedEvent;
		sliRan_cam2.OnRangeValChangedEvent += sliRan_cam2_OnRangeValChangedEvent;
	}

	private void btn_connect_Click(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Button val = (Button)sender;
			if (GD.Inst.UsbFSM?.SPobj == null || !GD.Inst.UsbFSM.IsComOpened)
			{
				if (GD.Inst.UsbFSM != null)
				{
					GD.Inst.UsbFSM.SerialConnectedStateChange -= UsbFSM_SerialConneStateChange;
					GD.Inst.UsbFSM.Dispose();
				}
				UsbDevInfo usbDevInfo = new UsbDevInfo();
				for (int i = 0; i < _usbDevInfo.Count; i++)
				{
					if (_currPortname == _usbDevInfo[i].PortName)
					{
						usbDevInfo = _usbDevInfo[i];
					}
				}
				if (usbDevInfo.PortName == null)
				{
					WriteLog.WriteLogFileToUI("获取usb信息失败", Color.DarkRed);
					return;
				}
				GD.Inst.UsbFSM = new UsbSerialportFSM(usbDevInfo);
				if (!GD.Inst.UsbFSM.Open(_currPortname))
				{
					CommModalFrm commModalFrm = new CommModalFrm();
					commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("rcmode.device_connection_failed"), Color.Red);
					((Form)commModalFrm).ShowDialog();
					return;
				}
				btn_connect.DefaultBack = Color.FromArgb(95, 95, 96);
				btn_connect.ForeColor = Color.FromArgb(255, 255, 255);
				Button obj = btn_RCUpload;
				bool enabled = (((IControl)btn_HubUpload).Enabled = true);
				((IControl)obj).Enabled = enabled;
				GD.Inst.UpgFSM = new UpgradeProcessFSM(GD.Inst.UsbFSM);
				GD.Inst.UpgFSM.OnUpgProcHappenEvent += UpgFSM_OnUpgProcHappenEvent;
				((Control)val).Text = Lang.T("rcmode.btn_disconnect");
			}
			else
			{
				GD.Inst.UsbFSM?.Close();
				((Control)val).Text = Lang.T("common.btn_connect");
				btn_connect.DefaultBack = Color.FromArgb(255, 233, 0);
				btn_connect.ForeColor = Color.FromArgb(35, 35, 35);
				Button obj2 = btn_RCUpload;
				bool enabled = (((IControl)btn_HubUpload).Enabled = false);
				((IControl)obj2).Enabled = enabled;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("打开串口失败:" + ex.Message, Color.Red);
		}
	}

	private void UsbFSM_SerialConneStateChange(bool b, string arg1, int arg2)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!b)
			{
				GD.Inst.UsbFSM?.Close();
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(Lang.T("common.title_error_warning"), Lang.T("rcmode.device_disconnected_check_usb"), Color.Red);
				((Form)commModalFrm).ShowDialog();
				btn_Quit_Click(null, null);
			}
		}
		catch (Exception)
		{
		}
	}

	private void UpgFSM_OnUpgProcHappenEvent(object sender, UpgProcHappenEventArgs e)
	{
		StopTimeout();
		switch (e.infoType)
		{
		case InfoType.info:
			if (e.Percent >= 1f)
			{
				GD.Inst.UsbFSM.SerialConnectedStateChange += UsbFSM_SerialConneStateChange;
				Button obj = btn_RCUpload;
				bool enabled = (((IControl)btn_HubUpload).Enabled = true);
				((IControl)obj).Enabled = enabled;
			}
			processAndDesc1.SetProcValAndText(e.Percent, e.Desc);
			break;
		}
	}

	private void btn_refresh_Click(object sender, EventArgs e)
	{
		ManualSearchDevices(out _usbDevInfo);
		sel_portname.Items.Clear();
		for (int i = 0; i < _usbDevInfo.Count; i++)
		{
			sel_portname.Items.Add((object)_usbDevInfo[i].PortName);
		}
		sel_portname.SelectedIndex = 0;
		string currPortname = (((Control)sel_portname).Text = _usbDevInfo[0].PortName);
		_currPortname = currPortname;
	}

	private bool ManualSearchDevices(out List<UsbDevInfo> usbinfo)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		usbinfo = new List<UsbDevInfo>();
		try
		{
			string[] portNames = SerialPort.GetPortNames();
			if (portNames.Length < 1)
			{
				return false;
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
								usbinfo.Add(usbDevInfo);
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
			}
			return usbinfo.Count > 0;
		}
		catch (Exception)
		{
			return false;
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

	private void sel_portname_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		if (((VEventArgs<int>)(object)e).Value >= 0)
		{
			Select val = (Select)((sender is Select) ? sender : null);
			if (val.SelectedValue != null)
			{
				_currPortname = val.SelectedValue.ToString();
			}
		}
	}

	private void btn_SaveUpload_Click(object sender, EventArgs e)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Invalid comparison between Unknown and I4
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title;
			string desc;
			if (GD.Inst.UsbFSM == null || GD.Inst.UsbFSM.SPobj == null || !GD.Inst.UsbFSM.IsComOpened)
			{
				title = Lang.T("common.title_prompt");
				desc = Lang.T("rcmode.connect_device_first");
				commModalFrm.SetAllTxt(title, desc, Color.Red);
				((Form)commModalFrm).ShowDialog();
				return;
			}
			if (lab_Inp_zoom.Sel_Idx == 0)
			{
				title = Lang.T("common.title_prompt");
				desc = Lang.T("rcmode.zoom_channel_required");
				commModalFrm.SetAllTxt(title, desc, Color.Red);
				((Form)commModalFrm).ShowDialog();
				return;
			}
			title = Lang.T("common.title_prompt");
			desc = Lang.T("rcmode.confirm_upload_rc_file");
			commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc);
			((Form)commModalFrm).ShowDialog();
			if ((int)commModalFrm.FrmResult == 1)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(ConstVal.RCMODE_JSON_Path));
				bool flag = ReadCurrSlotData(out _currSlotData);
				string contents = JsonConvert.SerializeObject((object)_currSlotData, (Formatting)1);
				File.WriteAllText(ConstVal.RCMODE_JSON_Path, contents);
				GD.Inst.UsbFSM.SerialConnectedStateChange -= UsbFSM_SerialConneStateChange;
				processAndDesc1.ShowInterCtrl();
				processAndDesc1.SetProcValAndText(0.05f, Lang.T("rcmode.uploading"));
				Button obj = btn_RCUpload;
				bool enabled = (((IControl)btn_HubUpload).Enabled = false);
				((IControl)obj).Enabled = enabled;
				_filePath = ConstVal.RCMODE_JSON_Path;
				GD.Inst.UpgFSM.SetFilePath_Json(ConstVal.RCMODE_JSON_Path);
				GD.Inst.UpgFSM.Send_FileStart_Json();
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("保存json失败:" + ex.Message, Color.Red);
		}
	}

	private void btn_hubUpload_Click(object sender, EventArgs e)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title;
			string desc;
			if (GD.Inst.UsbFSM == null || GD.Inst.UsbFSM.SPobj == null || !GD.Inst.UsbFSM.IsComOpened)
			{
				title = Lang.T("common.title_prompt");
				desc = Lang.T("rcmode.connect_device_first");
				commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
				((Form)commModalFrm).ShowDialog();
				return;
			}
			title = Lang.T("common.title_prompt");
			desc = Lang.T("rcmode.confirm_upload_hub_file");
			commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc);
			((Form)commModalFrm).ShowDialog();
			if ((int)commModalFrm.FrmResult == 1)
			{
				Button obj = btn_RCUpload;
				bool enabled = (((IControl)btn_HubUpload).Enabled = false);
				((IControl)obj).Enabled = enabled;
				GD.Inst.UpgFSM.SetFilePath_Json(ConstVal.HUB_JSON_Path);
				GD.Inst.UpgFSM.Send_FileStart_Json();
				processAndDesc1.ShowInterCtrl();
				processAndDesc1.SetProcValAndText(0.05f, Lang.T("rcmode.uploading"));
			}
		}
		catch (Exception)
		{
		}
	}

	private void btn_closeTips_Click(object sender, EventArgs e)
	{
		Button obj = button2;
		Label obj2 = lab_FWTips;
		bool flag = (((IControl)btn_closeTips).Visible = false);
		bool visible = (((Control)obj2).Visible = flag);
		((IControl)obj).Visible = visible;
		((Control)pahead_Tips).BackColor = Color.FromArgb(38, 41, 43);
	}

	private void inp_Min_Leave(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		try
		{
			Input val = (Input)sender;
			string name = ((Control)val).Name;
			int result = 0;
			if (int.TryParse(((Control)val).Text, out result))
			{
				switch (name)
				{
				case "inp_zoomMin":
					sliRan_zoom.LowerValue = result;
					((Control)val).Text = sliRan_zoom.LowerValue.ToString();
					break;
				case "inp_resetMin":
					sliRan_reset.LowerValue = result;
					((Control)val).Text = sliRan_reset.LowerValue.ToString();
					break;
				case "inp_cam1Min":
					sliRan_cam1.LowerValue = result;
					((Control)val).Text = sliRan_cam1.LowerValue.ToString();
					break;
				case "inp_cam2Min":
					sliRan_cam2.LowerValue = result;
					((Control)val).Text = sliRan_cam2.LowerValue.ToString();
					break;
				}
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("RCMode.InpMin error,desc=" + ex.Message, Color.DarkRed);
		}
	}

	private void inp_Max_Leave(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		try
		{
			Input val = (Input)sender;
			string name = ((Control)val).Name;
			int result = 0;
			if (int.TryParse(((Control)val).Text, out result))
			{
				switch (name)
				{
				case "inp_zoomMax":
					sliRan_zoom.UpperValue = result;
					((Control)val).Text = sliRan_zoom.UpperValue.ToString();
					break;
				case "inp_resetMax":
					sliRan_reset.UpperValue = result;
					((Control)val).Text = sliRan_reset.UpperValue.ToString();
					break;
				case "inp_cam1Max":
					sliRan_cam1.UpperValue = result;
					((Control)val).Text = sliRan_cam1.UpperValue.ToString();
					break;
				case "inp_cam2Max":
					sliRan_cam2.UpperValue = result;
					((Control)val).Text = sliRan_cam2.UpperValue.ToString();
					break;
				}
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("RCMode.InpMax error,desc=" + ex.Message, Color.DarkRed);
		}
	}

	private void lab_Inp_zoom_OnSelectedIndexChangedEvent(object sender, IntEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		try
		{
			Select val = (Select)sender;
			if (((VEventArgs<int>)(object)e).Value == 0)
			{
				Label obj = lab_zoomMin;
				Label obj2 = lab_zoomMax;
				Input obj3 = inp_zoomMin;
				Input obj4 = inp_zoomMax;
				bool flag = (((Control)sliRan_zoom).Visible = false);
				bool flag3 = (((IControl)obj4).Visible = flag);
				bool flag5 = (((IControl)obj3).Visible = flag3);
				bool visible = (((Control)obj2).Visible = flag5);
				((Control)obj).Visible = visible;
			}
			else if (!((Control)sliRan_zoom).Visible)
			{
				Label obj5 = lab_zoomMin;
				Label obj6 = lab_zoomMax;
				Input obj7 = inp_zoomMin;
				Input obj8 = inp_zoomMax;
				bool flag = (((Control)sliRan_zoom).Visible = true);
				bool flag3 = (((IControl)obj8).Visible = flag);
				bool flag5 = (((IControl)obj7).Visible = flag3);
				bool visible = (((Control)obj6).Visible = flag5);
				((Control)obj5).Visible = visible;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("lab_Inp_zoom_OnSelectedIndexChangedEvent error ,desc=" + ex.Message, Color.DarkRed);
		}
	}

	private void lab_Inp_reset_OnSelectedIndexChangedEvent(object sender, IntEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		try
		{
			Select val = (Select)sender;
			if (((VEventArgs<int>)(object)e).Value == 0)
			{
				Label obj = lab_resetMin;
				Label obj2 = lab_resetMax;
				Input obj3 = inp_resetMax;
				Input obj4 = inp_resetMin;
				bool flag = (((Control)sliRan_reset).Visible = false);
				bool flag3 = (((IControl)obj4).Visible = flag);
				bool flag5 = (((IControl)obj3).Visible = flag3);
				bool visible = (((Control)obj2).Visible = flag5);
				((Control)obj).Visible = visible;
			}
			else if (!((Control)sliRan_reset).Visible)
			{
				Label obj5 = lab_resetMin;
				Label obj6 = lab_resetMax;
				Input obj7 = inp_resetMax;
				Input obj8 = inp_resetMin;
				bool flag = (((Control)sliRan_reset).Visible = true);
				bool flag3 = (((IControl)obj8).Visible = flag);
				bool flag5 = (((IControl)obj7).Visible = flag3);
				bool visible = (((Control)obj6).Visible = flag5);
				((Control)obj5).Visible = visible;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("lab_Inp_reset_OnSelectedIndexChangedEvent error ,desc=" + ex.Message, Color.DarkRed);
		}
	}

	private void lab_Inp_cam1_OnSelectedIndexChangedEvent(object sender, IntEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		try
		{
			Select val = (Select)sender;
			if (((VEventArgs<int>)(object)e).Value == 0)
			{
				Label obj = lab_cam1Min;
				Label obj2 = lab_cam1Max;
				Input obj3 = inp_cam1Min;
				Input obj4 = inp_cam1Max;
				bool flag = (((Control)sliRan_cam1).Visible = false);
				bool flag3 = (((IControl)obj4).Visible = flag);
				bool flag5 = (((IControl)obj3).Visible = flag3);
				bool visible = (((Control)obj2).Visible = flag5);
				((Control)obj).Visible = visible;
			}
			else if (!((Control)sliRan_cam1).Visible)
			{
				Label obj5 = lab_cam1Min;
				Label obj6 = lab_cam1Max;
				Input obj7 = inp_cam1Min;
				Input obj8 = inp_cam1Max;
				bool flag = (((Control)sliRan_cam1).Visible = true);
				bool flag3 = (((IControl)obj8).Visible = flag);
				bool flag5 = (((IControl)obj7).Visible = flag3);
				bool visible = (((Control)obj6).Visible = flag5);
				((Control)obj5).Visible = visible;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("lab_Inp_cam1_OnSelectedIndexChangedEvent error ,desc=" + ex.Message, Color.DarkRed);
		}
	}

	private void lab_Inp_cam2_OnSelectedIndexChangedEvent(object sender, IntEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		try
		{
			Select val = (Select)sender;
			if (((VEventArgs<int>)(object)e).Value == 0)
			{
				Label obj = lab_cam2Min;
				Label obj2 = lab_cam2Max;
				Input obj3 = inp_cam2Min;
				Input obj4 = inp_cam2Max;
				bool flag = (((Control)sliRan_cam2).Visible = false);
				bool flag3 = (((IControl)obj4).Visible = flag);
				bool flag5 = (((IControl)obj3).Visible = flag3);
				bool visible = (((Control)obj2).Visible = flag5);
				((Control)obj).Visible = visible;
			}
			else if (!((Control)sliRan_cam2).Visible)
			{
				Label obj5 = lab_cam2Min;
				Label obj6 = lab_cam2Max;
				Input obj7 = inp_cam2Min;
				Input obj8 = inp_cam2Max;
				bool flag = (((Control)sliRan_cam2).Visible = true);
				bool flag3 = (((IControl)obj8).Visible = flag);
				bool flag5 = (((IControl)obj7).Visible = flag3);
				bool visible = (((Control)obj6).Visible = flag5);
				((Control)obj5).Visible = visible;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("lab_Inp_cam2_OnSelectedIndexChangedEvent error ,desc=" + ex.Message, Color.DarkRed);
		}
	}

	private void lab_Inp_reset_Load(object sender, EventArgs e)
	{
	}

	private void sliRan_zoomCtrl_OnRangeValChangedEvent(object sender, RangeValChangedEventArgs e)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)inp_zoomMin).Text = e.MinVal.ToString();
			((Control)inp_zoomMax).Text = e.MaxVal.ToString();
		});
	}

	private void sliRan_reset_OnRangeValChangedEvent(object sender, RangeValChangedEventArgs e)
	{
		((Control)inp_resetMin).Text = e.MinVal.ToString();
		((Control)inp_resetMax).Text = e.MaxVal.ToString();
	}

	private void sliRan_cam1_OnRangeValChangedEvent(object sender, RangeValChangedEventArgs e)
	{
		((Control)inp_cam1Min).Text = e.MinVal.ToString();
		((Control)inp_cam1Max).Text = e.MaxVal.ToString();
	}

	private void sliRan_cam2_OnRangeValChangedEvent(object sender, RangeValChangedEventArgs e)
	{
		((Control)inp_cam2Min).Text = e.MinVal.ToString();
		((Control)inp_cam2Max).Text = e.MaxVal.ToString();
	}

	private void inp_Min_EnterDown(object sender, KeyEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		try
		{
			if ((int)e.KeyCode != 13)
			{
				return;
			}
			Input val = (Input)sender;
			string name = ((Control)val).Name;
			int result = 0;
			if (int.TryParse(((Control)val).Text, out result))
			{
				switch (name)
				{
				case "inp_zoomMin":
					sliRan_zoom.LowerValue = result;
					((Control)val).Text = sliRan_zoom.LowerValue.ToString();
					break;
				case "inp_resetMin":
					sliRan_reset.LowerValue = result;
					((Control)val).Text = sliRan_reset.LowerValue.ToString();
					break;
				case "inp_cam1Min":
					sliRan_cam1.LowerValue = result;
					((Control)val).Text = sliRan_cam1.LowerValue.ToString();
					break;
				case "inp_cam2Min":
					sliRan_cam2.LowerValue = result;
					((Control)val).Text = sliRan_cam2.LowerValue.ToString();
					break;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void inp_Max_EnterDown(object sender, KeyEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		try
		{
			if ((int)e.KeyCode != 13)
			{
				return;
			}
			Input val = (Input)sender;
			string name = ((Control)val).Name;
			int result = 0;
			if (int.TryParse(((Control)val).Text, out result))
			{
				switch (name)
				{
				case "inp_zoomMax":
					sliRan_zoom.UpperValue = result;
					((Control)val).Text = sliRan_zoom.UpperValue.ToString();
					break;
				case "inp_resetMax":
					sliRan_reset.UpperValue = result;
					((Control)val).Text = sliRan_reset.UpperValue.ToString();
					break;
				case "inp_cam1Max":
					sliRan_cam1.UpperValue = result;
					((Control)val).Text = sliRan_cam1.UpperValue.ToString();
					break;
				case "inp_cam2Max":
					sliRan_cam2.UpperValue = result;
					((Control)val).Text = sliRan_cam2.UpperValue.ToString();
					break;
				}
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("RCMode.InpMax enter error,desc=" + ex.Message, Color.DarkRed);
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
					CommModalFrm comm = new CommModalFrm();
					comm.SetAllTxt(Lang.T("common.title_error"), Lang.T("rcmode.device_response_timeout"), Color.Red);
					((Form)comm).ShowDialog();
				}
			}
			catch
			{
			}
		});
	}

	private void processAndDesc1_Load(object sender, EventArgs e)
	{
	}

	private void pageHeader1_Click(object sender, EventArgs e)
	{
	}

	private void StopTimeout()
	{
		_ctsTimeout?.Cancel();
	}

	private void btn_LoadJson_Click(object sender, EventArgs e)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			GD.Inst.UsbFSM.Reopen_Json(150000);
		}
		catch (Exception ex)
		{
			MessageBox.Show(Lang.T("rcmode.param_read_failed", ex.Message));
		}
	}

	private void btn_Quit_Click(object sender, EventArgs e)
	{
		try
		{
			StopTimeout();
			if (GD.Inst.UsbFSM != null)
			{
				GD.Inst.UsbFSM.SerialConnectedStateChange -= UsbFSM_SerialConneStateChange;
			}
			if (GD.Inst.UpgFSM != null)
			{
				GD.Inst.UpgFSM.OnUpgProcHappenEvent -= UpgFSM_OnUpgProcHappenEvent;
			}
			GD.Inst.UsbFSM?.Dispose();
			GD.Inst.UpgFSM?.Dispose();
			FrmEventArgs fe = new FrmEventArgs
			{
				Desc = "ManualCloseDevice",
				InfoType = InfoType.ctrlSign
			};
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				OnRCModeCtrlEvnet?.Invoke(null, fe);
				((Control)this).Hide();
			});
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("退出RCMode报错,desc=" + ex.Message, Color.Red);
		}
	}

	private bool ReadCurrSlotData(out SlotData data)
	{
		data = new SlotData();
		try
		{
			data.slot0 = lab_Inp_zoom.Sel_Idx;
			data.slot0_min = sliRan_zoom.LowerValue;
			data.slot0_max = sliRan_zoom.UpperValue;
			if (lab_Inp_reset.Sel_Idx == 0)
			{
				data.slot1 = lab_Inp_zoom.Sel_Idx;
				data.slot1_min = 9990;
				data.slot1_max = 9999;
			}
			else
			{
				data.slot1 = lab_Inp_reset.Sel_Idx;
				data.slot1_min = sliRan_reset.LowerValue;
				data.slot1_max = sliRan_reset.UpperValue;
			}
			if (lab_Inp_cam1.Sel_Idx == 0)
			{
				data.slot2 = lab_Inp_zoom.Sel_Idx;
				data.slot2_min = 9990;
				data.slot2_max = 9999;
			}
			else
			{
				data.slot2 = lab_Inp_cam1.Sel_Idx;
				data.slot2_min = sliRan_cam1.LowerValue;
				data.slot2_max = sliRan_cam1.UpperValue;
			}
			if (lab_Inp_cam2.Sel_Idx == 0)
			{
				data.slot3 = lab_Inp_zoom.Sel_Idx;
				data.slot3_min = 9990;
				data.slot3_max = 9999;
			}
			else
			{
				data.slot3 = lab_Inp_cam2.Sel_Idx;
				data.slot3_min = sliRan_cam2.LowerValue;
				data.slot3_max = sliRan_cam2.UpperValue;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void inp_VerifyChar(object sender, InputVerifyCharEventArgs e)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
		string numberDecimalSeparator = numberFormat.NumberDecimalSeparator;
		string numberGroupSeparator = numberFormat.NumberGroupSeparator;
		string negativeSign = numberFormat.NegativeSign;
		string text = e.Char.ToString();
		int num = ((Control)(Input)sender).Text.IndexOf('.');
		if (char.IsDigit(e.Char))
		{
			e.Result = true;
			return;
		}
		if (e.Char == '\b')
		{
			e.Result = true;
			return;
		}
		e.Result = false;
		CommModalFrm commModalFrm = new CommModalFrm();
		commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("gimbal.only_numbers_allowed"), isshowBtnOK: true, isshowBtnCan: false);
		((Form)commModalFrm).ShowDialog();
	}

	private void inp_VerifyChar_NegativeDigit(object sender, InputVerifyCharEventArgs e)
	{
	}

	public void ReloadFont()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 10f);
		Label obj = lab_zoomMin;
		Label obj2 = lab_zoomMax;
		Label obj3 = lab_resetMax;
		Label obj4 = lab_resetMin;
		Label obj5 = lab_cam1Max;
		Label obj6 = lab_cam1Min;
		Label obj7 = lab_cam2Max;
		Label obj8 = lab_cam2Min;
		Input obj9 = inp_zoomMin;
		Input obj10 = inp_zoomMax;
		Input obj11 = inp_resetMin;
		Input obj12 = inp_resetMax;
		Input obj13 = inp_cam1Min;
		Input obj14 = inp_cam1Max;
		Input obj15 = inp_cam2Max;
		Font val2 = (((Control)inp_cam2Min).Font = val);
		Font val4 = (((Control)obj15).Font = val2);
		Font val6 = (((Control)obj14).Font = val4);
		Font val8 = (((Control)obj13).Font = val6);
		Font val10 = (((Control)obj12).Font = val8);
		Font val12 = (((Control)obj11).Font = val10);
		Font val14 = (((Control)obj10).Font = val12);
		Font val16 = (((Control)obj9).Font = val14);
		Font val18 = (((Control)obj8).Font = val16);
		Font val20 = (((Control)obj7).Font = val18);
		Font val22 = (((Control)obj6).Font = val20);
		Font val24 = (((Control)obj5).Font = val22);
		Font val26 = (((Control)obj4).Font = val24);
		Font val28 = (((Control)obj3).Font = val26);
		Font font = (((Control)obj2).Font = val28);
		((Control)obj).Font = font;
		Font val31 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
		Button obj16 = btn_RCUpload;
		Button obj17 = btn_RCUpload;
		val28 = (((Control)sel_portname).Font = val31);
		font = (((Control)obj17).Font = val28);
		((Control)obj16).Font = font;
	}

	public void ReloadLang()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			bool flag = GD.Inst.UsbFSM?.SPobj != null && GD.Inst.UsbFSM.IsComOpened;
			lab_Inp_zoom.SetLabelText(Lang.T("rcmode.zoom_control"));
			lab_Inp_reset.SetLabelText(Lang.T("rcmode.zoom_reset"));
			lab_Inp_cam1.SetLabelText(Lang.T("rcmode.cam_change"));
			lab_Inp_cam2.SetLabelText(Lang.T("rcmode.cam_reset"));
			((Control)btn_connect).Text = (flag ? Lang.T("rcmode.btn_disconnect") : Lang.T("common.btn_connect"));
			((Control)btn_refresh).Text = Lang.T("rcmode.refresh_device");
			((Control)btn_RCUpload).Text = Lang.T("rcmode.upload_rc_file");
			((Control)btn_quit).Text = Lang.T("rcmode.quit");
			((Control)btn_HubUpload).Text = Lang.T("rcmode.upload_hub_file");
			((Control)lab_FWTips).Text = Lang.T("rcmode.fw_tips");
		});
	}

	public void Dispose()
	{
		GD.Inst.UsbFSM?.Dispose();
		GD.Inst.UpgFSM?.Dispose();
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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Expected O, but got Unknown
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Expected O, but got Unknown
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected O, but got Unknown
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Expected O, but got Unknown
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Expected O, but got Unknown
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Expected O, but got Unknown
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Expected O, but got Unknown
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Expected O, but got Unknown
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Expected O, but got Unknown
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Expected O, but got Unknown
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c26: Expected O, but got Unknown
		//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd1: Expected O, but got Unknown
		//IL_0e6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed3: Expected O, but got Unknown
		//IL_0ee1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eeb: Expected O, but got Unknown
		//IL_0f3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1049: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10af: Expected O, but got Unknown
		//IL_10bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c7: Expected O, but got Unknown
		//IL_1118: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_135e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1471: Unknown result type (might be due to invalid IL or missing references)
		//IL_147b: Expected O, but got Unknown
		//IL_151c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1526: Expected O, but got Unknown
		//IL_16c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_171e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1728: Expected O, but got Unknown
		//IL_1736: Unknown result type (might be due to invalid IL or missing references)
		//IL_1740: Expected O, but got Unknown
		//IL_1792: Unknown result type (might be due to invalid IL or missing references)
		//IL_189e: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1904: Expected O, but got Unknown
		//IL_1912: Unknown result type (might be due to invalid IL or missing references)
		//IL_191c: Expected O, but got Unknown
		//IL_196d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ccd: Expected O, but got Unknown
		//IL_1d6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d78: Expected O, but got Unknown
		//IL_1f14: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f70: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f7a: Expected O, but got Unknown
		//IL_1f88: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f92: Expected O, but got Unknown
		//IL_1fe4: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_214c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2156: Expected O, but got Unknown
		//IL_2164: Unknown result type (might be due to invalid IL or missing references)
		//IL_216e: Expected O, but got Unknown
		//IL_21bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_2260: Unknown result type (might be due to invalid IL or missing references)
		//IL_2414: Unknown result type (might be due to invalid IL or missing references)
		//IL_2527: Unknown result type (might be due to invalid IL or missing references)
		//IL_2531: Expected O, but got Unknown
		//IL_25d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_25dc: Expected O, but got Unknown
		//IL_2790: Unknown result type (might be due to invalid IL or missing references)
		//IL_27ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_27f6: Expected O, but got Unknown
		//IL_2804: Unknown result type (might be due to invalid IL or missing references)
		//IL_280e: Expected O, but got Unknown
		//IL_2860: Unknown result type (might be due to invalid IL or missing references)
		//IL_294e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2958: Expected O, but got Unknown
		//IL_298e: Unknown result type (might be due to invalid IL or missing references)
		//IL_29ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_29f4: Expected O, but got Unknown
		//IL_2a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a0c: Expected O, but got Unknown
		//IL_2a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a53: Expected O, but got Unknown
		//IL_2a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c39: Unknown result type (might be due to invalid IL or missing references)
		//IL_2cf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2da9: Expected O, but got Unknown
		//IL_2dd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2eb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_30f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_30fb: Expected O, but got Unknown
		//IL_312e: Unknown result type (might be due to invalid IL or missing references)
		//IL_31b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_323a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3244: Expected O, but got Unknown
		//IL_3293: Unknown result type (might be due to invalid IL or missing references)
		//IL_33c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_33d3: Expected O, but got Unknown
		//IL_3431: Unknown result type (might be due to invalid IL or missing references)
		//IL_34e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_34ee: Expected O, but got Unknown
		//IL_353d: Unknown result type (might be due to invalid IL or missing references)
		//IL_35d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_35da: Expected O, but got Unknown
		//IL_3629: Unknown result type (might be due to invalid IL or missing references)
		//IL_3796: Unknown result type (might be due to invalid IL or missing references)
		//IL_3837: Unknown result type (might be due to invalid IL or missing references)
		//IL_38cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_38d6: Expected O, but got Unknown
		//IL_3924: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3af2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3afc: Expected O, but got Unknown
		//IL_3b4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ba9: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bb3: Expected O, but got Unknown
		//IL_3c37: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d16: Unknown result type (might be due to invalid IL or missing references)
		btn_connect = new Button();
		grpan_Main = new GridPanel();
		gridPanel8 = new GridPanel();
		processAndDesc1 = new ProcessAndDesc();
		label14 = new Label();
		btn_HubUpload = new Button();
		stackPanel1 = new StackPanel();
		gridPanel6 = new GridPanel();
		sliRan_cam2 = new RangeSliderControl();
		gridPanel7 = new GridPanel();
		inp_cam2Max = new Input();
		lab_cam2Max = new Label();
		inp_cam2Min = new Input();
		lab_cam2Min = new Label();
		lab_Inp_cam2 = new Label_Input();
		gridPanel4 = new GridPanel();
		sliRan_cam1 = new RangeSliderControl();
		gridPanel5 = new GridPanel();
		inp_cam1Max = new Input();
		lab_cam1Max = new Label();
		inp_cam1Min = new Input();
		lab_cam1Min = new Label();
		lab_Inp_cam1 = new Label_Input();
		gridPanel1 = new GridPanel();
		sliRan_reset = new RangeSliderControl();
		gridPanel2 = new GridPanel();
		inp_resetMax = new Input();
		lab_resetMax = new Label();
		inp_resetMin = new Input();
		lab_resetMin = new Label();
		lab_Inp_reset = new Label_Input();
		grpan_zoom = new GridPanel();
		sliRan_zoom = new RangeSliderControl();
		gridPanel3 = new GridPanel();
		inp_zoomMax = new Input();
		lab_zoomMax = new Label();
		inp_zoomMin = new Input();
		lab_zoomMin = new Label();
		lab_Inp_zoom = new Label_Input();
		pahead_Tips = new PageHeader();
		btn_closeTips = new Button();
		lab_FWTips = new Label();
		button2 = new Button();
		grpan_flyCtrl = new GridPanel();
		stackPanel2 = new StackPanel();
		label2 = new Label();
		switch1 = new Switch();
		label12 = new Label();
		input1 = new Input();
		select4 = new Select();
		label4 = new Label();
		label1 = new Label();
		grpan_Btn = new GridPanel();
		label13 = new Label();
		btn_RCUpload = new Button();
		btn_quit = new Button();
		sel_portname = new Select();
		btn_refresh = new Button();
		((Control)grpan_Main).SuspendLayout();
		((Control)gridPanel8).SuspendLayout();
		((Control)stackPanel1).SuspendLayout();
		((Control)gridPanel6).SuspendLayout();
		((Control)gridPanel7).SuspendLayout();
		((Control)gridPanel4).SuspendLayout();
		((Control)gridPanel5).SuspendLayout();
		((Control)gridPanel1).SuspendLayout();
		((Control)gridPanel2).SuspendLayout();
		((Control)grpan_zoom).SuspendLayout();
		((Control)gridPanel3).SuspendLayout();
		((Control)pahead_Tips).SuspendLayout();
		((Control)grpan_flyCtrl).SuspendLayout();
		((Control)stackPanel2).SuspendLayout();
		((Control)grpan_Btn).SuspendLayout();
		((Control)this).SuspendLayout();
		btn_connect.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_connect).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_connect.ForeColor = Color.FromArgb(35, 35, 35);
		grpan_Btn.SetIndex((Control)(object)btn_connect, 3);
		((Control)btn_connect).Location = new Point(213, 5);
		((Control)btn_connect).Margin = new Padding(5);
		((Control)btn_connect).Name = "btn_connect";
		((Control)btn_connect).Size = new Size(156, 40);
		((Control)btn_connect).TabIndex = 30;
		((Control)btn_connect).Text = "disconnect";
		btn_connect.WaveSize = 0;
		((Control)btn_connect).Click += btn_connect_Click;
		((ContainerPanel)grpan_Main).Back = Color.Transparent;
		((Control)grpan_Main).BackColor = Color.Transparent;
		((Control)grpan_Main).Controls.Add((Control)(object)gridPanel8);
		((Control)grpan_Main).Controls.Add((Control)(object)stackPanel1);
		((Control)grpan_Main).Controls.Add((Control)(object)pahead_Tips);
		((Control)grpan_Main).Controls.Add((Control)(object)grpan_flyCtrl);
		((Control)grpan_Main).Controls.Add((Control)(object)grpan_Btn);
		((Control)grpan_Main).Dock = (DockStyle)5;
		((Control)grpan_Main).Location = new Point(5, 15);
		((Control)grpan_Main).Margin = new Padding(0);
		((Control)grpan_Main).Name = "grpan_Main";
		((Control)grpan_Main).Size = new Size(830, 970);
		grpan_Main.Span = "100%;100%;100%;100%;100%;\r\n-50 50 90 90% 10%";
		((Control)grpan_Main).TabIndex = 11;
		((Control)grpan_Main).TabStop = false;
		((Control)grpan_Main).Text = "gridPanel1";
		((ContainerPanel)gridPanel8).BorderColor = Color.FromArgb(235, 237, 240);
		((Control)gridPanel8).Controls.Add((Control)(object)processAndDesc1);
		((Control)gridPanel8).Controls.Add((Control)(object)label14);
		((Control)gridPanel8).Controls.Add((Control)(object)btn_HubUpload);
		grpan_Main.SetIndex((Control)(object)gridPanel8, 1);
		((Control)gridPanel8).Location = new Point(0, 50);
		((Control)gridPanel8).Margin = new Padding(0);
		((Control)gridPanel8).Name = "gridPanel8";
		((Control)gridPanel8).Size = new Size(830, 50);
		gridPanel8.Span = "50% 20% 20% 10%";
		((Control)gridPanel8).TabIndex = 60;
		((Control)gridPanel8).Text = "gridPanel8";
		((Control)processAndDesc1).BackColor = Color.FromArgb(38, 41, 43);
		gridPanel8.SetIndex((Control)(object)processAndDesc1, 1);
		((Control)processAndDesc1).Location = new Point(5, 5);
		((Control)processAndDesc1).Margin = new Padding(5);
		((Control)processAndDesc1).Name = "processAndDesc1";
		processAndDesc1.ProcessRatio = 0.7f;
		((Control)processAndDesc1).Size = new Size(405, 40);
		((Control)processAndDesc1).TabIndex = 40;
		((Control)label14).AutoSize = true;
		gridPanel8.SetIndex((Control)(object)label14, 2);
		((Control)label14).Location = new Point(420, 5);
		((Control)label14).Margin = new Padding(5);
		((Control)label14).Name = "label14";
		((Control)label14).Size = new Size(156, 40);
		((Control)label14).TabIndex = 39;
		label14.TextAlign = (ContentAlignment)64;
		btn_HubUpload.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_HubUpload).Dock = (DockStyle)4;
		((Control)btn_HubUpload).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_HubUpload.ForeColor = Color.FromArgb(35, 35, 35);
		gridPanel8.SetIndex((Control)(object)btn_HubUpload, 3);
		((Control)btn_HubUpload).Location = new Point(586, 5);
		((Control)btn_HubUpload).Margin = new Padding(5);
		((Control)btn_HubUpload).Name = "btn_HubUpload";
		((Control)btn_HubUpload).Size = new Size(156, 40);
		((Control)btn_HubUpload).TabIndex = 38;
		((Control)btn_HubUpload).Text = "Upload Hub file";
		btn_HubUpload.WaveSize = 0;
		((Control)btn_HubUpload).Click += btn_hubUpload_Click;
		stackPanel1.AutoScroll = true;
		((ContainerPanel)stackPanel1).Back = Color.FromArgb(38, 41, 43);
		((Control)stackPanel1).BackColor = Color.Transparent;
		stackPanel1.Controls.Add((Control)(object)gridPanel6);
		stackPanel1.Controls.Add((Control)(object)gridPanel4);
		stackPanel1.Controls.Add((Control)(object)gridPanel1);
		stackPanel1.Controls.Add((Control)(object)grpan_zoom);
		grpan_Main.SetIndex((Control)(object)stackPanel1, 3);
		stackPanel1.ItemSize = "85";
		((Control)stackPanel1).Location = new Point(3, 193);
		((Control)stackPanel1).Name = "stackPanel1";
		((Control)stackPanel1).Size = new Size(824, 696);
		((Control)stackPanel1).TabIndex = 12;
		((Control)stackPanel1).Text = "stackPanel1";
		stackPanel1.Vertical = true;
		((ContainerPanel)gridPanel6).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel6).BorderWidth = 2f;
		((Control)gridPanel6).Controls.Add((Control)(object)sliRan_cam2);
		((Control)gridPanel6).Controls.Add((Control)(object)gridPanel7);
		((Control)gridPanel6).Controls.Add((Control)(object)lab_Inp_cam2);
		((Control)gridPanel6).Location = new Point(3, 276);
		((Control)gridPanel6).Name = "gridPanel6";
		((Control)gridPanel6).Size = new Size(818, 85);
		gridPanel6.Span = "20% 25% 53% 2%";
		((Control)gridPanel6).TabIndex = 16;
		((Control)gridPanel6).TabStop = false;
		((Control)gridPanel6).Text = "gridPanel6";
		((Control)sliRan_cam2).BackColor = Color.Transparent;
		((Control)sliRan_cam2).Location = new Point(373, 5);
		sliRan_cam2.LowerValue = 800;
		((Control)sliRan_cam2).Margin = new Padding(5, 5, 0, 5);
		sliRan_cam2.Maximum = 2100;
		sliRan_cam2.Minimum = 800;
		sliRan_cam2.MinimumRange = 1;
		((Control)sliRan_cam2).MinimumSize = new Size(80, 60);
		((Control)sliRan_cam2).Name = "sliRan_cam2";
		sliRan_cam2.SelectedTrackColor = Color.FromArgb(255, 233, 0);
		((Control)sliRan_cam2).Size = new Size(429, 75);
		sliRan_cam2.Step = 50;
		((Control)sliRan_cam2).TabIndex = 7;
		((Control)sliRan_cam2).TabStop = false;
		sliRan_cam2.TextInterval = 8;
		sliRan_cam2.ThumbColor = Color.FromArgb(255, 233, 0);
		sliRan_cam2.ThumbSize = 8;
		sliRan_cam2.TickColor = Color.Gray;
		sliRan_cam2.TickFont = new Font("Segoe UI", 8f);
		sliRan_cam2.TickFrequency = 50;
		sliRan_cam2.TickTextColor = Color.FromArgb(164, 164, 165);
		sliRan_cam2.TrackColor = Color.FromArgb(250, 250, 250);
		sliRan_cam2.TrackHeightCustom = 6;
		sliRan_cam2.UpperValue = 1500;
		sliRan_cam2.ValueBubbleColor = Color.FromArgb(95, 95, 96);
		sliRan_cam2.ValueBubbleVisible = true;
		sliRan_cam2.ValueFont = new Font("Segoe UI", 9f);
		((Control)gridPanel7).Controls.Add((Control)(object)inp_cam2Max);
		((Control)gridPanel7).Controls.Add((Control)(object)lab_cam2Max);
		((Control)gridPanel7).Controls.Add((Control)(object)inp_cam2Min);
		((Control)gridPanel7).Controls.Add((Control)(object)lab_cam2Min);
		gridPanel6.SetIndex((Control)(object)gridPanel7, 2);
		((Control)gridPanel7).Location = new Point(167, 3);
		((Control)gridPanel7).Name = "gridPanel7";
		((Control)gridPanel7).RightToLeft = (RightToLeft)0;
		((Control)gridPanel7).Size = new Size(198, 79);
		gridPanel7.Span = "45% 55%;45% 55%";
		((Control)gridPanel7).TabIndex = 3;
		((Control)gridPanel7).Text = "gridPanel7";
		inp_cam2Max.BackColor = Color.FromArgb(30, 34, 37);
		inp_cam2Max.BorderActive = Color.FromArgb(255, 233, 0);
		inp_cam2Max.BorderColor = Color.FromArgb(66, 69, 71);
		inp_cam2Max.BorderHover = Color.FromArgb(255, 233, 0);
		inp_cam2Max.ForeColor = Color.White;
		((Control)inp_cam2Max).Location = new Point(92, 43);
		((Control)inp_cam2Max).Margin = new Padding(3, 3, 15, 3);
		((Control)inp_cam2Max).Name = "inp_cam2Max";
		((Control)inp_cam2Max).Size = new Size(91, 34);
		((Control)inp_cam2Max).TabIndex = 3;
		((Control)inp_cam2Max).Text = "1000";
		inp_cam2Max.VerifyChar += new InputVerifyCharEventHandler(inp_VerifyChar);
		((Control)inp_cam2Max).KeyDown += new KeyEventHandler(inp_Max_EnterDown);
		((Control)inp_cam2Max).Leave += inp_Max_Leave;
		((Control)lab_cam2Max).AutoSize = true;
		((Control)lab_cam2Max).ForeColor = Color.White;
		((Control)lab_cam2Max).Location = new Point(5, 45);
		((Control)lab_cam2Max).Margin = new Padding(5);
		((Control)lab_cam2Max).Name = "lab_cam2Max";
		((Control)lab_cam2Max).Size = new Size(79, 30);
		((Control)lab_cam2Max).TabIndex = 2;
		((Control)lab_cam2Max).Text = "Max :";
		lab_cam2Max.TextAlign = (ContentAlignment)64;
		inp_cam2Min.BackColor = Color.FromArgb(30, 34, 37);
		inp_cam2Min.BorderActive = Color.FromArgb(255, 233, 0);
		inp_cam2Min.BorderColor = Color.FromArgb(66, 69, 71);
		inp_cam2Min.BorderHover = Color.FromArgb(255, 233, 0);
		inp_cam2Min.ForeColor = Color.White;
		((Control)inp_cam2Min).Location = new Point(92, 3);
		((Control)inp_cam2Min).Margin = new Padding(3, 3, 15, 3);
		((Control)inp_cam2Min).Name = "inp_cam2Min";
		((Control)inp_cam2Min).Size = new Size(91, 34);
		((Control)inp_cam2Min).TabIndex = 1;
		((Control)inp_cam2Min).Text = "1000";
		inp_cam2Min.VerifyChar += new InputVerifyCharEventHandler(inp_VerifyChar);
		((Control)inp_cam2Min).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_cam2Min).Leave += inp_Min_Leave;
		((Control)lab_cam2Min).AutoSize = true;
		((Control)lab_cam2Min).ForeColor = Color.White;
		((Control)lab_cam2Min).Location = new Point(5, 5);
		((Control)lab_cam2Min).Margin = new Padding(5);
		((Control)lab_cam2Min).Name = "lab_cam2Min";
		((Control)lab_cam2Min).Size = new Size(79, 30);
		((Control)lab_cam2Min).TabIndex = 0;
		((Control)lab_cam2Min).Text = "Min :";
		lab_cam2Min.TextAlign = (ContentAlignment)64;
		((Control)lab_Inp_cam2).BackColor = Color.FromArgb(57, 59, 48);
		gridPanel6.SetIndex((Control)(object)lab_Inp_cam2, 1);
		((Control)lab_Inp_cam2).Location = new Point(5, 5);
		((Control)lab_Inp_cam2).Margin = new Padding(5);
		((Control)lab_Inp_cam2).Name = "lab_Inp_cam2";
		lab_Inp_cam2.Sel_Idx = -1;
		((Control)lab_Inp_cam2).Size = new Size(154, 75);
		((Control)lab_Inp_cam2).TabIndex = 2;
		((Control)lab_Inp_cam2).TabStop = false;
		lab_Inp_cam2.OnSelectedIndexChangedEvent += lab_Inp_cam2_OnSelectedIndexChangedEvent;
		((ContainerPanel)gridPanel4).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel4).BorderWidth = 2f;
		((Control)gridPanel4).Controls.Add((Control)(object)sliRan_cam1);
		((Control)gridPanel4).Controls.Add((Control)(object)gridPanel5);
		((Control)gridPanel4).Controls.Add((Control)(object)lab_Inp_cam1);
		((Control)gridPanel4).Location = new Point(3, 185);
		((Control)gridPanel4).Name = "gridPanel4";
		((Control)gridPanel4).Size = new Size(818, 85);
		gridPanel4.Span = "20% 25% 53% 2%";
		((Control)gridPanel4).TabIndex = 15;
		((Control)gridPanel4).TabStop = false;
		((Control)gridPanel4).Text = "gridPanel4";
		((Control)sliRan_cam1).BackColor = Color.Transparent;
		((Control)sliRan_cam1).Location = new Point(373, 5);
		sliRan_cam1.LowerValue = 800;
		((Control)sliRan_cam1).Margin = new Padding(5, 5, 0, 5);
		sliRan_cam1.Maximum = 2100;
		sliRan_cam1.Minimum = 800;
		sliRan_cam1.MinimumRange = 1;
		((Control)sliRan_cam1).MinimumSize = new Size(80, 60);
		((Control)sliRan_cam1).Name = "sliRan_cam1";
		sliRan_cam1.SelectedTrackColor = Color.FromArgb(255, 233, 0);
		((Control)sliRan_cam1).Size = new Size(429, 75);
		sliRan_cam1.Step = 50;
		((Control)sliRan_cam1).TabIndex = 7;
		((Control)sliRan_cam1).TabStop = false;
		sliRan_cam1.TextInterval = 8;
		sliRan_cam1.ThumbColor = Color.FromArgb(255, 233, 0);
		sliRan_cam1.ThumbSize = 8;
		sliRan_cam1.TickColor = Color.Gray;
		sliRan_cam1.TickFont = new Font("Segoe UI", 8f);
		sliRan_cam1.TickFrequency = 50;
		sliRan_cam1.TickTextColor = Color.FromArgb(164, 164, 165);
		sliRan_cam1.TrackColor = Color.FromArgb(250, 250, 250);
		sliRan_cam1.TrackHeightCustom = 6;
		sliRan_cam1.UpperValue = 1500;
		sliRan_cam1.ValueBubbleColor = Color.FromArgb(95, 95, 96);
		sliRan_cam1.ValueBubbleVisible = true;
		sliRan_cam1.ValueFont = new Font("Segoe UI", 9f);
		((Control)gridPanel5).Controls.Add((Control)(object)inp_cam1Max);
		((Control)gridPanel5).Controls.Add((Control)(object)lab_cam1Max);
		((Control)gridPanel5).Controls.Add((Control)(object)inp_cam1Min);
		((Control)gridPanel5).Controls.Add((Control)(object)lab_cam1Min);
		gridPanel4.SetIndex((Control)(object)gridPanel5, 2);
		((Control)gridPanel5).Location = new Point(167, 3);
		((Control)gridPanel5).Name = "gridPanel5";
		((Control)gridPanel5).RightToLeft = (RightToLeft)0;
		((Control)gridPanel5).Size = new Size(198, 79);
		gridPanel5.Span = "45% 55%;45% 55%";
		((Control)gridPanel5).TabIndex = 3;
		((Control)gridPanel5).Text = "gridPanel5";
		inp_cam1Max.BackColor = Color.FromArgb(30, 34, 37);
		inp_cam1Max.BorderActive = Color.FromArgb(255, 233, 0);
		inp_cam1Max.BorderColor = Color.FromArgb(66, 69, 71);
		inp_cam1Max.BorderHover = Color.FromArgb(255, 233, 0);
		inp_cam1Max.ForeColor = Color.White;
		((Control)inp_cam1Max).Location = new Point(92, 43);
		((Control)inp_cam1Max).Margin = new Padding(3, 3, 15, 3);
		((Control)inp_cam1Max).Name = "inp_cam1Max";
		((Control)inp_cam1Max).Size = new Size(91, 34);
		((Control)inp_cam1Max).TabIndex = 3;
		((Control)inp_cam1Max).Text = "1000";
		inp_cam1Max.VerifyChar += new InputVerifyCharEventHandler(inp_VerifyChar);
		((Control)inp_cam1Max).KeyDown += new KeyEventHandler(inp_Max_EnterDown);
		((Control)inp_cam1Max).Leave += inp_Max_Leave;
		((Control)lab_cam1Max).AutoSize = true;
		((Control)lab_cam1Max).ForeColor = Color.White;
		((Control)lab_cam1Max).Location = new Point(5, 45);
		((Control)lab_cam1Max).Margin = new Padding(5);
		((Control)lab_cam1Max).Name = "lab_cam1Max";
		((Control)lab_cam1Max).Size = new Size(79, 30);
		((Control)lab_cam1Max).TabIndex = 2;
		((Control)lab_cam1Max).Text = "Max :";
		lab_cam1Max.TextAlign = (ContentAlignment)64;
		inp_cam1Min.BackColor = Color.FromArgb(30, 34, 37);
		inp_cam1Min.BorderActive = Color.FromArgb(255, 233, 0);
		inp_cam1Min.BorderColor = Color.FromArgb(66, 69, 71);
		inp_cam1Min.BorderHover = Color.FromArgb(255, 233, 0);
		inp_cam1Min.ForeColor = Color.White;
		((Control)inp_cam1Min).Location = new Point(92, 3);
		((Control)inp_cam1Min).Margin = new Padding(3, 3, 15, 3);
		((Control)inp_cam1Min).Name = "inp_cam1Min";
		((Control)inp_cam1Min).Size = new Size(91, 34);
		((Control)inp_cam1Min).TabIndex = 1;
		((Control)inp_cam1Min).Text = "1000";
		inp_cam1Min.VerifyChar += new InputVerifyCharEventHandler(inp_VerifyChar);
		((Control)inp_cam1Min).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_cam1Min).Leave += inp_Min_Leave;
		((Control)lab_cam1Min).AutoSize = true;
		((Control)lab_cam1Min).ForeColor = Color.White;
		((Control)lab_cam1Min).Location = new Point(5, 5);
		((Control)lab_cam1Min).Margin = new Padding(5);
		((Control)lab_cam1Min).Name = "lab_cam1Min";
		((Control)lab_cam1Min).Size = new Size(79, 30);
		((Control)lab_cam1Min).TabIndex = 0;
		((Control)lab_cam1Min).Text = "Min :";
		lab_cam1Min.TextAlign = (ContentAlignment)64;
		((Control)lab_Inp_cam1).BackColor = Color.FromArgb(57, 59, 48);
		gridPanel4.SetIndex((Control)(object)lab_Inp_cam1, 1);
		((Control)lab_Inp_cam1).Location = new Point(5, 5);
		((Control)lab_Inp_cam1).Margin = new Padding(5);
		((Control)lab_Inp_cam1).Name = "lab_Inp_cam1";
		lab_Inp_cam1.Sel_Idx = -1;
		((Control)lab_Inp_cam1).Size = new Size(154, 75);
		((Control)lab_Inp_cam1).TabIndex = 2;
		((Control)lab_Inp_cam1).TabStop = false;
		lab_Inp_cam1.OnSelectedIndexChangedEvent += lab_Inp_cam1_OnSelectedIndexChangedEvent;
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)sliRan_reset);
		((Control)gridPanel1).Controls.Add((Control)(object)gridPanel2);
		((Control)gridPanel1).Controls.Add((Control)(object)lab_Inp_reset);
		((Control)gridPanel1).Location = new Point(3, 94);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(818, 85);
		gridPanel1.Span = "20% 25% 53% 2%";
		((Control)gridPanel1).TabIndex = 14;
		((Control)gridPanel1).TabStop = false;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)sliRan_reset).BackColor = Color.Transparent;
		((Control)sliRan_reset).Location = new Point(373, 5);
		sliRan_reset.LowerValue = 800;
		((Control)sliRan_reset).Margin = new Padding(5, 5, 0, 5);
		sliRan_reset.Maximum = 2100;
		sliRan_reset.Minimum = 800;
		sliRan_reset.MinimumRange = 1;
		((Control)sliRan_reset).MinimumSize = new Size(80, 60);
		((Control)sliRan_reset).Name = "sliRan_reset";
		sliRan_reset.SelectedTrackColor = Color.FromArgb(255, 233, 0);
		((Control)sliRan_reset).Size = new Size(429, 75);
		sliRan_reset.Step = 50;
		((Control)sliRan_reset).TabIndex = 6;
		((Control)sliRan_reset).TabStop = false;
		sliRan_reset.TextInterval = 8;
		sliRan_reset.ThumbColor = Color.FromArgb(255, 233, 0);
		sliRan_reset.ThumbSize = 8;
		sliRan_reset.TickColor = Color.Gray;
		sliRan_reset.TickFont = new Font("Segoe UI", 8f);
		sliRan_reset.TickFrequency = 50;
		sliRan_reset.TickTextColor = Color.FromArgb(164, 164, 165);
		sliRan_reset.TrackColor = Color.FromArgb(250, 250, 250);
		sliRan_reset.TrackHeightCustom = 6;
		sliRan_reset.UpperValue = 1500;
		sliRan_reset.ValueBubbleColor = Color.FromArgb(95, 95, 96);
		sliRan_reset.ValueBubbleVisible = true;
		sliRan_reset.ValueFont = new Font("Segoe UI", 9f);
		((Control)gridPanel2).Controls.Add((Control)(object)inp_resetMax);
		((Control)gridPanel2).Controls.Add((Control)(object)lab_resetMax);
		((Control)gridPanel2).Controls.Add((Control)(object)inp_resetMin);
		((Control)gridPanel2).Controls.Add((Control)(object)lab_resetMin);
		gridPanel1.SetIndex((Control)(object)gridPanel2, 2);
		((Control)gridPanel2).Location = new Point(167, 3);
		((Control)gridPanel2).Name = "gridPanel2";
		((Control)gridPanel2).RightToLeft = (RightToLeft)0;
		((Control)gridPanel2).Size = new Size(198, 79);
		gridPanel2.Span = "45% 55%;45% 55%";
		((Control)gridPanel2).TabIndex = 3;
		((Control)gridPanel2).Text = "gridPanel2";
		inp_resetMax.BackColor = Color.FromArgb(30, 34, 37);
		inp_resetMax.BorderActive = Color.FromArgb(255, 233, 0);
		inp_resetMax.BorderColor = Color.FromArgb(66, 69, 71);
		inp_resetMax.BorderHover = Color.FromArgb(255, 233, 0);
		inp_resetMax.ForeColor = Color.White;
		((Control)inp_resetMax).Location = new Point(92, 43);
		((Control)inp_resetMax).Margin = new Padding(3, 3, 15, 3);
		((Control)inp_resetMax).Name = "inp_resetMax";
		((Control)inp_resetMax).Size = new Size(91, 34);
		((Control)inp_resetMax).TabIndex = 3;
		((Control)inp_resetMax).Text = "1000";
		inp_resetMax.VerifyChar += new InputVerifyCharEventHandler(inp_VerifyChar);
		((Control)inp_resetMax).KeyDown += new KeyEventHandler(inp_Max_EnterDown);
		((Control)inp_resetMax).Leave += inp_Max_Leave;
		((Control)lab_resetMax).AutoSize = true;
		((Control)lab_resetMax).ForeColor = Color.White;
		((Control)lab_resetMax).Location = new Point(5, 45);
		((Control)lab_resetMax).Margin = new Padding(5);
		((Control)lab_resetMax).Name = "lab_resetMax";
		((Control)lab_resetMax).Size = new Size(79, 30);
		((Control)lab_resetMax).TabIndex = 2;
		((Control)lab_resetMax).Text = "Max :";
		lab_resetMax.TextAlign = (ContentAlignment)64;
		inp_resetMin.BackColor = Color.FromArgb(30, 34, 37);
		inp_resetMin.BorderActive = Color.FromArgb(255, 233, 0);
		inp_resetMin.BorderColor = Color.FromArgb(66, 69, 71);
		inp_resetMin.BorderHover = Color.FromArgb(255, 233, 0);
		inp_resetMin.ForeColor = Color.White;
		((Control)inp_resetMin).Location = new Point(92, 3);
		((Control)inp_resetMin).Margin = new Padding(3, 3, 15, 3);
		((Control)inp_resetMin).Name = "inp_resetMin";
		((Control)inp_resetMin).Size = new Size(91, 34);
		((Control)inp_resetMin).TabIndex = 1;
		((Control)inp_resetMin).Text = "1000";
		inp_resetMin.VerifyChar += new InputVerifyCharEventHandler(inp_VerifyChar);
		((Control)inp_resetMin).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_resetMin).Leave += inp_Min_Leave;
		((Control)lab_resetMin).AutoSize = true;
		((Control)lab_resetMin).ForeColor = Color.White;
		((Control)lab_resetMin).Location = new Point(5, 5);
		((Control)lab_resetMin).Margin = new Padding(5);
		((Control)lab_resetMin).Name = "lab_resetMin";
		((Control)lab_resetMin).Size = new Size(79, 30);
		((Control)lab_resetMin).TabIndex = 0;
		((Control)lab_resetMin).Text = "Min :";
		lab_resetMin.TextAlign = (ContentAlignment)64;
		((Control)lab_Inp_reset).BackColor = Color.FromArgb(57, 59, 48);
		gridPanel1.SetIndex((Control)(object)lab_Inp_reset, 1);
		((Control)lab_Inp_reset).Location = new Point(5, 5);
		((Control)lab_Inp_reset).Margin = new Padding(5);
		((Control)lab_Inp_reset).Name = "lab_Inp_reset";
		lab_Inp_reset.Sel_Idx = -1;
		((Control)lab_Inp_reset).Size = new Size(154, 75);
		((Control)lab_Inp_reset).TabIndex = 2;
		((Control)lab_Inp_reset).TabStop = false;
		lab_Inp_reset.OnSelectedIndexChangedEvent += lab_Inp_reset_OnSelectedIndexChangedEvent;
		((ContainerPanel)grpan_zoom).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)grpan_zoom).BorderWidth = 2f;
		((Control)grpan_zoom).Controls.Add((Control)(object)sliRan_zoom);
		((Control)grpan_zoom).Controls.Add((Control)(object)gridPanel3);
		((Control)grpan_zoom).Controls.Add((Control)(object)lab_Inp_zoom);
		((Control)grpan_zoom).Location = new Point(3, 3);
		((Control)grpan_zoom).Name = "grpan_zoom";
		((Control)grpan_zoom).Size = new Size(818, 85);
		grpan_zoom.Span = "20% 25% 53% 2%";
		((Control)grpan_zoom).TabIndex = 13;
		((Control)grpan_zoom).TabStop = false;
		((Control)grpan_zoom).Text = "gridPanel1";
		((Control)sliRan_zoom).BackColor = Color.Transparent;
		grpan_zoom.SetIndex((Control)(object)sliRan_zoom, 3);
		((Control)sliRan_zoom).Location = new Point(373, 5);
		sliRan_zoom.LowerValue = 800;
		((Control)sliRan_zoom).Margin = new Padding(5, 5, 0, 5);
		sliRan_zoom.Maximum = 2100;
		sliRan_zoom.Minimum = 800;
		sliRan_zoom.MinimumRange = 1;
		((Control)sliRan_zoom).MinimumSize = new Size(80, 60);
		((Control)sliRan_zoom).Name = "sliRan_zoom";
		sliRan_zoom.SelectedTrackColor = Color.FromArgb(255, 233, 0);
		((Control)sliRan_zoom).Size = new Size(429, 75);
		sliRan_zoom.Step = 50;
		((Control)sliRan_zoom).TabIndex = 5;
		((Control)sliRan_zoom).TabStop = false;
		sliRan_zoom.TextInterval = 8;
		sliRan_zoom.ThumbColor = Color.FromArgb(255, 233, 0);
		sliRan_zoom.ThumbSize = 8;
		sliRan_zoom.TickColor = Color.Gray;
		sliRan_zoom.TickFont = new Font("Segoe UI", 8f);
		sliRan_zoom.TickFrequency = 50;
		sliRan_zoom.TickTextColor = Color.FromArgb(164, 164, 165);
		sliRan_zoom.TrackColor = Color.FromArgb(250, 250, 250);
		sliRan_zoom.TrackHeightCustom = 6;
		sliRan_zoom.UpperValue = 2100;
		sliRan_zoom.ValueBubbleColor = Color.FromArgb(95, 95, 96);
		sliRan_zoom.ValueBubbleVisible = true;
		sliRan_zoom.ValueFont = new Font("Segoe UI", 9f);
		sliRan_zoom.OnRangeValChangedEvent += sliRan_zoomCtrl_OnRangeValChangedEvent;
		((Control)gridPanel3).Controls.Add((Control)(object)inp_zoomMax);
		((Control)gridPanel3).Controls.Add((Control)(object)lab_zoomMax);
		((Control)gridPanel3).Controls.Add((Control)(object)inp_zoomMin);
		((Control)gridPanel3).Controls.Add((Control)(object)lab_zoomMin);
		grpan_zoom.SetIndex((Control)(object)gridPanel3, 2);
		((Control)gridPanel3).Location = new Point(167, 3);
		((Control)gridPanel3).Name = "gridPanel3";
		((Control)gridPanel3).RightToLeft = (RightToLeft)0;
		((Control)gridPanel3).Size = new Size(198, 79);
		gridPanel3.Span = "45% 55%;45% 55%";
		((Control)gridPanel3).TabIndex = 3;
		((Control)gridPanel3).Text = "gridPanel3";
		inp_zoomMax.BackColor = Color.FromArgb(30, 34, 37);
		inp_zoomMax.BorderActive = Color.FromArgb(255, 233, 0);
		inp_zoomMax.BorderColor = Color.FromArgb(66, 69, 71);
		inp_zoomMax.BorderHover = Color.FromArgb(255, 233, 0);
		inp_zoomMax.ForeColor = Color.White;
		((Control)inp_zoomMax).Location = new Point(92, 43);
		((Control)inp_zoomMax).Margin = new Padding(3, 3, 15, 3);
		((Control)inp_zoomMax).Name = "inp_zoomMax";
		((Control)inp_zoomMax).Size = new Size(91, 34);
		((Control)inp_zoomMax).TabIndex = 3;
		((Control)inp_zoomMax).Text = "1000";
		inp_zoomMax.VerifyChar += new InputVerifyCharEventHandler(inp_VerifyChar);
		((Control)inp_zoomMax).KeyDown += new KeyEventHandler(inp_Max_EnterDown);
		((Control)inp_zoomMax).Leave += inp_Max_Leave;
		((Control)lab_zoomMax).AutoSize = true;
		((Control)lab_zoomMax).ForeColor = Color.White;
		((Control)lab_zoomMax).Location = new Point(5, 45);
		((Control)lab_zoomMax).Margin = new Padding(5);
		((Control)lab_zoomMax).Name = "lab_zoomMax";
		((Control)lab_zoomMax).Size = new Size(79, 30);
		((Control)lab_zoomMax).TabIndex = 2;
		((Control)lab_zoomMax).Text = "Max :";
		lab_zoomMax.TextAlign = (ContentAlignment)64;
		inp_zoomMin.BackColor = Color.FromArgb(30, 34, 37);
		inp_zoomMin.BorderActive = Color.FromArgb(255, 233, 0);
		inp_zoomMin.BorderColor = Color.FromArgb(66, 69, 71);
		inp_zoomMin.BorderHover = Color.FromArgb(255, 233, 0);
		((Control)inp_zoomMin).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		inp_zoomMin.ForeColor = Color.White;
		((Control)inp_zoomMin).Location = new Point(92, 3);
		((Control)inp_zoomMin).Margin = new Padding(3, 3, 15, 3);
		((Control)inp_zoomMin).Name = "inp_zoomMin";
		((Control)inp_zoomMin).Size = new Size(91, 34);
		((Control)inp_zoomMin).TabIndex = 1;
		((Control)inp_zoomMin).Text = "1000";
		inp_zoomMin.VerifyChar += new InputVerifyCharEventHandler(inp_VerifyChar);
		((Control)inp_zoomMin).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_zoomMin).Leave += inp_Min_Leave;
		((Control)lab_zoomMin).AutoSize = true;
		((Control)lab_zoomMin).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_zoomMin).ForeColor = Color.White;
		((Control)lab_zoomMin).Location = new Point(5, 5);
		((Control)lab_zoomMin).Margin = new Padding(5);
		((Control)lab_zoomMin).Name = "lab_zoomMin";
		((Control)lab_zoomMin).Size = new Size(79, 30);
		((Control)lab_zoomMin).TabIndex = 0;
		((Control)lab_zoomMin).Text = "Min :";
		lab_zoomMin.TextAlign = (ContentAlignment)64;
		((Control)lab_Inp_zoom).BackColor = Color.FromArgb(57, 59, 48);
		grpan_zoom.SetIndex((Control)(object)lab_Inp_zoom, 1);
		((Control)lab_Inp_zoom).Location = new Point(5, 5);
		((Control)lab_Inp_zoom).Margin = new Padding(5);
		((Control)lab_Inp_zoom).Name = "lab_Inp_zoom";
		lab_Inp_zoom.Sel_Idx = -1;
		((Control)lab_Inp_zoom).Size = new Size(154, 75);
		((Control)lab_Inp_zoom).TabIndex = 2;
		((Control)lab_Inp_zoom).TabStop = false;
		lab_Inp_zoom.OnSelectedIndexChangedEvent += lab_Inp_zoom_OnSelectedIndexChangedEvent;
		((Control)pahead_Tips).BackColor = Color.FromArgb(49, 51, 41);
		((Control)pahead_Tips).Controls.Add((Control)(object)btn_closeTips);
		((Control)pahead_Tips).Controls.Add((Control)(object)lab_FWTips);
		((Control)pahead_Tips).Controls.Add((Control)(object)button2);
		((Control)pahead_Tips).Dock = (DockStyle)5;
		pahead_Tips.DragMove = false;
		grpan_Main.SetIndex((Control)(object)pahead_Tips, 2);
		((Control)pahead_Tips).Location = new Point(10, 110);
		((Control)pahead_Tips).Margin = new Padding(10);
		((Control)pahead_Tips).Name = "pahead_Tips";
		((Control)pahead_Tips).Size = new Size(810, 70);
		((Control)pahead_Tips).TabIndex = 58;
		((Control)pahead_Tips).Text = "";
		btn_closeTips.DisplayStyle = (TButtonDisplayStyle)2;
		((Control)btn_closeTips).Dock = (DockStyle)4;
		btn_closeTips.Ghost = true;
		((IControl)btn_closeTips).HandCursor = Cursors.Default;
		btn_closeTips.Icon = (Image)(object)Resources.关闭;
		((Control)btn_closeTips).Location = new Point(775, 0);
		((Control)btn_closeTips).Margin = new Padding(0, 5, 0, 0);
		((Control)btn_closeTips).Name = "btn_closeTips";
		((Control)btn_closeTips).Size = new Size(35, 70);
		((Control)btn_closeTips).TabIndex = 7;
		((Control)btn_closeTips).TabStop = false;
		((Control)btn_closeTips).Text = "button3";
		((Control)btn_closeTips).Click += btn_closeTips_Click;
		((Control)lab_FWTips).BackColor = Color.Transparent;
		((Control)lab_FWTips).Dock = (DockStyle)3;
		((Control)lab_FWTips).Font = new Font("Microsoft Sans Serif", 8.249999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_FWTips).ForeColor = Color.White;
		((Control)lab_FWTips).Location = new Point(35, 0);
		((Control)lab_FWTips).Margin = new Padding(0);
		((Control)lab_FWTips).Name = "lab_FWTips";
		((Control)lab_FWTips).Size = new Size(728, 70);
		((Control)lab_FWTips).TabIndex = 6;
		((Control)lab_FWTips).Text = "Slot 功能是在 MSP DisplayPort 数据流中提取 4 路独立的 RC 控制输入，通过不同通道数值区间的划分，实现变焦幅值映射、变焦复位、CAM 切换、HDMI IN 切换、采样频率设置及飞控平台配置等多种相机与视频传输相关的控制功能，可灵活适配不同飞控平台与拍摄场景，参数配置完成后，记得使用界面右下角的保存按钮来保存你的设置，确保配置生效并正常运行。";
		lab_FWTips.TextAlign = (ContentAlignment)16;
		button2.DisplayStyle = (TButtonDisplayStyle)2;
		((Control)button2).Dock = (DockStyle)3;
		button2.Ghost = true;
		((IControl)button2).HandCursor = Cursors.Default;
		((IControl)button2).HandDragFolder = false;
		button2.Icon = (Image)(object)Resources.tip_icon;
		button2.IconRatio = 1f;
		((Control)button2).Location = new Point(0, 0);
		((Control)button2).Margin = new Padding(0);
		((Control)button2).Name = "button2";
		((Control)button2).Size = new Size(35, 70);
		((Control)button2).TabIndex = 0;
		((Control)button2).TabStop = false;
		((Control)button2).Text = "button2";
		button2.WaveSize = 0;
		((Control)grpan_flyCtrl).Controls.Add((Control)(object)stackPanel2);
		((Control)grpan_flyCtrl).Controls.Add((Control)(object)label12);
		((Control)grpan_flyCtrl).Controls.Add((Control)(object)input1);
		((Control)grpan_flyCtrl).Controls.Add((Control)(object)select4);
		((Control)grpan_flyCtrl).Controls.Add((Control)(object)label4);
		((Control)grpan_flyCtrl).Controls.Add((Control)(object)label1);
		grpan_Main.SetIndex((Control)(object)grpan_flyCtrl, 5);
		((Control)grpan_flyCtrl).Location = new Point(3, 895);
		((Control)grpan_flyCtrl).Name = "grpan_flyCtrl";
		((Control)grpan_flyCtrl).Size = new Size(824, 72);
		grpan_flyCtrl.Span = "35% 10% 15% 10% 10% 20%;";
		((Control)grpan_flyCtrl).TabIndex = 14;
		((Control)grpan_flyCtrl).Text = "gridPanel2";
		stackPanel2.Controls.Add((Control)(object)label2);
		stackPanel2.Controls.Add((Control)(object)switch1);
		grpan_flyCtrl.SetIndex((Control)(object)stackPanel2, 1);
		((Control)stackPanel2).Location = new Point(3, 3);
		((Control)stackPanel2).Name = "stackPanel2";
		((Control)stackPanel2).Size = new Size(282, 66);
		((Control)stackPanel2).TabIndex = 33;
		((Control)stackPanel2).Text = "stackPanel2";
		((Control)label2).BackColor = Color.Transparent;
		((Control)label2).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label2).ForeColor = Color.FromArgb(51, 51, 51);
		((Control)label2).Location = new Point(121, 0);
		((Control)label2).Margin = new Padding(0);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(113, 66);
		((Control)label2).TabIndex = 30;
		((Control)label2).Text = "隐藏未使用";
		label2.TextAlign = (ContentAlignment)64;
		((Control)label2).Visible = false;
		((Control)switch1).Location = new Point(3, 5);
		((Control)switch1).Margin = new Padding(3, 5, 3, 5);
		((Control)switch1).Name = "switch1";
		((Control)switch1).Size = new Size(115, 56);
		((Control)switch1).TabIndex = 0;
		((Control)switch1).Text = "switch1";
		((IControl)switch1).Visible = false;
		((Control)label12).BackColor = Color.Transparent;
		((Control)label12).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label12).ForeColor = Color.FromArgb(66, 130, 248);
		grpan_flyCtrl.SetIndex((Control)(object)label12, 4);
		((Control)label12).Location = new Point(494, 0);
		((Control)label12).Margin = new Padding(0);
		((Control)label12).Name = "label12";
		((Control)label12).Size = new Size(82, 72);
		((Control)label12).TabIndex = 32;
		label12.TextAlign = (ContentAlignment)32;
		((IControl)input1).HandDragFolder = false;
		input1.HandShortcutKeys = false;
		grpan_flyCtrl.SetIndex((Control)(object)input1, 3);
		((Control)input1).Location = new Point(374, 3);
		((Control)input1).Name = "input1";
		input1.PrefixText = "";
		((Control)input1).Size = new Size(118, 66);
		input1.SuffixText = "Hz";
		((Control)input1).TabIndex = 31;
		((Control)input1).TabStop = false;
		((Control)input1).Text = "200";
		input1.TextAlign = (HorizontalAlignment)1;
		((IControl)input1).Visible = false;
		((Control)select4).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		grpan_flyCtrl.SetIndex((Control)(object)select4, 6);
		select4.Items.AddRange(new object[1] { "Betaflight" });
		select4.List = true;
		((Control)select4).Location = new Point(664, 5);
		((Control)select4).Margin = new Padding(5);
		((Control)select4).Name = "select4";
		select4.SelectedIndex = 0;
		select4.SelectedValue = "Betaflight";
		((Control)select4).Size = new Size(155, 62);
		((Control)select4).TabIndex = 30;
		((Control)select4).Text = "Betaflight";
		((IControl)select4).Visible = false;
		((Input)select4).WaveSize = 0;
		((Control)label4).BackColor = Color.Transparent;
		((Control)label4).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label4).ForeColor = Color.FromArgb(66, 130, 248);
		grpan_flyCtrl.SetIndex((Control)(object)label4, 5);
		((Control)label4).Location = new Point(577, 0);
		((Control)label4).Margin = new Padding(0);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(82, 72);
		((Control)label4).TabIndex = 29;
		((Control)label4).Text = "飞控类型";
		label4.TextAlign = (ContentAlignment)64;
		((Control)label4).Visible = false;
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label1).ForeColor = Color.FromArgb(66, 130, 248);
		grpan_flyCtrl.SetIndex((Control)(object)label1, 2);
		((Control)label1).Location = new Point(288, 0);
		((Control)label1).Margin = new Padding(0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(82, 72);
		((Control)label1).TabIndex = 1;
		((Control)label1).Text = "采样频率";
		label1.TextAlign = (ContentAlignment)64;
		((Control)label1).Visible = false;
		((ContainerPanel)grpan_Btn).Back = Color.Transparent;
		((Control)grpan_Btn).BackColor = Color.Transparent;
		((ContainerPanel)grpan_Btn).BorderColor = Color.FromArgb(235, 237, 240);
		((Control)grpan_Btn).Controls.Add((Control)(object)label13);
		((Control)grpan_Btn).Controls.Add((Control)(object)btn_RCUpload);
		((Control)grpan_Btn).Controls.Add((Control)(object)btn_quit);
		((Control)grpan_Btn).Controls.Add((Control)(object)sel_portname);
		((Control)grpan_Btn).Controls.Add((Control)(object)btn_refresh);
		((Control)grpan_Btn).Controls.Add((Control)(object)btn_connect);
		grpan_Main.SetIndex((Control)(object)grpan_Btn, 1);
		((Control)grpan_Btn).Location = new Point(0, 0);
		((Control)grpan_Btn).Margin = new Padding(0);
		((Control)grpan_Btn).Name = "grpan_Btn";
		((Control)grpan_Btn).Size = new Size(830, 50);
		grpan_Btn.Span = "20% 5% 20% 25% 20% 10%";
		((Control)grpan_Btn).TabIndex = 0;
		((Control)grpan_Btn).Text = "gridPanel2";
		((Control)label13).AutoSize = true;
		grpan_Btn.SetIndex((Control)(object)label13, 4);
		((Control)label13).Location = new Point(379, 5);
		((Control)label13).Margin = new Padding(5);
		((Control)label13).Name = "label13";
		((Control)label13).Size = new Size(198, 40);
		((Control)label13).TabIndex = 37;
		label13.TextAlign = (ContentAlignment)64;
		((IControl)btn_RCUpload).ColorScheme = (TAMode)1;
		btn_RCUpload.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_RCUpload).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_RCUpload.ForeColor = Color.FromArgb(35, 35, 35);
		grpan_Btn.SetIndex((Control)(object)btn_RCUpload, 5);
		((Control)btn_RCUpload).Location = new Point(586, 5);
		((Control)btn_RCUpload).Margin = new Padding(5);
		((Control)btn_RCUpload).Name = "btn_RCUpload";
		((Control)btn_RCUpload).Size = new Size(156, 40);
		((Control)btn_RCUpload).TabIndex = 36;
		((Control)btn_RCUpload).Text = "Upload RC file";
		btn_RCUpload.WaveSize = 0;
		((Control)btn_RCUpload).Click += btn_SaveUpload_Click;
		btn_quit.DisplayStyle = (TButtonDisplayStyle)2;
		btn_quit.ForeColor = Color.White;
		btn_quit.Ghost = true;
		btn_quit.Icon = (Image)(object)Resources.关闭1;
		btn_quit.IconRatio = 0.9f;
		grpan_Btn.SetIndex((Control)(object)btn_quit, 7);
		((Control)btn_quit).Location = new Point(747, 0);
		((Control)btn_quit).Margin = new Padding(0);
		((Control)btn_quit).Name = "btn_quit";
		((Control)btn_quit).Size = new Size(83, 50);
		((Control)btn_quit).TabIndex = 35;
		((Control)btn_quit).TabStop = false;
		((Control)btn_quit).Text = "Quit";
		btn_quit.WaveSize = 0;
		((Control)btn_quit).Click += btn_Quit_Click;
		((Input)sel_portname).BackColor = Color.FromArgb(30, 34, 37);
		((Input)sel_portname).BorderWidth = 0f;
		((IControl)sel_portname).ColorScheme = (TAMode)2;
		((Control)sel_portname).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_portname).ForeColor = Color.White;
		grpan_Btn.SetIndex((Control)(object)sel_portname, 1);
		sel_portname.List = true;
		((Control)sel_portname).Location = new Point(5, 5);
		((Control)sel_portname).Margin = new Padding(5);
		((Control)sel_portname).Name = "sel_portname";
		((Control)sel_portname).Size = new Size(156, 40);
		((Control)sel_portname).TabIndex = 26;
		((Input)sel_portname).WaveSize = 0;
		sel_portname.SelectedIndexChanged += new IntEventHandler(sel_portname_SelectedIndexChanged);
		btn_refresh.DisplayStyle = (TButtonDisplayStyle)2;
		btn_refresh.ForeColor = Color.White;
		btn_refresh.Ghost = true;
		btn_refresh.Icon = (Image)(object)Resources.refresh_yellow;
		btn_refresh.IconRatio = 1f;
		grpan_Btn.SetIndex((Control)(object)btn_refresh, 2);
		((Control)btn_refresh).Location = new Point(166, 0);
		((Control)btn_refresh).Margin = new Padding(0);
		((Control)btn_refresh).Name = "btn_refresh";
		((Control)btn_refresh).Size = new Size(42, 50);
		((Control)btn_refresh).TabIndex = 29;
		((Control)btn_refresh).Text = "Refresh";
		btn_refresh.WaveSize = 0;
		((Control)btn_refresh).Click += btn_refresh_Click;
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)0;
		((ScrollableControl)this).AutoScroll = true;
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).Controls.Add((Control)(object)grpan_Main);
		((Control)this).Margin = new Padding(0);
		((Control)this).MinimumSize = new Size(840, 648);
		((Control)this).Name = "RCModeCtrl";
		((Control)this).Padding = new Padding(5, 15, 5, 15);
		((Control)this).Size = new Size(840, 1000);
		((UserControl)this).Load += RCModeCtrl_Load;
		((Control)grpan_Main).ResumeLayout(false);
		((Control)gridPanel8).ResumeLayout(false);
		((Control)gridPanel8).PerformLayout();
		((Control)stackPanel1).ResumeLayout(false);
		((Control)gridPanel6).ResumeLayout(false);
		((Control)gridPanel7).ResumeLayout(false);
		((Control)gridPanel7).PerformLayout();
		((Control)gridPanel4).ResumeLayout(false);
		((Control)gridPanel5).ResumeLayout(false);
		((Control)gridPanel5).PerformLayout();
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel2).ResumeLayout(false);
		((Control)gridPanel2).PerformLayout();
		((Control)grpan_zoom).ResumeLayout(false);
		((Control)gridPanel3).ResumeLayout(false);
		((Control)gridPanel3).PerformLayout();
		((Control)pahead_Tips).ResumeLayout(false);
		((Control)grpan_flyCtrl).ResumeLayout(false);
		((Control)stackPanel2).ResumeLayout(false);
		((Control)grpan_Btn).ResumeLayout(false);
		((Control)grpan_Btn).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
