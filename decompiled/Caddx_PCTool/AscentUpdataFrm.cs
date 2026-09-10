using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class AscentUpdataFrm : UserControl, IDisposable
{
	private System.Timers.Timer _btnBlinkTimer;

	private int _blinkPlusing;

	private string _deviceName = "";

	private string _portName = "";

	private bool _isCanecl = false;

	private bool _IsswCheck;

	private string _perVer;

	private string _currVer;

	private bool _isStartUpg = false;

	private bool _isDeviceConnected;

	private SerialPortSessionHandle _upgradeSession;

	private UsbSerialportFSM _upgradeFsm;

	public Form Frm;

	private ResAscentInfo _currAsceDevInfo;

	private bool _isInterceUpg = true;

	private string _firmwarePath;

	private IContainer components = null;

	private UploadDragger uploadDragger1;

	private GridPanel grpan_main;

	private GridPanel grpan_process;

	private GridPanel grpan_procDesc;

	private Progress progress1;

	private PictureBox pictureBox2;

	private Button button2;

	private PageHeader pageHeader3;

	private PageHeader pageHeader4;

	private Button btn_closeTips;

	private Label lab_FWTips;

	private GridPanel grpan_upgrade;

	private Label lab_selectFile;

	private StackPanel stackPanel1;

	private Button btn_startUpg;

	private Label lab_versChange;

	private Label lab_upgDesc;

	private Label lab_filepath;

	private StackPanel stackPanel2;

	private Label label1;

	private PageHeader pageHeader1;

	private PictureBox pictureBox1;

	private Divider divider1;

	private Label lab_tempture;

	private Label lab_sn;

	private Button btn_Disconnect;

	private Label lab_firmwareVer;

	private Label lab_hardwareVer;

	private Label lab_devName;

	public string DeviceName
	{
		get
		{
			return string.IsNullOrEmpty(_deviceName) ? Lang.T("ascent_upgrade.device_not_found") : _deviceName;
		}
		set
		{
			_deviceName = value;
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

	public event EventHandler<AscentFrmEventArgs> OnAscentFrmEvnet;

	public AscentUpdataFrm()
	{
		InitializeComponent();
		((Control)this).Cursor = Cursors.Default;
	}

	public AscentUpdataFrm(ResAscentInfo info)
	{
		InitializeComponent();
		((Control)this).Cursor = Cursors.Default;
		_currAsceDevInfo = info;
	}

	public void AttachUpgradeSession(SerialPortSessionHandle handle, ResAscentInfo deviceInfo)
	{
		_upgradeSession = handle ?? throw new ArgumentNullException("handle");
		_currAsceDevInfo = deviceInfo ?? throw new ArgumentNullException("deviceInfo");
		_portName = handle.PortName;
	}

	private void AscentUpdataFrm_Load(object sender, EventArgs e)
	{
		((IControl)grpan_process).Visible = false;
		ReloadLang();
		ReloadFont();
		UpdateDeviceInfo(_currAsceDevInfo.DevName, _currAsceDevInfo.FWVers, _currAsceDevInfo.SN, _currAsceDevInfo.HWVers, _currAsceDevInfo.MCUTemp);
		((Control)lab_devName).Text = DeviceName.Replace("_", " ");
		ConnectDevice();
		ReloadProductImg();
		uploadDragger1.Filter = Lang.T("ascent_upgrade.file_filter");
	}

	private void ReloadProductImg()
	{
		switch (_deviceName.ToLower())
		{
		case "ascent lite vtx":
		case "ascent lite":
			pictureBox1.Image = (Image)(object)Resources.Ascent_Lite_VTX;
			((Control)lab_devName).Text = "Ascent Lite";
			break;
		case "ascent lite +":
		case "ascent lite plus":
			pictureBox1.Image = (Image)(object)Resources.Ascent_LitePlus;
			((Control)lab_devName).Text = "Ascent Lite +";
			break;
		case "yohd micro":
		case "ascent rc":
			pictureBox1.Image = (Image)(object)Resources.Ascent_RC;
			((Control)lab_devName).Text = "YoHD Micro";
			break;
		case "ascent gt":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GT;
			break;
		case "ascent gt 27k":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			((Control)lab_devName).Text = "Ascent GT 2.7k";
			break;
		case "ascent gt night":
		case "ascent gt ultra":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			break;
		case "ascent gt max hf":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTPro;
			break;
		case "ascent gt max wf":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GT_Max_WF;
			break;
		case "ascent gt max wf hub":
		case "ascent gt max hf hub":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			break;
		case "ascent gt max wf z8":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			((Control)lab_devName).Text = "Ascent GT Max WF Z8";
			break;
		case "ascent gt max hf z8":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			((Control)lab_devName).Text = "Ascent GT Max HF Z8";
			break;
		case "ascent gt max wf z40":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			((Control)lab_devName).Text = "Ascent GT Max WF Z40";
			break;
		case "ascent gt max hf z40":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			((Control)lab_devName).Text = "Ascent GT Max HF Z40";
			break;
		case "ascent gt pro":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTPro;
			break;
		case "ascent gt pro z8":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTProZ8;
			break;
		case "ascent gt pro z40":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTProZ40;
			break;
		case "ascent gt pro hub":
			pictureBox1.Image = (Image)(object)Resources.Ascent_CamHub;
			break;
		case "ascent vrx":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRX;
			break;
		case "ascent vrx pro":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRXPro;
			break;
		case "ascent vrx max hf":
		case "ascent vrx max":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRXMAX;
			break;
		case "ascent vrx max wf":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRX_MAX_WF;
			break;
		case "ascent relay":
		case "ascent repeater":
			pictureBox1.Image = (Image)(object)Resources.ascent_repeater;
			((Control)lab_devName).Text = "Ascent Repeater";
			break;
		case "ascent vrx cine":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRX_Cine;
			break;
		case "ascent google l":
			pictureBox1.Image = (Image)(object)Resources.Ascent_Goggles;
			break;
		case "opv2 vtx":
			pictureBox1.Image = (Image)(object)Resources.OPV2_VTX;
			break;
		case "opv2 z8 vtx":
			pictureBox1.Image = (Image)(object)Resources.OPV2_Z8_VTX;
			break;
		case "opv2 z40 vtx":
			pictureBox1.Image = (Image)(object)Resources.OPV2_Z40_VTX;
			break;
		case "opv2 vrx":
			pictureBox1.Image = (Image)(object)Resources.OPV2_VRX;
			break;
		default:
			pictureBox1.Image = (Image)(object)Resources.不支持设备;
			break;
		}
	}

	private void UpgFSM_OnUpgProcHappenEvent(object sender, UpgProcHappenEventArgs e)
	{
		switch (e.infoType)
		{
		case InfoType.debugInfo:
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				progress1.Value = e.Percent;
				((Control)lab_upgDesc).Text = e.Desc;
			});
			if (!(e.Percent >= 1f))
			{
				break;
			}
			progress1.State = (TType)1;
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				if (_isStartUpg)
				{
					_isStartUpg = false;
					progress1.Value = e.Percent;
					((Control)lab_upgDesc).Text = e.Desc;
					pictureBox2.Image = (Image)(object)Resources.升级成功;
					((IControl)btn_Disconnect).Enabled = true;
					progress1.Fill = Color.FromArgb(43, 164, 113);
					AscentFrmEventArgs e2 = new AscentFrmEventArgs
					{
						Desc = "UpgradeComp"
					};
					OnAscentFrmEvnet?.Invoke(null, e2);
				}
			});
			break;
		case InfoType.upgProcInfo:
			break;
		case InfoType.fail:
			_isStartUpg = false;
			Thread.Sleep(100);
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				progress1.State = (TType)4;
				((Control)lab_upgDesc).Text = e.Desc;
				pictureBox2.Image = (Image)(object)Resources.升级失败;
				progress1.Fill = Color.FromArgb(255, 63, 63);
				Button obj = btn_Disconnect;
				bool enabled = (((IControl)uploadDragger1).Enabled = true);
				((IControl)obj).Enabled = enabled;
				WriteLog.WriteLogFileToUI("升级失败，desc=" + e.Desc + ",err=" + e.ErrMsg, Color.Red);
				AscentFrmEventArgs e2 = new AscentFrmEventArgs
				{
					Desc = "UpgradeFail"
				};
				OnAscentFrmEvnet?.Invoke(null, e2);
			});
			break;
		case InfoType.procesInfo:
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				switch (e.upgState)
				{
				case UpgState.SendFileData:
					if (progress1.Value < 0.65f)
					{
						Progress obj2 = progress1;
						obj2.Value += 0.003f;
					}
					break;
				case UpgState.UpgradeStatus:
					if (progress1.Value < 0.95f)
					{
						Progress obj = progress1;
						obj.Value += 0.005f;
					}
					break;
				}
				((Control)lab_upgDesc).Text = e.Desc;
			});
			break;
		case InfoType.ctrlSign:
		case InfoType.warm:
		case InfoType.crash:
			break;
		}
	}

	private void UpgFSM_OnAsceDevRecInfo(uint arg1, ResAscentInfo arg2, object arg3)
	{
		if (((Control)this).IsDisposed || ((Control)this).Disposing || !((Control)this).IsHandleCreated)
		{
			return;
		}
		try
		{
			((Control)this).BeginInvoke((Delegate)(Action)delegate
			{
				_currAsceDevInfo = arg2;
				((IControl)uploadDragger1).Enabled = true;
				UpdateDeviceInfo(arg2.DevName, arg2.FWVers, arg2.SN, arg2.HWVers, arg2.MCUTemp);
				ReloadProductImg();
			});
		}
		catch (InvalidOperationException)
		{
		}
	}

	private void UpgFSM_OnUpgradeFail(string arg1, string arg2, Color arg3)
	{
		if (((Control)this).IsDisposed || ((Control)this).Disposing || !((Control)this).IsHandleCreated)
		{
			return;
		}
		try
		{
			((Control)this).BeginInvoke((Delegate)(Action)delegate
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				using (CommModalFrm commModalFrm = new CommModalFrm())
				{
					commModalFrm.SetAllTxt(arg1, arg2, Color.Red);
					((Form)commModalFrm).ShowDialog();
				}
				progress1.State = (TType)4;
				((Control)lab_upgDesc).Text = arg2;
				pictureBox2.Image = (Image)(object)Resources.升级失败;
				progress1.Fill = Color.FromArgb(255, 63, 63);
				((IControl)btn_Disconnect).Enabled = true;
				int result = 0;
				int.TryParse(_currAsceDevInfo.UsbInfo.VID, NumberStyles.HexNumber, null, out result);
				if (result != 7542 && result != 7541)
				{
					((IControl)uploadDragger1).Enabled = true;
				}
				WriteLog.WriteLogFileToUI("升级失败,err=" + arg2, Color.Red);
			});
		}
		catch (InvalidOperationException)
		{
		}
	}

	public void SubscribeUpgEvent()
	{
		_upgradeFsm.SerialConnectedStateChange += UsbFSM_SerialConneStateChange;
		GD.Inst.UpgFSM.OnUpgProcHappenEvent += UpgFSM_OnUpgProcHappenEvent;
		UpgradeProcessFSM upgFSM = GD.Inst.UpgFSM;
		upgFSM.OnAsceDevRecInfo = (Action<uint, ResAscentInfo, object>)Delegate.Combine(upgFSM.OnAsceDevRecInfo, new Action<uint, ResAscentInfo, object>(UpgFSM_OnAsceDevRecInfo));
		UpgradeProcessFSM upgFSM2 = GD.Inst.UpgFSM;
		upgFSM2.OnUpgradeFail = (Action<string, string, Color>)Delegate.Combine(upgFSM2.OnUpgradeFail, new Action<string, string, Color>(UpgFSM_OnUpgradeFail));
	}

	public void UnsubscribeUpgEvent()
	{
		try
		{
			if (_upgradeFsm != null)
			{
				_upgradeFsm.SerialConnectedStateChange -= UsbFSM_SerialConneStateChange;
				_upgradeFsm = null;
			}
			if (GD.Inst.UpgFSM != null)
			{
				GD.Inst.UpgFSM.OnUpgProcHappenEvent -= UpgFSM_OnUpgProcHappenEvent;
				UpgradeProcessFSM upgFSM = GD.Inst.UpgFSM;
				upgFSM.OnAsceDevRecInfo = (Action<uint, ResAscentInfo, object>)Delegate.Remove(upgFSM.OnAsceDevRecInfo, new Action<uint, ResAscentInfo, object>(UpgFSM_OnAsceDevRecInfo));
				UpgradeProcessFSM upgFSM2 = GD.Inst.UpgFSM;
				upgFSM2.OnUpgradeFail = (Action<string, string, Color>)Delegate.Remove(upgFSM2.OnUpgradeFail, new Action<string, string, Color>(UpgFSM_OnUpgradeFail));
				GD.Inst.UpgFSM.Dispose();
			}
		}
		catch (Exception)
		{
		}
	}

	private void ConnectDevice()
	{
		if (_upgradeSession == null || !GD.Inst.SerialPortSessions.TryGetTransport(_upgradeSession, out var transport))
		{
			ShowUpgradeSessionUnavailable();
		}
		else if (transport is LegacyFsmSessionTransport { Fsm: not null } legacyFsmSessionTransport)
		{
			_upgradeFsm = legacyFsmSessionTransport.Fsm;
			SetDeviceConnectedState(isConnected: true);
			if (GD.Inst.UpgFSM != null)
			{
				GD.Inst.UpgFSM.OnUpgProcHappenEvent -= UpgFSM_OnUpgProcHappenEvent;
				UpgradeProcessFSM upgFSM = GD.Inst.UpgFSM;
				upgFSM.OnAsceDevRecInfo = (Action<uint, ResAscentInfo, object>)Delegate.Remove(upgFSM.OnAsceDevRecInfo, new Action<uint, ResAscentInfo, object>(UpgFSM_OnAsceDevRecInfo));
				UpgradeProcessFSM upgFSM2 = GD.Inst.UpgFSM;
				upgFSM2.OnUpgradeFail = (Action<string, string, Color>)Delegate.Remove(upgFSM2.OnUpgradeFail, new Action<string, string, Color>(UpgFSM_OnUpgradeFail));
				GD.Inst.UpgFSM.Dispose();
			}
			GD.Inst.UpgFSM = new UpgradeProcessFSM(_upgradeFsm, _currAsceDevInfo);
			SubscribeUpgEvent();
			((IControl)uploadDragger1).Enabled = true;
		}
		else
		{
			ShowUpgradeSessionUnavailable();
		}
	}

	private void ShowUpgradeSessionUnavailable()
	{
		SetDeviceConnectedState(isConnected: false);
		string text = Lang.T("ascent_upgrade.error_prompt");
		string text2 = Lang.T("ascent_upgrade.unconnected_device");
		Notification.error(Frm, text, text2, (TAlignFrom)10, (Font)null, (int?)0);
	}

	public void UpdateRTB(string hex, int leng, ArProtocolHeader hea, UpgState Step, byte[] bytes)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
		});
	}

	public void UpdateDeviceInfo(string devname, string firmwareInfo, string sn, string hardwareVersion, int temptrue)
	{
		try
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				WriteLog.WriteLogFileToUI("刷新设备信息一次", Color.Black);
				if (!string.IsNullOrEmpty(devname))
				{
					AscentUpdataFrm ascentUpdataFrm = this;
					string deviceName = (((Control)lab_devName).Text = devname.Replace('_', ' '));
					ascentUpdataFrm._deviceName = deviceName;
				}
				string pattern = "V\\d+\\.\\d+-\\d+\\.\\d+";
				Match match2 = Regex.Match(hardwareVersion, pattern);
				string text3 = (match2.Success ? match2.Groups[0].Value : hardwareVersion);
				string text4 = text3;
				string text5 = FormatFirmwareDisplayVersion(firmwareInfo);
				lab_hardwareVer.Suffix = " : " + text4;
				lab_firmwareVer.Suffix = " : " + text5;
				lab_sn.Suffix = " : " + sn;
				lab_tempture.Suffix = " : " + temptrue + "℃";
				Match match3 = Regex.Match(firmwareInfo, "(\\d+_\\d+_\\d+)$");
				if (firmwareInfo.Length < 12)
				{
					firmwareInfo = "unRecogfirmwareInfo";
				}
				string text6 = firmwareInfo.Remove(12);
				if (match3.Success)
				{
					text6 = firmwareInfo.Remove(match3.Index - 1);
				}
				((Control)lab_FWTips).Text = Lang.T("ascent_upgrade.select_firmware_filename", text6 + "_xxx");
			});
			string text = FormatFirmwareDisplayVersion(firmwareInfo);
			if (!string.Equals(text, firmwareInfo, StringComparison.Ordinal))
			{
				_perVer = ("V" + text).Replace("_", ".");
				return;
			}
			Match match = Regex.Match(firmwareInfo, "(\\d+_\\d+_\\d+)");
			if (!match.Success)
			{
				_perVer = firmwareInfo;
			}
			else
			{
				_perVer = ("V" + match.Groups[1].Value).Replace("_", ".");
			}
		}
		catch (Exception)
		{
		}
	}

	private string FormatFirmwareDisplayVersion(string firmwareInfo)
	{
		if (string.IsNullOrWhiteSpace(firmwareInfo))
		{
			return firmwareInfo;
		}
		Match match = Regex.Match(firmwareInfo.Trim(), "\\d+(?:_\\d+)+$");
		return match.Success ? match.Groups[0].Value : firmwareInfo;
	}

	private void _upg_SigUpgradeProcessHint(string arg1, UpgState arg2)
	{
		WriteLog.WriteLogFileToUI("SigUpgrade=" + arg1 + "\r\nUpgState=" + arg2, Color.Red);
	}

	private void _upg_LogMessage(string obj)
	{
		WriteLog.WriteLogFileToUI("LogMessage=" + obj, Color.DarkSlateGray);
	}

	private void lab_sn_Click(object sender, EventArgs e)
	{
	}

	private void lab_hardwareVer_Click(object sender, EventArgs e)
	{
	}

	private void lab_firmwareVer_Click(object sender, EventArgs e)
	{
	}

	private void lab_tempture_Click(object sender, EventArgs e)
	{
	}

	private void btn_closeTips_Click(object sender, EventArgs e)
	{
	}

	private void btn_closeTips_MouseClick(object sender, MouseEventArgs e)
	{
		Button obj = button2;
		Label obj2 = lab_FWTips;
		bool flag = (((IControl)btn_closeTips).Visible = false);
		bool visible = (((Control)obj2).Visible = flag);
		((IControl)obj).Visible = visible;
		((Control)pageHeader4).BackColor = Color.FromArgb(38, 41, 43);
	}

	private void UsbFSM_SerialConneStateChange(bool b, string arg1, int arg2)
	{
		if (!_isStartUpg)
		{
			if (!b)
			{
			}
			AscentFrmEventArgs e = new AscentFrmEventArgs
			{
				Desc = (b ? "Connect" : "Disconnect"),
				PortName = arg1,
				IsOpen = b
			};
			OnAscentFrmEvnet?.Invoke(null, e);
		}
	}

	private void AscentUpdataFrm_FormClosing(object sender, FormClosingEventArgs e)
	{
		((Control)this).Hide();
		((CancelEventArgs)(object)e).Cancel = true;
	}

	private void uploadDragger1_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		try
		{
			OpenFileDialog val = new OpenFileDialog();
			try
			{
				((FileDialog)val).Title = Lang.T("ascent_upgrade.open_file_title");
				((FileDialog)val).InitialDirectory = "C:";
				((FileDialog)val).Filter = Lang.T("ascent_upgrade.file_filter");
				((FileDialog)val).FilterIndex = 1;
				((FileDialog)val).RestoreDirectory = true;
				val.Multiselect = false;
				if ((int)((CommonDialog)val).ShowDialog() == 1)
				{
					string fileName = ((FileDialog)val).FileName;
					HandleFirmwareFileSelected(fileName);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("AscentUpdataFrm.DraggerClick error，desc=" + ex.Message, Color.Red);
		}
	}

	private void uploadDragger1_DragChanged(object sender, StringsEventArgs e)
	{
		try
		{
			string[] value = ((VEventArgs<string[]>)(object)e).Value;
			string path = value[0];
			HandleFirmwareFileSelected(path);
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("AscentUpdataFrm.DragChanged error，desc=" + ex.Message, Color.Red);
		}
	}

	private void HandleFirmwareFileSelected(string path)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		FileAttributes attributes = File.GetAttributes(path);
		if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
		{
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("ascent_upgrade.drag_firmware_only");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
		}
		else if (SelectWrongFWFilePrompt(path))
		{
			_firmwarePath = path;
			((Control)lab_filepath).Text = path;
			((IControl)btn_startUpg).Enabled = true;
			GD.Inst.UpgFSM.SetFile(path);
			UpdateSelectedFirmwareVersion(path);
		}
	}

	private void UpdateSelectedFirmwareVersion(string path)
	{
		string text = ExtractVersion(Path.GetFileName(path));
		if (!string.IsNullOrEmpty(text))
		{
			_currVer = "V" + text;
			return;
		}
		Match match = Regex.Match(path, "(\\d+[\\._]\\d+[\\._]\\d+\\.img)");
		if (!match.Success)
		{
			_currVer = Lang.T("ascent_upgrade.unrecognized_firmware_version");
		}
		else if (match.Groups[1].Value.Contains("_"))
		{
			_currVer = ("V" + match.Groups[1].Value).Replace("_", ".").Replace(".img", "");
		}
		else if (match.Groups[1].Value.Contains("."))
		{
			_currVer = ("V" + match.Groups[1].Value).Replace(".img", "");
		}
	}

	private void uploadDragger1_Click(object sender, EventArgs e)
	{
	}

	private async void btnstartUpg_Click(object sender, EventArgs e)
	{
		int tempi = JudgePerVersAndCurrVers(out var res);
		if (res < 0 && tempi == 1)
		{
			CommModalFrm commfrm = new CommModalFrm();
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("ascent_upgrade.confirm_downgrade");
			commfrm.SetAllTxt(title, desc);
			((Form)commfrm).ShowDialog();
			if ((int)commfrm.FrmResult != 1)
			{
				return;
			}
		}
		StartUpgAscentModalFrm md = new StartUpgAscentModalFrm();
		((Form)md).ShowDialog();
		if ((int)md.result == 1)
		{
			ResetCrtl();
			AscentFrmEventArgs afe = new AscentFrmEventArgs
			{
				Desc = "StartUpgrade"
			};
			OnAscentFrmEvnet?.Invoke(null, afe);
			if (!GD.Inst.IsUseNewAscentUpg)
			{
				GD.Inst.UpgFSM.Start();
			}
		}
	}

	private void OnUpgradeCompleted(bool success, string msg)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (success)
		{
			progress1.Value = 1f;
			progress1.State = (TType)1;
			progress1.Fill = Color.FromArgb(43, 164, 113);
			pictureBox2.Image = (Image)(object)Resources.升级成功;
			WriteLog.WriteLogFileToUI("升级完成", Color.Green);
		}
		else
		{
			progress1.State = (TType)4;
			pictureBox2.Image = (Image)(object)Resources.升级失败;
			WriteLog.WriteLogFileToUI("升级失败: " + msg, Color.Red);
			MessageBox.Show(msg, Lang.T("ascent_upgrade.upgrade_failed"), (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
	}

	private void ResetCrtl()
	{
		_isStartUpg = true;
		((IControl)grpan_process).Visible = true;
		progress1.Value = 0.05f;
		((Control)lab_upgDesc).Text = Lang.T("ascent_upgrade.start_firmware_upgrade");
		progress1.State = (TType)0;
		progress1.Fill = Color.FromArgb(255, 233, 0);
		pictureBox2.Image = (Image)(object)Resources.升级中;
		((Control)lab_versChange).Text = _perVer + " >> " + _currVer;
		Button obj = btn_Disconnect;
		UploadDragger obj2 = uploadDragger1;
		bool flag = (((IControl)btn_startUpg).Enabled = false);
		bool enabled = (((IControl)obj2).Enabled = flag);
		((IControl)obj).Enabled = enabled;
	}

	private void btn_Disconnect_Click(object sender, EventArgs e)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Invalid comparison between Unknown and I4
		if (!_isDeviceConnected)
		{
			ConnectDevice();
			return;
		}
		CommModalFrm commModalFrm = new CommModalFrm();
		string title = Lang.T("ascent_upgrade.disconnect_title");
		string desc = Lang.T("ascent_upgrade.disconnect_confirm_desc");
		string btnok = Lang.T("common.btn_confirm");
		string btnCan = Lang.T("common.btn_cancel");
		commModalFrm.SetAllTxt(title, desc, btnok, btnCan);
		((Form)commModalFrm).ShowDialog();
		if ((int)commModalFrm.FrmResult == 1)
		{
			UnsubscribeUpgEvent();
			ReleaseUpgradeSessionAsync(SerialPortReleaseReason.OwnerRequested);
			SetDeviceConnectedState(isConnected: false);
			AscentFrmEventArgs e2 = new AscentFrmEventArgs
			{
				Desc = "ManualCloseDevice"
			};
			OnAscentFrmEvnet?.Invoke(null, e2);
			((Control)this).Hide();
		}
	}

	private void btn_question_Click(object sender, EventArgs e)
	{
		((IControl)uploadDragger1).Enabled = true;
	}

	private void btn_loadFromPC_Click(object sender, EventArgs e)
	{
	}

	private void DisConnectDeviceResetFrm(bool isConn)
	{
		SetDeviceConnectedState(isConn);
	}

	private void SetDeviceConnectedState(bool isConnected)
	{
		_isDeviceConnected = isConnected;
		if (!isConnected)
		{
			((Control)btn_Disconnect).Text = Lang.T("ascent_upgrade.btn_reconnect");
			btn_Disconnect.IconRatio = 0f;
		}
		else
		{
			((Control)btn_Disconnect).Text = Lang.T("ascent_upgrade.btn_disconnect");
			btn_Disconnect.IconRatio = 0.8f;
		}
		UploadDragger obj = uploadDragger1;
		bool enabled = (((IControl)btn_startUpg).Visible = isConnected);
		((IControl)obj).Enabled = enabled;
	}

	public int JudgeDeviceAndVers(out int res)
	{
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		res = 0;
		string text = _currAsceDevInfo.FWVers.ToLower();
		if (!text.Contains("sky") || !text.Contains("z8") || !text.Contains("z40"))
		{
			WriteLog.WriteLogFileToUI("不是天空端，不需要判断版本过低", Color.DimGray);
			return 1;
		}
		Match match = Regex.Match(text, "(\\d+)_(\\d+)_(\\d+)$");
		if (!match.Success)
		{
			WriteLog.WriteLogFileToUI("匹配失败", Color.Red);
			return 2;
		}
		List<int> list = new List<int>();
		List<string> list2 = new List<string>();
		for (int i = 0; i < match.Groups.Count; i++)
		{
			int result = 0;
			list2.Add(match.Groups[i].Value);
			if (int.TryParse(match.Groups[i].Value, out result))
			{
				list.Add(result);
			}
		}
		Version version = new Version(match.Groups[0].Value.Replace('_', '.'));
		Version version2 = new Version("16.4.4");
		Version version3 = new Version(_currVer.Replace("V", ""));
		int num = version3.CompareTo(version2);
		if (num <= 0)
		{
			return 0;
		}
		int num2 = version.CompareTo(version2);
		if (num2 >= 0)
		{
			return 0;
		}
		int num3 = version2.CompareTo(version3);
		if (num3 >= 0)
		{
			return 0;
		}
		res = num3;
		CommModalFrm commModalFrm = new CommModalFrm();
		commModalFrm.SetAllTxt(Lang.T("ascent_upgrade.error_prompt"), Lang.T("ascent_upgrade.version_too_low_select_1644"), Lang.T("common.btn_confirm"));
		((Form)commModalFrm).ShowDialog();
		return 0;
	}

	private int JudgePerVersAndCurrVers(out int res)
	{
		res = 0;
		try
		{
			string input = _currAsceDevInfo.FWVers.ToLower();
			Match match = Regex.Match(input, "(\\d+)_(\\d+)_(\\d+)$");
			if (!match.Success)
			{
				WriteLog.WriteLogFileToUI("匹配失败", Color.Red);
				return 2;
			}
			List<int> list = new List<int>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < match.Groups.Count; i++)
			{
				int result = 0;
				list2.Add(match.Groups[i].Value);
				if (int.TryParse(match.Groups[i].Value, out result))
				{
					list.Add(result);
				}
			}
			Version value = new Version(match.Groups[0].Value.Replace('_', '.'));
			Version version = new Version(_currVer.Replace("V", "").Replace('_', '.'));
			int num = version.CompareTo(value);
			if (num < 0)
			{
				res = num;
				return 1;
			}
			return 0;
		}
		catch (Exception)
		{
			return 9;
		}
	}

	private string GetVersion(string input, bool b)
	{
		string input2 = input.Replace(".img", "");
		input2 = Regex.Replace(input2, "[\\(\\[（].*?[\\)\\]）]|-\\d+$", "");
		string pattern = "_z(\\d+)_(\\d+)";
		Match match = Regex.Match(input2, pattern);
		int result;
		if (match.Success)
		{
			string pattern2 = "^(.*?)_z\\d+_\\d+$";
			Match match2 = Regex.Match(input2, pattern2);
			if (match2.Success)
			{
				string value = match2.Groups[1].Value;
				string[] array = value.Split(new char[1] { '_' });
				if (array.Length >= 3)
				{
					string text = array[^3];
					string text2 = array[^2];
					string text3 = array[^1];
					if (int.TryParse(text, out result) && int.TryParse(text2, out result) && int.TryParse(text3, out result))
					{
						string value2 = match.Groups[2].Value;
						return text + "_" + text2 + "_" + text3 + "_" + value2;
					}
				}
			}
		}
		else
		{
			string[] array2 = input2.Split(new char[1] { '_' });
			if (array2.Length >= 3)
			{
				string text4 = array2[^1];
				string text5 = array2[^2];
				string text6 = array2[^3];
				if (int.TryParse(text4, out result) && int.TryParse(text5, out result) && int.TryParse(text6, out result))
				{
					return text6 + "_" + text5 + "_" + text4;
				}
			}
		}
		return "";
	}

	private string ExtractVersion(string fileName)
	{
		string pattern = ".*?(\\d+)_(\\d+)_(\\d+)(?:(?:_z8_|_z40_|_hub_)(\\d+)|)\\.img$";
		Match match = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase);
		if (match.Success)
		{
			string value = match.Groups[1].Value;
			string value2 = match.Groups[2].Value;
			string value3 = match.Groups[3].Value;
			string text = (match.Groups[4].Success ? match.Groups[4].Value : string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				return value + "." + value2 + "." + value3 + "." + text;
			}
			return value + "." + value2 + "." + value3;
		}
		return string.Empty;
	}

	private bool SelectWrongFWFilePrompt(string path)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Invalid comparison between Unknown and I4
		CommModalFrm commModalFrm = new CommModalFrm();
		try
		{
			if (path.Length > 200)
			{
				string title = Lang.T("common.title_warning");
				string desc = Lang.T("ascent_upgrade.firmware_path_too_long");
				commModalFrm.SetAllTxt(title, desc, Color.Red);
				((Form)commModalFrm).ShowDialog();
				return false;
			}
			string text = path.Replace(".img", "");
			string fWVers = _currAsceDevInfo.FWVers;
			string devName = _currAsceDevInfo.DevName;
			Match match = Regex.Match(_currAsceDevInfo.FWVers, "(\\d+\\d+\\d+)$");
			string value = _currAsceDevInfo.FWVers.Remove(12);
			if (match.Success)
			{
				value = _currAsceDevInfo.FWVers.Remove(match.Index - 1).Replace("Ascent", "");
			}
			if (!text.Contains(value))
			{
				string title = Lang.T("common.title_warning");
				string desc = Lang.T("ascent_upgrade.firmware_name_mismatch");
				commModalFrm.SetAllTxt(title, desc);
				((Form)commModalFrm).ShowDialog();
				if ((int)commModalFrm.FrmResult != 1)
				{
					return false;
				}
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public void GetCurrentUsbDevInfo(out UsbDevInfo devInfo)
	{
		devInfo = _currAsceDevInfo.UsbInfo;
	}

	public void OldProtocolSendFinddev()
	{
		GD.Inst.UpgFSM.Send_FindDevice();
	}

	public void NewProtocolSendFinddev()
	{
	}

	public void NewProtocolSendRebootClean()
	{
	}

	public void ReloadFont()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Expected O, but got Unknown
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 18f);
			((Control)lab_devName).Font = font;
			Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 10f);
			Label obj = lab_FWTips;
			Button obj2 = btn_Disconnect;
			Label obj3 = lab_filepath;
			Label obj4 = lab_hardwareVer;
			Label obj5 = lab_firmwareVer;
			Label obj6 = lab_tempture;
			Progress obj7 = progress1;
			Label obj8 = lab_sn;
			UploadDragger obj9 = uploadDragger1;
			Font val2 = (((Control)btn_startUpg).Font = val);
			Font val4 = (((Control)obj9).Font = val2);
			Font val6 = (((Control)obj8).Font = val4);
			Font val8 = (((Control)obj7).Font = val6);
			Font val10 = (((Control)obj6).Font = val8);
			Font val12 = (((Control)obj5).Font = val10);
			Font val14 = (((Control)obj4).Font = val12);
			Font val16 = (((Control)obj3).Font = val14);
			Font font2 = (((Control)obj2).Font = val16);
			((Control)obj).Font = font2;
			Font font3 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
			((Control)lab_versChange).Font = font3;
			Font font4 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 8f);
			((Control)lab_upgDesc).Font = font4;
		});
	}

	public void ReloadLang()
	{
		((Control)lab_hardwareVer).Text = Lang.T("ascent_upgrade.hardware");
		((Control)lab_firmwareVer).Text = Lang.T("ascent_upgrade.firmware");
		((Control)lab_filepath).Text = Lang.T("ascent_upgrade.drag_firmware_hint");
		lab_filepath.Suffix = Lang.T("ascent_upgrade.select_firmware");
		((Control)btn_startUpg).Text = Lang.T("ascent_upgrade.btn_upgrade");
		SetDeviceConnectedState(_isDeviceConnected);
		((Control)lab_tempture).Text = Lang.T("ascent_upgrade.temperature");
		((Control)lab_selectFile).Text = Lang.T("ascent_upgrade.select_file");
	}

	private void lab_tempture_MouseDoubleClick(object sender, MouseEventArgs e)
	{
	}

	public void Dispose()
	{
		UnsubscribeUpgEvent();
		ReleaseUpgradeSessionAsync(SerialPortReleaseReason.OwnerRequested);
	}

	private async Task ReleaseUpgradeSessionAsync(SerialPortReleaseReason reason)
	{
		SerialPortSessionHandle session = _upgradeSession;
		if (session == null)
		{
			return;
		}
		try
		{
			SerialPortReleaseResult result = await GD.Inst.SerialPortSessions.ReleaseAsync(session, reason);
			if (result.Succeeded || result.FailureReason == SerialPortSessionFailureReason.StaleHandle)
			{
				_upgradeSession = null;
				return;
			}
			WriteLog.WriteLogFileToUI(string.Format("释放升级串口会话未完成，port={0}, reason={1}, owner={2}, state={3}", new object[4] { session.PortName, result.FailureReason, result.CurrentOwner, result.CurrentState }), Color.DarkOrange);
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("释放升级串口会话异常，port=" + session.PortName + ", desc=" + ex.Message, Color.Red);
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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
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
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
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
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Expected O, but got Unknown
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Expected O, but got Unknown
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Expected O, but got Unknown
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcc: Expected O, but got Unknown
		//IL_0c17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cad: Expected O, but got Unknown
		//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da3: Expected O, but got Unknown
		//IL_0e11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f45: Expected O, but got Unknown
		//IL_0fce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_106a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1074: Expected O, but got Unknown
		//IL_1082: Unknown result type (might be due to invalid IL or missing references)
		//IL_108c: Expected O, but got Unknown
		//IL_10c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cc: Expected O, but got Unknown
		//IL_110d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c3: Expected O, but got Unknown
		//IL_126d: Unknown result type (might be due to invalid IL or missing references)
		//IL_133b: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ae: Expected O, but got Unknown
		//IL_13f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_14da: Unknown result type (might be due to invalid IL or missing references)
		//IL_1669: Unknown result type (might be due to invalid IL or missing references)
		//IL_1715: Unknown result type (might be due to invalid IL or missing references)
		//IL_187a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1884: Expected O, but got Unknown
		//IL_18da: Unknown result type (might be due to invalid IL or missing references)
		//IL_1986: Unknown result type (might be due to invalid IL or missing references)
		//IL_1990: Expected O, but got Unknown
		//IL_19d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e1: Expected O, but got Unknown
		//IL_1a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b18: Expected O, but got Unknown
		//IL_1ba5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c59: Expected O, but got Unknown
		//IL_1caf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dac: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db6: Expected O, but got Unknown
		//IL_1e0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f11: Expected O, but got Unknown
		//IL_1f5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_202a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2034: Expected O, but got Unknown
		//IL_2037: Unknown result type (might be due to invalid IL or missing references)
		//IL_2050: Unknown result type (might be due to invalid IL or missing references)
		grpan_process = new GridPanel();
		grpan_procDesc = new GridPanel();
		stackPanel1 = new StackPanel();
		lab_upgDesc = new Label();
		lab_versChange = new Label();
		progress1 = new Progress();
		pictureBox2 = new PictureBox();
		grpan_main = new GridPanel();
		stackPanel2 = new StackPanel();
		label1 = new Label();
		grpan_upgrade = new GridPanel();
		pageHeader3 = new PageHeader();
		lab_selectFile = new Label();
		btn_startUpg = new Button();
		uploadDragger1 = new UploadDragger();
		lab_filepath = new Label();
		pageHeader4 = new PageHeader();
		btn_closeTips = new Button();
		lab_FWTips = new Label();
		button2 = new Button();
		pageHeader1 = new PageHeader();
		pictureBox1 = new PictureBox();
		divider1 = new Divider();
		lab_tempture = new Label();
		lab_sn = new Label();
		btn_Disconnect = new Button();
		lab_firmwareVer = new Label();
		lab_hardwareVer = new Label();
		lab_devName = new Label();
		((Control)grpan_process).SuspendLayout();
		((Control)grpan_procDesc).SuspendLayout();
		((Control)stackPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox2).BeginInit();
		((Control)grpan_main).SuspendLayout();
		((Control)stackPanel2).SuspendLayout();
		((Control)grpan_upgrade).SuspendLayout();
		((Control)pageHeader3).SuspendLayout();
		((Control)uploadDragger1).SuspendLayout();
		((Control)pageHeader4).SuspendLayout();
		((Control)pageHeader1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((ContainerPanel)grpan_process).Back = Color.FromArgb(33, 36, 39);
		((Control)grpan_process).BackColor = Color.FromArgb(33, 36, 39);
		((Control)grpan_process).Controls.Add((Control)(object)grpan_procDesc);
		((Control)grpan_process).Controls.Add((Control)(object)pictureBox2);
		((Control)grpan_process).Dock = (DockStyle)5;
		grpan_upgrade.SetIndex((Control)(object)grpan_process, 3);
		((Control)grpan_process).Location = new Point(5, 172);
		((Control)grpan_process).Margin = new Padding(5);
		((Control)grpan_process).Name = "grpan_process";
		((Control)grpan_process).Size = new Size(800, 102);
		grpan_process.Span = "10% 90%;";
		((Control)grpan_process).TabIndex = 55;
		((Control)grpan_process).Text = "gripan_process";
		((Control)grpan_procDesc).Controls.Add((Control)(object)stackPanel1);
		((Control)grpan_procDesc).Controls.Add((Control)(object)progress1);
		((Control)grpan_procDesc).Location = new Point(80, 0);
		((Control)grpan_procDesc).Margin = new Padding(0);
		((Control)grpan_procDesc).Name = "grpan_procDesc";
		((Control)grpan_procDesc).Size = new Size(720, 102);
		grpan_procDesc.Span = "100%;100%;-55% 45%";
		((Control)grpan_procDesc).TabIndex = 1;
		((Control)grpan_procDesc).Text = "gridPanel6";
		stackPanel1.Controls.Add((Control)(object)lab_upgDesc);
		stackPanel1.Controls.Add((Control)(object)lab_versChange);
		((Control)stackPanel1).Dock = (DockStyle)5;
		((Control)stackPanel1).Location = new Point(0, 0);
		((Control)stackPanel1).Margin = new Padding(0);
		((Control)stackPanel1).Name = "stackPanel1";
		((Control)stackPanel1).Size = new Size(720, 56);
		((Control)stackPanel1).TabIndex = 5;
		((Control)stackPanel1).Text = "stackPanel1";
		((Control)lab_upgDesc).Dock = (DockStyle)3;
		((Control)lab_upgDesc).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_upgDesc).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_upgDesc).Location = new Point(210, 0);
		((Control)lab_upgDesc).Margin = new Padding(0);
		((Control)lab_upgDesc).Name = "lab_upgDesc";
		((Control)lab_upgDesc).Size = new Size(467, 56);
		((Control)lab_upgDesc).TabIndex = 37;
		((Control)lab_upgDesc).Text = "正在升级";
		lab_upgDesc.TextAlign = (ContentAlignment)16;
		((Control)lab_versChange).Dock = (DockStyle)3;
		((Control)lab_versChange).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_versChange).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_versChange).Location = new Point(0, 0);
		((Control)lab_versChange).Margin = new Padding(0);
		((Control)lab_versChange).Name = "lab_versChange";
		((Control)lab_versChange).Size = new Size(210, 56);
		((Control)lab_versChange).TabIndex = 8;
		((Control)lab_versChange).Text = "V1.1.1--V2.1.1";
		lab_versChange.TextAlign = (ContentAlignment)16;
		progress1.Back = Color.FromArgb(99, 101, 103);
		((Control)progress1).Dock = (DockStyle)5;
		progress1.Fill = Color.FromArgb(255, 233, 0);
		progress1.ForeColor = Color.White;
		((IControl)progress1).HandCursor = Cursors.Default;
		grpan_procDesc.SetIndex((Control)(object)progress1, 3);
		((Control)progress1).Location = new Point(10, 56);
		((Control)progress1).Margin = new Padding(10, 0, 10, 0);
		((Control)progress1).Name = "progress1";
		((Control)progress1).Size = new Size(700, 46);
		((Control)progress1).TabIndex = 4;
		((Control)progress1).TabStop = false;
		((Control)progress1).Text = "";
		progress1.Value = 0.5f;
		progress1.ValueRatio = 1f;
		((Control)pictureBox2).Dock = (DockStyle)5;
		pictureBox2.Image = (Image)(object)Resources.升级中;
		((Control)pictureBox2).Location = new Point(10, 30);
		((Control)pictureBox2).Margin = new Padding(10, 30, 10, 20);
		((Control)pictureBox2).Name = "pictureBox2";
		((Control)pictureBox2).Size = new Size(60, 52);
		pictureBox2.SizeMode = (PictureBoxSizeMode)4;
		pictureBox2.TabIndex = 0;
		pictureBox2.TabStop = false;
		((ContainerPanel)grpan_main).Back = Color.FromArgb(38, 41, 43);
		((Control)grpan_main).BackColor = Color.FromArgb(38, 41, 43);
		((Control)grpan_main).Controls.Add((Control)(object)stackPanel2);
		((Control)grpan_main).Controls.Add((Control)(object)grpan_upgrade);
		((Control)grpan_main).Controls.Add((Control)(object)pageHeader4);
		((Control)grpan_main).Controls.Add((Control)(object)pageHeader1);
		((Control)grpan_main).Dock = (DockStyle)5;
		((IControl)grpan_main).HandCursor = Cursors.Default;
		((Control)grpan_main).Location = new Point(5, 5);
		((Control)grpan_main).Margin = new Padding(0);
		((Control)grpan_main).Name = "grpan_main";
		((ContainerPanel)grpan_main).Radius = 10;
		((Control)grpan_main).Size = new Size(830, 630);
		grpan_main.Span = "100%;100%;100%;100%;\r\n-25% 7% 45% 23%";
		((Control)grpan_main).TabIndex = 56;
		((Control)grpan_main).Text = "gridPanel1";
		stackPanel2.Controls.Add((Control)(object)label1);
		((Control)stackPanel2).Location = new Point(5, 490);
		((Control)stackPanel2).Margin = new Padding(5);
		((Control)stackPanel2).Name = "stackPanel2";
		((Control)stackPanel2).Size = new Size(820, 135);
		((Control)stackPanel2).TabIndex = 59;
		((Control)stackPanel2).Text = "stackPanel2";
		stackPanel2.Vertical = true;
		((Control)label1).Dock = (DockStyle)1;
		((Control)label1).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label1).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)label1).Location = new Point(0, 0);
		((Control)label1).Margin = new Padding(0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(820, 32);
		((Control)label1).TabIndex = 38;
		((Control)label1).Text = "正在升级";
		label1.TextAlign = (ContentAlignment)2;
		((Control)label1).Visible = false;
		((Control)grpan_upgrade).BackColor = Color.FromArgb(46, 49, 51);
		((ContainerPanel)grpan_upgrade).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)grpan_upgrade).BorderWidth = 2f;
		((Control)grpan_upgrade).Controls.Add((Control)(object)pageHeader3);
		((Control)grpan_upgrade).Controls.Add((Control)(object)uploadDragger1);
		((Control)grpan_upgrade).Controls.Add((Control)(object)grpan_process);
		((Control)grpan_upgrade).Dock = (DockStyle)5;
		((IControl)grpan_upgrade).HandCursor = Cursors.Default;
		((Control)grpan_upgrade).Location = new Point(10, 202);
		((Control)grpan_upgrade).Margin = new Padding(10, 0, 10, 5);
		((Control)grpan_upgrade).Name = "grpan_upgrade";
		((Control)grpan_upgrade).Size = new Size(810, 279);
		grpan_upgrade.Span = "100%;100%;100%;-15% 45% 40%";
		((Control)grpan_upgrade).TabIndex = 58;
		((Control)grpan_upgrade).Text = "grpan_upgrade";
		((Control)pageHeader3).BackColor = Color.Transparent;
		((Control)pageHeader3).Controls.Add((Control)(object)lab_selectFile);
		((Control)pageHeader3).Controls.Add((Control)(object)btn_startUpg);
		((Control)pageHeader3).Dock = (DockStyle)5;
		((Control)pageHeader3).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)pageHeader3).ForeColor = Color.FromArgb(51, 51, 51);
		grpan_upgrade.SetIndex((Control)(object)pageHeader3, 1);
		((Control)pageHeader3).Location = new Point(10, 0);
		((Control)pageHeader3).Margin = new Padding(10, 0, 10, 0);
		((Control)pageHeader3).Name = "pageHeader3";
		((Control)pageHeader3).Padding = new Padding(0, 3, 0, 0);
		((Control)pageHeader3).Size = new Size(790, 42);
		((Control)pageHeader3).TabIndex = 56;
		((Control)pageHeader3).Text = "";
		((Control)lab_selectFile).Dock = (DockStyle)3;
		((Control)lab_selectFile).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_selectFile).ForeColor = Color.White;
		((Control)lab_selectFile).Location = new Point(0, 3);
		((Control)lab_selectFile).Margin = new Padding(0);
		((Control)lab_selectFile).Name = "lab_selectFile";
		((Control)lab_selectFile).Size = new Size(607, 39);
		((Control)lab_selectFile).TabIndex = 36;
		((Control)lab_selectFile).Text = "选择文件";
		lab_selectFile.TextAlign = (ContentAlignment)16;
		btn_startUpg.AutoSizeMode = (TAutoSize)1;
		btn_startUpg.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_startUpg).Dock = (DockStyle)4;
		((IControl)btn_startUpg).Enabled = false;
		((Control)btn_startUpg).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_startUpg.ForeColor = Color.FromArgb(35, 35, 35);
		btn_startUpg.IconGap = 0f;
		btn_startUpg.IconRatio = 0f;
		btn_startUpg.IconSvg = "";
		((Control)btn_startUpg).Location = new Point(717, 3);
		((Control)btn_startUpg).Margin = new Padding(0);
		((Control)btn_startUpg).Name = "btn_startUpg";
		((Control)btn_startUpg).Size = new Size(73, 30);
		((Control)btn_startUpg).TabIndex = 35;
		((Control)btn_startUpg).Text = "开始升级";
		btn_startUpg.WaveSize = 0;
		((Control)btn_startUpg).Click += btnstartUpg_Click;
		uploadDragger1.Back = Color.FromArgb(33, 36, 39);
		((Control)uploadDragger1).BackColor = Color.FromArgb(33, 36, 39);
		uploadDragger1.BorderColor = Color.FromArgb(66, 69, 71);
		uploadDragger1.BorderWidth = 2f;
		uploadDragger1.ClickHand = false;
		((IControl)uploadDragger1).ColorScheme = (TAMode)2;
		((Control)uploadDragger1).Controls.Add((Control)(object)lab_filepath);
		((Control)uploadDragger1).Dock = (DockStyle)5;
		((Control)uploadDragger1).Font = new Font("Microsoft Sans Serif", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		uploadDragger1.ForeColor = Color.FromArgb(153, 153, 153);
		((IControl)uploadDragger1).HandDragFolder = false;
		uploadDragger1.IconRatio = 0f;
		uploadDragger1.IconSvg = "";
		grpan_upgrade.SetIndex((Control)(object)uploadDragger1, 2);
		((Control)uploadDragger1).Location = new Point(10, 42);
		((Control)uploadDragger1).Margin = new Padding(10, 0, 10, 0);
		uploadDragger1.Multiselect = false;
		((Control)uploadDragger1).Name = "uploadDragger1";
		((Control)uploadDragger1).Padding = new Padding(5);
		((Control)uploadDragger1).Size = new Size(790, 126);
		((Control)uploadDragger1).TabIndex = 0;
		((Control)uploadDragger1).TabStop = false;
		((Control)uploadDragger1).Text = "";
		uploadDragger1.TextDesc = "";
		((IControl)uploadDragger1).DragChanged += new DragEventHandler(uploadDragger1_DragChanged);
		((Control)uploadDragger1).MouseClick += new MouseEventHandler(uploadDragger1_MouseClick);
		((Control)lab_filepath).BackColor = Color.Transparent;
		((Control)lab_filepath).Dock = (DockStyle)2;
		((Control)lab_filepath).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_filepath.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_filepath).Location = new Point(6, 56);
		((Control)lab_filepath).Margin = new Padding(0);
		((Control)lab_filepath).Name = "lab_filepath";
		((Control)lab_filepath).Size = new Size(778, 64);
		lab_filepath.Suffix = "    选择文件";
		lab_filepath.SuffixColor = Color.FromArgb(255, 233, 0);
		((Control)lab_filepath).TabIndex = 37;
		((Control)lab_filepath).TabStop = false;
		((Control)lab_filepath).Text = "D:\\微信下载文件\\WXWork\\1688857547687314\\Cache\\File\\2026-03\\menu_rc_mode\\menu_rc_mode.png";
		lab_filepath.TextAlign = (ContentAlignment)2;
		((Control)lab_filepath).MouseClick += new MouseEventHandler(uploadDragger1_MouseClick);
		((Control)pageHeader4).BackColor = Color.FromArgb(49, 51, 41);
		((Control)pageHeader4).Controls.Add((Control)(object)btn_closeTips);
		((Control)pageHeader4).Controls.Add((Control)(object)lab_FWTips);
		((Control)pageHeader4).Controls.Add((Control)(object)button2);
		((Control)pageHeader4).Dock = (DockStyle)5;
		pageHeader4.DragMove = false;
		grpan_main.SetIndex((Control)(object)pageHeader4, 2);
		((Control)pageHeader4).Location = new Point(10, 168);
		((Control)pageHeader4).Margin = new Padding(10);
		((Control)pageHeader4).Name = "pageHeader4";
		((Control)pageHeader4).Size = new Size(810, 24);
		((Control)pageHeader4).TabIndex = 57;
		((Control)pageHeader4).Text = "";
		btn_closeTips.DisplayStyle = (TButtonDisplayStyle)2;
		((Control)btn_closeTips).Dock = (DockStyle)4;
		btn_closeTips.Ghost = true;
		((IControl)btn_closeTips).HandCursor = Cursors.Default;
		btn_closeTips.Icon = (Image)(object)Resources.关闭;
		btn_closeTips.IconRatio = 1f;
		((Control)btn_closeTips).Location = new Point(755, 0);
		((Control)btn_closeTips).Margin = new Padding(0, 5, 0, 0);
		((Control)btn_closeTips).Name = "btn_closeTips";
		((Control)btn_closeTips).Size = new Size(55, 24);
		((Control)btn_closeTips).TabIndex = 7;
		((Control)btn_closeTips).TabStop = false;
		((Control)btn_closeTips).Text = "button3";
		((Control)btn_closeTips).MouseClick += new MouseEventHandler(btn_closeTips_MouseClick);
		((Control)lab_FWTips).BackColor = Color.Transparent;
		((Control)lab_FWTips).Dock = (DockStyle)3;
		((Control)lab_FWTips).ForeColor = Color.White;
		((Control)lab_FWTips).Location = new Point(44, 0);
		((Control)lab_FWTips).Margin = new Padding(0);
		((Control)lab_FWTips).Name = "lab_FWTips";
		((Control)lab_FWTips).Size = new Size(697, 24);
		((Control)lab_FWTips).TabIndex = 6;
		((Control)lab_FWTips).Text = "tips";
		lab_FWTips.TextAlign = (ContentAlignment)16;
		button2.DisplayStyle = (TButtonDisplayStyle)2;
		((Control)button2).Dock = (DockStyle)3;
		button2.Ghost = true;
		((IControl)button2).HandCursor = Cursors.Default;
		((IControl)button2).HandDragFolder = false;
		button2.Icon = (Image)(object)Resources.tip_icon;
		button2.IconRatio = 1.5f;
		((Control)button2).Location = new Point(0, 0);
		((Control)button2).Margin = new Padding(0);
		((Control)button2).Name = "button2";
		((Control)button2).Size = new Size(44, 24);
		((Control)button2).TabIndex = 0;
		((Control)button2).TabStop = false;
		((Control)button2).Text = "button2";
		button2.WaveSize = 0;
		((Control)pageHeader1).BackColor = Color.Transparent;
		((Control)pageHeader1).Controls.Add((Control)(object)pictureBox1);
		((Control)pageHeader1).Controls.Add((Control)(object)divider1);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_tempture);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_sn);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_Disconnect);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_firmwareVer);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_hardwareVer);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_devName);
		pageHeader1.DragMove = false;
		pageHeader1.Gap = 0;
		((IControl)pageHeader1).HandCursor = Cursors.Default;
		grpan_main.SetIndex((Control)(object)pageHeader1, 1);
		((Control)pageHeader1).Location = new Point(5, 5);
		((Control)pageHeader1).Margin = new Padding(5);
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Size = new Size(820, 148);
		((Control)pageHeader1).TabIndex = 52;
		((Control)pageHeader1).TabStop = false;
		((Control)pageHeader1).Text = "";
		((Control)pictureBox1).BackColor = Color.Transparent;
		((Control)pictureBox1).Dock = (DockStyle)3;
		pictureBox1.Image = (Image)(object)Resources.ascent_repeater;
		((Control)pictureBox1).Location = new Point(0, 0);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(131, 138);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 36;
		pictureBox1.TabStop = false;
		((Control)divider1).BackColor = Color.Transparent;
		divider1.ColorSplit = Color.FromArgb(255, 233, 0);
		((Control)divider1).Dock = (DockStyle)2;
		((Control)divider1).Location = new Point(0, 138);
		((Control)divider1).Name = "divider1";
		divider1.OrientationMargin = 0f;
		((Control)divider1).Size = new Size(820, 10);
		((Control)divider1).TabIndex = 35;
		((Control)divider1).Text = "";
		divider1.TextPadding = 0f;
		divider1.Thickness = 2f;
		lab_tempture.AutoSizeMode = (TAutoSize)1;
		((Control)lab_tempture).BackColor = Color.Transparent;
		((Control)lab_tempture).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_tempture.ForeColor = Color.FromArgb(164, 164, 165);
		((IControl)lab_tempture).HandCursor = Cursors.Default;
		((Control)lab_tempture).Location = new Point(500, 112);
		((Control)lab_tempture).Margin = new Padding(0);
		((Control)lab_tempture).Name = "lab_tempture";
		((Control)lab_tempture).Size = new Size(111, 17);
		lab_tempture.Suffix = " :111";
		lab_tempture.SuffixColor = Color.White;
		((Control)lab_tempture).TabIndex = 34;
		((Control)lab_tempture).TabStop = false;
		((Control)lab_tempture).Text = "芯片温度：";
		lab_tempture.TextAlign = (ContentAlignment)32;
		lab_tempture.TextMultiLine = false;
		((Control)lab_tempture).MouseDoubleClick += new MouseEventHandler(lab_tempture_MouseDoubleClick);
		lab_sn.AutoSizeMode = (TAutoSize)1;
		((Control)lab_sn).BackColor = Color.Transparent;
		((Control)lab_sn).Cursor = Cursors.Default;
		((Control)lab_sn).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_sn.ForeColor = Color.FromArgb(164, 164, 165);
		((IControl)lab_sn).HandCursor = Cursors.Default;
		((Control)lab_sn).Location = new Point(505, 70);
		((Control)lab_sn).Margin = new Padding(0);
		((Control)lab_sn).Name = "lab_sn";
		((Control)lab_sn).Size = new Size(51, 17);
		lab_sn.Suffix = ": 111";
		lab_sn.SuffixColor = Color.White;
		((Control)lab_sn).TabIndex = 33;
		((Control)lab_sn).TabStop = false;
		((Control)lab_sn).Text = "SN";
		lab_sn.TextAlign = (ContentAlignment)32;
		lab_sn.TextMultiLine = false;
		btn_Disconnect.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_Disconnect).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_Disconnect.ForeColor = Color.FromArgb(35, 35, 35);
		btn_Disconnect.Icon = (Image)(object)Resources.断开连接_深色1;
		btn_Disconnect.IconGap = 0.3f;
		btn_Disconnect.IconPosition = (TAlignMini)0;
		btn_Disconnect.IconRatio = 0.8f;
		btn_Disconnect.IconSvg = "";
		((Control)btn_Disconnect).Location = new Point(505, 12);
		((Control)btn_Disconnect).Margin = new Padding(0);
		((Control)btn_Disconnect).Name = "btn_Disconnect";
		((Control)btn_Disconnect).Size = new Size(100, 36);
		((Control)btn_Disconnect).TabIndex = 30;
		((Control)btn_Disconnect).Text = "断开连接";
		btn_Disconnect.WaveSize = 0;
		((Control)btn_Disconnect).Click += btn_Disconnect_Click;
		lab_firmwareVer.AutoSizeMode = (TAutoSize)1;
		((Control)lab_firmwareVer).BackColor = Color.Transparent;
		((Control)lab_firmwareVer).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_firmwareVer.ForeColor = Color.FromArgb(164, 164, 165);
		((IControl)lab_firmwareVer).HandCursor = Cursors.Default;
		((Control)lab_firmwareVer).Location = new Point(141, 112);
		((Control)lab_firmwareVer).Margin = new Padding(0);
		((Control)lab_firmwareVer).Name = "lab_firmwareVer";
		((Control)lab_firmwareVer).Size = new Size(86, 17);
		lab_firmwareVer.Suffix = " :111";
		lab_firmwareVer.SuffixColor = Color.White;
		((Control)lab_firmwareVer).TabIndex = 32;
		((Control)lab_firmwareVer).TabStop = false;
		((Control)lab_firmwareVer).Text = "firmware";
		lab_firmwareVer.TextAlign = (ContentAlignment)32;
		lab_firmwareVer.TextMultiLine = false;
		((Control)lab_firmwareVer).Click += lab_firmwareVer_Click;
		lab_hardwareVer.AutoSizeMode = (TAutoSize)1;
		((Control)lab_hardwareVer).BackColor = Color.Transparent;
		((Control)lab_hardwareVer).Cursor = Cursors.Default;
		((Control)lab_hardwareVer).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_hardwareVer.ForeColor = Color.FromArgb(164, 164, 165);
		((IControl)lab_hardwareVer).HandCursor = Cursors.Default;
		((Control)lab_hardwareVer).Location = new Point(141, 70);
		((Control)lab_hardwareVer).Margin = new Padding(0);
		((Control)lab_hardwareVer).Name = "lab_hardwareVer";
		((Control)lab_hardwareVer).Size = new Size(98, 17);
		lab_hardwareVer.Suffix = "   :111";
		lab_hardwareVer.SuffixColor = Color.FromArgb(255, 255, 255);
		((Control)lab_hardwareVer).TabIndex = 31;
		((Control)lab_hardwareVer).TabStop = false;
		((Control)lab_hardwareVer).Text = "hardware";
		lab_hardwareVer.TextAlign = (ContentAlignment)32;
		lab_hardwareVer.TextMultiLine = false;
		((Control)lab_hardwareVer).Click += lab_hardwareVer_Click;
		lab_devName.AutoSizeMode = (TAutoSize)1;
		((Control)lab_devName).BackColor = Color.Transparent;
		((Control)lab_devName).Font = new Font("Microsoft Sans Serif", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		lab_devName.ForeColor = Color.White;
		((IControl)lab_devName).HandCursor = Cursors.Default;
		((Control)lab_devName).Location = new Point(141, 10);
		((Control)lab_devName).Margin = new Padding(15, 0, 0, 0);
		((Control)lab_devName).Name = "lab_devName";
		((Control)lab_devName).Size = new Size(183, 31);
		((Control)lab_devName).TabIndex = 1;
		((Control)lab_devName).TabStop = false;
		((Control)lab_devName).Text = "Ascent GT Pro";
		lab_devName.TextAlign = (ContentAlignment)32;
		lab_devName.TextMultiLine = false;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).Controls.Add((Control)(object)grpan_main);
		((Control)this).Font = new Font("宋体", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "AscentUpdataFrm";
		((Control)this).Padding = new Padding(5);
		((Control)this).Size = new Size(840, 640);
		((UserControl)this).Load += AscentUpdataFrm_Load;
		((Control)grpan_process).ResumeLayout(false);
		((Control)grpan_procDesc).ResumeLayout(false);
		((Control)stackPanel1).ResumeLayout(false);
		((ISupportInitialize)pictureBox2).EndInit();
		((Control)grpan_main).ResumeLayout(false);
		((Control)stackPanel2).ResumeLayout(false);
		((Control)grpan_upgrade).ResumeLayout(false);
		((Control)pageHeader3).ResumeLayout(false);
		((Control)pageHeader3).PerformLayout();
		((Control)uploadDragger1).ResumeLayout(false);
		((Control)pageHeader4).ResumeLayout(false);
		((Control)pageHeader1).ResumeLayout(false);
		((Control)pageHeader1).PerformLayout();
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
