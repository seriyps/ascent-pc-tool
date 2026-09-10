using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using AntdUI;

namespace Caddx_PCTool;

public class Label_Input : UserControl
{
	private IContainer components = null;

	private GridPanel gridPanel1;

	private Label label1;

	private Select select1;

	[Category("Behavior")]
	public int Sel_Idx
	{
		get
		{
			return select1.SelectedIndex;
		}
		set
		{
			select1.SelectedIndex = value;
		}
	}

	[Category("Behavior")]
	public event EventHandler<IntEventArgs> OnSelectedIndexChangedEvent;

	public Label_Input()
	{
		InitializeComponent();
	}

	private void Label_Input_Load(object sender, EventArgs e)
	{
	}

	public void SetLabelText(string title)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)label1).Text = title;
		});
	}

	public void SetInputlText(uint va1, uint val2)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
		});
	}

	private void inp_VerifyChar(object sender, InputVerifyCharEventArgs e)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
		string numberDecimalSeparator = numberFormat.NumberDecimalSeparator;
		string numberGroupSeparator = numberFormat.NumberGroupSeparator;
		string negativeSign = numberFormat.NegativeSign;
		string text = e.Char.ToString();
		int num = ((Control)(Input)sender).Text.IndexOf('.');
		if (char.IsDigit(e.Char))
		{
			e.Result = true;
			return;
		}
		if (e.Char == '\b')
		{
			e.Result = true;
			return;
		}
		e.Result = false;
		string title;
		string desc;
		if (GD.Inst.CurrLang == 1)
		{
			title = "警告";
			desc = "只允许输入数字。";
		}
		else
		{
			title = "Warning";
			desc = "Only numbers allowed.";
		}
		CommModalFrm commModalFrm = new CommModalFrm();
		commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
		((Form)commModalFrm).ShowDialog();
	}

	private void select1_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		OnSelectedIndexChangedEvent?.Invoke(sender, e);
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
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Expected O, but got Unknown
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Expected O, but got Unknown
		gridPanel1 = new GridPanel();
		select1 = new Select();
		label1 = new Label();
		((Control)gridPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((ContainerPanel)gridPanel1).Back = Color.Transparent;
		((Control)gridPanel1).BackColor = Color.Transparent;
		((Control)gridPanel1).Controls.Add((Control)(object)select1);
		((Control)gridPanel1).Controls.Add((Control)(object)label1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((Control)gridPanel1).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(150, 150);
		gridPanel1.Span = "100%;100%;-35% 65%";
		((Control)gridPanel1).TabIndex = 0;
		((Control)gridPanel1).Text = "gridPanel1";
		((Input)select1).BackColor = Color.FromArgb(30, 34, 37);
		((Input)select1).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)select1).BorderColor = Color.FromArgb(66, 69, 71);
		((Input)select1).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)select1).BorderWidth = 2f;
		((IControl)select1).ColorScheme = (TAMode)2;
		select1.DropDownArrow = true;
		((Input)select1).ForeColor = Color.FromArgb(255, 255, 255);
		select1.Items.AddRange(new object[18]
		{
			"NULL", "CH1", "CH2", "CH3", "CH4", "CH5", "CH6", "CH7", "CH8", "CH9",
			"CH10", "CH11", "CH12", "CH13", "CH14", "CH15", "CH16", "CH17"
		});
		select1.List = true;
		((Control)select1).Location = new Point(3, 55);
		select1.MaxCount = 5;
		((Control)select1).Name = "select1";
		((Control)select1).Size = new Size(144, 92);
		((Control)select1).TabIndex = 1;
		((Input)select1).TextAlign = (HorizontalAlignment)2;
		select1.SelectedIndexChanged += new IntEventHandler(select1_SelectedIndexChanged);
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Font = new Font("阿里巴巴普惠体 Medium", 9.749999f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)label1).ForeColor = Color.White;
		gridPanel1.SetIndex((Control)(object)label1, 1);
		((Control)label1).Location = new Point(3, 0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(144, 52);
		((Control)label1).TabIndex = 0;
		((Control)label1).Text = "label1";
		label1.TextAlign = (ContentAlignment)512;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(57, 59, 48);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Name = "Label_Input";
		((UserControl)this).Load += Label_Input_Load;
		((Control)gridPanel1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
