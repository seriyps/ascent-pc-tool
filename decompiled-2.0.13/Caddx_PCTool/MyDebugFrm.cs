using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using AntdUI;

namespace Caddx_PCTool;

public class MyDebugFrm : Form
{
	public List<Label> antdLab;

	public List<Label> frmLab = new List<Label>();

	private PrivateFontCollection oppoM;

	private PrivateFontCollection oppoR;

	private PrivateFontCollection aliM;

	private PrivateFontCollection aliR;

	private PrivateFontCollection hanM;

	private PrivateFontCollection hanR;

	private IContainer components = null;

	private MenuStrip menuStrip1;

	private ToolStripMenuItem 字体ToolStripMenuItem;

	private ToolStripMenuItem 改oppo字体ToolStripMenuItem;

	private ToolStripMenuItem 改阿里字体ToolStripMenuItem;

	private ToolStripMenuItem 字号加2ToolStripMenuItem;

	private ToolStripMenuItem 字号减2ToolStripMenuItem;

	private Label lab_ali2;

	private ToolStripMenuItem 改Han字体ToolStripMenuItem;

	private Label lab_oppo2;

	private Label lab_han1;

	private Label lab_han2;

	private Label lab_ali;

	private Label lab_oppo;

	private Label lab_han;

	private Label lab_fontSize;

	private ToolStripMenuItem 恢复默认ToolStripMenuItem;

	private TableLayoutPanel tableLayoutPanel1;

	private Panel panel1;

	private Label lab_oppo1;

	private Label lab_ali1;

