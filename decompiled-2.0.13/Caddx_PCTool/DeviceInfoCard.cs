using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class DeviceInfoCard : UserControl
{
	private string _deviceName;

	private string _portName;

	private string btn1_en = "Connect";

	private string btn1_cn = "连接";

	private IContainer components = null;

	private PictureBox pictureBox1;

	private GridPanel gridPanel1;

	private Button button1;

	private Label label2;

	private Label label1;

	public string DeviceName => _deviceName;

	public event EventHandler<DevCardEventArgs> OnDevCardHappenEvent;

	public DeviceInfoCard()
	{
		InitializeComponent();
	}

	public void ShowSearchDev()
	{
		_deviceName = ((GD.Inst.CurrLang == 1) ? "发现新设备" : "Discover devices");
		Label obj = label2;
		bool visible = (((IControl)label1).Visible = true);
		((IControl)obj).Visible = visible;
		((IControl)button1).Visible = false;
		((Control)label1).Text = " ";
		((Control)label2).Text = DeviceName;
		pictureBox1.Image = (Image)(object)Resources.搜索中;
	}

	public void ShowUnsupportDev(string portname)
	{
		_deviceName = ((GD.Inst.CurrLang == 1) ? "不支持该设备" : "Unsupport devices");
		Label obj = label2;
		bool visible = (((IControl)label1).Visible = true);
		((IControl)obj).Visible = visible;
		((IControl)button1).Visible = false;
		((Control)label1).Text = _deviceName;
		((Control)label2).Text = portname;
		pictureBox1.Image = (Image)(object)Resources.不支持该设备;
	}

	public void ShowDevOccupy()
	{
		_deviceName = ((GD.Inst.CurrLang == 1) ? "设备已占用" : "Device occupied");
		Label obj = label2;
		bool visible = (((IControl)label1).Visible = true);
		((IControl)obj).Visible = visible;
		((IControl)button1).Visible = false;
		((Control)label1).Text = _deviceName;
	}

	public void SetDeviceName(string devName, string portName)
	{
		string st = devName.Replace("_", " ");
		_deviceName = devName;
		_portName = portName;
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			Label obj = label2;
			Label obj2 = label1;
			bool flag = (((IControl)button1).Visible = true);
			bool visible = (((IControl)obj2).Visible = flag);
			((IControl)obj).Visible = visible;
			((Control)label1).Text = st;
			((Control)label2).Text = portName;
		});
		ReloadLang();
	}

	public void SetDeviceInfo(ResAscentInfo info)
	{
		Label obj = label2;
		Label obj2 = label1;
		bool flag = (((IControl)button1).Visible = true);
		bool visible = (((IControl)obj2).Visible = flag);
		((IControl)obj).Visible = visible;
		string portName = (((Control)label2).Text = info.UsbInfo.PortName);
		_portName = portName;
		ReloadLang();
		if (!string.IsNullOrEmpty(info.DevName))
		{
			DeviceNameJudge(info.DevName, info.UsbInfo.PortName, out _deviceName);
			((Control)label1).Text = _deviceName;
			return;
		}
		Match match = Regex.Match(info.HWVers, "\\d+");
		if (!match.Success)
		{
			pictureBox1.Image = (Image)(object)Resources.不支持该设备;
			((Control)label1).Text = ((GD.Inst.CurrLang == 1) ? "不支持该设备" : "Unsupport devices");
			WriteLog.WriteLogFileToUI("HWVers匹配失败,HWVers=" + info.HWVers + ",FWVers=" + info.FWVers, Color.Red);
			return;
		}
		string value = match.Groups[0].Value;
		if (value.Contains("Changekit"))
		{
			pictureBox1.Image = (Image)(object)Resources.不支持该设备;
		}
		if (value.Contains("486"))
		{
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTPro;
			((Control)label1).Text = "Ascent GT Pro";
		}
		else if (value.Contains("485"))
		{
			string value2 = Regex.Match(info.FWVers, "Ascent(?:_\\w+){2}").Value;
			if (value2.Contains("VRX-Pro"))
			{
				pictureBox1.Image = (Image)(object)Resources.Ascent_VRXPro;
				((Control)label1).Text = "Ascent VRX Pro";
			}
			else
			{
				pictureBox1.Image = (Image)(object)Resources.Ascent_VRX;
				((Control)label1).Text = "Ascent VRX";
			}
		}
		else if (value.Contains("482"))
		{
			pictureBox1.Image = (Image)(object)Resources.Ascent_Lite_VTX;
			((Control)label1).Text = "Ascent Lite VTX";
		}
		else if (value.Contains("472"))
		{
			pictureBox1.Image = (Image)(object)Resources.Ascent_LitePlus;
			((Control)label1).Text = "Ascent Lite +";
		}
		else if (value.Contains("492"))
		{
			pictureBox1.Image = (Image)(object)Resources.Ascent_RC;
			((Control)label1).Text = "YoHD Micro";
		}
		else if (info.HWVers.Contains("GM1"))
		{
			pictureBox1.Image = (Image)(object)Resources.GM1_V2;
			((Control)label1).Text = "GM1_V2";
		}
		else if (info.HWVers.Contains("GM3"))
		{
			pictureBox1.Image = (Image)(object)Resources.GM3_V2;
			((Control)label1).Text = "GM3_V2";
		}
		else if (info.HWVers.Contains("Ascent-VRX-Pro"))
		{
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRXPro;
			((Control)label1).Text = "Ascent VRX Pro";
		}
		_deviceName = ((Control)label1).Text;
	}

	public void SetDeviceInfo(ResAscentInfo info, ShowLogoStatus status)
	{
		Label obj = label2;
		Label obj2 = label1;
		bool flag = (((IControl)button1).Visible = true);
		bool visible = (((IControl)obj2).Visible = flag);
		((IControl)obj).Visible = visible;
		string portName = (((Control)label2).Text = info.UsbInfo.PortName);
		_portName = portName;
		ReloadLang();
		switch (Program.SwStatus)
		{
		case ShowLogoStatus.company:
			SetDeviceInfo_caddxLogo(info);
			break;
		default:
			SetDeviceInfo_noLogo(info);
			break;
		}
	}

	private void SetDeviceInfo_caddxLogo(ResAscentInfo info)
	{
		if (!string.IsNullOrEmpty(info.DevName))
		{
			DeviceNameJudge(info.DevName, info.UsbInfo.PortName, out _deviceName);
			((Control)label1).Text = _deviceName;
			return;
		}
		Match match = Regex.Match(info.HWVers, "\\d+");
		if (!match.Success)
		{
			pictureBox1.Image = (Image)(object)Resources.不支持该设备;
			((Control)label1).Text = ((GD.Inst.CurrLang == 1) ? "不支持该设备" : "Unsupport devices");
			WriteLog.WriteLogFileToUI("HWVers匹配失败,HWVers=" + info.HWVers + ",FWVers=" + info.FWVers, Color.Red);
		}
		else
		{
			string value = match.Groups[0].Value;
			_deviceName = ((Control)label1).Text;
		}
	}

	private void SetDeviceInfo_noLogo(ResAscentInfo info)
	{
		if (!string.IsNullOrEmpty(info.DevName))
		{
			DeviceNameJudge(info.DevName, info.UsbInfo.PortName, out _deviceName);
			((Control)label1).Text = _deviceName;
			return;
		}
		Match match = Regex.Match(info.HWVers, "\\d+");
		if (!match.Success)
		{
			pictureBox1.Image = (Image)(object)Resources.不支持该设备;
			((Control)label1).Text = ((GD.Inst.CurrLang == 1) ? "不支持该设备" : "Unsupport devices");
			WriteLog.WriteLogFileToUI("HWVers匹配失败,HWVers=" + info.HWVers + ",FWVers=" + info.FWVers, Color.Red);
		}
		else
		{
			string value = match.Groups[0].Value;
			_deviceName = ((Control)label1).Text;
		}
	}

	private void DeviceNameJudge(string devName, string portName, out string devname)
	{
		string text = devName.ToLower();
		devname = "";
		if (text.Contains("z40") || text.Contains("z8") || text.Contains("hub"))
		{
			text = devName.ToLower();
		}
		else if (text.Contains("ascent_gt_pro") && text.Length > 13)
		{
			text = "ascent_gt_pro";
		}
		else if (text.Contains("ascent_lite") && text.Length > 13)
		{
			text = ((!text.Contains("ascent_lite_plus")) ? "ascent_lite" : "ascent_lite_plus");
		}
		else if (text.Contains("yohd_micro") || text.Contains("ascent_rc"))
		{
			text = "yohd_micro";
		}
		switch (text)
		{
		case "ascent_vrx_cine":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			devname = "Ascent VRX Cine";
			break;
		case "ascent_vrx_max":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRXMAX;
			devname = "Ascent VRX Max";
			break;
		case "ascent_vrx":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRX;
			devname = "Ascent VRX";
			break;
		case "cx485_pro":
		case "ascent_vrx_pro":
			pictureBox1.Image = (Image)(object)Resources.Ascent_VRXPro;
			devname = "Ascent VRX Pro";
			break;
		case "ascent_gt_pro_z40":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTProZ40;
			devname = "Ascent GT Pro Z40";
			break;
		case "ascent_gt_pro_z8":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTProZ8;
			devname = "Ascent GT Pro Z8";
			break;
		case "ascent_gt_pro_hub":
			pictureBox1.Image = (Image)(object)Resources.Ascent_CamHub;
			devname = "Ascent GT Pro Hub";
			break;
		case "ascent_lite_plus":
			pictureBox1.Image = (Image)(object)Resources.Ascent_LitePlus;
			devname = "Ascent Lite +";
			break;
		case "ascent_lite":
			pictureBox1.Image = (Image)(object)Resources.Ascent_Lite_VTX;
			devname = "Ascent Lite VTX";
			break;
		case "yohd_micro":
		case "ascent_rc":
			pictureBox1.Image = (Image)(object)Resources.Ascent_RC;
			devname = "YoHD Micro";
			break;
		case "caddx_gm3":
			pictureBox1.Image = (Image)(object)Resources.GM3_V2;
			devname = "GM3 V2";
			break;
		case "caddx_gm1":
			pictureBox1.Image = (Image)(object)Resources.GM1_V2;
			devname = "GM1 V2";
			break;
		case "ascent_gt_pro":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GTPro;
			devname = "Ascent GT Pro";
			break;
		case "ascent_gt":
			pictureBox1.Image = (Image)(object)Resources.Ascent_GT;
			devname = "Ascent GT";
			break;
		case "ascent_gt_27":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			devname = "Ascent GT 2.7k";
			break;
		case "ascent_gt_night":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			devname = "Ascent GT night";
			break;
		case "ascent_gt_ultra":
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			devname = "Ascent GT ultra";
			break;
		case "caddxsimgm":
			pictureBox1.Image = (Image)(object)Resources.neutralLogo;
			devname = "SimGM";
			break;
		case "ascent_goggles":
			pictureBox1.Image = (Image)(object)Resources.Ascent_Goggles;
			devname = "Ascent Goggles L";
			break;
		default:
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			devname = "new device";
			break;
		}
		WriteLog.WriteLogFileToUI("返回的devname=" + devName + "，输出的devname=" + devname, Color.DarkBlue);
	}

	public void SetDeviceName(bool isshow, string name, string btnName)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			_deviceName = name;
			Label obj = label1;
			bool visible = (((IControl)button1).Visible = true);
			((IControl)obj).Visible = visible;
			((IControl)label2).Visible = false;
			((Control)label1).Text = name;
			((Control)button1).Text = btnName;
		});
	}

	public void SetLocation()
	{
	}

	private void button1_Click_1(object sender, EventArgs e)
	{
		DevCardEventArgs e2 = new DevCardEventArgs
		{
			Name = ((Control)this).Name,
			DeviceName = _deviceName,
			PortName = _portName,
			BtnName = ((Control)button1).Text
		};
		OnDevCardHappenEvent?.Invoke(null, e2);
	}

	private void DeviceInfoCard_Load(object sender, EventArgs e)
	{
		ReloadFont();
	}

	public void ReloadFont()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		try
		{
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 12f);
			((Control)label1).Font = font;
			Font font2 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 10f);
			((Control)label2).Font = font2;
			Font font3 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 10f);
			((Control)button1).Font = font3;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("DeviceInfoCard.ReloadFont error ,desc=" + ex.Message, Color.DarkRed);
		}
	}

	private void DeviceInfoCard_MouseEnter(object sender, EventArgs e)
	{
	}

	private void DeviceInfoCard_MouseLeave(object sender, EventArgs e)
	{
	}

	private void gridPanel1_MouseEnter(object sender, EventArgs e)
	{
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(255, 233, 0);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
	}

	private void gridPanel1_MouseLeave(object sender, EventArgs e)
	{
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)gridPanel1).BorderWidth = 1f;
	}

	public void ReloadLang()
	{
		try
		{
			switch ((LangType)GD.Inst.CurrLang)
			{
			case LangType.zh_CN:
				((Control)button1).Text = btn1_cn;
				break;
			case LangType.en_US:
				((Control)button1).Text = btn1_en;
				if (((Control)label1).Text == "设备已占用")
				{
					((Control)label1).Text = "Device occupied";
				}
				break;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("DeviceInfoCard.ReloadLang error ,desc=" + ex.Message, Color.DarkRed);
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
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Expected O, but got Unknown
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Expected O, but got Unknown
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		gridPanel1 = new GridPanel();
		label2 = new Label();
		label1 = new Label();
		button1 = new Button();
		pictureBox1 = new PictureBox();
		((Control)gridPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((Control)gridPanel1).BackColor = Color.FromArgb(46, 49, 51);
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)button1);
		((Control)gridPanel1).Controls.Add((Control)(object)label2);
		((Control)gridPanel1).Controls.Add((Control)(object)label1);
		((Control)gridPanel1).Controls.Add((Control)(object)pictureBox1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((ContainerPanel)gridPanel1).Radius = 15;
		((Control)gridPanel1).Size = new Size(483, 476);
		gridPanel1.Span = "100%;100%;100%;100%;-50% 15% 15% 20%";
		((Control)gridPanel1).TabIndex = 14;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)gridPanel1).MouseEnter += gridPanel1_MouseEnter;
		((Control)gridPanel1).MouseLeave += gridPanel1_MouseLeave;
		((Control)label2).BackColor = Color.Transparent;
		((Control)label2).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label2.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)label2).Location = new Point(10, 309);
		((Control)label2).Margin = new Padding(10, 0, 10, 0);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(463, 71);
		((Control)label2).TabIndex = 15;
		((Control)label2).Text = "Discovered new devices";
		label2.TextAlign = (ContentAlignment)32;
		((Control)label2).MouseEnter += gridPanel1_MouseEnter;
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Font = new Font("阿里巴巴普惠体", 15f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label1.ForeColor = Color.White;
		((Control)label1).Location = new Point(10, 238);
		((Control)label1).Margin = new Padding(10, 0, 10, 0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(463, 71);
		((Control)label1).TabIndex = 14;
		((Control)label1).Text = "label1";
		label1.TextAlign = (ContentAlignment)32;
		((Control)label1).MouseEnter += gridPanel1_MouseEnter;
		button1.BackColor = Color.FromArgb(26, 66, 130, 248);
		button1.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)button1).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button1.ForeColor = Color.FromArgb(35, 35, 35);
		button1.Icon = (Image)(object)Resources.连接设备;
		button1.IconGap = 0.4f;
		button1.IconRatio = 0.8f;
		((Control)button1).Location = new Point(5, 386);
		((Control)button1).Margin = new Padding(5);
		((Control)button1).Name = "button1";
		button1.Radius = 10;
		((Control)button1).Size = new Size(473, 85);
		((Control)button1).TabIndex = 16;
		((Control)button1).Text = "button1";
		button1.WaveSize = 0;
		((Control)button1).Click += button1_Click_1;
		((Control)button1).MouseEnter += gridPanel1_MouseEnter;
		((Control)pictureBox1).BackColor = Color.FromArgb(46, 49, 51);
		pictureBox1.Image = (Image)(object)Resources.Ascent_GT;
		((Control)pictureBox1).Location = new Point(10, 10);
		((Control)pictureBox1).Margin = new Padding(10);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(463, 218);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 13;
		pictureBox1.TabStop = false;
		((Control)pictureBox1).MouseEnter += gridPanel1_MouseEnter;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Control)this).BackgroundImageLayout = (ImageLayout)4;
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).DoubleBuffered = true;
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "DeviceInfoCard";
		((Control)this).Size = new Size(483, 476);
		((UserControl)this).Load += DeviceInfoCard_Load;
		((Control)this).MouseEnter += DeviceInfoCard_MouseEnter;
		((Control)this).MouseLeave += DeviceInfoCard_MouseLeave;
		((Control)gridPanel1).ResumeLayout(false);
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
