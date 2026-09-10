using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;
using PdfiumViewer;

namespace Caddx_PCTool;

public class UserMaualPdfCtrl : UserControl, IDisposable
{
	private string _className = "pdfReadCtrl";

	private string _devname = "Ascent Goggles";

	private string manual_cn = "说明书";

	private string video_cn = "视频教程";

	private string videoPagin_cn = "详细视频";

	private string manual_en = "Manual";

	private string video_en = "Video";

	private string videoPagin_en = "Video pagination";

	private IContainer components = null;

	private Tabs tabs1;

	private TabPage tabPage1;

	private TabPage tabPage2;

	private TabPage tabPage3;

	private PdfViewer pdfViewer1;

	private GridPanel gridPanel1;

	private Label label1;

	private Button button1;

	public string DeviceName
	{
		get
		{
			return _devname;
		}
		set
		{
			_devname = value;
		}
	}

	public event EventHandler<HappenEventArgs> OnPdfCtrlHappenEvent;

	public UserMaualPdfCtrl()
	{
		InitializeComponent();
	}

	public UserMaualPdfCtrl(string devname)
	{
		InitializeComponent();
		_devname = devname;
	}

	private void UserMaualPdfCtrl_Load(object sender, EventArgs e)
	{
		LoadPDF(_devname switch
		{
			"Ascent Goggles" => Path.Combine(Application.StartupPath, "pdf", "Ascent_Goggles_manual V1.0.pdf"), 
			"Ascent\u00a0Lite\u00a0VTX" => Path.Combine(Application.StartupPath, "pdf", "Ascent_Lite_manual_V1.0.pdf"), 
			"Ascent VRX" => Path.Combine(Application.StartupPath, "pdf", "Ascent_VRX_manual V1.0.pdf"), 
			"Ascent GT PRO" => Path.Combine(Application.StartupPath, "pdf", "Ascent_GT_PRO_manual_V1.0.pdf"), 
			_ => Path.Combine(Application.StartupPath, "pdf", "Ascent Goggles manual V1.0.pdf"), 
		});
		tabs1.SelectTab(tabPage1);
		ReloadFont();
		ReloadLang();
	}

