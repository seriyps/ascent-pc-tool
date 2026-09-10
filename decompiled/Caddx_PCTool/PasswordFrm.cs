using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AntdUI_Ex;

namespace Caddx_PCTool;

public class PasswordFrm : Form
{
	private IContainer components = null;

	private GridPanel gridPanel1;

	private PageHeader pageHeader2;

	private Button btn_Cancel;

	private Label label2;

	private Button btn_ok;

	private PageHeader pageHeader1;

	private Label lab_title;

	private GridPanel gridPanel2;

	private Label label1;

	private Input input1;

	public event EventHandler<FrmEventArgs> OnPasswordFrmEvent;

	public PasswordFrm()
	{
		InitializeComponent();
	}

	private void btn_ok_Click(object sender, EventArgs e)
	{
		string text = ((Control)input1).Text.Trim().ToLower();
		UserLevel userLevel = UserLevel.op;
		string text2 = text;
		string text3 = text2;
		userLevel = ((text3 == "caddx905") ? UserLevel.caddx : ((text3 == "roi") ? UserLevel.customA : UserLevel.op));
		GD.Inst.CurrUserLevel = userLevel;
		((Form)this).DialogResult = (DialogResult)1;
		OnPasswordFrmEvent?.Invoke(this, new FrmEventArgs
		{
			CurrLevel = userLevel
		});
		((Form)this).Close();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		((Form)this).DialogResult = (DialogResult)2;
		((Form)this).Close();
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
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Expected O, but got Unknown
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Expected O, but got Unknown
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Expected O, but got Unknown
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Expected O, but got Unknown
		//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
		gridPanel1 = new GridPanel();
		gridPanel2 = new GridPanel();
		input1 = new Input();
		label1 = new Label();
		pageHeader2 = new PageHeader();
		btn_Cancel = new Button();
		label2 = new Label();
		btn_ok = new Button();
		pageHeader1 = new PageHeader();
		lab_title = new Label();
		((Control)gridPanel1).SuspendLayout();
		((Control)gridPanel2).SuspendLayout();
		((Control)pageHeader2).SuspendLayout();
		((Control)pageHeader1).SuspendLayout();
		((Control)this).SuspendLayout();
		((ContainerPanel)gridPanel1).Back = Color.Transparent;
		((Control)gridPanel1).BackColor = Color.Transparent;
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)gridPanel2);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader2);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((IControl)gridPanel1).HandCursor = Cursors.Default;
		((IControl)gridPanel1).HandDragFolder = false;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(514, 233);
		gridPanel1.Span = "100%;100%;100%;-23% 50% 27%";
		((Control)gridPanel1).TabIndex = 9;
		((Control)gridPanel1).TabStop = false;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)gridPanel2).Controls.Add((Control)(object)input1);
		((Control)gridPanel2).Controls.Add((Control)(object)label1);
		gridPanel1.SetIndex((Control)(object)gridPanel2, 2);
		((Control)gridPanel2).Location = new Point(3, 57);
		((Control)gridPanel2).Name = "gridPanel2";
		((Control)gridPanel2).Size = new Size(508, 110);
		gridPanel2.Span = "20% 80%;";
		((Control)gridPanel2).TabIndex = 8;
		((Control)gridPanel2).Text = "gridPanel2";
		input1.BorderActive = Color.FromArgb(255, 233, 0);
		input1.BorderColor = Color.FromArgb(66, 69, 71);
		input1.BorderHover = Color.FromArgb(255, 233, 0);
		input1.BorderWidth = 2f;
		((IControl)input1).ColorScheme = (TAMode)2;
		((Control)input1).Location = new Point(107, 15);
		((Control)input1).Margin = new Padding(5, 15, 5, 15);
		((Control)input1).Name = "input1";
		((Control)input1).Size = new Size(396, 80);
		((Control)input1).TabIndex = 1;
		input1.WaveSize = 0;
		((Control)label1).ForeColor = Color.White;
		((Control)label1).Location = new Point(3, 0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(96, 110);
		((Control)label1).TabIndex = 0;
		((Control)label1).Text = "Password:";
		label1.TextAlign = (ContentAlignment)64;
		((Control)pageHeader2).BackColor = Color.Transparent;
		((Control)pageHeader2).Controls.Add((Control)(object)btn_Cancel);
		((Control)pageHeader2).Controls.Add((Control)(object)label2);
		((Control)pageHeader2).Controls.Add((Control)(object)btn_ok);
		((Control)pageHeader2).Dock = (DockStyle)5;
		((IControl)pageHeader2).HandCursor = Cursors.Default;
		gridPanel1.SetIndex((Control)(object)pageHeader2, 3);
		((Control)pageHeader2).Location = new Point(8, 178);
		((Control)pageHeader2).Margin = new Padding(8);
		((Control)pageHeader2).Name = "pageHeader2";
		((Control)pageHeader2).Size = new Size(498, 47);
		((Control)pageHeader2).TabIndex = 7;
		((Control)pageHeader2).Text = "";
		btn_Cancel.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_Cancel).Dock = (DockStyle)4;
		((Control)btn_Cancel).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_Cancel.ForeColor = Color.White;
		((Control)btn_Cancel).Location = new Point(232, 0);
		((Control)btn_Cancel).Margin = new Padding(0);
		((Control)btn_Cancel).Name = "btn_Cancel";
		((Control)btn_Cancel).Size = new Size(123, 47);
		((Control)btn_Cancel).TabIndex = 9;
		((Control)btn_Cancel).Text = "Cancel";
		((Control)btn_Cancel).Click += btn_Cancel_Click;
		((Control)label2).Dock = (DockStyle)4;
		((Control)label2).Location = new Point(355, 0);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(20, 47);
		((Control)label2).TabIndex = 8;
		btn_ok.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_ok).Dock = (DockStyle)4;
		((Control)btn_ok).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_ok.ForeColor = Color.FromArgb(35, 35, 35);
		((Control)btn_ok).Location = new Point(375, 0);
		((Control)btn_ok).Margin = new Padding(0);
		((Control)btn_ok).Name = "btn_ok";
		((Control)btn_ok).Size = new Size(123, 47);
		((Control)btn_ok).TabIndex = 6;
		((Control)btn_ok).Text = "Confirm";
		((Control)btn_ok).Click += btn_ok_Click;
		((Control)pageHeader1).BackColor = Color.Transparent;
		((Control)pageHeader1).Controls.Add((Control)(object)lab_title);
		pageHeader1.DividerShow = true;
		((Control)pageHeader1).Dock = (DockStyle)5;
		((Control)pageHeader1).Font = new Font("Microsoft Sans Serif", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((IControl)pageHeader1).HandDragFolder = false;
		gridPanel1.SetIndex((Control)(object)pageHeader1, 1);
		((Control)pageHeader1).Location = new Point(5, 5);
		((Control)pageHeader1).Margin = new Padding(5);
		pageHeader1.MaximizeBox = false;
		pageHeader1.MinimizeBox = false;
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Size = new Size(504, 44);
		((Control)pageHeader1).TabIndex = 0;
		((Control)pageHeader1).Text = "";
		((Control)lab_title).BackColor = Color.Transparent;
		((Control)lab_title).Dock = (DockStyle)3;
		((Control)lab_title).Font = new Font("Microsoft Sans Serif", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_title).ForeColor = Color.White;
		((Control)lab_title).Location = new Point(0, 0);
		((Control)lab_title).Margin = new Padding(10);
		((Control)lab_title).Name = "lab_title";
		((Control)lab_title).Size = new Size(494, 44);
		((Control)lab_title).TabIndex = 1;
		((Control)lab_title).Text = "Industry Enter";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Form)this).ClientSize = new Size(514, 233);
		((Form)this).ControlBox = false;
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).DoubleBuffered = true;
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Control)this).Name = "PasswordFrm";
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "Form1";
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel2).ResumeLayout(false);
		((Control)pageHeader2).ResumeLayout(false);
		((Control)pageHeader1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
