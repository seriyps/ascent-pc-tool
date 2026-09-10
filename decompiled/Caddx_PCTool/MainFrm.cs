using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;
using Caddx_PCTool._00_view;

namespace Caddx_PCTool;

public class MainFrm : Window
{
	private RunMsgFrm _runMsgFrm;

	private AscentUpdataFrm _ascentFrm;

	private FindDeviceFrm _findDeviceFrm;

	private SoftwareSettingFrm _softwareSettingFrm;

	private ContactUsCtrl _contactUs;

	private MainHelpCenter _helpCtrl;

	private UserMaualPdfCtrl _pdfCtrl;

	private NewGimbalFrm _newGimbalFrm;

	private CamHubCtrl _camHubCtrl;

	private RCModeCtrl _rcModeCtrl;

	private ContactUS_EN _contactUS_EN;

	private SidebarCtrl _sidebarCtrl;

	private updateChannel _upChanCtrl;

	private FunctionSelectFrm _funSelectFrm;

	private TabPage page_upgrade;

	private TabPage page_set;

	private TabPage page_help;

	private TabPage page_camHub;

	private TabPage page_rcMode;

	private bool setcolor = false;

	private FormFloatButton flBtn = null;

	private int _stepPlus = 0;

	private CancellationTokenSource _ctsSPStatus;

	private AutoSizeFormClass asc = new AutoSizeFormClass();

	private Timer _topMostTimer;

	private CancellationTokenSource _spStatusCts;

	private string find_en = "Connect device";

	private string fw_en = "Firmware upgrade";

	private string set_en = "Settings";

	private string help_en = "Help center";

	private string find_cn = "连接设备";

	private string fw_cn = "升级固件";

	private string set_cn = "设置";

	private string help_cn = "帮助中心";

	private List<UsbDevInfo> _currUsbInfo = new List<UsbDevInfo>();

	private IContainer components = null;

	private PageHeader pageHeader1;

	private Tabs tabs1;

	private GridPanel gridPanel1;

	private StatusStrip statusStrip1;

	private ToolStripDropDownButton toolStripDropDownButton1;

	private ToolStripMenuItem setYawGainToolStripMenuItem;

	private ToolStripMenuItem setPitchGainToolStripMenuItem;

	private ToolStripMenuItem setRollGainToolStripMenuItem;

	private ToolStripDropDownButton toolStripDropDownButton2;

	private ToolStripMenuItem 遍历控件ToolStripMenuItem;

	private ToolStripDropDownButton toolStripDropDownButton3;

	private ToolStripMenuItem 打开联系我们ToolStripMenuItem;

	private ToolStripMenuItem 打开说明书ToolStripMenuItem;

	private ToolStripDropDownButton toolStripDropDownButton4;

	private ToolStripMenuItem 测试3ToolStripMenuItem;

	private ToolStripMenuItem 测试2ToolStripMenuItem;

	private ToolStripMenuItem 测试1ToolStripMenuItem;

	private ToolStripMenuItem 显示pageToolStripMenuItem;

	private ToolStripMenuItem 显示连接ToolStripMenuItem;

	private ToolStripMenuItem 显示升级ToolStripMenuItem;

	private ToolStripMenuItem 显示帮助ToolStripMenuItem;

	private ToolStripMenuItem 添加所有pageToolStripMenuItem;

	private ToolStripMenuItem 清空所有pageToolStripMenuItem;

	private ToolStripMenuItem 测试连接获取型号ToolStripMenuItem;

	private ToolStripMenuItem 停止持续搜索ToolStripMenuItem;

	private ToolStripMenuItem 开始持续搜索ToolStripMenuItem;

	private ToolStripMenuItem 打开云台界面ToolStripMenuItem;

	private ToolStripDropDownButton toolStripDropDownButton5;

	private ToolStripStatusLabel toolStripStatusLabel3;

	private ToolStripStatusLabel tssl_comStatus;

	private Button btn_close;

	private Button button3;

	private Button button2;

	private ToolStripMenuItem 版本过低提示ToolStripMenuItem;

	private ToolStripMenuItem 普通模式云台界面ToolStripMenuItem;

	private ToolStripMenuItem 打开弹窗ToolStripMenuItem;

	private ToolStripMenuItem 打开camhub界面ToolStripMenuItem;

	private ToolStripMenuItem 创建示例帧ToolStripMenuItem;

	private ToolStripMenuItem 打开联系我们英文ToolStripMenuItem;

	private ToolStripMenuItem 下载页面ToolStripMenuItem;

	private TabPage tabPage1;

	private TabPage tabPage2;

	private PictureBox pic_LeftTopLogo;

	private ToolStripMenuItem 开启更改文件数据包ToolStripMenuItem;

	private ToolStripDropDownButton toolStripDropDownButton6;

	private ToolStripMenuItem sendRebootCleanToolStripMenuItem;

	private ToolStripMenuItem sendRemoteUpgradeToolStripMenuItem;

	private ToolStripMenuItem sendSENDFILESTARTToolStripMenuItem;

	private ToolStripMenuItem 读img中的md5ToolStripMenuItem;

	private ToolStripMenuItem sendSENDFILEDATAToolStripMenuItem;

	private ToolStripMenuItem sendSENDFILEENDToolStripMenuItem;

	private ToolStripMenuItem sendUPGRADESTATUSToolStripMenuItem;

	private Panel panel1;

	private ToolStripMenuItem bbfreq页ToolStripMenuItem;

	private ToolStripMenuItem 功能选择ToolStripMenuItem;

	private ToolStripStatusLabel toolStripStatusLabel1;

	private ToolStripMenuItem 新流程发finddevToolStripMenuItem;

	private ToolStripMenuItem 新流程发rebootToolStripMenuItem;

	private ToolStripMenuItem 旧流程发finddevToolStripMenuItem;

	private ToolStripDropDownButton toolStripDropDownButton7;

	private ToolStripMenuItem 读imgToolStripMenuItem;

	public bool IsFindDeviceActive => _findDeviceFrm != null && !((Control)_findDeviceFrm).Disposing && !((Control)_findDeviceFrm).IsDisposed && (object)((Control)_findDeviceFrm).Parent == page_upgrade && tabs1.SelectedTab == page_upgrade;

	public MainFrm()
	{
		InitializeComponent();
		((Form)this).TopLevel = true;
		((Control)this).MinimumSize = new Size(1024, 720);
		((Control)this).MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
	}

	private void MainFrm_Load(object sender, EventArgs e)
	{
		((Control)this).BringToFront();
		((Form)this).TopMost = true;
		InitFrm();
		InitSysComponent();
		if (_ctsSPStatus == null)
		{
		}
		DeleteFilesByNameDate("Log//", DateTime.Now, "yyyy-MM-dd", ".");
		Program.SplashScreenSW.Stop();
		WriteLog.WriteLogFileToUI($"启动耗时(ms)={Program.SplashScreenSW.ElapsedMilliseconds},版本号={Program.SoftwareVersion}", Color.Black);
		ShowDebugFrm();
		ReLoadFont();
		ReloadLang();
		((ToolStripItem)toolStripStatusLabel1).Text = $"当前用户等级={GD.Inst.CurrUserLevel}";
	}

	private void TopMostTimerCallback(object state)
	{
		((BaseForm)this).Invoke((Action)delegate
		{
			((Form)this).Activate();
			((Form)this).TopMost = false;
			((Control)this).BringToFront();
		});
		WriteLog.WriteLogFileToUI("合适时间重置激活窗体", Color.Black);
	}

