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
using AntdUI;
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

	private string startGM_cn = "启动云台";

	private string stopGM_cn = "停止云台";

	private string calib_cn = "校准陀螺仪";

	private string startUpg_cn = "开始升级";

	private string openParam_cn = "载入参数";

	private string saveParam_cn = "保存参数";

	private string writeParam_cn = "烧写参数";

	private string modeChan_cn = "模式通道";

	private string sensChan_cn = "灵敏度通道";

	private string rollChan_cn = "滚转通道";

	private string pitchChan_cn = "俯仰通道";

	private string yawChan_cn = "指向通道";

	private string rollGain_cn = "滚转增益";

	private string pitchGain_cn = "俯仰增益";

	private string yawGain_cn = "指向增益";

	private string disconn_cn = "断开串口";

	private string closeCtrl_cn = "关闭页面";

	private string selTitle_cn = "选择文件";

	private string selectPath_cn = "  选择固件";

	private string desc_cn = "将固件拖拽到此区域 ，或";

	private string upgDesc_cn = "正在传输固件";

	private string hardware_cn = "硬件 : ";

	private string firmware_cn = "固件 : ";

	private string tempture_cn = "芯片温度 : ";

	private string gmMode_cn = "云台模式选择";

	private string currMode_cn = "当前云台模式";

	private string sensNum_cn = "灵敏度设置";

	private string rollNum_cn = "滚转角度设置";

	private string pitchNum_cn = "俯仰角度设置";

	private string yawNum_cn = "指向角度设置";

	private string angProtect_cn = "限角保护";

	private string fpvMode_cn = "三轴跟随";

	private string pitchMode_cn = "双轴跟随";

	private string horiMode_cn = "单轴跟随";

	private string lookDownMode_cn = "LookDown-Mode";

	private string startGM_en = "Start";

	private string stopGM_en = "Stop";

	private string calib_en = "Calib gyro";

	private string startUpg_en = "Upgrade";

	private string openParam_en = "Load param";

	private string saveParam_en = "Save param";

	private string writeParam_en = "Write param";

	private string modeChan_en = "Mode Chan";

	private string sensChan_en = "Sensit Chan";

	private string rollChan_en = "Roll Chan";

	private string pitchChan_en = "Pitch Chan";

	private string yawChan_en = "Yaw Chan";

	private string rollGain_en = "Roll gain";

	private string pitchGain_en = "Pitch gain";

	private string yawGain_en = "Yaw gain";

	private string disconn_en = "Disconnect";

	private string closeCtrl_en = "ClosePage";

	private string selTitle_en = "Select firmware";

	private string selectPath_en = "  Select file";

	private string desc_en = "Drag the firmware to this area, or";

	private string upgDesc_en = "Transfer firmware file";

	private string hardware_en = "Hardware : ";

	private string firmware_en = "Firmware : ";

	private string tempture_en = "Temperature : ";

	private string angProtect_en = "Angle protect";

	private string gmMode_en = "Mode select";

	private string currMode_en = "Curr gimbal mode";

	private string sensNum_en = "Sens";

	private string rollNum_en = "Roll ang";

	private string pitchNum_en = "Pitch ang";

	private string yawNum_en = "Yaw ang";

	private string fpvMode_en = "FPV-Mode";

	private string pitchMode_en = "Pitch-Stabilized-Mode";

	private string horiMode_en = "Horizon-Mode";

	private string lookDownMode_en = "LookDown-Mode";

	private float[] _modeSlotCMD = new float[3];

	private bool _isChangeModeSlot = false;

	private List<UsbDevInfo> _usbDevInfo;

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

	private Button btn_calib;

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

	private Label lab_filepath;

	private Label lab_gmMode;

	private Label lab_yawNum;

	private Input inp_yawNum;

	private Input inp_pitchNum;

	private Label lab_pitchNum;

	private Input inp_rollNum;

	private Input inp_sensNum;

	private Label lab_rollNum;

	private Label lab_sensNum;

	private Button btn_stopGM;

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
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		((IControl)grpan_process).Visible = false;
		((IControl)grpan_upgrade).Visible = true;
		InitCtrlList();
		if (GD.Inst.SP_Gim != null)
		{
			GD.Inst.SP_Gim.Dispose();
			GD.Inst.SP_Gim = null;
		}
		GD.Inst.SP_Gim = new Serialport_Gimbal(_resAsceInfo.UsbInfo);
		if (!GD.Inst.SP_Gim.Open(GD.Inst.CurrPortName))
		{
			WriteLog.WriteLogFileToUI("云台串口打开失败，请检查连接及端口设置", Color.Red);
			btn_Disconnect.DefaultBack = Color.FromArgb(255, 233, 0);
			btn_Disconnect.ForeColor = Color.FromArgb(35, 35, 35);
			((Control)btn_Disconnect).Text = ((GD.Inst.CurrLang == 1) ? "重新连接" : "Reconnect");
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "警告";
				desc = "云台串口未连接";
			}
			else
			{
				title = "Warning";
				desc = "gimbal serialport is not connect.";
			}
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, Color.Red);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		((Control)btn_Disconnect).Text = ((GD.Inst.CurrLang == 1) ? disconn_cn : disconn_en);
		GD.Inst.Upg_Gim = new Upg_Gimbal(GD.Inst.SP_Gim);
		BindEventHandler();
		Thread.Sleep(100);
		_currCMD = 0;
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.StartGim);
		lab_hardwareVer.Suffix = _resAsceInfo.HWVers;
		lab_firmwareVer.Suffix = _resAsceInfo.FWVers;
		lab_tempture.Suffix = _resAsceInfo.MCUTemp + "℃";
		if (_resAsceInfo.SN != null)
		{
			int count = ((_resAsceInfo.SN.Length > 6) ? (_resAsceInfo.SN.Length - 6) : 0);
			((Control)lab_sn).Text = "SN : ";
			lab_sn.Suffix = _resAsceInfo.SN.Remove(0, count);
			if (_resAsceInfo.DevName == "SimGM")
			{
				pictureBox2.Image = (Image)(object)Resources.new_Logo;
				((Control)lab_devName).Text = "SimGM";
			}
			else if (_resAsceInfo.DevName.Contains("GM1"))
			{
				pictureBox2.Image = (Image)(object)Resources.GM1_V2;
				((Control)lab_devName).Text = "GM1 V2";
			}
			else if (_resAsceInfo.DevName.Contains("GM3"))
			{
				pictureBox2.Image = (Image)(object)Resources.GM3_V2;
				((Control)lab_devName).Text = "GM3 V2";
			}
		}
		ReloadSelItems();
		ReloadFont();
		ReloadLang();
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
		for (int i = 0; i < 11; i++)
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
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.AngProtect, rollcmd);
		}
	}

	private void btn_CloseGM_Click(object sender, EventArgs e)
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Invalid comparison between Unknown and I4
		try
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
		if (((Control)val).Text == disconn_cn || ((Control)val).Text == disconn_en)
		{
			GD.Inst.SP_Gim.Close();
			((Control)val).Text = ((GD.Inst.CurrLang == 1) ? "重新连接" : "Reconnect");
			val.DefaultBack = Color.FromArgb(255, 233, 0);
			val.ForeColor = Color.FromArgb(35, 35, 35);
		}
		else if (GD.Inst.SP_Gim.Open(GD.Inst.CurrPortName))
		{
			((Control)val).Text = ((GD.Inst.CurrLang == 1) ? disconn_cn : disconn_en);
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
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
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
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "警告";
				desc = "只允许一位小数点。";
			}
			else
			{
				title = "Warning";
				desc = "Allow only one decimal separator.";
			}
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		int num2 = ((Control)(Input)sender).Text.IndexOf('.');
		if (char.IsDigit(e.Char))
		{
			e.Result = true;
			return;
		}
		if (text.Equals(numberDecimalSeparator) && ((Control)(Input)sender).Text.IndexOf('.') < 0)
		{
			e.Result = true;
			return;
		}
		if (e.Char == '\b')
		{
			e.Result = true;
			return;
		}
		if (e.Char == '-')
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
			return;
		}
		e.Result = false;
		string title2;
		string desc2;
		if (GD.Inst.CurrLang == 1)
		{
			title2 = "警告";
			desc2 = "只允许输入数字。";
		}
		else
		{
			title2 = "Warning";
			desc2 = "Only numbers allowed.";
		}
		CommModalFrm commModalFrm2 = new CommModalFrm();
		commModalFrm2.SetAllTxt(title2, desc2, isshowBtnOK: true, isshowBtnCan: false);
		((Form)commModalFrm2).ShowDialog();
	}

	private void inp_sensNum_Leave(object sender, EventArgs e)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
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
		if (num > 1f || num < -1f)
		{
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "警告";
				desc = "灵敏度数值超限，自动修改为限制内。";
			}
			else
			{
				title = "Warning";
				desc = "If the sensitivity value exceeds the limit, it will be automatically modified to within the limit.";
			}
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			if (num < -1f)
			{
				num = -1f;
			}
			else if (num > 1f)
			{
				num = 1f;
			}
			((Control)val).Text = num.ToString("f1");
		}
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetSensNum, num);
	}

	private void inp_rollNum_Leave(object sender, EventArgs e)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
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
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "警告";
				desc = "滚转角度数值超限，自动修改为限制内。";
			}
			else
			{
				title = "Warning";
				desc = "Roll angle value exceeds limit, automatically modified to within limit.";
			}
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
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetRollNum, num);
	}

	private void inp_pitchNum_Leave(object sender, EventArgs e)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
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
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "警告";
				desc = "俯仰角度数值超限，自动修改为限制内。";
			}
			else
			{
				title = "Warning";
				desc = "Pitch angle value exceeds limit, automatically modified to within limit.";
			}
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
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetPitchNum, num);
	}

	private void inp_yawNum_Leave(object sender, EventArgs e)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
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
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "警告";
				desc = "偏航数值超限,自动修改为限制内。";
			}
			else
			{
				title = "Warning";
				desc = "Yaw angle value exceeds limit, automatically modified to within limit.";
			}
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
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetYawNum, num);
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
				GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetGMMode, val2);
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
			break;
		case 2:
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
			break;
		case 3:
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
			break;
		}
	}

	private async void OnDeviceFrameInfo(bool arg1, ConstEnum.RecvPacket_Gim arg2, object arg3)
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
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
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
					((Control)lab_upgDesc).Text = "Gyro calib is completed ";
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
					commModalFrm.SetAllTxt("Error", e.msg, Color.Red);
					((Form)commModalFrm).ShowDialog();
				}
				break;
			case InfoType.crash:
				black = Color.DarkRed;
				break;
			case InfoType.upgProcInfo:
				progress2.Value = e.processVal;
				if (e.msg == "procFinish")
				{
					pictureBox1.Image = (Image)(object)Resources.升级成功;
					((Control)lab_upgDesc).Text = e.processDesc;
					progress2.State = (TType)1;
					progress2.Fill = Color.FromArgb(43, 164, 113);
					GridPanel obj4 = grpan_chann;
					GridPanel obj5 = grpan_param;
					bool flag = (((IControl)grpan_gain).Enabled = true);
					bool enabled = (((IControl)obj5).Enabled = flag);
					((IControl)obj4).Enabled = enabled;
					_isStartUpg = false;
					EnableCtrl(!_isStartUpg);
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
					((Control)lab_upgDesc).Text = "Gyro calibration completed";
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

	private void RefreshTxtbox(ConstEnum.RecvPacket_Gim pac)
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
			string text = "Gimbal stop";
			switch (pac.currMode)
			{
			case 1:
				text = ((GD.Inst.CurrLang == 1) ? horiMode_cn : horiMode_en);
				break;
			case 4:
				text = ((GD.Inst.CurrLang == 1) ? lookDownMode_cn : lookDownMode_en);
				break;
			case 6:
				text = ((GD.Inst.CurrLang == 1) ? pitchMode_cn : pitchMode_en);
				break;
			case 7:
				text = ((GD.Inst.CurrLang == 1) ? fpvMode_cn : fpvMode_en);
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
			}
			sel_M2.SelectedIndex = selectedIndex3;
			switch1.Checked = ((pac.angleProtectEnable != 0) ? true : false);
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
	}

	private bool CheckSerialportConnect()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
			{
				CommModalFrm commModalFrm = new CommModalFrm();
				string title;
				string desc;
				if (GD.Inst.CurrLang == 1)
				{
					title = "提示";
					desc = "请先连接云台";
				}
				else
				{
					title = "Warning";
					desc = "Please connect the gimbal.";
				}
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
			_currCMD = 0;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.StartGim);
		}
	}

	private void btn_stopGM_Click(object sender, EventArgs e)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "提示";
				desc = "请先连接云台";
			}
			else
			{
				title = "Warning";
				desc = "Please connect the gimbal.";
			}
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
		}
		else
		{
			_currCMD = 1;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.StopGim);
		}
	}

	private void btn_calib_Click(object sender, EventArgs e)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Invalid comparison between Unknown and I4
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "提示";
				desc = "请先连接云台";
			}
			else
			{
				title = "Warning";
				desc = "Please connect the gimbal.";
			}
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		string title2;
		string desc2;
		if (GD.Inst.CurrLang == 1)
		{
			title2 = "提示";
			desc2 = "陀螺仪校准约20s，云台需静止。";
		}
		else
		{
			title2 = "Prompt";
			desc2 = "Gyro calibration takes about 20s, gimbal needs to be stationary。";
		}
		CommModalFrm commModalFrm2 = new CommModalFrm();
		commModalFrm2.SetAllTxt(title2, desc2);
		((Form)commModalFrm2).ShowDialog();
		if ((int)commModalFrm2.FrmResult == 1)
		{
			GD.Inst.SW_Calib.Restart();
			((Control)lab_upgDesc).Text = ((GD.Inst.CurrLang == 1) ? "陀螺仪校准中" : "Gyro in calibration");
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
			}
			_currCMD = 2;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.CalibGyro);
		}
	}

	private void EnableCtrl(bool isEnable)
	{
		Button obj = btn_startUpg;
		UploadDragger obj2 = uploadDragger1;
		Select obj3 = sel_gmMoed;
		Input obj4 = inp_sensNum;
		Input obj5 = inp_rollNum;
		Button obj6 = btn_refresh;
		Input obj7 = inp_pitchNum;
		Input obj8 = inp_yawNum;
		GridPanel obj9 = grpan_chann;
		GridPanel obj10 = grpan_gain;
		GridPanel obj11 = grpan_param;
		bool flag = (((IControl)btn_Disconnect).Enabled = isEnable);
		bool flag3 = (((IControl)obj11).Enabled = flag);
		bool flag5 = (((IControl)obj10).Enabled = flag3);
		bool flag7 = (((IControl)obj9).Enabled = flag5);
		bool flag9 = (((IControl)obj8).Enabled = flag7);
		bool flag11 = (((IControl)obj7).Enabled = flag9);
		bool flag13 = (((IControl)obj6).Enabled = flag11);
		bool flag15 = (((IControl)obj5).Enabled = flag13);
		bool flag17 = (((IControl)obj4).Enabled = flag15);
		bool flag19 = (((IControl)obj3).Enabled = flag17);
		bool enabled = (((IControl)obj2).Enabled = flag19);
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
			((FileDialog)ofd).Title = "请选择参数文件";
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
			for (int i = 0; i < paramList.Count; i++)
			{
				WriteLog.WriteLogFileToUI($"param[{i}]={paramList[i]}", Color.Black);
				await Task.Delay(2);
			}
			sel_modeChan.SelectedIndex = paramList[0];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetModeChann, paramList[0], 0f, 0f);
			await Task.Delay(20);
			sel_senseChan.SelectedIndex = paramList[1];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetSensChann, paramList[1], 0f, 0f);
			await Task.Delay(20);
			sel_rollChan.SelectedIndex = paramList[2];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetRollChann, paramList[2], 0f, 0f);
			await Task.Delay(20);
			sel_pitchChan.SelectedIndex = paramList[3];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetPitchChann, paramList[3], 0f, 0f);
			await Task.Delay(20);
			sel_yawChan.SelectedIndex = paramList[4];
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetYawChann, paramList[4], 0f, 0f);
			await Task.Delay(20);
			((Control)inp_rollGain).Text = paramList[5].ToString();
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetRollGain, paramList[5], 0f, 0f);
			await Task.Delay(20);
			((Control)inp_pitchGain).Text = paramList[6].ToString();
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetPitchGain, 0f, paramList[6]);
			await Task.Delay(20);
			((Control)inp_yawGain).Text = paramList[7].ToString();
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetYawGain, 0f, 0f, paramList[7]);
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
			}
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.ModeSlot, paramList[8], paramList[9], paramList[10]);
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
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Invalid comparison between Unknown and I4
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "提示";
				desc = "请先连接云台";
			}
			else
			{
				title = "Warning";
				desc = "Please connect the gimbal.";
			}
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		SaveFileDialog val = new SaveFileDialog();
		try
		{
			((FileDialog)val).Title = "保存参数文件";
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
				string contents = ((JToken)val2).ToString((Formatting)1);
				File.WriteAllText(((FileDialog)val).FileName, contents);
				CommModalFrm commModalFrm2 = new CommModalFrm();
				commModalFrm2.SetAllTxt("Prompt", "Save param successful", isshowBtnOK: true, isshowBtnCan: false);
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
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		if (GD.Inst.Upg_Gim == null || GD.Inst.SP_Gim.SPobj == null || !GD.Inst.SP_Gim.IsComOpened)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "提示";
				desc = "请先连接云台";
			}
			else
			{
				title = "Warning";
				desc = "Please connect the gimbal.";
			}
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		if (_isChangeModeSlot)
		{
			_isChangeModeSlot = false;
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.ModeSlot, _modeSlotCMD[0], _modeSlotCMD[1], _modeSlotCMD[2]);
			Thread.Sleep(10);
			Thread.Sleep(10);
			float rollcmd = ((!switch1.Checked) ? 1 : 0);
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.AngProtect, rollcmd);
			Thread.Sleep(10);
		}
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.WriteParam);
		string title2;
		string desc2;
		if (GD.Inst.CurrLang == 1)
		{
			title2 = "提示";
			desc2 = "烧写参数成功";
		}
		else
		{
			title2 = "Prompt";
			desc2 = "Write param successful。";
		}
		CommModalFrm commModalFrm2 = new CommModalFrm();
		commModalFrm2.SetAllTxt(title2, desc2, isshowBtnOK: true, isshowBtnCan: false);
		((Form)commModalFrm2).ShowDialog();
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
					((Control)lab_filepath).Text = (_filePath = path);
					((IControl)btn_startUpg).Enabled = true;
				});
				string text = "";
				string version = GetVersion(path, b: false);
				string text2 = path.ToLower();
				if (text2.Contains("g_sky") && (text2.Contains("z8") || text2.Contains("z40")))
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
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
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
				commModalFrm.SetAllTxt("提示", "当前升级机制为Asce机制");
				((Form)commModalFrm).ShowDialog();
			}
			_isStartUpg = true;
			pictureBox1.Image = (Image)(object)Resources.升级中;
			((Control)lab_upgDesc).Text = ((GD.Inst.CurrLang == 1) ? "开始升级" : "Start upgrading");
			EnableCtrl(!_isStartUpg);
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
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string[] value = ((VEventArgs<string[]>)(object)e).Value;
			string text = value[0];
			FileAttributes attributes = File.GetAttributes(text);
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
			else
			{
				((Control)lab_filepath).Text = (_filePath = text);
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
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetModeChann, rollcmd);
	}

	private void inp_sensChan_Leave(object sender, EventArgs e)
	{
		InputNumber val = (InputNumber)((sender is InputNumber) ? sender : null);
		float rollcmd = float.Parse(((Control)val).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetSensChann, rollcmd);
	}

	private void inp_rollChan_Leave(object sender, EventArgs e)
	{
		InputNumber val = (InputNumber)((sender is InputNumber) ? sender : null);
		float rollcmd = float.Parse(((Control)val).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetRollChann, rollcmd);
	}

	private void inp_pitchChan_Leave(object sender, EventArgs e)
	{
		InputNumber val = (InputNumber)((sender is InputNumber) ? sender : null);
		float rollcmd = float.Parse(((Control)val).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetPitchChann, rollcmd);
	}

	private void inp_yawChan_Leave(object sender, EventArgs e)
	{
		InputNumber val = (InputNumber)((sender is InputNumber) ? sender : null);
		float rollcmd = float.Parse(((Control)val).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetYawChann, rollcmd);
	}

	private void lap_rollGain_MouseClick(object sender, MouseEventArgs e)
	{
	}

	private void lab_pitchGain_MouseClick(object sender, MouseEventArgs e)
	{
		float pitchcmd = float.Parse(((Control)inp_pitchGain).Text);
		GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetPitchGain, 0f, pitchcmd);
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
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetRollGain, rollcmd);
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
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetPitchGain, 0f, pitchcmd);
		}
	}

	private void inp_yawGain_Leave(object sender, EventArgs e)
	{
		if (CheckSerialportConnect())
		{
			Input val = (Input)((sender is Input) ? sender : null);
			float yawcmd = float.Parse(((Control)val).Text);
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetYawGain, 0f, 0f, yawcmd);
		}
	}

	private void inp_rollGain_VerifyChar(object sender, InputVerifyCharEventArgs e)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
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
		string title;
		string desc;
		if (GD.Inst.CurrLang == 1)
		{
			title = "警告";
			desc = "只允许输入数字。";
		}
		else
		{
			title = "Warning";
			desc = "Only numbers allowed.";
		}
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
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetModeChann, rollcmd);
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
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetSensChann, rollcmd);
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
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetRollChann, rollcmd);
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
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetPitchChann, rollcmd);
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
			GD.Inst.Upg_Gim.BuildSendPacket_Gimbal(ConstEnum.GIM_CMD.SetYawChann, rollcmd);
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
		}
		_modeSlotCMD[2] = num;
		paramList[10] = (int)num;
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
			switch ((LangType)GD.Inst.CurrLang)
			{
			case LangType.zh_CN:
				((Control)btn_startGM).Text = startGM_cn;
				((Control)btn_stopGM).Text = stopGM_cn;
				((Control)btn_calib).Text = calib_cn;
				((Control)lab_modeChan).Text = modeChan_cn;
				((Control)lab_sensChan).Text = sensChan_cn;
				((Control)lab_rollChan).Text = rollChan_cn;
				((Control)lab_pitchChan).Text = pitchChan_cn;
				((Control)lab_yawChan).Text = yawChan_cn;
				((Control)lab_rollGain).Text = rollGain_cn;
				((Control)lab_pitchGain).Text = pitchGain_cn;
				((Control)lab_yawGain).Text = yawGain_cn;
				((Control)btn_openParam).Text = openParam_cn;
				((Control)btn_saveParam).Text = saveParam_cn;
				((Control)btn_WriteParam).Text = writeParam_cn;
				((Control)btn_startUpg).Text = startUpg_cn;
				((Control)btn_Disconnect).Text = disconn_cn;
				((Control)lab_filepath).Text = desc_cn;
				lab_filepath.Suffix = selectPath_cn;
				((Control)lab_upgDesc).Text = upgDesc_cn;
				((Control)lab_hardwareVer).Text = hardware_cn;
				((Control)lab_firmwareVer).Text = firmware_cn;
				((Control)lab_tempture).Text = tempture_cn;
				((Control)lab_gmMode).Text = gmMode_cn;
				((Control)lab_sensNum).Text = sensNum_cn;
				((Control)lab_rollNum).Text = rollNum_cn;
				((Control)lab_pitchNum).Text = pitchNum_cn;
				((Control)lab_yawNum).Text = yawNum_cn;
				((Control)lab_currGMMode).Text = currMode_cn;
				((Control)lab_angProtect).Text = angProtect_cn;
				break;
			case LangType.en_US:
				((Control)btn_startGM).Text = startGM_en;
				((Control)btn_stopGM).Text = stopGM_en;
				((Control)btn_calib).Text = calib_en;
				((Control)lab_modeChan).Text = modeChan_en;
				((Control)lab_sensChan).Text = sensChan_en;
				((Control)lab_rollChan).Text = rollChan_en;
				((Control)lab_pitchChan).Text = pitchChan_en;
				((Control)lab_yawChan).Text = yawChan_en;
				((Control)lab_rollGain).Text = rollGain_en;
				((Control)lab_pitchGain).Text = pitchGain_en;
				((Control)lab_yawGain).Text = yawGain_en;
				((Control)btn_openParam).Text = openParam_en;
				((Control)btn_saveParam).Text = saveParam_en;
				((Control)btn_WriteParam).Text = writeParam_en;
				((Control)btn_startUpg).Text = startUpg_en;
				((Control)btn_Disconnect).Text = disconn_en;
				((Control)lab_filepath).Text = desc_en;
				lab_filepath.Suffix = selectPath_en;
				((Control)lab_upgDesc).Text = upgDesc_en;
				((Control)lab_hardwareVer).Text = hardware_en;
				((Control)lab_firmwareVer).Text = firmware_en;
				((Control)lab_tempture).Text = tempture_en;
				((Control)lab_gmMode).Text = gmMode_en;
				((Control)lab_sensNum).Text = sensNum_en;
				((Control)lab_rollNum).Text = rollNum_en;
				((Control)lab_pitchNum).Text = pitchNum_en;
				((Control)lab_yawNum).Text = yawNum_en;
				((Control)lab_currGMMode).Text = currMode_en;
				((Control)lab_angProtect).Text = angProtect_en;
				break;
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

	private void ReloadSelItems()
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		Select[] array = (Select[])(object)new Select[3] { sel_M0, sel_M1, sel_M2 };
		string[] array2 = new string[4] { fpvMode_cn, pitchMode_cn, horiMode_cn, lookDownMode_cn };
		string[] array3 = new string[4] { fpvMode_en, pitchMode_en, horiMode_en, lookDownMode_en };
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Items.Clear();
			for (int j = 0; j < array2.Length; j++)
			{
				if (GD.Inst.CurrLang == 1)
				{
					array[i].Items.Add((object)array2[j]);
				}
				else
				{
					array[i].Items.Add((object)array3[j]);
				}
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
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Expected O, but got Unknown
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
			Button obj14 = btn_calib;
			Button obj15 = btn_WriteParam;
			Button obj16 = btn_openParam;
			Button obj17 = btn_saveParam;
			Label obj18 = lab_filepath;
			Label obj19 = lab_upgDesc;
			Button obj20 = btn_startUpg;
			Label obj21 = lab_currGMMode;
			Label obj22 = lab_gmMode;
			Label obj23 = lab_rollNum;
			Label obj24 = lab_pitchNum;
			Label obj25 = lab_yawNum;
			Select obj26 = sel_gmMoed;
			Input obj27 = inp_currMode;
			Input obj28 = inp_sensNum;
			Input obj29 = inp_rollNum;
			Input obj30 = inp_pitchNum;
			Input obj31 = inp_yawNum;
			Select obj32 = sel_portname;
			Button obj33 = btn_Disconnect;
			Button obj34 = btn_CloseGM;
			Label obj35 = lab_angProtect;
			Select obj36 = sel_M0;
			Select obj37 = sel_M1;
			Select obj38 = sel_M2;
			Label obj39 = label2;
			Label obj40 = label3;
			Font val2 = (((Control)label4).Font = val);
			Font val4 = (((Control)obj40).Font = val2);
			Font val6 = (((Control)obj39).Font = val4);
			Font val8 = (((Control)obj38).Font = val6);
			Font val10 = (((Control)obj37).Font = val8);
			Font val12 = (((Control)obj36).Font = val10);
			Font val14 = (((Control)obj35).Font = val12);
			Font val16 = (((Control)obj34).Font = val14);
			Font val18 = (((Control)obj33).Font = val16);
			Font val20 = (((Control)obj32).Font = val18);
			Font val22 = (((Control)obj31).Font = val20);
			Font val24 = (((Control)obj30).Font = val22);
			Font val26 = (((Control)obj29).Font = val24);
			Font val28 = (((Control)obj28).Font = val26);
			Font val30 = (((Control)obj27).Font = val28);
			Font val32 = (((Control)obj26).Font = val30);
			Font val34 = (((Control)obj25).Font = val32);
			Font val36 = (((Control)obj24).Font = val34);
			Font val38 = (((Control)obj23).Font = val36);
			Font val40 = (((Control)obj22).Font = val38);
			Font val42 = (((Control)obj21).Font = val40);
			Font val44 = (((Control)obj20).Font = val42);
			Font val46 = (((Control)obj19).Font = val44);
			Font val48 = (((Control)obj18).Font = val46);
			Font val50 = (((Control)obj17).Font = val48);
			Font val52 = (((Control)obj16).Font = val50);
			Font val54 = (((Control)obj15).Font = val52);
			Font val56 = (((Control)obj14).Font = val54);
			Font val58 = (((Control)obj13).Font = val56);
			Font val60 = (((Control)obj12).Font = val58);
			Font val62 = (((Control)obj11).Font = val60);
			Font val64 = (((Control)obj10).Font = val62);
			Font val66 = (((Control)obj9).Font = val64);
			Font val68 = (((Control)obj8).Font = val66);
			Font val70 = (((Control)obj7).Font = val68);
			Font val72 = (((Control)obj6).Font = val70);
			Font val74 = (((Control)obj5).Font = val72);
			Font val76 = (((Control)obj4).Font = val74);
			Font val78 = (((Control)obj3).Font = val76);
			Font font2 = (((Control)obj2).Font = val78);
			((Control)obj).Font = font2;
			Font val81 = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 13f);
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
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Expected O, but got Unknown
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Expected O, but got Unknown
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Expected O, but got Unknown
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Expected O, but got Unknown
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b6: Expected O, but got Unknown
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ce: Expected O, but got Unknown
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Expected O, but got Unknown
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b16: Expected O, but got Unknown
		//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c06: Expected O, but got Unknown
		//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
		//IL_105d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1110: Unknown result type (might be due to invalid IL or missing references)
		//IL_111a: Expected O, but got Unknown
		//IL_12f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ad: Expected O, but got Unknown
		//IL_1583: Unknown result type (might be due to invalid IL or missing references)
		//IL_1636: Unknown result type (might be due to invalid IL or missing references)
		//IL_1640: Expected O, but got Unknown
		//IL_1816: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d3: Expected O, but got Unknown
		//IL_1aa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b59: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b63: Expected O, but got Unknown
		//IL_1b99: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba3: Expected O, but got Unknown
		//IL_1bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c96: Expected O, but got Unknown
		//IL_1cea: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d89: Expected O, but got Unknown
		//IL_1ddd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e72: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e7c: Expected O, but got Unknown
		//IL_1ed0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f47: Expected O, but got Unknown
		//IL_1f7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f87: Expected O, but got Unknown
		//IL_1fd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_215a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2226: Unknown result type (might be due to invalid IL or missing references)
		//IL_2230: Expected O, but got Unknown
		//IL_2285: Unknown result type (might be due to invalid IL or missing references)
		//IL_2363: Unknown result type (might be due to invalid IL or missing references)
		//IL_236d: Expected O, but got Unknown
		//IL_2491: Unknown result type (might be due to invalid IL or missing references)
		//IL_249b: Expected O, but got Unknown
		//IL_24ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_28e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c74: Expected O, but got Unknown
		//IL_2d0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2db9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dc3: Expected O, but got Unknown
		//IL_2e04: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f36: Expected O, but got Unknown
		//IL_2fcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_307b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3085: Expected O, but got Unknown
		//IL_30c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_31ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_31f8: Expected O, but got Unknown
		//IL_328e: Unknown result type (might be due to invalid IL or missing references)
		//IL_333a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3344: Expected O, but got Unknown
		//IL_3382: Unknown result type (might be due to invalid IL or missing references)
		//IL_34cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_3571: Unknown result type (might be due to invalid IL or missing references)
		//IL_357b: Expected O, but got Unknown
		//IL_35bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_3648: Unknown result type (might be due to invalid IL or missing references)
		//IL_3652: Expected O, but got Unknown
		//IL_3693: Unknown result type (might be due to invalid IL or missing references)
		//IL_37e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_387c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3886: Expected O, but got Unknown
		//IL_3894: Unknown result type (might be due to invalid IL or missing references)
		//IL_389e: Expected O, but got Unknown
		//IL_3989: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a2f: Expected O, but got Unknown
		//IL_3a3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a47: Expected O, but got Unknown
		//IL_3a88: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a92: Expected O, but got Unknown
		//IL_3ad3: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c06: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ca2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cac: Expected O, but got Unknown
		//IL_3cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cc4: Expected O, but got Unknown
		//IL_3daf: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e55: Expected O, but got Unknown
		//IL_3e63: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e6d: Expected O, but got Unknown
		//IL_3eae: Unknown result type (might be due to invalid IL or missing references)
		//IL_3eb8: Expected O, but got Unknown
		//IL_3ef9: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f82: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f8c: Expected O, but got Unknown
		//IL_3fcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_4056: Unknown result type (might be due to invalid IL or missing references)
		//IL_4060: Expected O, but got Unknown
		//IL_409d: Unknown result type (might be due to invalid IL or missing references)
		//IL_41b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_41be: Expected O, but got Unknown
		//IL_4253: Unknown result type (might be due to invalid IL or missing references)
		//IL_4303: Unknown result type (might be due to invalid IL or missing references)
		//IL_430d: Expected O, but got Unknown
		//IL_4364: Unknown result type (might be due to invalid IL or missing references)
		//IL_436e: Expected O, but got Unknown
		//IL_43aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_4535: Unknown result type (might be due to invalid IL or missing references)
		//IL_45ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_4663: Unknown result type (might be due to invalid IL or missing references)
		//IL_466d: Expected O, but got Unknown
		//IL_46b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_4734: Unknown result type (might be due to invalid IL or missing references)
		//IL_473e: Expected O, but got Unknown
		//IL_477e: Unknown result type (might be due to invalid IL or missing references)
		//IL_47f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_4802: Expected O, but got Unknown
		//IL_48bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_4976: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ab2: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ad6: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b31: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b3b: Expected O, but got Unknown
		//IL_4b49: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b53: Expected O, but got Unknown
		//IL_4b89: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b93: Expected O, but got Unknown
		//IL_4bd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c82: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c8c: Expected O, but got Unknown
		//IL_4e15: Unknown result type (might be due to invalid IL or missing references)
		//IL_4eaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_4eb4: Expected O, but got Unknown
		//IL_4eea: Unknown result type (might be due to invalid IL or missing references)
		//IL_5073: Unknown result type (might be due to invalid IL or missing references)
		//IL_507d: Expected O, but got Unknown
		//IL_518b: Unknown result type (might be due to invalid IL or missing references)
		//IL_5195: Expected O, but got Unknown
		//IL_5211: Unknown result type (might be due to invalid IL or missing references)
		//IL_52d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_52e1: Expected O, but got Unknown
		//IL_5324: Unknown result type (might be due to invalid IL or missing references)
		//IL_53e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_549f: Unknown result type (might be due to invalid IL or missing references)
		//IL_5530: Unknown result type (might be due to invalid IL or missing references)
		//IL_553a: Expected O, but got Unknown
		//IL_557f: Unknown result type (might be due to invalid IL or missing references)
		//IL_5710: Unknown result type (might be due to invalid IL or missing references)
		//IL_571a: Expected O, but got Unknown
		//IL_575f: Unknown result type (might be due to invalid IL or missing references)
		//IL_5822: Unknown result type (might be due to invalid IL or missing references)
		//IL_582c: Expected O, but got Unknown
		//IL_5871: Unknown result type (might be due to invalid IL or missing references)
		//IL_5945: Unknown result type (might be due to invalid IL or missing references)
		//IL_594f: Expected O, but got Unknown
		//IL_5994: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a61: Expected O, but got Unknown
		//IL_5a9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5beb: Unknown result type (might be due to invalid IL or missing references)
		//IL_5c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_5c90: Unknown result type (might be due to invalid IL or missing references)
		grpan_gain = new GridPanel();
		inp_yawGain = new Input();
		inp_pitchGain = new Input();
		inp_rollGain = new Input();
		lab_yawGain = new Label();
		lab_pitchGain = new Label();
		lab_rollGain = new Label();
		grpan_chann = new GridPanel();
		sel_yawChan = new Select();
		sel_pitchChan = new Select();
		sel_rollChan = new Select();
		sel_senseChan = new Select();
		sel_modeChan = new Select();
		lab_yawChan = new Label();
		lab_pitchChan = new Label();
		lab_rollChan = new Label();
		lab_sensChan = new Label();
		lab_modeChan = new Label();
		grpan_param = new GridPanel();
		btn_stopGM = new Button();
		btn_calib = new Button();
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
		lab_filepath = new Label();
		pageHeader1 = new PageHeader();
		lab_angProtect = new Label();
		switch1 = new Switch();
		btn_Disconnect = new Button();
		btn_CloseGM = new Button();
		sel_portname = new Select();
		btn_refresh = new Button();
		pictureBox2 = new PictureBox();
		lab_firmwareVer = new Label();
		divider1 = new Divider();
		lab_tempture = new Label();
		lab_hardwareVer = new Label();
		lab_sn = new Label();
		lab_devName = new Label();
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
		((Control)uploadDragger1).SuspendLayout();
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
		((Control)grpan_chann).Controls.Add((Control)(object)sel_pitchChan);
		((Control)grpan_chann).Controls.Add((Control)(object)sel_rollChan);
		((Control)grpan_chann).Controls.Add((Control)(object)sel_senseChan);
		((Control)grpan_chann).Controls.Add((Control)(object)sel_modeChan);
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
		grpan_chann.Span = "20% 20% 20% 20% 20%;\r\n20% 20% 20% 20% 20%;";
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
		((Input)sel_yawChan).HandShortcutKeys = false;
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
		((Input)sel_pitchChan).HandShortcutKeys = false;
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
		((Input)sel_rollChan).HandShortcutKeys = false;
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
		((Input)sel_senseChan).HandShortcutKeys = false;
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
		((Input)sel_modeChan).BackColor = Color.Transparent;
		((Input)sel_modeChan).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_modeChan).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)sel_modeChan).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_modeChan).BorderWidth = 2f;
		((IControl)sel_modeChan).ColorScheme = (TAMode)2;
		((Control)sel_modeChan).Dock = (DockStyle)5;
		sel_modeChan.EnterDropDown = false;
		((Input)sel_modeChan).ForeColor = Color.White;
		((IControl)sel_modeChan).HandDragFolder = false;
		((Input)sel_modeChan).HandShortcutKeys = false;
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
		sel_modeChan.SelectedIndex = 0;
		sel_modeChan.SelectedValue = "NULL";
		((Control)sel_modeChan).Size = new Size(146, 30);
		((Control)sel_modeChan).TabIndex = 11;
		((Control)sel_modeChan).TabStop = false;
		((Control)sel_modeChan).Text = "NULL";
		((Input)sel_modeChan).WaveSize = 0;
		((Control)sel_modeChan).MouseClick += new MouseEventHandler(sel_modeChan_MouseClick);
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
		((ContainerPanel)grpan_param).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)grpan_param).BorderWidth = 2f;
		((Control)grpan_param).Controls.Add((Control)(object)btn_stopGM);
		((Control)grpan_param).Controls.Add((Control)(object)btn_calib);
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
		btn_stopGM.BackHover = Color.FromArgb(27, 28, 30);
		btn_stopGM.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_stopGM).Dock = (DockStyle)5;
		((Control)btn_stopGM).Font = new Font("Microsoft Sans Serif", 10.5f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_stopGM.ForeColor = Color.White;
		((IControl)btn_stopGM).HandDragFolder = false;
		grpan_param.SetIndex((Control)(object)btn_stopGM, 1);
		((Control)btn_stopGM).Location = new Point(139, 5);
		((Control)btn_stopGM).Margin = new Padding(5);
		((Control)btn_stopGM).Name = "btn_stopGM";
		((Control)btn_stopGM).Size = new Size(124, 30);
		((Control)btn_stopGM).TabIndex = 10;
		((Control)btn_stopGM).TabStop = false;
		((Control)btn_stopGM).Text = "停止云台";
		btn_stopGM.WaveSize = 0;
		((Control)btn_stopGM).Click += btn_stopGM_Click;
		btn_calib.BackHover = Color.FromArgb(27, 28, 30);
		btn_calib.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_calib).Dock = (DockStyle)5;
		((Control)btn_calib).Font = new Font("Microsoft Sans Serif", 10.5f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_calib.ForeColor = Color.White;
		((IControl)btn_calib).HandDragFolder = false;
		grpan_param.SetIndex((Control)(object)btn_calib, 2);
		((Control)btn_calib).Location = new Point(272, 3);
		((Control)btn_calib).Name = "btn_calib";
		((Control)btn_calib).Size = new Size(132, 34);
		((Control)btn_calib).TabIndex = 9;
		((Control)btn_calib).TabStop = false;
		((Control)btn_calib).Text = "陀螺仪校准";
		btn_calib.WaveSize = 0;
		((Control)btn_calib).Click += btn_calib_Click;
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
		grpan_upgrade.SetIndex((Control)(object)pageHeader2, 1);
		((Control)pageHeader2).Location = new Point(3, 3);
		pageHeader2.Mode = (TAMode)2;
		((Control)pageHeader2).Name = "pageHeader2";
		((Control)pageHeader2).Size = new Size(824, 121);
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
		((Input)sel_M2).HandShortcutKeys = false;
		sel_M2.Items.AddRange(new object[4] { "FPV-Mode", "Pitch-Stabilized-Mode", "Horizon-Mode", "LookDown-Mode" });
		sel_M2.List = true;
		((Control)sel_M2).Location = new Point(580, 79);
		((Control)sel_M2).Margin = new Padding(10, 5, 10, 5);
		sel_M2.MaxCount = 5;
		((Control)sel_M2).Name = "sel_M2";
		sel_M2.Placement = (TAlignFrom)31;
		((Control)sel_M2).Size = new Size(222, 30);
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
		((Input)sel_M1).HandShortcutKeys = false;
		sel_M1.Items.AddRange(new object[4] { "FPV-Mode", "Pitch-Stabilized-Mode", "Horizon-Mode", "LookDown-Mode" });
		sel_M1.List = true;
		((Control)sel_M1).Location = new Point(312, 79);
		((Control)sel_M1).Margin = new Padding(10, 5, 10, 5);
		sel_M1.MaxCount = 5;
		((Control)sel_M1).Name = "sel_M1";
		sel_M1.Placement = (TAlignFrom)31;
		((Control)sel_M1).Size = new Size(230, 30);
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
		((Input)sel_M0).HandShortcutKeys = false;
		sel_M0.Items.AddRange(new object[4] { "FPV-Mode", "Pitch-Stabilized-Mode", "Horizon-Mode", "LookDown-Mode" });
		sel_M0.List = true;
		((Control)sel_M0).Location = new Point(41, 79);
		((Control)sel_M0).Margin = new Padding(10, 5, 10, 5);
		sel_M0.MaxCount = 5;
		((Control)sel_M0).Name = "sel_M0";
		sel_M0.Placement = (TAlignFrom)31;
		((Control)sel_M0).Size = new Size(230, 30);
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
		((Control)inp_currMode).Location = new Point(146, 33);
		((Control)inp_currMode).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_currMode).Name = "inp_currMode";
		inp_currMode.ReadOnly = true;
		((Control)inp_currMode).Size = new Size(163, 30);
		((Control)inp_currMode).TabIndex = 24;
		((Control)inp_currMode).TabStop = false;
		((Control)inp_currMode).Text = "NULL";
		inp_currMode.WaveSize = 0;
		((Control)lab_currGMMode).BackColor = Color.Transparent;
		((Control)lab_currGMMode).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_currGMMode).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_currGMMode).Location = new Point(141, 10);
		((Control)lab_currGMMode).Margin = new Padding(10);
		((Control)lab_currGMMode).Name = "lab_currGMMode";
		((Control)lab_currGMMode).Size = new Size(168, 20);
		((Control)lab_currGMMode).TabIndex = 23;
		((Control)lab_currGMMode).Text = "Current gimbal mode";
		lab_currGMMode.TextAlign = (ContentAlignment)256;
		((Control)lab_yawNum).BackColor = Color.Transparent;
		((Control)lab_yawNum).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_yawNum).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_yawNum).Location = new Point(616, 10);
		((Control)lab_yawNum).Margin = new Padding(10);
		((Control)lab_yawNum).Name = "lab_yawNum";
		((Control)lab_yawNum).Size = new Size(86, 20);
		((Control)lab_yawNum).TabIndex = 22;
		((Control)lab_yawNum).Text = "指向角度设置";
		lab_yawNum.TextAlign = (ContentAlignment)256;
		inp_yawNum.BackColor = Color.FromArgb(33, 36, 39);
		inp_yawNum.BorderActive = Color.FromArgb(255, 233, 0);
		inp_yawNum.BorderColor = Color.FromArgb(66, 69, 71);
		inp_yawNum.BorderHover = Color.FromArgb(255, 233, 0);
		inp_yawNum.BorderWidth = 2f;
		inp_yawNum.ForeColor = Color.FromArgb(255, 255, 255);
		((IControl)inp_yawNum).HandDragFolder = false;
		inp_yawNum.HandShortcutKeys = false;
		((Control)inp_yawNum).Location = new Point(616, 33);
		((Control)inp_yawNum).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_yawNum).Name = "inp_yawNum";
		inp_yawNum.PlaceholderColor = Color.FromArgb(133, 133, 133);
		inp_yawNum.PlaceholderText = "±150";
		((Control)inp_yawNum).Size = new Size(86, 30);
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
		((Control)inp_pitchNum).Location = new Point(519, 33);
		((Control)inp_pitchNum).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_pitchNum).Name = "inp_pitchNum";
		inp_pitchNum.PlaceholderColor = Color.FromArgb(133, 133, 133);
		inp_pitchNum.PlaceholderText = "±90";
		((Control)inp_pitchNum).Size = new Size(86, 30);
		((Control)inp_pitchNum).TabIndex = 20;
		((Control)inp_pitchNum).TabStop = false;
		inp_pitchNum.WaveSize = 0;
		inp_pitchNum.VerifyChar += new InputVerifyCharEventHandler(inp_sensNum_VerifyChar);
		((Control)inp_pitchNum).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_pitchNum).Leave += inp_pitchNum_Leave;
		((Control)lab_pitchNum).BackColor = Color.Transparent;
		((Control)lab_pitchNum).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_pitchNum).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_pitchNum).Location = new Point(519, 10);
		((Control)lab_pitchNum).Margin = new Padding(10);
		((Control)lab_pitchNum).Name = "lab_pitchNum";
		((Control)lab_pitchNum).Size = new Size(93, 20);
		((Control)lab_pitchNum).TabIndex = 19;
		((Control)lab_pitchNum).Text = "俯仰角度设置";
		lab_pitchNum.TextAlign = (ContentAlignment)256;
		inp_rollNum.BackColor = Color.FromArgb(33, 36, 39);
		inp_rollNum.BorderActive = Color.FromArgb(255, 233, 0);
		inp_rollNum.BorderColor = Color.FromArgb(66, 69, 71);
		inp_rollNum.BorderHover = Color.FromArgb(255, 233, 0);
		inp_rollNum.BorderWidth = 2f;
		inp_rollNum.ForeColor = Color.FromArgb(255, 255, 255);
		((Control)inp_rollNum).Location = new Point(423, 33);
		((Control)inp_rollNum).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_rollNum).Name = "inp_rollNum";
		inp_rollNum.PlaceholderColor = Color.FromArgb(133, 133, 133);
		inp_rollNum.PlaceholderText = "±55";
		((Control)inp_rollNum).Size = new Size(86, 30);
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
		((Control)inp_sensNum).Location = new Point(319, 33);
		((Control)inp_sensNum).Margin = new Padding(5, 7, 5, 7);
		((Control)inp_sensNum).Name = "inp_sensNum";
		inp_sensNum.PlaceholderColor = Color.FromArgb(133, 133, 133);
		inp_sensNum.PlaceholderText = "±1";
		((Control)inp_sensNum).Size = new Size(86, 30);
		((Control)inp_sensNum).TabIndex = 17;
		((Control)inp_sensNum).TabStop = false;
		inp_sensNum.WaveSize = 0;
		inp_sensNum.VerifyChar += new InputVerifyCharEventHandler(inp_sensNum_VerifyChar);
		((Control)inp_sensNum).KeyDown += new KeyEventHandler(inp_Min_EnterDown);
		((Control)inp_sensNum).Leave += inp_sensNum_Leave;
		((Control)lab_rollNum).BackColor = Color.Transparent;
		((Control)lab_rollNum).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_rollNum).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_rollNum).Location = new Point(423, 10);
		((Control)lab_rollNum).Margin = new Padding(10);
		((Control)lab_rollNum).Name = "lab_rollNum";
		((Control)lab_rollNum).Size = new Size(96, 20);
		((Control)lab_rollNum).TabIndex = 16;
		((Control)lab_rollNum).Text = "滚转角度设置";
		lab_rollNum.TextAlign = (ContentAlignment)256;
		((Control)lab_sensNum).BackColor = Color.Transparent;
		((Control)lab_sensNum).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_sensNum).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_sensNum).Location = new Point(319, 10);
		((Control)lab_sensNum).Margin = new Padding(10);
		((Control)lab_sensNum).Name = "lab_sensNum";
		((Control)lab_sensNum).Size = new Size(86, 20);
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
		((Input)sel_gmMoed).HandShortcutKeys = false;
		sel_gmMoed.Items.AddRange(new object[4] { "NULL", "M0", "M1", "M2" });
		sel_gmMoed.List = true;
		((Control)sel_gmMoed).Location = new Point(7, 33);
		((Control)sel_gmMoed).Margin = new Padding(10, 5, 10, 5);
		sel_gmMoed.MaxCount = 5;
		((Control)sel_gmMoed).Name = "sel_gmMoed";
		sel_gmMoed.Placement = (TAlignFrom)31;
		sel_gmMoed.SelectedIndex = 0;
		sel_gmMoed.SelectedValue = "NULL";
		((Control)sel_gmMoed).Size = new Size(114, 30);
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
		((Control)btn_startUpg).Location = new Point(716, 15);
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
		((Control)stackPanel1).Controls.Add((Control)(object)lab_upgDesc);
		((Control)stackPanel1).Controls.Add((Control)(object)lab_versChange);
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
		((Control)uploadDragger1).Controls.Add((Control)(object)lab_filepath);
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
		((Control)uploadDragger1).Text = "";
		((IControl)uploadDragger1).DragChanged += new DragEventHandler(uploadDragger1_DragChanged);
		((Control)uploadDragger1).MouseClick += new MouseEventHandler(uploadDragger1_MouseClick);
		((Control)lab_filepath).BackColor = Color.Transparent;
		((Control)lab_filepath).Dock = (DockStyle)2;
		((Control)lab_filepath).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_filepath.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_filepath).Location = new Point(11, 20);
		((Control)lab_filepath).Margin = new Padding(0);
		((Control)lab_filepath).Name = "lab_filepath";
		((Control)lab_filepath).Size = new Size(798, 50);
		lab_filepath.Suffix = "12324";
		lab_filepath.SuffixColor = Color.FromArgb(255, 233, 0);
		((Control)lab_filepath).TabIndex = 38;
		((Control)lab_filepath).TabStop = false;
		((Control)lab_filepath).Text = "111";
		lab_filepath.TextAlign = (ContentAlignment)32;
		((Control)lab_filepath).MouseClick += new MouseEventHandler(uploadDragger1_MouseClick);
		((Control)pageHeader1).BackColor = Color.Transparent;
		((Control)pageHeader1).Controls.Add((Control)(object)lab_angProtect);
		((Control)pageHeader1).Controls.Add((Control)(object)switch1);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_Disconnect);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_CloseGM);
		((Control)pageHeader1).Controls.Add((Control)(object)sel_portname);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_refresh);
		((Control)pageHeader1).Controls.Add((Control)(object)pictureBox2);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_firmwareVer);
		((Control)pageHeader1).Controls.Add((Control)(object)divider1);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_tempture);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_hardwareVer);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_sn);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_devName);
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
		lab_angProtect.AutoSizeMode = (TAutoSize)1;
		((Control)lab_angProtect).BackColor = Color.Transparent;
		((Control)lab_angProtect).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		lab_angProtect.ForeColor = Color.White;
		((Control)lab_angProtect).Location = new Point(624, 85);
		((Control)lab_angProtect).Margin = new Padding(0);
		((Control)lab_angProtect).Name = "lab_angProtect";
		((Control)lab_angProtect).Size = new Size(77, 15);
		lab_angProtect.SuffixColor = Color.White;
		((Control)lab_angProtect).TabIndex = 43;
		((Control)lab_angProtect).TabStop = false;
		((Control)lab_angProtect).Text = "Angle protect";
		lab_angProtect.TextAlign = (ContentAlignment)32;
		lab_angProtect.TextMultiLine = false;
		((Control)switch1).BackColor = Color.FromArgb(38, 41, 43);
		switch1.Checked = true;
		switch1.Fill = Color.FromArgb(255, 233, 0);
		switch1.FillHover = Color.FromArgb(255, 255, 255);
		((IControl)switch1).HandDragFolder = false;
		((Control)switch1).Location = new Point(713, 80);
		((Control)switch1).Name = "switch1";
		((Control)switch1).Size = new Size(83, 23);
		((Control)switch1).TabIndex = 42;
		((Control)switch1).TabStop = false;
		switch1.UnCheckedText = "close";
		switch1.WaveSize = 0;
		switch1.CheckedChanged += new BoolEventHandler(switch1_CheckedChanged);
		btn_Disconnect.DefaultBack = Color.FromArgb(95, 95, 96);
		btn_Disconnect.ForeColor = Color.White;
		((Control)btn_Disconnect).Location = new Point(565, 13);
		((Control)btn_Disconnect).Name = "btn_Disconnect";
		((Control)btn_Disconnect).Size = new Size(100, 30);
		((Control)btn_Disconnect).TabIndex = 41;
		((Control)btn_Disconnect).Text = "Disconnect";
		btn_Disconnect.WaveSize = 0;
		((Control)btn_Disconnect).Click += btn_Disconnect_Click_1;
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
		btn_CloseGM.WaveSize = 0;
		((Control)btn_CloseGM).Click += btn_CloseGM_Click;
		((Input)sel_portname).BackColor = Color.FromArgb(30, 34, 37);
		((Input)sel_portname).BorderWidth = 0f;
		((IControl)sel_portname).ColorScheme = (TAMode)2;
		((Control)sel_portname).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_portname).ForeColor = Color.White;
		sel_portname.List = true;
		((Control)sel_portname).Location = new Point(417, 13);
		((Control)sel_portname).Margin = new Padding(5);
		((Control)sel_portname).Name = "sel_portname";
		((Control)sel_portname).Size = new Size(97, 30);
		((Control)sel_portname).TabIndex = 38;
		((Input)sel_portname).WaveSize = 0;
		btn_refresh.DisplayStyle = (TButtonDisplayStyle)2;
		btn_refresh.ForeColor = Color.White;
		btn_refresh.Ghost = true;
		btn_refresh.Icon = (Image)(object)Resources.refresh_yellow;
		btn_refresh.IconRatio = 1f;
		((Control)btn_refresh).Location = new Point(517, 13);
		((Control)btn_refresh).Margin = new Padding(0);
		((Control)btn_refresh).Name = "btn_refresh";
		((Control)btn_refresh).Size = new Size(42, 30);
		((Control)btn_refresh).TabIndex = 39;
		((Control)btn_refresh).Text = "Refresh";
		btn_refresh.WaveSize = 0;
		((Control)btn_refresh).Click += btn_refresh_Click;
		((Control)pictureBox2).BackColor = Color.Transparent;
		((Control)pictureBox2).Dock = (DockStyle)3;
		pictureBox2.Image = (Image)(object)Resources.Ascent_GT;
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
		((Control)lab_firmwareVer).Location = new Point(152, 85);
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
		((Control)lab_tempture).Location = new Point(417, 85);
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
		((Control)lab_hardwareVer).Location = new Point(152, 54);
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
		((Control)lab_sn).Location = new Point(417, 55);
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
		((Control)lab_devName).Location = new Point(152, 5);
		((Control)lab_devName).Margin = new Padding(15, 0, 0, 0);
		((Control)lab_devName).Name = "lab_devName";
		((Control)lab_devName).Size = new Size(183, 31);
		((Control)lab_devName).TabIndex = 1;
		((Control)lab_devName).TabStop = false;
		((Control)lab_devName).Text = "Ascent GT Pro";
		lab_devName.TextAlign = (ContentAlignment)32;
		lab_devName.TextMultiLine = false;
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
		((Control)uploadDragger1).ResumeLayout(false);
		((Control)pageHeader1).ResumeLayout(false);
		((Control)pageHeader1).PerformLayout();
		((ISupportInitialize)pictureBox2).EndInit();
		((Control)grpan_main).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
