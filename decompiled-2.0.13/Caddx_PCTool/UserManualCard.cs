using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class UserManualCard : UserControl
{
	private string _devName = "";

	private IContainer components = null;

	private PictureBox pictureBox1;

	private GridPanel gridPanel1;

	private Button button1;

	public event EventHandler<HappenEventArgs> OnUserManualCardHappenEvent;

	public UserManualCard()
	{
		InitializeComponent();
	}

	public UserManualCard(string devName)
	{
		InitializeComponent();
		_devName = devName;
	}

	private void UserManualCard_Load(object sender, EventArgs e)
	{
		if (_devName.Length > 0)
		{
			SetDevName(_devName);
		}
		ReloadFont();
	}

	private void label1_MouseClick(object sender, MouseEventArgs e)
	{
	}

	private void UserManualCard_MouseClick(object sender, MouseEventArgs e)
	{
		HappenEventArgs e2 = new HappenEventArgs
		{
			eventType = EventType.frmSign,
			DevName = _devName
		};
		OnUserManualCardHappenEvent?.Invoke(((Control)this).Name, e2);
	}

	public void SetDevName(string name)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)button1).Text = name;
			if (name == "Ascent Goggles")
			{
				pictureBox1.Image = (Image)(object)Resources.Ascent_Goggles;
			}
			if (name == "Ascent\u00a0Lite\u00a0VTX")
			{
				pictureBox1.Image = (Image)(object)Resources.Ascent_Lite_VTX;
			}
			else if (name == "Ascent VRX")
			{
				pictureBox1.Image = (Image)(object)Resources.Ascent_VRX;
			}
			else if (name.Contains("Ascent GT PRO"))
			{
				pictureBox1.Image = (Image)(object)Resources.Ascent_GT;
			}
		});
	}

	private void gridPanel1_MouseEnter(object sender, EventArgs e)
	{
	}

	private void gridPanel1_MouseLeave(object sender, EventArgs e)
	{
	}

	private void ReloadFont()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		Font font = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 14f);
		((Control)button1).Font = font;
	}

	private void ReloadLang()
	{
		switch ((LangType)GD.Inst.CurrLang)
		{
		case LangType.zh_CN:
			((Control)this).Invoke((Delegate)(Action)delegate
			{
			});
			break;
		case LangType.en_US:
			((Control)this).Invoke((Delegate)(Action)delegate
			{
			});
			break;
		}
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
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Expected O, but got Unknown
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Expected O, but got Unknown
		gridPanel1 = new GridPanel();
		button1 = new Button();
		pictureBox1 = new PictureBox();
		((Control)gridPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((ContainerPanel)gridPanel1).Back = Color.Transparent;
		((Control)gridPanel1).BackColor = Color.Transparent;
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 68, 70);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)button1);
		((Control)gridPanel1).Controls.Add((Control)(object)pictureBox1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((IControl)gridPanel1).HandCursor = Cursors.Default;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(300, 370);
		gridPanel1.Span = "100%;100%;-80% 20%";
		((Control)gridPanel1).TabIndex = 2;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)gridPanel1).MouseClick += new MouseEventHandler(UserManualCard_MouseClick);
		((Control)gridPanel1).MouseEnter += gridPanel1_MouseEnter;
		((Control)gridPanel1).MouseLeave += gridPanel1_MouseLeave;
		button1.DefaultBack = Color.FromArgb(0, 0, 22, 119);
		button1.DisplayStyle = (TButtonDisplayStyle)1;
		((Control)button1).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button1.ForeColor = Color.White;
		button1.Ghost = true;
		((IControl)button1).HandCursor = Cursors.Default;
		((Control)button1).Location = new Point(5, 301);
		((Control)button1).Margin = new Padding(5);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(290, 64);
		((Control)button1).TabIndex = 1;
		((Control)button1).Text = "Ascent\u00a0Lite\u00a0VTX";
		button1.WaveSize = 0;
		((Control)button1).MouseClick += new MouseEventHandler(UserManualCard_MouseClick);
		((Control)button1).MouseEnter += gridPanel1_MouseEnter;
		((Control)button1).MouseLeave += gridPanel1_MouseLeave;
		((Control)pictureBox1).BackColor = Color.Transparent;
		pictureBox1.Image = (Image)(object)Resources.Ascent_Goggles;
		gridPanel1.SetIndex((Control)(object)pictureBox1, 1);
		((Control)pictureBox1).Location = new Point(5, 5);
		((Control)pictureBox1).Margin = new Padding(5);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(290, 286);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 0;
		pictureBox1.TabStop = false;
		((Control)pictureBox1).MouseClick += new MouseEventHandler(UserManualCard_MouseClick);
		((Control)pictureBox1).MouseEnter += gridPanel1_MouseEnter;
		((Control)pictureBox1).MouseLeave += gridPanel1_MouseLeave;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Name = "UserManualCard";
		((Control)this).Size = new Size(300, 370);
		((UserControl)this).Load += UserManualCard_Load;
		((Control)gridPanel1).ResumeLayout(false);
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
