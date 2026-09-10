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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Caddx_PCTool;

public class NewGimbalFrm : UserControl, IDisposable
{
	private List<int> paramList = new List<int>();

	private List<Select> selectCtrlList = new List<Select>();

	private List<Label> labCtrlList = new List<Label>();

	private List<Input> inputCtrlList = new List<Input>();

	private string _perVer;

	private string _currVer;

	private UsbDevInfo _usbInfo;

	private bool _isCalibGryo = false;

	private ResAscentInfo _resAsceInfo;

	private UsbSerialportFSM _usbSP;

	private byte _currCMD;

	private string _filePath = "";

	private bool _isModeChannChange = false;

	private bool _isSenseChannChange = false;

	private bool _isRollChannChange = false;

	private bool _isPitchChannChange = false;

	private bool _isYawChannChange = false;

	private bool _isStartUpg = false;

	private float[] _modeSlotCMD = new float[3];

	private bool _isChangeModeSlot = false;

	private List<UsbDevInfo> _usbDevInfo;

	private RecvPacket_Gim _currPac;

	private IContainer components = null;

	private GridPanel gridPanel1;

	private GridPanel gridPanel2;

	private Label label1;

	private GridPanel gridPanel3;

	private Button button3;

	private Button btn_StopGim;

	private Button button5;

	private Button button4;

	private Button btn_Connect;

	private Input input25;

	private Button btn_SelectFile;

	private GroupBox groupBox5;

	private Input input24;

	private Input input23;

	private Input input22;

	private Input input21;

	private Label label14;

	private Label label12;

	private Label label13;

	private Label label11;

	private Label label10;

	private GridPanel grpan_gain;

	private Input inp_yawGain;

	private Input inp_pitchGain;

	private Input inp_rollGain;

	private Label lab_yawGain;

	private Label lab_pitchGain;

	private Label lab_rollGain;

	private GridPanel grpan_main;

	private PageHeader pageHeader1;

	private Label lab_tempture;

	private Label lab_sn;

	private Label lab_firmwareVer;

	private Label lab_hardwareVer;

	private Label lab_devName;

	private GridPanel grpan_upgrade;

	private GridPanel grpan_process;

	private GridPanel grpan_procDesc;

	private StackPanel stackPanel1;

	private Label lab_upgDesc;

	private Label lab_versChange;

	private Progress progress2;

	private PictureBox pictureBox1;

	private Button btn_startUpg;

	private GridPanel grpan_param;

	private Button btn_gryoCalib;

	private Button btn_startGM;

	private Button btn_WriteParam;

	private Button btn_saveParam;

	private Button btn_openParam;

	private GridPanel grpan_chann;

	private Select sel_yawChan;

	private Select sel_pitchChan;

	private Select sel_rollChan;

	private Select sel_senseChan;

	private Select sel_modeChan;

	private Label lab_yawChan;

	private Label lab_pitchChan;

	private Label lab_rollChan;

	private Label lab_sensChan;

	private Label lab_modeChan;

	private Divider divider1;

	private PictureBox pictureBox2;

	private PageHeader pageHeader2;

	private Select sel_portname;

	private Button btn_refresh;

	private Button btn_CloseGM;

	private Button btn_Disconnect;

	private Select sel_gmMoed;

	private UploadDragger uploadDragger1;

	private Label lab_gmMode;

	private Label lab_yawNum;

	private Input inp_yawNum;

	private Input inp_pitchNum;

	private Label lab_pitchNum;

	private Input inp_rollNum;

	private Input inp_sensNum;

	private Label lab_rollNum;

	private Label lab_sensNum;

	private Input inp_currMode;

	private Label lab_currGMMode;

	private Switch switch1;

	private Label lab_angProtect;

	private Select sel_M2;

	private Label label4;

	private Select sel_M1;

	private Label label3;

	private Select sel_M0;

	private Label label2;

	private Button btn_posCalib;

	private Select sel_LensType;

	private Label lab_LensType;

	public event EventHandler<FrmEventArgs> OnGimFrmHappenEvnet;

	public NewGimbalFrm()
	{
		InitializeComponent();
	}

	public NewGimbalFrm(ResAscentInfo resInfo)
	{
		InitializeComponent();
		_usbInfo = resInfo.UsbInfo;
		_resAsceInfo = resInfo;
	}

