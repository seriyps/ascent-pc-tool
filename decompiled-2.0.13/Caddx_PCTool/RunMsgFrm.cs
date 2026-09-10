using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class RunMsgFrm : Form
{
	private IContainer components = null;

	public RichTextBox richTextBox1;

	public RunMsgFrm()
	{
		InitializeComponent();
	}

	private void RunMsgFrm_Load(object sender, EventArgs e)
	{
		ReloadFont();
	}

	private void RunMsgFrm_FormClosing(object sender, FormClosingEventArgs e)
	{
		((Control)this).Hide();
		((CancelEventArgs)(object)e).Cancel = true;
	}

	private void richTextBox1_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		((TextBoxBase)richTextBox1).Clear();
	}

	public void ReloadFont()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 12f);
		((Control)richTextBox1).Font = font;
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(RunMsgFrm));
		richTextBox1 = new RichTextBox();
		((Control)this).SuspendLayout();
		((Control)richTextBox1).BackColor = Color.Gainsboro;
		((TextBoxBase)richTextBox1).BorderStyle = (BorderStyle)0;
		((Control)richTextBox1).Dock = (DockStyle)5;
		((Control)richTextBox1).ForeColor = Color.FromArgb(51, 51, 51);
		((Control)richTextBox1).Location = new Point(0, 0);
		((Control)richTextBox1).Margin = new Padding(0);
		((Control)richTextBox1).Name = "richTextBox1";
		((TextBoxBase)richTextBox1).ReadOnly = true;
		((Control)richTextBox1).Size = new Size(213, 510);
		((Control)richTextBox1).TabIndex = 0;
		((Control)richTextBox1).TabStop = false;
		((Control)richTextBox1).Text = "";
		((Control)richTextBox1).MouseDoubleClick += new MouseEventHandler(richTextBox1_MouseDoubleClick);
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.White;
		((Form)this).ClientSize = new Size(213, 510);
		((Control)this).Controls.Add((Control)(object)richTextBox1);
		((Form)this).Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
		((Control)this).Name = "RunMsgFrm";
		((Control)this).Text = "运行信息";
		((Form)this).FormClosing += new FormClosingEventHandler(RunMsgFrm_FormClosing);
		((Form)this).Load += RunMsgFrm_Load;
		((Control)this).ResumeLayout(false);
	}
}
