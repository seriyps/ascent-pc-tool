using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class AscentUpdataFrm : UserControl, IDisposable
{
	private System.Timers.Timer _btnBlinkTimer;

	private int _blinkPlusing;

	private string _deviceName = "未找到设备";

	private string _portName = "";

	private bool _isCanecl = false;

	private bool _IsswCheck;

	private string _perVer;

	private string _currVer;

	private bool _isStartUpg = false;

	private string hardware_en = "Hardware";

	private string firmware_en = "Firmware";

	private string desc_en = "Drag the firmware to this area, or";

	private string selectPath_en = "  select the firmware";

	private string btn_upgrade_en = "Upgrade";

	private string btn_disconn_en = "Disconnect";

	private string btn_FAQ_en = "FAQ";

	private string tempture_en = "Temperature";

	private string selectFile_en = "Select file";

	private string hardware_cn = "硬件";

	private string firmware_cn = "固件";

	private string desc_cn = "将固件拖拽到此区域 ，或";

	private string selectPath_cn = "  选择固件";

	private string btn_upgrade_cn = "开始升级";

	private string btn_disconn_cn = "断开连接";

	private string btn_FAQ_cn = "常见问题";

	private string tempture_cn = "芯片温度";

	private string selectFile_cn = "选择文件";

	public Form Frm;

	private ResAscentInfo _currAsceDevInfo;

	private bool _isInterceUpg = true;

	private IContainer components = null;

	private Label lab_devName;

	private PageHeader pageHeader1;

	private Button btn_Disconnect;

	private Label lab_hardwareVer;

	private UploadDragger uploadDragger1;

	private Label lab_firmwareVer;

	private Label lab_sn;

	private Label lab_tempture;

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

	private PictureBox pictureBox1;

	private Divider divider1;

	private Label lab_filepath;

	private StackPanel stackPanel2;

	private Label label1;

	public string DeviceName
	{
		get
		{
			return _deviceName;
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
	}

	public AscentUpdataFrm(ResAscentInfo info)
	{
		InitializeComponent();
		_currAsceDevInfo = info;
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
		uploadDragger1.Filter = "固件 (*.img)|*.img|所有文件 (*.*)|*.*";
	}

	private void RefreshDeviceName(string name)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
		});
	}

	private void ReloadProductImg()
	{
		switch (_deviceName)
		{
		case "Ascent Lite VTX":
			pictureBox1.Image = (Image)(object)Resources.Ascent_Lite_VTX;
			break;
		case "Ascent Lite +":
			pictureBox1.Image = (Image)(object)Resources.Ascent_LitePlus;
			break;
		case "YoHD Micro":
			pictureBox1.Image = (Image)(object)Resources.Ascent_RC;
			break;
		case "Ascent GT":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GT;
			break;
		case "Ascent GT 2.7k":
		case "Ascent GT night":
		case "Ascent GT ultra":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			break;
		case "Ascent GT Pro":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTPro;
			break;
		case "Ascent GT Pro Z8":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTProZ8;
			break;
		case "Ascent GT Pro Z40":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTProZ40;
			break;
		case "Ascent GT Pro Hub":
			pictureBox1.Image = (Image)(object)Resources.Ascent_CamHub;
			break;
		case "Ascent VRX":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRX;
			break;
		case "Ascent VRX Pro":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRXPro;
			break;
		case "Ascent VRX Max":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRXMAX;
			break;
		case "Ascent VRX Cine":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			break;
		case "Ascent Google L":
			pictureBox1.Image = (Image)(object)Resources.Ascent_Goggles;
			break;
		default:
			pictureBox1.Image = (Image)(object)Resources.不支持该设备;
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
					Button obj = btn_startUpg;
					Button obj2 = btn_Disconnect;
					bool flag = (((IControl)uploadDragger1).Enabled = true);
					bool enabled = (((IControl)obj2).Enabled = flag);
					((IControl)obj).Enabled = enabled;
					progress1.Fill = Color.FromArgb(43, 164, 113);
					AscentFrmEventArgs e2 = new AscentFrmEventArgs
					{
						Desc = "UpgradeComp"
					};
					OnAscentFrmEvnet?.Invoke(null, e2);
				}
			});
			Thread.Sleep(50);
			GD.Inst.UpgFSM.Send_FindDevice();
			break;
		case InfoType.upgProcInfo:
			break;
		case InfoType.fail:
			_isStartUpg = false;
			Thread.Sleep(50);
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				progress1.State = (TType)4;
				((Control)lab_upgDesc).Text = e.Desc;
				pictureBox2.Image = (Image)(object)Resources.升级失败;
				Button obj = btn_Disconnect;
				UploadDragger obj2 = uploadDragger1;
				bool flag = (((IControl)btn_startUpg).Enabled = true);
				bool enabled = (((IControl)obj2).Enabled = flag);
				((IControl)obj).Enabled = enabled;
				WriteLog.WriteLogFileToUI("升级失败，desc=" + e.Desc + ",err=" + e.errMsg, Color.Red);
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
						obj2.Value += 0.002f;
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
		_currAsceDevInfo = arg2;
		UpdateDeviceInfo(arg2.DevName, arg2.FWVers, arg2.SN, arg2.HWVers, arg2.MCUTemp);
	}

	public void SubscribeUpgEvent()
	{
		GD.Inst.UsbFSM.SerialConnectedStateChange += UsbFSM_SerialConneStateChange;
		GD.Inst.UpgFSM.OnUpgProcHappenEvent += UpgFSM_OnUpgProcHappenEvent;
		UpgradeProcessFSM upgFSM = GD.Inst.UpgFSM;
		upgFSM.OnAsceDevRecInfo = (Action<uint, ResAscentInfo, object>)Delegate.Combine(upgFSM.OnAsceDevRecInfo, new Action<uint, ResAscentInfo, object>(UpgFSM_OnAsceDevRecInfo));
	}

	public void UnsubscribeUpgEvent()
	{
		try
		{
			if (GD.Inst.UsbFSM != null)
			{
				GD.Inst.UsbFSM.SerialConnectedStateChange -= UsbFSM_SerialConneStateChange;
				GD.Inst.UsbFSM.Dispose();
			}
			if (GD.Inst.UpgFSM != null)
			{
				GD.Inst.UpgFSM.OnUpgProcHappenEvent -= UpgFSM_OnUpgProcHappenEvent;
				UpgradeProcessFSM upgFSM = GD.Inst.UpgFSM;
				upgFSM.OnAsceDevRecInfo = (Action<uint, ResAscentInfo, object>)Delegate.Remove(upgFSM.OnAsceDevRecInfo, new Action<uint, ResAscentInfo, object>(UpgFSM_OnAsceDevRecInfo));
				GD.Inst.UpgFSM.Dispose();
			}
		}
		catch (Exception)
		{
		}
	}

	private void ConnectDevice()
	{
		if (GD.Inst.UsbFSM != null)
		{
			GD.Inst.UsbFSM.SerialConnectedStateChange -= UsbFSM_SerialConneStateChange;
			GD.Inst.UsbFSM?.Dispose();
		}
		GD.Inst.UsbFSM = new UsbSerialportFSM(_currAsceDevInfo.UsbInfo);
		bool flag = GD.Inst.UsbFSM.Open(_portName);
		DisConnectDeviceResetFrm(flag);
		if (!flag)
		{
			string text = ((GD.Inst.CurrLang == 1) ? "错误提示" : "Error prompt");
			string text2 = ((GD.Inst.CurrLang == 1) ? "未连接设备" : "Unconnected device");
			Notification.error(Frm, text, text2, (TAlignFrom)10, (Font)null, (int?)0);
			return;
		}
		if (GD.Inst.UpgFSM != null)
		{
			GD.Inst.UpgFSM.OnUpgProcHappenEvent -= UpgFSM_OnUpgProcHappenEvent;
			UpgradeProcessFSM upgFSM = GD.Inst.UpgFSM;
			upgFSM.OnAsceDevRecInfo = (Action<uint, ResAscentInfo, object>)Delegate.Remove(upgFSM.OnAsceDevRecInfo, new Action<uint, ResAscentInfo, object>(UpgFSM_OnAsceDevRecInfo));
			GD.Inst.UpgFSM?.Dispose();
		}
		GD.Inst.UpgFSM = new UpgradeProcessFSM(GD.Inst.UsbFSM);
		SubscribeUpgEvent();
		Thread.Sleep(100);
		((IControl)uploadDragger1).Enabled = true;
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
					((Control)lab_devName).Text = devname;
				}
				string pattern = "(?:486|485|482|472|492)?[^\\d]*([\\d.]+(?:-\\d+\\.\\d+)?)";
				Match match2 = Regex.Match(hardwareVersion, pattern);
				string text = (match2.Success ? match2.Groups[1].Value : hardwareVersion);
				string text2;
				if (text.Contains("486") || text.Contains("485") || text.Contains("482") || text.Contains("472") || text.Contains("492"))
				{
					int num = hardwareVersion.IndexOf(text);
					text2 = hardwareVersion.Remove(0, num + 4);
				}
				else
				{
					text2 = hardwareVersion;
				}
				string text3 = ((GD.Inst.CurrLang == 1) ? hardware_cn : hardware_en);
				lab_hardwareVer.Suffix = " : " + text2;
				text3 = ((GD.Inst.CurrLang == 1) ? firmware_cn : firmware_en);
				lab_firmwareVer.Suffix = " : " + firmwareInfo;
				lab_sn.Suffix = " : " + sn;
				text3 = ((GD.Inst.CurrLang == 1) ? tempture_cn : tempture_en);
				lab_tempture.Suffix = " : " + temptrue + "℃";
				Match match3 = Regex.Match(firmwareInfo, "(\\d+_\\d+_\\d+)$");
				string text4 = firmwareInfo.Remove(12);
				if (match3.Success)
				{
					text4 = firmwareInfo.Remove(match3.Index - 1);
				}
				((Control)lab_FWTips).Text = ((GD.Inst.CurrLang == 1) ? ("请选择\"" + text4 + "_xxx\"文件名固件") : ("Please select\"" + text4 + "_xxx\"filename firmware"));
			});
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
				GD.Inst.UsbFSM?.Close();
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
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		try
		{
			OpenFileDialog val = new OpenFileDialog();
			try
			{
				((FileDialog)val).Title = "Open";
				((FileDialog)val).InitialDirectory = "D:";
				((FileDialog)val).Filter = "firmware (*.img)|*.img|All (*.*)|*.*";
				((FileDialog)val).FilterIndex = 1;
				((FileDialog)val).RestoreDirectory = true;
				val.Multiselect = false;
				if ((int)((CommonDialog)val).ShowDialog() != 1)
				{
					return;
				}
				string path = ((FileDialog)val).FileName;
				((Control)this).Invoke((Delegate)(Action)delegate
				{
					((Control)lab_filepath).Text = path;
					((IControl)btn_startUpg).Enabled = true;
				});
				GD.Inst.UpgFSM.SetFile(path);
				string text = "";
				string version = GetVersion(path, b: false);
				string text2 = ExtractVersion(val.SafeFileName);
				string text3 = val.SafeFileName.ToLower();
				if (!string.IsNullOrEmpty(text2))
				{
					_currVer = "V" + text2;
					return;
				}
				Match match = Regex.Match(((FileDialog)val).FileName, "(\\d+[\\._]\\d+[\\._]\\d+\\.img)");
				if (!match.Success)
				{
					_currVer = "V_unRecog";
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
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string[] value = ((VEventArgs<string[]>)(object)e).Value;
			string path = value[0];
			FileAttributes attributes = File.GetAttributes(path);
			if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
			{
				string title;
				string desc;
				if (GD.Inst.CurrLang == 1)
				{
					title = "警告";
					desc = "只可拖拽固件文件。";
				}
				else
				{
					title = "Warning";
					desc = "Drag firmware files only。";
				}
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
				((Form)commModalFrm).ShowDialog();
			}
			else if (SelectWrongFWFilePrompt(path))
			{
				((Control)this).Invoke((Delegate)(Action)delegate
				{
					((Control)lab_filepath).Text = path;
					((IControl)btn_startUpg).Enabled = true;
				});
				GD.Inst.UpgFSM.SetFile(path);
				Match match = Regex.Match(path, "(\\d+[\\._]\\d+[\\._]\\d+\\.img)");
				if (!match.Success)
				{
					_currVer = "V_unRecog";
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
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("AscentUpdataFrm.DragChanged error，desc=" + ex.Message, Color.Red);
		}
	}

	private void btnstartUpg_Click(object sender, EventArgs e)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Invalid comparison between Unknown and I4
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Invalid comparison between Unknown and I4
		int num = JudgePerVersAndCurrVers(out var res);
		if (res < 0 && num == 1)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "警告";
				desc = "不建议对设备进行降级。";
			}
			else
			{
				title = "Warning";
				desc = "It is not recommended to device。";
			}
			commModalFrm.SetAllTxt(title, desc);
			DialogResult val = ((Form)commModalFrm).ShowDialog();
			if ((int)commModalFrm.FrmResult != 1)
			{
				return;
			}
		}
		StartUpgAscentModalFrm startUpgAscentModalFrm = new StartUpgAscentModalFrm();
		((Form)startUpgAscentModalFrm).ShowDialog();
		if ((int)startUpgAscentModalFrm.result == 1)
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				_isStartUpg = true;
				((IControl)grpan_process).Visible = true;
				progress1.Value = 0.05f;
				((Control)lab_upgDesc).Text = ((GD.Inst.CurrLang == 1) ? "开始升级固件" : "Start firmware upgrade");
				progress1.State = (TType)0;
				progress1.Fill = Color.FromArgb(255, 233, 0);
				pictureBox2.Image = (Image)(object)Resources.升级中;
				((Control)lab_versChange).Text = _perVer + " >> " + _currVer;
				Button obj = btn_Disconnect;
				UploadDragger obj2 = uploadDragger1;
				bool flag = (((IControl)btn_startUpg).Enabled = false);
				bool enabled = (((IControl)obj2).Enabled = flag);
				((IControl)obj).Enabled = enabled;
			});
			AscentFrmEventArgs e2 = new AscentFrmEventArgs
			{
				Desc = "StartUpgrade"
			};
			OnAscentFrmEvnet?.Invoke(null, e2);
			GD.Inst.UpgFSM.IsAutoUpgrade = true;
			GD.Inst.UpgFSM.Start();
		}
	}

	private void button4_Click(object sender, EventArgs e)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Invalid comparison between Unknown and I4
		if (((Control)btn_Disconnect).Text == btn_disconn_cn || ((Control)btn_Disconnect).Text == btn_disconn_en)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title;
			string desc;
			string btnok;
			string btnCan;
			if (GD.Inst.CurrLang == 1)
			{
				title = "断开连接";
				desc = "是否确认断开设备连接";
				btnok = "确认";
				btnCan = "取消";
			}
			else
			{
				title = "Disconnect";
				desc = "Are you sure you want to disconnect the device?";
				btnok = "Confirm";
				btnCan = "Cancel";
			}
			commModalFrm.SetAllTxt(title, desc, btnok, btnCan);
			((Form)commModalFrm).ShowDialog();
			if ((int)commModalFrm.FrmResult == 1)
			{
				UnsubscribeUpgEvent();
				AscentFrmEventArgs e2 = new AscentFrmEventArgs
				{
					Desc = "ManualCloseDevice"
				};
				OnAscentFrmEvnet?.Invoke(null, e2);
				((Control)this).Hide();
			}
		}
		else if (((Control)btn_Disconnect).Text == "重新连接")
		{
			ConnectDevice();
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
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			if (!isConn)
			{
				((Control)btn_Disconnect).Text = "Reconnect";
				btn_Disconnect.IconRatio = 0f;
			}
			else
			{
				((Control)btn_Disconnect).Text = ((GD.Inst.CurrLang == 1) ? btn_disconn_cn : btn_disconn_en);
			}
			UploadDragger obj = uploadDragger1;
			bool enabled = (((IControl)btn_startUpg).Visible = isConn);
			((IControl)obj).Enabled = enabled;
		});
	}

	public int JudgeDeviceAndVers(out int res)
	{
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
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
		commModalFrm.SetAllTxt("错误提示", "当前版本过低，要先升级为16_4_4，请重新选择16_4_4固件的路径", "确认");
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
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Invalid comparison between Unknown and I4
		try
		{
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
				string title;
				string desc;
				if (GD.Inst.CurrLang == 1)
				{
					title = "警告";
					desc = "所选固件的文件名和设备不匹配，固件文件是否选错?";
				}
				else
				{
					title = "Warning";
					desc = "The filename of the selected firmware does not match the device. Whether the firmware file is selected incorrectly?";
				}
				CommModalFrm commModalFrm = new CommModalFrm();
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

	public void ReloadFont()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Expected O, but got Unknown
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 20f);
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
			Button obj10 = btn_startUpg;
			Font val2 = (((Control)lab_upgDesc).Font = val);
			Font val4 = (((Control)obj10).Font = val2);
			Font val6 = (((Control)obj9).Font = val4);
			Font val8 = (((Control)obj8).Font = val6);
			Font val10 = (((Control)obj7).Font = val8);
			Font val12 = (((Control)obj6).Font = val10);
			Font val14 = (((Control)obj5).Font = val12);
			Font val16 = (((Control)obj4).Font = val14);
			Font val18 = (((Control)obj3).Font = val16);
			Font font2 = (((Control)obj2).Font = val18);
			((Control)obj).Font = font2;
			Font font3 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
			((Control)lab_versChange).Font = font3;
		});
	}

	public void ReloadLang()
	{
		switch ((LangType)GD.Inst.CurrLang)
		{
		case LangType.zh_CN:
			((Control)lab_hardwareVer).Text = hardware_cn;
			((Control)lab_firmwareVer).Text = firmware_cn;
			((Control)lab_filepath).Text = desc_cn;
			lab_filepath.Suffix = selectPath_cn;
			((Control)btn_startUpg).Text = btn_upgrade_cn;
			((Control)btn_Disconnect).Text = btn_disconn_cn;
			((Control)lab_tempture).Text = tempture_cn;
			((Control)lab_selectFile).Text = selectFile_cn;
			break;
		case LangType.en_US:
			((Control)lab_hardwareVer).Text = hardware_en;
			((Control)lab_firmwareVer).Text = firmware_en;
			((Control)lab_filepath).Text = desc_en;
			lab_filepath.Suffix = selectPath_en;
			((Control)btn_startUpg).Text = btn_upgrade_en;
			((Control)btn_Disconnect).Text = btn_disconn_en;
			((Control)lab_tempture).Text = tempture_en;
			((Control)lab_selectFile).Text = selectFile_en;
			break;
		}
	}

	public void Dispose()
	{
		UnsubscribeUpgEvent();
		GC.Collect();
		GC.WaitForPendingFinalizers();
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
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Expected O, but got Unknown
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Expected O, but got Unknown
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Expected O, but got Unknown
		//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0992: Unknown result type (might be due to invalid IL or missing references)
		//IL_099c: Expected O, but got Unknown
		//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Expected O, but got Unknown
		//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e66: Expected O, but got Unknown
		//IL_0ea5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f34: Expected O, but got Unknown
		//IL_0f6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1074: Unknown result type (might be due to invalid IL or missing references)
		//IL_1136: Unknown result type (might be due to invalid IL or missing references)
		//IL_1244: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1375: Unknown result type (might be due to invalid IL or missing references)
		//IL_137f: Expected O, but got Unknown
		//IL_13ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c6: Expected O, but got Unknown
		//IL_1611: Unknown result type (might be due to invalid IL or missing references)
		//IL_1637: Unknown result type (might be due to invalid IL or missing references)
		//IL_169d: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a7: Expected O, but got Unknown
		//IL_16d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1793: Unknown result type (might be due to invalid IL or missing references)
		//IL_179d: Expected O, but got Unknown
		//IL_180b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1935: Unknown result type (might be due to invalid IL or missing references)
		//IL_193f: Expected O, but got Unknown
		//IL_19c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6e: Expected O, but got Unknown
		//IL_1a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a86: Expected O, but got Unknown
		//IL_1abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac6: Expected O, but got Unknown
		//IL_1b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bbd: Expected O, but got Unknown
		//IL_1c67: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da8: Expected O, but got Unknown
		//IL_1df3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f91: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f9b: Expected O, but got Unknown
		//IL_1f9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fb7: Unknown result type (might be due to invalid IL or missing references)
		lab_devName = new Label();
		pageHeader1 = new PageHeader();
		pictureBox1 = new PictureBox();
		divider1 = new Divider();
		lab_tempture = new Label();
		lab_sn = new Label();
		btn_Disconnect = new Button();
		lab_firmwareVer = new Label();
		lab_hardwareVer = new Label();
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
		((Control)pageHeader1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
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
		((Control)this).SuspendLayout();
		lab_devName.AutoSizeMode = (TAutoSize)1;
		((Control)lab_devName).BackColor = Color.Transparent;
		((Control)lab_devName).Font = new Font("阿里巴巴普惠体 Medium", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		lab_devName.ForeColor = Color.White;
		((Control)lab_devName).Location = new Point(141, 10);
		((Control)lab_devName).Margin = new Padding(15, 0, 0, 0);
		((Control)lab_devName).Name = "lab_devName";
		((Control)lab_devName).Size = new Size(183, 38);
		((Control)lab_devName).TabIndex = 1;
		((Control)lab_devName).TabStop = false;
		((Control)lab_devName).Text = "Ascent GT Pro";
		lab_devName.TextAlign = (ContentAlignment)32;
		lab_devName.TextMultiLine = false;
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
		pictureBox1.Image = (Image)(object)Resources.Ascent_VRXMAX;
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
		((Control)lab_tempture).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_tempture.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_tempture).Location = new Point(435, 112);
		((Control)lab_tempture).Margin = new Padding(0);
		((Control)lab_tempture).Name = "lab_tempture";
		((Control)lab_tempture).Size = new Size(98, 20);
		lab_tempture.Suffix = " :111";
		lab_tempture.SuffixColor = Color.White;
		((Control)lab_tempture).TabIndex = 34;
		((Control)lab_tempture).TabStop = false;
		((Control)lab_tempture).Text = "芯片温度：";
		lab_tempture.TextAlign = (ContentAlignment)32;
		lab_tempture.TextMultiLine = false;
		((Control)lab_tempture).Click += lab_tempture_Click;
		lab_sn.AutoSizeMode = (TAutoSize)1;
		((Control)lab_sn).BackColor = Color.Transparent;
		((Control)lab_sn).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_sn.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_sn).Location = new Point(435, 70);
		((Control)lab_sn).Margin = new Padding(0);
		((Control)lab_sn).Name = "lab_sn";
		((Control)lab_sn).Size = new Size(50, 20);
		lab_sn.Suffix = ": 111";
		lab_sn.SuffixColor = Color.White;
		((Control)lab_sn).TabIndex = 33;
		((Control)lab_sn).TabStop = false;
		((Control)lab_sn).Text = "SN";
		lab_sn.TextAlign = (ContentAlignment)32;
		lab_sn.TextMultiLine = false;
		btn_Disconnect.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_Disconnect).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_Disconnect.ForeColor = Color.FromArgb(35, 35, 35);
		btn_Disconnect.Icon = (Image)(object)Resources.断开连接_深色;
		btn_Disconnect.IconGap = 0.3f;
		btn_Disconnect.IconPosition = (TAlignMini)0;
		btn_Disconnect.IconRatio = 0.8f;
		btn_Disconnect.IconSvg = "";
		((Control)btn_Disconnect).Location = new Point(435, 12);
		((Control)btn_Disconnect).Margin = new Padding(0);
		((Control)btn_Disconnect).Name = "btn_Disconnect";
		((Control)btn_Disconnect).Size = new Size(100, 36);
		((Control)btn_Disconnect).TabIndex = 30;
		((Control)btn_Disconnect).Text = "断开连接";
		btn_Disconnect.WaveSize = 0;
		((Control)btn_Disconnect).Click += button4_Click;
		lab_firmwareVer.AutoSizeMode = (TAutoSize)1;
		((Control)lab_firmwareVer).BackColor = Color.Transparent;
		((Control)lab_firmwareVer).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_firmwareVer.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_firmwareVer).Location = new Point(141, 112);
		((Control)lab_firmwareVer).Margin = new Padding(0);
		((Control)lab_firmwareVer).Name = "lab_firmwareVer";
		((Control)lab_firmwareVer).Size = new Size(95, 20);
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
		((Control)lab_hardwareVer).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_hardwareVer.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_hardwareVer).Location = new Point(141, 70);
		((Control)lab_hardwareVer).Margin = new Padding(0);
		((Control)lab_hardwareVer).Name = "lab_hardwareVer";
		((Control)lab_hardwareVer).Size = new Size(105, 20);
		lab_hardwareVer.Suffix = "   :111";
		lab_hardwareVer.SuffixColor = Color.FromArgb(255, 255, 255);
		((Control)lab_hardwareVer).TabIndex = 31;
		((Control)lab_hardwareVer).TabStop = false;
		((Control)lab_hardwareVer).Text = "hardware";
		lab_hardwareVer.TextAlign = (ContentAlignment)32;
		lab_hardwareVer.TextMultiLine = false;
		((Control)lab_hardwareVer).Click += lab_hardwareVer_Click;
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
		grpan_procDesc.Span = "100%;100%;-40% 60%";
		((Control)grpan_procDesc).TabIndex = 1;
		((Control)grpan_procDesc).Text = "gridPanel6";
		((Control)stackPanel1).Controls.Add((Control)(object)lab_upgDesc);
		((Control)stackPanel1).Controls.Add((Control)(object)lab_versChange);
		((Control)stackPanel1).Dock = (DockStyle)5;
		((Control)stackPanel1).Location = new Point(0, 0);
		((Control)stackPanel1).Margin = new Padding(0);
		((Control)stackPanel1).Name = "stackPanel1";
		((Control)stackPanel1).Size = new Size(720, 41);
		((Control)stackPanel1).TabIndex = 5;
		((Control)stackPanel1).Text = "stackPanel1";
		((Control)lab_upgDesc).Dock = (DockStyle)3;
		((Control)lab_upgDesc).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_upgDesc).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_upgDesc).Location = new Point(210, 0);
		((Control)lab_upgDesc).Margin = new Padding(0);
		((Control)lab_upgDesc).Name = "lab_upgDesc";
		((Control)lab_upgDesc).Size = new Size(467, 41);
		((Control)lab_upgDesc).TabIndex = 37;
		((Control)lab_upgDesc).Text = "正在升级";
		lab_upgDesc.TextAlign = (ContentAlignment)16;
		((Control)lab_versChange).Dock = (DockStyle)3;
		((Control)lab_versChange).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_versChange).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_versChange).Location = new Point(0, 0);
		((Control)lab_versChange).Margin = new Padding(0);
		((Control)lab_versChange).Name = "lab_versChange";
		((Control)lab_versChange).Size = new Size(210, 41);
		((Control)lab_versChange).TabIndex = 8;
		((Control)lab_versChange).Text = "V1.1.1--V2.1.1";
		lab_versChange.TextAlign = (ContentAlignment)16;
		progress1.Back = Color.FromArgb(99, 101, 103);
		((Control)progress1).Dock = (DockStyle)5;
		progress1.Fill = Color.FromArgb(255, 233, 0);
		progress1.ForeColor = Color.White;
		((IControl)progress1).HandCursor = Cursors.Default;
		grpan_procDesc.SetIndex((Control)(object)progress1, 3);
		((Control)progress1).Location = new Point(10, 41);
		((Control)progress1).Margin = new Padding(10, 0, 10, 0);
		((Control)progress1).Name = "progress1";
		((Control)progress1).Size = new Size(700, 61);
		((Control)progress1).TabIndex = 4;
		((Control)progress1).TabStop = false;
		((Control)progress1).Text = "";
		progress1.Value = 0.5f;
		progress1.ValueRatio = 1f;
		((Control)pictureBox2).Dock = (DockStyle)5;
		pictureBox2.Image = (Image)(object)Resources.升级中;
		((Control)pictureBox2).Location = new Point(10, 20);
		((Control)pictureBox2).Margin = new Padding(10, 20, 10, 20);
		((Control)pictureBox2).Name = "pictureBox2";
		((Control)pictureBox2).Size = new Size(60, 62);
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
		((Control)grpan_main).Location = new Point(5, 5);
		((Control)grpan_main).Margin = new Padding(0);
		((Control)grpan_main).Name = "grpan_main";
		((ContainerPanel)grpan_main).Radius = 10;
		((Control)grpan_main).Size = new Size(830, 630);
		grpan_main.Span = "100%;100%;100%;100%;\r\n-25% 7% 45% 23%";
		((Control)grpan_main).TabIndex = 56;
		((Control)grpan_main).Text = "gridPanel1";
		((Control)stackPanel2).Controls.Add((Control)(object)label1);
		((Control)stackPanel2).Location = new Point(5, 490);
		((Control)stackPanel2).Margin = new Padding(5);
		((Control)stackPanel2).Name = "stackPanel2";
		((Control)stackPanel2).Size = new Size(820, 135);
		((Control)stackPanel2).TabIndex = 59;
		((Control)stackPanel2).Text = "stackPanel2";
		stackPanel2.Vertical = true;
		((Control)label1).Dock = (DockStyle)1;
		((Control)label1).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
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
		((Control)pageHeader3).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
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
		((Control)lab_selectFile).Font = new Font("阿里巴巴普惠体 Medium", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
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
		((Control)btn_startUpg).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_startUpg.ForeColor = Color.FromArgb(35, 35, 35);
		btn_startUpg.IconGap = 0f;
		btn_startUpg.IconRatio = 0f;
		btn_startUpg.IconSvg = "";
		((Control)btn_startUpg).Location = new Point(720, 3);
		((Control)btn_startUpg).Margin = new Padding(0);
		((Control)btn_startUpg).Name = "btn_startUpg";
		((Control)btn_startUpg).Size = new Size(70, 36);
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
		((Control)uploadDragger1).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
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
		((Control)lab_filepath).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
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
		button2.Icon = (Image)(object)Resources.tips;
		button2.IconRatio = 1.5f;
		((Control)button2).Location = new Point(0, 0);
		((Control)button2).Margin = new Padding(0);
		((Control)button2).Name = "button2";
		((Control)button2).Size = new Size(44, 24);
		((Control)button2).TabIndex = 0;
		((Control)button2).TabStop = false;
		((Control)button2).Text = "button2";
		button2.WaveSize = 0;
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
		((Control)pageHeader1).ResumeLayout(false);
		((Control)pageHeader1).PerformLayout();
		((ISupportInitialize)pictureBox1).EndInit();
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
		((Control)this).ResumeLayout(false);
	}
}