	private void NewGimbalFrm_Load(object sender, EventArgs e)
	{
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		((IControl)grpan_process).Visible = false;
		((IControl)grpan_upgrade).Visible = true;
		InitCtrlList();
		if (GD.Inst.SP_Gim != null)
		{
			GD.Inst.SP_Gim.Dispose();
			GD.Inst.SP_Gim = null;
		}
		GD.Inst.SP_Gim = new Serialport_Gimbal(_resAsceInfo.UsbInfo);
		bool flag = false;
		for (int i = 0; i < 3; i++)
		{
			if (flag)
			{
				break;
			}
			if (i > 0)
			{
				Thread.Sleep(150);
			}
			flag = GD.Inst.SP_Gim.Open(GD.Inst.CurrPortName);
			if (!flag)
			{
				WriteLog.WriteLogFileToUI($"云台串口打开失败，第{i + 1}次尝试，串口名={GD.Inst.CurrPortName}", Color.DarkOrange);
			}
		}
		if (!flag)
		{
			WriteLog.WriteLogFileToUI("云台串口打开失败，请检查连接及端口设置", Color.Red);
			btn_Disconnect.DefaultBack = Color.FromArgb(255, 233, 0);
			btn_Disconnect.ForeColor = Color.FromArgb(35, 35, 35);
			((Control)btn_Disconnect).Text = Lang.T("gimbal.btn_reconnect");
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("gimbal.port_not_connected");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, Color.Red);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		((Control)btn_Disconnect).Text = Lang.T("gimbal.btn_disconnect");
		GD.Inst.Upg_Gim = new Upg_Gimbal(GD.Inst.SP_Gim);
		BindEventHandler();
		Thread.Sleep(100);
		_currCMD = 0;
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.StartGim);
		if (UpdateDevInfo(_resAsceInfo))
		{
			ReloadSelItems();
			ReloadFont();
			ReloadLang();
			_currPac = new RecvPacket_Gim();
		}
	}

	private bool UpdateDevInfo(ResAscentInfo info)
	{
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		CommModalFrm commModalFrm = new CommModalFrm();
		bool result = false;
		try
		{
			if (info == null)
			{
				commModalFrm.SetAllTxt("Error", "The device information is empty, please check the connection cable between the device and PC", Color.Red);
				((Form)commModalFrm).ShowDialog();
				return result;
			}
			WriteLog.WriteLogFileToUI("info=" + info.ToString(), Color.Black);
			_resAsceInfo = info;
			lab_hardwareVer.Suffix = info.HWVers;
			lab_firmwareVer.Suffix = ExtractFirmwareVersion(info.FWVers);
			lab_tempture.Suffix = info.MCUTemp + "℃";
			if (info.SN != null)
			{
				int count = ((info.SN.Length > 6) ? (info.SN.Length - 6) : 0);
				((Control)lab_sn).Text = "SN : ";
				lab_sn.Suffix = info.SN.Remove(0, count);
				if (string.Equals(info.DevName, "SimGM", StringComparison.OrdinalIgnoreCase) || string.Equals(info.DevName, "caddxsimgm", StringComparison.OrdinalIgnoreCase))
				{
					pictureBox2.Image = (Image)(object)Resources.new_Logo;
					((Control)lab_devName).Text = "SimGM";
				}
				else if (info.DevName.Contains("GM1"))
				{
					pictureBox2.Image = (Image)(object)Resources.GM1_V2;
					((Control)lab_devName).Text = "GM1 V2";
				}
				else if (info.DevName.Contains("GM2"))
				{
					pictureBox2.Image = (Image)(object)Resources.new_Logo;
					((Control)lab_devName).Text = "GM2 V2";
				}
				else if (info.DevName.Contains("GM3"))
				{
					pictureBox2.Image = (Image)(object)Resources.GM3_V2;
					((Control)lab_devName).Text = "GM3 V2";
				}
				return true;
			}
			return result;
		}
		catch (Exception ex)
		{
			commModalFrm.SetAllTxt("Error", "An error occurred while updating device information.,desc=" + ex.Message, Color.Red);
			((Form)commModalFrm).ShowDialog();
			return result;
		}
	}

	private bool UpdateDevInfo(ResDeviceInfo info)
	{
		if (info == null)
		{
			return UpdateDevInfo((ResAscentInfo)null);
		}
		return UpdateDevInfo(new ResAscentInfo
		{
			SN = GetAsciiString(info.serialNumber),
			FWVers = GetAsciiString(info.firmwareInfo),
			HWVers = GetAsciiString(info.hardwareVersion),
			MCUTemp = info.cputemp,
			DevName = GetAsciiString(info.devicename),
			SDKVers = GetAsciiString(info.sdkversion),
			Details = GetAsciiString(info.detail),
			RecMaxSize = info.receiveMaxSize,
			UsbInfo = _resAsceInfo?.UsbInfo,
			UsbTime = DateTime.Now
		});
	}

	private string GetAsciiString(byte[] data)
	{
		return (data == null) ? string.Empty : Encoding.ASCII.GetString(data).TrimEnd(new char[1]);
	}

	private string ExtractFirmwareVersion(string firmwareVersion)
	{
		if (string.IsNullOrWhiteSpace(firmwareVersion))
		{
			return firmwareVersion;
		}
		Match match = Regex.Match(firmwareVersion, "V\\d+\\.\\d+\\.\\d+", RegexOptions.IgnoreCase);
		return match.Success ? match.Value : firmwareVersion;
	}

	private void BindEventHandler()
	{
		GD.Inst.SP_Gim.SerialConnectedStateChange += SerialConneStateChange;
		GD.Inst.Upg_Gim.OnUpgProcHappenEvent += OnUpgProcHappen;
		GD.Inst.Upg_Gim.DeviceFrameInfo += OnDeviceFrameInfo;
		GD.Inst.IsAutoRefreshGimData = true;
	}

	private void UnbindEventHandler()
	{
		if (GD.Inst.SP_Gim != null)
		{
			GD.Inst.SP_Gim.SerialConnectedStateChange -= SerialConneStateChange;
		}
		if (GD.Inst.Upg_Gim != null)
		{
			GD.Inst.Upg_Gim.OnUpgProcHappenEvent -= OnUpgProcHappen;
			GD.Inst.Upg_Gim.DeviceFrameInfo -= OnDeviceFrameInfo;
		}
		GD.Inst.IsAutoRefreshGimData = false;
	}

	private void lab_sensChan_MouseClick(object sender, MouseEventArgs e)
	{
		if (GD.Inst.IsOpenAscentUpg)
		{
			GD.Inst.Upg_Gim.Send_RebootClean();
		}
	}

	private void InitCtrlList()
	{
		for (int i = 0; i < 12; i++)
		{
			paramList.Add(0);
		}
		sel_portname.Items.Clear();
		sel_portname.Items.Add((object)_usbInfo.PortName);
		sel_portname.SelectedIndex = 0;
	}

	private void btn_refresh_Click(object sender, EventArgs e)
	{
		ManualSearchDevices(out _usbDevInfo);
		sel_portname.Items.Clear();
		for (int i = 0; i < _usbDevInfo.Count; i++)
		{
			sel_portname.Items.Add((object)_usbDevInfo[i].PortName);
		}
	}

	private void switch1_CheckedChanged(object sender, BoolEventArgs e)
	{
		if (CheckSerialportConnect())
		{
			float rollcmd = ((!((VEventArgs<bool>)(object)e).Value) ? 1 : 0);
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.AngProtect, rollcmd);
		}
	}

	private void btn_CloseGM_Click(object sender, EventArgs e)
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		try
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title = Lang.T("gimbal.disconnect_title");
			string desc = Lang.T("gimbal.disconnect_confirm_desc");
			string btnok = Lang.T("common.btn_confirm");
			string btnCan = Lang.T("common.btn_cancel");
			commModalFrm.SetAllTxt(title, desc, btnok, btnCan);
			((Form)commModalFrm).ShowDialog();
			if ((int)commModalFrm.FrmResult == 1)
			{
				UnbindEventHandler();
				GD.Inst.SP_Gim.Close();
				FrmEventArgs e2 = new FrmEventArgs
				{
					InfoType = InfoType.ctrlSign,
					Desc = "ManualCloseDevice",
					PortName = ((GD.Inst.SP_Gim.SPobj != null) ? GD.Inst.SP_Gim.SPobj.PortName : GD.Inst.CurrPortName)
				};
				OnGimFrmHappenEvnet?.Invoke(null, e2);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("CloseGMFrm error,desc=" + ex.Message);
		}
	}

	private void btn_Disconnect_Click_1(object sender, EventArgs e)
	{
		Button val = (Button)((sender is Button) ? sender : null);
		if (((Control)val).Text == Lang.T("gimbal.btn_disconnect"))
		{
			GD.Inst.SP_Gim.Close();
			((Control)val).Text = Lang.T("gimbal.btn_reconnect");
			val.DefaultBack = Color.FromArgb(255, 233, 0);
			val.ForeColor = Color.FromArgb(35, 35, 35);
		}
		else if (GD.Inst.SP_Gim.Open(GD.Inst.CurrPortName))
		{
			((Control)val).Text = Lang.T("gimbal.btn_disconnect");
			val.DefaultBack = Color.FromArgb(95, 95, 96);
			val.ForeColor = Color.FromArgb(255, 255, 255);
		}
	}

	private void OnLog(InfoType arg1, string arg2, object arg3)
	{
		Color black = Color.Black;
		switch (arg1)
		{
		case InfoType.debugInfo:
			black = Color.DarkBlue;
			break;
		case InfoType.none:
		case InfoType.info:
		case InfoType.warm:
			black = Color.LightGoldenrodYellow;
			break;
		case InfoType.fail:
		case InfoType.crash:
			black = Color.DarkRed;
			break;
		case InfoType.ctrlSign:
		case InfoType.upgProcInfo:
			break;
		}
	}

	private void inp_sensNum_VerifyChar(object sender, InputVerifyCharEventArgs e)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
		string numberDecimalSeparator = numberFormat.NumberDecimalSeparator;
		string numberGroupSeparator = numberFormat.NumberGroupSeparator;
		string negativeSign = numberFormat.NegativeSign;
		string text = e.Char.ToString();
		if (e.Char == '。' || e.Char == '.')
		{
			e.ReplaceText = ".";
			int num = ((Control)(Input)sender).Text.IndexOf('.');
			if (num < 0)
			{
				e.Result = true;
				return;
			}
			e.Result = false;
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("gimbal.only_one_decimal_point");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		int num2 = ((Control)(Input)sender).Text.IndexOf('.');
		if (char.IsDigit(e.Char))
		{
			e.Result = true;
		}
		else if (text.Equals(numberDecimalSeparator) && ((Control)(Input)sender).Text.IndexOf('.') < 0)
		{
			e.Result = true;
		}
		else if (e.Char == '\b')
		{
			e.Result = true;
		}
		else if (e.Char == '-')
		{
			int num3 = ((Control)(Input)sender).Text.IndexOf('-');
			if (num3 < 0)
			{
				e.Result = true;
			}
			else
			{
				e.Result = false;
			}
		}
		else
		{
			e.Result = false;
			string title2 = Lang.T("common.title_warning");
			string desc2 = Lang.T("gimbal.only_numbers_allowed");
			CommModalFrm commModalFrm2 = new CommModalFrm();
			commModalFrm2.SetAllTxt(title2, desc2, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm2).ShowDialog();
		}
	}

	private void inp_sensNum_Leave(object sender, EventArgs e)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckSerialportConnect())
		{
			return;
		}
		Input val = (Input)((sender is Input) ? sender : null);
		string text = ((Control)val).Text;
		if (string.IsNullOrEmpty(text))
		{
			WriteLog.WriteLogFileToUI("inp_sensNum=null", Color.OrangeRed);
			return;
		}
		float num = float.Parse(((Control)val).Text);
		if ((double)num > 0.5 || (double)num < -0.5)
		{
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("gimbal.sensitivity_out_of_range");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			if ((double)num < -0.5)
			{
				num = -0.5f;
			}
			else if ((double)num > 0.5)
			{
				num = 0.5f;
			}
			((Control)val).Text = num.ToString("f1");
		}
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetSensNum, num);
	}

	private void inp_rollNum_Leave(object sender, EventArgs e)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckSerialportConnect())
		{
			return;
		}
		Input val = (Input)((sender is Input) ? sender : null);
		string text = ((Control)val).Text;
		if (string.IsNullOrEmpty(text))
		{
			WriteLog.WriteLogFileToUI("inp_rollNum=null", Color.OrangeRed);
			return;
		}
		float num = float.Parse(((Control)val).Text);
		if (num > 55f || num < -55f)
		{
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("gimbal.roll_angle_out_of_range");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			if (num > 55f)
			{
				num = 55f;
			}
			else if (num < -55f)
			{
				num = -55f;
			}
			((Control)val).Text = num.ToString("f0");
		}
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetRollNum, num);
	}

	private void inp_pitchNum_Leave(object sender, EventArgs e)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckSerialportConnect())
		{
			return;
		}
		Input val = (Input)((sender is Input) ? sender : null);
		string text = ((Control)val).Text;
		if (string.IsNullOrEmpty(text))
		{
			WriteLog.WriteLogFileToUI("inp_pitchNum=null", Color.OrangeRed);
			return;
		}
		float num = float.Parse(((Control)val).Text);
		if (num > 90f || num < -90f)
		{
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("gimbal.pitch_angle_out_of_range");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			if (num > 90f)
			{
				num = 90f;
			}
			else if (num < -90f)
			{
				num = -90f;
			}
			((Control)val).Text = num.ToString("f0");
		}
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetPitchNum, num);
	}

	private void inp_yawNum_Leave(object sender, EventArgs e)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckSerialportConnect())
		{
			return;
		}
		Input val = (Input)((sender is Input) ? sender : null);
		string text = ((Control)val).Text;
		if (string.IsNullOrEmpty(text))
		{
			WriteLog.WriteLogFileToUI("inp_yawNum=null", Color.OrangeRed);
			return;
		}
		float num = float.Parse(((Control)val).Text);
		if (num > 150f || num < -150f)
		{
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("gimbal.yaw_out_of_range");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			if (num > 150f)
			{
				num = 150f;
			}
			else if (num < -150f)
			{
				num = -150f;
			}
			((Control)val).Text = num.ToString("f0");
		}
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetYawNum, num);
	}

	private void sel_gmMoed_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		if (CheckSerialportConnect())
		{
			Select val = (Select)((sender is Select) ? sender : null);
			if (((VEventArgs<int>)(object)e).Value > 0)
			{
				float val2 = 1f;
				GetCurrModeVal(((VEventArgs<int>)(object)e).Value, out val2, out var ss);
				GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetGMMode, val2);
				((Control)inp_currMode).Text = ss;
			}
		}
	}

	private void GetCurrModeVal(int idx, out float val, out string ss)
	{
		val = 4f;
		ss = "";
		switch (idx)
		{
		case 1:
			if (sel_M0.SelectedValue != null)
			{
				ss = sel_M0.SelectedValue.ToString();
				if (sel_M0.SelectedIndex == 0)
				{
					val = 7f;
				}
				else if (sel_M0.SelectedIndex == 1)
				{
					val = 6f;
				}
				else if (sel_M0.SelectedIndex == 2)
				{
					val = 1f;
				}
				else if (sel_M0.SelectedIndex == 3)
				{
					val = 4f;
				}
				else if (sel_M0.SelectedIndex == 4)
				{
					val = 9f;
				}
			}
			break;
		case 2:
			if (sel_M1.SelectedValue != null)
			{
				ss = sel_M1.SelectedValue.ToString();
				if (sel_M1.SelectedIndex == 0)
				{
					val = 7f;
				}
				else if (sel_M1.SelectedIndex == 1)
				{
					val = 6f;
				}
				else if (sel_M1.SelectedIndex == 2)
				{
					val = 1f;
				}
				else if (sel_M1.SelectedIndex == 3)
				{
					val = 4f;
				}
				else if (sel_M1.SelectedIndex == 4)
				{
					val = 9f;
				}
			}
			break;
		case 3:
			if (sel_M2.SelectedValue != null)
			{
				ss = sel_M2.SelectedValue.ToString();
				if (sel_M2.SelectedIndex == 0)
				{
					val = 7f;
				}
				else if (sel_M2.SelectedIndex == 1)
				{
					val = 6f;
				}
				else if (sel_M2.SelectedIndex == 2)
				{
					val = 1f;
				}
				else if (sel_M2.SelectedIndex == 3)
				{
					val = 4f;
				}
				else if (sel_M2.SelectedIndex == 4)
				{
					val = 9f;
				}
			}
			break;
		}
	}

	private void OnDeviceFrameInfo(bool arg1, RecvPacket_Gim arg2, object arg3)
	{
		if (arg1)
		{
			RefreshTxtbox(arg2);
		}
	}

	private void OnUpgProcHappen(object sender, HappenEventArgs e)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			Color black = Color.Black;
			switch (e.infoType)
			{
			case InfoType.debugInfo:
				black = Color.DarkBlue;
				break;
			case InfoType.none:
			case InfoType.info:
			case InfoType.warm:
				black = Color.LightGoldenrodYellow;
				break;
			case InfoType.fail:
				if (e.msg == "CalibGyro timeout")
				{
					pictureBox1.Image = (Image)(object)Resources.升级成功;
					((Control)lab_upgDesc).Text = Lang.T("gimbal.gyro_calib_completed");
					progress2.State = (TType)1;
					progress2.Value = 1f;
					progress2.Fill = Color.FromArgb(43, 164, 113);
					GridPanel obj2 = grpan_chann;
					GridPanel obj3 = grpan_param;
					bool flag = (((IControl)grpan_gain).Enabled = true);
					bool enabled = (((IControl)obj3).Enabled = flag);
					((IControl)obj2).Enabled = enabled;
					_isCalibGryo = false;
					EnableCtrl(isEnable: true);
				}
				else
				{
					((Control)lab_upgDesc).Text = e.className;
					progress2.Fill = Color.FromArgb(255, 63, 63);
					progress2.State = (TType)4;
					pictureBox1.Image = (Image)(object)Resources.升级失败;
					CommModalFrm commModalFrm = new CommModalFrm();
					commModalFrm.SetAllTxt(Lang.T("common.title_error"), e.msg, Color.Red);
					((Form)commModalFrm).ShowDialog();
				}
				break;
			case InfoType.crash:
				black = Color.DarkRed;
				break;
			case InfoType.upgProcInfo:
				if (e.processVal > progress2.Value)
				{
					progress2.Value = e.processVal;
				}
				if (e.msg == "procFinish")
				{
					pictureBox1.Image = (Image)(object)Resources.升级成功;
					((Control)lab_upgDesc).Text = e.processDesc;
					progress2.State = (TType)1;
					progress2.Fill = Color.FromArgb(43, 164, 113);
					GD inst = GD.Inst;
					GridPanel obj4 = grpan_chann;
					GridPanel obj5 = grpan_param;
					bool flag4 = (((IControl)grpan_gain).Enabled = true);
					bool flag = (((IControl)obj5).Enabled = flag4);
					bool enabled = (((IControl)obj4).Enabled = flag);
					inst.IsAutoRefreshGimData = enabled;
					_isStartUpg = false;
					EnableCtrl(!_isStartUpg);
					GD.Inst.Upg_Gim.Send_FindDevice();
				}
				else if (e.msg == "UpgradeComplet" && sender is ResDeviceInfo info)
				{
					UpdateDevInfo(info);
				}
				break;
			case InfoType.procesInfo:
				if (e.msg == "CalibGyro")
				{
					if (progress2.Value < 0.95f)
					{
						Progress obj = progress2;
						obj.Value += 0.005f;
					}
				}
				else if (e.msg == "CalibGyroFinsh")
				{
					((Control)lab_upgDesc).Text = Lang.T("gimbal.gyro_calibration_completed");
					progress2.Fill = Color.FromArgb(43, 164, 113);
					pictureBox1.Image = (Image)(object)Resources.升级成功;
					progress2.State = (TType)1;
					progress2.Value = 1f;
					_isCalibGryo = false;
					EnableCtrl(isEnable: true);
				}
				break;
			case InfoType.ctrlSign:
				break;
			}
		});
	}

	private void SerialConneStateChange(bool b, string arg1, int arg2)
	{
		if (!_isStartUpg)
		{
			if (!b)
			{
				GD.Inst.UsbFSM?.Close();
			}
			FrmEventArgs e = new FrmEventArgs
			{
				InfoType = InfoType.ctrlSign,
				Desc = (b ? "Connect" : "Disconnect"),
				PortName = arg1,
				IsOpen = b
			};
			OnGimFrmHappenEvnet?.Invoke(arg1, e);
		}
	}

	private void NewGimbalFrm_FormClosing(object sender, FormClosingEventArgs e)
	{
	}

	private void RefreshTxtbox(RecvPacket_Gim pac)
	{
		_isModeChannChange = (_isSenseChannChange = false);
		((Control)this).BeginInvoke((Delegate)(Action)delegate
		{
			((Control)inp_rollGain).Text = pac.rollgain.ToString("f0");
			((Control)inp_pitchGain).Text = pac.pitchgain.ToString("f0");
			((Control)inp_yawGain).Text = pac.yawgain.ToString("f0");
			sel_modeChan.SelectedIndex = ((pac.modeChann > 0) ? pac.modeChann : 0);
			sel_senseChan.SelectedIndex = ((pac.sensChann > 0) ? pac.sensChann : 0);
			sel_rollChan.SelectedIndex = ((pac.rollChann > 0) ? pac.rollChann : 0);
			sel_pitchChan.SelectedIndex = ((pac.pitchChann > 0) ? pac.pitchChann : 0);
			sel_yawChan.SelectedIndex = ((pac.yawChann > 0) ? pac.yawChann : 0);
			_currPac.currMode = pac.currMode;
			string text = Lang.T("gimbal.gimbal_stop");
			switch (pac.currMode)
			{
			case 1:
				text = Lang.T("gimbal.mode_horizon");
				break;
			case 4:
				text = Lang.T("gimbal.mode_lookdown");
				break;
			case 6:
				text = Lang.T("gimbal.mode_pitch_stab");
				break;
			case 7:
				text = Lang.T("gimbal.mode_fpv");
				break;
			case 9:
				text = Lang.T("gimbal.mode_fpv_lookdown");
				break;
			}
			((Control)inp_currMode).Text = text;
			int selectedIndex = 0;
			switch (pac.m0Mode)
			{
			case 1:
				selectedIndex = 2;
				break;
			case 4:
				selectedIndex = 3;
				break;
			case 6:
				selectedIndex = 1;
				break;
			case 7:
				selectedIndex = 0;
				break;
			case 9:
				selectedIndex = 4;
				break;
			}
			sel_M0.SelectedIndex = selectedIndex;
			int selectedIndex2 = 0;
			switch (pac.m1Mode)
			{
			case 1:
				selectedIndex2 = 2;
				break;
			case 4:
				selectedIndex2 = 3;
				break;
			case 6:
				selectedIndex2 = 1;
				break;
			case 7:
				selectedIndex2 = 0;
				break;
			case 9:
				selectedIndex2 = 4;
				break;
			}
			sel_M1.SelectedIndex = selectedIndex2;
			int selectedIndex3 = 0;
			switch (pac.m2Mode)
			{
			case 1:
				selectedIndex3 = 2;
				break;
			case 4:
				selectedIndex3 = 3;
				break;
			case 6:
				selectedIndex3 = 1;
				break;
			case 7:
				selectedIndex3 = 0;
				break;
			case 9:
				selectedIndex3 = 4;
				break;
			}
			sel_M2.SelectedIndex = selectedIndex3;
			switch1.Checked = ((pac.angleProtectEnable != 0) ? true : false);
			sel_LensType.SelectedIndex = pac.lensProfile;
		});
		if (paramList[0] != pac.modeChann)
		{
			paramList[0] = pac.modeChann;
		}
		if (paramList[1] != pac.sensChann)
		{
			paramList[1] = pac.sensChann;
		}
		if (paramList[2] != pac.rollChann)
		{
			paramList[2] = pac.rollChann;
		}
		if (paramList[3] != pac.pitchChann)
		{
			paramList[3] = pac.pitchChann;
		}
		if (paramList[4] != pac.yawChann)
		{
			paramList[4] = pac.yawChann;
		}
		if (paramList[5] != pac.rollgain)
		{
			paramList[5] = pac.rollgain;
		}
		if (paramList[6] != pac.pitchgain)
		{
			paramList[6] = pac.pitchgain;
		}
		if (paramList[7] != pac.yawgain)
		{
			paramList[7] = pac.yawgain;
		}
		if (paramList[8] != pac.m0Mode)
		{
			paramList[8] = pac.m0Mode;
		}
		if (paramList[9] != pac.m1Mode)
		{
			paramList[9] = pac.m1Mode;
		}
		if (paramList[10] != pac.m2Mode)
		{
			paramList[10] = pac.m2Mode;
		}
		if (paramList[11] != pac.lensProfile)
		{
			paramList[11] = pac.lensProfile;
		}
	}

	private bool CheckSerialportConnect()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
			{
				string title = Lang.T("common.title_warning");
				string desc = Lang.T("gimbal.connect_gimbal_first");
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
				((Form)commModalFrm).ShowDialog();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void btn_startGM_Click(object sender, EventArgs e)
	{
		if (CheckSerialportConnect())
		{
			if (((Control)btn_startGM).Text == Lang.T("gimbal.btn_stop_gimbal"))
			{
				_currCMD = 1;
				GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.StopGim);
				((Control)btn_startGM).Text = Lang.T("gimbal.btn_start_gimbal");
			}
			else
			{
				_currCMD = 0;
				GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.StartGim);
				((Control)btn_startGM).Text = Lang.T("gimbal.btn_stop_gimbal");
			}
		}
	}

	private void btn_stopGM_Click(object sender, EventArgs e)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
		{
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("gimbal.connect_gimbal_first");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
		}
		else
		{
			_currCMD = 1;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.StopGim);
		}
	}

	private void btn_calib_Click(object sender, EventArgs e)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		if (!CheckSerialportConnect())
		{
			return;
		}
		if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title = Lang.T("common.title_prompt");
			string desc = Lang.T("gimbal.gyro_calib_tip");
			CommModalFrm commModalFrm2 = new CommModalFrm();
			commModalFrm2.SetAllTxt(title, desc);
			((Form)commModalFrm2).ShowDialog();
			if ((int)commModalFrm2.FrmResult != 1)
			{
				return;
			}
		}
		GD.Inst.SW_Calib.Restart();
		((Control)lab_upgDesc).Text = Lang.T("gimbal.gyro_in_calibration");
		pictureBox1.Image = (Image)(object)Resources.升级中;
		((IControl)grpan_process).Visible = true;
		progress2.State = (TType)0;
		progress2.Value = 0.02f;
		progress2.Fill = Color.FromArgb(255, 233, 0);
		WriteLog.WriteLogFileToUI("开始校准陀螺仪", Color.DarkBlue);
		_isCalibGryo = true;
		EnableCtrl(!_isCalibGryo);
		for (int i = 0; i < 4; i++)
		{
			GD.Inst.Upg_Gim.BuildSendPacket_Delay(GIM_CMD.StopGim, 100);
		}
		_currCMD = 2;
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.CalibGyro);
	}

	private void btn_posCalib_Click(object sender, EventArgs e)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (CheckSerialportConnect())
		{
			if (_currPac.currMode != 7 && _currPac.currMode != 9)
			{
				string title = Lang.T("common.title_warning");
				string desc = Lang.T("gimbal.pos_calib_requires_fpv_mode");
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
				((Form)commModalFrm).ShowDialog();
			}
			else
			{
				_currCMD = 36;
				GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.PosCalib);
			}
		}
	}

	private void EnableCtrl(bool isEnable)
	{
		UploadDragger obj = uploadDragger1;
		Select obj2 = sel_gmMoed;
		Input obj3 = inp_sensNum;
		Input obj4 = inp_rollNum;
		Button obj5 = btn_refresh;
		Input obj6 = inp_pitchNum;
		Input obj7 = inp_yawNum;
		GridPanel obj8 = grpan_chann;
		GridPanel obj9 = grpan_gain;
		GridPanel obj10 = grpan_param;
		Button obj11 = btn_Disconnect;
		Select obj12 = sel_M0;
		Select obj13 = sel_M1;
		bool flag = (((IControl)sel_M2).Enabled = isEnable);
		bool flag3 = (((IControl)obj13).Enabled = flag);
		bool flag5 = (((IControl)obj12).Enabled = flag3);
		bool flag7 = (((IControl)obj11).Enabled = flag5);
		bool flag9 = (((IControl)obj10).Enabled = flag7);
		bool flag11 = (((IControl)obj9).Enabled = flag9);
		bool flag13 = (((IControl)obj8).Enabled = flag11);
		bool flag15 = (((IControl)obj7).Enabled = flag13);
		bool flag17 = (((IControl)obj6).Enabled = flag15);
		bool flag19 = (((IControl)obj5).Enabled = flag17);
		bool flag21 = (((IControl)obj4).Enabled = flag19);
		bool flag23 = (((IControl)obj3).Enabled = flag21);
		bool enabled = (((IControl)obj2).Enabled = flag23);
		((IControl)obj).Enabled = enabled;
	}

	private void btn_Disconnect_Click(object sender, EventArgs e)
	{
	}

	private async void btn_openParam_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog ofd = new OpenFileDialog();
			((FileDialog)ofd).Title = Lang.T("gimbal.select_param_file");
			((FileDialog)ofd).InitialDirectory = "C:\\Documents";
			((FileDialog)ofd).Filter = "Parameter File (*.json)|*.json";
			((FileDialog)ofd).FilterIndex = 1;
			((FileDialog)ofd).RestoreDirectory = true;
			ofd.Multiselect = false;
			if ((int)((CommonDialog)ofd).ShowDialog() != 1)
			{
				return;
			}
			string jsonString = File.ReadAllText(((FileDialog)ofd).FileName);
			if (string.IsNullOrEmpty(jsonString))
			{
				WriteLog.WriteLogFileToUI("参数文件内容为空", Color.Red);
				return;
			}
			GD.Inst.IsAutoRefreshGimData = false;
			JObject jsonObj = JObject.Parse(jsonString);
			paramList.Clear();
			List<int> list = paramList;
			JToken obj = jsonObj["mode"];
			list.Add((obj != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj) : 0);
			List<int> list2 = paramList;
			JToken obj2 = jsonObj["sen"];
			list2.Add((obj2 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj2) : 0);
			List<int> list3 = paramList;
			JToken obj3 = jsonObj["channelLR"];
			list3.Add((obj3 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj3) : 0);
			List<int> list4 = paramList;
			JToken obj4 = jsonObj["channelP"];
			list4.Add((obj4 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj4) : 0);
			List<int> list5 = paramList;
			JToken obj5 = jsonObj["channelY"];
			list5.Add((obj5 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj5) : 0);
			List<int> list6 = paramList;
			JToken obj6 = jsonObj["rollgain"];
			list6.Add((obj6 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj6) : 0);
			List<int> list7 = paramList;
			JToken obj7 = jsonObj["pitchgain"];
			list7.Add((obj7 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj7) : 0);
			List<int> list8 = paramList;
			JToken obj8 = jsonObj["yawgain"];
			list8.Add((obj8 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj8) : 0);
			List<int> list9 = paramList;
			JToken obj9 = jsonObj["modePresetM0"];
			list9.Add((obj9 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj9) : 0);
			List<int> list10 = paramList;
			JToken obj10 = jsonObj["modePresetM1"];
			list10.Add((obj10 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj10) : 0);
			List<int> list11 = paramList;
			JToken obj11 = jsonObj["modePresetM2"];
			list11.Add((obj11 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj11) : 0);
			List<int> list12 = paramList;
			JToken obj12 = jsonObj["LensType"];
			list12.Add((obj12 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj12) : 0);
			for (int i = 0; i < paramList.Count; i++)
			{
				WriteLog.WriteLogFileToUI($"param[{i}]={paramList[i]}", Color.Black);
				await Task.Delay(2);
			}
			sel_modeChan.SelectedIndex = paramList[0];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetModeChann, paramList[0]);
			await Task.Delay(20);
			sel_senseChan.SelectedIndex = paramList[1];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetSensChann, paramList[1]);
			await Task.Delay(20);
			sel_rollChan.SelectedIndex = paramList[2];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetRollChann, paramList[2]);
			await Task.Delay(20);
			sel_pitchChan.SelectedIndex = paramList[3];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetPitchChann, paramList[3]);
			await Task.Delay(20);
			sel_yawChan.SelectedIndex = paramList[4];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetYawChann, paramList[4]);
			await Task.Delay(20);
			((Control)inp_rollGain).Text = paramList[5].ToString();
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetRollGain, paramList[5]);
			await Task.Delay(20);
			((Control)inp_pitchGain).Text = paramList[6].ToString();
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetPitchGain, 0f, paramList[6]);
			await Task.Delay(20);
			((Control)inp_yawGain).Text = paramList[7].ToString();
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetYawGain, 0f, 0f, paramList[7]);
			await Task.Delay(20);
			switch (paramList[8])
			{
			case 7:
				sel_M0.SelectedIndex = 0;
				break;
			case 6:
				sel_M0.SelectedIndex = 1;
				break;
			case 4:
				sel_M0.SelectedIndex = 3;
				break;
			case 1:
				sel_M0.SelectedIndex = 2;
				break;
			case 9:
				sel_M0.SelectedIndex = 4;
				break;
			}
			switch (paramList[9])
			{
			case 7:
				sel_M1.SelectedIndex = 0;
				break;
			case 6:
				sel_M1.SelectedIndex = 1;
				break;
			case 4:
				sel_M1.SelectedIndex = 3;
				break;
			case 1:
				sel_M1.SelectedIndex = 2;
				break;
			case 9:
				sel_M1.SelectedIndex = 4;
				break;
			}
			switch (paramList[10])
			{
			case 7:
				sel_M2.SelectedIndex = 0;
				break;
			case 6:
				sel_M2.SelectedIndex = 1;
				break;
			case 4:
				sel_M2.SelectedIndex = 3;
				break;
			case 1:
				sel_M2.SelectedIndex = 2;
				break;
			case 9:
				sel_M2.SelectedIndex = 4;
				break;
			}
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.ModeSlot, paramList[8], paramList[9], paramList[10]);
			await Task.Delay(20);
			GD.Inst.IsAutoRefreshGimData = true;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			WriteLog.WriteLogFileToUI("err desc=" + ex2.Message, Color.Red);
			GD.Inst.IsAutoRefreshGimData = true;
		}
	}

	private void btn_saveParam_Click(object sender, EventArgs e)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Invalid comparison between Unknown and I4
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
		{
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("gimbal.connect_gimbal_first");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		SaveFileDialog val = new SaveFileDialog();
		try
		{
			((FileDialog)val).Title = Lang.T("gimbal.save_gimbal_params");
			((FileDialog)val).Filter = "JSON Files (*.json)|*.json";
			((FileDialog)val).FilterIndex = 1;
			((FileDialog)val).RestoreDirectory = true;
			if ((int)((CommonDialog)val).ShowDialog() == 1)
			{
				JObject val2 = new JObject();
				val2["mode"] = JToken.op_Implicit(paramList[0]);
				val2["sen"] = JToken.op_Implicit(paramList[1]);
				val2["channelLR"] = JToken.op_Implicit(paramList[2]);
				val2["channelP"] = JToken.op_Implicit(paramList[3]);
				val2["channelY"] = JToken.op_Implicit(paramList[4]);
				val2["rollgain"] = JToken.op_Implicit(paramList[5]);
				val2["pitchgain"] = JToken.op_Implicit(paramList[6]);
				val2["yawgain"] = JToken.op_Implicit(paramList[7]);
				val2["modePresetM0"] = JToken.op_Implicit(paramList[8]);
				val2["modePresetM1"] = JToken.op_Implicit(paramList[9]);
				val2["modePresetM2"] = JToken.op_Implicit(paramList[10]);
				val2["LensType"] = JToken.op_Implicit(paramList[11]);
				string contents = ((JToken)val2).ToString((Formatting)1);
				File.WriteAllText(((FileDialog)val).FileName, contents);
				CommModalFrm commModalFrm2 = new CommModalFrm();
				commModalFrm2.SetAllTxt(Lang.T("common.title_prompt"), Lang.T("gimbal.save_param_success"), isshowBtnOK: true, isshowBtnCan: false);
				((Form)commModalFrm2).ShowDialog();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void btn_WriteParam_Click(object sender, EventArgs e)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
		{
			string title = Lang.T("common.title_warning");
			string desc = Lang.T("gimbal.connect_gimbal_first");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		GD.Inst.IsAutoRefreshGimData = false;
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.ModeSlot, _modeSlotCMD[0], _modeSlotCMD[1], _modeSlotCMD[2]);
		float rollcmd = ((!switch1.Checked) ? 1 : 0);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.AngProtect, rollcmd);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.WriteParam);
		string title2 = Lang.T("common.title_prompt");
		string desc2 = Lang.T("gimbal.write_param_success");
		CommModalFrm commModalFrm2 = new CommModalFrm();
		commModalFrm2.SetAllTxt(title2, desc2, isshowBtnOK: true, isshowBtnCan: false);
		((Form)commModalFrm2).ShowDialog();
		GD.Inst.IsAutoRefreshGimData = true;
	}

	private void uploadDragger1_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Invalid comparison between Unknown and I4
		try
		{
			OpenFileDialog val = new OpenFileDialog();
			try
			{
				((FileDialog)val).Title = Lang.T("gimbal.select_firmware");
				((FileDialog)val).InitialDirectory = "D:";
				((FileDialog)val).Filter = "firmware (*.bin)|*.bin|All (*.*)|*.*";
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
					((Control)uploadDragger1).Text = (_filePath = path);
					((IControl)btn_startUpg).Enabled = true;
				});
				string version = GetVersion(path, b: false);
				string text = path.ToLower();
				if (text.Contains("g_sky") && (text.Contains("z8") || text.Contains("z40")))
				{
					Match match = Regex.Match(path, "(\\d+)_(\\d+)_(\\d+)(?=(?:_\\w+)*\\.img$)");
					string value = match.Groups[0].Value;
					if (!match.Success)
					{
						_currVer = "V_unRecog";
					}
					else
					{
						_currVer = "V" + value.Replace("_", ".");
					}
					return;
				}
				Match match2 = Regex.Match(((FileDialog)val).FileName, "(\\d+[\\._]\\d+[\\._]\\d+\\.img)");
				if (!match2.Success)
				{
					_currVer = "V_unRecog";
				}
				else if (match2.Groups[1].Value.Contains("_"))
				{
					_currVer = ("V" + match2.Groups[1].Value).Replace("_", ".").Replace(".img", "");
				}
				else if (match2.Groups[1].Value.Contains("."))
				{
					_currVer = ("V" + match2.Groups[1].Value).Replace(".img", "");
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("文件拖入处理失败：" + ex.Message, Color.Red);
		}
	}

	private void btn_startUpg_Click(object sender, EventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckSerialportConnect())
		{
			return;
		}
		StartUpgAscentModalFrm startUpgAscentModalFrm = new StartUpgAscentModalFrm();
		((Form)startUpgAscentModalFrm).ShowDialog();
		if ((int)startUpgAscentModalFrm.result == 1)
		{
			if (GD.Inst.IsOpenAscentUpg)
			{
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(Lang.T("common.title_prompt"), Lang.T("gimbal.ascent_upgrade_mechanism"));
				((Form)commModalFrm).ShowDialog();
			}
			_isStartUpg = true;
			pictureBox1.Image = (Image)(object)Resources.升级中;
			((Control)lab_upgDesc).Text = Lang.T("gimbal.start_upgrading");
			EnableCtrl(!_isStartUpg);
			((IControl)btn_startUpg).Enabled = false;
			progress2.State = (TType)0;
			progress2.Fill = Color.FromArgb(255, 233, 0);
			GD.Inst.IsAutoRefreshGimData = false;
			((IControl)grpan_process).Visible = true;
			progress2.Value = 0.02f;
			GD.Inst.Upg_Gim.StartUpgrade(_filePath);
		}
	}

	private void uploadDragger1_DragChanged(object sender, StringsEventArgs e)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string[] value = ((VEventArgs<string[]>)(object)e).Value;
			string text = value[0];
			FileAttributes attributes = File.GetAttributes(text);
			if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
			{
				string title = Lang.T("common.title_warning");
				string desc = Lang.T("gimbal.drag_firmware_only");
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
				((Form)commModalFrm).ShowDialog();
			}
			else
			{
				((Control)uploadDragger1).Text = (_filePath = text);
				((IControl)btn_startUpg).Enabled = true;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("文件拖入处理失败：" + ex.Message, Color.Red);
		}
	}

	private void lab_modeChan_Click(object sender, EventArgs e)
	{
		if (GD.Inst.IsOpenAscentUpg)
		{
			GD.Inst.Upg_Gim.Send_RebootClean();
		}
	}

	private void inp_modeChan_Leave(object sender, EventArgs e)
	{
		InputNumber val = (InputNumber)((sender is InputNumber) ? sender : null);
		float rollcmd = float.Parse(((Control)val).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetModeChann, rollcmd);
	}

	private void inp_sensChan_Leave(object sender, EventArgs e)
	{
		InputNumber val = (InputNumber)((sender is InputNumber) ? sender : null);
		float rollcmd = float.Parse(((Control)val).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetSensChann, rollcmd);
	}

	private void inp_rollChan_Leave(object sender, EventArgs e)
	{
		InputNumber val = (InputNumber)((sender is InputNumber) ? sender : null);
		float rollcmd = float.Parse(((Control)val).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetRollChann, rollcmd);
	}

	private void inp_pitchChan_Leave(object sender, EventArgs e)
	{
		InputNumber val = (InputNumber)((sender is InputNumber) ? sender : null);
		float rollcmd = float.Parse(((Control)val).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetPitchChann, rollcmd);
	}

	private void inp_yawChan_Leave(object sender, EventArgs e)
	{
		InputNumber val = (InputNumber)((sender is InputNumber) ? sender : null);
		float rollcmd = float.Parse(((Control)val).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetYawChann, rollcmd);
	}

	private void lap_rollGain_MouseClick(object sender, MouseEventArgs e)
	{
	}

	private void lab_pitchGain_MouseClick(object sender, MouseEventArgs e)
	{
		float pitchcmd = float.Parse(((Control)inp_pitchGain).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetPitchGain, 0f, pitchcmd);
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

	private void inp_rollGain_Leave(object sender, EventArgs e)
	{
		if (CheckSerialportConnect())
		{
			Input val = (Input)((sender is Input) ? sender : null);
			string text = ((Control)val).Text;
			if (string.IsNullOrEmpty(text))
			{
				WriteLog.WriteLogFileToUI("inp_rollGain=null", Color.OrangeRed);
				return;
			}
			float rollcmd = float.Parse(((Control)val).Text);
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetRollGain, rollcmd);
		}
	}

	private void inp_pitchGain_Leave(object sender, EventArgs e)
	{
		if (CheckSerialportConnect())
		{
			Input val = (Input)((sender is Input) ? sender : null);
			string text = ((Control)val).Text;
			if (string.IsNullOrEmpty(text))
			{
				WriteLog.WriteLogFileToUI("inp_pitchGain=null", Color.OrangeRed);
				return;
			}
			float pitchcmd = float.Parse(((Control)val).Text);
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetPitchGain, 0f, pitchcmd);
		}
	}

	private void inp_yawGain_Leave(object sender, EventArgs e)
	{
		if (CheckSerialportConnect())
		{
			Input val = (Input)((sender is Input) ? sender : null);
			float yawcmd = float.Parse(((Control)val).Text);
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetYawGain, 0f, 0f, yawcmd);
		}
	}

	private void inp_rollGain_VerifyChar(object sender, InputVerifyCharEventArgs e)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
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
		string title = Lang.T("common.title_warning");
		string desc = Lang.T("gimbal.only_numbers_allowed");
		CommModalFrm commModalFrm = new CommModalFrm();
		commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
		((Form)commModalFrm).ShowDialog();
	}

	private void sel_modeChan_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_modeChan.SelectedIndexChanged -= new IntEventHandler(sel_modeChan_SelectedIndexChanged);
		if (CheckSerialportConnect() && _isModeChannChange)
		{
			_isModeChannChange = false;
			float rollcmd = ((VEventArgs<int>)(object)e).Value;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetModeChann, rollcmd);
		}
	}

	private void sel_senseChan_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_senseChan.SelectedIndexChanged -= new IntEventHandler(sel_senseChan_SelectedIndexChanged);
		if (CheckSerialportConnect() && _isSenseChannChange)
		{
			_isSenseChannChange = false;
			float rollcmd = ((VEventArgs<int>)(object)e).Value;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetSensChann, rollcmd);
		}
	}

	private void sel_modeChan_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_modeChan.SelectedIndexChanged += new IntEventHandler(sel_modeChan_SelectedIndexChanged);
		_isModeChannChange = true;
	}

	private void sel_senseChan_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_senseChan.SelectedIndexChanged += new IntEventHandler(sel_senseChan_SelectedIndexChanged);
		_isSenseChannChange = true;
	}

	private void lab_selTitle_MouseClick(object sender, MouseEventArgs e)
	{
	}

	private void lab_versChange_MouseClick(object sender, MouseEventArgs e)
	{
	}

	private void lab_selTitle_Click(object sender, EventArgs e)
	{
	}

	private void sel_rollChan_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		sel_rollChan.SelectedIndexChanged -= new IntEventHandler(sel_rollChan_SelectedIndexChanged);
		if (CheckSerialportConnect() && _isRollChannChange)
		{
			_isRollChannChange = false;
			sel_rollChan.SelectedIndexChanged -= new IntEventHandler(sel_rollChan_SelectedIndexChanged);
			float rollcmd = ((VEventArgs<int>)(object)e).Value;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetRollChann, rollcmd);
		}
	}

	private void sel_pitchChan_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_pitchChan.SelectedIndexChanged += new IntEventHandler(sel_pitchChan_SelectedIndexChanged);
		_isPitchChannChange = true;
	}

	private void sel_yawChan_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_yawChan.SelectedIndexChanged += new IntEventHandler(sel_yawChan_SelectedIndexChanged);
		_isYawChannChange = true;
	}

	private void sel_pitchChan_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_pitchChan.SelectedIndexChanged -= new IntEventHandler(sel_pitchChan_SelectedIndexChanged);
		if (CheckSerialportConnect() && _isPitchChannChange)
		{
			_isPitchChannChange = false;
			float rollcmd = ((VEventArgs<int>)(object)e).Value;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetPitchChann, rollcmd);
		}
	}

	private void sel_yawChan_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_yawChan.SelectedIndexChanged -= new IntEventHandler(sel_yawChan_SelectedIndexChanged);
		if (CheckSerialportConnect() && _isYawChannChange)
		{
			_isYawChannChange = false;
			float rollcmd = ((VEventArgs<int>)(object)e).Value;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetYawChann, rollcmd);
		}
	}

	private void sel_rollChan_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_rollChan.SelectedIndexChanged += new IntEventHandler(sel_rollChan_SelectedIndexChanged);
		_isRollChannChange = true;
	}

	private void sel_M0_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		_isChangeModeSlot = true;
		float num = 7f;
		switch (((VEventArgs<int>)(object)e).Value)
		{
		case 0:
			num = 7f;
			break;
		case 1:
			num = 6f;
			break;
		case 2:
			num = 1f;
			break;
		case 3:
			num = 4f;
			break;
		case 4:
			num = 9f;
			break;
		}
		_modeSlotCMD[0] = num;
		paramList[8] = (int)num;
	}

	private void sel_M1_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		_isChangeModeSlot = true;
		float num = 7f;
		switch (((VEventArgs<int>)(object)e).Value)
		{
		case 0:
			num = 7f;
			break;
		case 1:
			num = 6f;
			break;
		case 2:
			num = 1f;
			break;
		case 3:
			num = 4f;
			break;
		case 4:
			num = 9f;
			break;
		}
		_modeSlotCMD[1] = num;
		paramList[9] = (int)num;
	}

	private void sel_M2_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		_isChangeModeSlot = true;
		float num = 7f;
		switch (((VEventArgs<int>)(object)e).Value)
		{
		case 0:
			num = 7f;
			break;
		case 1:
			num = 6f;
			break;
		case 2:
			num = 1f;
			break;
		case 3:
			num = 4f;
			break;
		case 4:
			num = 9f;
			break;
		}
		_modeSlotCMD[2] = num;
		paramList[10] = (int)num;
	}

	private void sel_LensType_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_LensType.SelectedIndexChanged += new IntEventHandler(sel_LensType_SelectedIndexChanged);
	}

	private void sel_LensType_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		sel_LensType.SelectedIndexChanged -= new IntEventHandler(sel_LensType_SelectedIndexChanged);
		float rollcmd = ((VEventArgs<int>)(object)e).Value;
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(GIM_CMD.SetCamType, rollcmd);
	}

	public void GetCurrentUsbDevInfo(out UsbDevInfo devInfo)
	{
		devInfo = _resAsceInfo.UsbInfo;
	}

	private void inp_Min_EnterDown(object sender, KeyEventArgs e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		if (!CheckSerialportConnect())
		{
			return;
		}
		try
		{
			if ((int)e.KeyCode != 13)
			{
				return;
			}
			Input val = (Input)sender;
			string name = ((Control)val).Name;
			float result = 0f;
			if (!float.TryParse(((Control)val).Text, out result))
			{
				return;
			}
			if (name == "inp_rollGain")
			{
				inp_rollGain_Leave(sender, null);
				return;
			}
			if (name == "inp_pitchGain")
			{
				inp_pitchGain_Leave(sender, null);
				return;
			}
			if (name == "inp_yawGain")
			{
				inp_yawGain_Leave(sender, null);
				return;
			}
			if (name == "inp_sensNum")
			{
				inp_sensNum_Leave(sender, null);
				return;
			}
			switch (name)
			{
			case "inp_sensNum":
				inp_sensNum_Leave(sender, null);
				break;
			case "inp_rollNum":
				inp_rollNum_Leave(sender, null);
				break;
			case "inp_pitchNum":
				inp_pitchNum_Leave(sender, null);
				break;
			case "inp_yawNum":
				inp_yawNum_Leave(sender, null);
				break;
			}
		}
		catch (Exception)
		{
		}
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

	public void ReloadLang(bool isReloadFont = false)
	{
		try
		{
			((Control)btn_startGM).Text = Lang.T("gimbal.btn_stop_gimbal");
			((Control)btn_posCalib).Text = Lang.T("gimbal.btn_pos_calib");
			((Control)btn_gryoCalib).Text = Lang.T("gimbal.btn_gyro_calib");
			((Control)lab_modeChan).Text = Lang.T("gimbal.mode_chan");
			((Control)lab_sensChan).Text = Lang.T("gimbal.sensitivity_chan");
			((Control)lab_rollChan).Text = Lang.T("gimbal.roll_chan");
			((Control)lab_pitchChan).Text = Lang.T("gimbal.pitch_chan");
			((Control)lab_yawChan).Text = Lang.T("gimbal.yaw_chan");
			((Control)lab_rollGain).Text = Lang.T("gimbal.roll_gain");
			((Control)lab_pitchGain).Text = Lang.T("gimbal.pitch_gain");
			((Control)lab_yawGain).Text = Lang.T("gimbal.yaw_gain");
			((Control)btn_openParam).Text = Lang.T("gimbal.btn_load_param");
			((Control)btn_saveParam).Text = Lang.T("gimbal.btn_save_param");
			((Control)btn_WriteParam).Text = Lang.T("gimbal.btn_write_param");
			((Control)btn_startUpg).Text = Lang.T("gimbal.btn_start_upgrade");
			((Control)btn_Disconnect).Text = Lang.T("gimbal.btn_disconnect");
			((Control)uploadDragger1).Text = Lang.T("gimbal.drag_firmware_hint");
			uploadDragger1.TextDesc = Lang.T("gimbal.select_firmware");
			((Control)lab_upgDesc).Text = Lang.T("gimbal.transferring_firmware");
			((Control)lab_hardwareVer).Text = Lang.T("gimbal.hardware_prefix");
			((Control)lab_firmwareVer).Text = Lang.T("gimbal.firmware_prefix");
			((Control)lab_tempture).Text = Lang.T("gimbal.temperature_prefix");
			((Control)lab_gmMode).Text = Lang.T("gimbal.mode_select");
			((Control)lab_sensNum).Text = Lang.T("gimbal.sensitivity");
			((Control)lab_rollNum).Text = Lang.T("gimbal.roll_angle");
			((Control)lab_pitchNum).Text = Lang.T("gimbal.pitch_angle");
			((Control)lab_yawNum).Text = Lang.T("gimbal.yaw_angle");
			((Control)lab_currGMMode).Text = Lang.T("gimbal.curr_mode");
			((Control)lab_angProtect).Text = Lang.T("gimbal.angle_protect");
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

	private void ReloadSelItems()
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		Select[] array = (Select[])(object)new Select[3] { sel_M0, sel_M1, sel_M2 };
		string[] array2 = new string[5]
		{
			Lang.T("gimbal.mode_fpv"),
			Lang.T("gimbal.mode_pitch_stab"),
			Lang.T("gimbal.mode_horizon"),
			Lang.T("gimbal.mode_lookdown"),
			Lang.T("gimbal.mode_fpv_lookdown")
		};
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Items.Clear();
			for (int j = 0; j < array2.Length; j++)
			{
				array[i].Items.Add((object)array2[j]);
			}
		}
		sel_M0.SelectedIndexChanged += new IntEventHandler(sel_M0_SelectedIndexChanged);
		sel_M1.SelectedIndexChanged += new IntEventHandler(sel_M1_SelectedIndexChanged);
		sel_M2.SelectedIndexChanged += new IntEventHandler(sel_M2_SelectedIndexChanged);
	}

	private void ReloadFont()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Expected O, but got Unknown
		try
		{
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 18f);
			((Control)lab_devName).Font = font;
			Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 10f);
			Label obj = lab_hardwareVer;
			Label obj2 = lab_sn;
			Label obj3 = lab_firmwareVer;
			Label obj4 = lab_tempture;
			Label obj5 = lab_rollGain;
			Label obj6 = lab_pitchGain;
			Label obj7 = lab_yawGain;
			Label obj8 = lab_modeChan;
			Label obj9 = lab_sensChan;
			Label obj10 = lab_rollChan;
			Label obj11 = lab_pitchChan;
			Label obj12 = lab_yawChan;
			Button obj13 = btn_startGM;
			Button obj14 = btn_gryoCalib;
			Button obj15 = btn_WriteParam;
			Button obj16 = btn_openParam;
			Button obj17 = btn_saveParam;
			Label obj18 = lab_upgDesc;
			Button obj19 = btn_startUpg;
			Label obj20 = lab_currGMMode;
			Label obj21 = lab_gmMode;
			Label obj22 = lab_rollNum;
			Label obj23 = lab_pitchNum;
			Label obj24 = lab_yawNum;
			Select obj25 = sel_gmMoed;
			Input obj26 = inp_currMode;
			Input obj27 = inp_sensNum;
			Input obj28 = inp_rollNum;
			Input obj29 = inp_pitchNum;
			Input obj30 = inp_yawNum;
			Select obj31 = sel_portname;
			Button obj32 = btn_Disconnect;
			Button obj33 = btn_CloseGM;
			Label obj34 = lab_angProtect;
			Select obj35 = sel_M0;
			Select obj36 = sel_M1;
			Select obj37 = sel_M2;
			Label obj38 = label2;
			Label obj39 = label3;
			Label obj40 = label4;
			Label obj41 = lab_sensNum;
			Label obj42 = lab_rollNum;
			Label obj43 = lab_pitchNum;
			Label obj44 = lab_yawNum;
			Input obj45 = inp_sensNum;
			Input obj46 = inp_rollNum;
			Input obj47 = inp_pitchNum;
			Input obj48 = inp_yawNum;
			Button obj49 = btn_posCalib;
			Select obj50 = sel_LensType;
			Font val2 = (((Control)lab_LensType).Font = val);
			Font val4 = (((Control)obj50).Font = val2);
			Font val6 = (((Control)obj49).Font = val4);
			Font val8 = (((Control)obj48).Font = val6);
			Font val10 = (((Control)obj47).Font = val8);
			Font val12 = (((Control)obj46).Font = val10);
			Font val14 = (((Control)obj45).Font = val12);
			Font val16 = (((Control)obj44).Font = val14);
			Font val18 = (((Control)obj43).Font = val16);
			Font val20 = (((Control)obj42).Font = val18);
			Font val22 = (((Control)obj41).Font = val20);
			Font val24 = (((Control)obj40).Font = val22);
			Font val26 = (((Control)obj39).Font = val24);
			Font val28 = (((Control)obj38).Font = val26);
			Font val30 = (((Control)obj37).Font = val28);
			Font val32 = (((Control)obj36).Font = val30);
			Font val34 = (((Control)obj35).Font = val32);
			Font val36 = (((Control)obj34).Font = val34);
			Font val38 = (((Control)obj33).Font = val36);
			Font val40 = (((Control)obj32).Font = val38);
			Font val42 = (((Control)obj31).Font = val40);
			Font val44 = (((Control)obj30).Font = val42);
			Font val46 = (((Control)obj29).Font = val44);
			Font val48 = (((Control)obj28).Font = val46);
			Font val50 = (((Control)obj27).Font = val48);
			Font val52 = (((Control)obj26).Font = val50);
			Font val54 = (((Control)obj25).Font = val52);
			Font val56 = (((Control)obj24).Font = val54);
			Font val58 = (((Control)obj23).Font = val56);
			Font val60 = (((Control)obj22).Font = val58);
			Font val62 = (((Control)obj21).Font = val60);
			Font val64 = (((Control)obj20).Font = val62);
			Font val66 = (((Control)obj19).Font = val64);
			Font val68 = (((Control)obj18).Font = val66);
			Font val70 = (((Control)obj17).Font = val68);
			Font val72 = (((Control)obj16).Font = val70);
			Font val74 = (((Control)obj15).Font = val72);
			Font val76 = (((Control)obj14).Font = val74);
			Font val78 = (((Control)obj13).Font = val76);
			Font val80 = (((Control)obj12).Font = val78);
			Font val82 = (((Control)obj11).Font = val80);
			Font val84 = (((Control)obj10).Font = val82);
			Font val86 = (((Control)obj9).Font = val84);
			Font val88 = (((Control)obj8).Font = val86);
			Font val90 = (((Control)obj7).Font = val88);
			Font val92 = (((Control)obj6).Font = val90);
			Font val94 = (((Control)obj5).Font = val92);
			Font val96 = (((Control)obj4).Font = val94);
			Font val98 = (((Control)obj3).Font = val96);
			Font font2 = (((Control)obj2).Font = val98);
			((Control)obj).Font = font2;
			Font val101 = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 13f);
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("FindDeviceFrm.ReloadFont error ,desc=" + ex.Message, Color.Red);
		}
	}

	public void Dispose()
	{
		UnbindEventHandler();
		GD.Inst.SP_Gim?.Dispose();
		GD.Inst.Upg_Gim?.Dispose();
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
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
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
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
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
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Expected O, but got Unknown
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Expected O, but got Unknown
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Expected O, but got Unknown
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Expected O, but got Unknown
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Expected O, but got Unknown
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Expected O, but got Unknown
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Expected O, but got Unknown
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Expected O, but got Unknown
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Expected O, but got Unknown
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Expected O, but got Unknown
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Expected O, but got Unknown
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Expected O, but got Unknown
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Expected O, but got Unknown
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Expected O, but got Unknown
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Expected O, but got Unknown
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_080c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0816: Expected O, but got Unknown
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Expected O, but got Unknown
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Expected O, but got Unknown
		//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cd: Expected O, but got Unknown
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a25: Expected O, but got Unknown
		//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b15: Expected O, but got Unknown
		//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c05: Expected O, but got Unknown
		//IL_0c56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e15: Unknown result type (might be due to invalid IL or missing references)
		//IL_104f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1102: Unknown result type (might be due to invalid IL or missing references)
		//IL_110c: Expected O, but got Unknown
		//IL_11c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11cd: Expected O, but got Unknown
		//IL_12e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_138d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1397: Expected O, but got Unknown
		//IL_1560: Unknown result type (might be due to invalid IL or missing references)
		//IL_1613: Unknown result type (might be due to invalid IL or missing references)
		//IL_161d: Expected O, but got Unknown
		//IL_17e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1899: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a3: Expected O, but got Unknown
		//IL_1a6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b29: Expected O, but got Unknown
		//IL_1b5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b69: Expected O, but got Unknown
		//IL_1bbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c52: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c5c: Expected O, but got Unknown
		//IL_1cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d4f: Expected O, but got Unknown
		//IL_1da3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e42: Expected O, but got Unknown
		//IL_1e96: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f03: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f0d: Expected O, but got Unknown
		//IL_1f43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f4d: Expected O, but got Unknown
		//IL_1f9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_20cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d6: Expected O, but got Unknown
		//IL_2176: Unknown result type (might be due to invalid IL or missing references)
		//IL_2226: Unknown result type (might be due to invalid IL or missing references)
		//IL_2230: Expected O, but got Unknown
		//IL_2266: Unknown result type (might be due to invalid IL or missing references)
		//IL_2270: Expected O, but got Unknown
		//IL_22a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_240a: Unknown result type (might be due to invalid IL or missing references)
		//IL_24d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_24e0: Expected O, but got Unknown
		//IL_2535: Unknown result type (might be due to invalid IL or missing references)
		//IL_2613: Unknown result type (might be due to invalid IL or missing references)
		//IL_261d: Expected O, but got Unknown
		//IL_2741: Unknown result type (might be due to invalid IL or missing references)
		//IL_274b: Expected O, but got Unknown
		//IL_279c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b96: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e12: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f36: Expected O, but got Unknown
		//IL_2fc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_306e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3078: Expected O, but got Unknown
		//IL_30b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_31e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_31eb: Expected O, but got Unknown
		//IL_3277: Unknown result type (might be due to invalid IL or missing references)
		//IL_3323: Unknown result type (might be due to invalid IL or missing references)
		//IL_332d: Expected O, but got Unknown
		//IL_336e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3496: Unknown result type (might be due to invalid IL or missing references)
		//IL_34a0: Expected O, but got Unknown
		//IL_3531: Unknown result type (might be due to invalid IL or missing references)
		//IL_35dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_35e7: Expected O, but got Unknown
		//IL_3625: Unknown result type (might be due to invalid IL or missing references)
		//IL_376f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3811: Unknown result type (might be due to invalid IL or missing references)
		//IL_381b: Expected O, but got Unknown
		//IL_3859: Unknown result type (might be due to invalid IL or missing references)
		//IL_38e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_38ef: Expected O, but got Unknown
		//IL_3930: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a70: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b16: Expected O, but got Unknown
		//IL_3b24: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b2e: Expected O, but got Unknown
		//IL_3c19: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cbf: Expected O, but got Unknown
		//IL_3ccd: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cd7: Expected O, but got Unknown
		//IL_3d18: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d22: Expected O, but got Unknown
		//IL_3d63: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e96: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f32: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f3c: Expected O, but got Unknown
		//IL_3f4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f54: Expected O, but got Unknown
		//IL_403f: Unknown result type (might be due to invalid IL or missing references)
		//IL_40db: Unknown result type (might be due to invalid IL or missing references)
		//IL_40e5: Expected O, but got Unknown
		//IL_40f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_40fd: Expected O, but got Unknown
		//IL_413e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4148: Expected O, but got Unknown
		//IL_4189: Unknown result type (might be due to invalid IL or missing references)
		//IL_4212: Unknown result type (might be due to invalid IL or missing references)
		//IL_421c: Expected O, but got Unknown
		//IL_425d: Unknown result type (might be due to invalid IL or missing references)
		//IL_42e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_42f0: Expected O, but got Unknown
		//IL_432d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4444: Unknown result type (might be due to invalid IL or missing references)
		//IL_444e: Expected O, but got Unknown
		//IL_44d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_4586: Unknown result type (might be due to invalid IL or missing references)
		//IL_4590: Expected O, but got Unknown
		//IL_45e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_45f1: Expected O, but got Unknown
		//IL_462d: Unknown result type (might be due to invalid IL or missing references)
		//IL_47b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_4870: Unknown result type (might be due to invalid IL or missing references)
		//IL_48e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_48f0: Expected O, but got Unknown
		//IL_4933: Unknown result type (might be due to invalid IL or missing references)
		//IL_49b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_49c1: Expected O, but got Unknown
		//IL_4a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a85: Expected O, but got Unknown
		//IL_4b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_4bf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cec: Expected O, but got Unknown
		//IL_4d72: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d96: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e02: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e0c: Expected O, but got Unknown
		//IL_4e1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e24: Expected O, but got Unknown
		//IL_4fdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_508c: Unknown result type (might be due to invalid IL or missing references)
		//IL_5096: Expected O, but got Unknown
		//IL_50d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_52e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_52eb: Expected O, but got Unknown
		//IL_5321: Unknown result type (might be due to invalid IL or missing references)
		//IL_532b: Expected O, but got Unknown
		//IL_5361: Unknown result type (might be due to invalid IL or missing references)
		//IL_5448: Unknown result type (might be due to invalid IL or missing references)
		//IL_5452: Expected O, but got Unknown
		//IL_54ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_5599: Unknown result type (might be due to invalid IL or missing references)
		//IL_562a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5634: Expected O, but got Unknown
		//IL_5679: Unknown result type (might be due to invalid IL or missing references)
		//IL_580a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5814: Expected O, but got Unknown
		//IL_5859: Unknown result type (might be due to invalid IL or missing references)
		//IL_591c: Unknown result type (might be due to invalid IL or missing references)
		//IL_5926: Expected O, but got Unknown
		//IL_596b: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a49: Expected O, but got Unknown
		//IL_5a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_5b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_5b5b: Expected O, but got Unknown
		//IL_5b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_5c7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_5dcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_5e57: Unknown result type (might be due to invalid IL or missing references)
		//IL_5e70: Unknown result type (might be due to invalid IL or missing references)
		grpan_gain = new GridPanel();
		inp_yawGain = new Input();
		inp_pitchGain = new Input();
		inp_rollGain = new Input();
		lab_yawGain = new Label();
		lab_pitchGain = new Label();
		lab_rollGain = new Label();
		grpan_chann = new GridPanel();
		sel_yawChan = new Select();
		sel_modeChan = new Select();
		sel_pitchChan = new Select();
		sel_rollChan = new Select();
		sel_senseChan = new Select();
		lab_yawChan = new Label();
		lab_pitchChan = new Label();
		lab_rollChan = new Label();
		lab_sensChan = new Label();
		lab_modeChan = new Label();
		sel_LensType = new Select();
		lab_LensType = new Label();
		grpan_param = new GridPanel();
		btn_posCalib = new Button();
		btn_gryoCalib = new Button();
		btn_startGM = new Button();
		btn_WriteParam = new Button();
		btn_saveParam = new Button();
		btn_openParam = new Button();
		grpan_upgrade = new GridPanel();
		pageHeader2 = new PageHeader();
		sel_M2 = new Select();
		label4 = new Label();
		sel_M1 = new Select();
		label3 = new Label();
		sel_M0 = new Select();
		label2 = new Label();
		inp_currMode = new Input();
		lab_currGMMode = new Label();
		lab_yawNum = new Label();
		inp_yawNum = new Input();
		inp_pitchNum = new Input();
		lab_pitchNum = new Label();
		inp_rollNum = new Input();
		inp_sensNum = new Input();
		lab_rollNum = new Label();
		lab_sensNum = new Label();
		lab_gmMode = new Label();
		sel_gmMoed = new Select();
		btn_startUpg = new Button();
		grpan_process = new GridPanel();
		grpan_procDesc = new GridPanel();
		stackPanel1 = new StackPanel();
		lab_upgDesc = new Label();
		lab_versChange = new Label();
		progress2 = new Progress();
		pictureBox1 = new PictureBox();
		uploadDragger1 = new UploadDragger();
		pageHeader1 = new PageHeader();
		sel_portname = new Select();
		btn_Disconnect = new Button();
		switch1 = new Switch();
		lab_angProtect = new Label();
		btn_CloseGM = new Button();
		pictureBox2 = new PictureBox();
		lab_firmwareVer = new Label();
		divider1 = new Divider();
		lab_tempture = new Label();
		lab_hardwareVer = new Label();
		lab_sn = new Label();
		lab_devName = new Label();
		btn_refresh = new Button();
		grpan_main = new GridPanel();
		((Control)grpan_gain).SuspendLayout();
		((Control)grpan_chann).SuspendLayout();
		((Control)grpan_param).SuspendLayout();
		((Control)grpan_upgrade).SuspendLayout();
		((Control)pageHeader2).SuspendLayout();
		((Control)grpan_process).SuspendLayout();
		((Control)grpan_procDesc).SuspendLayout();
		((Control)stackPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)pageHeader1).SuspendLayout();
		((ISupportInitialize)pictureBox2).BeginInit();
		((Control)grpan_main).SuspendLayout();
		((Control)this).SuspendLayout();
		((ContainerPanel)grpan_gain).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)grpan_gain).BorderWidth = 2f;
		((Control)grpan_gain).Controls.Add((Control)(object)inp_yawGain);
		((Control)grpan_gain).Controls.Add((Control)(object)inp_pitchGain);
		((Control)grpan_gain).Controls.Add((Control)(object)inp_rollGain);
		((Control)grpan_gain).Controls.Add((Control)(object)lab_yawGain);
		((Control)grpan_gain).Controls.Add((Control)(object)lab_pitchGain);
		((Control)grpan_gain).Controls.Add((Control)(object)lab_rollGain);
		((IControl)grpan_gain).HandCursor = Cursors.Default;
		grpan_main.SetIndex((Control)(object)grpan_gain, 4);
		((Control)grpan_gain).Location = new Point(8, 231);
		((Control)grpan_gain).Margin = new Padding(8);
		((Control)grpan_gain).Name = "grpan_gain";
		((Control)grpan_gain).Size = new Size(407, 80);
		grpan_gain.Span = "33% 33% 34%;\r\n33% 33% 34%;";
		((Control)grpan_gain).TabIndex = 2;
		((Control)grpan_gain).Text = "gridPanel5";
		inp_yawGain.BackColor = Color.FromArgb(33, 36, 39);
		inp_yawGain.BorderActive = Color.FromArgb(255, 233, 0);
		inp_yawGain.BorderColor = Color.FromArgb(66, 69, 71);
		inp_yawGain.BorderHover = Color.FromArgb(255, 233, 0);
		inp_yawGain.BorderWidth = 2f;
		((Control)inp_yawGain).Dock = (DockStyle)5;
		inp_yawGain.ForeColor = Color.FromArgb(255, 255, 255);
		grpan_gain.SetIndex((Control)(object)inp_yawGain, 7);
		((Control)inp_yawGain).Location = new Point(274, 45);
		((Control)inp_yawGain).Margin = new Padding(5);
		((Control)inp_yawGain).Name = "inp_yawGain";
		((Control)inp_yawGain).Size = new Size(128, 30);
		((Control)inp_yawGain).TabIndex = 7;
		((Control)inp_yawGain).TabStop = false;
		((Control)inp_yawGain).Text = "11";
		inp_yawGain.WaveSize = 0;
		inp_yawGain.VerifyChar += new InputVerifyCharEventHandler(inp_rollGain_VerifyChar);
		((Control)inp_yawGain).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_yawGain).Leave += inp_yawGain_Leave;
		inp_pitchGain.BackColor = Color.FromArgb(33, 36, 39);
		inp_pitchGain.BorderActive = Color.FromArgb(255, 233, 0);
		inp_pitchGain.BorderColor = Color.FromArgb(66, 69, 71);
		inp_pitchGain.BorderHover = Color.FromArgb(255, 233, 0);
		inp_pitchGain.BorderWidth = 2f;
		((Control)inp_pitchGain).Dock = (DockStyle)5;
		inp_pitchGain.ForeColor = Color.FromArgb(255, 255, 255);
		grpan_gain.SetIndex((Control)(object)inp_pitchGain, 6);
		((Control)inp_pitchGain).Location = new Point(139, 47);
		((Control)inp_pitchGain).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_pitchGain).Name = "inp_pitchGain";
		((Control)inp_pitchGain).Size = new Size(124, 26);
		((Control)inp_pitchGain).TabIndex = 6;
		((Control)inp_pitchGain).TabStop = false;
		((Control)inp_pitchGain).Text = "11";
		inp_pitchGain.WaveSize = 0;
		inp_pitchGain.VerifyChar += new InputVerifyCharEventHandler(inp_rollGain_VerifyChar);
		((Control)inp_pitchGain).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_pitchGain).Leave += inp_pitchGain_Leave;
		inp_rollGain.BackColor = Color.FromArgb(33, 36, 39);
		inp_rollGain.BorderActive = Color.FromArgb(255, 233, 0);
		inp_rollGain.BorderColor = Color.FromArgb(66, 69, 71);
		inp_rollGain.BorderHover = Color.FromArgb(255, 233, 0);
		inp_rollGain.BorderWidth = 2f;
		((Control)inp_rollGain).Dock = (DockStyle)5;
		inp_rollGain.ForeColor = Color.FromArgb(255, 255, 255);
		grpan_gain.SetIndex((Control)(object)inp_rollGain, 5);
		((Control)inp_rollGain).Location = new Point(5, 47);
		((Control)inp_rollGain).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_rollGain).Name = "inp_rollGain";
		((Control)inp_rollGain).Size = new Size(124, 26);
		((Control)inp_rollGain).TabIndex = 5;
		((Control)inp_rollGain).TabStop = false;
		((Control)inp_rollGain).Text = "11";
		inp_rollGain.WaveSize = 0;
		inp_rollGain.VerifyChar += new InputVerifyCharEventHandler(inp_rollGain_VerifyChar);
		((Control)inp_rollGain).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_rollGain).Leave += inp_rollGain_Leave;
		((Control)lab_yawGain).BackColor = Color.Transparent;
		((Control)lab_yawGain).Dock = (DockStyle)5;
		((Control)lab_yawGain).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_yawGain).ForeColor = Color.FromArgb(164, 164, 165);
		grpan_gain.SetIndex((Control)(object)lab_yawGain, 4);
		((Control)lab_yawGain).Location = new Point(279, 10);
		((Control)lab_yawGain).Margin = new Padding(10);
		((Control)lab_yawGain).Name = "lab_yawGain";
		((Control)lab_yawGain).Size = new Size(118, 20);
		((Control)lab_yawGain).TabIndex = 3;
		((Control)lab_yawGain).Text = "指向增益";
		lab_yawGain.TextAlign = (ContentAlignment)16;
		((Control)lab_pitchGain).BackColor = Color.Transparent;
		((Control)lab_pitchGain).Dock = (DockStyle)5;
		((Control)lab_pitchGain).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_pitchGain).ForeColor = Color.FromArgb(164, 164, 165);
		grpan_gain.SetIndex((Control)(object)lab_pitchGain, 3);
		((Control)lab_pitchGain).Location = new Point(144, 10);
		((Control)lab_pitchGain).Margin = new Padding(10);
		((Control)lab_pitchGain).Name = "lab_pitchGain";
		((Control)lab_pitchGain).Size = new Size(114, 20);
		((Control)lab_pitchGain).TabIndex = 2;
		((Control)lab_pitchGain).Text = "俯仰增益";
		lab_pitchGain.TextAlign = (ContentAlignment)16;
		((Control)lab_rollGain).BackColor = Color.Transparent;
		((Control)lab_rollGain).Dock = (DockStyle)5;
		((Control)lab_rollGain).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_rollGain).ForeColor = Color.FromArgb(164, 164, 165);
		grpan_gain.SetIndex((Control)(object)lab_rollGain, 2);
		((Control)lab_rollGain).Location = new Point(10, 10);
		((Control)lab_rollGain).Margin = new Padding(10);
		((Control)lab_rollGain).Name = "lab_rollGain";
		((Control)lab_rollGain).Size = new Size(114, 20);
		((Control)lab_rollGain).TabIndex = 1;
		((Control)lab_rollGain).Text = "滚转增益";
		lab_rollGain.TextAlign = (ContentAlignment)16;
		((ContainerPanel)grpan_chann).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)grpan_chann).BorderWidth = 2f;
		((Control)grpan_chann).Controls.Add((Control)(object)sel_yawChan);
		((Control)grpan_chann).Controls.Add((Control)(object)sel_modeChan);
		((Control)grpan_chann).Controls.Add((Control)(object)sel_pitchChan);
		((Control)grpan_chann).Controls.Add((Control)(object)sel_rollChan);
		((Control)grpan_chann).Controls.Add((Control)(object)sel_senseChan);
		((Control)grpan_chann).Controls.Add((Control)(object)lab_yawChan);
		((Control)grpan_chann).Controls.Add((Control)(object)lab_pitchChan);
		((Control)grpan_chann).Controls.Add((Control)(object)lab_rollChan);
		((Control)grpan_chann).Controls.Add((Control)(object)lab_sensChan);
		((Control)grpan_chann).Controls.Add((Control)(object)lab_modeChan);
		((Control)grpan_chann).Dock = (DockStyle)5;
		((IControl)grpan_chann).HandCursor = Cursors.Default;
		grpan_main.SetIndex((Control)(object)grpan_chann, 3);
		((Control)grpan_chann).Location = new Point(8, 136);
		((Control)grpan_chann).Margin = new Padding(8);
		((Control)grpan_chann).Name = "grpan_chann";
		((Control)grpan_chann).Size = new Size(830, 80);
		grpan_chann.Span = "20% 20% 20% 20% 20%;\r\n20% 20% 20% 20% 20%;-50% 50%";
		((Control)grpan_chann).TabIndex = 3;
		((Control)grpan_chann).Text = "gridPanel5";
		((Input)sel_yawChan).BackColor = Color.Transparent;
		((Input)sel_yawChan).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_yawChan).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_yawChan).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_yawChan).BorderWidth = 2f;
		((IControl)sel_yawChan).ColorScheme = (TAMode)2;
		((Control)sel_yawChan).Dock = (DockStyle)5;
		sel_yawChan.EnterDropDown = false;
		((Input)sel_yawChan).ForeColor = Color.FromArgb(255, 255, 255);
		((IControl)sel_yawChan).HandDragFolder = false;
		grpan_chann.SetIndex((Control)(object)sel_yawChan, 7);
		sel_yawChan.Items.AddRange(new object[17]
		{
			"NULL", "CH1", "CH2", "CH3", "CH4", "CH5", "CH6", "CH7", "CH8", "CH9",
			"CH10", "CH11", "CH12", "CH13", "CH14", "CH15", "CH16"
		});
		sel_yawChan.List = true;
		sel_yawChan.ListAutoWidth = true;
		((Control)sel_yawChan).Location = new Point(674, 45);
		((Control)sel_yawChan).Margin = new Padding(10, 5, 10, 5);
		sel_yawChan.MaxCount = 5;
		((Control)sel_yawChan).Name = "sel_yawChan";
		sel_yawChan.Placement = (TAlignFrom)31;
		sel_yawChan.SelectedIndex = 0;
		sel_yawChan.SelectedValue = "NULL";
		((Control)sel_yawChan).Size = new Size(146, 30);
		((Control)sel_yawChan).TabIndex = 15;
		((Control)sel_yawChan).TabStop = false;
		((Control)sel_yawChan).Text = "NULL";
		((Input)sel_yawChan).WaveSize = 0;
		((Control)sel_yawChan).MouseClick += new MouseEventHandler(sel_yawChan_MouseClick);
		((Input)sel_modeChan).BackColor = Color.Transparent;
		((Input)sel_modeChan).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_modeChan).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_modeChan).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_modeChan).BorderWidth = 2f;
		((IControl)sel_modeChan).ColorScheme = (TAMode)2;
		sel_modeChan.EnterDropDown = false;
		((Control)sel_modeChan).Font = new Font("思源黑体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_modeChan).ForeColor = Color.White;
		((IControl)sel_modeChan).HandDragFolder = false;
		grpan_chann.SetIndex((Control)(object)sel_modeChan, 6);
		sel_modeChan.Items.AddRange(new object[17]
		{
			"NULL", "CH1", "CH2", "CH3", "CH4", "CH5", "CH6", "CH7", "CH8", "CH9",
			"CH10", "CH11", "CH12", "CH13", "CH14", "CH15", "CH16"
		});
		sel_modeChan.List = true;
		sel_modeChan.ListAutoWidth = true;
		((Control)sel_modeChan).Location = new Point(10, 45);
		((Control)sel_modeChan).Margin = new Padding(10, 5, 10, 5);
		sel_modeChan.MaxCount = 5;
		((Control)sel_modeChan).Name = "sel_modeChan";
		sel_modeChan.Placement = (TAlignFrom)31;
		sel_modeChan.SelectedValue = "Default";
		((Control)sel_modeChan).Size = new Size(146, 30);
		((Control)sel_modeChan).TabIndex = 11;
		((Control)sel_modeChan).TabStop = false;
		((Control)sel_modeChan).Text = "Default";
		((Input)sel_modeChan).WaveSize = 0;
		((Control)sel_modeChan).MouseClick += new MouseEventHandler(sel_modeChan_MouseClick);
		((Input)sel_pitchChan).BackColor = Color.Transparent;
		((Input)sel_pitchChan).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_pitchChan).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_pitchChan).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_pitchChan).BorderWidth = 2f;
		((IControl)sel_pitchChan).ColorScheme = (TAMode)2;
		((Control)sel_pitchChan).Dock = (DockStyle)5;
		sel_pitchChan.EnterDropDown = false;
		((Input)sel_pitchChan).ForeColor = Color.White;
		((IControl)sel_pitchChan).HandDragFolder = false;
		grpan_chann.SetIndex((Control)(object)sel_pitchChan, 7);
		sel_pitchChan.Items.AddRange(new object[17]
		{
			"NULL", "CH1", "CH2", "CH3", "CH4", "CH5", "CH6", "CH7", "CH8", "CH9",
			"CH10", "CH11", "CH12", "CH13", "CH14", "CH15", "CH16"
		});
		sel_pitchChan.List = true;
		sel_pitchChan.ListAutoWidth = true;
		((Control)sel_pitchChan).Location = new Point(508, 45);
		((Control)sel_pitchChan).Margin = new Padding(10, 5, 10, 5);
		sel_pitchChan.MaxCount = 5;
		((Control)sel_pitchChan).Name = "sel_pitchChan";
		sel_pitchChan.Placement = (TAlignFrom)31;
		sel_pitchChan.SelectedIndex = 0;
		sel_pitchChan.SelectedValue = "NULL";
		((Control)sel_pitchChan).Size = new Size(146, 30);
		((Control)sel_pitchChan).TabIndex = 14;
		((Control)sel_pitchChan).TabStop = false;
		((Control)sel_pitchChan).Text = "NULL";
		((Input)sel_pitchChan).WaveSize = 0;
		((Control)sel_pitchChan).MouseClick += new MouseEventHandler(sel_pitchChan_MouseClick);
		((Input)sel_rollChan).BackColor = Color.Transparent;
		((Input)sel_rollChan).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_rollChan).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_rollChan).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_rollChan).BorderWidth = 2f;
		((IControl)sel_rollChan).ColorScheme = (TAMode)2;
		((Control)sel_rollChan).Dock = (DockStyle)5;
		sel_rollChan.EnterDropDown = false;
		((Input)sel_rollChan).ForeColor = Color.White;
		((IControl)sel_rollChan).HandDragFolder = false;
		grpan_chann.SetIndex((Control)(object)sel_rollChan, 7);
		sel_rollChan.Items.AddRange(new object[17]
		{
			"NULL", "CH1", "CH2", "CH3", "CH4", "CH5", "CH6", "CH7", "CH8", "CH9",
			"CH10", "CH11", "CH12", "CH13", "CH14", "CH15", "CH16"
		});
		sel_rollChan.List = true;
		sel_rollChan.ListAutoWidth = true;
		((Control)sel_rollChan).Location = new Point(342, 45);
		((Control)sel_rollChan).Margin = new Padding(10, 5, 10, 5);
		sel_rollChan.MaxCount = 5;
		((Control)sel_rollChan).Name = "sel_rollChan";
		sel_rollChan.Placement = (TAlignFrom)31;
		sel_rollChan.SelectedIndex = 0;
		sel_rollChan.SelectedValue = "NULL";
		((Control)sel_rollChan).Size = new Size(146, 30);
		((Control)sel_rollChan).TabIndex = 13;
		((Control)sel_rollChan).TabStop = false;
		((Control)sel_rollChan).Text = "NULL";
		((Input)sel_rollChan).WaveSize = 0;
		((Control)sel_rollChan).MouseClick += new MouseEventHandler(sel_rollChan_MouseClick);
		((Input)sel_senseChan).BackColor = Color.Transparent;
		((Input)sel_senseChan).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_senseChan).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_senseChan).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_senseChan).BorderWidth = 2f;
		((IControl)sel_senseChan).ColorScheme = (TAMode)2;
		((Control)sel_senseChan).Dock = (DockStyle)5;
		sel_senseChan.EnterDropDown = false;
		((Input)sel_senseChan).ForeColor = Color.White;
		((IControl)sel_senseChan).HandDragFolder = false;
		grpan_chann.SetIndex((Control)(object)sel_senseChan, 7);
		sel_senseChan.Items.AddRange(new object[17]
		{
			"NULL", "CH1", "CH2", "CH3", "CH4", "CH5", "CH6", "CH7", "CH8", "CH9",
			"CH10", "CH11", "CH12", "CH13", "CH14", "CH15", "CH16"
		});
		sel_senseChan.List = true;
		sel_senseChan.ListAutoWidth = true;
		((Control)sel_senseChan).Location = new Point(176, 45);
		((Control)sel_senseChan).Margin = new Padding(10, 5, 10, 5);
		sel_senseChan.MaxCount = 5;
		((Control)sel_senseChan).Name = "sel_senseChan";
		sel_senseChan.Placement = (TAlignFrom)31;
		sel_senseChan.SelectedIndex = 0;
		sel_senseChan.SelectedValue = "NULL";
		((Control)sel_senseChan).Size = new Size(146, 30);
		((Control)sel_senseChan).TabIndex = 12;
		((Control)sel_senseChan).TabStop = false;
		((Control)sel_senseChan).Text = "NULL";
		((Input)sel_senseChan).WaveSize = 0;
		((Control)sel_senseChan).MouseClick += new MouseEventHandler(sel_senseChan_MouseClick);
		((Control)lab_yawChan).BackColor = Color.Transparent;
		((Control)lab_yawChan).Dock = (DockStyle)5;
		((Control)lab_yawChan).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_yawChan).ForeColor = Color.FromArgb(164, 164, 165);
		grpan_chann.SetIndex((Control)(object)lab_yawChan, 4);
		((Control)lab_yawChan).Location = new Point(674, 10);
		((Control)lab_yawChan).Margin = new Padding(10);
		((Control)lab_yawChan).Name = "lab_yawChan";
		((Control)lab_yawChan).Size = new Size(146, 20);
		((Control)lab_yawChan).TabIndex = 5;
		((Control)lab_yawChan).Text = "指向";
		lab_yawChan.TextAlign = (ContentAlignment)16;
		((Control)lab_pitchChan).BackColor = Color.Transparent;
		((Control)lab_pitchChan).Dock = (DockStyle)5;
		((Control)lab_pitchChan).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_pitchChan).ForeColor = Color.FromArgb(164, 164, 165);
		grpan_chann.SetIndex((Control)(object)lab_pitchChan, 4);
		((Control)lab_pitchChan).Location = new Point(508, 10);
		((Control)lab_pitchChan).Margin = new Padding(10);
		((Control)lab_pitchChan).Name = "lab_pitchChan";
		((Control)lab_pitchChan).Size = new Size(146, 20);
		((Control)lab_pitchChan).TabIndex = 4;
		((Control)lab_pitchChan).Text = "俯仰";
		lab_pitchChan.TextAlign = (ContentAlignment)16;
		((Control)lab_rollChan).BackColor = Color.Transparent;
		((Control)lab_rollChan).Dock = (DockStyle)5;
		((Control)lab_rollChan).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_rollChan).ForeColor = Color.FromArgb(164, 164, 165);
		grpan_chann.SetIndex((Control)(object)lab_rollChan, 4);
		((Control)lab_rollChan).Location = new Point(342, 10);
		((Control)lab_rollChan).Margin = new Padding(10);
		((Control)lab_rollChan).Name = "lab_rollChan";
		((Control)lab_rollChan).Size = new Size(146, 20);
		((Control)lab_rollChan).TabIndex = 3;
		((Control)lab_rollChan).Text = "滚转";
		lab_rollChan.TextAlign = (ContentAlignment)16;
		((Control)lab_sensChan).BackColor = Color.Transparent;
		((Control)lab_sensChan).Dock = (DockStyle)5;
		((Control)lab_sensChan).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_sensChan).ForeColor = Color.FromArgb(164, 164, 165);
		grpan_chann.SetIndex((Control)(object)lab_sensChan, 3);
		((Control)lab_sensChan).Location = new Point(176, 10);
		((Control)lab_sensChan).Margin = new Padding(10);
		((Control)lab_sensChan).Name = "lab_sensChan";
		((Control)lab_sensChan).Size = new Size(146, 20);
		((Control)lab_sensChan).TabIndex = 2;
		((Control)lab_sensChan).Text = "灵敏度";
		lab_sensChan.TextAlign = (ContentAlignment)16;
		((Control)lab_sensChan).MouseClick += new MouseEventHandler(lab_sensChan_MouseClick);
		((Control)lab_modeChan).BackColor = Color.Transparent;
		((Control)lab_modeChan).Dock = (DockStyle)5;
		((Control)lab_modeChan).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_modeChan).ForeColor = Color.FromArgb(164, 164, 165);
		grpan_chann.SetIndex((Control)(object)lab_modeChan, 2);
		((Control)lab_modeChan).Location = new Point(10, 10);
		((Control)lab_modeChan).Margin = new Padding(10);
		((Control)lab_modeChan).Name = "lab_modeChan";
		((Control)lab_modeChan).Size = new Size(146, 20);
		((Control)lab_modeChan).TabIndex = 1;
		((Control)lab_modeChan).Text = "模式";
		lab_modeChan.TextAlign = (ContentAlignment)16;
		((Control)lab_modeChan).Click += lab_modeChan_Click;
		((Input)sel_LensType).BackColor = Color.Transparent;
		((Input)sel_LensType).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_LensType).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_LensType).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_LensType).BorderWidth = 2f;
		((IControl)sel_LensType).ColorScheme = (TAMode)2;
		sel_LensType.EnterDropDown = false;
		((Control)sel_LensType).Font = new Font("思源黑体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_LensType).ForeColor = Color.FromArgb(255, 255, 255);
		((IControl)sel_LensType).HandDragFolder = false;
		sel_LensType.Items.AddRange(new object[3] { "Default", "Z40", "Lite+" });
		sel_LensType.List = true;
		sel_LensType.ListAutoWidth = true;
		((Control)sel_LensType).Location = new Point(583, 82);
		((Control)sel_LensType).Margin = new Padding(10, 5, 10, 5);
		sel_LensType.MaxCount = 5;
		((Control)sel_LensType).Name = "sel_LensType";
		sel_LensType.Placement = (TAlignFrom)31;
		sel_LensType.SelectedIndex = 0;
		sel_LensType.SelectedValue = "Default";
		((Control)sel_LensType).Size = new Size(107, 23);
		((Control)sel_LensType).TabIndex = 17;
		((Control)sel_LensType).TabStop = false;
		((Control)sel_LensType).Text = "Default";
		((Input)sel_LensType).WaveSize = 0;
		((Control)sel_LensType).MouseClick += new MouseEventHandler(sel_LensType_MouseClick);
		((Control)lab_LensType).AutoSize = true;
		((Control)lab_LensType).BackColor = Color.Transparent;
		((Control)lab_LensType).Font = new Font("思源黑体", 8.249999f, (FontStyle)0, (GraphicsUnit)3, (byte)128);
		((Control)lab_LensType).ForeColor = Color.White;
		((Control)lab_LensType).Location = new Point(589, 63);
		((Control)lab_LensType).Margin = new Padding(10);
		((Control)lab_LensType).Name = "lab_LensType";
		((Control)lab_LensType).Size = new Size(57, 16);
		((Control)lab_LensType).TabIndex = 16;
		((Control)lab_LensType).Text = "LensType";
		lab_LensType.TextAlign = (ContentAlignment)16;
		((ContainerPanel)grpan_param).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)grpan_param).BorderWidth = 2f;
		((Control)grpan_param).Controls.Add((Control)(object)btn_posCalib);
		((Control)grpan_param).Controls.Add((Control)(object)btn_gryoCalib);
		((Control)grpan_param).Controls.Add((Control)(object)btn_startGM);
		((Control)grpan_param).Controls.Add((Control)(object)btn_WriteParam);
		((Control)grpan_param).Controls.Add((Control)(object)btn_saveParam);
		((Control)grpan_param).Controls.Add((Control)(object)btn_openParam);
		((Control)grpan_param).Dock = (DockStyle)5;
		((IControl)grpan_param).HandCursor = Cursors.Default;
		grpan_main.SetIndex((Control)(object)grpan_param, 4);
		((Control)grpan_param).Location = new Point(431, 231);
		((Control)grpan_param).Margin = new Padding(8);
		((Control)grpan_param).Name = "grpan_param";
		((Control)grpan_param).Size = new Size(407, 80);
		grpan_param.Span = "33% 33% 34%;33% 33% 34%;";
		((Control)grpan_param).TabIndex = 4;
		((Control)grpan_param).TabStop = false;
		((Control)grpan_param).Text = "gridPanel5";
		btn_posCalib.BackHover = Color.FromArgb(27, 28, 30);
		btn_posCalib.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_posCalib).Dock = (DockStyle)5;
		((Control)btn_posCalib).Font = new Font("Microsoft Sans Serif", 10.5f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_posCalib.ForeColor = Color.White;
		((IControl)btn_posCalib).HandDragFolder = false;
		grpan_param.SetIndex((Control)(object)btn_posCalib, 1);
		((Control)btn_posCalib).Location = new Point(139, 5);
		((Control)btn_posCalib).Margin = new Padding(5);
		((Control)btn_posCalib).Name = "btn_posCalib";
		((Control)btn_posCalib).Size = new Size(124, 30);
		((Control)btn_posCalib).TabIndex = 10;
		((Control)btn_posCalib).TabStop = false;
		((Control)btn_posCalib).Text = "位置校准";
		btn_posCalib.WaveSize = 0;
		((Control)btn_posCalib).Click += btn_posCalib_Click;
		btn_gryoCalib.BackHover = Color.FromArgb(27, 28, 30);
		btn_gryoCalib.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_gryoCalib).Dock = (DockStyle)5;
		((Control)btn_gryoCalib).Font = new Font("Microsoft Sans Serif", 10.5f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_gryoCalib.ForeColor = Color.White;
		((IControl)btn_gryoCalib).HandDragFolder = false;
		grpan_param.SetIndex((Control)(object)btn_gryoCalib, 2);
		((Control)btn_gryoCalib).Location = new Point(272, 3);
		((Control)btn_gryoCalib).Name = "btn_gryoCalib";
		((Control)btn_gryoCalib).Size = new Size(132, 34);
		((Control)btn_gryoCalib).TabIndex = 9;
		((Control)btn_gryoCalib).TabStop = false;
		((Control)btn_gryoCalib).Text = "陀螺仪校准";
		btn_gryoCalib.WaveSize = 0;
		((Control)btn_gryoCalib).Click += btn_calib_Click;
		btn_startGM.BackHover = Color.FromArgb(27, 28, 30);
		btn_startGM.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_startGM).Dock = (DockStyle)5;
		((Control)btn_startGM).Font = new Font("Microsoft Sans Serif", 10.5f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_startGM.ForeColor = Color.White;
		((IControl)btn_startGM).HandDragFolder = false;
		grpan_param.SetIndex((Control)(object)btn_startGM, 1);
		((Control)btn_startGM).Location = new Point(5, 5);
		((Control)btn_startGM).Margin = new Padding(5);
		((Control)btn_startGM).Name = "btn_startGM";
		((Control)btn_startGM).Size = new Size(124, 30);
		((Control)btn_startGM).TabIndex = 8;
		((Control)btn_startGM).TabStop = false;
		((Control)btn_startGM).Text = "启动云台";
		btn_startGM.WaveSize = 0;
		((Control)btn_startGM).Click += btn_startGM_Click;
		btn_WriteParam.BackHover = Color.FromArgb(27, 28, 30);
		btn_WriteParam.DefaultBack = Color.FromArgb(95, 95, 96);
		btn_WriteParam.ForeColor = Color.White;
		grpan_param.SetIndex((Control)(object)btn_WriteParam, 3);
		((Control)btn_WriteParam).Location = new Point(3, 43);
		((Control)btn_WriteParam).Name = "btn_WriteParam";
		((Control)btn_WriteParam).Size = new Size(128, 34);
		((Control)btn_WriteParam).TabIndex = 5;
		((Control)btn_WriteParam).Text = "烧写参数";
		btn_WriteParam.WaveSize = 0;
		((Control)btn_WriteParam).Click += btn_WriteParam_Click;
		btn_saveParam.BackHover = Color.FromArgb(27, 28, 30);
		btn_saveParam.DefaultBack = Color.FromArgb(95, 95, 96);
		btn_saveParam.ForeColor = Color.White;
		grpan_param.SetIndex((Control)(object)btn_saveParam, 5);
		((Control)btn_saveParam).Location = new Point(272, 43);
		((Control)btn_saveParam).Name = "btn_saveParam";
		((Control)btn_saveParam).Size = new Size(132, 34);
		((Control)btn_saveParam).TabIndex = 4;
		((Control)btn_saveParam).Text = "保存参数";
		btn_saveParam.WaveSize = 0;
		((Control)btn_saveParam).Click += btn_saveParam_Click;
		btn_openParam.BackHover = Color.FromArgb(27, 28, 30);
		btn_openParam.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_openParam).Dock = (DockStyle)5;
		btn_openParam.ForeColor = Color.White;
		grpan_param.SetIndex((Control)(object)btn_openParam, 4);
		((Control)btn_openParam).Location = new Point(137, 43);
		((Control)btn_openParam).Name = "btn_openParam";
		((Control)btn_openParam).Size = new Size(128, 34);
		((Control)btn_openParam).TabIndex = 3;
		((Control)btn_openParam).Text = "打开参数";
		btn_openParam.WaveSize = 0;
		((Control)btn_openParam).Click += btn_openParam_Click;
		((ContainerPanel)grpan_upgrade).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)grpan_upgrade).BorderWidth = 2f;
		((Control)grpan_upgrade).Controls.Add((Control)(object)pageHeader2);
		((Control)grpan_upgrade).Controls.Add((Control)(object)grpan_process);
		((Control)grpan_upgrade).Controls.Add((Control)(object)uploadDragger1);
		((Control)grpan_upgrade).Dock = (DockStyle)5;
		((IControl)grpan_upgrade).HandCursor = Cursors.Default;
		grpan_main.SetIndex((Control)(object)grpan_upgrade, 4);
		((Control)grpan_upgrade).Location = new Point(8, 327);
		((Control)grpan_upgrade).Margin = new Padding(8);
		((Control)grpan_upgrade).Name = "grpan_upgrade";
		((Control)grpan_upgrade).Size = new Size(830, 303);
		grpan_upgrade.Span = "100%;100%;100%;-42% 30% 28%";
		((Control)grpan_upgrade).TabIndex = 6;
		((Control)grpan_upgrade).TabStop = false;
		((Control)grpan_upgrade).Text = "gridPanel5";
		((IControl)grpan_upgrade).Visible = false;
		((IControl)pageHeader2).ColorScheme = (TAMode)2;
		((Control)pageHeader2).Controls.Add((Control)(object)sel_M2);
		((Control)pageHeader2).Controls.Add((Control)(object)label4);
		((Control)pageHeader2).Controls.Add((Control)(object)sel_M1);
		((Control)pageHeader2).Controls.Add((Control)(object)label3);
		((Control)pageHeader2).Controls.Add((Control)(object)sel_M0);
		((Control)pageHeader2).Controls.Add((Control)(object)label2);
		((Control)pageHeader2).Controls.Add((Control)(object)inp_currMode);
		((Control)pageHeader2).Controls.Add((Control)(object)lab_currGMMode);
		((Control)pageHeader2).Controls.Add((Control)(object)lab_yawNum);
		((Control)pageHeader2).Controls.Add((Control)(object)inp_yawNum);
		((Control)pageHeader2).Controls.Add((Control)(object)inp_pitchNum);
		((Control)pageHeader2).Controls.Add((Control)(object)lab_pitchNum);
		((Control)pageHeader2).Controls.Add((Control)(object)inp_rollNum);
		((Control)pageHeader2).Controls.Add((Control)(object)inp_sensNum);
		((Control)pageHeader2).Controls.Add((Control)(object)lab_rollNum);
		((Control)pageHeader2).Controls.Add((Control)(object)lab_sensNum);
		((Control)pageHeader2).Controls.Add((Control)(object)lab_gmMode);
		((Control)pageHeader2).Controls.Add((Control)(object)sel_gmMoed);
		((Control)pageHeader2).Controls.Add((Control)(object)btn_startUpg);
		((Control)pageHeader2).Dock = (DockStyle)5;
		grpan_upgrade.SetIndex((Control)(object)pageHeader2, 1);
		((Control)pageHeader2).Location = new Point(0, 0);
		((Control)pageHeader2).Margin = new Padding(0);
		((Control)pageHeader2).Name = "pageHeader2";
		((Control)pageHeader2).Size = new Size(830, 127);
		((Control)pageHeader2).TabIndex = 10;
		((Control)pageHeader2).Text = "";
		((Input)sel_M2).BackColor = Color.Transparent;
		((Input)sel_M2).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_M2).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_M2).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_M2).BorderWidth = 2f;
		((IControl)sel_M2).ColorScheme = (TAMode)2;
		((Control)sel_M2).Cursor = Cursors.Hand;
		sel_M2.EnterDropDown = false;
		((Control)sel_M2).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_M2).ForeColor = Color.White;
		((IControl)sel_M2).HandDragFolder = false;
		sel_M2.Items.AddRange(new object[4] { "FPV-Mode", "Pitch-Stabilized-Mode", "Horizon-Mode", "LookDown-Mode" });
		sel_M2.List = true;
		((Control)sel_M2).Location = new Point(580, 79);
		((Control)sel_M2).Margin = new Padding(10, 5, 10, 5);
		sel_M2.MaxCount = 5;
		((Control)sel_M2).Name = "sel_M2";
		sel_M2.Placement = (TAlignFrom)31;
		((Control)sel_M2).Size = new Size(210, 30);
		((Control)sel_M2).TabIndex = 30;
		((Control)sel_M2).TabStop = false;
		((Input)sel_M2).WaveSize = 0;
		((Control)label4).AutoSize = true;
		((Control)label4).BackColor = Color.Transparent;
		((Control)label4).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label4).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)label4).Location = new Point(557, 86);
		((Control)label4).Margin = new Padding(10);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(25, 16);
		((Control)label4).TabIndex = 29;
		((Control)label4).Text = "M2";
		label4.TextAlign = (ContentAlignment)256;
		((Input)sel_M1).BackColor = Color.Transparent;
		((Input)sel_M1).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_M1).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_M1).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_M1).BorderWidth = 2f;
		((IControl)sel_M1).ColorScheme = (TAMode)2;
		((Control)sel_M1).Cursor = Cursors.Hand;
		sel_M1.EnterDropDown = false;
		((Control)sel_M1).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_M1).ForeColor = Color.White;
		((IControl)sel_M1).HandDragFolder = false;
		sel_M1.Items.AddRange(new object[4] { "FPV-Mode", "Pitch-Stabilized-Mode", "Horizon-Mode", "LookDown-Mode" });
		sel_M1.List = true;
		((Control)sel_M1).Location = new Point(311, 79);
		((Control)sel_M1).Margin = new Padding(10, 5, 10, 5);
		sel_M1.MaxCount = 5;
		((Control)sel_M1).Name = "sel_M1";
		sel_M1.Placement = (TAlignFrom)31;
		((Control)sel_M1).Size = new Size(210, 30);
		((Control)sel_M1).TabIndex = 28;
		((Control)sel_M1).TabStop = false;
		((Input)sel_M1).WaveSize = 0;
		((Control)label3).AutoSize = true;
		((Control)label3).BackColor = Color.Transparent;
		((Control)label3).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label3).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)label3).Location = new Point(287, 86);
		((Control)label3).Margin = new Padding(10);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(25, 16);
		((Control)label3).TabIndex = 27;
		((Control)label3).Text = "M1";
		label3.TextAlign = (ContentAlignment)256;
		((Input)sel_M0).BackColor = Color.Transparent;
		((Input)sel_M0).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_M0).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_M0).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_M0).BorderWidth = 2f;
		((IControl)sel_M0).ColorScheme = (TAMode)2;
		((Control)sel_M0).Cursor = Cursors.Hand;
		sel_M0.EnterDropDown = false;
		((Control)sel_M0).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_M0).ForeColor = Color.White;
		((IControl)sel_M0).HandDragFolder = false;
		sel_M0.Items.AddRange(new object[5] { "FPV-Mode", "Pitch-Stabilized-Mode", "Horizon-Mode", "Horzion-lookdown-Mode", "FPV-lookdown-Mode" });
		sel_M0.List = true;
		((Control)sel_M0).Location = new Point(39, 79);
		((Control)sel_M0).Margin = new Padding(10, 5, 10, 5);
		sel_M0.MaxCount = 5;
		((Control)sel_M0).Name = "sel_M0";
		sel_M0.Placement = (TAlignFrom)31;
		((Control)sel_M0).Size = new Size(210, 30);
		((Control)sel_M0).TabIndex = 26;
		((Control)sel_M0).TabStop = false;
		((Input)sel_M0).WaveSize = 0;
		((Control)label2).AutoSize = true;
		((Control)label2).BackColor = Color.Transparent;
		((Control)label2).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label2).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)label2).Location = new Point(12, 86);
		((Control)label2).Margin = new Padding(10);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(25, 16);
		((Control)label2).TabIndex = 25;
		((Control)label2).Text = "M0";
		label2.TextAlign = (ContentAlignment)256;
		inp_currMode.BackColor = Color.FromArgb(33, 36, 39);
		inp_currMode.BorderActive = Color.FromArgb(255, 233, 0);
		inp_currMode.BorderColor = Color.FromArgb(66, 69, 71);
		inp_currMode.BorderHover = Color.FromArgb(255, 233, 0);
		inp_currMode.BorderWidth = 2f;
		((IControl)inp_currMode).ColorScheme = (TAMode)2;
		((IControl)inp_currMode).Enabled = false;
		inp_currMode.ForeColor = Color.FromArgb(255, 255, 255);
		((Control)inp_currMode).Location = new Point(123, 33);
		((Control)inp_currMode).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_currMode).Name = "inp_currMode";
		inp_currMode.ReadOnly = true;
		((Control)inp_currMode).Size = new Size(223, 30);
		((Control)inp_currMode).TabIndex = 24;
		((Control)inp_currMode).TabStop = false;
		((Control)inp_currMode).Text = "NULL";
		inp_currMode.WaveSize = 0;
		((Control)lab_currGMMode).BackColor = Color.Transparent;
		((Control)lab_currGMMode).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_currGMMode).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_currGMMode).Location = new Point(124, 10);
		((Control)lab_currGMMode).Margin = new Padding(10);
		((Control)lab_currGMMode).Name = "lab_currGMMode";
		((Control)lab_currGMMode).Size = new Size(168, 20);
		((Control)lab_currGMMode).TabIndex = 23;
		((Control)lab_currGMMode).Text = "Current gimbal mode";
		lab_currGMMode.TextAlign = (ContentAlignment)256;
		((Control)lab_yawNum).BackColor = Color.Transparent;
		((Control)lab_yawNum).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_yawNum).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_yawNum).Location = new Point(627, 10);
		((Control)lab_yawNum).Margin = new Padding(10);
		((Control)lab_yawNum).Name = "lab_yawNum";
		((Control)lab_yawNum).Size = new Size(71, 20);
		((Control)lab_yawNum).TabIndex = 22;
		((Control)lab_yawNum).Text = "指向角度";
		lab_yawNum.TextAlign = (ContentAlignment)256;
		inp_yawNum.BackColor = Color.FromArgb(33, 36, 39);
		inp_yawNum.BorderActive = Color.FromArgb(255, 233, 0);
		inp_yawNum.BorderColor = Color.FromArgb(66, 69, 71);
		inp_yawNum.BorderHover = Color.FromArgb(255, 233, 0);
		inp_yawNum.BorderWidth = 2f;
		inp_yawNum.ForeColor = Color.FromArgb(255, 255, 255);
		((IControl)inp_yawNum).HandDragFolder = false;
		((Control)inp_yawNum).Location = new Point(627, 33);
		((Control)inp_yawNum).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_yawNum).Name = "inp_yawNum";
		inp_yawNum.PlaceholderColor = Color.FromArgb(133, 133, 133);
		inp_yawNum.PlaceholderText = "±150";
		((Control)inp_yawNum).Size = new Size(75, 30);
		((Control)inp_yawNum).TabIndex = 21;
		((Control)inp_yawNum).TabStop = false;
		inp_yawNum.WaveSize = 0;
		inp_yawNum.VerifyChar += new InputVerifyCharEventHandler(inp_sensNum_VerifyChar);
		((Control)inp_yawNum).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_yawNum).Leave += inp_yawNum_Leave;
		inp_pitchNum.BackColor = Color.FromArgb(33, 36, 39);
		inp_pitchNum.BorderActive = Color.FromArgb(255, 233, 0);
		inp_pitchNum.BorderColor = Color.FromArgb(66, 69, 71);
		inp_pitchNum.BorderHover = Color.FromArgb(255, 233, 0);
		inp_pitchNum.BorderWidth = 2f;
		inp_pitchNum.ForeColor = Color.FromArgb(255, 255, 255);
		((Control)inp_pitchNum).Location = new Point(538, 33);
		((Control)inp_pitchNum).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_pitchNum).Name = "inp_pitchNum";
		inp_pitchNum.PlaceholderColor = Color.FromArgb(133, 133, 133);
		inp_pitchNum.PlaceholderText = "±90";
		((Control)inp_pitchNum).Size = new Size(75, 30);
		((Control)inp_pitchNum).TabIndex = 20;
		((Control)inp_pitchNum).TabStop = false;
		inp_pitchNum.WaveSize = 0;
		inp_pitchNum.VerifyChar += new InputVerifyCharEventHandler(inp_sensNum_VerifyChar);
		((Control)inp_pitchNum).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_pitchNum).Leave += inp_pitchNum_Leave;
		((Control)lab_pitchNum).BackColor = Color.Transparent;
		((Control)lab_pitchNum).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_pitchNum).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_pitchNum).Location = new Point(538, 10);
		((Control)lab_pitchNum).Margin = new Padding(10);
		((Control)lab_pitchNum).Name = "lab_pitchNum";
		((Control)lab_pitchNum).Size = new Size(72, 20);
		((Control)lab_pitchNum).TabIndex = 19;
		((Control)lab_pitchNum).Text = "俯仰角度";
		lab_pitchNum.TextAlign = (ContentAlignment)256;
		inp_rollNum.BackColor = Color.FromArgb(33, 36, 39);
		inp_rollNum.BorderActive = Color.FromArgb(255, 233, 0);
		inp_rollNum.BorderColor = Color.FromArgb(66, 69, 71);
		inp_rollNum.BorderHover = Color.FromArgb(255, 233, 0);
		inp_rollNum.BorderWidth = 2f;
		inp_rollNum.ForeColor = Color.FromArgb(255, 255, 255);
		((Control)inp_rollNum).Location = new Point(449, 33);
		((Control)inp_rollNum).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_rollNum).Name = "inp_rollNum";
		inp_rollNum.PlaceholderColor = Color.FromArgb(133, 133, 133);
		inp_rollNum.PlaceholderText = "±55";
		((Control)inp_rollNum).Size = new Size(75, 30);
		((Control)inp_rollNum).TabIndex = 18;
		((Control)inp_rollNum).TabStop = false;
		inp_rollNum.WaveSize = 0;
		inp_rollNum.VerifyChar += new InputVerifyCharEventHandler(inp_sensNum_VerifyChar);
		((Control)inp_rollNum).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_rollNum).Leave += inp_rollNum_Leave;
		inp_sensNum.BackColor = Color.FromArgb(33, 36, 39);
		inp_sensNum.BorderActive = Color.FromArgb(255, 233, 0);
		inp_sensNum.BorderColor = Color.FromArgb(66, 69, 71);
		inp_sensNum.BorderHover = Color.FromArgb(255, 233, 0);
		inp_sensNum.BorderWidth = 2f;
		inp_sensNum.ForeColor = Color.FromArgb(255, 255, 255);
		((Control)inp_sensNum).Location = new Point(360, 33);
		((Control)inp_sensNum).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_sensNum).Name = "inp_sensNum";
		inp_sensNum.PlaceholderColor = Color.FromArgb(133, 133, 133);
		inp_sensNum.PlaceholderText = "±0.5";
		((Control)inp_sensNum).Size = new Size(75, 30);
		((Control)inp_sensNum).TabIndex = 17;
		((Control)inp_sensNum).TabStop = false;
		inp_sensNum.WaveSize = 0;
		inp_sensNum.VerifyChar += new InputVerifyCharEventHandler(inp_sensNum_VerifyChar);
		((Control)inp_sensNum).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_sensNum).Leave += inp_sensNum_Leave;
		((Control)lab_rollNum).BackColor = Color.Transparent;
		((Control)lab_rollNum).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_rollNum).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_rollNum).Location = new Point(449, 10);
		((Control)lab_rollNum).Margin = new Padding(10);
		((Control)lab_rollNum).Name = "lab_rollNum";
		((Control)lab_rollNum).Size = new Size(75, 20);
		((Control)lab_rollNum).TabIndex = 16;
		((Control)lab_rollNum).Text = "滚转角度";
		lab_rollNum.TextAlign = (ContentAlignment)256;
		((Control)lab_sensNum).BackColor = Color.Transparent;
		((Control)lab_sensNum).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_sensNum).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_sensNum).Location = new Point(360, 10);
		((Control)lab_sensNum).Margin = new Padding(10);
		((Control)lab_sensNum).Name = "lab_sensNum";
		((Control)lab_sensNum).Size = new Size(67, 20);
		((Control)lab_sensNum).TabIndex = 14;
		((Control)lab_sensNum).Text = "Roll ang";
		lab_sensNum.TextAlign = (ContentAlignment)256;
		((Control)lab_gmMode).BackColor = Color.Transparent;
		((Control)lab_gmMode).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_gmMode).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_gmMode).Location = new Point(7, 10);
		((Control)lab_gmMode).Margin = new Padding(10);
		((Control)lab_gmMode).Name = "lab_gmMode";
		((Control)lab_gmMode).Size = new Size(114, 20);
		((Control)lab_gmMode).TabIndex = 13;
		((Control)lab_gmMode).Text = "云台模式选择";
		lab_gmMode.TextAlign = (ContentAlignment)256;
		((Input)sel_gmMoed).BackColor = Color.Transparent;
		((Input)sel_gmMoed).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_gmMoed).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_gmMoed).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_gmMoed).BorderWidth = 2f;
		((IControl)sel_gmMoed).ColorScheme = (TAMode)2;
		sel_gmMoed.EnterDropDown = false;
		((Control)sel_gmMoed).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_gmMoed).ForeColor = Color.White;
		((IControl)sel_gmMoed).HandDragFolder = false;
		sel_gmMoed.Items.AddRange(new object[4] { "NULL", "M0", "M1", "M2" });
		sel_gmMoed.List = true;
		((Control)sel_gmMoed).Location = new Point(7, 33);
		((Control)sel_gmMoed).Margin = new Padding(10, 5, 10, 5);
		sel_gmMoed.MaxCount = 5;
		((Control)sel_gmMoed).Name = "sel_gmMoed";
		sel_gmMoed.Placement = (TAlignFrom)31;
		sel_gmMoed.SelectedIndex = 0;
		sel_gmMoed.SelectedValue = "NULL";
		((Control)sel_gmMoed).Size = new Size(102, 30);
		((Control)sel_gmMoed).TabIndex = 12;
		((Control)sel_gmMoed).TabStop = false;
		((Control)sel_gmMoed).Text = "NULL";
		((Input)sel_gmMoed).WaveSize = 0;
		sel_gmMoed.SelectedIndexChanged += new IntEventHandler(sel_gmMoed_SelectedIndexChanged);
		((Control)btn_startUpg).Cursor = Cursors.Hand;
		btn_startUpg.DefaultBack = Color.FromArgb(255, 233, 0);
		((IControl)btn_startUpg).Enabled = false;
		((Control)btn_startUpg).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_startUpg.ForeColor = Color.FromArgb(35, 35, 35);
		((Control)btn_startUpg).Location = new Point(714, 15);
		((Control)btn_startUpg).Margin = new Padding(5);
		((Control)btn_startUpg).Name = "btn_startUpg";
		((Control)btn_startUpg).Size = new Size(86, 48);
		((Control)btn_startUpg).TabIndex = 7;
		((Control)btn_startUpg).TabStop = false;
		((Control)btn_startUpg).Text = "开始升级";
		btn_startUpg.WaveSize = 0;
		((Control)btn_startUpg).Click += btn_startUpg_Click;
		((Control)grpan_process).Controls.Add((Control)(object)grpan_procDesc);
		((Control)grpan_process).Controls.Add((Control)(object)pictureBox1);
		((Control)grpan_process).Dock = (DockStyle)5;
		grpan_upgrade.SetIndex((Control)(object)grpan_process, 3);
		((Control)grpan_process).Location = new Point(3, 221);
		((Control)grpan_process).Name = "grpan_process";
		((Control)grpan_process).Size = new Size(824, 79);
		grpan_process.Span = "10% 90%;";
		((Control)grpan_process).TabIndex = 9;
		((Control)grpan_process).Text = "gripan_process";
		((Control)grpan_procDesc).Controls.Add((Control)(object)stackPanel1);
		((Control)grpan_procDesc).Controls.Add((Control)(object)progress2);
		((Control)grpan_procDesc).Location = new Point(82, 0);
		((Control)grpan_procDesc).Margin = new Padding(0, 0, 5, 5);
		((Control)grpan_procDesc).Name = "grpan_procDesc";
		((Control)grpan_procDesc).Size = new Size(737, 74);
		grpan_procDesc.Span = "100%;100%;-45% 55%";
		((Control)grpan_procDesc).TabIndex = 1;
		((Control)grpan_procDesc).Text = "gridPanel6";
		stackPanel1.Controls.Add((Control)(object)lab_upgDesc);
		stackPanel1.Controls.Add((Control)(object)lab_versChange);
		((Control)stackPanel1).Dock = (DockStyle)5;
		((Control)stackPanel1).Location = new Point(0, 0);
		((Control)stackPanel1).Margin = new Padding(0);
		((Control)stackPanel1).Name = "stackPanel1";
		((Control)stackPanel1).Size = new Size(737, 33);
		((Control)stackPanel1).TabIndex = 6;
		((Control)stackPanel1).Text = "stackPanel1";
		((Control)lab_upgDesc).Dock = (DockStyle)3;
		((Control)lab_upgDesc).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_upgDesc).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_upgDesc).Location = new Point(275, 0);
		((Control)lab_upgDesc).Margin = new Padding(10, 0, 0, 0);
		((Control)lab_upgDesc).Name = "lab_upgDesc";
		((Control)lab_upgDesc).Size = new Size(388, 33);
		((Control)lab_upgDesc).TabIndex = 7;
		((Control)lab_upgDesc).Text = "正在升级";
		lab_upgDesc.TextAlign = (ContentAlignment)16;
		((Control)lab_versChange).Dock = (DockStyle)3;
		((Control)lab_versChange).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_versChange).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_versChange).Location = new Point(10, 0);
		((Control)lab_versChange).Margin = new Padding(10, 0, 0, 0);
		((Control)lab_versChange).Name = "lab_versChange";
		((Control)lab_versChange).Size = new Size(255, 33);
		((Control)lab_versChange).TabIndex = 0;
		((Control)lab_versChange).Text = "V16.1.1 >> V16.1.2";
		lab_versChange.TextAlign = (ContentAlignment)16;
		((Control)lab_versChange).Visible = false;
		((Control)lab_versChange).MouseClick += new MouseEventHandler(lab_versChange_MouseClick);
		progress2.Back = Color.FromArgb(99, 101, 103);
		((Control)progress2).BackColor = Color.FromArgb(33, 36, 39);
		((IControl)progress2).ColorScheme = (TAMode)2;
		((Control)progress2).Dock = (DockStyle)5;
		progress2.Fill = Color.FromArgb(255, 233, 0);
		((IControl)progress2).HandCursor = Cursors.Default;
		((IControl)progress2).HandDragFolder = false;
		grpan_procDesc.SetIndex((Control)(object)progress2, 3);
		((Control)progress2).Location = new Point(5, 38);
		((Control)progress2).Margin = new Padding(5);
		((Control)progress2).Name = "progress2";
		((Control)progress2).Size = new Size(727, 31);
		((Control)progress2).TabIndex = 4;
		((Control)progress2).TabStop = false;
		((Control)progress2).Text = "";
		progress2.Value = 0.2f;
		progress2.ValueRatio = 0.7f;
		((Control)pictureBox1).Dock = (DockStyle)5;
		pictureBox1.Image = (Image)(object)Resources.升级中;
		((Control)pictureBox1).Location = new Point(5, 5);
		((Control)pictureBox1).Margin = new Padding(5);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(72, 69);
		pictureBox1.SizeMode = (PictureBoxSizeMode)3;
		pictureBox1.TabIndex = 0;
		pictureBox1.TabStop = false;
		uploadDragger1.Back = Color.FromArgb(33, 36, 39);
		((Control)uploadDragger1).BackColor = Color.FromArgb(33, 36, 39);
		uploadDragger1.BorderColor = Color.FromArgb(66, 69, 71);
		uploadDragger1.BorderWidth = 2f;
		uploadDragger1.ClickHand = false;
		((IControl)uploadDragger1).ColorScheme = (TAMode)2;
		((Control)uploadDragger1).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		uploadDragger1.ForeColor = Color.FromArgb(255, 255, 255);
		((IControl)uploadDragger1).HandDragFolder = false;
		uploadDragger1.IconRatio = 0f;
		uploadDragger1.IconSvg = "";
		grpan_upgrade.SetIndex((Control)(object)uploadDragger1, 2);
		((Control)uploadDragger1).Location = new Point(5, 132);
		((Control)uploadDragger1).Margin = new Padding(5);
		((Control)uploadDragger1).Name = "uploadDragger1";
		((Control)uploadDragger1).Padding = new Padding(10);
		((Control)uploadDragger1).Size = new Size(820, 81);
		((Control)uploadDragger1).TabIndex = 8;
		((Control)uploadDragger1).TabStop = false;
		((Control)uploadDragger1).Text = "1234";
		uploadDragger1.TextDesc = "5678";
		((IControl)uploadDragger1).DragChanged += new DragEventHandler(uploadDragger1_DragChanged);
		((Control)uploadDragger1).MouseClick += new MouseEventHandler(uploadDragger1_MouseClick);
		((Control)pageHeader1).BackColor = Color.Transparent;
		((Control)pageHeader1).Controls.Add((Control)(object)sel_portname);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_Disconnect);
		((Control)pageHeader1).Controls.Add((Control)(object)sel_LensType);
		((Control)pageHeader1).Controls.Add((Control)(object)switch1);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_angProtect);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_LensType);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_CloseGM);
		((Control)pageHeader1).Controls.Add((Control)(object)pictureBox2);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_firmwareVer);
		((Control)pageHeader1).Controls.Add((Control)(object)divider1);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_tempture);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_hardwareVer);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_sn);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_devName);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_refresh);
		pageHeader1.DragMove = false;
		pageHeader1.Gap = 0;
		grpan_main.SetIndex((Control)(object)pageHeader1, 1);
		((Control)pageHeader1).Location = new Point(5, 5);
		((Control)pageHeader1).Margin = new Padding(5);
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Size = new Size(836, 118);
		((Control)pageHeader1).TabIndex = 53;
		((Control)pageHeader1).TabStop = false;
		((Control)pageHeader1).Text = "";
		((Input)sel_portname).BackColor = Color.FromArgb(30, 34, 37);
		((Input)sel_portname).BorderWidth = 0f;
		((IControl)sel_portname).ColorScheme = (TAMode)2;
		((Control)sel_portname).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_portname).ForeColor = Color.White;
		sel_portname.List = true;
		((Control)sel_portname).Location = new Point(448, 13);
		((Control)sel_portname).Margin = new Padding(5);
		((Control)sel_portname).Name = "sel_portname";
		((Control)sel_portname).Size = new Size(97, 30);
		((Control)sel_portname).TabIndex = 38;
		((Input)sel_portname).WaveSize = 0;
		btn_Disconnect.DefaultBack = Color.FromArgb(95, 95, 96);
		btn_Disconnect.ForeColor = Color.White;
		((Control)btn_Disconnect).Location = new Point(577, 13);
		((Control)btn_Disconnect).Name = "btn_Disconnect";
		((Control)btn_Disconnect).Size = new Size(100, 30);
		((Control)btn_Disconnect).TabIndex = 41;
		((Control)btn_Disconnect).Text = "Disconnect";
		btn_Disconnect.TextMultiLine = true;
		btn_Disconnect.WaveSize = 0;
		((Control)btn_Disconnect).Click += btn_Disconnect_Click_1;
		((Control)switch1).BackColor = Color.FromArgb(38, 41, 43);
		switch1.Checked = true;
		switch1.Fill = Color.FromArgb(255, 233, 0);
		switch1.FillHover = Color.FromArgb(255, 255, 255);
		((IControl)switch1).HandDragFolder = false;
		((Control)switch1).Location = new Point(711, 87);
		((Control)switch1).Name = "switch1";
		((Control)switch1).Size = new Size(77, 15);
		((Control)switch1).TabIndex = 42;
		((Control)switch1).TabStop = false;
		switch1.UnCheckedText = "close";
		switch1.WaveSize = 0;
		switch1.CheckedChanged += new BoolEventHandler(switch1_CheckedChanged);
		lab_angProtect.AutoSizeMode = (TAutoSize)1;
		((Control)lab_angProtect).BackColor = Color.Transparent;
		((Control)lab_angProtect).Font = new Font("思源黑体", 8.249999f, (FontStyle)0, (GraphicsUnit)3, (byte)128);
		lab_angProtect.ForeColor = Color.White;
		((Control)lab_angProtect).Location = new Point(711, 63);
		((Control)lab_angProtect).Margin = new Padding(0);
		((Control)lab_angProtect).Name = "lab_angProtect";
		((Control)lab_angProtect).Size = new Size(70, 16);
		lab_angProtect.SuffixColor = Color.White;
		((Control)lab_angProtect).TabIndex = 43;
		((Control)lab_angProtect).TabStop = false;
		((Control)lab_angProtect).Text = "Angle protect";
		lab_angProtect.TextAlign = (ContentAlignment)32;
		lab_angProtect.TextMultiLine = false;
		btn_CloseGM.BackColor = Color.FromArgb(255, 233, 0);
		btn_CloseGM.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_CloseGM).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_CloseGM.ForeColor = Color.FromArgb(35, 35, 35);
		btn_CloseGM.IconGap = 0f;
		btn_CloseGM.IconPosition = (TAlignMini)0;
		btn_CloseGM.IconRatio = 0f;
		btn_CloseGM.IconSvg = "";
		((Control)btn_CloseGM).Location = new Point(696, 13);
		((Control)btn_CloseGM).Margin = new Padding(0);
		((Control)btn_CloseGM).Name = "btn_CloseGM";
		((Control)btn_CloseGM).Size = new Size(100, 30);
		((Control)btn_CloseGM).TabIndex = 40;
		((Control)btn_CloseGM).Text = "CloseGimbal";
		btn_CloseGM.TextMultiLine = true;
		btn_CloseGM.WaveSize = 0;
		((Control)btn_CloseGM).Click += btn_CloseGM_Click;
		((Control)pictureBox2).BackColor = Color.Transparent;
		((Control)pictureBox2).Dock = (DockStyle)3;
		pictureBox2.Image = (Image)(object)Resources.new_Logo;
		((Control)pictureBox2).Location = new Point(0, 0);
		((Control)pictureBox2).Margin = new Padding(0);
		((Control)pictureBox2).Name = "pictureBox2";
		((Control)pictureBox2).Size = new Size(131, 108);
		pictureBox2.SizeMode = (PictureBoxSizeMode)4;
		pictureBox2.TabIndex = 37;
		pictureBox2.TabStop = false;
		lab_firmwareVer.AutoSizeMode = (TAutoSize)1;
		((Control)lab_firmwareVer).BackColor = Color.Transparent;
		((Control)lab_firmwareVer).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_firmwareVer.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_firmwareVer).Location = new Point(139, 85);
		((Control)lab_firmwareVer).Margin = new Padding(0);
		((Control)lab_firmwareVer).Name = "lab_firmwareVer";
		((Control)lab_firmwareVer).Size = new Size(29, 15);
		lab_firmwareVer.SuffixColor = Color.White;
		((Control)lab_firmwareVer).TabIndex = 32;
		((Control)lab_firmwareVer).TabStop = false;
		((Control)lab_firmwareVer).Text = "固件";
		lab_firmwareVer.TextAlign = (ContentAlignment)32;
		lab_firmwareVer.TextMultiLine = false;
		((Control)divider1).BackColor = Color.Transparent;
		divider1.ColorSplit = Color.FromArgb(255, 233, 0);
		((Control)divider1).Dock = (DockStyle)2;
		((Control)divider1).Location = new Point(0, 108);
		((Control)divider1).Name = "divider1";
		divider1.OrientationMargin = 0f;
		((Control)divider1).Size = new Size(836, 10);
		((Control)divider1).TabIndex = 36;
		((Control)divider1).Text = "";
		divider1.TextPadding = 0f;
		divider1.Thickness = 2f;
		lab_tempture.AutoSizeMode = (TAutoSize)1;
		((Control)lab_tempture).BackColor = Color.Transparent;
		((Control)lab_tempture).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_tempture.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_tempture).Location = new Point(364, 85);
		((Control)lab_tempture).Margin = new Padding(0);
		((Control)lab_tempture).Name = "lab_tempture";
		((Control)lab_tempture).Size = new Size(72, 15);
		lab_tempture.SuffixColor = Color.White;
		((Control)lab_tempture).TabIndex = 34;
		((Control)lab_tempture).TabStop = false;
		((Control)lab_tempture).Text = "芯片温度：";
		lab_tempture.TextAlign = (ContentAlignment)32;
		lab_tempture.TextMultiLine = false;
		lab_hardwareVer.AutoSizeMode = (TAutoSize)1;
		((Control)lab_hardwareVer).BackColor = Color.Transparent;
		((Control)lab_hardwareVer).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_hardwareVer.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_hardwareVer).Location = new Point(138, 54);
		((Control)lab_hardwareVer).Margin = new Padding(0);
		((Control)lab_hardwareVer).Name = "lab_hardwareVer";
		((Control)lab_hardwareVer).Size = new Size(33, 17);
		lab_hardwareVer.Suffix = "";
		lab_hardwareVer.SuffixColor = Color.White;
		((Control)lab_hardwareVer).TabIndex = 31;
		((Control)lab_hardwareVer).TabStop = false;
		((Control)lab_hardwareVer).Text = "硬件";
		lab_hardwareVer.TextAlign = (ContentAlignment)32;
		lab_hardwareVer.TextMultiLine = false;
		lab_sn.AutoSizeMode = (TAutoSize)1;
		((Control)lab_sn).BackColor = Color.Transparent;
		((Control)lab_sn).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_sn.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_sn).Location = new Point(366, 55);
		((Control)lab_sn).Margin = new Padding(0);
		((Control)lab_sn).Name = "lab_sn";
		((Control)lab_sn).Size = new Size(33, 15);
		lab_sn.SuffixColor = Color.White;
		((Control)lab_sn).TabIndex = 33;
		((Control)lab_sn).TabStop = false;
		((Control)lab_sn).Text = "SN :  ";
		lab_sn.TextAlign = (ContentAlignment)32;
		lab_sn.TextMultiLine = false;
		lab_devName.AutoSizeMode = (TAutoSize)1;
		((Control)lab_devName).BackColor = Color.Transparent;
		((Control)lab_devName).Font = new Font("Microsoft Sans Serif", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		lab_devName.ForeColor = Color.White;
		((Control)lab_devName).Location = new Point(138, 5);
		((Control)lab_devName).Margin = new Padding(15, 0, 0, 0);
		((Control)lab_devName).Name = "lab_devName";
		((Control)lab_devName).Size = new Size(131, 31);
		((Control)lab_devName).TabIndex = 1;
		((Control)lab_devName).TabStop = false;
		((Control)lab_devName).Text = "SimulaGM";
		lab_devName.TextAlign = (ContentAlignment)32;
		lab_devName.TextMultiLine = false;
		btn_refresh.DisplayStyle = (TButtonDisplayStyle)2;
		btn_refresh.ForeColor = Color.White;
		btn_refresh.Ghost = true;
		btn_refresh.Icon = (Image)(object)Resources.refresh_yellow;
		btn_refresh.IconRatio = 1f;
		((Control)btn_refresh).Location = new Point(545, 13);
		((Control)btn_refresh).Margin = new Padding(0);
		((Control)btn_refresh).Name = "btn_refresh";
		((Control)btn_refresh).Size = new Size(25, 30);
		((Control)btn_refresh).TabIndex = 39;
		((Control)btn_refresh).Text = "Refresh";
		btn_refresh.WaveSize = 0;
		((Control)btn_refresh).Click += btn_refresh_Click;
		((ContainerPanel)grpan_main).Back = Color.Transparent;
		((Control)grpan_main).BackColor = Color.Transparent;
		((ContainerPanel)grpan_main).BorderColor = Color.Black;
		((Control)grpan_main).Controls.Add((Control)(object)pageHeader1);
		((Control)grpan_main).Controls.Add((Control)(object)grpan_upgrade);
		((Control)grpan_main).Controls.Add((Control)(object)grpan_param);
		((Control)grpan_main).Controls.Add((Control)(object)grpan_chann);
		((Control)grpan_main).Controls.Add((Control)(object)grpan_gain);
		((Control)grpan_main).Dock = (DockStyle)5;
		((Control)grpan_main).Location = new Point(5, 5);
		((Control)grpan_main).Margin = new Padding(0);
		((Control)grpan_main).Name = "grpan_main";
		((Control)grpan_main).Size = new Size(846, 638);
		grpan_main.Span = "100%;100%;50% 50%;\r\n100%;\r\n-20% 15% 15% 50%";
		((Control)grpan_main).TabIndex = 0;
		((Control)grpan_main).Text = "gridPanel4";
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).Controls.Add((Control)(object)grpan_main);
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "NewGimbalFrm";
		((Control)this).Padding = new Padding(5);
		((Control)this).Size = new Size(856, 648);
		((UserControl)this).Load += NewGimbalFrm_Load;
		((Control)grpan_gain).ResumeLayout(false);
		((Control)grpan_chann).ResumeLayout(false);
		((Control)grpan_param).ResumeLayout(false);
		((Control)grpan_upgrade).ResumeLayout(false);
		((Control)pageHeader2).ResumeLayout(false);
		((Control)pageHeader2).PerformLayout();
		((Control)grpan_process).ResumeLayout(false);
		((Control)grpan_procDesc).ResumeLayout(false);
		((Control)stackPanel1).ResumeLayout(false);
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)pageHeader1).ResumeLayout(false);
		((Control)pageHeader1).PerformLayout();
		((ISupportInitialize)pictureBox2).EndInit();
		((Control)grpan_main).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
