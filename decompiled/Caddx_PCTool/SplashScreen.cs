using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class SplashScreen : Window
{
	private static SplashScreen ms_frmSplash = null;

	private static Thread ms_oThread = null;

	private double m_dblOpacityIncrement;

	private double m_dblOpacityDecrement;

	private const int TIMER_INTERVAL = 50;

	private static readonly Size SplashClientSize = new Size(800, 600);

	private string m_sStatus;

	private string m_sTimeRemaining;

	private double m_dblCompletionFraction;

	private Rectangle m_rProgress;

	private double m_dblLastCompletionFraction;

	private double m_dblPBIncrementPerTimerInterval;

	private int m_iIndex;

	private int m_iActualTicks;

	private ArrayList m_alPreviousCompletionFraction;

	private ArrayList m_alActualTimes;

	private DateTime m_dtStart;

	private bool m_bFirstLaunch;

	private bool m_bDTSet;

	private IContainer components;

	private Label lblStatus;

	private Label lblTimeRemaining;

	private Timer UpdateTimer;

	private Panel pnlStatus;

	private PictureBox pictureBox1;

	private Label label1;

	public bool SetTopmost
	{
		get
		{
			return ((Form)this).TopMost;
		}
		set
		{
			((BaseForm)this).Invoke((Action)delegate
			{
				((Form)this).TopMost = value;
			});
		}
	}

	public SplashScreen()
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		m_dblOpacityIncrement = 0.05;
		m_dblOpacityDecrement = 0.08;
		m_dblCompletionFraction = 0.0;
		m_dblLastCompletionFraction = 0.0;
		m_dblPBIncrementPerTimerInterval = 0.015;
		m_iIndex = 1;
		m_iActualTicks = 0;
		m_alActualTimes = new ArrayList();
		m_bFirstLaunch = false;
		m_bDTSet = false;
		components = null;
		((Window)this)._002Ector();
		InitializeComponent();
		((Form)this).Opacity = 0.0;
		UpdateTimer.Interval = 50;
		UpdateTimer.Start();
		((Control)label1).Font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 28f);
		((Control)lblStatus).Font = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 16f);
		((Control)lblTimeRemaining).Font = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 16f);
		DispLogoStatus logoStatus = Program.LogoStatus;
		DispLogoStatus dispLogoStatus = logoStatus;
		if (dispLogoStatus <= DispLogoStatus.caddx_Industry)
		{
			((IControl)label1).Visible = true;
			((Control)label1).Text = "Caddx PC Tool";
			pictureBox1.Image = (Image)(object)Resources.new_Logo;
		}
		ApplySplashLayout();
	}

	public static void ShowSplashScreen()
	{
		if (ms_frmSplash == null)
		{
			ms_oThread = new Thread(ShowForm);
			ms_oThread.IsBackground = true;
			ms_oThread.SetApartmentState(ApartmentState.STA);
			ms_oThread.Start();
			while (ms_frmSplash == null || !((Control)ms_frmSplash).IsHandleCreated)
			{
				Thread.Sleep(50);
			}
		}
	}

	public static void CloseForm()
	{
		if (ms_frmSplash != null && !((Control)ms_frmSplash).IsDisposed)
		{
			ms_frmSplash.m_dblOpacityIncrement = 0.0 - ms_frmSplash.m_dblOpacityDecrement;
		}
		ms_oThread = null;
		ms_frmSplash = null;
	}

	public static void SetStatus(string newStatus)
	{
		SetStatus(newStatus, setReference: true);
	}

	public static void SetStatus(string newStatus, bool setReference)
	{
		if (ms_frmSplash != null)
		{
			ms_frmSplash.m_sStatus = newStatus;
			if (setReference)
			{
				ms_frmSplash.SetReferenceInternal();
			}
		}
	}

	public static void SetReferencePoint()
	{
		if (ms_frmSplash != null)
		{
			ms_frmSplash.SetReferenceInternal();
		}
	}

	private static void ShowForm()
	{
		ms_frmSplash = new SplashScreen();
		Application.Run((Form)(object)ms_frmSplash);
	}

	private void SetReferenceInternal()
	{
		if (!m_bDTSet)
		{
			m_bDTSet = true;
			m_dtStart = DateTime.Now;
			ReadIncrements();
		}
		double num = ElapsedMilliSeconds();
		m_alActualTimes.Add(num);
		m_dblLastCompletionFraction = m_dblCompletionFraction;
		if (m_alPreviousCompletionFraction != null && m_iIndex < m_alPreviousCompletionFraction.Count)
		{
			m_dblCompletionFraction = (double)m_alPreviousCompletionFraction[m_iIndex++];
		}
		else
		{
			m_dblCompletionFraction = ((m_iIndex > 0) ? 1 : 0);
		}
	}

	private double ElapsedMilliSeconds()
	{
		return (DateTime.Now - m_dtStart).TotalMilliseconds;
	}

	private void ReadIncrements()
	{
		string interval = SplashScreenXMLStorage.Interval;
		if (double.TryParse(interval, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var result))
		{
			m_dblPBIncrementPerTimerInterval = result;
		}
		else
		{
			m_dblPBIncrementPerTimerInterval = 0.0015;
		}
		string percents = SplashScreenXMLStorage.Percents;
		if (percents != "")
		{
			string[] array = percents.Split((char[]?)null);
			m_alPreviousCompletionFraction = new ArrayList();
			for (int i = 0; i < array.Length; i++)
			{
				if (double.TryParse(array[i], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var result2))
				{
					m_alPreviousCompletionFraction.Add(result2);
				}
				else
				{
					m_alPreviousCompletionFraction.Add(1.0);
				}
			}
		}
		else
		{
			m_bFirstLaunch = true;
			m_sTimeRemaining = "";
		}
	}

	private void StoreIncrements()
	{
		string text = "";
		double num = ElapsedMilliSeconds();
		for (int i = 0; i < m_alActualTimes.Count; i++)
		{
			text = text + ((double)m_alActualTimes[i] / num).ToString("0.####", NumberFormatInfo.InvariantInfo) + " ";
		}
		SplashScreenXMLStorage.Percents = text;
		m_dblPBIncrementPerTimerInterval = 1.0 / (double)m_iActualTicks;
		SplashScreenXMLStorage.Interval = m_dblPBIncrementPerTimerInterval.ToString("#.000000", NumberFormatInfo.InvariantInfo);
	}

	private void LoadLocalImage()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		string text = Application.StartupPath + "\\res\\Splash.jpeg";
		if (File.Exists(text))
		{
			Bitmap backgroundImage = new Bitmap(text);
			((Control)this).BackgroundImage = (Image)(object)backgroundImage;
		}
	}

	public static SplashScreen GetSplashScreen()
	{
		return ms_frmSplash;
	}

	private void UpdateTimer_Tick(object sender, EventArgs e)
	{
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Expected O, but got Unknown
		((Control)lblStatus).Text = m_sStatus;
		if (m_dblOpacityIncrement > 0.0)
		{
			m_iActualTicks++;
			if (((Form)this).Opacity < 1.0)
			{
				((Form)this).Opacity = ((Form)this).Opacity + m_dblOpacityIncrement;
			}
		}
		else if (((Form)this).Opacity > 0.0)
		{
			((Form)this).Opacity = ((Form)this).Opacity + m_dblOpacityIncrement;
		}
		else
		{
			StoreIncrements();
			UpdateTimer.Stop();
			((Form)this).Close();
		}
		if (!m_bFirstLaunch && m_dblLastCompletionFraction < m_dblCompletionFraction)
		{
			m_dblLastCompletionFraction += m_dblPBIncrementPerTimerInterval;
			int num = (int)Math.Floor((double)((Control)pnlStatus).ClientRectangle.Width * m_dblLastCompletionFraction);
			int height = ((Control)pnlStatus).ClientRectangle.Height;
			int x = ((Control)pnlStatus).ClientRectangle.X;
			int y = ((Control)pnlStatus).ClientRectangle.Y;
			if (num > 0 && height > 0)
			{
				m_rProgress = new Rectangle(x, y, num, height);
				if (!((Control)pnlStatus).IsDisposed)
				{
					Graphics val = ((Control)pnlStatus).CreateGraphics();
					LinearGradientBrush val2 = new LinearGradientBrush(m_rProgress, Color.FromArgb(255, 233, 0), Color.FromArgb(255, 233, 0), (LinearGradientMode)0);
					val.FillRectangle((Brush)(object)val2, m_rProgress);
					val.Dispose();
				}
				int num2 = 1 + (int)(50.0 * ((1.0 - m_dblLastCompletionFraction) / m_dblPBIncrementPerTimerInterval)) / 1000;
				m_sTimeRemaining = ((num2 == 1) ? string.Format("1 second", Array.Empty<object>()) : $"{num2} seconds");
			}
		}
		((Control)lblTimeRemaining).Text = m_sTimeRemaining;
	}

	private void SplashScreen_DoubleClick(object sender, EventArgs e)
	{
	}

	private void SplashScreen_Load(object sender, EventArgs e)
	{
		ApplySplashLayout();
	}

	private void ApplySplashLayout()
	{
		((Window)this).ClientSize = SplashClientSize;
		CenterSplashOnPrimaryScreen();
		CenterBrandControls();
	}

	private void CenterSplashOnPrimaryScreen()
	{
		Rectangle bounds = Screen.PrimaryScreen.Bounds;
		((Window)this).Location = new Point(bounds.Left + (bounds.Width - ((Window)this).Width) / 2, bounds.Top + (bounds.Height - ((Window)this).Height) / 2);
	}

	private void CenterBrandControls()
	{
		((Control)pictureBox1).Location = new Point((((Window)this).ClientSize.Width - ((Control)pictureBox1).Width) / 2, ((Control)pictureBox1).Location.Y);
		((Control)label1).Location = new Point((((Window)this).ClientSize.Width - ((Control)label1).Width) / 2, ((Control)label1).Location.Y);
	}

	private void pnlStatus_Paint(object sender, PaintEventArgs e)
	{
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Expected O, but got Unknown
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Expected O, but got Unknown
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Expected O, but got Unknown
		components = new Container();
		lblStatus = new Label();
		pnlStatus = new Panel();
		lblTimeRemaining = new Label();
		UpdateTimer = new Timer(components);
		pictureBox1 = new PictureBox();
		label1 = new Label();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((Control)lblStatus).Anchor = (AnchorStyles)10;
		((Control)lblStatus).BackColor = Color.Transparent;
		((Control)lblStatus).Font = new Font("Microsoft Sans Serif", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lblStatus).ForeColor = Color.FromArgb(102, 102, 102);
		((Control)lblStatus).Location = new Point(17, 538);
		((Control)lblStatus).Name = "lblStatus";
		((Control)lblStatus).Size = new Size(275, 34);
		((Control)lblStatus).TabIndex = 0;
		((Control)lblStatus).Text = "124";
		((Control)lblStatus).DoubleClick += SplashScreen_DoubleClick;
		((Control)pnlStatus).BackColor = Color.Black;
		((Control)pnlStatus).Location = new Point(17, 517);
		((Control)pnlStatus).Margin = new Padding(20, 0, 20, 0);
		((Control)pnlStatus).Name = "pnlStatus";
		((Control)pnlStatus).Size = new Size(760, 17);
		((Control)pnlStatus).TabIndex = 1;
		((Control)pnlStatus).Paint += new PaintEventHandler(pnlStatus_Paint);
		((Control)pnlStatus).DoubleClick += SplashScreen_DoubleClick;
		((Control)lblTimeRemaining).Anchor = (AnchorStyles)10;
		((Control)lblTimeRemaining).BackColor = Color.Transparent;
		((Control)lblTimeRemaining).Font = new Font("Microsoft Sans Serif", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lblTimeRemaining).ForeColor = Color.FromArgb(102, 102, 102);
		((Control)lblTimeRemaining).Location = new Point(641, 538);
		((Control)lblTimeRemaining).Name = "lblTimeRemaining";
		((Control)lblTimeRemaining).Size = new Size(157, 34);
		((Control)lblTimeRemaining).TabIndex = 2;
		((Control)lblTimeRemaining).Text = "1 seconds";
		((Control)lblTimeRemaining).DoubleClick += SplashScreen_DoubleClick;
		UpdateTimer.Tick += UpdateTimer_Tick;
		((Control)pictureBox1).BackColor = Color.Transparent;
		pictureBox1.Image = (Image)(object)Resources.neutralLogo;
		((Control)pictureBox1).Location = new Point(328, 170);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(180, 140);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 3;
		pictureBox1.TabStop = false;
		label1.AutoSizeMode = (TAutoSize)1;
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Font = new Font("Arial", 26.25f, (FontStyle)0, (GraphicsUnit)3, (byte)238);
		label1.ForeColor = Color.FromArgb(255, 255, 255);
		((Control)label1).Location = new Point(357, 313);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(127, 40);
		((Control)label1).TabIndex = 4;
		((Control)label1).Text = "PC Tool";
		((Form)this).AutoScaleBaseSize = new Size(6, 14);
		((Control)this).BackColor = Color.White;
		((Control)this).BackgroundImage = (Image)(object)Resources.Start_Loading_Bg;
		((Control)this).BackgroundImageLayout = (ImageLayout)3;
		((Window)this).ClientSize = new Size(800, 600);
		((Control)this).Controls.Add((Control)(object)label1);
		((Control)this).Controls.Add((Control)(object)pictureBox1);
		((Control)this).Controls.Add((Control)(object)lblTimeRemaining);
		((Control)this).Controls.Add((Control)(object)pnlStatus);
		((Control)this).Controls.Add((Control)(object)lblStatus);
		((Control)this).DoubleBuffered = true;
		((BaseForm)this).FormBorderStyle = (FormBorderStyle)0;
		((Control)this).Name = "SplashScreen";
		((Window)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)0;
		((Control)this).Text = "SplashScreen";
		((Form)this).TopMost = true;
		((Form)this).Load += SplashScreen_Load;
		((Control)this).DoubleClick += SplashScreen_DoubleClick;
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
