using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;
using Newtonsoft.Json;

namespace Caddx_PCTool;

public class CamHubCtrl : UserControl
{
	private readonly string HUB_JSON_PATH = "pdf/mipi_hub.json";

	private List<string> _jsonSrting = new List<string>();

	private CamHubData _hubData;

	private string paramPanel_cn = "配置面板";

	private string createJson_cn = "生成文件";

	private string loadJson_cn = "读取文件";

	private string paramPanel_en = "Param panel";

	private string createJson_en = "Generate files";

	private string loadJson_en = "Read file";

	private IContainer components = null;

	private GridPanel grpan_main;

	private GridPanel gridPanel1;

	private PageHeader pageHeader1;

	private PageHeader pageHeader2;

	private Button btn_CreateJson;

	private Label label5;

	private Label label4;

	private Label label3;

	private Label label2;

	private Panel panel1;

	private Label label10;

	private Label label9;

	private Label label8;

	private Label label7;

	private Label label6;

	private PictureBox pictureBox1;

	private Button btn_LoadJson;

	private Button button3;

	private GridPanel gridPanel4;

	private Label lab_title;

	private Label label14;

	private Label label13;

	private Label label16;

	private Label label15;

	private Label label17;

	private Select sel_vtx;

	private Select sel_cam2;

	private Select sel_cam1;

	private Select sel_hdmi;

	private Select sel_cam3;

	private Label label1;

	private MutiRadioBtn mutiRadioBtn_HighPrior;

	private MutiRadioBtn mutiRadioBtn_Selected;

	public CamHubCtrl()
	{
		InitializeComponent();
	}

	public CamHubCtrl(CamHubData data)
	{
		InitializeComponent();
		_hubData = data;
	}

	private void CamHubCtrl_Load(object sender, EventArgs e)
	{
		InitData();
		RefreshCtrl();
		ReloadLang();
	}

	private void InitData()
	{
		if (File.Exists(HUB_JSON_PATH))
		{
			string text = File.ReadAllText(HUB_JSON_PATH);
			_hubData = JsonConvert.DeserializeObject<CamHubData>(text);
		}
		else
		{
			WriteLog.WriteLogFileToUI("hub.json is null", Color.Red);
		}
	}

	private void RefreshCtrl()
	{
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		sel_vtx.SelectedIndex = 0;
		if (_hubData != null)
		{
			sel_cam1.SelectedValue = _hubData.node0.dev_name;
			sel_cam2.SelectedValue = _hubData.node1.dev_name;
			sel_cam3.SelectedValue = _hubData.node2.dev_name;
			if (_hubData.node3.res_info == "none")
			{
				sel_hdmi.SelectedIndex = 0;
			}
			else
			{
				sel_hdmi.SelectedValue = _hubData.node3.res_info;
			}
		}
		else
		{
			_hubData = new CamHubData();
		}
		mutiRadioBtn_HighPrior.SetAlltxt("High Priority", "CAM1", "CAM2", "CAM3", "HDMI");
		mutiRadioBtn_HighPrior.SetFont();
		mutiRadioBtn_HighPrior.SetRaBtnCheck(_hubData.NodeList, "High Priority");
		mutiRadioBtn_HighPrior.OnRadioBtnClick += MutiRadioBtn_HighPrior_OnRadioBtnClick;
		mutiRadioBtn_Selected.SetAlltxt("Selected", "CAM1", "CAM2", "CAM3", "HDMI");
		mutiRadioBtn_Selected.SetFont();
		mutiRadioBtn_Selected.SetRaBtnCheck(_hubData.NodeList, "Selected");
		mutiRadioBtn_Selected.OnRadioBtnClick += MutiRadioBtn_Selected_OnRadioBtnClick;
		sel_cam1.SelectedValueChanged += new ObjectNEventHandler(sel_cam1_SelectedValueChanged);
		sel_cam2.SelectedValueChanged += new ObjectNEventHandler(sel_cam2_SelectedValueChanged);
		sel_cam3.SelectedValueChanged += new ObjectNEventHandler(sel_cam3_SelectedValueChanged);
		sel_hdmi.SelectedValueChanged += new ObjectNEventHandler(sel_hdmi_SelectedValueChanged);
	}

