using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class FunctionCard : UserControl
{
	private FuntionType _currType;

	public Action<string, FuntionType, object> OnFunctionCardClickEvent;

	private IContainer components = null;

	private GridPanel gridPanel1;

	private Label label1;

	private PictureBox pictureBox1;

	public FuntionType CurrFunType
	{
		get
		{
			return _currType;
		}
		set
		{
			_currType = value;
			SetCtrl();
		}
	}

	public FunctionCard()
	{
		InitializeComponent();
	}

	public void ReloadLang()
	{
		switch (_currType)
		{
		case FuntionType.unknown:
			((Control)label1).Text = Lang.T("fun_unknown");
			break;
		case FuntionType.upgrade:
			((Control)label1).Text = Lang.T("fun_upgrade");
			break;
		case FuntionType.gmSet:
			((Control)label1).Text = Lang.T("fun_gm_set");
			break;
		case FuntionType.camhub:
			((Control)label1).Text = Lang.T("fun_cam_hub");
			break;
		case FuntionType.rcMode:
			((Control)label1).Text = Lang.T("fun_rc_mode");
			break;
		case FuntionType.bb_freq:
			((Control)label1).Text = Lang.T("fun_update_channel");
			break;
		case FuntionType.findDevice:
			break;
		}
	}

	public FunctionCard(FuntionType type)
	{
		InitializeComponent();
		_currType = type;
		ReloadLang();
	}

	private void SetCtrl()
	{
		switch (_currType)
		{
		case FuntionType.unknown:
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			break;
		case FuntionType.upgrade:
			pictureBox1.Image = (Image)(object)Resources.Upgrade_icon;
			break;
		case FuntionType.gmSet:
			pictureBox1.Image = (Image)(object)Resources.GM_icon;
			break;
		case FuntionType.camhub:
			pictureBox1.Image = (Image)(object)Resources.Hub_icon;
			break;
		case FuntionType.rcMode:
			pictureBox1.Image = (Image)(object)Resources.RCMode_icon;
			break;
		case FuntionType.bb_freq:
			pictureBox1.Image = (Image)(object)Resources.UC_icon;
			break;
		}
		ReloadLang();
	}

	private void gridPanel1_MouseHover(object sender, EventArgs e)
	{
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(255, 233, 0);
	}

	private void gridPanel1_MouseLeave(object sender, EventArgs e)
	{
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 68, 70);
	}

	public void SetCardInfo(FuntionType type)
	{
		switch (type)
		{
		case FuntionType.unknown:
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
			break;
		case FuntionType.upgrade:
			pictureBox1.Image = (Image)(object)Resources.Upgrade_icon;
			break;
		case FuntionType.gmSet:
			pictureBox1.Image = (Image)(object)Resources.GM_icon;
			break;
		case FuntionType.camhub:
			pictureBox1.Image = (Image)(object)Resources.Hub_icon;
			break;
		case FuntionType.rcMode:
			pictureBox1.Image = (Image)(object)Resources.RCMode_icon;
			break;
		case FuntionType.bb_freq:
			pictureBox1.Image = (Image)(object)Resources.UC_icon;
			break;
		}
		_currType = type;
		ReloadLang();
	}

	public void SetCardVisible(bool visible)
	{
		((IControl)gridPanel1).Visible = visible;
	}

	private void FunctionCard_MouseClick(object sender, MouseEventArgs e)
	{
		OnFunctionCardClickEvent?.Invoke(((Control)label1).Text, _currType, null);
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Expected O, but got Unknown
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Expected O, but got Unknown
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Expected O, but got Unknown
		gridPanel1 = new GridPanel();
		label1 = new Label();
		pictureBox1 = new PictureBox();
		((Control)gridPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 68, 70);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)label1);
		((Control)gridPanel1).Controls.Add((Control)(object)pictureBox1);
		((Control)gridPanel1).Cursor = Cursors.Hand;
		((Control)gridPanel1).Dock = (DockStyle)5;
		((Control)gridPanel1).Location = new Point(5, 5);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((ContainerPanel)gridPanel1).Radius = 6;
		((Control)gridPanel1).Size = new Size(110, 138);
		gridPanel1.Span = "100%;100%;100%;-50% 30% 20%";
		((Control)gridPanel1).TabIndex = 0;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)gridPanel1).MouseClick += new MouseEventHandler(FunctionCard_MouseClick);
		((Control)gridPanel1).MouseLeave += gridPanel1_MouseLeave;
		((Control)gridPanel1).MouseHover += gridPanel1_MouseHover;
		((Control)label1).AutoSize = true;
		((Control)label1).ForeColor = Color.White;
		gridPanel1.SetIndex((Control)(object)label1, 2);
		((Control)label1).Location = new Point(5, 74);
		((Control)label1).Margin = new Padding(5);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(100, 31);
		((Control)label1).TabIndex = 1;
		((Control)label1).Text = "label1";
		label1.TextAlign = (ContentAlignment)32;
		((Control)label1).MouseClick += new MouseEventHandler(FunctionCard_MouseClick);
		((Control)label1).MouseLeave += gridPanel1_MouseLeave;
		((Control)label1).MouseHover += gridPanel1_MouseHover;
		pictureBox1.Image = (Image)(object)Resources.UC_icon;
		((Control)pictureBox1).Location = new Point(3, 20);
		((Control)pictureBox1).Margin = new Padding(3, 20, 3, 3);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(104, 46);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 0;
		pictureBox1.TabStop = false;
		((Control)pictureBox1).MouseClick += new MouseEventHandler(FunctionCard_MouseClick);
		((Control)pictureBox1).MouseLeave += gridPanel1_MouseLeave;
		((Control)pictureBox1).MouseHover += gridPanel1_MouseHover;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "FunctionCard";
		((Control)this).Padding = new Padding(5);
		((Control)this).Size = new Size(120, 148);
		((Control)this).MouseClick += new MouseEventHandler(FunctionCard_MouseClick);
		((Control)this).MouseLeave += gridPanel1_MouseLeave;
		((Control)this).MouseHover += gridPanel1_MouseHover;
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel1).PerformLayout();
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
