using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI;

namespace Caddx_PCTool;

public class CommModalFrm : Form
{
	public string _title;

	public string _desc;

	public string _btnok;

	public string _btncan;

	private Color _color;

	private Color _btnBackColor;

	public DialogResult FrmResult;

	private IContainer components;

	private PageHeader pageHeader1;

	private PageHeader pageHeader2;

	private Button btn_ok;

	private GridPanel gridPanel1;

	private Label label1;

	private Label lab_title;

	private Button btn_Cancel;

	private Label label2;

	public PointF ParentsLocation { get; set; }

	public CommModalFrm()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		_title = "断开连接";
		_desc = "是否确认断开设备连接？";
		_btnok = "确认";
		_btncan = "取消";
		_color = Color.FromArgb(153, 153, 153);
		_btnBackColor = Color.FromArgb(33, 150, 243);
		FrmResult = (DialogResult)2;
		components = null;
		((Form)this)._002Ector();
		InitializeComponent();
	}

	private void ModalFrm_Load(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			((Form)this).TopMost = true;
			FrmResult = (DialogResult)2;
			ReloadFont();
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				((Control)lab_title).Text = _title;
				((Control)label1).Text = _desc;
				((Control)btn_ok).Text = _btnok;
				((Control)btn_Cancel).Text = _btncan;
				if (_color != Color.Red)
				{
					btn_ok.DefaultBack = Color.FromArgb(255, 233, 0);
					btn_ok.ForeColor = Color.FromArgb(35, 35, 35);
				}
				else
				{
					Button obj = btn_ok;
					Label obj2 = label1;
					Color color = (((Control)lab_title).BackColor = _color);
					Color value = (((Control)obj2).ForeColor = color);
					obj.DefaultBack = value;
				}
			});
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("ModalFrm_Load Err:" + ex.Message, Color.DarkRed);
		}
	}

	public void SetAllTxt(string title, string desc, string btnok, string btnCan)
	{
		_title = title;
		_desc = desc;
		_btnok = btnok;
		_btncan = btnCan;
		Button obj = btn_Cancel;
		bool visible = (((IControl)btn_ok).Visible = true);
		((IControl)obj).Visible = visible;
	}

	public void SetAllTxt(string title, string desc, string btnok)
	{
		_title = title;
		_desc = desc;
		_btnok = btnok;
		((IControl)btn_Cancel).Visible = false;
	}

	public void SetAllTxt(string title, string desc, Color color)
	{
		_title = title;
		_desc = desc;
		_btnok = ((GD.Inst.CurrLang == 1) ? "确认" : "Confirm");
		((IControl)btn_Cancel).Visible = false;
		_color = color;
	}

	public void SetAllTxt(string title, string desc, bool isshowBtnOK = true, bool isshowBtnCan = true)
	{
		_title = title;
		_desc = desc;
		_btnok = ((GD.Inst.CurrLang == 1) ? "确认" : "Confirm");
		_btncan = ((GD.Inst.CurrLang == 1) ? "取消" : "Cancel");
		((IControl)btn_ok).Visible = isshowBtnOK;
		((IControl)btn_Cancel).Visible = isshowBtnCan;
	}

	private void ReloadFont()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 16f);
			((Control)lab_title).Font = font;
			Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
			Label obj = label1;
			Button obj2 = btn_ok;
			Font val2 = (((Control)btn_Cancel).Font = val);
			Font font2 = (((Control)obj2).Font = val2);
			((Control)obj).Font = font2;
		});
	}

	private void btn_ok_Click(object sender, EventArgs e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((Form)this).TopMost = false;
		FrmResult = (DialogResult)1;
		((Form)this).Close();
	}

	private void ModalFrm_FormClosing(object sender, FormClosingEventArgs e)
	{
	}

	private void label1_Click(object sender, EventArgs e)
	{
	}

	private void btn_cancel_Click(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		FrmResult = (DialogResult)2;
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
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Expected O, but got Unknown
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Expected O, but got Unknown
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Expected O, but got Unknown
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Expected O, but got Unknown
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Expected O, but got Unknown
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CommModalFrm));
		pageHeader1 = new PageHeader();
		lab_title = new Label();
		pageHeader2 = new PageHeader();
		btn_Cancel = new Button();
		label2 = new Label();
		btn_ok = new Button();
		gridPanel1 = new GridPanel();
		label1 = new Label();
		((Control)pageHeader1).SuspendLayout();
		((Control)pageHeader2).SuspendLayout();
		((Control)gridPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)pageHeader1).BackColor = Color.Transparent;
		((Control)pageHeader1).Controls.Add((Control)(object)lab_title);
		pageHeader1.DividerShow = true;
		((Control)pageHeader1).Dock = (DockStyle)5;
		((Control)pageHeader1).Font = new Font("阿里巴巴普惠体", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
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
		((Control)lab_title).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_title).ForeColor = Color.White;
		((Control)lab_title).Location = new Point(0, 0);
		((Control)lab_title).Margin = new Padding(10);
		((Control)lab_title).Name = "lab_title";
		((Control)lab_title).Size = new Size(494, 44);
		((Control)lab_title).TabIndex = 1;
		((Control)lab_title).Text = "提示";
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
		((Control)btn_Cancel).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_Cancel.ForeColor = Color.White;
		((Control)btn_Cancel).Location = new Point(232, 0);
		((Control)btn_Cancel).Margin = new Padding(0);
		((Control)btn_Cancel).Name = "btn_Cancel";
		((Control)btn_Cancel).Size = new Size(123, 47);
		((Control)btn_Cancel).TabIndex = 9;
		((Control)btn_Cancel).Text = "取消";
		((Control)btn_Cancel).Click += btn_cancel_Click;
		((Control)label2).Dock = (DockStyle)4;
		((Control)label2).Location = new Point(355, 0);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(20, 47);
		((Control)label2).TabIndex = 8;
		btn_ok.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_ok).Dock = (DockStyle)4;
		((Control)btn_ok).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_ok.ForeColor = Color.FromArgb(35, 35, 35);
		((Control)btn_ok).Location = new Point(375, 0);
		((Control)btn_ok).Margin = new Padding(0);
		((Control)btn_ok).Name = "btn_ok";
		((Control)btn_ok).Size = new Size(123, 47);
		((Control)btn_ok).TabIndex = 6;
		((Control)btn_ok).Text = "确认";
		((Control)btn_ok).Click += btn_ok_Click;
		((ContainerPanel)gridPanel1).Back = Color.Transparent;
		((Control)gridPanel1).BackColor = Color.Transparent;
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 69, 71);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)label1);
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
		((Control)gridPanel1).TabIndex = 8;
		((Control)gridPanel1).TabStop = false;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)label1).AutoSize = true;
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Dock = (DockStyle)5;
		((Control)label1).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label1).ForeColor = Color.White;
		gridPanel1.SetIndex((Control)(object)label1, 2);
		((Control)label1).Location = new Point(10, 64);
		((Control)label1).Margin = new Padding(10);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(494, 96);
		((Control)label1).TabIndex = 0;
		((Control)label1).Text = "是否确认断开设备连接？";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(16f, 37f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Form)this).ClientSize = new Size(514, 233);
		((Form)this).ControlBox = false;
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Font = new Font("阿里巴巴普惠体", 20.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
		((Form)this).Margin = new Padding(8, 9, 8, 9);
		((Control)this).Name = "CommModalFrm";
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "ModalFrm";
		((Form)this).TopMost = true;
		((Form)this).FormClosing += new FormClosingEventHandler(ModalFrm_FormClosing);
		((Form)this).Load += ModalFrm_Load;
		((Control)pageHeader1).ResumeLayout(false);
		((Control)pageHeader2).ResumeLayout(false);
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel1).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
