using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class FunctionSelectFrm : Form
{
	private const byte MAXCOUNT = 5;

	private List<FunctionCard> _funCard;

	private FuntionType _currSelectFunType;

	private FuntionType[] _allType;

	private DevCardEventArgs _devCardArgs;

	public Action<string, FuntionType, DevCardEventArgs> OnFunctionSelectFrmEvent;

	private IContainer components = null;

	private GridPanel gridPanel1;

	private PageHeader pageHeader1;

	private Button button1;

	private StackPanel stackPanel1;

	public FunctionSelectFrm()
	{
		InitializeComponent();
		_funCard = new List<FunctionCard>();
		_allType = new FuntionType[5]
		{
			FuntionType.bb_freq,
			FuntionType.rcMode,
			FuntionType.camhub,
			FuntionType.gmSet,
			FuntionType.upgrade
		};
		_devCardArgs = new DevCardEventArgs();
	}

	public void ReloadLang()
	{
		((Control)button1).Text = Lang.T("btn_find_device");
		foreach (FunctionCard item in _funCard)
		{
			item.ReloadLang();
		}
	}

	private void FunctionSelectFrm_Load(object sender, EventArgs e)
	{
	}

	private void OnFunctionCardClick(string a1, FuntionType a2, object a3)
	{
		OnFunctionSelectFrmEvent?.Invoke(a1, a2, _devCardArgs);
		GD.Inst.CurrSelFunType = a2;
		((Control)this).Hide();
	}

	private void button1_MouseClick(object sender, MouseEventArgs e)
	{
		GD.Inst.CurrSelFunType = FuntionType.findDevice;
		((Control)this).Hide();
	}

	private void InitFuntionCard(int count = 3)
	{
	}

	public void DispFunCard(int count, FuntionType[] types)
	{
		stackPanel1.Controls.Clear();
		_funCard.Clear();
		stackPanel1.ItemSize = (100 / count).ToString("f0") + "%";
		for (int i = 0; i < count; i++)
		{
			FunctionCard functionCard = new FunctionCard();
			functionCard.OnFunctionCardClickEvent = (Action<string, FuntionType, object>)Delegate.Combine(functionCard.OnFunctionCardClickEvent, new Action<string, FuntionType, object>(OnFunctionCardClick));
			functionCard.CurrFunType = types[i];
			functionCard.SetCardVisible(visible: true);
			_funCard.Add(functionCard);
			stackPanel1.Controls.Add((Control)(object)functionCard);
		}
	}

	public void DispFunCard(UserLevel level)
	{
		stackPanel1.Controls.Clear();
		_funCard.Clear();
		switch (level)
		{
		case UserLevel.caddx:
		{
			stackPanel1.ItemSize = 20.ToString("f0") + "%";
			for (int j = 0; j < 5; j++)
			{
				FunctionCard functionCard2 = new FunctionCard();
				functionCard2.OnFunctionCardClickEvent = (Action<string, FuntionType, object>)Delegate.Combine(functionCard2.OnFunctionCardClickEvent, new Action<string, FuntionType, object>(OnFunctionCardClick));
				functionCard2.CurrFunType = _allType[j];
				functionCard2.SetCardVisible(visible: true);
				_funCard.Add(functionCard2);
				stackPanel1.Controls.Add((Control)(object)functionCard2);
			}
			break;
		}
		case UserLevel.customA:
		{
			stackPanel1.ItemSize = 50.ToString("f0") + "%";
			FuntionType[] array = new FuntionType[2]
			{
				FuntionType.bb_freq,
				FuntionType.upgrade
			};
			for (int k = 0; k < 2; k++)
			{
				FunctionCard functionCard3 = new FunctionCard();
				functionCard3.OnFunctionCardClickEvent = (Action<string, FuntionType, object>)Delegate.Combine(functionCard3.OnFunctionCardClickEvent, new Action<string, FuntionType, object>(OnFunctionCardClick));
				functionCard3.CurrFunType = array[k];
				functionCard3.SetCardVisible(visible: true);
				_funCard.Add(functionCard3);
				stackPanel1.Controls.Add((Control)(object)functionCard3);
			}
			break;
		}
		default:
		{
			stackPanel1.ItemSize = "100%";
			for (int i = 0; i < 1; i++)
			{
				FunctionCard functionCard = new FunctionCard();
				functionCard.OnFunctionCardClickEvent = (Action<string, FuntionType, object>)Delegate.Combine(functionCard.OnFunctionCardClickEvent, new Action<string, FuntionType, object>(OnFunctionCardClick));
				functionCard.CurrFunType = FuntionType.upgrade;
				functionCard.SetCardVisible(visible: true);
				_funCard.Add(functionCard);
				stackPanel1.Controls.Add((Control)(object)functionCard);
			}
			break;
		}
		}
	}

	public void SetDevCardEventArgs(DevCardEventArgs devArg)
	{
		_devCardArgs = devArg;
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
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Expected O, but got Unknown
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Expected O, but got Unknown
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FunctionSelectFrm));
		gridPanel1 = new GridPanel();
		stackPanel1 = new StackPanel();
		pageHeader1 = new PageHeader();
		button1 = new Button();
		((Control)gridPanel1).SuspendLayout();
		((Control)pageHeader1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)gridPanel1).Controls.Add((Control)(object)stackPanel1);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(680, 245);
		gridPanel1.Span = "100%;100%;100%;-60 120 100%";
		((Control)gridPanel1).TabIndex = 0;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)stackPanel1).Dock = (DockStyle)5;
		((Control)stackPanel1).Location = new Point(5, 65);
		((Control)stackPanel1).Margin = new Padding(5);
		((Control)stackPanel1).Name = "stackPanel1";
		((Control)stackPanel1).Size = new Size(670, 110);
		((Control)stackPanel1).TabIndex = 1;
		((Control)stackPanel1).Text = "stackPanel1";
		((IControl)pageHeader1).ColorScheme = (TAMode)2;
		((Control)pageHeader1).Controls.Add((Control)(object)button1);
		((Control)pageHeader1).Font = new Font("宋体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((IControl)pageHeader1).HandDragFolder = false;
		((Control)pageHeader1).Location = new Point(3, 3);
		pageHeader1.MaximizeBox = false;
		pageHeader1.MinimizeBox = false;
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Padding = new Padding(5, 5, 20, 5);
		((Control)pageHeader1).Size = new Size(674, 54);
		((Control)pageHeader1).TabIndex = 0;
		((Control)pageHeader1).TabStop = false;
		((Control)pageHeader1).Text = "Fuction select";
		button1.DisplayStyle = (TButtonDisplayStyle)2;
		((Control)button1).Dock = (DockStyle)4;
		button1.Ghost = true;
		button1.Icon = (Image)(object)Resources.通用关闭_灰;
		((Control)button1).Location = new Point(642, 5);
		((Control)button1).Margin = new Padding(0);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(12, 44);
		((Control)button1).TabIndex = 0;
		((Control)button1).Text = "button1";
		button1.WaveSize = 0;
		((Control)button1).MouseClick += new MouseEventHandler(button1_MouseClick);
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Form)this).ClientSize = new Size(680, 245);
		((Form)this).ControlBox = false;
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
		((Control)this).MaximumSize = new Size(680, 245);
		((Control)this).Name = "FunctionSelectFrm";
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)4;
		((Form)this).Load += FunctionSelectFrm_Load;
		((Control)gridPanel1).ResumeLayout(false);
		((Control)pageHeader1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
