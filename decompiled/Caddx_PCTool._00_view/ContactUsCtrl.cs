using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool._00_view;

public class ContactUsCtrl : UserControl
{
	private IContainer components = null;

	private GridPanel gridPanel5;

	private Label label12;

	private GridPanel gridPanel3;

	private Label lab_WechatDesc;

	private Label lab_Wechat;

	private PageHeader pageHeader3;

	private PictureBox pictureBox3;

	private GridPanel gridPanel1;

	private Label lab_emailNum;

	private Label lab_emailDesc;

	private Label lab_email;

	private PageHeader pageHeader2;

	private PictureBox pictureBox4;

	private GridPanel gridPanel2;

	private Label lab_qqGDesc;

	private Label lab_qqGroup;

	private PageHeader pageHeader1;

	private PictureBox pictureBox1;

	private Label label4;

	private Label label3;

	private Label lab_desc;

	private Label lab_title;

	private PictureBox pictureBox2;

	private StackPanel stackPanel1;

	private Label lab_QQGNum2;

	private Label lab_QQGNum;

	public ContactUsCtrl()
	{
		InitializeComponent();
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)2;
	}

	private void ContactUs_Load(object sender, EventArgs e)
	{
		ReloadFont();
		ReloadLang();
	}

	public void ReloadFont()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 20f);
			((Control)lab_title).Font = font;
			Font val = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 14f);
			Label obj = lab_qqGroup;
			Label obj2 = lab_email;
			Font val2 = (((Control)lab_Wechat).Font = val);
			Font font2 = (((Control)obj2).Font = val2);
			((Control)obj).Font = font2;
			Font val5 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
			Label obj3 = lab_QQGNum2;
			Label obj4 = lab_QQGNum;
			Label obj5 = lab_emailNum;
			Label obj6 = lab_desc;
			Label obj7 = lab_qqGDesc;
			Label obj8 = lab_emailDesc;
			Font val6 = (((Control)lab_WechatDesc).Font = val5);
			Font val8 = (((Control)obj8).Font = val6);
			Font val10 = (((Control)obj7).Font = val8);
			Font val12 = (((Control)obj6).Font = val10);
			val2 = (((Control)obj5).Font = val12);
			font2 = (((Control)obj4).Font = val2);
			((Control)obj3).Font = font2;
		});
	}

	public void ReloadLang()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)lab_title).Text = Lang.T("contact.title");
			((Control)lab_desc).Text = Lang.T("contact.desc");
			((Control)lab_qqGroup).Text = Lang.T("contact.qq_group");
			((Control)lab_qqGDesc).Text = Lang.T("contact.qq_group_desc");
			((Control)lab_email).Text = Lang.T("contact.email");
			((Control)lab_emailDesc).Text = Lang.T("contact.email_desc");
			((Control)lab_Wechat).Text = Lang.T("contact.wechat");
			((Control)lab_WechatDesc).Text = Lang.T("contact.wechat_desc");
		});
	}

	public void FontChange(bool isAdd)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			Font font = ((Control)lab_desc).Font;
			float size = font.Size;
			size = ((!isAdd) ? (size - 2f) : (size + 2f));
			Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], size);
			Label obj = lab_QQGNum;
			Label obj2 = lab_desc;
			Label obj3 = lab_qqGDesc;
			Label obj4 = lab_emailDesc;
			Font val2 = (((Control)lab_WechatDesc).Font = val);
			Font val4 = (((Control)obj4).Font = val2);
			Font val6 = (((Control)obj3).Font = val4);
			Font font2 = (((Control)obj2).Font = val6);
			((Control)obj).Font = font2;
		});
	}

	public void FontFamiliesChange(bool isAdd)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
		});
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
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Expected O, but got Unknown
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Expected O, but got Unknown
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_0987: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Expected O, but got Unknown
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b10: Expected O, but got Unknown
		//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be6: Expected O, but got Unknown
		//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fae: Unknown result type (might be due to invalid IL or missing references)
		//IL_1046: Unknown result type (might be due to invalid IL or missing references)
		//IL_1050: Expected O, but got Unknown
		//IL_108d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1108: Unknown result type (might be due to invalid IL or missing references)
		//IL_1112: Expected O, but got Unknown
		//IL_1150: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e2: Expected O, but got Unknown
		//IL_1226: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b8: Expected O, but got Unknown
		//IL_12ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1453: Unknown result type (might be due to invalid IL or missing references)
		//IL_15bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c7: Expected O, but got Unknown
		//IL_1616: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bb: Expected O, but got Unknown
		//IL_16ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ad: Unknown result type (might be due to invalid IL or missing references)
		gridPanel5 = new GridPanel();
		label12 = new Label();
		gridPanel3 = new GridPanel();
		pictureBox2 = new PictureBox();
		lab_WechatDesc = new Label();
		lab_Wechat = new Label();
		pageHeader3 = new PageHeader();
		pictureBox3 = new PictureBox();
		gridPanel1 = new GridPanel();
		lab_emailNum = new Label();
		lab_emailDesc = new Label();
		lab_email = new Label();
		pageHeader2 = new PageHeader();
		pictureBox4 = new PictureBox();
		gridPanel2 = new GridPanel();
		stackPanel1 = new StackPanel();
		lab_QQGNum2 = new Label();
		lab_QQGNum = new Label();
		lab_qqGDesc = new Label();
		lab_qqGroup = new Label();
		pageHeader1 = new PageHeader();
		pictureBox1 = new PictureBox();
		label4 = new Label();
		label3 = new Label();
		lab_desc = new Label();
		lab_title = new Label();
		((Control)gridPanel5).SuspendLayout();
		((Control)gridPanel3).SuspendLayout();
		((ISupportInitialize)pictureBox2).BeginInit();
		((Control)pageHeader3).SuspendLayout();
		((ISupportInitialize)pictureBox3).BeginInit();
		((Control)gridPanel1).SuspendLayout();
		((Control)pageHeader2).SuspendLayout();
		((ISupportInitialize)pictureBox4).BeginInit();
		((Control)gridPanel2).SuspendLayout();
		((Control)stackPanel1).SuspendLayout();
		((Control)pageHeader1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((ContainerPanel)gridPanel5).Back = Color.FromArgb(38, 41, 43);
		((Control)gridPanel5).BackColor = Color.FromArgb(38, 41, 43);
		((Control)gridPanel5).Controls.Add((Control)(object)label12);
		((Control)gridPanel5).Controls.Add((Control)(object)gridPanel3);
		((Control)gridPanel5).Controls.Add((Control)(object)gridPanel1);
		((Control)gridPanel5).Controls.Add((Control)(object)gridPanel2);
		((Control)gridPanel5).Controls.Add((Control)(object)label4);
		((Control)gridPanel5).Controls.Add((Control)(object)label3);
		((Control)gridPanel5).Controls.Add((Control)(object)lab_desc);
		((Control)gridPanel5).Controls.Add((Control)(object)lab_title);
		((Control)gridPanel5).Dock = (DockStyle)5;
		((Control)gridPanel5).Location = new Point(0, 0);
		((Control)gridPanel5).Margin = new Padding(0);
		((Control)gridPanel5).Name = "gridPanel5";
		((Control)gridPanel5).Size = new Size(1000, 750);
		gridPanel5.Span = "100%;100%;5% 28% 3% 28% 3% 28% 10%;100%;\r\n-10% 10% 60% 20%";
		((Control)gridPanel5).TabIndex = 3;
		((Control)gridPanel5).Text = "gridPanel5";
		((Control)label12).AutoSize = true;
		gridPanel5.SetIndex((Control)(object)label12, 5);
		((Control)label12).Location = new Point(333, 150);
		((Control)label12).Name = "label12";
		((Control)label12).Size = new Size(24, 450);
		((Control)label12).TabIndex = 26;
		((ContainerPanel)gridPanel3).Back = Color.Transparent;
		((Control)gridPanel3).BackColor = Color.Transparent;
		((ContainerPanel)gridPanel3).BorderColor = Color.FromArgb(26, 255, 255, 255);
		((ContainerPanel)gridPanel3).BorderWidth = 2f;
		((Control)gridPanel3).Controls.Add((Control)(object)pictureBox2);
		((Control)gridPanel3).Controls.Add((Control)(object)lab_WechatDesc);
		((Control)gridPanel3).Controls.Add((Control)(object)lab_Wechat);
		((Control)gridPanel3).Controls.Add((Control)(object)pageHeader3);
		gridPanel5.SetIndex((Control)(object)gridPanel3, 7);
		((Control)gridPanel3).Location = new Point(670, 150);
		((Control)gridPanel3).Margin = new Padding(0);
		((Control)gridPanel3).Name = "gridPanel3";
		((ContainerPanel)gridPanel3).Radius = 6;
		((Control)gridPanel3).Size = new Size(280, 450);
		gridPanel3.Span = "100%;100%;100%;100%;\r\n-15% 15% 30% 40% ";
		((Control)gridPanel3).TabIndex = 25;
		((Control)gridPanel3).Text = "gridPanel3";
		pictureBox2.Image = (Image)(object)Resources.CADDX技术客服1;
		((Control)pictureBox2).Location = new Point(12, 275);
		((Control)pictureBox2).Margin = new Padding(12, 5, 5, 5);
		((Control)pictureBox2).Name = "pictureBox2";
		((Control)pictureBox2).Size = new Size(263, 170);
		pictureBox2.SizeMode = (PictureBoxSizeMode)4;
		pictureBox2.TabIndex = 24;
		pictureBox2.TabStop = false;
		((Control)lab_WechatDesc).AutoSize = true;
		((Control)lab_WechatDesc).BackColor = Color.Transparent;
		((Control)lab_WechatDesc).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_WechatDesc).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_WechatDesc).Location = new Point(12, 140);
		((Control)lab_WechatDesc).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_WechatDesc).Name = "lab_WechatDesc";
		((Control)lab_WechatDesc).Size = new Size(263, 125);
		((Control)lab_WechatDesc).TabIndex = 23;
		((Control)lab_WechatDesc).Text = "联系客服企业微信：通过企业微信与我们";
		((Control)lab_Wechat).AutoSize = true;
		((Control)lab_Wechat).BackColor = Color.Transparent;
		((Control)lab_Wechat).Font = new Font("阿里巴巴普惠体 Medium", 14.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_Wechat).ForeColor = Color.White;
		((Control)lab_Wechat).Location = new Point(12, 73);
		((Control)lab_Wechat).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_Wechat).Name = "lab_Wechat";
		((Control)lab_Wechat).Size = new Size(263, 58);
		((Control)lab_Wechat).TabIndex = 22;
		((Control)lab_Wechat).Text = "客服企业微信";
		((Control)pageHeader3).BackColor = Color.Transparent;
		((Control)pageHeader3).Controls.Add((Control)(object)pictureBox3);
		((Control)pageHeader3).Dock = (DockStyle)5;
		gridPanel3.SetIndex((Control)(object)pageHeader3, 1);
		((Control)pageHeader3).Location = new Point(2, 5);
		((Control)pageHeader3).Margin = new Padding(2, 5, 5, 5);
		((Control)pageHeader3).Name = "pageHeader3";
		((Control)pageHeader3).Padding = new Padding(10, 0, 0, 0);
		((Control)pageHeader3).Size = new Size(273, 58);
		((Control)pageHeader3).TabIndex = 13;
		((Control)pageHeader3).Text = "";
		((Control)pictureBox3).BackColor = Color.Transparent;
		((Control)pictureBox3).Dock = (DockStyle)3;
		pictureBox3.Image = (Image)(object)Resources.wechat;
		((Control)pictureBox3).Location = new Point(10, 0);
		((Control)pictureBox3).Margin = new Padding(0);
		((Control)pictureBox3).Name = "pictureBox3";
		((Control)pictureBox3).Size = new Size(49, 58);
		pictureBox3.SizeMode = (PictureBoxSizeMode)4;
		pictureBox3.TabIndex = 10;
		pictureBox3.TabStop = false;
		((ContainerPanel)gridPanel1).Back = Color.FromArgb(38, 41, 43);
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(26, 255, 255, 255);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)lab_emailNum);
		((Control)gridPanel1).Controls.Add((Control)(object)lab_emailDesc);
		((Control)gridPanel1).Controls.Add((Control)(object)lab_email);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader2);
		gridPanel5.SetIndex((Control)(object)gridPanel1, 6);
		((Control)gridPanel1).Location = new Point(360, 150);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((ContainerPanel)gridPanel1).Radius = 6;
		((Control)gridPanel1).Size = new Size(280, 450);
		gridPanel1.Span = "100%;100%;100%;100%;\r\n-15% 15% 30% 40%";
		((Control)gridPanel1).TabIndex = 24;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)lab_emailNum).AutoSize = true;
		((Control)lab_emailNum).BackColor = Color.Transparent;
		((Control)lab_emailNum).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_emailNum).ForeColor = Color.FromArgb(255, 233, 0);
		((Control)lab_emailNum).Location = new Point(12, 280);
		((Control)lab_emailNum).Margin = new Padding(12, 10, 5, 5);
		((Control)lab_emailNum).Name = "lab_emailNum";
		((Control)lab_emailNum).Size = new Size(263, 165);
		((Control)lab_emailNum).TabIndex = 24;
		((Control)lab_emailNum).Text = "support@caddxfpv.com";
		((Control)lab_emailDesc).AutoSize = true;
		((Control)lab_emailDesc).BackColor = Color.Transparent;
		((Control)lab_emailDesc).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_emailDesc).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_emailDesc).Location = new Point(12, 140);
		((Control)lab_emailDesc).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_emailDesc).Name = "lab_emailDesc";
		((Control)lab_emailDesc).Size = new Size(263, 125);
		((Control)lab_emailDesc).TabIndex = 23;
		((Control)lab_emailDesc).Text = "发送邮件给我们客服：我们会在24小时内回复。";
		((Control)lab_email).AutoSize = true;
		((Control)lab_email).BackColor = Color.Transparent;
		((Control)lab_email).Font = new Font("阿里巴巴普惠体 Medium", 14.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_email).ForeColor = Color.White;
		((Control)lab_email).Location = new Point(12, 73);
		((Control)lab_email).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_email).Name = "lab_email";
		((Control)lab_email).Size = new Size(263, 58);
		((Control)lab_email).TabIndex = 22;
		((Control)lab_email).Text = "邮件联系";
		((Control)pageHeader2).BackColor = Color.Transparent;
		((Control)pageHeader2).Controls.Add((Control)(object)pictureBox4);
		((Control)pageHeader2).Dock = (DockStyle)5;
		gridPanel1.SetIndex((Control)(object)pageHeader2, 1);
		((Control)pageHeader2).Location = new Point(2, 5);
		((Control)pageHeader2).Margin = new Padding(2, 5, 5, 5);
		((Control)pageHeader2).Name = "pageHeader2";
		((Control)pageHeader2).Padding = new Padding(5, 0, 0, 0);
		((Control)pageHeader2).Size = new Size(273, 58);
		((Control)pageHeader2).TabIndex = 13;
		((Control)pageHeader2).Text = "";
		((Control)pictureBox4).BackColor = Color.Transparent;
		((Control)pictureBox4).Dock = (DockStyle)3;
		pictureBox4.Image = (Image)(object)Resources.email;
		((Control)pictureBox4).Location = new Point(5, 0);
		((Control)pictureBox4).Margin = new Padding(0);
		((Control)pictureBox4).Name = "pictureBox4";
		((Control)pictureBox4).Size = new Size(49, 58);
		pictureBox4.SizeMode = (PictureBoxSizeMode)4;
		pictureBox4.TabIndex = 10;
		pictureBox4.TabStop = false;
		((ContainerPanel)gridPanel2).Back = Color.Transparent;
		((Control)gridPanel2).BackColor = Color.Transparent;
		((ContainerPanel)gridPanel2).BorderColor = Color.FromArgb(26, 255, 255, 255);
		((ContainerPanel)gridPanel2).BorderWidth = 2f;
		((Control)gridPanel2).Controls.Add((Control)(object)stackPanel1);
		((Control)gridPanel2).Controls.Add((Control)(object)lab_qqGDesc);
		((Control)gridPanel2).Controls.Add((Control)(object)lab_qqGroup);
		((Control)gridPanel2).Controls.Add((Control)(object)pageHeader1);
		gridPanel5.SetIndex((Control)(object)gridPanel2, 4);
		((Control)gridPanel2).Location = new Point(50, 150);
		((Control)gridPanel2).Margin = new Padding(0);
		((Control)gridPanel2).Name = "gridPanel2";
		((ContainerPanel)gridPanel2).Radius = 6;
		((Control)gridPanel2).Size = new Size(280, 450);
		gridPanel2.Span = "100%;100%;100%;100%;\r\n-15% 15% 30% 40%";
		((Control)gridPanel2).TabIndex = 23;
		((Control)gridPanel2).Text = "gridPanel2";
		((ContainerPanel)stackPanel1).Back = Color.Transparent;
		((Control)stackPanel1).BackColor = Color.Transparent;
		stackPanel1.Controls.Add((Control)(object)lab_QQGNum2);
		stackPanel1.Controls.Add((Control)(object)lab_QQGNum);
		((Control)stackPanel1).Location = new Point(0, 270);
		((Control)stackPanel1).Margin = new Padding(0);
		((Control)stackPanel1).Name = "stackPanel1";
		((Control)stackPanel1).Size = new Size(280, 180);
		((Control)stackPanel1).TabIndex = 24;
		((Control)stackPanel1).Text = "stackPanel1";
		stackPanel1.Vertical = true;
		((Control)lab_QQGNum2).BackColor = Color.Transparent;
		((Control)lab_QQGNum2).Dock = (DockStyle)1;
		((Control)lab_QQGNum2).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_QQGNum2).ForeColor = Color.FromArgb(255, 233, 0);
		((Control)lab_QQGNum2).Location = new Point(12, 60);
		((Control)lab_QQGNum2).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_QQGNum2).Name = "lab_QQGNum2";
		((Control)lab_QQGNum2).Size = new Size(263, 39);
		((Control)lab_QQGNum2).TabIndex = 27;
		((Control)lab_QQGNum2).Text = "Walksnail\u00a0一群： 481303331";
		((Control)lab_QQGNum).BackColor = Color.Transparent;
		((Control)lab_QQGNum).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_QQGNum).ForeColor = Color.FromArgb(255, 233, 0);
		((Control)lab_QQGNum).Location = new Point(12, 10);
		((Control)lab_QQGNum).Margin = new Padding(12, 10, 5, 5);
		((Control)lab_QQGNum).Name = "lab_QQGNum";
		((Control)lab_QQGNum).Size = new Size(263, 40);
		((Control)lab_QQGNum).TabIndex = 26;
		((Control)lab_QQGNum).Text = "Caddxfpv\u00a0二群: 796793179";
		((Control)lab_qqGDesc).AutoSize = true;
		((Control)lab_qqGDesc).BackColor = Color.Transparent;
		((Control)lab_qqGDesc).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_qqGDesc).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_qqGDesc).Location = new Point(12, 140);
		((Control)lab_qqGDesc).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_qqGDesc).Name = "lab_qqGDesc";
		((Control)lab_qqGDesc).Size = new Size(263, 125);
		((Control)lab_qqGDesc).TabIndex = 23;
		((Control)lab_qqGDesc).Text = "加入我们的QQ群：与我们团队实时沟通";
		((Control)lab_qqGroup).AutoSize = true;
		((Control)lab_qqGroup).BackColor = Color.Transparent;
		((Control)lab_qqGroup).Font = new Font("阿里巴巴普惠体 Medium", 14.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_qqGroup).ForeColor = Color.White;
		((Control)lab_qqGroup).Location = new Point(12, 73);
		((Control)lab_qqGroup).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_qqGroup).Name = "lab_qqGroup";
		((Control)lab_qqGroup).Size = new Size(263, 58);
		((Control)lab_qqGroup).TabIndex = 22;
		((Control)lab_qqGroup).Text = "QQ群联系";
		((Control)pageHeader1).BackColor = Color.Transparent;
		((Control)pageHeader1).Controls.Add((Control)(object)pictureBox1);
		((Control)pageHeader1).Dock = (DockStyle)5;
		gridPanel2.SetIndex((Control)(object)pageHeader1, 1);
		((Control)pageHeader1).Location = new Point(2, 5);
		((Control)pageHeader1).Margin = new Padding(2, 5, 5, 5);
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Padding = new Padding(8, 0, 0, 0);
		((Control)pageHeader1).Size = new Size(273, 58);
		((Control)pageHeader1).TabIndex = 13;
		((Control)pageHeader1).Text = "";
		((Control)pictureBox1).BackColor = Color.Transparent;
		((Control)pictureBox1).Dock = (DockStyle)3;
		pictureBox1.Image = (Image)(object)Resources.qqGroup;
		((Control)pictureBox1).Location = new Point(8, 0);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(49, 58);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 10;
		pictureBox1.TabStop = false;
		((Control)label4).AutoSize = true;
		gridPanel5.SetIndex((Control)(object)label4, 7);
		((Control)label4).Location = new Point(643, 150);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(24, 450);
		((Control)label4).TabIndex = 22;
		((Control)label3).AutoSize = true;
		gridPanel5.SetIndex((Control)(object)label3, 3);
		((Control)label3).Location = new Point(3, 150);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(44, 450);
		((Control)label3).TabIndex = 21;
		((Control)lab_desc).AutoSize = true;
		((Control)lab_desc).Dock = (DockStyle)5;
		((Control)lab_desc).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_desc).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel5.SetIndex((Control)(object)lab_desc, 2);
		((Control)lab_desc).Location = new Point(5, 80);
		((Control)lab_desc).Margin = new Padding(5);
		((Control)lab_desc).Name = "lab_desc";
		((Control)lab_desc).Size = new Size(990, 65);
		((Control)lab_desc).TabIndex = 20;
		((Control)lab_desc).Text = "我们的团队都随时准备为您提供支持。 选择以下任意方式与我们取得联系：";
		lab_desc.TextAlign = (ContentAlignment)2;
		((Control)lab_title).BackColor = Color.FromArgb(38, 41, 43);
		((Control)lab_title).Dock = (DockStyle)5;
		((Control)lab_title).Font = new Font("阿里巴巴普惠体 Medium", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		lab_title.ForeColor = Color.White;
		gridPanel5.SetIndex((Control)(object)lab_title, 1);
		((Control)lab_title).Location = new Point(5, 5);
		((Control)lab_title).Margin = new Padding(5);
		((Control)lab_title).Name = "lab_title";
		((Control)lab_title).Size = new Size(990, 65);
		((Control)lab_title).TabIndex = 0;
		((Control)lab_title).TabStop = false;
		((Control)lab_title).Text = "联系我们";
		lab_title.TextAlign = (ContentAlignment)512;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(96f, 96f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)2;
		((Control)this).BackColor = Color.White;
		((Control)this).Controls.Add((Control)(object)gridPanel5);
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "ContactUsCtrl";
		((Control)this).Size = new Size(1000, 750);
		((UserControl)this).Load += ContactUs_Load;
		((Control)gridPanel5).ResumeLayout(false);
		((Control)gridPanel5).PerformLayout();
		((Control)gridPanel3).ResumeLayout(false);
		((Control)gridPanel3).PerformLayout();
		((ISupportInitialize)pictureBox2).EndInit();
		((Control)pageHeader3).ResumeLayout(false);
		((ISupportInitialize)pictureBox3).EndInit();
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel1).PerformLayout();
		((Control)pageHeader2).ResumeLayout(false);
		((ISupportInitialize)pictureBox4).EndInit();
		((Control)gridPanel2).ResumeLayout(false);
		((Control)gridPanel2).PerformLayout();
		((Control)stackPanel1).ResumeLayout(false);
		((Control)pageHeader1).ResumeLayout(false);
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
