using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class StartUpgAscentModalFrm : Form
{
	private string title_en;

	private string desc_en;

	private string list1desc_en;

	private string list2desc_en;

	private string list3desc_en;

	private string btn4_en;

	private string btn3_en;

	private string title_cn;

	private string desc_cn;

	private string list1desc_cn;

	private string list2desc_cn;

	private string list3desc_cn;

	private string btn4_cn;

	private string btn3_cn;

	public DialogResult result;

	private IContainer components;

	private PageHeader pageHeader1;

	private FlowPanel flowPanel1;

	private Label label1;

	private PageHeader pageHeader2;

	private Button button4;

	private Label label4;

	private Label label3;

	private Label label2;

	private PictureBox pictureBox3;

	private PictureBox pictureBox2;

	private PictureBox pictureBox1;

	private Button button1;

	private Label label5;

	private GridPanel gridPanel1;

	private Label lab_title;

	public StartUpgAscentModalFrm()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		title_en = "Kind tips";
		desc_en = "During the upgrade, the device may experience the following: the status indicator light may flash abnormally or it may restart on its own. This is normal. Please wait patiently for the firmware upgrade to complete.";
		list1desc_en = "Keep your computer connected to the Internet";
		list2desc_en = "Keep USB devices connected";
		list3desc_en = "The device is powered and has sufficient charge";
		btn4_en = "Upgrade";
		btn3_en = "Cancel";
		title_cn = "温馨提示";
		desc_cn = "升级过程中，设备可能会出现以下情况：状态指示灯异常闪烁或自行重启。以上均为正常现象。请耐心等待固件升级完成。";
		list1desc_cn = "保持电脑连接网络";
		list2desc_cn = "保持USB连接设备";
		list3desc_cn = "设备已供电且电量充足";
		btn4_cn = "开始升级";
		btn3_cn = "取消";
		result = (DialogResult)2;
		components = null;
		((Form)this)._002Ector();
		InitializeComponent();
	}

	private void ModalFrm_Load(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		result = (DialogResult)2;
		ReloadFont();
		ReloadLang();
	}

	private void hyperlinkLabel1_LinkClicked(object sender, LinkClickedEventArgs e)
	{
	}

	private void ReloadFont()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			Font font = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 16f);
			((Control)lab_title).Font = font;
			Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 14f);
			Label obj = label2;
			Label obj2 = label3;
			Label obj3 = label4;
			Label obj4 = label1;
			Font val2 = (((Control)button4).Font = val);
			Font val4 = (((Control)obj4).Font = val2);
			Font val6 = (((Control)obj3).Font = val4);
			Font font2 = (((Control)obj2).Font = val6);
			((Control)obj).Font = font2;
		});
	}

	private void ReloadLang()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			switch ((LangType)GD.Inst.CurrLang)
			{
			case LangType.zh_CN:
				((Control)lab_title).Text = title_cn;
				((Control)label1).Text = desc_cn;
				((Control)label2).Text = list1desc_cn;
				((Control)label3).Text = list2desc_cn;
				((Control)label4).Text = list3desc_cn;
				((Control)button4).Text = btn4_cn;
				((Control)button1).Text = btn3_cn;
				break;
			case LangType.en_US:
				((Control)lab_title).Text = title_en;
				((Control)label1).Text = desc_en;
				((Control)label2).Text = list1desc_en;
				((Control)label3).Text = list2desc_en;
				((Control)label4).Text = list3desc_en;
				((Control)button4).Text = btn4_en;
				((Control)button1).Text = btn3_en;
				break;
			}
		});
	}

	private void button4_Click(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		result = (DialogResult)1;
		((Form)this).Close();
	}

	private void button1_Click(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		result = (DialogResult)2;
		((Form)this).Close();
	}

	private void ModalFrm_FormClosing(object sender, FormClosingEventArgs e)
	{
		((Component)this).Dispose();
		GC.Collect();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
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
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Expected O, but got Unknown
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Expected O, but got Unknown
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Expected O, but got Unknown
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Expected O, but got Unknown
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Expected O, but got Unknown
		//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd5: Expected O, but got Unknown
		//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5d: Expected O, but got Unknown
		//IL_0e6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eba: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(StartUpgAscentModalFrm));
		pageHeader1 = new PageHeader();
		flowPanel1 = new FlowPanel();
		label4 = new Label();
		label3 = new Label();
		label2 = new Label();
		pictureBox3 = new PictureBox();
		pictureBox2 = new PictureBox();
		pictureBox1 = new PictureBox();
		label1 = new Label();
		pageHeader2 = new PageHeader();
		button1 = new Button();
		label5 = new Label();
		button4 = new Button();
		gridPanel1 = new GridPanel();
		lab_title = new Label();
		((Control)pageHeader1).SuspendLayout();
		((Control)flowPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox3).BeginInit();
		((ISupportInitialize)pictureBox2).BeginInit();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)pageHeader2).SuspendLayout();
		((Control)gridPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)pageHeader1).BackColor = Color.FromArgb(46, 49, 51);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_title);
		pageHeader1.DividerShow = true;
		((Control)pageHeader1).Dock = (DockStyle)5;
		((Control)pageHeader1).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((IControl)pageHeader1).HandDragFolder = false;
		gridPanel1.SetIndex((Control)(object)pageHeader1, 1);
		((Control)pageHeader1).Location = new Point(5, 0);
		((Control)pageHeader1).Margin = new Padding(5, 0, 5, 0);
		pageHeader1.MaximizeBox = false;
		pageHeader1.MinimizeBox = false;
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Padding = new Padding(10, 5, 0, 0);
		((Control)pageHeader1).Size = new Size(640, 35);
		((Control)pageHeader1).TabIndex = 0;
		((Control)pageHeader1).Text = "";
		((Control)flowPanel1).BackColor = Color.Transparent;
		((Control)flowPanel1).Controls.Add((Control)(object)label4);
		((Control)flowPanel1).Controls.Add((Control)(object)label3);
		((Control)flowPanel1).Controls.Add((Control)(object)label2);
		((Control)flowPanel1).Controls.Add((Control)(object)pictureBox3);
		((Control)flowPanel1).Controls.Add((Control)(object)pictureBox2);
		((Control)flowPanel1).Controls.Add((Control)(object)pictureBox1);
		((Control)flowPanel1).Controls.Add((Control)(object)label1);
		gridPanel1.SetIndex((Control)(object)flowPanel1, 2);
		((Control)flowPanel1).Location = new Point(0, 35);
		((Control)flowPanel1).Margin = new Padding(0);
		((Control)flowPanel1).Name = "flowPanel1";
		((Control)flowPanel1).Size = new Size(650, 400);
		((Control)flowPanel1).TabIndex = 1;
		((Control)flowPanel1).Text = "flowPanel1";
		((Control)label4).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label4.ForeColor = Color.FromArgb(164, 164, 165);
		flowPanel1.SetIndex((Control)(object)label4, 6);
		((Control)label4).Location = new Point(85, 344);
		((Control)label4).Margin = new Padding(5, 5, 0, 0);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(548, 57);
		label4.SuffixSvg = "";
		((Control)label4).TabIndex = 11;
		((Control)label4).Text = "The device is powered and has sufficient charge";
		((Control)label3).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label3.ForeColor = Color.FromArgb(164, 164, 165);
		flowPanel1.SetIndex((Control)(object)label3, 4);
		((Control)label3).Location = new Point(85, 277);
		((Control)label3).Margin = new Padding(5, 5, 0, 0);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(554, 57);
		label3.SuffixSvg = "";
		((Control)label3).TabIndex = 10;
		((Control)label3).Text = "Keep USB devices connected";
		((Control)label2).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label2.ForeColor = Color.FromArgb(164, 164, 165);
		flowPanel1.SetIndex((Control)(object)label2, 2);
		((Control)label2).Location = new Point(85, 210);
		((Control)label2).Margin = new Padding(5, 5, 0, 0);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(553, 57);
		label2.SuffixSvg = "";
		((Control)label2).TabIndex = 9;
		((Control)label2).Text = "Keep your computer connected to the Internet";
		pictureBox3.Image = (Image)(object)Resources.电池;
		flowPanel1.SetIndex((Control)(object)pictureBox3, 5);
		((Control)pictureBox3).Location = new Point(21, 349);
		((Control)pictureBox3).Margin = new Padding(10, 10, 5, 0);
		((Control)pictureBox3).Name = "pictureBox3";
		((Control)pictureBox3).Size = new Size(54, 47);
		pictureBox3.SizeMode = (PictureBoxSizeMode)4;
		pictureBox3.TabIndex = 8;
		pictureBox3.TabStop = false;
		pictureBox2.Image = (Image)(object)Resources.USB连接;
		flowPanel1.SetIndex((Control)(object)pictureBox2, 3);
		((Control)pictureBox2).Location = new Point(21, 282);
		((Control)pictureBox2).Margin = new Padding(10, 10, 5, 0);
		((Control)pictureBox2).Name = "pictureBox2";
		((Control)pictureBox2).Size = new Size(54, 47);
		pictureBox2.SizeMode = (PictureBoxSizeMode)4;
		pictureBox2.TabIndex = 7;
		pictureBox2.TabStop = false;
		pictureBox1.Image = (Image)(object)Resources.网络;
		flowPanel1.SetIndex((Control)(object)pictureBox1, 1);
		((Control)pictureBox1).Location = new Point(21, 215);
		((Control)pictureBox1).Margin = new Padding(10, 10, 5, 10);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(54, 47);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 6;
		pictureBox1.TabStop = false;
		label1.AutoEllipsis = true;
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label1.ForeColor = Color.White;
		flowPanel1.SetIndex((Control)(object)label1, 0);
		((Control)label1).Location = new Point(12, 10);
		((Control)label1).Margin = new Padding(10, 10, 10, 0);
		((Control)label1).Name = "label1";
		label1.ShowTooltip = false;
		((Control)label1).Size = new Size(626, 185);
		((Control)label1).TabIndex = 0;
		((Control)label1).Text = componentResourceManager.GetString("label1.Text");
		((Control)pageHeader2).BackColor = Color.Transparent;
		((Control)pageHeader2).Controls.Add((Control)(object)button1);
		((Control)pageHeader2).Controls.Add((Control)(object)label5);
		((Control)pageHeader2).Controls.Add((Control)(object)button4);
		((IControl)pageHeader2).HandCursor = Cursors.Default;
		gridPanel1.SetIndex((Control)(object)pageHeader2, 3);
		((Control)pageHeader2).Location = new Point(0, 435);
		((Control)pageHeader2).Margin = new Padding(0);
		((Control)pageHeader2).Name = "pageHeader2";
		((Control)pageHeader2).Padding = new Padding(0, 5, 10, 5);
		((Control)pageHeader2).Size = new Size(650, 65);
		((Control)pageHeader2).TabIndex = 7;
		((Control)pageHeader2).Text = "";
		button1.DefaultBack = Color.FromArgb(95, 95, 96);
		button1.DialogResult = (DialogResult)2;
		((Control)button1).Dock = (DockStyle)4;
		((Control)button1).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button1.ForeColor = Color.White;
		((IControl)button1).HandDragFolder = false;
		((Control)button1).Location = new Point(349, 5);
		((Control)button1).Margin = new Padding(0);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(123, 55);
		((Control)button1).TabIndex = 10;
		((Control)button1).TabStop = false;
		((Control)button1).Text = "取消";
		button1.WaveSize = 0;
		((Control)button1).Click += button1_Click;
		((Control)label5).Dock = (DockStyle)4;
		((Control)label5).Location = new Point(472, 5);
		((Control)label5).Margin = new Padding(0);
		((Control)label5).Name = "label5";
		((Control)label5).Size = new Size(45, 55);
		((Control)label5).TabIndex = 9;
		((Control)label5).Text = " ";
		button4.DefaultBack = Color.FromArgb(255, 233, 0);
		button4.DialogResult = (DialogResult)1;
		((Control)button4).Dock = (DockStyle)4;
		((Control)button4).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button4.ForeColor = Color.FromArgb(35, 35, 35);
		((IControl)button4).HandDragFolder = false;
		((Control)button4).Location = new Point(517, 5);
		((Control)button4).Margin = new Padding(0);
		((Control)button4).Name = "button4";
		((Control)button4).Size = new Size(123, 55);
		((Control)button4).TabIndex = 6;
		((Control)button4).TabStop = false;
		((Control)button4).Text = "开始升级";
		button4.WaveSize = 0;
		((Control)button4).Click += button4_Click;
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader1);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader2);
		((Control)gridPanel1).Controls.Add((Control)(object)flowPanel1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(650, 500);
		gridPanel1.Span = "100%;100%;100%;-7% 80% 13%";
		((Control)gridPanel1).TabIndex = 8;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)lab_title).Dock = (DockStyle)3;
		((Control)lab_title).ForeColor = Color.White;
		((Control)lab_title).Location = new Point(10, 5);
		((Control)lab_title).Name = "lab_title";
		((Control)lab_title).Size = new Size(162, 30);
		((Control)lab_title).TabIndex = 0;
		((Control)lab_title).Text = "label6";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(16f, 37f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Form)this).ClientSize = new Size(650, 500);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Font = new Font("阿里巴巴普惠体", 20.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).Margin = new Padding(8, 9, 8, 9);
		((Control)this).Name = "StartUpgAscentModalFrm";
		((Form)this).ShowIcon = false;
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "ModalFrm";
		((Form)this).FormClosing += new FormClosingEventHandler(ModalFrm_FormClosing);
		((Form)this).Load += ModalFrm_Load;
		((Control)pageHeader1).ResumeLayout(false);
		((Control)flowPanel1).ResumeLayout(false);
		((ISupportInitialize)pictureBox3).EndInit();
		((ISupportInitialize)pictureBox2).EndInit();
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)pageHeader2).ResumeLayout(false);
		((Control)gridPanel1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