	private void LoadPDF(string path)
	{
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			FileStream fileStream = File.OpenRead(path);
			pdfViewer1.Document = (IPdfDocument)(object)PdfDocument.Load((Stream)fileStream);
			pdfViewer1.ZoomMode = (PdfViewerZoomMode)1;
			pdfViewer1.Renderer.ZoomMode = (PdfViewerZoomMode)1;
			((PanningZoomingScrollControl)pdfViewer1.Renderer).ZoomIn();
			((PanningZoomingScrollControl)pdfViewer1.Renderer).ZoomOut();
			((PanningZoomingScrollControl)pdfViewer1.Renderer).ZoomMax = 2.0;
			((PanningZoomingScrollControl)pdfViewer1.Renderer).ZoomMin = 0.5;
			((PanningZoomingScrollControl)pdfViewer1.Renderer).ZoomFactor = 1.1;
			double zoom = ((PanningZoomingScrollControl)pdfViewer1.Renderer).Zoom;
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error loading PDF: " + ex.Message);
		}
	}

	private void OnDocumentTitleChanged(object sender, object e)
	{
	}

	private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
	{
		HappenEventArgs e2 = new HappenEventArgs
		{
			eventType = EventType.frmSign,
			eventDesc = "ClosePDF",
			className = _className
		};
		OnPdfCtrlHappenEvent?.Invoke(_className, e2);
	}

	private void label1_Click(object sender, EventArgs e)
	{
	}

	private void ReloadFont()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 30f);
		((Control)label1).Font = font;
		Font font2 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 16f);
		((Control)tabs1).Font = font2;
	}

	public void ReloadLang()
	{
		switch ((LangType)GD.Inst.CurrLang)
		{
		case LangType.zh_CN:
			((Control)tabPage1).Text = manual_cn;
			break;
		case LangType.en_US:
			((Control)tabPage1).Text = manual_en;
			break;
		}
	}

	private void tabs1_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		int value = ((VEventArgs<int>)(object)e).Value;
		WriteLog.WriteLogFileToUI($"eval={value}", Color.Black);
		switch (((VEventArgs<int>)(object)e).Value)
		{
		case 0:
			break;
		case 1:
			break;
		case 2:
			break;
		}
	}

	public void ReloadCtrl(string devname)
	{
		LoadPDF(_devname switch
		{
			"Ascent Goggles" => Path.Combine(Application.StartupPath, "pdf", "Ascent_Goggles_manual V1.0.pdf"), 
			"Ascent\u00a0Lite\u00a0VTX" => Path.Combine(Application.StartupPath, "pdf", "Ascent_Lite_manual_V1.0.pdf"), 
			"Ascent VRX" => Path.Combine(Application.StartupPath, "pdf", "Ascent_VRX_manual V1.0.pdf"), 
			"Ascent GT PRO" => Path.Combine(Application.StartupPath, "pdf", "Ascent_GT_PRO_manual_V1.0.pdf"), 
			_ => Path.Combine(Application.StartupPath, "pdf", "Ascent Goggles manual V1.0.pdf"), 
		});
		tabs1.SelectTab(tabPage1);
		((Control)label1).Text = _devname;
	}

	public void Dispose()
	{
		((Component)(object)pdfViewer1)?.Dispose();
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Expected O, but got Unknown
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Expected O, but got Unknown
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Expected O, but got Unknown
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Expected O, but got Unknown
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		StyleLine val = new StyleLine();
		tabs1 = new Tabs();
		tabPage1 = new TabPage();
		pdfViewer1 = new PdfViewer();
		tabPage2 = new TabPage();
		tabPage3 = new TabPage();
		gridPanel1 = new GridPanel();
		button1 = new Button();
		label1 = new Label();
		((Control)tabs1).SuspendLayout();
		((Control)tabPage1).SuspendLayout();
		((Control)gridPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)tabs1).BackColor = Color.Transparent;
		tabs1.Centered = true;
		((IControl)tabs1).ColorScheme = (TAMode)2;
		((Control)tabs1).Controls.Add((Control)(object)tabPage1);
		((Control)tabs1).Controls.Add((Control)(object)tabPage2);
		((Control)tabs1).Controls.Add((Control)(object)tabPage3);
		((Control)tabs1).Cursor = Cursors.Default;
		((Control)tabs1).Dock = (DockStyle)5;
		tabs1.Fill = Color.FromArgb(255, 233, 0);
		((Control)tabs1).Font = new Font("阿里巴巴普惠体", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		tabs1.ForeColor = Color.FromArgb(233, 30, 99);
		((IControl)tabs1).HandCursor = Cursors.Default;
		((IControl)tabs1).HandDragFolder = false;
		gridPanel1.SetIndex((Control)(object)tabs1, 3);
		((Control)tabs1).Location = new Point(0, 57);
		((Control)tabs1).Margin = new Padding(0);
		((Control)tabs1).Name = "tabs1";
		((iCollection<TabPage>)(object)tabs1.Pages).Add(tabPage1);
		((iCollection<TabPage>)(object)tabs1.Pages).Add(tabPage2);
		((iCollection<TabPage>)(object)tabs1.Pages).Add(tabPage3);
		((Control)tabs1).Size = new Size(772, 516);
		val.Back = Color.FromArgb(59, 62, 64);
		tabs1.Style = (IStyle)(object)val;
		((Control)tabs1).TabIndex = 1;
		((Control)tabs1).TabStop = false;
		((Control)tabs1).Text = "tabs1";
		tabs1.SelectedIndexChanged += new IntEventHandler(tabs1_SelectedIndexChanged);
		((Control)tabPage1).BackColor = Color.Transparent;
		((Control)tabPage1).Controls.Add((Control)(object)pdfViewer1);
		((Control)tabPage1).Dock = (DockStyle)5;
		((Control)tabPage1).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)tabPage1).ForeColor = Color.FromArgb(255, 233, 0);
		((Control)tabPage1).Location = new Point(0, 37);
		((Control)tabPage1).Margin = new Padding(0);
		((Control)tabPage1).Name = "tabPage1";
		((Control)tabPage1).Padding = new Padding(10);
		((Control)tabPage1).Size = new Size(772, 479);
		((Control)tabPage1).TabIndex = 0;
		((Control)tabPage1).Text = "111";
		((Control)pdfViewer1).Dock = (DockStyle)5;
		((Control)pdfViewer1).Location = new Point(10, 10);
		((Control)pdfViewer1).Margin = new Padding(0);
		((Control)pdfViewer1).Name = "pdfViewer1";
		pdfViewer1.ShowBookmarks = false;
		((Control)pdfViewer1).Size = new Size(752, 459);
		((Control)pdfViewer1).TabIndex = 0;
		((Control)pdfViewer1).TabStop = false;
		pdfViewer1.ZoomMode = (PdfViewerZoomMode)1;
		((Control)tabPage2).Dock = (DockStyle)5;
		((Control)tabPage2).Location = new Point(0, 37);
		((Control)tabPage2).Name = "tabPage2";
		((Control)tabPage2).Size = new Size(772, 479);
		((Control)tabPage2).TabIndex = 1;
		((Control)tabPage3).Dock = (DockStyle)5;
		((Control)tabPage3).Location = new Point(0, 37);
		((Control)tabPage3).Name = "tabPage3";
		((Control)tabPage3).Size = new Size(772, 479);
		((Control)tabPage3).TabIndex = 2;
		((ContainerPanel)gridPanel1).Back = Color.Transparent;
		((Control)gridPanel1).BackColor = Color.Transparent;
		((Control)gridPanel1).Controls.Add((Control)(object)button1);
		((Control)gridPanel1).Controls.Add((Control)(object)label1);
		((Control)gridPanel1).Controls.Add((Control)(object)tabs1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(772, 573);
		gridPanel1.Span = "92% 8%;100%-10% 90%";
		((Control)gridPanel1).TabIndex = 2;
		((Control)gridPanel1).Text = "gridPanel1";
		button1.BackColor = Color.Transparent;
		button1.DefaultBack = Color.Transparent;
		button1.DefaultBorderColor = Color.FromArgb(235, 237, 240);
		button1.DisplayStyle = (TButtonDisplayStyle)2;
		button1.Icon = (Image)(object)Resources.关闭1;
		button1.IconRatio = 1f;
		gridPanel1.SetIndex((Control)(object)button1, 2);
		((Control)button1).Location = new Point(710, 0);
		((Control)button1).Margin = new Padding(0);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(62, 57);
		((Control)button1).TabIndex = 3;
		button1.WaveSize = 0;
		((Control)button1).MouseClick += new MouseEventHandler(pictureBox1_MouseClick);
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Font = new Font("阿里巴巴普惠体 Medium", 30f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		label1.ForeColor = Color.White;
		((IControl)label1).HandDragFolder = false;
		gridPanel1.SetIndex((Control)(object)label1, 1);
		((Control)label1).Location = new Point(0, 0);
		((Control)label1).Margin = new Padding(0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(710, 57);
		((Control)label1).TabIndex = 0;
		((Control)label1).TabStop = false;
		((Control)label1).Text = "  Ascent Lite VTX";
		label1.TextAlign = (ContentAlignment)32;
		((Control)label1).Click += label1_Click;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Name = "UserMaualPdfCtrl";
		((Control)this).Size = new Size(772, 573);
		((UserControl)this).Load += UserMaualPdfCtrl_Load;
		((Control)tabs1).ResumeLayout(false);
		((Control)tabPage1).ResumeLayout(false);
		((Control)gridPanel1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
