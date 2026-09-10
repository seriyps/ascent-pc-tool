using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class DownloadFW : UserControl
{
	private IContainer components = null;

	private TableLayoutPanel tableLayoutPanel1;

	private PageHeader pageHeader1;

	private Label lab_tempture;

	private Label lab_sn;

	private Label lab_firmwareVer;

	private Label lab_hardwareVer;

	private Button btn_Disconnect;

	private Label lab_devName;

	private Button btn_loadFromPC;

	private Button btn_question;

	private PictureBox pictureBox1;

	private Table table1;

	private Pagination pagination1;

	public DownloadFW()
	{
		InitializeComponent();
	}

	private void DownloadFW_Load(object sender, EventArgs e)
	{
		InitTable();
	}

	private void InitTable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0069: Expected O, but got Unknown
		Table obj = table1;
		ColumnCollection val = new ColumnCollection();
		val.Add(new Column("Id", "ID"));
		val.Add(new Column("Name", "姓名"));
		val.Add(new Column("Age", "年龄"));
		val.Add(new Column("Address", "地址"));
		obj.Columns = val;
		table1.DataSource = new List<User>
		{
			new User
			{
				Id = 1,
				Name = "Tom",
				Age = 20,
				Address = "Shanghai"
			},
			new User
			{
				Id = 2,
				Name = "Jerry",
				Age = 25,
				Address = "Beijing"
			},
			new User
			{
				Id = 3,
				Name = "Alice",
				Age = 30,
				Address = "Shenzhen"
			}
		};
	}

	private object GetPageData(int current, int pageSize)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		List<TestClass> list = new List<TestClass>(pageSize);
		int num = Math.Abs(current - 1) * pageSize;
		DateTime now = DateTime.Now;
		DateTime dateTime = new DateTime(1983, 7, 4);
		DateTime dateTime2 = new DateTime(1974, 9, 30);
		list.Add(new TestClass(num, 0, Localization.Get("Table.Data.Name1", "胡彦斌"), (int)Math.Round((now - dateTime).TotalDays / 365.0)));
		list.Add(new TestClass(num + 1, 1, Localization.Get("Table.Data.Name2", "吴彦祖"), (int)Math.Round((now - dateTime2).TotalDays / 365.0))
		{
			tag = (CellTag[])(object)new CellTag[2]
			{
				new CellTag("NICE", (TTypeMini)2),
				new CellTag("DEVELOPER", (TTypeMini)5)
			}
		});
		for (int i = 2; i < pageSize; i++)
		{
			int num2 = num + i;
			list.Add(new TestClass(num2, i, Localization.Get("Table.Data.Name3", "胡彦祖"), 20 + num2));
		}
		return list;
	}

	private object GetPageData(int current, int pageSize, bool nouse)
	{
		List<FirmwareInfo> list = new List<FirmwareInfo>(pageSize);
		int num = Math.Abs(current - 1) * pageSize;
		list.Add(new FirmwareInfo());
		list.Add(new FirmwareInfo());
		for (int i = 2; i < pageSize; i++)
		{
			int num2 = num + i;
			list.Add(new FirmwareInfo($"{i}.{i + 1}.5", "9-8-7", "abcde"));
		}
		return list;
	}

	private void table1_CellButtonClick(object sender, TableButtonEventArgs e)
	{
		WriteLog.WriteLogFileToUI("sss", Color.Red);
		string id = e.Btn.Id;
		id = e.Btn.Text;
		TestClass testClass = (TestClass)((ITableMouseNullEventArgs)e).Record;
		id = testClass.name;
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
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Expected O, but got Unknown
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Expected O, but got Unknown
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Expected O, but got Unknown
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Expected O, but got Unknown
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Expected O, but got Unknown
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Expected O, but got Unknown
		//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_099e: Expected O, but got Unknown
		//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aea: Expected O, but got Unknown
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc3: Expected O, but got Unknown
		//IL_0cf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d62: Expected O, but got Unknown
		tableLayoutPanel1 = new TableLayoutPanel();
		pageHeader1 = new PageHeader();
		lab_tempture = new Label();
		lab_sn = new Label();
		lab_firmwareVer = new Label();
		lab_hardwareVer = new Label();
		btn_Disconnect = new Button();
		lab_devName = new Label();
		btn_loadFromPC = new Button();
		btn_question = new Button();
		pictureBox1 = new PictureBox();
		table1 = new Table();
		pagination1 = new Pagination();
		((Control)tableLayoutPanel1).SuspendLayout();
		((Control)pageHeader1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		((Control)tableLayoutPanel1).BackColor = Color.White;
		tableLayoutPanel1.ColumnCount = 1;
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 100f));
		tableLayoutPanel1.Controls.Add((Control)(object)pageHeader1, 0, 0);
		tableLayoutPanel1.Controls.Add((Control)(object)table1, 0, 1);
		tableLayoutPanel1.Controls.Add((Control)(object)pagination1, 0, 2);
		((Control)tableLayoutPanel1).Dock = (DockStyle)5;
		((Control)tableLayoutPanel1).Location = new Point(0, 0);
		((Control)tableLayoutPanel1).Margin = new Padding(0);
		((Control)tableLayoutPanel1).Name = "tableLayoutPanel1";
		tableLayoutPanel1.RowCount = 3;
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 23.80952f));
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 76.19048f));
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)1, 50f));
		((Control)tableLayoutPanel1).Size = new Size(840, 640);
		((Control)tableLayoutPanel1).TabIndex = 0;
		((Control)pageHeader1).Controls.Add((Control)(object)lab_tempture);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_sn);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_firmwareVer);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_hardwareVer);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_Disconnect);
		((Control)pageHeader1).Controls.Add((Control)(object)lab_devName);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_loadFromPC);
		((Control)pageHeader1).Controls.Add((Control)(object)btn_question);
		((Control)pageHeader1).Controls.Add((Control)(object)pictureBox1);
		((Control)pageHeader1).Dock = (DockStyle)5;
		pageHeader1.DragMove = false;
		pageHeader1.Gap = 0;
		((Control)pageHeader1).Location = new Point(5, 5);
		((Control)pageHeader1).Margin = new Padding(5);
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Size = new Size(830, 130);
		((Control)pageHeader1).TabIndex = 53;
		((Control)pageHeader1).TabStop = false;
		((Control)pageHeader1).Text = "";
		lab_tempture.AutoSizeMode = (TAutoSize)1;
		((Control)lab_tempture).BackColor = Color.Transparent;
		((Control)lab_tempture).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_tempture).Location = new Point(435, 99);
		((Control)lab_tempture).Margin = new Padding(0);
		((Control)lab_tempture).Name = "lab_tempture";
		((Control)lab_tempture).Size = new Size(87, 27);
		((Control)lab_tempture).TabIndex = 34;
		((Control)lab_tempture).TabStop = false;
		((Control)lab_tempture).Text = "芯片温度：";
		lab_tempture.TextAlign = (ContentAlignment)32;
		lab_tempture.TextMultiLine = false;
		lab_sn.AutoSizeMode = (TAutoSize)1;
		((Control)lab_sn).BackColor = Color.Transparent;
		((Control)lab_sn).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_sn).Location = new Point(435, 58);
		((Control)lab_sn).Margin = new Padding(0);
		((Control)lab_sn).Name = "lab_sn";
		((Control)lab_sn).Size = new Size(32, 27);
		((Control)lab_sn).TabIndex = 33;
		((Control)lab_sn).TabStop = false;
		((Control)lab_sn).Text = "SN:";
		lab_sn.TextAlign = (ContentAlignment)32;
		lab_sn.TextMultiLine = false;
		lab_firmwareVer.AutoSizeMode = (TAutoSize)1;
		((Control)lab_firmwareVer).BackColor = Color.Transparent;
		((Control)lab_firmwareVer).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_firmwareVer).Location = new Point(154, 99);
		((Control)lab_firmwareVer).Margin = new Padding(0);
		((Control)lab_firmwareVer).Name = "lab_firmwareVer";
		((Control)lab_firmwareVer).Size = new Size(38, 27);
		((Control)lab_firmwareVer).TabIndex = 32;
		((Control)lab_firmwareVer).TabStop = false;
		((Control)lab_firmwareVer).Text = "固件";
		lab_firmwareVer.TextAlign = (ContentAlignment)32;
		lab_firmwareVer.TextMultiLine = false;
		lab_hardwareVer.AutoSizeMode = (TAutoSize)1;
		((Control)lab_hardwareVer).BackColor = Color.Transparent;
		((Control)lab_hardwareVer).Font = new Font("阿里巴巴普惠体", 14.25f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_hardwareVer).Location = new Point(154, 58);
		((Control)lab_hardwareVer).Margin = new Padding(0);
		((Control)lab_hardwareVer).Name = "lab_hardwareVer";
		((Control)lab_hardwareVer).Size = new Size(94, 27);
		((Control)lab_hardwareVer).TabIndex = 31;
		((Control)lab_hardwareVer).TabStop = false;
		((Control)lab_hardwareVer).Text = "硬件版本号";
		lab_hardwareVer.TextAlign = (ContentAlignment)32;
		lab_hardwareVer.TextMultiLine = false;
		btn_Disconnect.AutoSizeMode = (TAutoSize)1;
		btn_Disconnect.BackColor = Color.White;
		btn_Disconnect.BorderWidth = 2f;
		btn_Disconnect.DefaultBack = Color.FromArgb(26, 66, 130, 248);
		((Control)btn_Disconnect).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_Disconnect.ForeColor = Color.FromArgb(66, 130, 248);
		btn_Disconnect.Icon = (Image)(object)Resources.断开连接按钮;
		btn_Disconnect.IconGap = 0.3f;
		btn_Disconnect.IconPosition = (TAlignMini)0;
		btn_Disconnect.IconRatio = 0.8f;
		btn_Disconnect.IconSvg = "";
		((Control)btn_Disconnect).Location = new Point(435, 10);
		((Control)btn_Disconnect).Margin = new Padding(0);
		((Control)btn_Disconnect).Name = "btn_Disconnect";
		((Control)btn_Disconnect).Size = new Size(97, 40);
		((Control)btn_Disconnect).TabIndex = 30;
		((Control)btn_Disconnect).Text = "断开连接";
		btn_Disconnect.WaveSize = 0;
		lab_devName.AutoSizeMode = (TAutoSize)1;
		((Control)lab_devName).BackColor = Color.Transparent;
		((Control)lab_devName).Font = new Font("阿里巴巴普惠体 Medium", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_devName).Location = new Point(152, 10);
		((Control)lab_devName).Margin = new Padding(15, 0, 0, 0);
		((Control)lab_devName).Name = "lab_devName";
		((Control)lab_devName).Size = new Size(183, 38);
		((Control)lab_devName).TabIndex = 1;
		((Control)lab_devName).TabStop = false;
		((Control)lab_devName).Text = "Ascent GT Pro";
		lab_devName.TextAlign = (ContentAlignment)32;
		lab_devName.TextMultiLine = false;
		btn_loadFromPC.AutoSizeMode = (TAutoSize)1;
		btn_loadFromPC.BackColor = Color.White;
		btn_loadFromPC.BorderWidth = 2f;
		btn_loadFromPC.DefaultBack = Color.White;
		((Control)btn_loadFromPC).Dock = (DockStyle)4;
		((Control)btn_loadFromPC).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_loadFromPC.ForeColor = Color.FromArgb(153, 153, 153);
		btn_loadFromPC.IconGap = 0f;
		btn_loadFromPC.IconRatio = 0f;
		btn_loadFromPC.IconSvg = "";
		((Control)btn_loadFromPC).Location = new Point(592, 0);
		((Control)btn_loadFromPC).Margin = new Padding(0);
		((Control)btn_loadFromPC).Name = "btn_loadFromPC";
		((Control)btn_loadFromPC).Size = new Size(125, 48);
		((Control)btn_loadFromPC).TabIndex = 27;
		((Control)btn_loadFromPC).Text = "本地加载固件";
		((IControl)btn_loadFromPC).Visible = false;
		btn_question.AutoSizeMode = (TAutoSize)1;
		btn_question.BackColor = Color.White;
		btn_question.BorderWidth = 2f;
		btn_question.DefaultBack = Color.White;
		((Control)btn_question).Dock = (DockStyle)4;
		((Control)btn_question).Font = new Font("阿里巴巴普惠体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_question.ForeColor = Color.FromArgb(153, 153, 153);
		btn_question.Icon = (Image)(object)Resources.语言选择;
		btn_question.IconGap = 0.3f;
		btn_question.IconRatio = 1f;
		btn_question.IconSvg = "";
		((Control)btn_question).Location = new Point(717, 0);
		((Control)btn_question).Margin = new Padding(0);
		((Control)btn_question).Name = "btn_question";
		((Control)btn_question).Size = new Size(113, 44);
		((Control)btn_question).TabIndex = 28;
		((Control)btn_question).Text = "常见问题";
		((IControl)btn_question).Visible = false;
		btn_question.WaveSize = 0;
		((Control)pictureBox1).BackColor = Color.White;
		((Control)pictureBox1).Dock = (DockStyle)3;
		pictureBox1.Image = (Image)(object)Resources.固件升级_default;
		((Control)pictureBox1).Location = new Point(0, 0);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(131, 130);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 23;
		pictureBox1.TabStop = false;
		((Control)table1).BackColor = Color.Silver;
		((Control)table1).Dock = (DockStyle)5;
		((Control)table1).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		table1.Gap = 12;
		((Control)table1).Location = new Point(5, 145);
		((Control)table1).Margin = new Padding(5);
		((Control)table1).Name = "table1";
		((Control)table1).Size = new Size(830, 439);
		((Control)table1).TabIndex = 54;
		((Control)table1).Text = "table1";
		table1.CellButtonClick += new ClickButtonEventHandler(table1_CellButtonClick);
		((Control)pagination1).BackColor = Color.Silver;
		((Control)pagination1).Dock = (DockStyle)5;
		((Control)pagination1).Location = new Point(3, 592);
		pagination1.MaxPageTotal = 20;
		((Control)pagination1).Name = "pagination1";
		((Control)pagination1).RightToLeft = (RightToLeft)1;
		((Control)pagination1).Size = new Size(834, 45);
		((Control)pagination1).TabIndex = 55;
		((Control)pagination1).TabStop = false;
		((Control)pagination1).Text = "1234";
		pagination1.Total = 200;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.White;
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "DownloadFW";
		((Control)this).Size = new Size(840, 640);
		((UserControl)this).Load += DownloadFW_Load;
		((Control)tableLayoutPanel1).ResumeLayout(false);
		((Control)pageHeader1).ResumeLayout(false);
		((Control)pageHeader1).PerformLayout();
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
