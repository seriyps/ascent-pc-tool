using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class SoftwareSettingFrm : UserControl
{
	private string lang_cn = "语言选择/Language";

	private string software_cn = "软件版本";

	private string userAgree_cn = "用户协议";

	private string checkUpdate_cn = "检查更新";

	private string lang_en = "Language/语言选择";

	private string software_en = "Software version";

	private string userAgree_en = "User Agreement";

	private string checkUpdate_en = "Check Updates";

	private string _className = "SoftwareSettingFrm";

	private IContainer components = null;

	private Dropdown dropdown1;

	private FlowPanel flowPanel4;

	private Label label8;

	private PictureBox pictureBox4;

	private Button button1;

	private Button button2;

	private GridPanel grPan_Main;

	private GridPanel gridPanel4;

	private GridPanel gridPanel5;

	private Label label1;

	private GridPanel gridPanel6;

	private PictureBox pictureBox1;

	private Label lab_userAgree;

	private Label lab_language;

	private PictureBox pictureBox2;

	private Label lab_software;

	private PictureBox pictureBox3;

	public event EventHandler<HappenEventArgs> OnSWSetFrmEvnet;

	public SoftwareSettingFrm()
	{
		InitializeComponent();
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)2;
	}

	private void SoftwareSettingFrm_Load(object sender, EventArgs e)
	{
		((Control)label1).Text = GD.Inst.SoftwareVersion;
		ReloadFont();
		ReloadLang(GD.Inst.CurrLang);
	}

	private void dropdown1_SelectedValueChanged(object sender, ObjectNEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		Dropdown val = (Dropdown)sender;
		((Control)val).Text = (string)val.SelectedValue;
		string text = (string)((VEventArgs<object>)(object)e).Value;
		int num = (text.Contains("简体中文") ? 1 : 2);
		ReloadLang((byte)num);
		HappenEventArgs e2 = new HappenEventArgs
		{
			eventType = EventType.frmSign,
			className = _className,
			msg = (string)((VEventArgs<object>)(object)e).Value,
			Index = num
		};
		OnSWSetFrmEvnet?.Invoke(_className, e2);
	}

	private void lab_checkUpdate_Click(object sender, EventArgs e)
	{
	}

	private void label7_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserAgreementFrm userAgreementFrm = new UserAgreementFrm();
		((Form)userAgreementFrm).ShowDialog();
	}

	public void ReloadFont()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 10f);
		Label obj = lab_language;
		Label obj2 = lab_software;
		Label obj3 = label1;
		Label obj4 = lab_userAgree;
		Label obj5 = label8;
		Font val2 = (((Control)dropdown1).Font = val);
		Font val4 = (((Control)obj5).Font = val2);
		Font val6 = (((Control)obj4).Font = val4);
		Font val8 = (((Control)obj3).Font = val6);
		Font font = (((Control)obj2).Font = val8);
		((Control)obj).Font = font;
	}

	public void FontChange(bool isAdd)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			Font font = ((Control)lab_language).Font;
			FontFamily fontFamily = ((Control)lab_language).Font.FontFamily;
			float size = font.Size;
			size = ((!isAdd) ? (size - 2f) : (size + 2f));
			Font val = new Font(fontFamily, size);
			Label obj = lab_language;
			Label obj2 = lab_software;
			Label obj3 = label1;
			Label obj4 = lab_userAgree;
			Label obj5 = label8;
			Font val2 = (((Control)dropdown1).Font = val);
			Font val4 = (((Control)obj5).Font = val2);
			Font val6 = (((Control)obj4).Font = val4);
			Font val8 = (((Control)obj3).Font = val6);
			Font font2 = (((Control)obj2).Font = val8);
			((Control)obj).Font = font2;
		});
	}

	public void FontFamiliesChange(bool isoppo)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
		});
	}

	public void ReloadLang(byte idx)
	{
		switch ((LangType)idx)
		{
		case LangType.zh_CN:
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				((Control)lab_language).Text = lang_cn;
				((Control)lab_software).Text = software_cn;
				((Control)lab_userAgree).Text = userAgree_cn;
				((Control)dropdown1).Text = "简体中文   ";
			});
			break;
		case LangType.en_US:
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				((Control)lab_language).Text = lang_en;
				((Control)lab_software).Text = software_en;
				((Control)lab_userAgree).Text = userAgree_en;
				((Control)dropdown1).Text = "English   ";
			});
			break;
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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Expected O, but got Unknown
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Expected O, but got Unknown
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Expected O, but got Unknown
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Expected O, but got Unknown
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Expected O, but got Unknown
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Expected O, but got Unknown
		//IL_0902: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d74: Expected O, but got Unknown
		//IL_0dd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f81: Expected O, but got Unknown
		//IL_0fc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1046: Unknown result type (might be due to invalid IL or missing references)
		//IL_1050: Expected O, but got Unknown
		//IL_1086: Unknown result type (might be due to invalid IL or missing references)
		//IL_1133: Unknown result type (might be due to invalid IL or missing references)
		//IL_113d: Expected O, but got Unknown
		//IL_1140: Unknown result type (might be due to invalid IL or missing references)
		//IL_115a: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SoftwareSettingFrm));
		button1 = new Button();
		flowPanel4 = new FlowPanel();
		button2 = new Button();
		label8 = new Label();
		pictureBox4 = new PictureBox();
		dropdown1 = new Dropdown();
		grPan_Main = new GridPanel();
		gridPanel6 = new GridPanel();
		lab_userAgree = new Label();
		pictureBox1 = new PictureBox();
		gridPanel5 = new GridPanel();
		lab_software = new Label();
		pictureBox3 = new PictureBox();
		label1 = new Label();
		gridPanel4 = new GridPanel();
		lab_language = new Label();
		pictureBox2 = new PictureBox();
		((Control)flowPanel4).SuspendLayout();
		((ISupportInitialize)pictureBox4).BeginInit();
		((Control)grPan_Main).SuspendLayout();
		((Control)gridPanel6).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)gridPanel5).SuspendLayout();
		((ISupportInitialize)pictureBox3).BeginInit();
		((Control)gridPanel4).SuspendLayout();
		((ISupportInitialize)pictureBox2).BeginInit();
		((Control)this).SuspendLayout();
		button1.BackHover = Color.Transparent;
		button1.DefaultBack = Color.Transparent;
		button1.DisplayStyle = (TButtonDisplayStyle)1;
		((Control)button1).Font = new Font("阿里巴巴普惠体", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button1.ForeColor = Color.White;
		button1.Ghost = true;
		((IControl)button1).HandDragFolder = false;
		gridPanel6.SetIndex((Control)(object)button1, 3);
		((Control)button1).Location = new Point(239, 5);
		((Control)button1).Margin = new Padding(5, 5, 20, 5);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(521, 39);
		((Control)button1).TabIndex = 2;
		((Control)button1).Text = " >";
		button1.TextAlign = (ContentAlignment)64;
		button1.WaveSize = 0;
		((Control)button1).MouseClick += new MouseEventHandler(label7_MouseClick);
		((ContainerPanel)flowPanel4).BorderColor = Color.FromArgb(235, 237, 240);
		((ContainerPanel)flowPanel4).BorderWidth = 3f;
		((Control)flowPanel4).Controls.Add((Control)(object)button2);
		((Control)flowPanel4).Controls.Add((Control)(object)label8);
		((Control)flowPanel4).Controls.Add((Control)(object)pictureBox4);
		grPan_Main.SetIndex((Control)(object)flowPanel4, 5);
		((Control)flowPanel4).Location = new Point(0, 177);
		((Control)flowPanel4).Margin = new Padding(0);
		((Control)flowPanel4).Name = "flowPanel4";
		((ContainerPanel)flowPanel4).Radius = 6;
		((Control)flowPanel4).Size = new Size(790, 59);
		((Control)flowPanel4).TabIndex = 6;
		((Control)flowPanel4).Text = "flowPanel4";
		((IControl)flowPanel4).Visible = false;
		((Control)button2).Font = new Font("阿里巴巴普惠体", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)button2).Location = new Point(237, 10);
		((Control)button2).Margin = new Padding(20, 10, 0, 0);
		((Control)button2).Name = "button2";
		((Control)button2).Size = new Size(282, 50);
		((Control)button2).TabIndex = 3;
		((Control)button2).Text = " >";
		button2.TextAlign = (ContentAlignment)64;
		button2.WaveSize = 0;
		((Control)label8).Dock = (DockStyle)3;
		((Control)label8).Font = new Font("阿里巴巴普惠体", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label8.ForeColor = Color.FromArgb(51, 51, 51);
		((Control)label8).Location = new Point(85, 15);
		((Control)label8).Margin = new Padding(15, 15, 0, 0);
		((Control)label8).Name = "label8";
		((Control)label8).Size = new Size(132, 40);
		((Control)label8).TabIndex = 1;
		((Control)label8).Text = "建议反馈";
		((Control)pictureBox4).Dock = (DockStyle)3;
		pictureBox4.Image = (Image)componentResourceManager.GetObject("pictureBox4.Image");
		((Control)pictureBox4).Location = new Point(20, 18);
		((Control)pictureBox4).Margin = new Padding(20, 18, 0, 0);
		((Control)pictureBox4).Name = "pictureBox4";
		((Control)pictureBox4).Size = new Size(50, 35);
		pictureBox4.SizeMode = (PictureBoxSizeMode)4;
		pictureBox4.TabIndex = 0;
		pictureBox4.TabStop = false;
		((Button)dropdown1).BackColor = Color.Transparent;
		((Button)dropdown1).BackHover = Color.Transparent;
		((IControl)dropdown1).ColorScheme = (TAMode)2;
		((Button)dropdown1).DefaultBack = Color.Transparent;
		((Control)dropdown1).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Button)dropdown1).ForeColor = Color.White;
		gridPanel4.SetIndex((Control)(object)dropdown1, 3);
		dropdown1.Items.AddRange(new object[2] { "简体中文   ", "English   " });
		((Control)dropdown1).Location = new Point(317, 5);
		((Control)dropdown1).Margin = new Padding(5, 5, 20, 5);
		((Control)dropdown1).Name = "dropdown1";
		dropdown1.Placement = (TAlignFrom)31;
		((Button)dropdown1).ShowArrow = true;
		((Control)dropdown1).Size = new Size(443, 39);
		((Control)dropdown1).TabIndex = 2;
		((Control)dropdown1).TabStop = false;
		((Control)dropdown1).Text = "简体中文    ";
		((Button)dropdown1).TextAlign = (ContentAlignment)64;
		((Button)dropdown1).WaveSize = 0;
		dropdown1.SelectedValueChanged += new ObjectNEventHandler(dropdown1_SelectedValueChanged);
		((ContainerPanel)grPan_Main).Back = Color.Transparent;
		((Control)grPan_Main).BackColor = Color.Transparent;
		((Control)grPan_Main).Controls.Add((Control)(object)gridPanel6);
		((Control)grPan_Main).Controls.Add((Control)(object)gridPanel5);
		((Control)grPan_Main).Controls.Add((Control)(object)gridPanel4);
		((Control)grPan_Main).Controls.Add((Control)(object)flowPanel4);
		((Control)grPan_Main).Dock = (DockStyle)5;
		((IControl)grPan_Main).HandCursor = Cursors.Default;
		((Control)grPan_Main).Location = new Point(25, 25);
		((Control)grPan_Main).Name = "grPan_Main";
		((Control)grPan_Main).Size = new Size(790, 590);
		grPan_Main.Span = "100%;100%;100%;\r\n100%;100%;100%;\r\n100%;100%;100%;\r\n-10% 10% 10% 10% 10% 10% 10% 10% 20%";
		((Control)grPan_Main).TabIndex = 9;
		((Control)grPan_Main).Text = "gridPanel1";
		((ContainerPanel)gridPanel6).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel6).BorderWidth = 2f;
		((Control)gridPanel6).Controls.Add((Control)(object)lab_userAgree);
		((Control)gridPanel6).Controls.Add((Control)(object)pictureBox1);
		((Control)gridPanel6).Controls.Add((Control)(object)button1);
		((IControl)gridPanel6).HandCursor = Cursors.Default;
		grPan_Main.SetIndex((Control)(object)gridPanel6, 3);
		((Control)gridPanel6).Location = new Point(5, 123);
		((Control)gridPanel6).Margin = new Padding(5);
		((Control)gridPanel6).Name = "gridPanel6";
		((ContainerPanel)gridPanel6).Radius = 6;
		((Control)gridPanel6).Size = new Size(780, 49);
		gridPanel6.Span = "10% 20% 70%;";
		((Control)gridPanel6).TabIndex = 13;
		((Control)gridPanel6).Text = "gridPanel6";
		((Control)lab_userAgree).AutoSize = true;
		((Control)lab_userAgree).ForeColor = Color.White;
		gridPanel6.SetIndex((Control)(object)lab_userAgree, 2);
		((Control)lab_userAgree).Location = new Point(78, 10);
		((Control)lab_userAgree).Margin = new Padding(0, 10, 0, 10);
		((Control)lab_userAgree).Name = "lab_userAgree";
		((Control)lab_userAgree).Size = new Size(156, 29);
		((Control)lab_userAgree).TabIndex = 14;
		((Control)lab_userAgree).Text = "User Agreement";
		lab_userAgree.TextAlign = (ContentAlignment)16;
		((Control)pictureBox1).Dock = (DockStyle)5;
		pictureBox1.Image = (Image)(object)Resources.用户协议1;
		gridPanel6.SetIndex((Control)(object)pictureBox1, 1);
		((Control)pictureBox1).Location = new Point(20, 10);
		((Control)pictureBox1).Margin = new Padding(20, 10, 0, 10);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(58, 29);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 10;
		pictureBox1.TabStop = false;
		((ContainerPanel)gridPanel5).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel5).BorderWidth = 2f;
		((Control)gridPanel5).Controls.Add((Control)(object)lab_software);
		((Control)gridPanel5).Controls.Add((Control)(object)pictureBox3);
		((Control)gridPanel5).Controls.Add((Control)(object)label1);
		((IControl)gridPanel5).HandCursor = Cursors.Default;
		grPan_Main.SetIndex((Control)(object)gridPanel5, 3);
		((Control)gridPanel5).Location = new Point(5, 64);
		((Control)gridPanel5).Margin = new Padding(5);
		((Control)gridPanel5).Name = "gridPanel5";
		((ContainerPanel)gridPanel5).Radius = 6;
		((Control)gridPanel5).Size = new Size(780, 49);
		gridPanel5.Span = "10% 20% 70%;";
		((Control)gridPanel5).TabIndex = 12;
		((Control)gridPanel5).Text = "gridPanel5";
		((Control)lab_software).AutoSize = true;
		((Control)lab_software).ForeColor = Color.White;
		gridPanel5.SetIndex((Control)(object)lab_software, 2);
		((Control)lab_software).Location = new Point(78, 10);
		((Control)lab_software).Margin = new Padding(0, 10, 0, 10);
		((Control)lab_software).Name = "lab_software";
		((Control)lab_software).Size = new Size(156, 29);
		((Control)lab_software).TabIndex = 15;
		((Control)lab_software).Text = "Software version";
		lab_software.TextAlign = (ContentAlignment)16;
		((Control)pictureBox3).Dock = (DockStyle)5;
		pictureBox3.Image = (Image)(object)Resources.软件版本1;
		gridPanel5.SetIndex((Control)(object)pictureBox3, 1);
		((Control)pictureBox3).Location = new Point(20, 10);
		((Control)pictureBox3).Margin = new Padding(20, 10, 0, 10);
		((Control)pictureBox3).Name = "pictureBox3";
		((Control)pictureBox3).Size = new Size(58, 29);
		pictureBox3.SizeMode = (PictureBoxSizeMode)4;
		pictureBox3.TabIndex = 14;
		pictureBox3.TabStop = false;
		((Control)label1).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label1.ForeColor = Color.White;
		((IControl)label1).HandCursor = Cursors.Default;
		gridPanel5.SetIndex((Control)(object)label1, 3);
		((Control)label1).Location = new Point(239, 5);
		((Control)label1).Margin = new Padding(5, 5, 20, 5);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(521, 39);
		((Control)label1).TabIndex = 13;
		((Control)label1).Text = "V1.0.8   ";
		label1.TextAlign = (ContentAlignment)64;
		((ContainerPanel)gridPanel4).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel4).BorderWidth = 2f;
		((Control)gridPanel4).Controls.Add((Control)(object)lab_language);
		((Control)gridPanel4).Controls.Add((Control)(object)pictureBox2);
		((Control)gridPanel4).Controls.Add((Control)(object)dropdown1);
		((IControl)gridPanel4).HandCursor = Cursors.Default;
		grPan_Main.SetIndex((Control)(object)gridPanel4, 3);
		((Control)gridPanel4).Location = new Point(5, 5);
		((Control)gridPanel4).Margin = new Padding(5);
		((Control)gridPanel4).Name = "gridPanel4";
		((ContainerPanel)gridPanel4).Radius = 6;
		((Control)gridPanel4).Size = new Size(780, 49);
		gridPanel4.Span = "10% 30% 60%;";
		((Control)gridPanel4).TabIndex = 11;
		((Control)gridPanel4).Text = "gridPanel4";
		((Control)lab_language).AutoSize = true;
		((Control)lab_language).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_language).ForeColor = Color.White;
		gridPanel4.SetIndex((Control)(object)lab_language, 2);
		((Control)lab_language).Location = new Point(78, 10);
		((Control)lab_language).Margin = new Padding(0, 10, 0, 10);
		((Control)lab_language).Name = "lab_language";
		((Control)lab_language).Size = new Size(234, 29);
		((Control)lab_language).TabIndex = 15;
		((Control)lab_language).Text = "语言选择/Language";
		lab_language.TextAlign = (ContentAlignment)16;
		((Control)pictureBox2).Dock = (DockStyle)5;
		pictureBox2.Image = (Image)componentResourceManager.GetObject("pictureBox2.Image");
		gridPanel4.SetIndex((Control)(object)pictureBox2, 1);
		((Control)pictureBox2).Location = new Point(20, 10);
		((Control)pictureBox2).Margin = new Padding(20, 10, 0, 10);
		((Control)pictureBox2).Name = "pictureBox2";
		((Control)pictureBox2).Size = new Size(58, 29);
		pictureBox2.SizeMode = (PictureBoxSizeMode)4;
		pictureBox2.TabIndex = 14;
		pictureBox2.TabStop = false;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(96f, 96f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)2;
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).Controls.Add((Control)(object)grPan_Main);
		((Control)this).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)this).Margin = new Padding(5);
		((Control)this).Name = "SoftwareSettingFrm";
		((Control)this).Padding = new Padding(25);
		((Control)this).Size = new Size(840, 640);
		((UserControl)this).Load += SoftwareSettingFrm_Load;
		((Control)flowPanel4).ResumeLayout(false);
		((ISupportInitialize)pictureBox4).EndInit();
		((Control)grPan_Main).ResumeLayout(false);
		((Control)gridPanel6).ResumeLayout(false);
		((Control)gridPanel6).PerformLayout();
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)gridPanel5).ResumeLayout(false);
		((Control)gridPanel5).PerformLayout();
		((ISupportInitialize)pictureBox3).EndInit();
		((Control)gridPanel4).ResumeLayout(false);
		((Control)gridPanel4).PerformLayout();
		((ISupportInitialize)pictureBox2).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