	public MyDebugFrm()
	{
		InitializeComponent();
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)2;
	}

	private void LoadFont()
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
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		oppoM = new PrivateFontCollection();
		oppoR = new PrivateFontCollection();
		aliM = new PrivateFontCollection();
		aliR = new PrivateFontCollection();
		hanM = new PrivateFontCollection();
		hanR = new PrivateFontCollection();
		oppoM.AddFontFile(Application.StartupPath + "\\font\\OPPOSans-M.ttf");
		oppoR.AddFontFile(Application.StartupPath + "\\font\\OPPOSans-R.ttf");
		aliM.AddFontFile(Application.StartupPath + "\\font\\Medium.ttf");
		aliR.AddFontFile(Application.StartupPath + "\\font\\Regular.ttf");
		hanM.AddFontFile(Application.StartupPath + "\\font\\SourceHanSans-Medium.ttf");
		hanR.AddFontFile(Application.StartupPath + "\\font\\SourceHanSans-Regular.ttf");
		((Control)lab_ali).Font = new Font(((FontCollection)aliR).Families[0], 20f);
		((Control)lab_oppo).Font = new Font(((FontCollection)oppoR).Families[0], 20f);
		((Control)lab_han).Font = new Font(((FontCollection)hanR).Families[0], 20f);
		((Control)lab_ali2).Font = new Font(((FontCollection)aliM).Families[0], 16f);
		((Control)lab_oppo2).Font = new Font(((FontCollection)oppoM).Families[0], 16f);
		((Control)lab_han1).Font = new Font(((FontCollection)hanR).Families[0], 16f);
		((Control)lab_han2).Font = new Font(((FontCollection)hanM).Families[0], 16f);
	}

	private void MyDebugFrm_Load(object sender, EventArgs e)
	{
		LoadFont();
		antdLab = new List<Label> { lab_ali1, lab_oppo1, lab_han1 };
		frmLab = new List<Label> { lab_ali2, lab_oppo2, lab_han2 };
	}

	private void 改oppo字体ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		Font font = ((Control)lab_ali1).Font;
		for (int i = 0; i < antdLab.Count; i++)
		{
			((Control)antdLab[i]).Font = new Font(((FontCollection)oppoR).Families[0], font.Size);
			((Control)frmLab[i]).Font = new Font(((FontCollection)oppoM).Families[0], font.Size);
		}
	}

	private void 改阿里字体ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		Font font = ((Control)lab_ali1).Font;
		for (int i = 0; i < antdLab.Count; i++)
		{
			((Control)antdLab[i]).Font = new Font(((FontCollection)aliR).Families[0], font.Size);
			((Control)frmLab[i]).Font = new Font(((FontCollection)aliM).Families[0], font.Size);
		}
	}

	private void 字号加2ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		Font font = ((Control)lab_ali1).Font;
		float num = font.Size + 2f;
		for (int i = 0; i < antdLab.Count; i++)
		{
			((Control)antdLab[i]).Font = new Font(((Control)antdLab[i]).Font.FontFamily, num);
			((Control)frmLab[i]).Font = new Font(((Control)frmLab[i]).Font.FontFamily, num);
		}
		((Control)lab_fontSize).Text = "当前字号=" + num;
	}

	private void 字号减2ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		Font font = ((Control)lab_ali1).Font;
		float num = font.Size - 2f;
		for (int i = 0; i < antdLab.Count; i++)
		{
			((Control)antdLab[i]).Font = new Font(((Control)antdLab[i]).Font.FontFamily, num);
			((Control)frmLab[i]).Font = new Font(((Control)frmLab[i]).Font.FontFamily, num);
		}
		((Control)lab_fontSize).Text = "当前字号=" + num;
	}

	private void 改Han字体ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		Font font = ((Control)lab_ali1).Font;
		for (int i = 0; i < antdLab.Count; i++)
		{
			((Control)antdLab[i]).Font = new Font(((FontCollection)hanR).Families[0], font.Size);
			((Control)frmLab[i]).Font = new Font(((FontCollection)hanM).Families[0], font.Size);
		}
	}

	private void 恢复默认ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		((Control)lab_ali1).Font = new Font(((FontCollection)aliR).Families[0], 16f);
		((Control)lab_ali2).Font = new Font(((FontCollection)aliM).Families[0], 16f);
		((Control)lab_oppo1).Font = new Font(((FontCollection)oppoR).Families[0], 16f);
		((Control)lab_oppo2).Font = new Font(((FontCollection)oppoM).Families[0], 16f);
		((Control)lab_han1).Font = new Font(((FontCollection)hanR).Families[0], 16f);
		((Control)lab_han2).Font = new Font(((FontCollection)hanM).Families[0], 16f);
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
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Expected O, but got Unknown
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Expected O, but got Unknown
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Expected O, but got Unknown
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_0852: Expected O, but got Unknown
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Expected O, but got Unknown
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f0: Expected O, but got Unknown
		//IL_0902: Unknown result type (might be due to invalid IL or missing references)
		//IL_090c: Expected O, but got Unknown
		//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Expected O, but got Unknown
		//IL_0a6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a76: Expected O, but got Unknown
		//IL_0a88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a92: Expected O, but got Unknown
		//IL_0b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c31: Unknown result type (might be due to invalid IL or missing references)
		menuStrip1 = new MenuStrip();
		字体ToolStripMenuItem = new ToolStripMenuItem();
		改oppo字体ToolStripMenuItem = new ToolStripMenuItem();
		改阿里字体ToolStripMenuItem = new ToolStripMenuItem();
		改Han字体ToolStripMenuItem = new ToolStripMenuItem();
		恢复默认ToolStripMenuItem = new ToolStripMenuItem();
		字号加2ToolStripMenuItem = new ToolStripMenuItem();
		字号减2ToolStripMenuItem = new ToolStripMenuItem();
		lab_ali2 = new Label();
		lab_oppo2 = new Label();
		lab_han1 = new Label();
		lab_han2 = new Label();
		lab_ali = new Label();
		lab_oppo = new Label();
		lab_han = new Label();
		lab_fontSize = new Label();
		tableLayoutPanel1 = new TableLayoutPanel();
		panel1 = new Panel();
		lab_ali1 = new Label();
		lab_oppo1 = new Label();
		((Control)menuStrip1).SuspendLayout();
		((Control)tableLayoutPanel1).SuspendLayout();
		((Control)panel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((ToolStrip)menuStrip1).Items.AddRange((ToolStripItem[])(object)new ToolStripItem[3]
		{
			(ToolStripItem)字体ToolStripMenuItem,
			(ToolStripItem)字号加2ToolStripMenuItem,
			(ToolStripItem)字号减2ToolStripMenuItem
		});
		((Control)menuStrip1).Location = new Point(0, 0);
		((Control)menuStrip1).Name = "menuStrip1";
		((Control)menuStrip1).Size = new Size(866, 25);
		((Control)menuStrip1).TabIndex = 0;
		((Control)menuStrip1).Text = "menuStrip1";
		((ToolStripDropDownItem)字体ToolStripMenuItem).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[4]
		{
			(ToolStripItem)改oppo字体ToolStripMenuItem,
			(ToolStripItem)改阿里字体ToolStripMenuItem,
			(ToolStripItem)改Han字体ToolStripMenuItem,
			(ToolStripItem)恢复默认ToolStripMenuItem
		});
		((ToolStripItem)字体ToolStripMenuItem).Name = "字体ToolStripMenuItem";
		((ToolStripItem)字体ToolStripMenuItem).Size = new Size(44, 21);
		((ToolStripItem)字体ToolStripMenuItem).Text = "字体";
		((ToolStripItem)改oppo字体ToolStripMenuItem).Name = "改oppo字体ToolStripMenuItem";
		((ToolStripItem)改oppo字体ToolStripMenuItem).Size = new Size(156, 22);
		((ToolStripItem)改oppo字体ToolStripMenuItem).Text = "全改oppo字体";
		((ToolStripItem)改oppo字体ToolStripMenuItem).Click += 改oppo字体ToolStripMenuItem_Click;
		((ToolStripItem)改阿里字体ToolStripMenuItem).Name = "改阿里字体ToolStripMenuItem";
		((ToolStripItem)改阿里字体ToolStripMenuItem).Size = new Size(156, 22);
		((ToolStripItem)改阿里字体ToolStripMenuItem).Text = "全改阿里字体";
		((ToolStripItem)改阿里字体ToolStripMenuItem).Click += 改阿里字体ToolStripMenuItem_Click;
		((ToolStripItem)改Han字体ToolStripMenuItem).Name = "改Han字体ToolStripMenuItem";
		((ToolStripItem)改Han字体ToolStripMenuItem).Size = new Size(156, 22);
		((ToolStripItem)改Han字体ToolStripMenuItem).Text = "全改Han字体";
		((ToolStripItem)改Han字体ToolStripMenuItem).Click += 改Han字体ToolStripMenuItem_Click;
		((ToolStripItem)恢复默认ToolStripMenuItem).Name = "恢复默认ToolStripMenuItem";
		((ToolStripItem)恢复默认ToolStripMenuItem).Size = new Size(156, 22);
		((ToolStripItem)恢复默认ToolStripMenuItem).Text = "恢复默认";
		((ToolStripItem)恢复默认ToolStripMenuItem).Click += 恢复默认ToolStripMenuItem_Click;
		((ToolStripItem)字号加2ToolStripMenuItem).Name = "字号加2ToolStripMenuItem";
		((ToolStripItem)字号加2ToolStripMenuItem).Size = new Size(63, 21);
		((ToolStripItem)字号加2ToolStripMenuItem).Text = "字号加2";
		((ToolStripItem)字号加2ToolStripMenuItem).Click += 字号加2ToolStripMenuItem_Click;
		((ToolStripItem)字号减2ToolStripMenuItem).Name = "字号减2ToolStripMenuItem";
		((ToolStripItem)字号减2ToolStripMenuItem).Size = new Size(63, 21);
		((ToolStripItem)字号减2ToolStripMenuItem).Text = "字号减2";
		((ToolStripItem)字号减2ToolStripMenuItem).Click += 字号减2ToolStripMenuItem_Click;
		((Control)lab_ali2).BackColor = Color.White;
		((Control)lab_ali2).Dock = (DockStyle)5;
		((Control)lab_ali2).Location = new Point(5, 283);
		((Control)lab_ali2).Margin = new Padding(5);
		((Control)lab_ali2).Name = "lab_ali2";
		((Control)lab_ali2).Size = new Size(278, 229);
		((Control)lab_ali2).TabIndex = 2;
		((Control)lab_ali2).Text = "壹贰叁肆1234ABCD";
		lab_ali2.TextAlign = (ContentAlignment)32;
		((Control)lab_oppo2).BackColor = Color.White;
		((Control)lab_oppo2).Dock = (DockStyle)5;
		((Control)lab_oppo2).Location = new Point(293, 283);
		((Control)lab_oppo2).Margin = new Padding(5);
		((Control)lab_oppo2).Name = "lab_oppo2";
		((Control)lab_oppo2).Size = new Size(278, 229);
		((Control)lab_oppo2).TabIndex = 4;
		((Control)lab_oppo2).Text = "壹贰叁肆1234ABCD";
		lab_oppo2.TextAlign = (ContentAlignment)32;
		((Control)lab_han1).BackColor = Color.White;
		((Control)lab_han1).Dock = (DockStyle)5;
		((Control)lab_han1).Location = new Point(581, 45);
		((Control)lab_han1).Margin = new Padding(5);
		((Control)lab_han1).Name = "lab_han1";
		((Control)lab_han1).Size = new Size(280, 228);
		((Control)lab_han1).TabIndex = 5;
		((Control)lab_han1).Text = "壹贰叁肆1234ABCD";
		lab_han1.TextAlign = (ContentAlignment)32;
		((Control)lab_han2).BackColor = Color.White;
		((Control)lab_han2).Dock = (DockStyle)5;
		((Control)lab_han2).Location = new Point(581, 283);
		((Control)lab_han2).Margin = new Padding(5);
		((Control)lab_han2).Name = "lab_han2";
		((Control)lab_han2).Size = new Size(280, 229);
		((Control)lab_han2).TabIndex = 6;
		((Control)lab_han2).Text = "壹贰叁肆1234ABCD";
		lab_han2.TextAlign = (ContentAlignment)32;
		((Control)lab_ali).AutoSize = true;
		((Control)lab_ali).Font = new Font("阿里巴巴普惠体", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_ali).Location = new Point(3, 0);
		((Control)lab_ali).Name = "lab_ali";
		((Control)lab_ali).Size = new Size(55, 29);
		((Control)lab_ali).TabIndex = 9;
		((Control)lab_ali).Text = "阿里";
		((Control)lab_oppo).AutoSize = true;
		((Control)lab_oppo).Font = new Font("阿里巴巴普惠体", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_oppo).Location = new Point(291, 0);
		((Control)lab_oppo).Name = "lab_oppo";
		((Control)lab_oppo).Size = new Size(65, 29);
		((Control)lab_oppo).TabIndex = 10;
		((Control)lab_oppo).Text = "oppo";
		((Control)lab_han).AutoSize = true;
		((Control)lab_han).Dock = (DockStyle)3;
		((Control)lab_han).Font = new Font("思源黑体", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_han).Location = new Point(0, 0);
		((Control)lab_han).Name = "lab_han";
		((Control)lab_han).Size = new Size(53, 30);
		((Control)lab_han).TabIndex = 11;
		((Control)lab_han).Text = "Han";
		((Control)lab_fontSize).AutoSize = true;
		((Control)lab_fontSize).Dock = (DockStyle)3;
		((Control)lab_fontSize).Font = new Font("思源黑体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)128);
		((Control)lab_fontSize).Location = new Point(53, 0);
		((Control)lab_fontSize).Name = "lab_fontSize";
		((Control)lab_fontSize).Size = new Size(101, 23);
		((Control)lab_fontSize).TabIndex = 12;
		((Control)lab_fontSize).Text = "当前字号=16";
		tableLayoutPanel1.ColumnCount = 3;
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 33.33333f));
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 33.33334f));
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 33.33334f));
		tableLayoutPanel1.Controls.Add((Control)(object)lab_oppo1, 1, 1);
		tableLayoutPanel1.Controls.Add((Control)(object)lab_ali1, 0, 1);
		tableLayoutPanel1.Controls.Add((Control)(object)lab_han1, 2, 1);
		tableLayoutPanel1.Controls.Add((Control)(object)lab_han2, 2, 2);
		tableLayoutPanel1.Controls.Add((Control)(object)lab_oppo, 1, 0);
		tableLayoutPanel1.Controls.Add((Control)(object)lab_oppo2, 1, 2);
		tableLayoutPanel1.Controls.Add((Control)(object)lab_ali, 0, 0);
		tableLayoutPanel1.Controls.Add((Control)(object)lab_ali2, 0, 2);
		tableLayoutPanel1.Controls.Add((Control)(object)panel1, 2, 0);
		((Control)tableLayoutPanel1).Dock = (DockStyle)5;
		((Control)tableLayoutPanel1).Location = new Point(0, 25);
		((Control)tableLayoutPanel1).Margin = new Padding(0);
		((Control)tableLayoutPanel1).Name = "tableLayoutPanel1";
		tableLayoutPanel1.RowCount = 3;
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)1, 40f));
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 50f));
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 50f));
		((Control)tableLayoutPanel1).Size = new Size(866, 517);
		((Control)tableLayoutPanel1).TabIndex = 13;
		((Control)panel1).Controls.Add((Control)(object)lab_fontSize);
		((Control)panel1).Controls.Add((Control)(object)lab_han);
		((Control)panel1).Dock = (DockStyle)5;
		((Control)panel1).Location = new Point(576, 0);
		((Control)panel1).Margin = new Padding(0);
		((Control)panel1).Name = "panel1";
		((Control)panel1).Size = new Size(290, 40);
		((Control)panel1).TabIndex = 11;
		((Control)lab_ali1).BackColor = Color.White;
		((Control)lab_ali1).Dock = (DockStyle)5;
		((Control)lab_ali1).Location = new Point(5, 45);
		((Control)lab_ali1).Margin = new Padding(5);
		((Control)lab_ali1).Name = "lab_ali1";
		((Control)lab_ali1).Size = new Size(278, 228);
		((Control)lab_ali1).TabIndex = 12;
		((Control)lab_ali1).Text = "壹贰叁肆1234ABCD";
		lab_ali1.TextAlign = (ContentAlignment)32;
		((Control)lab_oppo1).BackColor = Color.White;
		((Control)lab_oppo1).Dock = (DockStyle)5;
		((Control)lab_oppo1).Location = new Point(293, 45);
		((Control)lab_oppo1).Margin = new Padding(5);
		((Control)lab_oppo1).Name = "lab_oppo1";
		((Control)lab_oppo1).Size = new Size(278, 228);
		((Control)lab_oppo1).TabIndex = 13;
		((Control)lab_oppo1).Text = "壹贰叁肆1234ABCD";
		lab_oppo1.TextAlign = (ContentAlignment)32;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(96f, 96f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)2;
		((Control)this).BackColor = SystemColors.ControlDark;
		((Form)this).ClientSize = new Size(866, 542);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Controls.Add((Control)(object)menuStrip1);
		((Form)this).MainMenuStrip = menuStrip1;
		((Control)this).Name = "MyDebugFrm";
		((Control)this).Text = "MyDebugFrm";
		((Form)this).Load += MyDebugFrm_Load;
		((Control)menuStrip1).ResumeLayout(false);
		((Control)menuStrip1).PerformLayout();
		((Control)tableLayoutPanel1).ResumeLayout(false);
		((Control)tableLayoutPanel1).PerformLayout();
		((Control)panel1).ResumeLayout(false);
		((Control)panel1).PerformLayout();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
