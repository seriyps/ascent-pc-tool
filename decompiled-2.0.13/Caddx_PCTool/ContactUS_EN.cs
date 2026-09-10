using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class ContactUS_EN : UserControl
{
	private IContainer components = null;

	private Label lab_title;

	private Label lab_desc;

	private GridPanel gridPanel5;

	private Label label3;

	private GridPanel gridPanel2;

	private PageHeader pageHeader1;

	private PictureBox pictureBox1;

	private Label label4;

	private Label lab_discord;

	private PictureBox pictureBox2;

	private Label lab_discordDesc;

	private GridPanel gridPanel1;

	private Label lab_emailDesc;

	private Label lab_email;

	private PageHeader pageHeader2;

	private PictureBox pictureBox4;

	private GridPanel gridPanel3;

	private Label lab_WebsiteNum;

	private Label lab_WebsiteDesc;

	private Label lab_Website;

	private PageHeader pageHeader3;

	private PictureBox pictureBox3;

	private Label lab_emailNum;

	private Label label12;

	public ContactUS_EN()
	{
		InitializeComponent();
	}

	private void ContactUS_EN_Load(object sender, EventArgs e)
	{
		ReloadFont();
	}

	private void ReloadFont()
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
		Label obj = lab_discord;
		Label obj2 = lab_email;
		Font val2 = (((Control)lab_Website).Font = val);
		Font font2 = (((Control)obj2).Font = val2);
		((Control)obj).Font = font2;
		Font val5 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
		Label obj3 = lab_emailNum;
		Label obj4 = lab_desc;
		Label obj5 = lab_discordDesc;
		Label obj6 = lab_emailDesc;
		Font val6 = (((Control)lab_WebsiteDesc).Font = val5);
		Font val8 = (((Control)obj6).Font = val6);
		val2 = (((Control)obj5).Font = val8);
		font2 = (((Control)obj4).Font = val2);
		((Control)obj3).Font = font2;
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
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Expected O, but got Unknown
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Expected O, but got Unknown
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Expected O, but got Unknown
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Expected O, but got Unknown
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bee: Expected O, but got Unknown
		//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc3: Expected O, but got Unknown
		//IL_0d04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d96: Expected O, but got Unknown
		//IL_0dc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1046: Unknown result type (might be due to invalid IL or missing references)
		//IL_1107: Unknown result type (might be due to invalid IL or missing references)
		//IL_119b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a5: Expected O, but got Unknown
		//IL_11e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_126e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1278: Expected O, but got Unknown
		//IL_12aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1362: Unknown result type (might be due to invalid IL or missing references)
		//IL_13fe: Unknown result type (might be due to invalid IL or missing references)
		lab_title = new Label();
		lab_desc = new Label();
		gridPanel5 = new GridPanel();
		label12 = new Label();
		gridPanel3 = new GridPanel();
		lab_WebsiteNum = new Label();
		lab_WebsiteDesc = new Label();
		lab_Website = new Label();
		pageHeader3 = new PageHeader();
		pictureBox3 = new PictureBox();
		gridPanel1 = new GridPanel();
		lab_emailNum = new Label();
		lab_emailDesc = new Label();
		lab_email = new Label();
		pageHeader2 = new PageHeader();
		pictureBox4 = new PictureBox();
		gridPanel2 = new GridPanel();
		pictureBox2 = new PictureBox();
		lab_discordDesc = new Label();
		lab_discord = new Label();
		pageHeader1 = new PageHeader();
		pictureBox1 = new PictureBox();
		label4 = new Label();
		label3 = new Label();
		((Control)gridPanel5).SuspendLayout();
		((Control)gridPanel3).SuspendLayout();
		((Control)pageHeader3).SuspendLayout();
		((ISupportInitialize)pictureBox3).BeginInit();
		((Control)gridPanel1).SuspendLayout();
		((Control)pageHeader2).SuspendLayout();
		((ISupportInitialize)pictureBox4).BeginInit();
		((Control)gridPanel2).SuspendLayout();
		((ISupportInitialize)pictureBox2).BeginInit();
		((Control)pageHeader1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((Control)lab_title).Dock = (DockStyle)5;
		((Control)lab_title).Font = new Font("阿里巴巴普惠体 Medium", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		lab_title.ForeColor = Color.FromArgb(255, 255, 255);
		gridPanel5.SetIndex((Control)(object)lab_title, 1);
		((Control)lab_title).Location = new Point(5, 5);
		((Control)lab_title).Margin = new Padding(5);
		((Control)lab_title).Name = "lab_title";
		((Control)lab_title).Size = new Size(840, 55);
		((Control)lab_title).TabIndex = 0;
		((Control)lab_title).TabStop = false;
		((Control)lab_title).Text = "Contact Us";
		lab_title.TextAlign = (ContentAlignment)512;
		((Control)lab_desc).AutoSize = true;
		((Control)lab_desc).Dock = (DockStyle)5;
		((Control)lab_desc).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_desc).ForeColor = Color.FromArgb(153, 153, 153);
		gridPanel5.SetIndex((Control)(object)lab_desc, 2);
		((Control)lab_desc).Location = new Point(5, 70);
		((Control)lab_desc).Margin = new Padding(5);
		((Control)lab_desc).Name = "lab_desc";
		((Control)lab_desc).Size = new Size(840, 55);
		((Control)lab_desc).TabIndex = 20;
		((Control)lab_desc).Text = "Our team is always ready to support you. Contact us using any of the following methods:";
		lab_desc.TextAlign = (ContentAlignment)2;
		((ContainerPanel)gridPanel5).Back = Color.FromArgb(46, 49, 51);
		((Control)gridPanel5).BackColor = Color.FromArgb(46, 49, 51);
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
		((Control)gridPanel5).Size = new Size(850, 650);
		gridPanel5.Span = "100%;100%;5% 28% 3% 28% 3% 28% 10%;100%;\r\n-10% 10% 60% 20%";
		((Control)gridPanel5).TabIndex = 2;
		((Control)gridPanel5).Text = "gridPanel5";
		((Control)label12).AutoSize = true;
		gridPanel5.SetIndex((Control)(object)label12, 5);
		((Control)label12).Location = new Point(283, 130);
		((Control)label12).Name = "label12";
		((Control)label12).Size = new Size(20, 390);
		((Control)label12).TabIndex = 26;
		((ContainerPanel)gridPanel3).Back = Color.Transparent;
		((ContainerPanel)gridPanel3).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)gridPanel3).BorderWidth = 2f;
		((Control)gridPanel3).Controls.Add((Control)(object)lab_WebsiteNum);
		((Control)gridPanel3).Controls.Add((Control)(object)lab_WebsiteDesc);
		((Control)gridPanel3).Controls.Add((Control)(object)lab_Website);
		((Control)gridPanel3).Controls.Add((Control)(object)pageHeader3);
		gridPanel5.SetIndex((Control)(object)gridPanel3, 7);
		((Control)gridPanel3).Location = new Point(570, 130);
		((Control)gridPanel3).Margin = new Padding(0);
		((Control)gridPanel3).Name = "gridPanel3";
		((ContainerPanel)gridPanel3).Radius = 6;
		((Control)gridPanel3).Size = new Size(238, 390);
		gridPanel3.Span = "100%;100%;100%;100%;\r\n-15% 12% 35% 38%";
		((Control)gridPanel3).TabIndex = 25;
		((Control)gridPanel3).Text = "gridPanel3";
		((Control)lab_WebsiteNum).AutoSize = true;
		((Control)lab_WebsiteNum).BackColor = Color.Transparent;
		((Control)lab_WebsiteNum).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_WebsiteNum).ForeColor = Color.FromArgb(255, 233, 0);
		((Control)lab_WebsiteNum).Location = new Point(12, 247);
		((Control)lab_WebsiteNum).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_WebsiteNum).Name = "lab_WebsiteNum";
		((Control)lab_WebsiteNum).Size = new Size(221, 138);
		((Control)lab_WebsiteNum).TabIndex = 24;
		((Control)lab_WebsiteNum).Text = "www.caddxfpv.com";
		((Control)lab_WebsiteDesc).AutoSize = true;
		((Control)lab_WebsiteDesc).BackColor = Color.Transparent;
		((Control)lab_WebsiteDesc).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_WebsiteDesc).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_WebsiteDesc).Location = new Point(12, 110);
		((Control)lab_WebsiteDesc).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_WebsiteDesc).Name = "lab_WebsiteDesc";
		((Control)lab_WebsiteDesc).Size = new Size(221, 126);
		((Control)lab_WebsiteDesc).TabIndex = 23;
		((Control)lab_WebsiteDesc).Text = "Official\u00a0Website:Find\u00a0brand\u00a0news,\u00a0product\u00a0tutorials,\u00a0and\u00a0official\u00a0aftersales\u00a0support\u00a0all\u00a0in\u00a0one\u00a0place.";
		((Control)lab_Website).AutoSize = true;
		((Control)lab_Website).BackColor = Color.Transparent;
		((Control)lab_Website).Font = new Font("阿里巴巴普惠体 Medium", 14.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_Website).ForeColor = Color.White;
		((Control)lab_Website).Location = new Point(12, 64);
		((Control)lab_Website).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_Website).Name = "lab_Website";
		((Control)lab_Website).Size = new Size(221, 37);
		((Control)lab_Website).TabIndex = 22;
		((Control)lab_Website).Text = "Official\u00a0Website";
		((Control)pageHeader3).BackColor = Color.Transparent;
		((Control)pageHeader3).Controls.Add((Control)(object)pictureBox3);
		((Control)pageHeader3).Dock = (DockStyle)5;
		gridPanel3.SetIndex((Control)(object)pageHeader3, 1);
		((Control)pageHeader3).Location = new Point(2, 5);
		((Control)pageHeader3).Margin = new Padding(2, 5, 5, 5);
		((Control)pageHeader3).Name = "pageHeader3";
		((Control)pageHeader3).Size = new Size(231, 49);
		((Control)pageHeader3).TabIndex = 13;
		((Control)pageHeader3).Text = "";
		((Control)pictureBox3).BackColor = Color.Transparent;
		((Control)pictureBox3).Dock = (DockStyle)3;
		pictureBox3.Image = (Image)(object)Resources.official__Website;
		((Control)pictureBox3).Location = new Point(0, 0);
		((Control)pictureBox3).Margin = new Padding(0);
		((Control)pictureBox3).Name = "pictureBox3";
		((Control)pictureBox3).Size = new Size(49, 49);
		pictureBox3.SizeMode = (PictureBoxSizeMode)4;
		pictureBox3.TabIndex = 10;
		pictureBox3.TabStop = false;
		((ContainerPanel)gridPanel1).Back = Color.Transparent;
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)lab_emailNum);
		((Control)gridPanel1).Controls.Add((Control)(object)lab_emailDesc);
		((Control)gridPanel1).Controls.Add((Control)(object)lab_email);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader2);
		gridPanel5.SetIndex((Control)(object)gridPanel1, 6);
		((Control)gridPanel1).Location = new Point(306, 130);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((ContainerPanel)gridPanel1).Radius = 6;
		((Control)gridPanel1).Size = new Size(238, 390);
		gridPanel1.Span = "100%;100%;100%;100%;\r\n-15% 12% 35% 38%";
		((Control)gridPanel1).TabIndex = 24;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)lab_emailNum).AutoSize = true;
		((Control)lab_emailNum).BackColor = Color.Transparent;
		((Control)lab_emailNum).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_emailNum).ForeColor = Color.FromArgb(255, 233, 0);
		((Control)lab_emailNum).Location = new Point(12, 247);
		((Control)lab_emailNum).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_emailNum).Name = "lab_emailNum";
		((Control)lab_emailNum).Size = new Size(221, 138);
		((Control)lab_emailNum).TabIndex = 24;
		((Control)lab_emailNum).Text = "support@caddxfpv.com";
		((Control)lab_emailDesc).AutoSize = true;
		((Control)lab_emailDesc).BackColor = Color.Transparent;
		((Control)lab_emailDesc).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_emailDesc).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_emailDesc).Location = new Point(12, 110);
		((Control)lab_emailDesc).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_emailDesc).Name = "lab_emailDesc";
		((Control)lab_emailDesc).Size = new Size(221, 126);
		((Control)lab_emailDesc).TabIndex = 23;
		((Control)lab_emailDesc).Text = "Email our customer service: We'll respond within 24 hours.";
		((Control)lab_email).AutoSize = true;
		((Control)lab_email).BackColor = Color.Transparent;
		((Control)lab_email).Font = new Font("阿里巴巴普惠体 Medium", 14.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_email).ForeColor = Color.White;
		((Control)lab_email).Location = new Point(12, 64);
		((Control)lab_email).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_email).Name = "lab_email";
		((Control)lab_email).Size = new Size(221, 37);
		((Control)lab_email).TabIndex = 22;
		((Control)lab_email).Text = "Email Contact";
		((Control)pageHeader2).BackColor = Color.Transparent;
		((Control)pageHeader2).Controls.Add((Control)(object)pictureBox4);
		((Control)pageHeader2).Dock = (DockStyle)5;
		gridPanel1.SetIndex((Control)(object)pageHeader2, 1);
		((Control)pageHeader2).Location = new Point(2, 5);
		((Control)pageHeader2).Margin = new Padding(2, 5, 5, 5);
		((Control)pageHeader2).Name = "pageHeader2";
		((Control)pageHeader2).Size = new Size(231, 49);
		((Control)pageHeader2).TabIndex = 13;
		((Control)pageHeader2).Text = "";
		((Control)pictureBox4).BackColor = Color.Transparent;
		((Control)pictureBox4).Dock = (DockStyle)3;
		pictureBox4.Image = (Image)(object)Resources.email;
		((Control)pictureBox4).Location = new Point(0, 0);
		((Control)pictureBox4).Margin = new Padding(0);
		((Control)pictureBox4).Name = "pictureBox4";
		((Control)pictureBox4).Size = new Size(49, 49);
		pictureBox4.SizeMode = (PictureBoxSizeMode)4;
		pictureBox4.TabIndex = 10;
		pictureBox4.TabStop = false;
		((ContainerPanel)gridPanel2).Back = Color.Transparent;
		((ContainerPanel)gridPanel2).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)gridPanel2).BorderWidth = 2f;
		((Control)gridPanel2).Controls.Add((Control)(object)pictureBox2);
		((Control)gridPanel2).Controls.Add((Control)(object)lab_discordDesc);
		((Control)gridPanel2).Controls.Add((Control)(object)lab_discord);
		((Control)gridPanel2).Controls.Add((Control)(object)pageHeader1);
		gridPanel5.SetIndex((Control)(object)gridPanel2, 4);
		((Control)gridPanel2).Location = new Point(42, 130);
		((Control)gridPanel2).Margin = new Padding(0);
		((Control)gridPanel2).Name = "gridPanel2";
		((ContainerPanel)gridPanel2).Radius = 6;
		((Control)gridPanel2).Size = new Size(238, 390);
		gridPanel2.Span = "100%;100%;100%;100%;\r\n-15% 12% 35% 38%";
		((Control)gridPanel2).TabIndex = 23;
		((Control)gridPanel2).Text = "gridPanel2";
		((Control)pictureBox2).BackColor = Color.Transparent;
		((Control)pictureBox2).Dock = (DockStyle)5;
		pictureBox2.Image = (Image)(object)Resources.CADDXFPV_Discord_QR_CODE;
		((Control)pictureBox2).Location = new Point(5, 247);
		((Control)pictureBox2).Margin = new Padding(5);
		((Control)pictureBox2).Name = "pictureBox2";
		((Control)pictureBox2).Size = new Size(228, 138);
		pictureBox2.SizeMode = (PictureBoxSizeMode)4;
		pictureBox2.TabIndex = 24;
		pictureBox2.TabStop = false;
		((Control)lab_discordDesc).AutoSize = true;
		((Control)lab_discordDesc).BackColor = Color.Transparent;
		((Control)lab_discordDesc).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_discordDesc).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_discordDesc).Location = new Point(12, 110);
		((Control)lab_discordDesc).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_discordDesc).Name = "lab_discordDesc";
		((Control)lab_discordDesc).Size = new Size(221, 126);
		((Control)lab_discordDesc).TabIndex = 23;
		((Control)lab_discordDesc).Text = "Join\u00a0CADDXFPV\u00a0Discord:Communiate with our team in real time and get more information.\r\n";
		((Control)lab_discord).AutoSize = true;
		((Control)lab_discord).BackColor = Color.Transparent;
		((Control)lab_discord).Font = new Font("阿里巴巴普惠体 Medium", 14.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_discord).ForeColor = Color.White;
		((Control)lab_discord).Location = new Point(12, 64);
		((Control)lab_discord).Margin = new Padding(12, 5, 5, 5);
		((Control)lab_discord).Name = "lab_discord";
		((Control)lab_discord).Size = new Size(221, 37);
		((Control)lab_discord).TabIndex = 22;
		((Control)lab_discord).Text = "CADDXFPV\u00a0Discord";
		((Control)pageHeader1).BackColor = Color.Transparent;
		((Control)pageHeader1).Controls.Add((Control)(object)pictureBox1);
		((Control)pageHeader1).Dock = (DockStyle)5;
		gridPanel2.SetIndex((Control)(object)pageHeader1, 1);
		((Control)pageHeader1).Location = new Point(2, 5);
		((Control)pageHeader1).Margin = new Padding(2, 5, 5, 5);
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Size = new Size(231, 49);
		((Control)pageHeader1).TabIndex = 13;
		((Control)pageHeader1).Text = "";
		((Control)pictureBox1).BackColor = Color.Transparent;
		((Control)pictureBox1).Dock = (DockStyle)3;
		pictureBox1.Image = (Image)(object)Resources.Discord;
		((Control)pictureBox1).Location = new Point(0, 0);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(49, 49);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 10;
		pictureBox1.TabStop = false;
		((Control)label4).AutoSize = true;
		gridPanel5.SetIndex((Control)(object)label4, 7);
		((Control)label4).Location = new Point(547, 130);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(20, 390);
		((Control)label4).TabIndex = 22;
		((Control)label3).AutoSize = true;
		gridPanel5.SetIndex((Control)(object)label3, 3);
		((Control)label3).Location = new Point(3, 130);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(36, 390);
		((Control)label3).TabIndex = 21;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.White;
		((Control)this).Controls.Add((Control)(object)gridPanel5);
		((Control)this).Name = "ContactUS_EN";
		((Control)this).Size = new Size(850, 650);
		((UserControl)this).Load += ContactUS_EN_Load;
		((Control)gridPanel5).ResumeLayout(false);
		((Control)gridPanel5).PerformLayout();
		((Control)gridPanel3).ResumeLayout(false);
		((Control)gridPanel3).PerformLayout();
		((Control)pageHeader3).ResumeLayout(false);
		((ISupportInitialize)pictureBox3).EndInit();
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel1).PerformLayout();
		((Control)pageHeader2).ResumeLayout(false);
		((ISupportInitialize)pictureBox4).EndInit();
		((Control)gridPanel2).ResumeLayout(false);
		((Control)gridPanel2).PerformLayout();
		((ISupportInitialize)pictureBox2).EndInit();
		((Control)pageHeader1).ResumeLayout(false);
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