	private void MutiRadioBtn_Selected_OnRadioBtnClick(object sender, MouseEventArgs e)
	{
		try
		{
			for (int i = 0; i < _hubData.NodeList.Count; i++)
			{
				_hubData.NodeList[i].selected = 0;
			}
			int selected = 1;
			Button val = (Button)((sender is Button) ? sender : null);
			if (((Control)val).Name == "btn1")
			{
				_hubData.node0.selected = selected;
			}
			else if (((Control)val).Name == "btn2")
			{
				_hubData.node1.selected = selected;
			}
			else if (((Control)val).Name == "btn3")
			{
				_hubData.node2.selected = selected;
			}
			else if (((Control)val).Name == "btn4")
			{
				_hubData.node3.selected = selected;
			}
		}
		catch (Exception)
		{
		}
	}

	private void MutiRadioBtn_HighPrior_OnRadioBtnClick(object sender, MouseEventArgs e)
	{
		try
		{
			for (int i = 0; i < _hubData.NodeList.Count; i++)
			{
				_hubData.NodeList[i].high_priority = 0;
			}
			int high_priority = 1;
			Button val = (Button)((sender is Button) ? sender : null);
			if (((Control)val).Name == "btn1")
			{
				_hubData.node0.high_priority = high_priority;
			}
			else if (((Control)val).Name == "btn2")
			{
				_hubData.node1.high_priority = high_priority;
			}
			else if (((Control)val).Name == "btn3")
			{
				_hubData.node2.high_priority = high_priority;
			}
			else if (((Control)val).Name == "btn4")
			{
				_hubData.node3.high_priority = high_priority;
			}
		}
		catch (Exception)
		{
		}
	}

