using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class StartUpgAscentModalFrm : Window
{
	public DialogResult result;

	private IContainer components;

	private PageHeader pageHeader1;

	private PageHeader pageHeader2;

	private Button button4;

	private Button button1;

	private Label label5;

	private GridPanel gridPanel1;

	private Label lab_title;

	private GridPanel_Ex gridPanel_Ex1;

	private Label label4;

	private PictureBox pictureBox3;

	private Label label3;

	private PictureBox pictureBox2;

	private Label label2;

	private PictureBox pictureBox1;

	private Label label1;

	public StartUpgAscentModalFrm()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
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
		ReloadLang();
	}

	private void ReloadFont()
	{
		((BaseForm)this).Invoke((Action)delegate
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
		((BaseForm)this).Invoke((Action)delegate
		{
			((Control)lab_title).Text = Lang.T("startup_modal.title");
			((Control)label1).Text = Lang.T("startup_modal.desc");
			((Control)label2).Text = Lang.T("startup_modal.keep_network");
			((Control)label3).Text = Lang.T("startup_modal.keep_usb");
			((Control)label4).Text = Lang.T("startup_modal.keep_power");
			((Control)button4).Text = Lang.T("startup_modal.btn_upgrade");
			((Control)button1).Text = Lang.T("startup_modal.btn_cancel");
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
		((Window)this).Dispose(disposing);
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
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Expected O, but got Unknown
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Expected O, but got Unknown
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Expected O, but got Unknown
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0d: Expected O, but got Unknown
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b67: Expected O, but got Unknown
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd9: Expected O, but got Unknown
		//IL_0d10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfe: Expected O, but got Unknown
		//IL_0e0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5b: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(StartUpgAscentModalFrm));
		pageHeader1 = new PageHeader();
		lab_title = new Label();
		pageHeader2 = new PageHeader();
		button1 = new Button();
		label5 = new Label();
		button4 = new Button();
		gridPanel1 = new GridPanel();
		gridPanel_Ex1 = new GridPanel_Ex();
		label4 = new Label();
		pictureBox3 = new PictureBox();
		label3 = new Label();
		pictureBox2 = new PictureBox();
		label2 = new Label();
		pictureBox1 = new PictureBox();
		label1 = new Label();
		((Control)pageHeader1).SuspendLayout();
		((Control)pageHeader2).SuspendLayout();
		((Control)gridPanel1).SuspendLayout();
		((Control)gridPanel_Ex1).SuspendLayout();
		((ISupportInitialize)pictureBox3).BeginInit();
		((ISupportInitialize)pictureBox2).BeginInit();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((Control)pageHeader1).BackColor = Color.FromArgb(46, 49, 51);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_title);
		pageHeader1.DividerShow = true;
		((Control)pageHeader1).Dock = (DockStyle)5;
		((Control)pageHeader1).Font = new Font("Microsoft Sans Serif", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((IControl)pageHeader1).HandDragFolder = false;
		gridPanel1.SetIndex((Control)(object)pageHeader1, 1);
		((Control)pageHeader1).Location = new Point(5, 0);
		((Control)pageHeader1).Margin = new Padding(5, 0, 5, 0);
		pageHeader1.MaximizeBox = false;
		pageHeader1.MinimizeBox = false;
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Padding = new Padding(10, 5, 0, 0);
		((Control)pageHeader1).Size = new Size(640, 50);
		((Control)pageHeader1).TabIndex = 0;
		((Control)pageHeader1).Text = "";
		((Control)lab_title).Dock = (DockStyle)3;
		((Control)lab_title).ForeColor = Color.White;
		((Control)lab_title).Location = new Point(10, 5);
		((Control)lab_title).Name = "lab_title";
		((Control)lab_title).Size = new Size(162, 45);
		((Control)lab_title).TabIndex = 0;
		((Control)lab_title).Text = "label6";
		((Control)pageHeader2).BackColor = Color.Transparent;
		((Control)pageHeader2).Controls.Add((Control)(object)button1);
		((Control)pageHeader2).Controls.Add((Control)(object)label5);
		((Control)pageHeader2).Controls.Add((Control)(object)button4);
		((IControl)pageHeader2).HandCursor = Cursors.Default;
		gridPanel1.SetIndex((Control)(object)pageHeader2, 3);
		((Control)pageHeader2).Location = new Point(0, 450);
		((Control)pageHeader2).Margin = new Padding(0);
		((Control)pageHeader2).Name = "pageHeader2";
		((Control)pageHeader2).Padding = new Padding(0, 5, 10, 5);
		((Control)pageHeader2).Size = new Size(650, 50);
		((Control)pageHeader2).TabIndex = 7;
		((Control)pageHeader2).Text = "";
		button1.DefaultBack = Color.FromArgb(95, 95, 96);
		button1.DialogResult = (DialogResult)2;
		((Control)button1).Dock = (DockStyle)4;
		((Control)button1).Font = new Font("Microsoft Sans Serif", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button1.ForeColor = Color.White;
		((IControl)button1).HandDragFolder = false;
		((Control)button1).Location = new Point(404, 5);
		((Control)button1).Margin = new Padding(0);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(101, 40);
		((Control)button1).TabIndex = 10;
		((Control)button1).TabStop = false;
		((Control)button1).Text = "取消";
		button1.WaveSize = 0;
		((Control)button1).Click += button1_Click;
		((Control)label5).Dock = (DockStyle)4;
		((Control)label5).Location = new Point(505, 5);
		((Control)label5).Margin = new Padding(0);
		((Control)label5).Name = "label5";
		((Control)label5).Size = new Size(30, 40);
		((Control)label5).TabIndex = 9;
		((Control)label5).Text = " ";
		button4.DefaultBack = Color.FromArgb(255, 233, 0);
		button4.DialogResult = (DialogResult)1;
		((Control)button4).Dock = (DockStyle)4;
		((Control)button4).Font = new Font("Microsoft Sans Serif", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button4.ForeColor = Color.FromArgb(35, 35, 35);
		((IControl)button4).HandDragFolder = false;
		((Control)button4).Location = new Point(535, 5);
		((Control)button4).Margin = new Padding(0);
		((Control)button4).Name = "button4";
		((Control)button4).Size = new Size(105, 40);
		((Control)button4).TabIndex = 6;
		((Control)button4).TabStop = false;
		((Control)button4).Text = "开始升级";
		button4.WaveSize = 0;
		((Control)button4).Click += button4_Click;
		((Control)gridPanel1).Controls.Add((Control)(object)gridPanel_Ex1);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader1);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader2);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(650, 500);
		gridPanel1.Span = "100%;100%;100%;-10% 80% 10%";
		((Control)gridPanel1).TabIndex = 8;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)gridPanel_Ex1).Controls.Add((Control)(object)label4);
		((Control)gridPanel_Ex1).Controls.Add((Control)(object)pictureBox3);
		((Control)gridPanel_Ex1).Controls.Add((Control)(object)label3);
		((Control)gridPanel_Ex1).Controls.Add((Control)(object)pictureBox2);
		((Control)gridPanel_Ex1).Controls.Add((Control)(object)label2);
		((Control)gridPanel_Ex1).Controls.Add((Control)(object)pictureBox1);
		((Control)gridPanel_Ex1).Controls.Add((Control)(object)label1);
		((Control)gridPanel_Ex1).Dock = (DockStyle)5;
		gridPanel1.SetIndex((Control)(object)gridPanel_Ex1, 2);
		((Control)gridPanel_Ex1).Location = new Point(15, 65);
		((Control)gridPanel_Ex1).Margin = new Padding(15);
		((Control)gridPanel_Ex1).Name = "gridPanel_Ex1";
		((Control)gridPanel_Ex1).Size = new Size(620, 370);
		gridPanel_Ex1.Span = "100%;10% 90%;10% 90%;10% 90%;100%;\r\n-50% 10% 10% 10% 20%";
		((Control)gridPanel_Ex1).TabIndex = 13;
		((Control)gridPanel_Ex1).TabStop = false;
		((Control)gridPanel_Ex1).Text = "gridPanel_Ex1";
		((Control)label4).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label4.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)label4).Location = new Point(62, 259);
		((Control)label4).Margin = new Padding(5, 5, 0, 0);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(558, 37);
		label4.SuffixSvg = "";
		((Control)label4).TabIndex = 14;
		((Control)label4).Text = "The device is powered and has sufficient charge";
		pictureBox3.Image = (Image)(object)Resources.电池;
		((Control)pictureBox3).Location = new Point(0, 259);
		((Control)pictureBox3).Margin = new Padding(10, 10, 5, 0);
		((Control)pictureBox3).Name = "pictureBox3";
		((Control)pictureBox3).Size = new Size(62, 37);
		pictureBox3.SizeMode = (PictureBoxSizeMode)4;
		pictureBox3.TabIndex = 13;
		pictureBox3.TabStop = false;
		((Control)label3).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label3.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)label3).Location = new Point(62, 222);
		((Control)label3).Margin = new Padding(5, 5, 0, 0);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(558, 37);
		label3.SuffixSvg = "";
		((Control)label3).TabIndex = 12;
		((Control)label3).Text = "Keep USB devices connected";
		pictureBox2.Image = (Image)(object)Resources.USB连接;
		((Control)pictureBox2).Location = new Point(0, 222);
		((Control)pictureBox2).Margin = new Padding(10, 10, 5, 0);
		((Control)pictureBox2).Name = "pictureBox2";
		((Control)pictureBox2).Size = new Size(62, 37);
		pictureBox2.SizeMode = (PictureBoxSizeMode)4;
		pictureBox2.TabIndex = 11;
		pictureBox2.TabStop = false;
		((Control)label2).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label2.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)label2).Location = new Point(62, 185);
		((Control)label2).Margin = new Padding(5, 5, 0, 0);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(558, 37);
		label2.SuffixSvg = "";
		((Control)label2).TabIndex = 10;
		((Control)label2).Text = "Keep your computer connected to the Internet";
		pictureBox1.Image = (Image)(object)Resources.网络;
		((Control)pictureBox1).Location = new Point(0, 185);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(62, 37);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 7;
		pictureBox1.TabStop = false;
		label1.AutoEllipsis = true;
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label1.ForeColor = Color.White;
		((Control)label1).Location = new Point(0, 0);
		((Control)label1).Margin = new Padding(10, 10, 10, 0);
		((Control)label1).Name = "label1";
		((Control)label1).Padding = new Padding(5);
		label1.ShowTooltip = false;
		((Control)label1).Size = new Size(620, 185);
		((Control)label1).TabIndex = 1;
		((Control)label1).Text = componentResourceManager.GetString("label1.Text");
		((ContainerControl)this).AutoScaleDimensions = new SizeF(16f, 31f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Window)this).ClientSize = new Size(650, 500);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Font = new Font("Microsoft Sans Serif", 20.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((BaseForm)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).Margin = new Padding(8, 9, 8, 9);
		((Control)this).Name = "StartUpgAscentModalFrm";
		((Form)this).ShowIcon = false;
		((Window)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "ModalFrm";
		((Form)this).FormClosing += new FormClosingEventHandler(ModalFrm_FormClosing);
		((Form)this).Load += ModalFrm_Load;
		((Control)pageHeader1).ResumeLayout(false);
		((Control)pageHeader2).ResumeLayout(false);
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel_Ex1).ResumeLayout(false);
		((ISupportInitialize)pictureBox3).EndInit();
		((ISupportInitialize)pictureBox2).EndInit();
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
