using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class ProcessAndDesc : UserControl
{
	private IContainer components = null;

	private TableLayoutPanel tableLayoutPanel1;

	private PictureBox pictureBox1;

	private TableLayoutPanel tableLayoutPanel2;

	private Progress progress1;

	private Label label1;

	[Category("Behavior")]
	public float ProcessRatio
	{
		get
		{
			return progress1.ValueRatio;
		}
		set
		{
			progress1.ValueRatio = value;
			((Control)this).Invalidate();
		}
	}

	public ProcessAndDesc()
	{
		InitializeComponent();
	}

	private void ProcessAndDesc_Load(object sender, EventArgs e)
	{
		progress1.State = (TType)0;
	}

	public void SetProcValAndText(float procVal, string text)
	{
		try
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				if ((double)procVal > 0.95)
				{
					progress1.State = (TType)1;
					pictureBox1.Image = (Image)(object)Resources.升级成功;
					progress1.Fill = Color.FromArgb(43, 164, 113);
				}
				else if ((double)procVal < 0.1)
				{
					progress1.State = (TType)0;
					pictureBox1.Image = (Image)(object)Resources.升级中;
					progress1.Fill = Color.FromArgb(255, 233, 0);
				}
				progress1.Value = procVal;
				((Control)label1).Text = text;
			});
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("ProcessAndDesc SetProcValAndText error,desc=" + ex.Message, Color.Red);
		}
	}

	public void HideInterCtrl()
	{
		TableLayoutPanel obj = tableLayoutPanel2;
		bool visible = (((Control)pictureBox1).Visible = false);
		((Control)obj).Visible = visible;
	}

	public void ShowInterCtrl()
	{
		TableLayoutPanel obj = tableLayoutPanel2;
		bool visible = (((Control)pictureBox1).Visible = true);
		((Control)obj).Visible = visible;
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
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected O, but got Unknown
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Expected O, but got Unknown
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Expected O, but got Unknown
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Expected O, but got Unknown
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		tableLayoutPanel1 = new TableLayoutPanel();
		pictureBox1 = new PictureBox();
		tableLayoutPanel2 = new TableLayoutPanel();
		progress1 = new Progress();
		label1 = new Label();
		((Control)tableLayoutPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)tableLayoutPanel1).BackColor = Color.Transparent;
		tableLayoutPanel1.ColumnCount = 2;
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 20f));
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle((SizeType)2, 80f));
		tableLayoutPanel1.Controls.Add((Control)(object)pictureBox1, 0, 0);
		tableLayoutPanel1.Controls.Add((Control)(object)tableLayoutPanel2, 1, 0);
		((Control)tableLayoutPanel1).Dock = (DockStyle)5;
		((Control)tableLayoutPanel1).Location = new Point(0, 0);
		((Control)tableLayoutPanel1).Margin = new Padding(0);
		((Control)tableLayoutPanel1).Name = "tableLayoutPanel1";
		tableLayoutPanel1.RowCount = 1;
		tableLayoutPanel1.RowStyles.Add(new RowStyle((SizeType)2, 100f));
		((Control)tableLayoutPanel1).Size = new Size(338, 100);
		((Control)tableLayoutPanel1).TabIndex = 0;
		((Control)pictureBox1).BackColor = Color.Transparent;
		((Control)pictureBox1).Dock = (DockStyle)5;
		pictureBox1.Image = (Image)(object)Resources.升级中;
		((Control)pictureBox1).Location = new Point(0, 0);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(67, 100);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 0;
		pictureBox1.TabStop = false;
		((Control)tableLayoutPanel2).BackColor = Color.Transparent;
		tableLayoutPanel2.ColumnCount = 1;
		tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle((SizeType)2, 100f));
		tableLayoutPanel2.Controls.Add((Control)(object)progress1, 0, 1);
		tableLayoutPanel2.Controls.Add((Control)(object)label1, 0, 0);
		((Control)tableLayoutPanel2).Dock = (DockStyle)5;
		((Control)tableLayoutPanel2).Location = new Point(67, 0);
		((Control)tableLayoutPanel2).Margin = new Padding(0);
		((Control)tableLayoutPanel2).Name = "tableLayoutPanel2";
		tableLayoutPanel2.RowCount = 2;
		tableLayoutPanel2.RowStyles.Add(new RowStyle((SizeType)2, 40f));
		tableLayoutPanel2.RowStyles.Add(new RowStyle((SizeType)2, 60f));
		((Control)tableLayoutPanel2).Size = new Size(271, 100);
		((Control)tableLayoutPanel2).TabIndex = 1;
		progress1.Back = Color.FromArgb(99, 101, 103);
		((Control)progress1).BackColor = Color.FromArgb(33, 36, 39);
		((Control)progress1).Dock = (DockStyle)5;
		progress1.Fill = Color.FromArgb(255, 233, 0);
		progress1.ForeColor = Color.White;
		((Control)progress1).Location = new Point(0, 40);
		((Control)progress1).Margin = new Padding(0);
		((Control)progress1).Name = "progress1";
		((Control)progress1).Size = new Size(271, 60);
		((Control)progress1).TabIndex = 0;
		((Control)progress1).Text = "progress1";
		progress1.Value = 0.5f;
		progress1.ValueRatio = 0.7f;
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Dock = (DockStyle)5;
		((Control)label1).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label1).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)label1).Location = new Point(0, 0);
		((Control)label1).Margin = new Padding(0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(271, 40);
		((Control)label1).TabIndex = 1;
		((Control)label1).Text = "升级进行中ABCDEFABCDEFAB";
		label1.TextAlign = (ContentAlignment)256;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "ProcessAndDesc";
		((Control)this).Size = new Size(338, 100);
		((UserControl)this).Load += ProcessAndDesc_Load;
		((Control)tableLayoutPanel1).ResumeLayout(false);
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)tableLayoutPanel2).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