	private void btn_CreateJson_Click(object sender, EventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			JsonSerializerSettings val = new JsonSerializerSettings
			{
				Formatting = (Formatting)1,
				NullValueHandling = (NullValueHandling)1
			};
			string contents = JsonConvert.SerializeObject((object)_hubData, val);
			File.WriteAllText(HUB_JSON_PATH, contents);
			Thread.Sleep(200);
			string title = ((GD.Inst.CurrLang == 1) ? "提示" : "Prompt");
			string desc = ((GD.Inst.CurrLang == 1) ? "Hub文件保存完成" : "Hub file save complete");
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
			((Form)commModalFrm).ShowDialog();
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("生成hub json失败,desc=" + ex.Message, Color.Red);
		}
	}

	private void btn_LoadJson_Click(object sender, EventArgs e)
	{
		if (File.Exists(HUB_JSON_PATH))
		{
			string text = File.ReadAllText(HUB_JSON_PATH);
			_hubData = JsonConvert.DeserializeObject<CamHubData>(text);
			RefreshCtrl();
		}
	}

	private void select1_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		Select val = (Select)((sender is Select) ? sender : null);
		int selectedIndex = val.SelectedIndex;
		string text = val.SelectedValue.ToString();
	}

	private void 生成jsonToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void sel_cam1_SelectedValueChanged(object sender, ObjectNEventArgs e)
	{
		_hubData.node0.dev_name = ((VEventArgs<object>)(object)e).Value.ToString();
		if (((VEventArgs<object>)(object)e).Value.ToString() == "none")
		{
			_hubData.node0.res_info = "none";
		}
		else
		{
			_hubData.node0.res_info = "1920x1080P60";
		}
	}

	private void sel_cam2_SelectedValueChanged(object sender, ObjectNEventArgs e)
	{
		_hubData.node1.dev_name = ((VEventArgs<object>)(object)e).Value.ToString();
		if (((VEventArgs<object>)(object)e).Value.ToString() == "none")
		{
			_hubData.node1.res_info = "none";
		}
		else
		{
			_hubData.node1.res_info = "1920x1080P60";
		}
	}

	private void sel_cam3_SelectedValueChanged(object sender, ObjectNEventArgs e)
	{
		_hubData.node2.dev_name = ((VEventArgs<object>)(object)e).Value.ToString();
		if (((VEventArgs<object>)(object)e).Value.ToString() == "none")
		{
			_hubData.node2.res_info = "none";
		}
		else
		{
			_hubData.node2.res_info = "2000x1500P50";
		}
	}

	private void sel_hdmi_SelectedValueChanged(object sender, ObjectNEventArgs e)
	{
		if (((VEventArgs<object>)(object)e).Value.ToString() == "none")
		{
			_hubData.node3.dev_name = "none";
		}
		else
		{
			_hubData.node3.dev_name = "HDMI";
		}
		_hubData.node3.res_info = ((VEventArgs<object>)(object)e).Value.ToString();
	}

	private void high_rdBtn_CheckedChanged(object sender, BoolEventArgs e)
	{
	}

	private void select_rdBtn_CheckedChanged(object sender, BoolEventArgs e)
	{
		int selected = (((VEventArgs<bool>)(object)e).Value ? 1 : 0);
		Radio val = (Radio)((sender is Radio) ? sender : null);
		if (((Control)val).Name == "select_rdBtnCam1")
		{
			_hubData.node0.selected = selected;
		}
		else if (((Control)val).Name == "select_rdBtnCam2")
		{
			_hubData.node1.selected = selected;
		}
		else if (((Control)val).Name == "select_rdBtnCam3")
		{
			_hubData.node2.selected = selected;
		}
		else if (((Control)val).Name == "select_rdBtnHDMI")
		{
			_hubData.node3.selected = selected;
		}
	}

	public void ReloadFont()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 12f);
		Font val2 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 10f);
		Label obj = label1;
		Label obj2 = label2;
		Label obj3 = label3;
		Label obj4 = label4;
		Label obj5 = label5;
		Button obj6 = btn_CreateJson;
		Font val3 = (((Control)btn_LoadJson).Font = val2);
		Font val5 = (((Control)obj6).Font = val3);
		Font val7 = (((Control)obj5).Font = val5);
		Font val9 = (((Control)obj4).Font = val7);
		Font val11 = (((Control)obj3).Font = val9);
		Font font = (((Control)obj2).Font = val11);
		((Control)obj).Font = font;
		Font font2 = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], 13f);
		((Control)lab_title).Font = font2;
	}

	public void ReloadLang()
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			switch ((LangType)GD.Inst.CurrLang)
			{
			case LangType.zh_CN:
				((Control)lab_title).Text = paramPanel_cn;
				((Control)btn_CreateJson).Text = createJson_cn;
				((Control)btn_LoadJson).Text = loadJson_cn;
				break;
			case LangType.en_US:
				((Control)lab_title).Text = paramPanel_en;
				((Control)btn_CreateJson).Text = createJson_en;
				((Control)btn_LoadJson).Text = loadJson_en;
				break;
			}
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
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2c: Expected O, but got Unknown
		//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
		//IL_15aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1667: Unknown result type (might be due to invalid IL or missing references)
		//IL_1728: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_199d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3a: Expected O, but got Unknown
		//IL_1a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b49: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd6: Expected O, but got Unknown
		//IL_1c11: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e82: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_207f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2089: Expected O, but got Unknown
		//IL_20a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_214c: Unknown result type (might be due to invalid IL or missing references)
		grpan_main = new GridPanel();
		gridPanel4 = new GridPanel();
		mutiRadioBtn_Selected = new MutiRadioBtn();
		mutiRadioBtn_HighPrior = new MutiRadioBtn();
		panel1 = new Panel();
		label10 = new Label();
		label9 = new Label();
		label8 = new Label();
		label7 = new Label();
		label6 = new Label();
		pictureBox1 = new PictureBox();
		gridPanel1 = new GridPanel();
		label1 = new Label();
		sel_hdmi = new Select();
		sel_cam3 = new Select();
		sel_cam2 = new Select();
		sel_cam1 = new Select();
		sel_vtx = new Select();
		label17 = new Label();
		label16 = new Label();
		label15 = new Label();
		label14 = new Label();
		label13 = new Label();
		pageHeader2 = new PageHeader();
		btn_LoadJson = new Button();
		button3 = new Button();
		btn_CreateJson = new Button();
		label5 = new Label();
		label4 = new Label();
		label3 = new Label();
		label2 = new Label();
		pageHeader1 = new PageHeader();
		lab_title = new Label();
		((Control)grpan_main).SuspendLayout();
		((Control)gridPanel4).SuspendLayout();
		((Control)panel1).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)gridPanel1).SuspendLayout();
		((Control)pageHeader2).SuspendLayout();
		((Control)pageHeader1).SuspendLayout();
		((Control)this).SuspendLayout();
		((ContainerPanel)grpan_main).Back = Color.FromArgb(38, 41, 43);
		((Control)grpan_main).BackColor = Color.FromArgb(38, 41, 43);
		((ContainerPanel)grpan_main).BorderColor = Color.Black;
		((Control)grpan_main).Controls.Add((Control)(object)gridPanel4);
		((Control)grpan_main).Controls.Add((Control)(object)panel1);
		((Control)grpan_main).Controls.Add((Control)(object)gridPanel1);
		((Control)grpan_main).Dock = (DockStyle)5;
		((Control)grpan_main).Location = new Point(0, 0);
		((Control)grpan_main).Margin = new Padding(0);
		((Control)grpan_main).Name = "grpan_main";
		((Control)grpan_main).Padding = new Padding(5);
		((Control)grpan_main).Size = new Size(840, 648);
		grpan_main.Span = "50% 50%;100%;100%;-60% 17% 20%";
		((Control)grpan_main).TabIndex = 0;
		((Control)grpan_main).Text = "gridPanel1";
		((ContainerPanel)gridPanel4).Back = Color.Transparent;
		((Control)gridPanel4).BackColor = Color.Transparent;
		((Control)gridPanel4).Controls.Add((Control)(object)mutiRadioBtn_Selected);
		((Control)gridPanel4).Controls.Add((Control)(object)mutiRadioBtn_HighPrior);
		((Control)gridPanel4).Location = new Point(8, 391);
		((Control)gridPanel4).Name = "gridPanel4";
		((Control)gridPanel4).Size = new Size(824, 102);
		gridPanel4.Span = "50% 50%;";
		((Control)gridPanel4).TabIndex = 12;
		((Control)gridPanel4).Text = "gridPanel4";
		((Control)mutiRadioBtn_Selected).BackColor = Color.FromArgb(46, 49, 51);
		((Control)mutiRadioBtn_Selected).Location = new Point(417, 5);
		((Control)mutiRadioBtn_Selected).Margin = new Padding(5);
		((Control)mutiRadioBtn_Selected).Name = "mutiRadioBtn_Selected";
		((Control)mutiRadioBtn_Selected).Size = new Size(402, 92);
		((Control)mutiRadioBtn_Selected).TabIndex = 1;
		((Control)mutiRadioBtn_HighPrior).BackColor = Color.FromArgb(46, 49, 51);
		((Control)mutiRadioBtn_HighPrior).Location = new Point(5, 5);
		((Control)mutiRadioBtn_HighPrior).Margin = new Padding(5);
		((Control)mutiRadioBtn_HighPrior).Name = "mutiRadioBtn_HighPrior";
		((Control)mutiRadioBtn_HighPrior).Size = new Size(402, 92);
		((Control)mutiRadioBtn_HighPrior).TabIndex = 0;
		panel1.Back = Color.FromArgb(46, 49, 51);
		((Control)panel1).BackColor = Color.Transparent;
		panel1.BorderColor = Color.FromArgb(66, 68, 70);
		panel1.BorderWidth = 2f;
		((Control)panel1).Controls.Add((Control)(object)label10);
		((Control)panel1).Controls.Add((Control)(object)label9);
		((Control)panel1).Controls.Add((Control)(object)label8);
		((Control)panel1).Controls.Add((Control)(object)label7);
		((Control)panel1).Controls.Add((Control)(object)label6);
		((Control)panel1).Controls.Add((Control)(object)pictureBox1);
		((Control)panel1).Dock = (DockStyle)5;
		grpan_main.SetIndex((Control)(object)panel1, 1);
		((Control)panel1).Location = new Point(10, 10);
		((Control)panel1).Margin = new Padding(5);
		((Control)panel1).Name = "panel1";
		((Control)panel1).Padding = new Padding(5);
		((Control)panel1).Size = new Size(405, 373);
		((Control)panel1).TabIndex = 11;
		((Control)panel1).Text = "panel1";
		((Control)label10).BackColor = SystemColors.ControlLight;
		((Control)label10).Location = new Point(112, 3);
		((Control)label10).Name = "label10";
		((Control)label10).Size = new Size(62, 34);
		((Control)label10).TabIndex = 14;
		((Control)label10).Text = "2";
		label10.TextAlign = (ContentAlignment)32;
		((Control)label10).Visible = false;
		((Control)label9).BackColor = SystemColors.ControlLight;
		((Control)label9).Location = new Point(23, 3);
		((Control)label9).Name = "label9";
		((Control)label9).Size = new Size(62, 34);
		((Control)label9).TabIndex = 13;
		((Control)label9).Text = "1";
		label9.TextAlign = (ContentAlignment)32;
		((Control)label9).Visible = false;
		((Control)label8).BackColor = SystemColors.ControlLight;
		((Control)label8).Location = new Point(394, 3);
		((Control)label8).Name = "label8";
		((Control)label8).Size = new Size(62, 34);
		((Control)label8).TabIndex = 12;
		((Control)label8).Text = "5";
		label8.TextAlign = (ContentAlignment)32;
		((Control)label8).Visible = false;
		((Control)label7).BackColor = SystemColors.ControlLight;
		((Control)label7).Location = new Point(301, 3);
		((Control)label7).Name = "label7";
		((Control)label7).Size = new Size(62, 34);
		((Control)label7).TabIndex = 11;
		((Control)label7).Text = "4";
		label7.TextAlign = (ContentAlignment)32;
		((Control)label7).Visible = false;
		((Control)label6).BackColor = SystemColors.ControlLight;
		((Control)label6).Location = new Point(201, 3);
		((Control)label6).Name = "label6";
		((Control)label6).Size = new Size(62, 34);
		((Control)label6).TabIndex = 10;
		((Control)label6).Text = "3";
		label6.TextAlign = (ContentAlignment)32;
		((Control)label6).Visible = false;
		((Control)pictureBox1).BackColor = Color.FromArgb(46, 49, 51);
		((Control)pictureBox1).Dock = (DockStyle)5;
		pictureBox1.Image = (Image)(object)Resources.hub示意图;
		((Control)pictureBox1).Location = new Point(7, 7);
		((Control)pictureBox1).Margin = new Padding(0);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(391, 359);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 1;
		pictureBox1.TabStop = false;
		((ContainerPanel)gridPanel1).Back = Color.FromArgb(46, 49, 51);
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(66, 68, 70);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)label1);
		((Control)gridPanel1).Controls.Add((Control)(object)sel_hdmi);
		((Control)gridPanel1).Controls.Add((Control)(object)sel_cam3);
		((Control)gridPanel1).Controls.Add((Control)(object)sel_cam2);
		((Control)gridPanel1).Controls.Add((Control)(object)sel_cam1);
		((Control)gridPanel1).Controls.Add((Control)(object)sel_vtx);
		((Control)gridPanel1).Controls.Add((Control)(object)label17);
		((Control)gridPanel1).Controls.Add((Control)(object)label16);
		((Control)gridPanel1).Controls.Add((Control)(object)label15);
		((Control)gridPanel1).Controls.Add((Control)(object)label14);
		((Control)gridPanel1).Controls.Add((Control)(object)label13);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader2);
		((Control)gridPanel1).Controls.Add((Control)(object)label5);
		((Control)gridPanel1).Controls.Add((Control)(object)label4);
		((Control)gridPanel1).Controls.Add((Control)(object)label3);
		((Control)gridPanel1).Controls.Add((Control)(object)label2);
		((Control)gridPanel1).Controls.Add((Control)(object)pageHeader1);
		((Control)gridPanel1).Dock = (DockStyle)5;
		grpan_main.SetIndex((Control)(object)gridPanel1, 2);
		((Control)gridPanel1).Location = new Point(425, 10);
		((Control)gridPanel1).Margin = new Padding(5);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(405, 373);
		gridPanel1.Span = "100%;30% 50% 20%;30% 50% 20%;30% 50% 20%;\r\n30% 50% 20%;30% 50% 20%;100%;100%;";
		((Control)gridPanel1).TabIndex = 1;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)label1).BackColor = Color.Transparent;
		((Control)label1).Dock = (DockStyle)5;
		((Control)label1).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label1.ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label1, 2);
		((Control)label1).Location = new Point(5, 52);
		((Control)label1).Margin = new Padding(5);
		((Control)label1).Name = "label1";
		label1.Prefix = "* ";
		label1.PrefixColor = Color.FromArgb(255, 63, 63);
		((Control)label1).Size = new Size(112, 37);
		((Control)label1).TabIndex = 23;
		((Control)label1).Text = "VTX ";
		label1.TextAlign = (ContentAlignment)64;
		((Input)sel_hdmi).BackColor = Color.FromArgb(33, 36, 39);
		((Input)sel_hdmi).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_hdmi).BorderColor = Color.FromArgb(56, 59, 61);
		((Input)sel_hdmi).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_hdmi).BorderWidth = 2f;
		((IControl)sel_hdmi).ColorScheme = (TAMode)2;
		sel_hdmi.EnterDropDown = false;
		((Input)sel_hdmi).ForeColor = Color.White;
		((IControl)sel_hdmi).HandDragFolder = false;
		((Input)sel_hdmi).HandShortcutKeys = false;
		gridPanel1.SetIndex((Control)(object)sel_hdmi, 15);
		sel_hdmi.Items.AddRange(new object[5] { "none", "1920x1080P60", "1920x1080P50", "1280x720P60", "1280x720P50" });
		sel_hdmi.List = true;
		((Control)sel_hdmi).Location = new Point(125, 236);
		sel_hdmi.MaxCount = 5;
		((Control)sel_hdmi).Name = "sel_hdmi";
		((Input)sel_hdmi).SelectionColor = Color.Empty;
		((Control)sel_hdmi).Size = new Size(196, 41);
		((Control)sel_hdmi).TabIndex = 22;
		((Control)sel_hdmi).TabStop = false;
		((Input)sel_hdmi).WaveSize = 0;
		((Input)sel_cam3).BackColor = Color.FromArgb(33, 36, 39);
		((Input)sel_cam3).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_cam3).BorderColor = Color.FromArgb(56, 59, 61);
		((Input)sel_cam3).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_cam3).BorderWidth = 2f;
		((IControl)sel_cam3).ColorScheme = (TAMode)2;
		sel_cam3.EnterDropDown = false;
		((Input)sel_cam3).ForeColor = Color.White;
		((IControl)sel_cam3).HandDragFolder = false;
		((Input)sel_cam3).HandShortcutKeys = false;
		gridPanel1.SetIndex((Control)(object)sel_cam3, 12);
		sel_cam3.Items.AddRange(new object[2] { "none", "Z40" });
		sel_cam3.List = true;
		((Control)sel_cam3).Location = new Point(125, 189);
		((Control)sel_cam3).Name = "sel_cam3";
		((Input)sel_cam3).SelectionColor = Color.Empty;
		((Control)sel_cam3).Size = new Size(196, 41);
		((Control)sel_cam3).TabIndex = 21;
		((Control)sel_cam3).TabStop = false;
		((Input)sel_cam3).WaveSize = 0;
		((Input)sel_cam2).BackColor = Color.FromArgb(33, 36, 39);
		((Input)sel_cam2).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_cam2).BorderColor = Color.FromArgb(56, 59, 61);
		((Input)sel_cam2).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_cam2).BorderWidth = 2f;
		((IControl)sel_cam2).ColorScheme = (TAMode)2;
		sel_cam2.EnterDropDown = false;
		((Input)sel_cam2).ForeColor = Color.White;
		((IControl)sel_cam2).HandDragFolder = false;
		((Input)sel_cam2).HandShortcutKeys = false;
		gridPanel1.SetIndex((Control)(object)sel_cam2, 9);
		sel_cam2.Items.AddRange(new object[4] { "none", "GT_PRO", "IR", "Z8" });
		sel_cam2.List = true;
		((Control)sel_cam2).Location = new Point(125, 143);
		((Control)sel_cam2).Name = "sel_cam2";
		((Input)sel_cam2).SelectionColor = Color.Empty;
		((Control)sel_cam2).Size = new Size(196, 41);
		((Control)sel_cam2).TabIndex = 20;
		((Control)sel_cam2).TabStop = false;
		((Input)sel_cam2).WaveSize = 0;
		((Input)sel_cam1).BackColor = Color.FromArgb(33, 36, 39);
		((Input)sel_cam1).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_cam1).BorderColor = Color.FromArgb(56, 59, 61);
		((Input)sel_cam1).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_cam1).BorderWidth = 2f;
		((IControl)sel_cam1).ColorScheme = (TAMode)2;
		sel_cam1.EnterDropDown = false;
		((Input)sel_cam1).ForeColor = Color.White;
		((IControl)sel_cam1).HandDragFolder = false;
		((Input)sel_cam1).HandShortcutKeys = false;
		gridPanel1.SetIndex((Control)(object)sel_cam1, 6);
		sel_cam1.Items.AddRange(new object[4] { "none", "GT_PRO", "IR", "Z8" });
		sel_cam1.List = true;
		((Control)sel_cam1).Location = new Point(125, 96);
		((Control)sel_cam1).Name = "sel_cam1";
		((Input)sel_cam1).SelectionColor = Color.Empty;
		((Control)sel_cam1).Size = new Size(196, 41);
		((Control)sel_cam1).TabIndex = 19;
		((Control)sel_cam1).TabStop = false;
		((Input)sel_cam1).WaveSize = 0;
		((Input)sel_vtx).BackColor = Color.FromArgb(33, 36, 39);
		((Input)sel_vtx).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_vtx).BorderColor = Color.FromArgb(56, 59, 61);
		((Input)sel_vtx).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_vtx).BorderWidth = 2f;
		((IControl)sel_vtx).ColorScheme = (TAMode)2;
		sel_vtx.EnterDropDown = false;
		((Input)sel_vtx).ForeColor = Color.White;
		((IControl)sel_vtx).HandDragFolder = false;
		((Input)sel_vtx).HandShortcutKeys = false;
		gridPanel1.SetIndex((Control)(object)sel_vtx, 3);
		sel_vtx.Items.AddRange(new object[1] { "GT_PRO" });
		sel_vtx.List = true;
		((Control)sel_vtx).Location = new Point(125, 50);
		((Control)sel_vtx).Name = "sel_vtx";
		((Input)sel_vtx).SelectionColor = Color.Empty;
		((Control)sel_vtx).Size = new Size(196, 41);
		((Control)sel_vtx).TabIndex = 18;
		((Control)sel_vtx).TabStop = false;
		((Input)sel_vtx).WaveSize = 0;
		((Control)label17).AutoSize = true;
		((Control)label17).BackColor = Color.Transparent;
		((Control)label17).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label17, 16);
		((Control)label17).Location = new Point(329, 238);
		((Control)label17).Margin = new Padding(5);
		((Control)label17).Name = "label17";
		((Control)label17).Size = new Size(71, 37);
		((Control)label17).TabIndex = 17;
		label17.TextAlign = (ContentAlignment)64;
		((Control)label16).AutoSize = true;
		((Control)label16).BackColor = Color.Transparent;
		((Control)label16).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label16, 7);
		((Control)label16).Location = new Point(329, 98);
		((Control)label16).Margin = new Padding(5);
		((Control)label16).Name = "label16";
		((Control)label16).Size = new Size(71, 37);
		((Control)label16).TabIndex = 16;
		label16.TextAlign = (ContentAlignment)64;
		((Control)label15).AutoSize = true;
		((Control)label15).BackColor = Color.Transparent;
		((Control)label15).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label15, 10);
		((Control)label15).Location = new Point(329, 145);
		((Control)label15).Margin = new Padding(5);
		((Control)label15).Name = "label15";
		((Control)label15).Size = new Size(71, 37);
		((Control)label15).TabIndex = 15;
		label15.TextAlign = (ContentAlignment)64;
		((Control)label14).AutoSize = true;
		((Control)label14).BackColor = Color.Transparent;
		((Control)label14).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label14, 13);
		((Control)label14).Location = new Point(329, 191);
		((Control)label14).Margin = new Padding(5);
		((Control)label14).Name = "label14";
		((Control)label14).Size = new Size(71, 37);
		((Control)label14).TabIndex = 14;
		label14.TextAlign = (ContentAlignment)64;
		((Control)label13).AutoSize = true;
		((Control)label13).BackColor = Color.Transparent;
		((Control)label13).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label13, 4);
		((Control)label13).Location = new Point(329, 52);
		((Control)label13).Margin = new Padding(5);
		((Control)label13).Name = "label13";
		((Control)label13).Size = new Size(71, 37);
		((Control)label13).TabIndex = 13;
		label13.TextAlign = (ContentAlignment)64;
		((Control)pageHeader2).BackColor = Color.Transparent;
		((Control)pageHeader2).Controls.Add((Control)(object)btn_LoadJson);
		((Control)pageHeader2).Controls.Add((Control)(object)button3);
		((Control)pageHeader2).Controls.Add((Control)(object)btn_CreateJson);
		((Control)pageHeader2).Dock = (DockStyle)5;
		pageHeader2.Gap = 0;
		gridPanel1.SetIndex((Control)(object)pageHeader2, 17);
		((Control)pageHeader2).Location = new Point(5, 280);
		((Control)pageHeader2).Margin = new Padding(5, 0, 5, 0);
		((Control)pageHeader2).Name = "pageHeader2";
		((Control)pageHeader2).Size = new Size(395, 47);
		((Control)pageHeader2).TabIndex = 11;
		((Control)pageHeader2).Text = "";
		btn_LoadJson.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_LoadJson).Dock = (DockStyle)4;
		((Control)btn_LoadJson).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_LoadJson.ForeColor = Color.FromArgb(255, 255, 255);
		((Control)btn_LoadJson).Location = new Point(185, 0);
		((Control)btn_LoadJson).Margin = new Padding(0);
		((Control)btn_LoadJson).Name = "btn_LoadJson";
		((Control)btn_LoadJson).Size = new Size(100, 47);
		((Control)btn_LoadJson).TabIndex = 3;
		((Control)btn_LoadJson).Text = "读取文件";
		((Control)btn_LoadJson).Click += btn_LoadJson_Click;
		button3.DefaultBack = Color.Black;
		((Control)button3).Dock = (DockStyle)4;
		button3.ForeColor = Color.White;
		button3.Ghost = true;
		((Control)button3).Location = new Point(285, 0);
		((Control)button3).Margin = new Padding(0);
		((Control)button3).Name = "button3";
		((Control)button3).Size = new Size(10, 47);
		((Control)button3).TabIndex = 2;
		btn_CreateJson.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_CreateJson).Dock = (DockStyle)4;
		((Control)btn_CreateJson).Font = new Font("阿里巴巴普惠体", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		btn_CreateJson.ForeColor = Color.FromArgb(35, 35, 35);
		((Control)btn_CreateJson).Location = new Point(295, 0);
		((Control)btn_CreateJson).Margin = new Padding(0);
		((Control)btn_CreateJson).Name = "btn_CreateJson";
		((Control)btn_CreateJson).Size = new Size(100, 47);
		((Control)btn_CreateJson).TabIndex = 0;
		((Control)btn_CreateJson).Text = "生成文件";
		((Control)btn_CreateJson).Click += btn_CreateJson_Click;
		((Control)label5).AutoSize = true;
		((Control)label5).BackColor = Color.Transparent;
		((Control)label5).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label5, 14);
		((Control)label5).Location = new Point(5, 238);
		((Control)label5).Margin = new Padding(5);
		((Control)label5).Name = "label5";
		((Control)label5).Size = new Size(112, 37);
		((Control)label5).TabIndex = 9;
		((Control)label5).Text = "HDMI";
		label5.TextAlign = (ContentAlignment)64;
		((Control)label4).AutoSize = true;
		((Control)label4).BackColor = Color.Transparent;
		((Control)label4).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label4, 11);
		((Control)label4).Location = new Point(5, 191);
		((Control)label4).Margin = new Padding(5);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(112, 37);
		((Control)label4).TabIndex = 7;
		((Control)label4).Text = "CAM3";
		label4.TextAlign = (ContentAlignment)64;
		((Control)label3).AutoSize = true;
		((Control)label3).BackColor = Color.Transparent;
		((Control)label3).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label3, 8);
		((Control)label3).Location = new Point(5, 145);
		((Control)label3).Margin = new Padding(5);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(112, 37);
		((Control)label3).TabIndex = 5;
		((Control)label3).Text = "CAM2";
		label3.TextAlign = (ContentAlignment)64;
		((Control)label2).AutoSize = true;
		((Control)label2).BackColor = Color.Transparent;
		((Control)label2).ForeColor = Color.FromArgb(164, 164, 165);
		gridPanel1.SetIndex((Control)(object)label2, 5);
		((Control)label2).Location = new Point(5, 98);
		((Control)label2).Margin = new Padding(5);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(112, 37);
		((Control)label2).TabIndex = 3;
		((Control)label2).Text = "CAM1";
		label2.TextAlign = (ContentAlignment)64;
		((Control)pageHeader1).BackColor = Color.Transparent;
		((Control)pageHeader1).Controls.Add((Control)(object)lab_title);
		((Control)pageHeader1).ForeColor = Color.White;
		gridPanel1.SetIndex((Control)(object)pageHeader1, 1);
		((Control)pageHeader1).Location = new Point(3, 3);
		((Control)pageHeader1).Name = "pageHeader1";
		((Control)pageHeader1).Size = new Size(399, 41);
		((Control)pageHeader1).TabIndex = 0;
		((Control)pageHeader1).TabStop = false;
		((Control)pageHeader1).Text = "";
		((Control)lab_title).Dock = (DockStyle)5;
		((Control)lab_title).Font = new Font("阿里巴巴普惠体 Medium", 12.75f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_title).Location = new Point(0, 0);
		((Control)lab_title).Margin = new Padding(5, 5, 5, 0);
		((Control)lab_title).Name = "lab_title";
		((Control)lab_title).Size = new Size(399, 41);
		((Control)lab_title).TabIndex = 11;
		((Control)lab_title).Text = "Selected";
		lab_title.TextAlign = (ContentAlignment)16;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(32, 33, 37);
		((Control)this).Controls.Add((Control)(object)grpan_main);
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "CamHubCtrl";
		((Control)this).Size = new Size(840, 648);
		((UserControl)this).Load += CamHubCtrl_Load;
		((Control)grpan_main).ResumeLayout(false);
		((Control)gridPanel4).ResumeLayout(false);
		((Control)panel1).ResumeLayout(false);
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel1).PerformLayout();
		((Control)pageHeader2).ResumeLayout(false);
		((Control)pageHeader1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
