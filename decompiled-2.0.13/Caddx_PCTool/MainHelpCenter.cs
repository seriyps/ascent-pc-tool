using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI;

namespace Caddx_PCTool;

public class MainHelpCenter : UserControl
{
	private string _className = "MainHelpCenter";

	private List<UserManualCard> _userCardList = new List<UserManualCard>();

	private List<Button> _btnList;

	private string[] _devNameArr = new string[4] { "Ascent\u00a0Lite\u00a0VTX", "Ascent Goggles", "Ascent VRX", "Ascent GT PRO" };

	private string btn_ascent_cn = "Ascent系列";

	private string btn_ascent_en = "Ascent Series";

	private IContainer components = null;

	private Divider divider1;

	private GridPanel gridPanel1;

	private GridPanel gridPan_DevCard;

	private GridPanel gridPanel2;

	private Button btn_ascent;

	private Button button2;

	public event EventHandler<HappenEventArgs> OnMainHelpCenterHappenEvent;

	public MainHelpCenter()
	{
		InitializeComponent();
	}

	private void MainHelpCenter_Load(object sender, EventArgs e)
	{
		InitGridPanDevCard();
		ReloadFont();
		ReloadLang();
	}

	private void InitGridPanDevCard()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			_userCardList.Clear();
			((Control)gridPan_DevCard).Controls.Clear();
			for (int i = 0; i < 2; i++)
			{
				_userCardList.Add(new UserManualCard(_devNameArr[i]));
				_userCardList[i].OnUserManualCardHappenEvent += OnUserManualCardHappen;
				((Control)gridPan_DevCard).Controls.Add((Control)(object)_userCardList[i]);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void OnUserManualCardHappen(object sender, HappenEventArgs e)
	{
		OnMainHelpCenterHappenEvent?.Invoke(((Control)this).Name, e);
	}

	private void ReloadFont()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		_btnList = new List<Button> { btn_ascent, button2 };
		Font font = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 14f);
		for (int i = 0; i < _btnList.Count; i++)
		{
			((Control)_btnList[i]).Font = font;
		}
		Font font2 = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 14f);
		((Control)_btnList[0]).Font = font2;
	}

	public void ReloadLang()
	{
		try
		{
			switch ((LangType)GD.Inst.CurrLang)
			{
			case LangType.zh_CN:
				((Control)btn_ascent).Text = btn_ascent_cn;
				break;
			case LangType.en_US:
				((Control)btn_ascent).Text = btn_ascent_en;
				break;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("helpCenter.ReloadLang error,desc=" + ex.Message, Color.Red);
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
	}

	private void button2_Click(object sender, EventArgs e)
	{
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
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Expected O, but got Unknown
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Expected O, but got Unknown
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		divider1 = new Divider();
		gridPanel1 = new GridPanel();
		gridPan_DevCard = new GridPanel();
		gridPanel2 = new GridPanel();
		button2 = new Button();
		btn_ascent = new Button();
		((Control)gridPanel1).SuspendLayout();
		((Control)gridPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)divider1).BackColor = Color.Transparent;
		divider1.ColorSplit = Color.FromArgb(26, 255, 255, 255);
		((Control)divider1).Dock = (DockStyle)5;
		((IControl)divider1).HandCursor = Cursors.Default;
		gridPanel1.SetIndex((Control)(object)divider1, 2);
		((Control)divider1).Location = new Point(168, 0);
		((Control)divider1).Margin = new Padding(0);
		((Control)divider1).Name = "divider1";
		divider1.OrientationMargin = 0f;
		((Control)divider1).Size = new Size(25, 640);
		((Control)divider1).TabIndex = 0;
		((Control)divider1).TabStop = false;
		((Control)divider1).Text = "";
		divider1.TextPadding = 0f;
		divider1.Thickness = 2f;
		divider1.Vertical = true;
		((ContainerPanel)gridPanel1).Back = Color.Transparent;
		((Control)gridPanel1).BackColor = Color.Transparent;
		((Control)gridPanel1).Controls.Add((Control)(object)gridPan_DevCard);
		((Control)gridPanel1).Controls.Add((Control)(object)gridPanel2);
		((Control)gridPanel1).Controls.Add((Control)(object)divider1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((IControl)gridPanel1).HandCursor = Cursors.Default;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(840, 640);
		gridPanel1.Span = "20% 3% 77%;";
		((Control)gridPanel1).TabIndex = 1;
		((Control)gridPanel1).Text = "gridPanel1";
		((ContainerPanel)gridPan_DevCard).Back = Color.Transparent;
		((Control)gridPan_DevCard).Dock = (DockStyle)5;
		((IControl)gridPan_DevCard).HandCursor = Cursors.Default;
		gridPanel1.SetIndex((Control)(object)gridPan_DevCard, 3);
		((Control)gridPan_DevCard).Location = new Point(193, 0);
		((Control)gridPan_DevCard).Margin = new Padding(0);
		((Control)gridPan_DevCard).Name = "gridPan_DevCard";
		((Control)gridPan_DevCard).Size = new Size(647, 640);
		gridPan_DevCard.Span = "33.3% 33.3% 33.4%;33.3% 33.3% 33.4%;-50% 50%";
		((Control)gridPan_DevCard).TabIndex = 4;
		((Control)gridPan_DevCard).Text = "gridPanel3";
		((ContainerPanel)gridPanel2).Back = Color.Transparent;
		((Control)gridPanel2).Controls.Add((Control)(object)button2);
		((Control)gridPanel2).Controls.Add((Control)(object)btn_ascent);
		((IControl)gridPanel2).HandCursor = Cursors.Default;
		gridPanel1.SetIndex((Control)(object)gridPanel2, 1);
		((Control)gridPanel2).Location = new Point(0, 0);
		((Control)gridPanel2).Margin = new Padding(0);
		((Control)gridPanel2).Name = "gridPanel2";
		((Control)gridPanel2).Size = new Size(168, 640);
		gridPanel2.Span = "100%;100%;100%;100%;-12% 12% 10% 70%";
		((Control)gridPanel2).TabIndex = 3;
		((Control)gridPanel2).Text = "gridPanel2";
		button2.DefaultBack = Color.Transparent;
		button2.DefaultBorderColor = Color.White;
		button2.DisplayStyle = (TButtonDisplayStyle)1;
		((Control)button2).Font = new Font("阿里巴巴普惠体", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button2.ForeColor = Color.FromArgb(164, 164, 165);
		((Control)button2).Location = new Point(3, 80);
		((Control)button2).Name = "button2";
		((Control)button2).Size = new Size(162, 71);
		((Control)button2).TabIndex = 1;
		((Control)button2).Text = "Gimbal Series";
		((IControl)button2).Visible = false;
		button2.WaveSize = 0;
		((Control)button2).Click += button2_Click;
		btn_ascent.DefaultBack = Color.FromArgb(255, 233, 0);
		btn_ascent.DisplayStyle = (TButtonDisplayStyle)1;
		((Control)btn_ascent).Font = new Font("阿里巴巴普惠体 Medium", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		btn_ascent.ForeColor = Color.FromArgb(35, 35, 35);
		((Control)btn_ascent).Location = new Point(10, 10);
		((Control)btn_ascent).Margin = new Padding(10);
		((Control)btn_ascent).Name = "btn_ascent";
		((Control)btn_ascent).Size = new Size(148, 57);
		((Control)btn_ascent).TabIndex = 0;
		((Control)btn_ascent).Text = "Ascent Series";
		btn_ascent.WaveSize = 0;
		((Control)btn_ascent).Click += button1_Click;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Margin = new Padding(10, 10, 3, 3);
		((Control)this).Name = "MainHelpCenter";
		((Control)this).Size = new Size(840, 640);
		((UserControl)this).Load += MainHelpCenter_Load;
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel2).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
