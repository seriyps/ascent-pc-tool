using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI_Ex;

namespace Caddx_PCTool;

public class CommModalFrm : Window, IDisposable
{
	public string _title;

	public string _desc;

	public string _btnok;

	public string _btncan;

	private string _releaseNotes;

	private Color _color;

	private Color _btnBackColor;

	public DialogResult FrmResult;

	private IContainer components;

	private PageHeader pageHeader1;

	private PageHeader pageHeader2;

	private Button btn_ok;

	private GridPanel gridPanel1;

	private Label label1;

	private RichTextBox releaseNotesBox;

	private Button btn_Cancel;

	private Label label2;

	private StackPanel stackPanel1;

	private Progress progress1;

	public PointF ParentsLocation { get; set; }

	public CommModalFrm()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		_title = string.Empty;
		_desc = string.Empty;
		_btnok = string.Empty;
		_btncan = string.Empty;
		_releaseNotes = string.Empty;
		_color = Color.FromArgb(153, 153, 153);
		_btnBackColor = Color.FromArgb(33, 150, 243);
		FrmResult = (DialogResult)2;
		components = null;
		((Window)this)._002Ector();
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
			((BaseForm)this).Invoke((Action)delegate
			{
				((Control)pageHeader1).Text = _title;
				((Control)label1).Text = _desc;
				if (!string.IsNullOrWhiteSpace(_releaseNotes))
				{
					((Control)releaseNotesBox).Text = _desc + Environment.NewLine + Environment.NewLine + _releaseNotes;
					((Control)label1).Visible = false;
					((Control)releaseNotesBox).Visible = true;
				}
				else
				{
					((Control)label1).Visible = true;
					((Control)releaseNotesBox).Visible = false;
				}
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
					Color color = (((Control)pageHeader1).BackColor = _color);
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
		_btnok = Lang.T("common.btn_confirm");
		((IControl)btn_Cancel).Visible = false;
		_color = color;
	}

	public void SetAllTxt(string title, string desc, bool isshowBtnOK = true, bool isshowBtnCan = true)
	{
		_title = title;
		_desc = desc;
		_btnok = Lang.T("common.btn_confirm");
		_btncan = Lang.T("common.btn_cancel");
		((IControl)btn_ok).Visible = isshowBtnOK;
		((IControl)btn_Cancel).Visible = isshowBtnCan;
	}

	public void SetReleaseNotes(string releaseNotes)
	{
		_releaseNotes = releaseNotes ?? string.Empty;
	}

	public void ShowProgressBar()
	{
		if (((Control)this).InvokeRequired)
		{
			((BaseForm)this).BeginInvoke((Action)ShowProgressBar);
			return;
		}
		Button obj = btn_ok;
		bool visible = (((IControl)btn_Cancel).Visible = false);
		((IControl)obj).Visible = visible;
		((Control)label1).Visible = false;
		((IControl)progress1).Visible = true;
		progress1.Value = 0f;
		progress1.State = (TType)0;
		progress1.Fill = Color.FromArgb(255, 233, 0);
		((Control)progress1).BringToFront();
	}

	public void SetProgress(int percent)
	{
		if (((Control)this).InvokeRequired)
		{
			((BaseForm)this).BeginInvoke((Action)delegate
			{
				SetProgress(percent);
			});
		}
		else
		{
			int num = Math.Max(0, Math.Min(100, percent));
			progress1.Value = (float)num / 100f;
			((Control)progress1).Text = $"{num}%";
		}
	}

	public void HideProgressBar()
	{
		if (((Control)this).InvokeRequired)
		{
			((BaseForm)this).BeginInvoke((Action)HideProgressBar);
			return;
		}
		((IControl)progress1).Visible = false;
		((Control)label1).Visible = true;
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
			Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
			Label obj = label1;
			RichTextBox obj2 = releaseNotesBox;
			Button obj3 = btn_ok;
			Font val2 = (((Control)btn_Cancel).Font = val);
			Font val4 = (((Control)obj3).Font = val2);
			Font font2 = (((Control)obj2).Font = val4);
			((Control)obj).Font = font2;
			((Control)progress1).Font = val;
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

	public void Dispose()
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
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Expected O, but got Unknown
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Expected O, but got Unknown
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Expected O, but got Unknown
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Expected O, but got Unknown
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Expected O, but got Unknown
		//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad4: Expected O, but got Unknown
		//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b31: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CommModalFrm));
		pageHeader1 = new PageHeader();
		pageHeader2 = new PageHeader();
		btn_Cancel = new Button();
		label2 = new Label();
		btn_ok = new Button();
		gridPanel1 = new GridPanel();
		label1 = new Label();
		releaseNotesBox = new RichTextBox();
		stackPanel1 = new StackPanel();
		progress1 = new Progress();
		((Control)pageHeader2).SuspendLayout();
		((Control)gridPanel1).SuspendLayout();
		((Control)stackPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)pageHeader1).BackColor = Color.Transparent;
		((IControl)pageHeader1).ColorScheme = (TAMode)2;
		pageHeader1.DividerShow = true;
		((Control)pageHeader1).Dock = (DockStyle)5;
		((Control)pageHeader1).Font = new Font("思源黑体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)128);
		((IControl)pageHeader1).HandDragFolder = false;
		gridPanel1.SetIndex((Control)(object)pageHeader1, 1);
		((Control)pageHeader1).Location = new Point(0, 0);
		((Control)pageHeader1).Margin = new Padding(0);
		pageHeader1.MaximizeBox = false;
		pageHeader1.MinimizeBox = false;
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Size = new Size(521, 35);
		((Control)pageHeader1).TabIndex = 0;
		((Control)pageHeader1).Text = "提示";
		((Control)pageHeader2).BackColor = Color.Transparent;
		((Control)pageHeader2).Controls.Add((Control)(object)btn_Cancel);
		((Control)pageHeader2).Controls.Add((Control)(object)label2);
		((Control)pageHeader2).Controls.Add((Control)(object)btn_ok);
		((Control)pageHeader2).Dock = (DockStyle)5;
		((IControl)pageHeader2).HandCursor = Cursors.Default;
		gridPanel1.SetIndex((Control)(object)pageHeader2, 4);
		((Control)pageHeader2).Location = new Point(8, 274);
		((Control)pageHeader2).Margin = new Padding(8);
		((Control)pageHeader2).Name = "pageHeader2";
		((Control)pageHeader2).Size = new Size(505, 35);
		((Control)pageHeader2).TabIndex = 7;
		((Control)pageHeader2).Text = "";
		btn_Cancel.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_Cancel).Dock = (DockStyle)4;
		((Control)btn_Cancel).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_Cancel.ForeColor = Color.White;
		((Control)btn_Cancel).Location = new Point(239, 0);
		((Control)btn_Cancel).Margin = new Padding(0);
		((Control)btn_Cancel).Name = "btn_Cancel";
		((Control)btn_Cancel).Size = new Size(123, 35);
		((Control)btn_Cancel).TabIndex = 9;
		((Control)btn_Cancel).Text = "取消";
		btn_Cancel.WaveSize = 0;
		((Control)btn_Cancel).Click += btn_cancel_Click;
		((Control)label2).Dock = (DockStyle)4;
		((Control)label2).Location = new Point(362, 0);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(20, 35);
		((Control)label2).TabIndex = 8;
		btn_ok.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_ok).Dock = (DockStyle)4;
		((Control)btn_ok).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_ok.ForeColor = Color.FromArgb(35, 35, 35);
		((Control)btn_ok).Location = new Point(382, 0);
		((Control)btn_ok).Margin = new Padding(0);
		((Control)btn_ok).Name = "btn_ok";
		((Control)btn_ok).Size = new Size(123, 35);
		((Control)btn_ok).TabIndex = 6;
		((Control)btn_ok).Text = "确认";
		btn_ok.WaveSize = 0;
		((Control)btn_ok).Click += btn_ok_Click;
		((ContainerPanel)gridPanel1).Back = Color.Transparent;
		((Control)gridPanel1).BackColor = Color.Transparent;
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 69, 71);
		((Control)gridPanel1).Controls.Add((Control)(object)stackPanel1);
		((Control)gridPanel1).Controls.Add((Control)(object)releaseNotesBox);
		((Control)gridPanel1).Controls.Add((Control)(object)label1);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader2);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((IControl)gridPanel1).HandCursor = Cursors.Default;
		((IControl)gridPanel1).HandDragFolder = false;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(521, 317);
		gridPanel1.Span = "100%;100%;100%;100%;-35 67% 15% 18%";
		((Control)gridPanel1).TabIndex = 8;
		((Control)gridPanel1).TabStop = false;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)label1).AutoSize = true;
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Dock = (DockStyle)5;
		((Control)label1).Font = new Font("思源黑体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)label1).ForeColor = Color.White;
		gridPanel1.SetIndex((Control)(object)label1, 2);
		((Control)label1).Location = new Point(10, 45);
		((Control)label1).Margin = new Padding(10);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(501, 169);
		((Control)label1).TabIndex = 0;
		((Control)label1).Text = "是否确认断开设备连接？";
		label1.TextAlign = (ContentAlignment)16;
		((Control)releaseNotesBox).BackColor = Color.FromArgb(46, 49, 51);
		((TextBoxBase)releaseNotesBox).BorderStyle = (BorderStyle)0;
		releaseNotesBox.DetectUrls = false;
		((Control)releaseNotesBox).Dock = (DockStyle)5;
		((Control)releaseNotesBox).ForeColor = Color.White;
		gridPanel1.SetIndex((Control)(object)releaseNotesBox, 2);
		((Control)releaseNotesBox).Location = new Point(10, 45);
		((Control)releaseNotesBox).Margin = new Padding(10);
		((Control)releaseNotesBox).Name = "releaseNotesBox";
		((TextBoxBase)releaseNotesBox).ReadOnly = true;
		releaseNotesBox.ScrollBars = (RichTextBoxScrollBars)2;
		((Control)releaseNotesBox).Size = new Size(501, 169);
		((Control)releaseNotesBox).TabIndex = 12;
		((Control)releaseNotesBox).TabStop = false;
		((Control)releaseNotesBox).Text = "";
		((Control)releaseNotesBox).Visible = false;
		((TextBoxBase)releaseNotesBox).WordWrap = true;
		stackPanel1.Controls.Add((Control)(object)progress1);
		gridPanel1.SetIndex((Control)(object)stackPanel1, 3);
		((Control)stackPanel1).Location = new Point(3, 227);
		((Control)stackPanel1).Name = "stackPanel1";
		((Control)stackPanel1).Size = new Size(515, 36);
		((Control)stackPanel1).TabIndex = 8;
		((Control)stackPanel1).Text = "stackPanel1";
		progress1.Back = Color.FromArgb(99, 101, 103);
		((Control)progress1).BackColor = Color.FromArgb(33, 36, 39);
		((IControl)progress1).ColorScheme = (TAMode)2;
		((Control)progress1).Dock = (DockStyle)5;
		progress1.Fill = Color.FromArgb(255, 233, 0);
		((Control)progress1).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		progress1.ForeColor = Color.White;
		((IControl)progress1).HandCursor = Cursors.Default;
		((Control)progress1).Location = new Point(10, 10);
		((Control)progress1).Margin = new Padding(10);
		((Control)progress1).Name = "progress1";
		((Control)progress1).Size = new Size(501, 16);
		((Control)progress1).TabIndex = 11;
		((Control)progress1).TabStop = false;
		((Control)progress1).Text = "";
		progress1.ValueRatio = 1f;
		((IControl)progress1).Visible = false;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(16f, 31f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Window)this).ClientSize = new Size(521, 317);
		((Form)this).ControlBox = false;
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((BaseForm)this).Dark = true;
		((Control)this).Font = new Font("Microsoft Sans Serif", 20.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((BaseForm)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
		((Form)this).Margin = new Padding(8, 9, 8, 9);
		((BaseForm)this).Mode = (TAMode)2;
		((Control)this).Name = "CommModalFrm";
		((Window)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "ModalFrm";
		((Form)this).TopMost = true;
		((Form)this).FormClosing += new FormClosingEventHandler(ModalFrm_FormClosing);
		((Form)this).Load += ModalFrm_Load;
		((Control)pageHeader2).ResumeLayout(false);
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel1).PerformLayout();
		((Control)stackPanel1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
