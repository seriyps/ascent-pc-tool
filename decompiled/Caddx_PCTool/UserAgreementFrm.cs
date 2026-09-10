using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class UserAgreementFrm : Window
{
	private string desc_en;

	private string desc_cn;

	private string title_en;

	private string title_cn;

	public DialogResult result;

	private IContainer components;

	private PageHeader pageHeader1;

	private Button button1;

	private Label label1;

	private RichTextBox richTextBox1;

	public UserAgreementFrm()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		desc_en = "Version: 1.0 \r\nEffective Date: September 2025\r\nWelcome to this software (hereinafter referred to as \"the Software\" or \"Software\"). Before installing, accessing, or using this Software, please carefully read this User Agreement (hereinafter referred to as \"Agreement\") regarding the use of this Software. This Agreement constitutes a legal document between you and the Software Provider (hereinafter referred to as \"the Company\" or \"we\") regarding the use of this Software.\r\nBy installing, using, or accessing this Software in any way, you agree to be bound by all terms of this Agreement. If you disagree with any part of this Agreement, please do not install or use this Software.\r\n\r\n1. Definitions\r\nSoftware: refers to the application developed and legally distributed by the Company, and all related components, updates, revisions, etc.\r\nUser: refers to the individual or organization using the Software.\r\nService: refers to all features and services related to the Software provided by the Company.\r\n\r\n2. License\r\nAuthorized Use: The Company grants you a non-exclusive, non-transferable license under this Agreement to install, access, and use the Software on legally authorized devices. Restrictions on Use: Unless expressly authorized by the Company, you may not reverse engineer, decrypt, disassemble, modify, or otherwise manipulate the source code of this Software, nor may you use the Software for commercial purposes without authorization.\r\nApplicable Devices: This Software is limited to the designated platforms or operating system versions. Please refer to the official website or software documentation for specific supported devices and operating systems.\r\n\r\n3. User Account\r\nAccount Registration: Some Software features may require you to create an account. You agree to provide accurate and complete registration information and keep it up to date.\r\nAccount Security: You are responsible for maintaining the confidentiality of your account information and preventing it from being accessed by unauthorized third parties. Please notify us promptly if you become aware of any unauthorized use of your account.\r\nAccount Deactivation: The Company reserves the right to suspend or terminate your account if you violate the terms of this Agreement, engage in illegal activity, or engage in conduct deemed detrimental to the interests of the Company.";
		desc_cn = "版本：1.0\r\n生效日期：2025年9月\r\n欢迎使用本软件（以下简称“本软件”或“软件”）。在您安装、访问或使用本软件之前，请仔细阅读本用户协议（以下简称“协议”）。本协议是您与软件提供方（以下简称“公司”或“我们”）之间关于使用本软件的法律文件。\r\n通过安装、使用或以任何方式访问本软件，您同意遵守本协议的所有条款。如果您不同意本协议的任何部分，请勿安装或使用本软件。\r\n\r\n\r\n1. 定义\r\n软件：指公司开发并通过合法途径发布的应用程序及其所有相关组件、更新、修订版本等。\r\n用户：指使用本软件的个人或组织。\r\n服务：公司提供的与本软件相关的所有功能和服务。\r\n\r\n2. 使用许可\r\n授权使用：公司根据本协议授予您非独占、不可转让的使用许可，允许您在合法授权的设备上安装、访问和使用本软件。\r\n使用限制：除非得到公司明确授权，您不得对本软件进行逆向工程、解密、拆解、修改或以其他方式对软件源代码进行操作，亦不得在未经授权的情况下将软件用于商业用途。\r\n适用设备：本软件仅限于在指定平台或操作系统版本上使用，具体支持的设备和操作系统请参见官方网站或软件文档。\r\n\r\n3. 用户账户\r\n账户注册：部分软件功能可能需要您创建账户。您同意提供准确、完整的注册信息，并保持信息的更新。\r\n账户安全：您负责保持账户信息的机密性，确保不被未经授权的第三方访问。若发现账户遭受未经授权的使用，请及时通知我们。\r\n账户停用：公司有权在以下情况下暂停或终止您的账户：违反本协议条款、涉及非法活动或被认为有损公司利益的行为。";
		title_en = "User Agreement";
		title_cn = "用户协议";
		result = (DialogResult)2;
		components = null;
		((Window)this)._002Ector();
		InitializeComponent();
	}

	private void ModalFrm_Load(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		result = (DialogResult)2;
		ReloadFont();
		((Control)label1).Text = ((GD.Inst.CurrLang == 1) ? title_cn : title_en);
		((TextBoxBase)richTextBox1).AppendText((GD.Inst.CurrLang == 1) ? desc_cn : desc_en);
	}

	private void ReloadFont()
	{
		((BaseForm)this).Invoke((Action)delegate
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 14f);
			((Control)pageHeader1).Font = font;
			Font font2 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
			((Control)richTextBox1).Font = font2;
		});
	}

	private void button4_Click(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		result = (DialogResult)1;
		((Form)this).Close();
	}

	private void ModalFrm_FormClosing(object sender, FormClosingEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		result = (DialogResult)2;
		((Control)this).Hide();
		((CancelEventArgs)(object)e).Cancel = true;
	}

	private void label1_Click(object sender, EventArgs e)
	{
	}

	private void button5_Click(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		result = (DialogResult)2;
		((Form)this).Close();
	}

	private void button1_Click(object sender, EventArgs e)
	{
	}

	private void pageHeader1_Click(object sender, EventArgs e)
	{
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Expected O, but got Unknown
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Expected O, but got Unknown
		pageHeader1 = new PageHeader();
		button1 = new Button();
		label1 = new Label();
		richTextBox1 = new RichTextBox();
		((Control)pageHeader1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)pageHeader1).BackColor = Color.Transparent;
		((Control)pageHeader1).Controls.Add((Control)(object)button1);
		((Control)pageHeader1).Controls.Add((Control)(object)label1);
		((Control)pageHeader1).Dock = (DockStyle)1;
		((Control)pageHeader1).Location = new Point(10, 10);
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Size = new Size(774, 30);
		((Control)pageHeader1).TabIndex = 0;
		((Control)pageHeader1).Text = "";
		button1.DisplayStyle = (TButtonDisplayStyle)2;
		((Control)button1).Dock = (DockStyle)4;
		button1.Ghost = true;
		button1.Icon = (Image)(object)Resources.关闭;
		((Control)button1).Location = new Point(692, 0);
		((Control)button1).Margin = new Padding(0);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(82, 30);
		((Control)button1).TabIndex = 1;
		((Control)button1).Click += button4_Click;
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Dock = (DockStyle)3;
		((Control)label1).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label1).ForeColor = Color.White;
		label1.ImageAlign = (ContentAlignment)16;
		((Control)label1).Location = new Point(0, 0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(272, 30);
		((Control)label1).TabIndex = 0;
		((Control)label1).Text = "label1";
		label1.TextAlign = (ContentAlignment)16;
		((Control)richTextBox1).BackColor = Color.FromArgb(46, 49, 51);
		((TextBoxBase)richTextBox1).BorderStyle = (BorderStyle)0;
		((Control)richTextBox1).Dock = (DockStyle)5;
		((Control)richTextBox1).ForeColor = Color.White;
		((Control)richTextBox1).Location = new Point(10, 40);
		((Control)richTextBox1).Margin = new Padding(0);
		((Control)richTextBox1).Name = "richTextBox1";
		((TextBoxBase)richTextBox1).ReadOnly = true;
		((Control)richTextBox1).Size = new Size(774, 700);
		((Control)richTextBox1).TabIndex = 1;
		((Control)richTextBox1).TabStop = false;
		((Control)richTextBox1).Text = "";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(16f, 37f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Window)this).ClientSize = new Size(794, 750);
		((Control)this).Controls.Add((Control)(object)richTextBox1);
		((Control)this).Controls.Add((Control)(object)pageHeader1);
		((Control)this).Font = new Font("阿里巴巴普惠体", 20.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((BaseForm)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).Margin = new Padding(8, 9, 8, 9);
		((Control)this).Name = "UserAgreementFrm";
		((Control)this).Padding = new Padding(10);
		((Window)this).Resizable = false;
		((Form)this).ShowIcon = false;
		((Window)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "ModalFrm";
		((Form)this).FormClosing += new FormClosingEventHandler(ModalFrm_FormClosing);
		((Form)this).Load += ModalFrm_Load;
		((Control)pageHeader1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
