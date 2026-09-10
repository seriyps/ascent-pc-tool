using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AntdUI_Ex;

namespace Caddx_PCTool;

public class DisplayCtrl : UserControl
{
	public Action<string, string> DispCtrlDoubleClick;

	private IContainer components = null;

	private TableLayoutPanel tableLayoutPanel1;

	private Label label1;

	private PictureBox pictureBox1;

	public DisplayCtrl()
	{
		InitializeComponent();
	}

	public void SetTxt(string txt)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)label1).Text = txt;
		});
	}

	public void SetImgAndTxt(Image img, string txt)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)label1).Text = txt;
			pictureBox1.Image = img;
		});
	}

	private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		DispCtrlDoubleClick?.Invoke(((Control)this).Name, ((Control)label1).Text);
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
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected O, but got Unknown
		tableLayoutPanel1 = new TableLayoutPanel();
		label1 = new Label();
		pictureBox1 = new PictureBox();
		((Control)tableLayoutPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		tableLayoutPanel1.ColumnCount = 1;
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 100f));
		tableLayoutPanel1.Controls.Add((Control)(object)label1, 0, 1);
		tableLayoutPanel1.Controls.Add((Control)(object)pictureBox1, 0, 0);
		((Control)tableLayoutPanel1).Dock = (DockStyle)5;
		((Control)tableLayoutPanel1).Location = new Point(0, 0);
		((Control)tableLayoutPanel1).Margin = new Padding(0);
		((Control)tableLayoutPanel1).Name = "tableLayoutPanel1";
		tableLayoutPanel1.RowCount = 2;
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 80f));
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 20f));
		((Control)tableLayoutPanel1).Size = new Size(277, 226);
		((Control)tableLayoutPanel1).TabIndex = 0;
		((Control)label1).Anchor = (AnchorStyles)1;
		label1.AutoSizeMode = (TAutoSize)1;
		((Control)label1).Font = new Font("宋体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label1).Location = new Point(122, 183);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(32, 19);
		((Control)label1).TabIndex = 0;
		((Control)label1).Text = "描述";
		label1.TextAlign = (ContentAlignment)32;
		((Control)pictureBox1).Dock = (DockStyle)5;
		((Control)pictureBox1).Location = new Point(0, 0);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(277, 180);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 1;
		pictureBox1.TabStop = false;
		((Control)pictureBox1).MouseDoubleClick += new MouseEventHandler(pictureBox1_MouseDoubleClick);
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((UserControl)this).BorderStyle = (BorderStyle)1;
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "DisplayCtrl";
		((Control)this).Size = new Size(277, 226);
		((Control)tableLayoutPanel1).ResumeLayout(false);
		((Control)tableLayoutPanel1).PerformLayout();
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
