using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class NulllDeviceCtrl : UserControl
{
	private string _deviceName;

	private string _portName;

	private IContainer components = null;

	private TableLayoutPanel tableLayoutPanel1;

	private PictureBox pictureBox1;

	private Label label1;

	public event EventHandler<DevCardEventArgs> OnNulllDeviceHappenEvent;

	public NulllDeviceCtrl()
	{
		InitializeComponent();
	}

	public void SetDeviceName(string name)
	{
		_deviceName = name;
		((Control)this).Invoke((Delegate)(Action)delegate
		{
		});
	}

	public void SetDeviceName(string devName, string portName)
	{
		_deviceName = devName;
		_portName = portName;
		((Control)this).Invoke((Delegate)(Action)delegate
		{
		});
	}

	public void SetDeviceName(bool isshow, string name, string btnName)
	{
		_deviceName = name;
		ReloadFont();
	}

	private void button1_Click(object sender, EventArgs e)
	{
		DevCardEventArgs e2 = new DevCardEventArgs();
		OnNulllDeviceHappenEvent?.Invoke(null, e2);
	}

	public void SetLocation()
	{
	}

	private void label1_MouseDoubleClick(object sender, MouseEventArgs e)
	{
	}

	private void DeviceInfoCard_Load(object sender, EventArgs e)
	{
		ReloadFont();
	}

	public void ReloadFont()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			((Control)label1).Font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 20f);
		});
		ReloadLang();
	}

	public void ReloadLang()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)label1).Text = Lang.T("null_device.tip_connect_usb");
		});
	}

	public void FontChange(bool isAdd)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			Font font = ((Control)label1).Font;
			float size = font.Size;
			size = ((!isAdd) ? (size - 2f) : (size + 2f));
			Font font2 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], size);
			((Control)label1).Font = font2;
		});
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
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Expected O, but got Unknown
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		tableLayoutPanel1 = new TableLayoutPanel();
		label1 = new Label();
		pictureBox1 = new PictureBox();
		((Control)tableLayoutPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((Control)tableLayoutPanel1).BackColor = Color.FromArgb(38, 41, 43);
		tableLayoutPanel1.ColumnCount = 1;
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 100f));
		tableLayoutPanel1.Controls.Add((Control)(object)pictureBox1, 0, 1);
		tableLayoutPanel1.Controls.Add((Control)(object)label1, 0, 2);
		((Control)tableLayoutPanel1).Dock = (DockStyle)5;
		((Control)tableLayoutPanel1).Location = new Point(0, 0);
		((Control)tableLayoutPanel1).Name = "tableLayoutPanel1";
		tableLayoutPanel1.RowCount = 4;
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 20f));
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 35f));
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 35f));
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 10f));
		((Control)tableLayoutPanel1).Size = new Size(1202, 912);
		((Control)tableLayoutPanel1).TabIndex = 11;
		((Control)label1).Dock = (DockStyle)5;
		((Control)label1).Font = new Font("阿里巴巴普惠体", 32.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label1.ForeColor = Color.FromArgb(255, 233, 0);
		((Control)label1).Location = new Point(0, 501);
		((Control)label1).Margin = new Padding(0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(1202, 319);
		((Control)label1).TabIndex = 12;
		((Control)label1).Text = "请连接USB设备";
		label1.TextAlign = (ContentAlignment)2;
		((Control)label1).MouseClick += new MouseEventHandler(label1_MouseDoubleClick);
		((Control)pictureBox1).Dock = (DockStyle)5;
		pictureBox1.Image = (Image)(object)Resources.未连接设备;
		((Control)pictureBox1).Location = new Point(0, 182);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(1202, 319);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 11;
		pictureBox1.TabStop = false;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).BackgroundImageLayout = (ImageLayout)4;
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).DoubleBuffered = true;
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "NulllDeviceCtrl";
		((Control)this).Size = new Size(1202, 912);
		((UserControl)this).Load += DeviceInfoCard_Load;
		((Control)tableLayoutPanel1).ResumeLayout(false);
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
