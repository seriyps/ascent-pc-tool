using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AntdUI_Ex;

namespace Caddx_PCTool;

public class MutiRadioBtn : UserControl
{
	private List<Button> btnList;

	private IContainer components = null;

	private GridPanel gridPanel1;

	private Label lab_title;

	private Button btn1;

	private Label lab_noues;

	private Button btn2;

	private Button btn4;

	private Button btn3;

	public event EventHandler<MouseEventArgs> OnRadioBtnClick;

	public MutiRadioBtn()
	{
		InitializeComponent();
	}

	private void MutiRadioBtn_Load(object sender, EventArgs e)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		btnList = new List<Button> { btn1, btn2, btn3, btn4 };
		((Control)btn1).MouseClick += new MouseEventHandler(btn_MouseClick);
		((Control)btn2).MouseClick += new MouseEventHandler(btn_MouseClick);
		((Control)btn3).MouseClick += new MouseEventHandler(btn_MouseClick);
		((Control)btn4).MouseClick += new MouseEventHandler(btn_MouseClick);
	}

	public void SetTitletxt(string txt)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)lab_title).Text = txt;
		});
	}

	public void SetBtntxt(string btn1 = null, string btn2 = null, string btn3 = null, string btn4 = null)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			if (btn1 != null)
			{
				((Control)this.btn1).Text = btn1;
				((IControl)this.btn1).Visible = true;
			}
			else
			{
				((IControl)this.btn1).Visible = false;
			}
			if (btn2 != null)
			{
				((Control)this.btn2).Text = btn2;
				((IControl)this.btn2).Visible = true;
			}
			else
			{
				((IControl)this.btn2).Visible = false;
			}
			if (btn3 != null)
			{
				((Control)this.btn3).Text = btn3;
				((IControl)this.btn3).Visible = true;
			}
			else
			{
				((IControl)this.btn3).Visible = false;
			}
			if (btn4 != null)
			{
				((Control)this.btn4).Text = btn4;
				((IControl)this.btn4).Visible = true;
			}
			else
			{
				((IControl)this.btn4).Visible = false;
			}
		});
	}

	public void SetAlltxt(string title, string btn1 = null, string btn2 = null, string btn3 = null, string btn4 = null)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)lab_title).Text = title;
			if (btn1 != null)
			{
				((Control)this.btn1).Text = btn1;
				((IControl)this.btn1).Visible = true;
			}
			else
			{
				((IControl)this.btn1).Visible = false;
			}
			if (btn2 != null)
			{
				((Control)this.btn2).Text = btn2;
				((IControl)this.btn2).Visible = true;
			}
			else
			{
				((IControl)this.btn2).Visible = false;
			}
			if (btn3 != null)
			{
				((Control)this.btn3).Text = btn3;
				((IControl)this.btn3).Visible = true;
			}
			else
			{
				((IControl)this.btn3).Visible = false;
			}
			if (btn4 != null)
			{
				((Control)this.btn4).Text = btn4;
				((IControl)this.btn4).Visible = true;
			}
			else
			{
				((IControl)this.btn4).Visible = false;
			}
		});
	}

	public void SetRaBtnCheck(List<NodeInfo> nodeInfos, string param)
	{
		int num = -1;
		for (int i = 0; i < nodeInfos.Count; i++)
		{
			if (param == "High Priority")
			{
				if (nodeInfos[i].high_priority == 1)
				{
					num = i;
					break;
				}
			}
			else if (param == "Selected" && nodeInfos[i].selected == 1)
			{
				num = i;
				break;
			}
		}
		string btnname = "btn" + (num + 1);
		RefreshBtnColor(btnname);
	}

	private void btn_MouseClick(object sen, MouseEventArgs me)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		try
		{
			Button val = (Button)sen;
			RefreshBtnColor(((Control)val).Name);
			OnRadioBtnClick?.Invoke(sen, me);
		}
		catch (Exception ex)
		{
			MessageBox.Show("MutiRadioBtn error,desc=" + ex.Message);
		}
	}

	private void RefreshBtnColor(string btnname)
	{
		for (int i = 0; i < btnList.Count; i++)
		{
			btnList[i].DefaultBack = Color.FromArgb(95, 95, 96);
			btnList[i].ForeColor = Color.FromArgb(255, 255, 255);
		}
		switch (btnname)
		{
		case "btn1":
			btn1.DefaultBack = Color.FromArgb(255, 233, 0);
			btn1.ForeColor = Color.FromArgb(35, 35, 35);
			break;
		case "btn2":
			btn2.DefaultBack = Color.FromArgb(255, 233, 0);
			btn2.ForeColor = Color.FromArgb(35, 35, 35);
			break;
		case "btn3":
			btn3.DefaultBack = Color.FromArgb(255, 233, 0);
			btn3.ForeColor = Color.FromArgb(35, 35, 35);
			break;
		case "btn4":
			btn4.DefaultBack = Color.FromArgb(255, 233, 0);
			btn4.ForeColor = Color.FromArgb(35, 35, 35);
			break;
		}
	}

	public void SetFont()
	{
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
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Expected O, but got Unknown
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Expected O, but got Unknown
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Expected O, but got Unknown
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Expected O, but got Unknown
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Expected O, but got Unknown
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		gridPanel1 = new GridPanel();
		btn4 = new Button();
		btn3 = new Button();
		btn2 = new Button();
		btn1 = new Button();
		lab_noues = new Label();
		lab_title = new Label();
		((Control)gridPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((ContainerPanel)gridPanel1).Back = Color.Transparent;
		((Control)gridPanel1).BackColor = Color.Transparent;
		((Control)gridPanel1).Controls.Add((Control)(object)btn4);
		((Control)gridPanel1).Controls.Add((Control)(object)btn3);
		((Control)gridPanel1).Controls.Add((Control)(object)btn2);
		((Control)gridPanel1).Controls.Add((Control)(object)btn1);
		((Control)gridPanel1).Controls.Add((Control)(object)lab_noues);
		((Control)gridPanel1).Controls.Add((Control)(object)lab_title);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(400, 110);
		gridPanel1.Span = "50% 50%;25% 25% 25% 25%;\r\n-40% 60%";
		((Control)gridPanel1).TabIndex = 0;
		((Control)gridPanel1).Text = "gridPanel1";
		btn4.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn4).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn4.ForeColor = Color.FromArgb(255, 255, 255);
		gridPanel1.SetIndex((Control)(object)btn4, 6);
		((Control)btn4).Location = new Point(305, 49);
		((Control)btn4).Margin = new Padding(5);
		((Control)btn4).Name = "btn4";
		((Control)btn4).Size = new Size(90, 56);
		((Control)btn4).TabIndex = 7;
		((Control)btn4).Text = "HDMI";
		btn4.WaveSize = 0;
		btn3.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn3).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn3.ForeColor = Color.FromArgb(255, 255, 255);
		gridPanel1.SetIndex((Control)(object)btn3, 5);
		((Control)btn3).Location = new Point(205, 49);
		((Control)btn3).Margin = new Padding(5);
		((Control)btn3).Name = "btn3";
		((Control)btn3).Size = new Size(90, 56);
		((Control)btn3).TabIndex = 6;
		((Control)btn3).Text = "CAM3";
		btn3.WaveSize = 0;
		btn2.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn2).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn2.ForeColor = Color.FromArgb(255, 255, 255);
		gridPanel1.SetIndex((Control)(object)btn2, 4);
		((Control)btn2).Location = new Point(105, 49);
		((Control)btn2).Margin = new Padding(5);
		((Control)btn2).Name = "btn2";
		((Control)btn2).Size = new Size(90, 56);
		((Control)btn2).TabIndex = 5;
		((Control)btn2).Text = "CAM2";
		btn2.WaveSize = 0;
		btn1.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn1).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn1.ForeColor = Color.FromArgb(35, 35, 35);
		gridPanel1.SetIndex((Control)(object)btn1, 3);
		((Control)btn1).Location = new Point(5, 49);
		((Control)btn1).Margin = new Padding(5);
		((Control)btn1).Name = "btn1";
		((Control)btn1).Size = new Size(90, 56);
		((Control)btn1).TabIndex = 4;
		((Control)btn1).Text = "CAM1";
		btn1.WaveSize = 0;
		((Control)lab_noues).AutoSize = true;
		((Control)lab_noues).BackColor = Color.Transparent;
		((Control)lab_noues).Font = new Font("宋体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_noues).ForeColor = Color.White;
		gridPanel1.SetIndex((Control)(object)lab_noues, 2);
		((Control)lab_noues).Location = new Point(210, 3);
		((Control)lab_noues).Margin = new Padding(10, 3, 3, 3);
		((Control)lab_noues).Name = "lab_noues";
		((Control)lab_noues).Size = new Size(187, 38);
		((Control)lab_noues).TabIndex = 1;
		lab_noues.TextAlign = (ContentAlignment)16;
		((Control)lab_title).AutoSize = true;
		((Control)lab_title).BackColor = Color.Transparent;
		((Control)lab_title).Font = new Font("宋体", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_title).ForeColor = Color.White;
		gridPanel1.SetIndex((Control)(object)lab_title, 1);
		((Control)lab_title).Location = new Point(10, 3);
		((Control)lab_title).Margin = new Padding(10, 3, 3, 3);
		((Control)lab_title).Name = "lab_title";
		((Control)lab_title).Size = new Size(187, 38);
		((Control)lab_title).TabIndex = 0;
		((Control)lab_title).Text = "High Priority";
		lab_title.TextAlign = (ContentAlignment)16;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(46, 49, 51);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "MutiRadioBtn";
		((Control)this).Size = new Size(400, 110);
		((UserControl)this).Load += MutiRadioBtn_Load;
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel1).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