	private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Invalid comparison between I4 and Unknown
		try
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.ParentsLocation = ((Window)this).Location;
			string title;
			string desc;
			string btnok;
			string btnCan;
			if (GD.Inst.CurrLang == 1)
			{
				title = "提示";
				desc = "确定关闭软件";
				btnok = "确认";
				btnCan = "取消";
			}
			else
			{
				title = "Notification";
				desc = "Are you sure you want to close the software?";
				btnok = "Confirm";
				btnCan = "Cancel";
			}
			commModalFrm.SetAllTxt(title, desc, btnok, btnCan);
			((Form)commModalFrm).ShowDialog();
			if (1 != (int)commModalFrm.FrmResult)
			{
				((CancelEventArgs)(object)e).Cancel = true;
				return;
			}
			Program.WriteParam();
			_spStatusCts?.Cancel();
			_spStatusCts?.Dispose();
			_pdfCtrl?.Dispose();
			_ascentFrm?.Dispose();
			_findDeviceFrm?.Dispose();
			GD.Inst.UsbFSM?.Dispose();
			GD.Inst.UpgFSM?.Dispose();
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("MainFrm_FormClosing error,desc=" + ex.Message, Color.DarkRed);
			Environment.Exit(0);
		}
	}

	private void InitFrm()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		_sidebarCtrl = new SidebarCtrl();
		((Control)_sidebarCtrl).Dock = (DockStyle)5;
		((Control)_sidebarCtrl).Margin = new Padding(0);
		_sidebarCtrl.OnSidebarFrmEvnet += OnSidebarFrm;
		_sidebarCtrl.ReloadLang();
		panel1.Controls.Add((Control)(object)_sidebarCtrl);
		((Control)statusStrip1).Visible = false;
		((Control)gridPanel1).Dock = (DockStyle)5;
		page_upgrade = new TabPage
		{
			Name = "page_Upgrade",
			Dock = (DockStyle)5,
			TabStop = false,
			ReadOnly = true,
			BackColor = Color.FromArgb(3422525),
			ForeColor = Color.FromArgb(16777215)
		};
		page_camHub = new TabPage
		{
			Name = "page_camHub",
			Dock = (DockStyle)5,
			TabStop = false,
			Visible = false,
			Text = "Hub"
		};
		page_rcMode = new TabPage
		{
			Name = "page_rcMode",
			Dock = (DockStyle)5,
			TabStop = false,
			Visible = false,
			Text = "RC Mode",
			ReadOnly = true
		};
		page_help = new TabPage
		{
			Name = "page_help",
			Dock = (DockStyle)5,
			TabStop = false,
			Visible = false
		};
		page_set = new TabPage
		{
			Name = "page_set",
			Dock = (DockStyle)5,
			TabStop = false,
			Visible = false
		};
		((iCollection<TabPage>)(object)tabs1.Pages).Add(page_upgrade);
		((iCollection<TabPage>)(object)tabs1.Pages).Add(page_camHub);
		((iCollection<TabPage>)(object)tabs1.Pages).Add(page_rcMode);
		((iCollection<TabPage>)(object)tabs1.Pages).Add(page_set);
		((iCollection<TabPage>)(object)tabs1.Pages).Add(page_help);
		_runMsgFrm = new RunMsgFrm();
		WriteLog writeLog = new WriteLog(_runMsgFrm.richTextBox1, (Form)(object)this);
		_softwareSettingFrm = new SoftwareSettingFrm();
		_softwareSettingFrm.OnSWSetFrmEvnet += OnSWSetFrm;
		TabPageAddCtrl(page_set, (Control)(object)_softwareSettingFrm, isshow: false);
		_contactUs = new ContactUsCtrl();
		TabPageAddCtrl(page_help, (Control)(object)_contactUs, isshow: false);
		_helpCtrl = new MainHelpCenter();
		_helpCtrl.OnMainHelpCenterHappenEvent += OnMainHelpCenterHappen;
		TabPageAddCtrl(page_help, (Control)(object)_helpCtrl, isshow: false);
		_findDeviceFrm = new FindDeviceFrm();
		_findDeviceFrm.OnFindDeviceFrmConnectEvent += OnFindDeviceFrm;
		TabPageAddCtrl(page_upgrade, (Control)(object)_findDeviceFrm, isshow: false);
		GD.Inst.FindDeviceFrm = _findDeviceFrm;
		_funSelectFrm = new FunctionSelectFrm();
		FunctionSelectFrm funSelectFrm = _funSelectFrm;
		funSelectFrm.OnFunctionSelectFrmEvent = (Action<string, FuntionType, DevCardEventArgs>)Delegate.Combine(funSelectFrm.OnFunctionSelectFrmEvent, new Action<string, FuntionType, DevCardEventArgs>(OnFunctionSelectFrm));
		DispLogoStatus logoStatus = Program.LogoStatus;
		DispLogoStatus dispLogoStatus = logoStatus;
		if (dispLogoStatus <= DispLogoStatus.caddx_Industry)
		{
			((Control)this).Text = "CADDX PC Tool";
			((Form)this).Icon = Resources.caddx_logo;
			pic_LeftTopLogo.Image = (Image)(object)Resources.左上角logo;
		}
	}

	private void InitSysComponent()
	{
	}

	private void ReLoadFont()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		if (GD.Inst.TitlePFC == null)
		{
			GD.Inst.TitlePFC = new PrivateFontCollection();
			string text = Application.StartupPath + "\\font\\Medium.ttf";
			GD.Inst.TitlePFC.AddFontFile(text);
		}
		if (GD.Inst.TextPFC == null)
		{
			GD.Inst.TextPFC = new PrivateFontCollection();
			string text2 = Application.StartupPath + "\\font\\Regular.ttf";
			GD.Inst.TextPFC.AddFontFile(text2);
		}
		Font val = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 14f);
		Font font = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
		((Control)tabs1).Font = font;
		for (int i = 0; i < ((iCollection<TabPage>)(object)tabs1.Pages).Count; i++)
		{
			((Control)((iCollection<TabPage>)(object)tabs1.Pages)[i]).Font = font;
		}
	}

	private void ReloadLang()
	{
		((Control)page_upgrade).Text = Lang.T("mainfrm.connect_device");
		((Control)page_set).Text = Lang.T("mainfrm.settings");
		((Control)page_help).Text = Lang.T("mainfrm.help_center");
	}

	private void OnFunctionSelectFrm(string a1, FuntionType a2, DevCardEventArgs a3)
	{
		switch (a2)
		{
		case FuntionType.upgrade:
			if (a3.DeviceName.Contains("Ascent") || a3.DeviceName.Contains("YoHD"))
			{
				AscentUpgradeCtrlOpend(a3);
			}
			else if (a3.DeviceName.Contains("Optical Photon"))
			{
				AscentUpgradeCtrlOpend(a3);
			}
			break;
		case FuntionType.gmSet:
			GimbalUpgradeCtrlOpend(a3);
			break;
		case FuntionType.camhub:
			CamhubCtrlOpend();
			break;
		case FuntionType.rcMode:
			RCModeCtrlOpend();
			break;
		case FuntionType.bb_freq:
			BBFreqCtrlOpend(a3);
			break;
		}
	}

	private void OnSidebarFrm(object sender, SidebarFrmEventArgs e)
	{
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Invalid comparison between Unknown and I4
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Invalid comparison between Unknown and I4
		((Form)this).TopMost = false;
		switch (e.Text)
		{
		case "固件升级":
		case "Upgrade":
			if (_softwareSettingFrm != null)
			{
				_softwareSettingFrm.ReleaseCliSession();
			}
			tabs1.SelectTab(page_upgrade);
			((Control)page_upgrade).Show();
			break;
		case "软件设置":
		case "Settings":
			((Control)page_set).Show();
			tabs1.SelectTab(page_set);
			break;
		case "帮助中心":
		case "Help Center":
			((Control)page_help).Show();
			tabs1.SelectTab(page_help);
			break;
		case "联系客服":
		case "Customer Service":
			if (GD.Inst.CurrLang == 1)
			{
				_contactUs = new ContactUsCtrl();
				TabPageAddCtrl(page_help, (Control)(object)_contactUs);
			}
			else
			{
				_contactUS_EN = new ContactUS_EN();
				TabPageAddCtrl(page_help, (Control)(object)_contactUS_EN);
			}
			break;
		case "产品系列":
		case "Product Series":
			if (_helpCtrl == null)
			{
				_helpCtrl = new MainHelpCenter();
				_helpCtrl.OnMainHelpCenterHappenEvent += OnMainHelpCenterHappen;
			}
			TabPageAddCtrl(page_help, (Control)(object)_helpCtrl);
			break;
		case "Hub":
			if (_camHubCtrl != null)
			{
				((Component)(object)_camHubCtrl).Dispose();
				_camHubCtrl = null;
			}
			_camHubCtrl = new CamHubCtrl();
			((Control)page_camHub).Text = "Hub";
			TabPageAddCtrl(page_camHub, (Control)(object)_camHubCtrl);
			break;
		case "RC Mode":
			if (((Control)tabs1.SelectedTab).Name == "page_rcMode")
			{
				return;
			}
			if ((int)DisconnectDevice() != 1)
			{
				_sidebarCtrl.SelectedBtn("firmware");
				return;
			}
			if (_rcModeCtrl != null)
			{
				_rcModeCtrl.OnRCModeCtrlEvnet -= OnRCModeCtrl;
				_rcModeCtrl?.Dispose();
			}
			_rcModeCtrl = new RCModeCtrl(_currUsbInfo);
			_rcModeCtrl.OnRCModeCtrlEvnet += OnRCModeCtrl;
			((Control)page_rcMode).Text = "RC Mode";
			TabPageAddCtrl(page_rcMode, (Control)(object)_rcModeCtrl);
			break;
		}
		switch (e.Name)
		{
		case "lab_firmware":
			tabs1.SelectTab(page_upgrade);
			((Control)page_upgrade).Show();
			break;
		case "lab_setting":
			((Control)page_set).Show();
			tabs1.SelectTab(page_set);
			break;
		case "lab_HelpCenter":
			((Control)page_help).Show();
			tabs1.SelectTab(page_help);
			break;
		case "lab_customerService":
			if (GD.Inst.CurrLang == 1)
			{
				_contactUs = new ContactUsCtrl();
				TabPageAddCtrl(page_help, (Control)(object)_contactUs);
			}
			else
			{
				_contactUS_EN = new ContactUS_EN();
				TabPageAddCtrl(page_help, (Control)(object)_contactUS_EN);
			}
			break;
		case "lab_prodSer":
			if (_helpCtrl == null)
			{
				_helpCtrl = new MainHelpCenter();
				_helpCtrl.OnMainHelpCenterHappenEvent += OnMainHelpCenterHappen;
			}
			TabPageAddCtrl(page_help, (Control)(object)_helpCtrl);
			break;
		case "lab_vers":
			break;
		case "lab_Channel":
			if (((Control)tabs1.SelectedTab).Name == "page_Upgrade")
			{
				break;
			}
			if ((int)DisconnectDevice() != 1)
			{
				_sidebarCtrl.SelectedBtn("firmware");
				break;
			}
			if (_upChanCtrl != null)
			{
				_upChanCtrl.OnChannFrmHappentEvnet -= OnChannFrmHappen;
				((Component)(object)_upChanCtrl).Dispose();
			}
			_upChanCtrl = new updateChannel(_currUsbInfo);
			_upChanCtrl.OnChannFrmHappentEvnet += OnChannFrmHappen;
			TabPageAddCtrl(page_upgrade, (Control)(object)_upChanCtrl);
			((Control)page_upgrade).Text = "Update channel";
			break;
		}
	}

	private void OnChannFrmHappen(object sender, FrmEventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		InfoType infoType = e.InfoType;
		InfoType infoType2 = infoType;
		if (infoType2 != InfoType.ctrlSign)
		{
			return;
		}
		if (e.Desc == "UpdataChannConnect")
		{
			if ((int)DisconnectDevice() != 1)
			{
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt("Error", "Failed to disconnect current device. Please check the device connection and try again.", Color.Red);
				((Form)commModalFrm).ShowDialog();
			}
		}
		else if (e.Desc == "ManualCloseDevice")
		{
			((Control)page_rcMode).Visible = false;
			tabs1.SelectedIndex = 0;
			if (_findDeviceFrm != null)
			{
				_findDeviceFrm.OnFindDeviceFrmConnectEvent -= OnFindDeviceFrm;
				_findDeviceFrm.Dispose();
			}
			_findDeviceFrm = new FindDeviceFrm();
			_findDeviceFrm.OnFindDeviceFrmConnectEvent += OnFindDeviceFrm;
			TabPageAddCtrl(page_upgrade, (Control)(object)_findDeviceFrm, isshow: false);
			((Control)page_upgrade).Text = "Connect";
			_sidebarCtrl.SelectedBtn("firmware");
		}
	}

	private void OnFindDeviceFrm(object sender, DevCardEventArgs e)
	{
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		if (e.Desc == "CloseSplashScreen")
		{
			GD.Inst.IsSWFirstRun = false;
			SplashScreen.CloseForm();
		}
		else
		{
			if (!(e.Name == "connect"))
			{
				return;
			}
			GD.Inst.CurrDeviceName = e.DeviceName;
			GD.Inst.CurrPortName = e.PortName;
			string text = e.DeviceName ?? string.Empty;
			if (text.Contains("Ascent") || text.Contains("YoHD") || text.Contains("Optical Photon"))
			{
				FuntionType[] availableFunctions = GetAvailableFunctions(text, GD.Inst.CurrUserLevel);
				if (IsSingleFunction(availableFunctions) && availableFunctions[0] == FuntionType.upgrade)
				{
					AscentUpgradeCtrlOpend(e);
					return;
				}
				_funSelectFrm.SetDevCardEventArgs(e);
				_funSelectFrm.DispFunCard(availableFunctions.Length, availableFunctions);
				((Form)_funSelectFrm).ShowDialog();
			}
			else if (IsGimbalDevice(text))
			{
				FuntionType[] array = new FuntionType[1] { FuntionType.gmSet };
				if (IsSingleFunction(array))
				{
					GimbalUpgradeCtrlOpend(e);
					return;
				}
				_funSelectFrm.SetDevCardEventArgs(e);
				_funSelectFrm.DispFunCard(array.Length, array);
				((Form)_funSelectFrm).ShowDialog();
			}
			else if (text.Contains("OPV2"))
			{
				FuntionType[] array2 = new FuntionType[1] { FuntionType.upgrade };
				if (IsSingleFunction(array2) && array2[0] == FuntionType.upgrade)
				{
					AscentUpgradeCtrlOpend(e);
					return;
				}
				_funSelectFrm.SetDevCardEventArgs(e);
				_funSelectFrm.DispFunCard(array2.Length, array2);
				((Form)_funSelectFrm).ShowDialog();
			}
		}
	}

	private void FunctionSelectDisp(string devname, UserLevel level)
	{
		FuntionType[] availableFunctions = GetAvailableFunctions(devname, level);
		_funSelectFrm.DispFunCard(availableFunctions.Length, availableFunctions);
	}

	private FuntionType[] GetAvailableFunctions(string devname, UserLevel level)
	{
		string text = (devname ?? string.Empty).ToLower();
		switch (level)
		{
		case UserLevel.caddx:
			return new FuntionType[5]
			{
				FuntionType.bb_freq,
				FuntionType.rcMode,
				FuntionType.camhub,
				FuntionType.gmSet,
				FuntionType.upgrade
			};
		case UserLevel.customA:
			if (text.Contains("max") || text.Contains("new device") || text == "ascent vrx")
			{
				return new FuntionType[2]
				{
					FuntionType.bb_freq,
					FuntionType.upgrade
				};
			}
			return new FuntionType[1] { FuntionType.upgrade };
		case UserLevel.op:
			if (text.Contains("z8") || text.Contains("z40") || text.Contains("hub"))
			{
				return new FuntionType[3]
				{
					FuntionType.camhub,
					FuntionType.rcMode,
					FuntionType.upgrade
				};
			}
			return new FuntionType[1] { FuntionType.upgrade };
		default:
			return new FuntionType[1] { FuntionType.upgrade };
		}
	}

	private static bool IsGimbalDevice(string deviceName)
	{
		if (string.IsNullOrWhiteSpace(deviceName))
		{
			return false;
		}
		return deviceName.IndexOf("GM", StringComparison.OrdinalIgnoreCase) >= 0 || string.Equals(deviceName, "SimGM", StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsSingleFunction(FuntionType[] functions)
	{
		return functions != null && functions.Length == 1;
	}

	private void AscentUpgradeCtrlOpend(DevCardEventArgs e)
	{
		if (e.SessionHandle == null)
		{
			WriteLog.WriteLogFileToUI("升级会话不存在，串口名=" + e.PortName, Color.DarkRed);
			return;
		}
		SerialPortTransferResult serialPortTransferResult = GD.Inst.SerialPortSessions.TryTransfer(e.SessionHandle, SerialPortOwner.FirmwareUpgrade);
		if (!serialPortTransferResult.Succeeded)
		{
			WriteLog.WriteLogFileToUI(string.Format("交接升级会话失败，串口名={0}, reason={1}, owner={2}, state={3}", new object[4] { e.PortName, serialPortTransferResult.FailureReason, serialPortTransferResult.CurrentOwner, serialPortTransferResult.CurrentState }), Color.DarkRed);
			return;
		}
		GD.Inst.CurrSysMode = SysMode.ascentUpgrade;
		_findDeviceFrm?.Dispose();
		if (_ascentFrm != null)
		{
			_ascentFrm?.Dispose();
			_ascentFrm.OnAscentFrmEvnet -= OnAscentFrm;
		}
		_ascentFrm = new AscentUpdataFrm(e.AscentInfo);
		_ascentFrm.AttachUpgradeSession(serialPortTransferResult.Handle, e.AscentInfo);
		_ascentFrm.OnAscentFrmEvnet += OnAscentFrm;
		_ascentFrm.Frm = (Form)(object)this;
		_ascentFrm.DeviceName = e.DeviceName;
		_ascentFrm.PortName = serialPortTransferResult.Handle.PortName;
		TabPageAddCtrl(page_upgrade, (Control)(object)_ascentFrm);
		((Control)page_upgrade).Text = ((GD.Inst.CurrLang == 1) ? fw_cn : fw_en);
	}

	private async void GimbalUpgradeCtrlOpend(DevCardEventArgs e)
	{
		GD.Inst.CurrSysMode = SysMode.gimbalUpgrade;
		if (_findDeviceFrm != null)
		{
			await _findDeviceFrm.ReleaseDiscoverySessionByPortAsync(e.PortName);
			_findDeviceFrm.Dispose();
		}
		if (_newGimbalFrm != null)
		{
			_newGimbalFrm?.Dispose();
			_newGimbalFrm.OnGimFrmHappenEvnet -= OnGimFrmHappen;
		}
		_newGimbalFrm = new NewGimbalFrm(e.AscentInfo);
		_newGimbalFrm.OnGimFrmHappenEvnet += OnGimFrmHappen;
		((Control)page_upgrade).Text = ((GD.Inst.CurrLang == 1) ? "云台升级" : "GM set");
		TabPageAddCtrl(page_upgrade, (Control)(object)_newGimbalFrm);
	}

	private void CamhubCtrlOpend()
	{
		if (_camHubCtrl != null)
		{
			_camHubCtrl.OnCamHubCtrlEvnet -= OnCamHubCtrlEvnet;
			((Component)(object)_camHubCtrl).Dispose();
			_camHubCtrl = null;
		}
		_camHubCtrl = new CamHubCtrl();
		_camHubCtrl.OnCamHubCtrlEvnet += OnCamHubCtrlEvnet;
		((Control)page_camHub).Text = "Hub";
		TabPageAddCtrl(page_upgrade, (Control)(object)_camHubCtrl);
	}

	private void OnCamHubCtrlEvnet(object sender, FrmEventArgs e)
	{
		InfoType infoType = e.InfoType;
		InfoType infoType2 = infoType;
		if (infoType2 != InfoType.ctrlSign)
		{
			return;
		}
		if (_findDeviceFrm != null)
		{
			_findDeviceFrm.OnFindDeviceFrmConnectEvent -= OnFindDeviceFrm;
			_findDeviceFrm.Dispose();
		}
		_findDeviceFrm = new FindDeviceFrm();
		_findDeviceFrm.OnFindDeviceFrmConnectEvent += OnFindDeviceFrm;
		((Control)page_upgrade).Text = ((GD.Inst.CurrLang == 1) ? find_cn : find_en);
		TabPageAddCtrl(page_upgrade, (Control)(object)_findDeviceFrm);
		Task.Run(async delegate
		{
			await Task.Delay(100);
			((BaseForm)this).Invoke((Action)delegate
			{
				tabs1.SelectTab(page_upgrade);
				((Control)page_upgrade).Show();
			});
		});
	}

	private void RCModeCtrlOpend()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)DisconnectDevice() == 1)
		{
			if (_rcModeCtrl != null)
			{
				_rcModeCtrl.OnRCModeCtrlEvnet -= OnRCModeCtrl;
				_rcModeCtrl?.Dispose();
			}
			_rcModeCtrl = new RCModeCtrl(_currUsbInfo);
			_rcModeCtrl.OnRCModeCtrlEvnet += OnRCModeCtrl;
			((Control)page_rcMode).Text = "RC Mode";
			TabPageAddCtrl(page_upgrade, (Control)(object)_rcModeCtrl);
		}
	}

	private void BBFreqCtrlOpend(DevCardEventArgs e)
	{
		if (_upChanCtrl != null)
		{
			_upChanCtrl.OnChannFrmHappentEvnet -= OnChannFrmHappen;
			((Component)(object)_upChanCtrl).Dispose();
		}
		_upChanCtrl = new updateChannel(_currUsbInfo);
		_upChanCtrl.OnChannFrmHappentEvnet += OnChannFrmHappen;
		TabPageAddCtrl(page_upgrade, (Control)(object)_upChanCtrl);
		((Control)page_upgrade).Text = "Update channel";
	}

	private void OnAscentFrm(object sender, AscentFrmEventArgs e)
	{
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		switch (e.Desc)
		{
		case "NextStep":
			_stepPlus++;
			if (_stepPlus > 9)
			{
				_stepPlus = 9;
			}
			GD.Inst.UpgFSM.NextState((UpgState)_stepPlus);
			break;
		case "PrevStep":
			_stepPlus--;
			if (_stepPlus < 1)
			{
				_stepPlus = 1;
			}
			GD.Inst.UpgFSM.NextState((UpgState)_stepPlus);
			break;
		case "ClearStep":
			break;
		case "Connect":
			break;
		case "Disconnect":
		{
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "错误警告";
				desc = "设备已断开连接，检查设备的USB连接";
				string text = "确认";
			}
			else
			{
				title = "Error Warning";
				desc = "The device has been disconnected. Check the USB connection of the device";
				string text = "Confirm";
			}
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, Color.Red);
			((Form)commModalFrm).ShowDialog();
			_ascentFrm?.Dispose();
			((BaseForm)this).Invoke((Action)delegate
			{
				if (_findDeviceFrm != null)
				{
					_findDeviceFrm.Dispose();
					_findDeviceFrm.OnFindDeviceFrmConnectEvent -= OnFindDeviceFrm;
				}
				_findDeviceFrm = new FindDeviceFrm();
				_findDeviceFrm.OnFindDeviceFrmConnectEvent += OnFindDeviceFrm;
				((Control)page_upgrade).Text = ((GD.Inst.CurrLang == 1) ? find_cn : find_en);
				TabPageAddCtrl(page_upgrade, (Control)(object)_findDeviceFrm);
			});
			break;
		}
		case "ManualCloseDevice":
			_ascentFrm?.Dispose();
			if (_findDeviceFrm != null)
			{
				_findDeviceFrm.OnFindDeviceFrmConnectEvent -= OnFindDeviceFrm;
				_findDeviceFrm.Dispose();
			}
			_findDeviceFrm = new FindDeviceFrm();
			_findDeviceFrm.OnFindDeviceFrmConnectEvent += OnFindDeviceFrm;
			TabPageAddCtrl(page_upgrade, (Control)(object)_findDeviceFrm);
			((Control)page_upgrade).Text = ((GD.Inst.CurrLang == 1) ? find_cn : find_en);
			break;
		case "StartUpgrade":
			break;
		case "UpgradeComp":
			break;
		case "UpgradeFail":
			((IControl)tabs1).Enabled = true;
			break;
		}
	}

	private void OnGimFrmHappen(object sender, FrmEventArgs e)
	{
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		InfoType infoType = e.InfoType;
		InfoType infoType2 = infoType;
		if (infoType2 != InfoType.ctrlSign)
		{
			return;
		}
		if (e.Desc == "ManualCloseDevice")
		{
			if (_findDeviceFrm != null)
			{
				_findDeviceFrm.OnFindDeviceFrmConnectEvent -= OnFindDeviceFrm;
				_findDeviceFrm.Dispose();
			}
			_findDeviceFrm = new FindDeviceFrm();
			_findDeviceFrm.OnFindDeviceFrmConnectEvent += OnFindDeviceFrm;
			((Control)page_upgrade).Text = ((GD.Inst.CurrLang == 1) ? find_cn : find_en);
			TabPageAddCtrl(page_upgrade, (Control)(object)_findDeviceFrm);
			Task.Run(async delegate
			{
				await Task.Delay(100);
				((BaseForm)this).Invoke((Action)delegate
				{
					tabs1.SelectTab(page_upgrade);
					((Control)page_upgrade).Show();
				});
			});
		}
		else
		{
			if (!(e.Desc == "Disconnect"))
			{
				return;
			}
			string title;
			string desc;
			if (GD.Inst.CurrLang == 1)
			{
				title = "错误警告";
				desc = "设备已断开连接，检查设备的USB连接";
				string text = "确认";
			}
			else
			{
				title = "Error Warning";
				desc = "The device has been disconnected. Check the USB connection of the device";
				string text = "Confirm";
			}
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, Color.Red);
			((Form)commModalFrm).ShowDialog();
			((BaseForm)this).Invoke((Action)delegate
			{
				if (_findDeviceFrm != null)
				{
					_findDeviceFrm.Dispose();
					_findDeviceFrm.OnFindDeviceFrmConnectEvent -= OnFindDeviceFrm;
				}
				_findDeviceFrm = new FindDeviceFrm();
				_findDeviceFrm.OnFindDeviceFrmConnectEvent += OnFindDeviceFrm;
				((Control)page_upgrade).Text = ((GD.Inst.CurrLang == 1) ? find_cn : find_en);
				TabPageAddCtrl(page_upgrade, (Control)(object)_findDeviceFrm);
			});
		}
	}

	private void OnRCModeCtrl(object sender, FrmEventArgs e)
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		InfoType infoType = e.InfoType;
		InfoType infoType2 = infoType;
		if (infoType2 != InfoType.ctrlSign)
		{
			return;
		}
		if (e.Desc == "ManualCloseDevice")
		{
			((Control)page_rcMode).Visible = false;
			tabs1.SelectedIndex = 0;
			_findDeviceFrm = new FindDeviceFrm();
			_findDeviceFrm.OnFindDeviceFrmConnectEvent += OnFindDeviceFrm;
			TabPageAddCtrl(page_upgrade, (Control)(object)_findDeviceFrm, isshow: false);
			_sidebarCtrl.SelectedBtn("firmware");
		}
		else if (e.Desc == "Disconnect")
		{
			string title;
			string desc;
			string btnok;
			if (GD.Inst.CurrLang == 1)
			{
				title = "错误警告";
				desc = "设备已断开连接，检查设备的USB连接";
				btnok = "确认";
			}
			else
			{
				title = "Error Warning";
				desc = "The device has been disconnected. Check the USB connection of the device";
				btnok = "Confirm";
			}
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, btnok);
			((Form)commModalFrm).ShowDialog();
		}
	}

	private void OnSWSetFrm(object sender, HappenEventArgs e)
	{
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		try
		{
			EventType eventType = e.eventType;
			EventType eventType2 = eventType;
			if (eventType2 != EventType.log && eventType2 == EventType.frmSign)
			{
				GD.Inst.CurrLang = (byte)e.Index;
				GD.Inst.TitlePFC = new PrivateFontCollection();
				GD.Inst.TitlePFC = ((e.Index == 2) ? GD.Inst.Title_EN_USPFC : GD.Inst.Title_ZH_CNPFC);
				GD.Inst.TextPFC = new PrivateFontCollection();
				GD.Inst.TextPFC = ((e.Index == 2) ? GD.Inst.Text_EN_USPFC : GD.Inst.Text_ZH_CNPFC);
				ReloadLang();
				_newGimbalFrm?.ReloadLang();
				_ascentFrm?.ReloadLang();
				_findDeviceFrm?.ReloadLang();
				_helpCtrl?.ReloadLang();
				_pdfCtrl?.ReloadLang();
				_sidebarCtrl?.ReloadLang();
				_rcModeCtrl?.ReloadLang();
				_camHubCtrl?.ReloadLang();
				if (GD.Inst.CurrLang == 1)
				{
					_contactUs = new ContactUsCtrl();
					TabPageAddCtrl(page_help, (Control)(object)_contactUs, isshow: false);
				}
				else
				{
					_contactUS_EN = new ContactUS_EN();
					TabPageAddCtrl(page_help, (Control)(object)_contactUS_EN, isshow: false);
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void OnMainHelpCenterHappen(object sender, HappenEventArgs e)
	{
		if (_pdfCtrl == null)
		{
			_pdfCtrl = new UserMaualPdfCtrl(e.DevName);
			_pdfCtrl.OnPdfCtrlHappenEvent += OnPdfCtrlHappen;
		}
		TabPageAddCtrl(page_help, (Control)(object)_pdfCtrl);
		_pdfCtrl.DeviceName = e.DevName;
		_pdfCtrl.ReloadCtrl(e.DevName);
	}

	private void OnPdfCtrlHappen(object sender, HappenEventArgs e)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		try
		{
			EventType eventType = e.eventType;
			EventType eventType2 = eventType;
			if (eventType2 != EventType.log && eventType2 == EventType.frmSign && e.eventDesc.Contains("ClosePDF"))
			{
				if (_helpCtrl == null)
				{
					_helpCtrl = new MainHelpCenter();
					_helpCtrl.OnMainHelpCenterHappenEvent += OnMainHelpCenterHappen;
				}
				TabPageAddCtrl(page_help, (Control)(object)_helpCtrl);
			}
		}
		catch (Exception ex)
		{
			Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 14f);
			Message.error((Form)(object)this, ex.Message, val, (int?)0);
		}
	}

	private DialogResult DisconnectDevice()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			_findDeviceFrm.GetAllUsbDevInfo(out _currUsbInfo);
			_findDeviceFrm.ShowDevOccupy();
			Thread.Sleep(100);
			_findDeviceFrm.OnFindDeviceFrmConnectEvent -= OnFindDeviceFrm;
			_findDeviceFrm?.Dispose();
			return (DialogResult)1;
		}
		catch (Exception)
		{
			return (DialogResult)2;
		}
	}

	private void DeleteFilesByNameDate(string folderPath, DateTime deleteBeforeDate, string datePattern = "yyyyMMdd", string filePattern = "*.*")
	{
		try
		{
			if (!Directory.Exists(folderPath))
			{
				WriteLog.WriteLogFileToUI("文件夹不存在: " + folderPath, Color.DarkOrange);
				return;
			}
			string[] files = Directory.GetFiles(folderPath, filePattern);
			DateTime dateTime = deleteBeforeDate.AddDays(-5.0);
			string[] array = files;
			foreach (string path in array)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
				DateTime? dateTime2 = ExtractDateFromFileName(fileNameWithoutExtension, datePattern);
				bool flag = dateTime2.Value < deleteBeforeDate.AddDays(-5.0);
				if (dateTime2.HasValue & flag)
				{
					File.Delete(path);
				}
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("MainFrm.DeleteFilesByNameDate   error，Desc=" + ex.Message, Color.Red);
		}
	}

	private void MainFrm_SizeChanged(object sender, EventArgs e)
	{
	}

	private void tabs1_MouseDoubleClick(object sender, MouseEventArgs e)
	{
	}

	private DateTime? ExtractDateFromFileName(string fileName, string datePattern)
	{
		string dateRegexPattern = GetDateRegexPattern(datePattern);
		Match match = Regex.Match(fileName, dateRegexPattern);
		if (match.Success)
		{
			string value = match.Value;
			if (DateTime.TryParseExact(value, datePattern, null, DateTimeStyles.None, out var result))
			{
				return result;
			}
		}
		return null;
	}

	private void tabs1_SelectedIndexChanged(object sender, IntEventArgs e)
	{
	}

	private string GetDateRegexPattern(string datePattern)
	{
		return datePattern switch
		{
			"yyyyMMdd" => "\\d{8}", 
			"yyyy-MM-dd" => "\\d{4}-\\d{2}-\\d{2}", 
			"dd/MM/yyyy" => "\\d{2}/\\d{2}/\\d{4}", 
			"MM-dd-yyyy" => "\\d{2}-\\d{2}-\\d{4}", 
			_ => "\\d{4}-?\\d{2}-?\\d{2}|\\d{2}/?\\d{2}/?\\d{4}|\\d{2}-?\\d{2}-?\\d{4}", 
		};
	}

	private void setRollGainToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 打开产品说明ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		MainHelpCenter mainHelpCenter = new MainHelpCenter();
	}

	private void toolStripDropDownButton3_Click(object sender, EventArgs e)
	{
	}

	private void 打开说明书ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void setPitchGainToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 测试1ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 测试2ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 测试3ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ConcurrentDictionary<string, ResAscentInfo> concurrentDictionary = new ConcurrentDictionary<string, ResAscentInfo>();
		UsbDevInfo usbInfo = new UsbDevInfo
		{
			VID = "1234",
			PID = "5678",
			PortName = "COM17",
			ConnectedTime = DateTime.Now
		};
		ResAscentInfo value = new ResAscentInfo
		{
			SN = "1001",
			FWVers = "zxc",
			HWVers = "qwe-486-1.0",
			MCUTemp = 15,
			UsbInfo = usbInfo,
			UsbTime = DateTime.Now
		};
		concurrentDictionary.TryAdd("COM17", value);
		_findDeviceFrm.AddCtrl(concurrentDictionary);
	}

	private bool tabs1_ClosingPage(object sender, ClosingPageEventArgs e)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Invalid comparison between Unknown and I4
		TabCollection pages = tabs1.Pages;
		TabPage value = ((VEventArgs<TabPage>)(object)e).Value;
		Tabs val = (Tabs)sender;
		if (((Control)value).Name == ((Control)page_upgrade).Name)
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
			if ((int)commModalFrm.FrmResult != 1)
			{
				return false;
			}
		}
		int count = ((iCollection<TabPage>)(object)val.Pages).Count;
		((Control)value).Hide();
		return false;
	}

	private void 添加帮助pageToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (_softwareSettingFrm != null)
		{
			((Component)(object)_helpCtrl).Dispose();
			_helpCtrl.OnMainHelpCenterHappenEvent -= OnMainHelpCenterHappen;
		}
		_helpCtrl = new MainHelpCenter();
		_helpCtrl.OnMainHelpCenterHappenEvent += OnMainHelpCenterHappen;
	}

	private void 添加设置pageToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (_softwareSettingFrm != null)
		{
			((Component)(object)_softwareSettingFrm).Dispose();
			_softwareSettingFrm.OnSWSetFrmEvnet -= OnSWSetFrm;
		}
		_softwareSettingFrm = new SoftwareSettingFrm();
		_softwareSettingFrm.OnSWSetFrmEvnet += OnSWSetFrm;
	}

	private void 测试同名pageToolStripMenuItem_Click(object sender, EventArgs e)
	{
		foreach (TabPage item in (iCollection<TabPage>)(object)tabs1.Pages)
		{
			if (item != null)
			{
				TabPage val = item;
				if (((Control)val).Name == "page_upgrade")
				{
					tabs1.SelectedTab = val;
					break;
				}
			}
		}
	}

	private void 添加升级pageToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 隐藏升级pageToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 显示连接ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 显示升级ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 显示帮助ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 添加所有pageToolStripMenuItem_Click(object sender, EventArgs e)
	{
		((iCollection<TabPage>)(object)tabs1.Pages).Add(page_upgrade);
		((iCollection<TabPage>)(object)tabs1.Pages).Add(page_set);
		((iCollection<TabPage>)(object)tabs1.Pages).Add(page_help);
		tabs1.SelectedIndex = 1;
	}

	private void 清空所有pageToolStripMenuItem_Click(object sender, EventArgs e)
	{
		int num = 0;
		while (num < ((iCollection<TabPage>)(object)tabs1.Pages).Count)
		{
			((iCollection<TabPage>)(object)tabs1.Pages).Remove(((iCollection<TabPage>)(object)tabs1.Pages)[0]);
		}
	}

	private void MainFrm_Shown(object sender, EventArgs e)
	{
		_topMostTimer = new Timer(TopMostTimerCallback, null, TimeSpan.FromMilliseconds(1200.0), Timeout.InfiniteTimeSpan);
	}

	private void MainFrm_Activated_1(object sender, EventArgs e)
	{
	}

	private void 测试连接获取型号ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 开始持续搜索ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 停止持续搜索ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 打开云台界面ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void OnGimFrm(object sender, FrmEventArgs e)
	{
		throw new NotImplementedException();
	}

	private void 添加固件pageToolStripMenuItem_Click(object sender, EventArgs e)
	{
		int count = ((iCollection<TabPage>)(object)tabs1.Pages).Count;
		if (count > 0)
		{
			string name = ((Control)((iCollection<TabPage>)(object)tabs1.Pages)[0]).Name;
		}
		if (_findDeviceFrm != null)
		{
			_findDeviceFrm.Dispose();
			_findDeviceFrm.OnFindDeviceFrmConnectEvent -= OnFindDeviceFrm;
		}
		_findDeviceFrm = new FindDeviceFrm();
		_findDeviceFrm.OnFindDeviceFrmConnectEvent += OnFindDeviceFrm;
	}

	private void 获取usb信息ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void btn_close_Click(object sender, MouseEventArgs e)
	{
		((Form)this).TopMost = false;
		((Form)this).Close();
	}

	private void btn_Min_MouseClick(object sender, MouseEventArgs e)
	{
		((Form)this).TopMost = false;
		((BaseForm)this).Min();
	}

	private void btn_Max_Click(object sender, MouseEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		Button obj = (Button)sender;
		((BaseForm)this).MaxRestore();
		((BaseForm)this).Invoke((Action)delegate
		{
			obj.Icon = (Image)(object)(((BaseForm)this).IsMax ? Resources.缩小 : Resources.全屏);
		});
	}

	private void 版本过低提示ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ascentFrm.JudgeDeviceAndVers(out var _);
	}

	private void 普通模式云台界面ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 打开弹窗ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		CommModalFrm commModalFrm = new CommModalFrm();
		commModalFrm.SetAllTxt("测试弹窗标题", "这是一个测试弹窗的内容描述信息，用于展示弹窗功能是否正常工作。");
		((Form)commModalFrm).ShowDialog();
	}

	private void byte转asciiToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 打开camhub界面ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_camHubCtrl = new CamHubCtrl();
		TabPageAddCtrl(page_camHub, (Control)(object)_camHubCtrl);
	}

	private void 创建示例帧ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		List<byte> list = new List<byte>();
		list.Add(170);
		list.Add(85);
		list.Add(1);
		string s = "/factory/msp_rc_cfg.json";
		byte[] array = Encoding.ASCII.GetBytes(s);
		Array.Resize(ref array, 64);
		list.AddRange(array);
		string text = StaticMethod.BytesToHexString(list.ToArray());
		byte[] array2 = File.ReadAllBytes("pdf/msp_rc_cfg.json");
		byte[] bytes = BitConverter.GetBytes(array2.Length);
		list.AddRange(bytes);
		list.AddRange(array2);
		byte[] bytes2 = BitConverter.GetBytes(CRC32_Gimbal.Calculate(list.ToArray()));
		list.AddRange(bytes2);
		string text2 = StaticMethod.BytesToHexString(list.ToArray());
	}

	private void 打开联系我们英文ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ContactUS_EN ctrl = new ContactUS_EN();
		TabPageAddCtrl(page_help, (Control)(object)ctrl);
	}

	private void 下载页面ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 开启更改文件数据包ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string s = "b463abc26088765c";
		byte[] bytes = Encoding.ASCII.GetBytes(s);
		string text = StaticMethod.BytesToHexString(bytes);
		string text2 = text.Replace(" ", "");
	}

	private void toolStripDropDownButton6_Click(object sender, EventArgs e)
	{
		if (((ToolStripItem)toolStripDropDownButton6).Text == "关闭Asce升级")
		{
			GD.Inst.IsOpenAscentUpg = true;
			((ToolStripItem)toolStripDropDownButton6).BackColor = Color.Green;
			((ToolStripItem)toolStripDropDownButton6).Text = "打开Asce升级";
		}
		else
		{
			GD.Inst.IsOpenAscentUpg = false;
			((ToolStripItem)toolStripDropDownButton6).BackColor = Color.Red;
			((ToolStripItem)toolStripDropDownButton6).Text = "关闭Asce升级";
		}
	}

	private void sendRebootCleanToolStripMenuItem_Click(object sender, EventArgs e)
	{
		GD.Inst.Upg_Gim?.Send_RebootClean();
	}

	private void sendRemoteUpgradeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		GD.Inst.Upg_Gim?.Send_RemoteUpgrade();
	}

	private void sendSENDFILESTARTToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		OpenFileDialog val = new OpenFileDialog();
		try
		{
			((FileDialog)val).Title = "Open";
			((FileDialog)val).InitialDirectory = "D:";
			((FileDialog)val).Filter = "firmware (*.bin)|*.bin|All (*.*)|*.*";
			((FileDialog)val).FilterIndex = 1;
			((FileDialog)val).RestoreDirectory = true;
			val.Multiselect = false;
			if ((int)((CommonDialog)val).ShowDialog() == 1)
			{
				string fileName = ((FileDialog)val).FileName;
				long filesize = 0L;
				GD.Inst.Upg_Gim?.SetFile(fileName, out filesize);
				WriteLog.WriteLogFileToUI($"Selected file={fileName}, Size={filesize} bytes", Color.Black);
				GD.Inst.Upg_Gim?.Send_SENDFILE_START();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void 读img中的md5ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string text = "D:\\1\\vrx_pro+max+cine\\Ascent_VRX_Pro_17_5_3.img";
		byte[] array = File.ReadAllBytes(text);
		long num = array.LongLength;
		byte[] array2 = new byte[40];
		byte b = array[num - 1];
		byte b2 = array[num - 2];
		Array.Copy(array, num - 40, array2, 0L, 40L);
		string expectedMd = Encoding.ASCII.GetString(array2);
		bool flag = CompareMd5(text, expectedMd, 0L, 32L);
		string expectedMd2 = ComputeMd5WithoutLast32Bytes(text);
		bool flag2 = CompareMd5WithoutLast32Bytes(text, expectedMd2);
		string text2 = FileHashHelper.ReadLast32Bytes(text);
		WriteLog.WriteLogFileToUI("tail=" + text2, Color.Black);
		string text3 = FileHashHelper.ComputeMd5WithoutLast32Bytes(text);
		WriteLog.WriteLogFileToUI("md5withoutlast32bytes=" + text3, Color.Black);
		if (string.Equals(text2.TrimEnd(new char[1]), text3, StringComparison.OrdinalIgnoreCase))
		{
			Console.WriteLine("校验通过");
		}
		else
		{
			Console.WriteLine("校验失败");
		}
	}

	private void setYawGainToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 遍历控件ToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void 打开联系我们ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ContactUsCtrl contactUsCtrl = new ContactUsCtrl();
	}

	private void sendSENDFILEDATAToolStripMenuItem_Click(object sender, EventArgs e)
	{
		GD.Inst.Upg_Gim?.Send_SENDFILE_DATA();
	}

	private void sendSENDFILEENDToolStripMenuItem_Click(object sender, EventArgs e)
	{
		GD.Inst.Upg_Gim?.Send_SENDFILE_END();
	}

	private void sendUPGRADESTATUSToolStripMenuItem_Click(object sender, EventArgs e)
	{
		GD.Inst.Upg_Gim?.Send_UPGRADE_STATUS();
	}

	private void bbfreq页ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		updateChannel ctrl = new updateChannel(_currUsbInfo);
		TabPageAddCtrl(page_upgrade, (Control)(object)ctrl);
	}

	private void 功能选择ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		FunctionSelectFrm functionSelectFrm = new FunctionSelectFrm();
		functionSelectFrm.OnFunctionSelectFrmEvent = (Action<string, FuntionType, DevCardEventArgs>)Delegate.Combine(functionSelectFrm.OnFunctionSelectFrmEvent, new Action<string, FuntionType, DevCardEventArgs>(OnFunctionSelectFrm));
		functionSelectFrm.DispFunCard(1, new FuntionType[1] { FuntionType.upgrade });
		((Form)functionSelectFrm).ShowDialog();
	}

	private void 新流程发finddevToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ascentFrm.NewProtocolSendFinddev();
	}

	private void 新流程发rebootToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ascentFrm.NewProtocolSendRebootClean();
	}

	private void 旧流程发finddevToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ascentFrm.OldProtocolSendFinddev();
	}

	private void 读imgToolStripMenuItem_Click(object sender, EventArgs e)
	{
		byte[] array = new byte[64];
		using FileStream fileStream = new FileStream("D:\\1\\485\\Ascent_G_Gnd_17_2_1.img", FileMode.Open, FileAccess.Read);
		int num = fileStream.Read(array, 0, 64);
		int num2 = BitConverter.ToInt32(array, 8);
		int num3 = BitConverter.ToInt32(array, 12);
		int num4 = BitConverter.ToInt32(array, 16);
		WriteLog.WriteLogFileToUI($"img中的版本号={num2}_{num3}_{num4}", Color.Black);
	}

	private void TabPageAddCtrl(TabPage tap, Control ctrl, bool isshow = true)
	{
		((BaseForm)this).Invoke((Action)delegate
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			((Control)tap).Controls.Clear();
			ctrl.Margin = new Padding(5);
			((Control)tap).Controls.Add(ctrl);
			ctrl.Dock = (DockStyle)5;
			if (isshow)
			{
				((Control)tap).Visible = true;
				tabs1.SelectTab(tap);
				ctrl.Show();
			}
		});
	}

	private void ShowDebugFrm()
	{
		((Control)statusStrip1).Visible = false;
		((Control)gridPanel1).Dock = (DockStyle)5;
		TabPage obj = page_set;
		bool visible = (((Control)page_help).Visible = false);
		((Control)obj).Visible = visible;
	}

	private void GetSPStatus(CancellationToken cts)
	{
		try
		{
			while (!cts.IsCancellationRequested)
			{
				((BaseForm)this).Invoke((Action)delegate
				{
					if (GD.Inst.UsbFSM != null && GD.Inst.UsbFSM.SPobj != null)
					{
						((ToolStripItem)tssl_comStatus).Text = (GD.Inst.UsbFSM.IsComOpened ? "已连接" : "断开");
						((ToolStripItem)tssl_comStatus).BackColor = (GD.Inst.UsbFSM.IsComOpened ? Color.Green : Color.Red);
					}
					else
					{
						((ToolStripItem)tssl_comStatus).Text = "断开";
						((ToolStripItem)tssl_comStatus).BackColor = Color.Red;
					}
				});
			}
		}
		catch (Exception)
		{
		}
	}

	public static bool CompareMd5(string filePath, string expectedMd5, long offset, long checkSize)
	{
		if (string.IsNullOrWhiteSpace(expectedMd5))
		{
			throw new ArgumentException("expectedMd5 不能为空", "expectedMd5");
		}
		expectedMd5 = expectedMd5.Trim().ToLowerInvariant();
		try
		{
			using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			if (offset < 0 || offset >= fileStream.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "偏移量超出文件范围");
			}
			if (checkSize < 0 || offset + checkSize > fileStream.Length)
			{
				throw new ArgumentOutOfRangeException("checkSize", "校验长度超出文件范围");
			}
			fileStream.Seek(offset, SeekOrigin.Begin);
			byte[] buffer = new byte[checkSize];
			int i;
			int num;
			for (i = 0; i < checkSize; i += num)
			{
				num = fileStream.Read(buffer, i, (int)(checkSize - i));
				if (num == 0)
				{
					break;
				}
			}
			if (i != checkSize)
			{
				throw new IOException($"未能读取足够的字节：预期 {checkSize}，实际 {i}");
			}
			using MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(buffer, 0, i);
			string a = BitConverter.ToString(array).Replace("-", "").ToLowerInvariant();
			return string.Equals(a, expectedMd5, StringComparison.OrdinalIgnoreCase);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("MD5Check failed for " + filePath + ": " + ex.Message);
			return false;
		}
	}

	public static string ComputeMd5WithoutLast32Bytes(string filePath)
	{
		using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		if (fileStream.Length < 32)
		{
			throw new IOException("文件长度不足 32 字节");
		}
		long num = fileStream.Length - 32;
		byte[] buffer = new byte[num];
		int i;
		int num2;
		for (i = 0; i < num; i += num2)
		{
			num2 = fileStream.Read(buffer, i, (int)(num - i));
			if (num2 == 0)
			{
				break;
			}
		}
		using MD5 mD = MD5.Create();
		byte[] array = mD.ComputeHash(buffer, 0, i);
		string text = Encoding.ASCII.GetString(array);
		return BitConverter.ToString(array).Replace("-", "").ToLowerInvariant();
	}

	public static bool CompareMd5WithoutLast32Bytes(string filePath, string expectedMd5)
	{
		string a = ComputeMd5WithoutLast32Bytes(filePath);
		return string.Equals(a, expectedMd5, StringComparison.OrdinalIgnoreCase);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Window)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
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
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Expected O, but got Unknown
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Expected O, but got Unknown
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Expected O, but got Unknown
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Expected O, but got Unknown
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Expected O, but got Unknown
		//IL_0ca2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cac: Expected O, but got Unknown
		//IL_0df9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e03: Expected O, but got Unknown
		//IL_0ef7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f01: Expected O, but got Unknown
		//IL_1242: Unknown result type (might be due to invalid IL or missing references)
		//IL_124c: Expected O, but got Unknown
		//IL_1654: Unknown result type (might be due to invalid IL or missing references)
		//IL_165e: Expected O, but got Unknown
		//IL_1966: Unknown result type (might be due to invalid IL or missing references)
		//IL_1970: Expected O, but got Unknown
		//IL_1c1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c27: Expected O, but got Unknown
		//IL_1cab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cde: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e74: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eea: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ef4: Expected O, but got Unknown
		//IL_1f5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fdc: Expected O, but got Unknown
		//IL_2044: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c4: Expected O, but got Unknown
		//IL_215d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2167: Expected O, but got Unknown
		//IL_2194: Unknown result type (might be due to invalid IL or missing references)
		//IL_21bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_21c5: Expected O, but got Unknown
		StyleCard2 val = new StyleCard2();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainFrm));
		tabs1 = new Tabs();
		gridPanel1 = new GridPanel();
		panel1 = new Panel();
		statusStrip1 = new StatusStrip();
		toolStripDropDownButton1 = new ToolStripDropDownButton();
		setYawGainToolStripMenuItem = new ToolStripMenuItem();
		setPitchGainToolStripMenuItem = new ToolStripMenuItem();
		setRollGainToolStripMenuItem = new ToolStripMenuItem();
		遍历控件ToolStripMenuItem = new ToolStripMenuItem();
		打开云台界面ToolStripMenuItem = new ToolStripMenuItem();
		普通模式云台界面ToolStripMenuItem = new ToolStripMenuItem();
		sendRebootCleanToolStripMenuItem = new ToolStripMenuItem();
		sendRemoteUpgradeToolStripMenuItem = new ToolStripMenuItem();
		sendSENDFILESTARTToolStripMenuItem = new ToolStripMenuItem();
		sendSENDFILEDATAToolStripMenuItem = new ToolStripMenuItem();
		sendSENDFILEENDToolStripMenuItem = new ToolStripMenuItem();
		sendUPGRADESTATUSToolStripMenuItem = new ToolStripMenuItem();
		toolStripDropDownButton5 = new ToolStripDropDownButton();
		开启更改文件数据包ToolStripMenuItem = new ToolStripMenuItem();
		读img中的md5ToolStripMenuItem = new ToolStripMenuItem();
		toolStripDropDownButton7 = new ToolStripDropDownButton();
		读imgToolStripMenuItem = new ToolStripMenuItem();
		toolStripDropDownButton3 = new ToolStripDropDownButton();
		打开联系我们ToolStripMenuItem = new ToolStripMenuItem();
		打开说明书ToolStripMenuItem = new ToolStripMenuItem();
		显示pageToolStripMenuItem = new ToolStripMenuItem();
		显示连接ToolStripMenuItem = new ToolStripMenuItem();
		显示升级ToolStripMenuItem = new ToolStripMenuItem();
		显示帮助ToolStripMenuItem = new ToolStripMenuItem();
		创建示例帧ToolStripMenuItem = new ToolStripMenuItem();
		toolStripDropDownButton4 = new ToolStripDropDownButton();
		测试3ToolStripMenuItem = new ToolStripMenuItem();
		测试2ToolStripMenuItem = new ToolStripMenuItem();
		测试1ToolStripMenuItem = new ToolStripMenuItem();
		停止持续搜索ToolStripMenuItem = new ToolStripMenuItem();
		开始持续搜索ToolStripMenuItem = new ToolStripMenuItem();
		测试连接获取型号ToolStripMenuItem = new ToolStripMenuItem();
		版本过低提示ToolStripMenuItem = new ToolStripMenuItem();
		新流程发finddevToolStripMenuItem = new ToolStripMenuItem();
		新流程发rebootToolStripMenuItem = new ToolStripMenuItem();
		旧流程发finddevToolStripMenuItem = new ToolStripMenuItem();
		toolStripDropDownButton2 = new ToolStripDropDownButton();
		清空所有pageToolStripMenuItem = new ToolStripMenuItem();
		添加所有pageToolStripMenuItem = new ToolStripMenuItem();
		打开弹窗ToolStripMenuItem = new ToolStripMenuItem();
		打开camhub界面ToolStripMenuItem = new ToolStripMenuItem();
		打开联系我们英文ToolStripMenuItem = new ToolStripMenuItem();
		下载页面ToolStripMenuItem = new ToolStripMenuItem();
		bbfreq页ToolStripMenuItem = new ToolStripMenuItem();
		功能选择ToolStripMenuItem = new ToolStripMenuItem();
		toolStripDropDownButton6 = new ToolStripDropDownButton();
		toolStripStatusLabel3 = new ToolStripStatusLabel();
		toolStripStatusLabel1 = new ToolStripStatusLabel();
		tssl_comStatus = new ToolStripStatusLabel();
		tabPage1 = new TabPage();
		tabPage2 = new TabPage();
		pageHeader1 = new PageHeader();
		pic_LeftTopLogo = new PictureBox();
		button3 = new Button();
		button2 = new Button();
		btn_close = new Button();
		((Control)gridPanel1).SuspendLayout();
		((Control)statusStrip1).SuspendLayout();
		((Control)pageHeader1).SuspendLayout();
		((ISupportInitialize)pic_LeftTopLogo).BeginInit();
		((Control)this).SuspendLayout();
		((Control)tabs1).BackColor = Color.FromArgb(32, 33, 37);
		((IControl)tabs1).ColorScheme = (TAMode)2;
		((Control)tabs1).Cursor = Cursors.Hand;
		((Control)tabs1).Dock = (DockStyle)5;
		tabs1.EnablePageScrolling = false;
		((Control)tabs1).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		tabs1.ForeColor = Color.White;
		tabs1.Gap = 10;
		((IControl)tabs1).HandCursor = Cursors.Default;
		((IControl)tabs1).HandDragFolder = false;
		gridPanel1.SetIndex((Control)(object)tabs1, 2);
		((Control)tabs1).Location = new Point(172, 0);
		((Control)tabs1).Margin = new Padding(0);
		((Control)tabs1).Name = "tabs1";
		((Control)tabs1).Padding = new Padding(0, 0, 10, 10);
		((Control)tabs1).Size = new Size(842, 680);
		val.Closable = (CloseType)1;
		tabs1.Style = (IStyle)(object)val;
		((Control)tabs1).TabIndex = 6;
		((Control)tabs1).TabStop = false;
		tabs1.Type = (TabType)2;
		tabs1.TypExceed = (TabTypExceed)2;
		tabs1.SelectedIndexChanged += new IntEventHandler(tabs1_SelectedIndexChanged);
		tabs1.ClosingPage += new ClosingPageEventHandler(tabs1_ClosingPage);
		((Control)tabs1).MouseDoubleClick += new MouseEventHandler(tabs1_MouseDoubleClick);
		((ContainerPanel)gridPanel1).Back = Color.FromArgb(32, 33, 37);
		((Control)gridPanel1).BackColor = Color.FromArgb(32, 33, 37);
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(255, 255, 255);
		((Control)gridPanel1).Controls.Add((Control)(object)panel1);
		((Control)gridPanel1).Controls.Add((Control)(object)tabs1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((IControl)gridPanel1).HandDragFolder = false;
		((Control)gridPanel1).Location = new Point(5, 35);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(1014, 680);
		gridPanel1.Span = "17% 83%;";
		((Control)gridPanel1).TabIndex = 7;
		((Control)gridPanel1).TabStop = false;
		((Control)gridPanel1).Text = "gridPanel1";
		panel1.Back = Color.FromArgb(32, 33, 37);
		((Control)panel1).BackColor = Color.FromArgb(32, 33, 37);
		((Control)panel1).Dock = (DockStyle)5;
		gridPanel1.SetIndex((Control)(object)panel1, 1);
		((Control)panel1).Location = new Point(0, 0);
		((Control)panel1).Margin = new Padding(0);
		((Control)panel1).Name = "panel1";
		((Control)panel1).Size = new Size(172, 680);
		((Control)panel1).TabIndex = 7;
		((Control)panel1).Text = "panel1";
		((Control)statusStrip1).AutoSize = false;
		((ToolStrip)statusStrip1).Items.AddRange((ToolStripItem[])(object)new ToolStripItem[10]
		{
			(ToolStripItem)toolStripDropDownButton1,
			(ToolStripItem)toolStripDropDownButton5,
			(ToolStripItem)toolStripDropDownButton7,
			(ToolStripItem)toolStripDropDownButton3,
			(ToolStripItem)toolStripDropDownButton4,
			(ToolStripItem)toolStripDropDownButton2,
			(ToolStripItem)toolStripDropDownButton6,
			(ToolStripItem)toolStripStatusLabel3,
			(ToolStripItem)toolStripStatusLabel1,
			(ToolStripItem)tssl_comStatus
		});
		((Control)statusStrip1).Location = new Point(5, 683);
		((Control)statusStrip1).Name = "statusStrip1";
		((Control)statusStrip1).Size = new Size(1014, 32);
		((Control)statusStrip1).TabIndex = 0;
		((Control)statusStrip1).Text = "statusStrip1";
		((Control)statusStrip1).Visible = false;
		((ToolStripItem)toolStripDropDownButton1).DisplayStyle = (ToolStripItemDisplayStyle)1;
		((ToolStripDropDownItem)toolStripDropDownButton1).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[12]
		{
			(ToolStripItem)setYawGainToolStripMenuItem,
			(ToolStripItem)setPitchGainToolStripMenuItem,
			(ToolStripItem)setRollGainToolStripMenuItem,
			(ToolStripItem)遍历控件ToolStripMenuItem,
			(ToolStripItem)打开云台界面ToolStripMenuItem,
			(ToolStripItem)普通模式云台界面ToolStripMenuItem,
			(ToolStripItem)sendRebootCleanToolStripMenuItem,
			(ToolStripItem)sendRemoteUpgradeToolStripMenuItem,
			(ToolStripItem)sendSENDFILESTARTToolStripMenuItem,
			(ToolStripItem)sendSENDFILEDATAToolStripMenuItem,
			(ToolStripItem)sendSENDFILEENDToolStripMenuItem,
			(ToolStripItem)sendUPGRADESTATUSToolStripMenuItem
		});
		((ToolStripItem)toolStripDropDownButton1).Image = (Image)componentResourceManager.GetObject("toolStripDropDownButton1.Image");
		((ToolStripItem)toolStripDropDownButton1).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)toolStripDropDownButton1).Name = "toolStripDropDownButton1";
		((ToolStripItem)toolStripDropDownButton1).Size = new Size(69, 30);
		((ToolStripItem)toolStripDropDownButton1).Text = "云台测试";
		((ToolStripItem)setYawGainToolStripMenuItem).Name = "setYawGainToolStripMenuItem";
		((ToolStripItem)setYawGainToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)setYawGainToolStripMenuItem).Text = "SetYawGain";
		((ToolStripItem)setYawGainToolStripMenuItem).Click += setYawGainToolStripMenuItem_Click;
		((ToolStripItem)setPitchGainToolStripMenuItem).Name = "setPitchGainToolStripMenuItem";
		((ToolStripItem)setPitchGainToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)setPitchGainToolStripMenuItem).Text = "发设备信息帧";
		((ToolStripItem)setPitchGainToolStripMenuItem).Click += setPitchGainToolStripMenuItem_Click;
		((ToolStripItem)setRollGainToolStripMenuItem).Name = "setRollGainToolStripMenuItem";
		((ToolStripItem)setRollGainToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)setRollGainToolStripMenuItem).Text = "解释升级帧";
		((ToolStripItem)setRollGainToolStripMenuItem).Click += setRollGainToolStripMenuItem_Click;
		((ToolStripItem)遍历控件ToolStripMenuItem).Name = "遍历控件ToolStripMenuItem";
		((ToolStripItem)遍历控件ToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)遍历控件ToolStripMenuItem).Text = "遍历控件";
		((ToolStripItem)遍历控件ToolStripMenuItem).Click += 遍历控件ToolStripMenuItem_Click;
		((ToolStripItem)打开云台界面ToolStripMenuItem).Name = "打开云台界面ToolStripMenuItem";
		((ToolStripItem)打开云台界面ToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)打开云台界面ToolStripMenuItem).Text = "打开云台界面";
		((ToolStripItem)打开云台界面ToolStripMenuItem).Click += 打开云台界面ToolStripMenuItem_Click;
		((ToolStripItem)普通模式云台界面ToolStripMenuItem).Name = "普通模式云台界面ToolStripMenuItem";
		((ToolStripItem)普通模式云台界面ToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)普通模式云台界面ToolStripMenuItem).Text = "普通模式云台界面";
		((ToolStripItem)普通模式云台界面ToolStripMenuItem).Click += 普通模式云台界面ToolStripMenuItem_Click;
		((ToolStripItem)sendRebootCleanToolStripMenuItem).Name = "sendRebootCleanToolStripMenuItem";
		((ToolStripItem)sendRebootCleanToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)sendRebootCleanToolStripMenuItem).Text = "Send_RebootClean";
		((ToolStripItem)sendRebootCleanToolStripMenuItem).Click += sendRebootCleanToolStripMenuItem_Click;
		((ToolStripItem)sendRemoteUpgradeToolStripMenuItem).Name = "sendRemoteUpgradeToolStripMenuItem";
		((ToolStripItem)sendRemoteUpgradeToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)sendRemoteUpgradeToolStripMenuItem).Text = "Send_RemoteUpgrade";
		((ToolStripItem)sendRemoteUpgradeToolStripMenuItem).Click += sendRemoteUpgradeToolStripMenuItem_Click;
		((ToolStripItem)sendSENDFILESTARTToolStripMenuItem).Name = "sendSENDFILESTARTToolStripMenuItem";
		((ToolStripItem)sendSENDFILESTARTToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)sendSENDFILESTARTToolStripMenuItem).Text = "Send_SENDFILE_START";
		((ToolStripItem)sendSENDFILESTARTToolStripMenuItem).Click += sendSENDFILESTARTToolStripMenuItem_Click;
		((ToolStripItem)sendSENDFILEDATAToolStripMenuItem).Name = "sendSENDFILEDATAToolStripMenuItem";
		((ToolStripItem)sendSENDFILEDATAToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)sendSENDFILEDATAToolStripMenuItem).Text = "Send_SENDFILE_DATA";
		((ToolStripItem)sendSENDFILEDATAToolStripMenuItem).Click += sendSENDFILEDATAToolStripMenuItem_Click;
		((ToolStripItem)sendSENDFILEENDToolStripMenuItem).Name = "sendSENDFILEENDToolStripMenuItem";
		((ToolStripItem)sendSENDFILEENDToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)sendSENDFILEENDToolStripMenuItem).Text = "Send_SENDFILE_END";
		((ToolStripItem)sendSENDFILEENDToolStripMenuItem).Click += sendSENDFILEENDToolStripMenuItem_Click;
		((ToolStripItem)sendUPGRADESTATUSToolStripMenuItem).Name = "sendUPGRADESTATUSToolStripMenuItem";
		((ToolStripItem)sendUPGRADESTATUSToolStripMenuItem).Size = new Size(217, 22);
		((ToolStripItem)sendUPGRADESTATUSToolStripMenuItem).Text = "Send_UPGRADE_STATUS";
		((ToolStripItem)sendUPGRADESTATUSToolStripMenuItem).Click += sendUPGRADESTATUSToolStripMenuItem_Click;
		((ToolStripItem)toolStripDropDownButton5).DisplayStyle = (ToolStripItemDisplayStyle)1;
		((ToolStripDropDownItem)toolStripDropDownButton5).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[2]
		{
			(ToolStripItem)开启更改文件数据包ToolStripMenuItem,
			(ToolStripItem)读img中的md5ToolStripMenuItem
		});
		((ToolStripItem)toolStripDropDownButton5).Image = (Image)componentResourceManager.GetObject("toolStripDropDownButton5.Image");
		((ToolStripItem)toolStripDropDownButton5).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)toolStripDropDownButton5).Name = "toolStripDropDownButton5";
		((ToolStripItem)toolStripDropDownButton5).Size = new Size(94, 30);
		((ToolStripItem)toolStripDropDownButton5).Text = "VRXPRO测试";
		((ToolStripItem)开启更改文件数据包ToolStripMenuItem).BackColor = Color.Transparent;
		((ToolStripItem)开启更改文件数据包ToolStripMenuItem).ForeColor = Color.Black;
		((ToolStripItem)开启更改文件数据包ToolStripMenuItem).Name = "开启更改文件数据包ToolStripMenuItem";
		((ToolStripItem)开启更改文件数据包ToolStripMenuItem).Size = new Size(160, 22);
		((ToolStripItem)开启更改文件数据包ToolStripMenuItem).Text = "开启更改数据包";
		((ToolStripItem)开启更改文件数据包ToolStripMenuItem).Click += 开启更改文件数据包ToolStripMenuItem_Click;
		((ToolStripItem)读img中的md5ToolStripMenuItem).Name = "读img中的md5ToolStripMenuItem";
		((ToolStripItem)读img中的md5ToolStripMenuItem).Size = new Size(160, 22);
		((ToolStripItem)读img中的md5ToolStripMenuItem).Text = "读img中的md5";
		((ToolStripItem)读img中的md5ToolStripMenuItem).Click += 读img中的md5ToolStripMenuItem_Click;
		((ToolStripItem)toolStripDropDownButton7).DisplayStyle = (ToolStripItemDisplayStyle)1;
		((ToolStripDropDownItem)toolStripDropDownButton7).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[1] { (ToolStripItem)读imgToolStripMenuItem });
		((ToolStripItem)toolStripDropDownButton7).Image = (Image)componentResourceManager.GetObject("toolStripDropDownButton7.Image");
		((ToolStripItem)toolStripDropDownButton7).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)toolStripDropDownButton7).Name = "toolStripDropDownButton7";
		((ToolStripItem)toolStripDropDownButton7).Size = new Size(89, 30);
		((ToolStripItem)toolStripDropDownButton7).Text = "GTMAX测试";
		((ToolStripItem)读imgToolStripMenuItem).Name = "读imgToolStripMenuItem";
		((ToolStripItem)读imgToolStripMenuItem).Size = new Size(170, 22);
		((ToolStripItem)读imgToolStripMenuItem).Text = "读img中的版本号";
		((ToolStripItem)读imgToolStripMenuItem).Click += 读imgToolStripMenuItem_Click;
		((ToolStripItem)toolStripDropDownButton3).DisplayStyle = (ToolStripItemDisplayStyle)1;
		((ToolStripDropDownItem)toolStripDropDownButton3).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[4]
		{
			(ToolStripItem)打开联系我们ToolStripMenuItem,
			(ToolStripItem)打开说明书ToolStripMenuItem,
			(ToolStripItem)显示pageToolStripMenuItem,
			(ToolStripItem)创建示例帧ToolStripMenuItem
		});
		((ToolStripItem)toolStripDropDownButton3).Image = (Image)componentResourceManager.GetObject("toolStripDropDownButton3.Image");
		((ToolStripItem)toolStripDropDownButton3).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)toolStripDropDownButton3).Name = "toolStripDropDownButton3";
		((ToolStripItem)toolStripDropDownButton3).Size = new Size(69, 30);
		((ToolStripItem)toolStripDropDownButton3).Text = "通用测试";
		((ToolStripItem)toolStripDropDownButton3).Click += toolStripDropDownButton3_Click;
		((ToolStripItem)打开联系我们ToolStripMenuItem).Name = "打开联系我们ToolStripMenuItem";
		((ToolStripItem)打开联系我们ToolStripMenuItem).Size = new Size(148, 22);
		((ToolStripItem)打开联系我们ToolStripMenuItem).Text = "打开联系我们";
		((ToolStripItem)打开联系我们ToolStripMenuItem).Click += 打开联系我们ToolStripMenuItem_Click;
		((ToolStripItem)打开说明书ToolStripMenuItem).Name = "打开说明书ToolStripMenuItem";
		((ToolStripItem)打开说明书ToolStripMenuItem).Size = new Size(148, 22);
		((ToolStripItem)打开说明书ToolStripMenuItem).Text = "打开说明书";
		((ToolStripItem)打开说明书ToolStripMenuItem).Click += 打开说明书ToolStripMenuItem_Click;
		((ToolStripDropDownItem)显示pageToolStripMenuItem).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[3]
		{
			(ToolStripItem)显示连接ToolStripMenuItem,
			(ToolStripItem)显示升级ToolStripMenuItem,
			(ToolStripItem)显示帮助ToolStripMenuItem
		});
		((ToolStripItem)显示pageToolStripMenuItem).Name = "显示pageToolStripMenuItem";
		((ToolStripItem)显示pageToolStripMenuItem).Size = new Size(148, 22);
		((ToolStripItem)显示pageToolStripMenuItem).Text = "显示page";
		((ToolStripItem)显示连接ToolStripMenuItem).Name = "显示连接ToolStripMenuItem";
		((ToolStripItem)显示连接ToolStripMenuItem).Size = new Size(124, 22);
		((ToolStripItem)显示连接ToolStripMenuItem).Text = "显示连接";
		((ToolStripItem)显示连接ToolStripMenuItem).Click += 显示连接ToolStripMenuItem_Click;
		((ToolStripItem)显示升级ToolStripMenuItem).Name = "显示升级ToolStripMenuItem";
		((ToolStripItem)显示升级ToolStripMenuItem).Size = new Size(124, 22);
		((ToolStripItem)显示升级ToolStripMenuItem).Text = "显示升级";
		((ToolStripItem)显示升级ToolStripMenuItem).Click += 显示升级ToolStripMenuItem_Click;
		((ToolStripItem)显示帮助ToolStripMenuItem).Name = "显示帮助ToolStripMenuItem";
		((ToolStripItem)显示帮助ToolStripMenuItem).Size = new Size(124, 22);
		((ToolStripItem)显示帮助ToolStripMenuItem).Text = "显示帮助";
		((ToolStripItem)显示帮助ToolStripMenuItem).Click += 显示帮助ToolStripMenuItem_Click;
		((ToolStripItem)创建示例帧ToolStripMenuItem).Name = "创建示例帧ToolStripMenuItem";
		((ToolStripItem)创建示例帧ToolStripMenuItem).Size = new Size(148, 22);
		((ToolStripItem)创建示例帧ToolStripMenuItem).Text = "创建示例帧";
		((ToolStripItem)创建示例帧ToolStripMenuItem).Click += 创建示例帧ToolStripMenuItem_Click;
		((ToolStripItem)toolStripDropDownButton4).DisplayStyle = (ToolStripItemDisplayStyle)1;
		((ToolStripDropDownItem)toolStripDropDownButton4).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[10]
		{
			(ToolStripItem)测试3ToolStripMenuItem,
			(ToolStripItem)测试2ToolStripMenuItem,
			(ToolStripItem)测试1ToolStripMenuItem,
			(ToolStripItem)停止持续搜索ToolStripMenuItem,
			(ToolStripItem)开始持续搜索ToolStripMenuItem,
			(ToolStripItem)测试连接获取型号ToolStripMenuItem,
			(ToolStripItem)版本过低提示ToolStripMenuItem,
			(ToolStripItem)新流程发finddevToolStripMenuItem,
			(ToolStripItem)新流程发rebootToolStripMenuItem,
			(ToolStripItem)旧流程发finddevToolStripMenuItem
		});
		((ToolStripItem)toolStripDropDownButton4).Image = (Image)componentResourceManager.GetObject("toolStripDropDownButton4.Image");
		((ToolStripItem)toolStripDropDownButton4).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)toolStripDropDownButton4).Name = "toolStripDropDownButton4";
		((ToolStripItem)toolStripDropDownButton4).Size = new Size(69, 30);
		((ToolStripItem)toolStripDropDownButton4).Text = "功能测试";
		((ToolStripItem)toolStripDropDownButton4).TextAlign = (ContentAlignment)16;
		((ToolStripItem)测试3ToolStripMenuItem).Name = "测试3ToolStripMenuItem";
		((ToolStripItem)测试3ToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)测试3ToolStripMenuItem).Text = "添加特定ctrl";
		((ToolStripItem)测试3ToolStripMenuItem).Click += 测试3ToolStripMenuItem_Click;
		((ToolStripItem)测试2ToolStripMenuItem).Name = "测试2ToolStripMenuItem";
		((ToolStripItem)测试2ToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)测试2ToolStripMenuItem).Text = "移除特定ctrl";
		((ToolStripItem)测试2ToolStripMenuItem).Click += 测试2ToolStripMenuItem_Click;
		((ToolStripItem)测试1ToolStripMenuItem).Name = "测试1ToolStripMenuItem";
		((ToolStripItem)测试1ToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)测试1ToolStripMenuItem).Text = "gpt手动搜素";
		((ToolStripItem)测试1ToolStripMenuItem).Click += 测试1ToolStripMenuItem_Click;
		((ToolStripItem)停止持续搜索ToolStripMenuItem).Name = "停止持续搜索ToolStripMenuItem";
		((ToolStripItem)停止持续搜索ToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)停止持续搜索ToolStripMenuItem).Text = "停止持续搜索";
		((ToolStripItem)停止持续搜索ToolStripMenuItem).Click += 停止持续搜索ToolStripMenuItem_Click;
		((ToolStripItem)开始持续搜索ToolStripMenuItem).Name = "开始持续搜索ToolStripMenuItem";
		((ToolStripItem)开始持续搜索ToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)开始持续搜索ToolStripMenuItem).Text = "开始持续搜索";
		((ToolStripItem)开始持续搜索ToolStripMenuItem).Click += 开始持续搜索ToolStripMenuItem_Click;
		((ToolStripItem)测试连接获取型号ToolStripMenuItem).Name = "测试连接获取型号ToolStripMenuItem";
		((ToolStripItem)测试连接获取型号ToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)测试连接获取型号ToolStripMenuItem).Text = "测试连接获取型号";
		((ToolStripItem)测试连接获取型号ToolStripMenuItem).Click += 测试连接获取型号ToolStripMenuItem_Click;
		((ToolStripItem)版本过低提示ToolStripMenuItem).Name = "版本过低提示ToolStripMenuItem";
		((ToolStripItem)版本过低提示ToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)版本过低提示ToolStripMenuItem).Text = "版本过低提示";
		((ToolStripItem)版本过低提示ToolStripMenuItem).Click += 版本过低提示ToolStripMenuItem_Click;
		((ToolStripItem)新流程发finddevToolStripMenuItem).Name = "新流程发finddevToolStripMenuItem";
		((ToolStripItem)新流程发finddevToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)新流程发finddevToolStripMenuItem).Text = "新流程发finddev";
		((ToolStripItem)新流程发finddevToolStripMenuItem).Click += 新流程发finddevToolStripMenuItem_Click;
		((ToolStripItem)新流程发rebootToolStripMenuItem).Name = "新流程发rebootToolStripMenuItem";
		((ToolStripItem)新流程发rebootToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)新流程发rebootToolStripMenuItem).Text = "新流程发reboot_clean";
		((ToolStripItem)新流程发rebootToolStripMenuItem).Click += 新流程发rebootToolStripMenuItem_Click;
		((ToolStripItem)旧流程发finddevToolStripMenuItem).Name = "旧流程发finddevToolStripMenuItem";
		((ToolStripItem)旧流程发finddevToolStripMenuItem).Size = new Size(199, 22);
		((ToolStripItem)旧流程发finddevToolStripMenuItem).Text = "旧流程发finddev";
		((ToolStripItem)旧流程发finddevToolStripMenuItem).Click += 旧流程发finddevToolStripMenuItem_Click;
		((ToolStripItem)toolStripDropDownButton2).DisplayStyle = (ToolStripItemDisplayStyle)1;
		((ToolStripDropDownItem)toolStripDropDownButton2).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[8]
		{
			(ToolStripItem)清空所有pageToolStripMenuItem,
			(ToolStripItem)添加所有pageToolStripMenuItem,
			(ToolStripItem)打开弹窗ToolStripMenuItem,
			(ToolStripItem)打开camhub界面ToolStripMenuItem,
			(ToolStripItem)打开联系我们英文ToolStripMenuItem,
			(ToolStripItem)下载页面ToolStripMenuItem,
			(ToolStripItem)bbfreq页ToolStripMenuItem,
			(ToolStripItem)功能选择ToolStripMenuItem
		});
		((ToolStripItem)toolStripDropDownButton2).Image = (Image)componentResourceManager.GetObject("toolStripDropDownButton2.Image");
		((ToolStripItem)toolStripDropDownButton2).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)toolStripDropDownButton2).Name = "toolStripDropDownButton2";
		((ToolStripItem)toolStripDropDownButton2).Size = new Size(69, 30);
		((ToolStripItem)toolStripDropDownButton2).Text = "界面测试";
		((ToolStripItem)清空所有pageToolStripMenuItem).Name = "清空所有pageToolStripMenuItem";
		((ToolStripItem)清空所有pageToolStripMenuItem).Size = new Size(172, 22);
		((ToolStripItem)清空所有pageToolStripMenuItem).Text = "清空所有page";
		((ToolStripItem)清空所有pageToolStripMenuItem).Click += 清空所有pageToolStripMenuItem_Click;
		((ToolStripItem)添加所有pageToolStripMenuItem).Name = "添加所有pageToolStripMenuItem";
		((ToolStripItem)添加所有pageToolStripMenuItem).Size = new Size(172, 22);
		((ToolStripItem)添加所有pageToolStripMenuItem).Text = "添加所有page";
		((ToolStripItem)添加所有pageToolStripMenuItem).Click += 添加所有pageToolStripMenuItem_Click;
		((ToolStripItem)打开弹窗ToolStripMenuItem).Name = "打开弹窗ToolStripMenuItem";
		((ToolStripItem)打开弹窗ToolStripMenuItem).Size = new Size(172, 22);
		((ToolStripItem)打开弹窗ToolStripMenuItem).Text = "打开弹窗";
		((ToolStripItem)打开弹窗ToolStripMenuItem).Click += 打开弹窗ToolStripMenuItem_Click;
		((ToolStripItem)打开camhub界面ToolStripMenuItem).Name = "打开camhub界面ToolStripMenuItem";
		((ToolStripItem)打开camhub界面ToolStripMenuItem).Size = new Size(172, 22);
		((ToolStripItem)打开camhub界面ToolStripMenuItem).Text = "打开camhub界面";
		((ToolStripItem)打开camhub界面ToolStripMenuItem).Click += 打开camhub界面ToolStripMenuItem_Click;
		((ToolStripItem)打开联系我们英文ToolStripMenuItem).Name = "打开联系我们英文ToolStripMenuItem";
		((ToolStripItem)打开联系我们英文ToolStripMenuItem).Size = new Size(172, 22);
		((ToolStripItem)打开联系我们英文ToolStripMenuItem).Text = "打开联系我们英文";
		((ToolStripItem)打开联系我们英文ToolStripMenuItem).Click += 打开联系我们英文ToolStripMenuItem_Click;
		((ToolStripItem)下载页面ToolStripMenuItem).Name = "下载页面ToolStripMenuItem";
		((ToolStripItem)下载页面ToolStripMenuItem).Size = new Size(172, 22);
		((ToolStripItem)下载页面ToolStripMenuItem).Text = "下载页面";
		((ToolStripItem)下载页面ToolStripMenuItem).Click += 下载页面ToolStripMenuItem_Click;
		((ToolStripItem)bbfreq页ToolStripMenuItem).Name = "bbfreq页ToolStripMenuItem";
		((ToolStripItem)bbfreq页ToolStripMenuItem).Size = new Size(172, 22);
		((ToolStripItem)bbfreq页ToolStripMenuItem).Text = "bbfreq页";
		((ToolStripItem)bbfreq页ToolStripMenuItem).Click += bbfreq页ToolStripMenuItem_Click;
		((ToolStripItem)功能选择ToolStripMenuItem).Name = "功能选择ToolStripMenuItem";
		((ToolStripItem)功能选择ToolStripMenuItem).Size = new Size(172, 22);
		((ToolStripItem)功能选择ToolStripMenuItem).Text = "功能选择";
		((ToolStripItem)功能选择ToolStripMenuItem).Click += 功能选择ToolStripMenuItem_Click;
		((ToolStripItem)toolStripDropDownButton6).BackColor = Color.Red;
		((ToolStripItem)toolStripDropDownButton6).DisplayStyle = (ToolStripItemDisplayStyle)1;
		((ToolStripItem)toolStripDropDownButton6).Image = (Image)componentResourceManager.GetObject("toolStripDropDownButton6.Image");
		((ToolStripItem)toolStripDropDownButton6).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)toolStripDropDownButton6).Name = "toolStripDropDownButton6";
		((ToolStripItem)toolStripDropDownButton6).Size = new Size(96, 30);
		((ToolStripItem)toolStripDropDownButton6).Text = "关闭Asce升级";
		((ToolStripItem)toolStripDropDownButton6).Click += toolStripDropDownButton6_Click;
		((ToolStripItem)toolStripStatusLabel3).BackColor = Color.White;
		((ToolStripItem)toolStripStatusLabel3).Name = "toolStripStatusLabel3";
		((ToolStripItem)toolStripStatusLabel3).Size = new Size(332, 27);
		toolStripStatusLabel3.Spring = true;
		((ToolStripItem)toolStripStatusLabel3).Text = "|";
		((ToolStripItem)toolStripStatusLabel1).BackColor = Color.White;
		((ToolStripItem)toolStripStatusLabel1).Name = "toolStripStatusLabel1";
		((ToolStripItem)toolStripStatusLabel1).Size = new Size(56, 27);
		((ToolStripItem)toolStripStatusLabel1).Text = "当前层级";
		((ToolStripItem)tssl_comStatus).BackColor = Color.White;
		((ToolStripItem)tssl_comStatus).Name = "tssl_comStatus";
		((ToolStripItem)tssl_comStatus).Size = new Size(56, 27);
		((ToolStripItem)tssl_comStatus).Text = "串口状态";
		((Control)tabPage1).Location = new Point(0, 35);
		((Control)tabPage1).Name = "tabPage1";
		((Control)tabPage1).Size = new Size(833, 635);
		((Control)tabPage1).TabIndex = 0;
		((Control)tabPage1).Text = "tabPage1";
		((Control)tabPage2).Location = new Point(0, 35);
		((Control)tabPage2).Name = "tabPage2";
		((Control)tabPage2).Size = new Size(833, 635);
		((Control)tabPage2).TabIndex = 1;
		((Control)tabPage2).Text = "tabPage2";
		((Control)pageHeader1).BackColor = Color.FromArgb(27, 28, 30);
		((Control)pageHeader1).Controls.Add((Control)(object)pic_LeftTopLogo);
		((Control)pageHeader1).Controls.Add((Control)(object)button3);
		((Control)pageHeader1).Controls.Add((Control)(object)button2);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_close);
		pageHeader1.DividerThickness = 0f;
		((Control)pageHeader1).Dock = (DockStyle)1;
		((Control)pageHeader1).Font = new Font("Microsoft Sans Serif", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)pageHeader1).ForeColor = Color.White;
		pageHeader1.Gap = 10;
		((IControl)pageHeader1).HandDragFolder = false;
		pageHeader1.Icon = (Image)(object)Resources.Ascent_CamHub;
		pageHeader1.IconRatio = 1f;
		pageHeader1.IconSvg = "";
		((Control)pageHeader1).Location = new Point(5, 5);
		((Control)pageHeader1).Margin = new Padding(0);
		pageHeader1.MaximizeBox = false;
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Padding = new Padding(5, 0, 5, 0);
		pageHeader1.ShowIcon = true;
		((Control)pageHeader1).Size = new Size(1014, 30);
		pageHeader1.SubGap = 0;
		pageHeader1.SubText = "";
		((Control)pageHeader1).TabIndex = 1;
		((Control)pageHeader1).TabStop = false;
		((Control)pageHeader1).Text = "";
		pageHeader1.UseTextBold = false;
		((Control)pic_LeftTopLogo).BackColor = Color.FromArgb(27, 28, 30);
		((Control)pic_LeftTopLogo).Dock = (DockStyle)3;
		pic_LeftTopLogo.Image = (Image)(object)Resources.左上角logo;
		((Control)pic_LeftTopLogo).Location = new Point(25, 0);
		((Control)pic_LeftTopLogo).Margin = new Padding(0);
		((Control)pic_LeftTopLogo).Name = "pic_LeftTopLogo";
		((Control)pic_LeftTopLogo).Size = new Size(81, 30);
		pic_LeftTopLogo.SizeMode = (PictureBoxSizeMode)4;
		pic_LeftTopLogo.TabIndex = 9;
		pic_LeftTopLogo.TabStop = false;
		button3.DisplayStyle = (TButtonDisplayStyle)2;
		((Control)button3).Dock = (DockStyle)4;
		button3.Ghost = true;
		button3.Icon = (Image)(object)Resources.最小化;
		button3.IconRatio = 0.5f;
		((Control)button3).Location = new Point(882, 0);
		((Control)button3).Margin = new Padding(0);
		((Control)button3).Name = "button3";
		((Control)button3).Size = new Size(45, 30);
		((Control)button3).TabIndex = 8;
		((Control)button3).TabStop = false;
		((Control)button3).Text = "btn_Min";
		button3.WaveSize = 0;
		((Control)button3).MouseClick += new MouseEventHandler(btn_Min_MouseClick);
		button2.DisplayStyle = (TButtonDisplayStyle)2;
		((Control)button2).Dock = (DockStyle)4;
		button2.Ghost = true;
		button2.Icon = (Image)(object)Resources.全屏;
		button2.IconRatio = 0.5f;
		((Control)button2).Location = new Point(927, 0);
		((Control)button2).Margin = new Padding(0);
		((Control)button2).Name = "button2";
		((Control)button2).Size = new Size(45, 30);
		((Control)button2).TabIndex = 7;
		((Control)button2).TabStop = false;
		((Control)button2).Text = "btn_Max";
		button2.WaveSize = 0;
		((Control)button2).MouseClick += new MouseEventHandler(btn_Max_Click);
		btn_close.DisplayStyle = (TButtonDisplayStyle)2;
		((Control)btn_close).Dock = (DockStyle)4;
		btn_close.Ghost = true;
		btn_close.Icon = (Image)(object)Resources.关闭;
		btn_close.IconRatio = 0.5f;
		((Control)btn_close).Location = new Point(972, 0);
		((Control)btn_close).Margin = new Padding(0);
		((Control)btn_close).Name = "btn_close";
		((Control)btn_close).Size = new Size(37, 30);
		((Control)btn_close).TabIndex = 0;
		((Control)btn_close).TabStop = false;
		((Control)btn_close).Text = "button1";
		btn_close.WaveSize = 0;
		((Control)btn_close).MouseClick += new MouseEventHandler(btn_close_Click);
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(32, 33, 37);
		((Window)this).ClientSize = new Size(1024, 720);
		((Form)this).ControlBox = false;
		((Control)this).Controls.Add((Control)(object)statusStrip1);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Controls.Add((Control)(object)pageHeader1);
		((BaseForm)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
		((Form)this).MinimizeBox = false;
		((Control)this).MinimumSize = new Size(1024, 720);
		((Control)this).Name = "MainFrm";
		((Control)this).Padding = new Padding(5);
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "PC Tool";
		((Form)this).FormClosing += new FormClosingEventHandler(MainFrm_FormClosing);
		((Form)this).Load += MainFrm_Load;
		((Form)this).Shown += MainFrm_Shown;
		((Control)this).SizeChanged += MainFrm_SizeChanged;
		((Control)gridPanel1).ResumeLayout(false);
		((Control)statusStrip1).ResumeLayout(false);
		((Control)statusStrip1).PerformLayout();
		((Control)pageHeader1).ResumeLayout(false);
		((ISupportInitialize)pic_LeftTopLogo).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
