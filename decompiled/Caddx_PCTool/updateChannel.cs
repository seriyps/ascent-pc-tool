using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Caddx_PCTool;

public class updateChannel : UserControl, IDisposable
{
	private const byte SINGLEMAXCOUNT = 36;

	private const byte TOTALMAXCOUNT = 120;

	private readonly string BBFreqTemplatePath = "resource\\param\\fpv_bb_freq.bin";

	private ResAscentInfo _currAsceDevInfo;

	private FpvSkyRfBoardName _currRFName = FpvSkyRfBoardName.FPV_SKY_RF_486;

	private List<UsbDevInfo> _usbDevInfo;

	private string _currPortname;

	private string _saveBBFreqPath;

	private string _currReadBBFreqPath;

	private string _currBandName = "bandA";

	private Dictionary<string, List<int>> _bandDict = new Dictionary<string, List<int>>();

	private ushort _clickPlusing = 0;

	private AntList<FreqItem> _dgvDataList;

	private int is_default = 0;

	private byte _fileRFName = 0;

	private IContainer components = null;

	private GridPanel gridPanel1;

	private GridPanel grpan_Btn;

	private Button btn_UploadBB;

	private Button btn_quit;

	private Select sel_portname;

	private Button btn_refresh;

	private Button btn_connect;

	private Button btn_SaveBB;

	private GridPanel gridPanel2;

	private Label label1;

	private Button btn_SaveMofify;

	private Select sel_band;

	private Button btn_AddListview;

	private Table dgv1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewRowToolStripMenuItem;

	private ToolStripMenuItem deleteSelectedRowToolStripMenuItem;

	private Panel panel2;

	private Panel panel1;

	private Label label3;

	private Label lab_FactoryReset;

	private Panel panel3;

	private Label label5;

	private Label label4;

	private Label label2;

	public event EventHandler<FrmEventArgs> OnChannFrmHappentEvnet;

	public updateChannel(List<UsbDevInfo> info)
	{
		InitializeComponent();
		_usbDevInfo = info;
	}

	private void updateChannel_Load(object sender, EventArgs e)
	{
		((Control)lab_FactoryReset).Text = "";
		InitDGV();
		_dgvDataList = new AntList<FreqItem>();
		dgv1.Binding<FreqItem>(_dgvDataList);
		Button obj = btn_SaveMofify;
		Select obj2 = sel_band;
		bool flag = (((IControl)dgv1).Visible = false);
		bool visible = (((IControl)obj2).Visible = flag);
		((IControl)obj).Visible = visible;
		ManualSearchDevices(out _usbDevInfo);
		sel_portname.Items.Clear();
		if (_usbDevInfo.Count == 0)
		{
			sel_portname.Items.Add((object)Lang.T("updateChannel.no_com"));
			sel_portname.SelectedIndex = 0;
		}
		else
		{
			for (int i = 0; i < _usbDevInfo.Count; i++)
			{
				sel_portname.Items.Add((object)_usbDevInfo[i].PortName);
			}
			string currPortname = (((Control)sel_portname).Text = _usbDevInfo[0].PortName);
			_currPortname = currPortname;
			sel_portname.SelectedIndex = 0;
		}
		EnableCtrl(isenable: false);
	}

	private void InitDGV()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0078: Expected O, but got Unknown
		dgv1.Columns.Clear();
		Table obj = dgv1;
		ColumnCollection val = new ColumnCollection();
		val.Add(new Column("Channel_Index", "Channel index", (ColumnAlign)2)
		{
			Width = "30%",
			Editable = false
		});
		val.Add(new Column("Value_KHz", "Value(KHz)", (ColumnAlign)2)
		{
			Width = "70%",
			Editable = true
		});
		obj.Columns = val;
	}

	private void FillRowHeight()
	{
	}

	private void FillCellVal()
	{
	}

	private void grpan_Btn_Click(object sender, EventArgs e)
	{
	}

	private void btn_quit_MouseClick(object sender, MouseEventArgs e)
	{
	}

	private void btn_refresh_Click(object sender, EventArgs e)
	{
		if (!ManualSearchDevices(out _usbDevInfo) || _usbDevInfo.Count <= 0)
		{
			sel_portname.Items.Clear();
			((Control)sel_portname).Text = Lang.T("updateChannel.no_com");
			sel_portname.Items.Add((object)Lang.T("updateChannel.no_com"));
			sel_portname.SelectedIndex = 0;
			return;
		}
		sel_portname.Items.Clear();
		((Control)sel_portname).Text = "";
		for (int i = 0; i < _usbDevInfo.Count; i++)
		{
			sel_portname.Items.Add((object)_usbDevInfo[i].PortName);
		}
		string currPortname = (((Control)sel_portname).Text = _usbDevInfo[0].PortName);
		_currPortname = currPortname;
		sel_portname.SelectedIndex = 0;
	}

	private bool ManualSearchDevices(out List<UsbDevInfo> usbinfo)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		usbinfo = new List<UsbDevInfo>();
		try
		{
			string[] portNames = SerialPort.GetPortNames();
			if (portNames.Length < 1)
			{
				return false;
			}
			HashSet<string> hashSet = new HashSet<string>(portNames.Select((string port) => port.TrimStart(new char[1] { '\\' })));
			foreach (string item in hashSet)
			{
				string text = "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%" + item + "%'";
				ManagementObjectSearcher val = new ManagementObjectSearcher(text);
				try
				{
					ManagementObjectCollection val2 = val.Get();
					ManagementObjectEnumerator enumerator2 = val2.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ManagementObject device = (ManagementObject)enumerator2.Current;
							UsbDevInfo usbDevInfo = ParseDeviceInformation((ManagementBaseObject)(object)device);
							if (usbDevInfo != null && Enumerable.Contains(portNames, usbDevInfo.PortName))
							{
								usbinfo.Add(usbDevInfo);
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			return usbinfo.Count > 0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private UsbDevInfo ParseDeviceInformation(ManagementBaseObject device)
	{
		try
		{
			string text = device["DeviceID"]?.ToString() ?? string.Empty;
			string text2 = device["Name"]?.ToString() ?? string.Empty;
			string text3 = device["Description"]?.ToString() ?? string.Empty;
			string input = device["PNPDeviceID"]?.ToString() ?? string.Empty;
			Match match = Regex.Match(text2, "\\(COM\\d+\\)");
			if (!match.Success)
			{
				return null;
			}
			string portName = match.Value.Trim(new char[2] { '(', ')' });
			Match match2 = Regex.Match(input, "VID_([0-9A-Fa-f]{4})&PID_([0-9A-Fa-f]{4})", RegexOptions.IgnoreCase);
			if (!match2.Success)
			{
				return null;
			}
			string value = match2.Groups[1].Value;
			string value2 = match2.Groups[2].Value;
			return new UsbDevInfo
			{
				PortName = portName,
				VID = value,
				PID = value2,
				DevName = text2,
				ConnectedTime = DateTime.Now
			};
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("解析设备信息时出错: " + ex.Message, Color.Red);
			return null;
		}
	}

	private void btn_connect_Click(object sender, EventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		Button val = (Button)sender;
		if (((Control)btn_connect).Text == Lang.T("common.btn_connect"))
		{
			FrmEventArgs e2 = new FrmEventArgs
			{
				Desc = "UpdataChannConnect",
				InfoType = InfoType.ctrlSign
			};
			OnChannFrmHappentEvnet?.Invoke(null, e2);
			CleanupFsmResources();
			UsbDevInfo usbDevInfo = _usbDevInfo?.FirstOrDefault((UsbDevInfo x) => _currPortname == x.PortName);
			if (usbDevInfo == null)
			{
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.device_connection_failed"), Color.Red);
				((Form)commModalFrm).ShowDialog();
				return;
			}
			GD.Inst.UsbFSM = new UsbSerialportFSM(usbDevInfo);
			GD.Inst.UsbFSM.SerialConnectedStateChange += UsbFSM_SerialConneStateChange;
			if (!GD.Inst.UsbFSM.Open(_currPortname))
			{
				CleanupFsmResources();
				CommModalFrm commModalFrm2 = new CommModalFrm();
				commModalFrm2.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.device_connection_failed"), Color.Red);
				((Form)commModalFrm2).ShowDialog();
				return;
			}
			val.DefaultBack = Color.FromArgb(95, 95, 96);
			val.ForeColor = Color.FromArgb(255, 255, 255);
			EnableCtrl(isenable: true);
			GD.Inst.UpgFSM = new UpgradeProcessFSM(GD.Inst.UsbFSM);
			GD.Inst.UpgFSM.OnUpgProcHappenEvent += UpgFSM_OnUpgProcHappenEvent;
			UpgradeProcessFSM upgFSM = GD.Inst.UpgFSM;
			upgFSM.OnAsceDevRecInfo = (Action<uint, ResAscentInfo, object>)Delegate.Combine(upgFSM.OnAsceDevRecInfo, new Action<uint, ResAscentInfo, object>(OnAsceDevRecInfo));
			((Control)val).Text = Lang.T("updateChannel.btn_disconnect");
			GD.Inst.UpgFSM.Send_FindDevice();
		}
		else
		{
			CleanupFsmResources();
			EnableCtrl(isenable: false);
			val.DefaultBack = Color.FromArgb(255, 233, 0);
			val.ForeColor = Color.FromArgb(35, 35, 35);
			((Control)val).Text = Lang.T("common.btn_connect");
		}
	}

	private void OnAsceDevRecInfo(uint arg1, ResAscentInfo arg2, object arg3)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			_currAsceDevInfo = arg2;
			Match match = Regex.Match(arg2.HWVers, "V(\\d+\\.\\d+)-(\\d+\\.\\d+)$");
			if (!match.Success)
			{
				commModalFrm.SetAllTxt(Lang.T("common.title_error"), string.Format(Lang.T("updateChannel.extract_rf_info_failed"), arg2.HWVers), Color.Red);
				((Form)commModalFrm).ShowDialog();
				return;
			}
			string text = match.Value.Substring(1);
			string[] array = text.Split(new char[1] { '-' });
			string text2 = array[0];
			string text3 = array[1];
			string value = match.Groups[2].Value;
			string s = text3.Split(new char[1] { '.' })[0];
			if (!int.TryParse(s, out var result))
			{
				WriteLog.WriteLogFileToUI("解析设备版本失败: " + text3, Color.Red);
				return;
			}
			byte b = (byte)(result * 16);
			if (Enum.IsDefined(typeof(FpvSkyRfBoardName), b))
			{
				_currRFName = (FpvSkyRfBoardName)b;
				WriteLog.WriteLogFileToUI($"_currRFName= {_currRFName}", Color.Black);
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("updateChannel.OnAsceDevRecInfo: " + ex.Message, Color.Red);
		}
	}

	private void UpgFSM_OnUpgProcHappenEvent(object sender, UpgProcHappenEventArgs e)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				switch (e.infoType)
				{
				case InfoType.info:
					EnableCtrl(isenable: true);
					break;
				case InfoType.ctrlSign:
				{
					string desc = e.Desc;
					File.WriteAllText(_saveBBFreqPath, e.Desc);
					CommModalFrm commModalFrm2 = new CommModalFrm();
					commModalFrm2.SetAllTxt(Lang.T("common.title_success"), Lang.T("updateChannel.bbfreq_saved_successfully"), Color.Green);
					((Form)commModalFrm2).ShowDialog();
					LoadDictDataFromJson(e.Desc);
					LoadDataToGrid(_saveBBFreqPath, ((Control)sel_band).Text);
					Button obj = btn_SaveMofify;
					Select obj2 = sel_band;
					bool flag = (((IControl)dgv1).Visible = true);
					bool visible = (((IControl)obj2).Visible = flag);
					((IControl)obj).Visible = visible;
					((IControl)btn_SaveBB).Enabled = true;
					break;
				}
				}
			});
		}
		catch (Exception ex)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(Lang.T("common.title_error"), "updateChannel.OnUpgProcHappenEvent error,desc=" + ex.Message, Color.Red);
			((Form)commModalFrm).ShowDialog();
		}
	}

	private void UsbFSM_SerialConneStateChange(bool b, string arg1, int arg2)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!b)
			{
				GD.Inst.UsbFSM?.Close();
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(Lang.T("common.title_error_warning"), Lang.T("updateChannel.device_disconnected_check_usb"), Color.Red);
				((Form)commModalFrm).ShowDialog();
			}
		}
		catch (Exception)
		{
		}
	}

	private void btn_SaveBB_Click(object sender, EventArgs e)
	{
		_saveBBFreqPath = BBFreqTemplatePath;
		GD.Inst.UpgFSM.Send_GetBBFreq(BBFreqTemplatePath);
	}

	private void btn_UploadBB_Click(object sender, EventArgs e)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		CommModalFrm commModalFrm = new CommModalFrm();
		if (GD.Inst.UpgFSM == null)
		{
			commModalFrm.SetAllTxt(Lang.T("common.title_error"), Lang.T("updateChannel.device_not_connected"), Color.Red);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		if ((uint)_fileRFName != (uint)_currRFName)
		{
			commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.rf_type_mismatch"), Color.Red);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		if (!ValidateAllValueKHz(_bandDict, out var errMsg))
		{
			commModalFrm.SetAllTxt(Lang.T("common.title_warning"), errMsg, Color.Red);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		RefreshDictVal(_currBandName);
		if (ReasonableJudgment(_bandDict))
		{
			Button obj = btn_UploadBB;
			bool enabled = (((IControl)btn_SaveBB).Enabled = false);
			((IControl)obj).Enabled = enabled;
			if (!SaveJsonAsPath(BBFreqTemplatePath))
			{
				commModalFrm.SetAllTxt(Lang.T("common.title_error"), Lang.T("updateChannel.file_save_failed"), Color.Red);
				((Form)commModalFrm).ShowDialog();
			}
			else
			{
				GD.Inst.UpgFSM.SetFilePath_BBFreq(BBFreqTemplatePath);
				GD.Inst.UpgFSM.Send_FileStart_BBFreq(BBFreqTemplatePath, "/factory/fpv_bb_freq.json");
			}
		}
	}

	private void btn_quit_Click(object sender, EventArgs e)
	{
		CleanupFsmResources();
		FrmEventArgs e2 = new FrmEventArgs
		{
			Desc = "ManualCloseDevice",
			InfoType = InfoType.ctrlSign
		};
		OnChannFrmHappentEvnet?.Invoke(null, e2);
		((Control)this).Hide();
	}

	private void CleanupFsmResources()
	{
		if (GD.Inst.UpgFSM != null)
		{
			GD.Inst.UpgFSM.OnUpgProcHappenEvent -= UpgFSM_OnUpgProcHappenEvent;
			UpgradeProcessFSM upgFSM = GD.Inst.UpgFSM;
			upgFSM.OnAsceDevRecInfo = (Action<uint, ResAscentInfo, object>)Delegate.Remove(upgFSM.OnAsceDevRecInfo, new Action<uint, ResAscentInfo, object>(OnAsceDevRecInfo));
			GD.Inst.UpgFSM.Dispose();
			GD.Inst.UpgFSM = null;
		}
		if (GD.Inst.UsbFSM != null)
		{
			GD.Inst.UsbFSM.SerialConnectedStateChange -= UsbFSM_SerialConneStateChange;
			GD.Inst.UsbFSM.Close();
			GD.Inst.UsbFSM.Dispose();
			GD.Inst.UsbFSM = null;
		}
	}

	private void sel_portname_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		Select val = (Select)sender;
		_currPortname = ((Control)val).Text;
	}

	private void sel_band_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		RefreshDictVal(_currBandName);
		Select val = (Select)sender;
		_currBandName = ((Control)val).Text;
		LoadDataToGrid(_currReadBBFreqPath, ((Control)val).Text);
	}

	private void btn_SaveMofify_Click(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Invalid comparison between Unknown and I4
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		CommModalFrm commModalFrm = new CommModalFrm();
		SaveFileDialog val = new SaveFileDialog();
		((FileDialog)val).Title = Lang.T("updateChannel.select_bbfreq_file");
		((FileDialog)val).InitialDirectory = "C:";
		((FileDialog)val).Filter = "Parameter File (*.bin)|*.bin|All (*.*)|*.*";
		((FileDialog)val).FilterIndex = 1;
		((FileDialog)val).RestoreDirectory = true;
		if ((int)((CommonDialog)val).ShowDialog() == 1 && ReasonableJudgment(_bandDict))
		{
			if (!SaveJsonAsPath(((FileDialog)val).FileName))
			{
				commModalFrm.SetAllTxt(Lang.T("common.title_error"), Lang.T("updateChannel.file_save_failed"), Color.Red);
				((Form)commModalFrm).ShowDialog();
			}
			else
			{
				commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(Lang.T("common.title_notice"), Lang.T("updateChannel.bbfreq_param_saved"), isshowBtnOK: true, isshowBtnCan: false);
				((Form)commModalFrm).ShowDialog();
			}
		}
	}

	private bool SaveJsonAsPath(string path)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		bool result = false;
		try
		{
			string text = JsonConvert.SerializeObject((object)_bandDict, (Formatting)1);
			JObject val = JObject.Parse(text);
			JObject val2 = new JObject
			{
				["RFBoardType"] = JToken.op_Implicit((byte)_currRFName),
				["is_defalut"] = JToken.op_Implicit(is_default)
			};
			foreach (JProperty item in val.Properties())
			{
				val2.Add(item.Name, item.Value);
			}
			string text2 = ((object)val2).ToString();
			File.WriteAllText(path, ((object)val2).ToString());
			return true;
		}
		catch (Exception)
		{
			return result;
		}
	}

	private bool ReasonableJudgment(Dictionary<string, List<int>> dict)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			if (!dict.TryGetValue(_currBandName, out var value) || value == null)
			{
				commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.band_data_invalid"), Color.Red);
				((Form)commModalFrm).ShowDialog();
				return false;
			}
			value.Clear();
			List<int> list = ((IEnumerable<FreqItem>)_dgvDataList).Select(delegate(FreqItem item)
			{
				PropertyInfo property = item.GetType().GetProperty("Value_KHz");
				return (int)property.GetValue(item);
			}).ToList();
			if (list == null || list.Count < 1)
			{
				commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.data_load_failed"), Color.Red);
				((Form)commModalFrm).ShowDialog();
				return false;
			}
			value.AddRange(list);
			int num = 0;
			for (int num2 = 0; num2 < dict.Count; num2++)
			{
				num += dict.ElementAt(num2).Value.Count;
			}
			if (num > 120)
			{
				commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.total_freq_exceed_limit"), Color.Red);
				((Form)commModalFrm).ShowDialog();
				return false;
			}
			List<string> list2 = (from kv in dict
				where kv.Value.Count != kv.Value.Distinct().Count()
				select kv.Key).ToList();
			if (list2.Count > 0)
			{
				string text = Lang.T("updateChannel.duplicate_values_in_band");
				for (int num3 = 0; num3 < list2.Count; num3++)
				{
					text = text + " " + list2[num3];
				}
				commModalFrm.SetAllTxt(Lang.T("common.title_warning"), text, Color.Red);
				((Form)commModalFrm).ShowDialog();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private List<int> ExtractModifiedFrequencies()
	{
		if (!(dgv1.DataSource is List<FreqItem> source))
		{
			return null;
		}
		return source.Select(delegate(FreqItem item)
		{
			PropertyInfo property = item.GetType().GetProperty("Value_KHz");
			return (int)property.GetValue(item);
		}).ToList();
	}

	private void btn_AddListview_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		OpenFileDialog val = new OpenFileDialog();
		((FileDialog)val).Title = Lang.T("updateChannel.select_bbfreq_file");
		((FileDialog)val).InitialDirectory = "C:\\";
		((FileDialog)val).Filter = "BB_Freq File (*.bin)|*.bin|All (*.*)|*.*";
		((FileDialog)val).FilterIndex = 1;
		((FileDialog)val).RestoreDirectory = true;
		val.Multiselect = false;
		if ((int)((CommonDialog)val).ShowDialog() == 1)
		{
			_currReadBBFreqPath = ((FileDialog)val).FileName;
			LoadDictDataFromFile(((FileDialog)val).FileName);
			LoadDataToGrid(((FileDialog)val).FileName, ((Control)sel_band).Text);
			Button obj = btn_SaveMofify;
			Select obj2 = sel_band;
			bool flag = (((IControl)dgv1).Visible = true);
			bool visible = (((IControl)obj2).Visible = flag);
			((IControl)obj).Visible = visible;
		}
	}

	private void RefreshDictVal(string keyname)
	{
		_bandDict[_currBandName].Clear();
		List<int> collection = ((IEnumerable<FreqItem>)_dgvDataList).Select(delegate(FreqItem item)
		{
			PropertyInfo property = item.GetType().GetProperty("Value_KHz");
			return (int)property.GetValue(item);
		}).ToList();
		_bandDict[_currBandName].AddRange(collection);
	}

	private void LoadDictDataFromFile(string path)
	{
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		CommModalFrm commModalFrm = new CommModalFrm();
		try
		{
			string text = File.ReadAllText(path);
			JObject val = JObject.Parse(text);
			if (val.ContainsKey("RFBoardType"))
			{
				JToken obj = val["RFBoardType"];
				_fileRFName = (byte)((obj != null) ? Extensions.Value<byte>((IEnumerable<JToken>)obj) : 0);
				val.Remove("RFBoardType");
			}
			if ((uint)_fileRFName != (uint)_currRFName)
			{
				commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.rf_type_mismatch"), Color.Red);
				((Form)commModalFrm).ShowDialog();
				return;
			}
			if (val.ContainsKey("is_defalut"))
			{
				JToken obj2 = val["is_defalut"];
				is_default = ((obj2 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj2) : 0);
				val.Remove("is_defalut");
			}
			JObject val2 = val;
			_bandDict = ((JToken)val2).ToObject<Dictionary<string, List<int>>>();
			if (_bandDict == null || _bandDict.Count == 0)
			{
				commModalFrm.SetAllTxt(Lang.T("common.title_error"), Lang.T("updateChannel.file_corrupted"), Color.Red);
				((Form)commModalFrm).ShowDialog();
				return;
			}
			sel_band.Items.Clear();
			for (int i = 0; i < _bandDict.Count; i++)
			{
				sel_band.Items.Add((object)_bandDict.ElementAt(i).Key);
			}
			if (sel_band.Items.Count != 0)
			{
				sel_band.SelectedIndex = 0;
				if (_bandDict.TryGetValue(((Control)sel_band).Text, out var value) && value == null)
				{
				}
			}
		}
		catch (Exception ex)
		{
			commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(Lang.T("common.title_error"), string.Format(Lang.T("updateChannel.load_file_failed"), ex.Message), Color.Red);
			((Form)commModalFrm).ShowDialog();
		}
	}

	private void LoadDictDataFromJson(string jsonSrc)
	{
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			JObject val = JObject.Parse(jsonSrc);
			if (val.ContainsKey("RFBoardType"))
			{
				JToken obj = val["RFBoardType"];
				_fileRFName = (byte)((obj != null) ? Extensions.Value<byte>((IEnumerable<JToken>)obj) : 16);
				val.Remove("RFBoardType");
			}
			if (val.ContainsKey("is_defalut"))
			{
				JToken obj2 = val["is_defalut"];
				is_default = ((obj2 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj2) : 0);
				val.Remove("is_defalut");
			}
			JObject val2 = val;
			_bandDict = ((JToken)val2).ToObject<Dictionary<string, List<int>>>();
			if (_bandDict == null || _bandDict.Count == 0)
			{
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(Lang.T("common.title_error"), Lang.T("updateChannel.file_corrupted"), Color.Red);
				((Form)commModalFrm).ShowDialog();
				return;
			}
			sel_band.Items.Clear();
			for (int i = 0; i < _bandDict.Count; i++)
			{
				sel_band.Items.Add((object)_bandDict.ElementAt(i).Key);
			}
			if (sel_band.Items.Count != 0)
			{
				sel_band.SelectedIndex = 0;
				if (_bandDict.TryGetValue(((Control)sel_band).Text, out var value) && value == null)
				{
				}
			}
		}
		catch (Exception ex)
		{
			CommModalFrm commModalFrm2 = new CommModalFrm();
			commModalFrm2.SetAllTxt(Lang.T("common.title_error"), string.Format(Lang.T("updateChannel.load_file_failed"), ex.Message), Color.Red);
			((Form)commModalFrm2).ShowDialog();
		}
	}

	private void LoadDataToGrid(string path, string keyname)
	{
		try
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				List<FreqItem> list = _bandDict[keyname].Select((int freq, int index) => new FreqItem
				{
					Channel_Index = (index + 1).ToString(),
					Value_KHz = freq
				}).ToList();
				_dgvDataList.Clear();
				_dgvDataList.AddRange((IList<FreqItem>)list);
				dgv1.Columns.Clear();
				InitDGV();
			});
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("加载数据到表格时出错: " + ex.Message, Color.Red);
		}
	}

	private void EnableCtrl(bool isenable)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			Button obj = btn_AddListview;
			Button obj2 = btn_SaveBB;
			bool flag = (((IControl)btn_UploadBB).Enabled = isenable);
			bool enabled = (((IControl)obj2).Enabled = flag);
			((IControl)obj).Enabled = enabled;
		});
	}

	private void dgv1_SelectIndexChanged(object sender, EventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		Table val = (Table)sender;
		WriteLog.WriteLogFileToUI($"当前选中索引={val.SelectedIndex}", Color.Black);
	}

	private bool ValidateAllValueKHz(List<int> values, out string errMsg)
	{
		errMsg = string.Empty;
		if (values == null || values.Count < 2)
		{
			errMsg = Lang.T("updateChannel.value_data_invalid");
			return false;
		}
		values.RemoveAt(values.Count - 1);
		int num = values[0];
		int num2 = values[values.Count - 1];
		if (num >= num2)
		{
			errMsg = Lang.T("updateChannel.first_value_less_than_max");
			return false;
		}
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < values.Count; i++)
		{
			int num3 = values[i];
			if (num3 % 1000 != 0)
			{
				errMsg = string.Format(Lang.T("updateChannel.value_must_be_multiple_of_1000"), i + 1);
				return false;
			}
			if (!hashSet.Add(num3))
			{
				errMsg = string.Format(Lang.T("updateChannel.value_duplicated"), num3);
				return false;
			}
			if (i != 0 && i != values.Count - 1 && (num3 <= num || num3 >= num2))
			{
				errMsg = string.Format(Lang.T("updateChannel.value_between_min_max"), i + 1, num, num2);
				return false;
			}
		}
		if (values.Min() != num)
		{
			errMsg = Lang.T("updateChannel.first_value_must_be_min");
			return false;
		}
		if (values.Max() != num2)
		{
			errMsg = Lang.T("updateChannel.last_value_must_be_max");
			return false;
		}
		return true;
	}

	private bool ValidateAllValueKHz(Dictionary<string, List<int>> dict, out string errMsg)
	{
		List<int> list = ((IEnumerable<FreqItem>)_dgvDataList).Select((FreqItem x) => x.Value_KHz).ToList();
		errMsg = string.Empty;
		if (list == null || list.Count < 2)
		{
			errMsg = Lang.T("updateChannel.value_data_invalid");
			return false;
		}
		list.RemoveAt(list.Count - 1);
		int num = list[0];
		int num2 = list[list.Count - 1];
		if (num >= num2)
		{
			errMsg = Lang.T("updateChannel.first_value_less_than_max");
			return false;
		}
		HashSet<int> hashSet = new HashSet<int>();
		for (int num3 = 0; num3 < list.Count; num3++)
		{
			int num4 = list[num3];
			if (num4 % 1000 != 0)
			{
				errMsg = string.Format(Lang.T("updateChannel.value_must_be_multiple_of_1000"), num3 + 1);
				return false;
			}
			if (!hashSet.Add(num4))
			{
				errMsg = string.Format(Lang.T("updateChannel.value_duplicated"), num4);
				return false;
			}
			if (num3 != 0 && num3 != list.Count - 1 && (num4 <= num || num4 >= num2))
			{
				errMsg = string.Format(Lang.T("updateChannel.value_between_min_max"), num3 + 1, num, num2);
				return false;
			}
		}
		if (list.Min() != num)
		{
			errMsg = Lang.T("updateChannel.first_value_must_be_min");
			return false;
		}
		if (list.Max() != num2)
		{
			errMsg = Lang.T("updateChannel.last_value_must_be_max");
			return false;
		}
		return true;
	}

	private bool dgv1_CellEndEdit(object sender, TableEndEditEventArgs e)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		if (((ITableEventArgs)e).ColumnIndex != 1)
		{
			return true;
		}
		if (_dgvDataList == null || _dgvDataList.Count == 0)
		{
			return false;
		}
		if (string.IsNullOrWhiteSpace(e.Value))
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.value_khz_empty"), Color.Red);
			((Form)commModalFrm).ShowDialog();
			return false;
		}
		if (!int.TryParse(e.Value, out var result))
		{
			CommModalFrm commModalFrm2 = new CommModalFrm();
			commModalFrm2.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.value_khz_must_be_integer"), Color.Red);
			((Form)commModalFrm2).ShowDialog();
			return false;
		}
		int num = ((ITableEventArgs)e).RowIndex - 1;
		if (num < 0 || num >= _dgvDataList.Count)
		{
			return false;
		}
		List<int> list = ((IEnumerable<FreqItem>)_dgvDataList).Select((FreqItem x) => x.Value_KHz).ToList();
		list[num] = result;
		if (!ValidateAllValueKHz(list, out var errMsg))
		{
			CommModalFrm commModalFrm3 = new CommModalFrm();
			commModalFrm3.SetAllTxt(Lang.T("common.title_warning"), errMsg, Color.Red);
			((Form)commModalFrm3).ShowDialog();
			return false;
		}
		return true;
	}

	private bool dgv1_CellBeginEdit(object sender, TableEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		Table val = (Table)sender;
		if (((ITableEventArgs)e).ColumnIndex != 1)
		{
			return false;
		}
		if (((ITableEventArgs)e).RowIndex == val.DisplayRowCount)
		{
			return false;
		}
		return true;
	}

	private void dgv1_CellBeginEditInputStyle(object sender, TableBeginEditInputStyleEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		Table val = (Table)sender;
	}

	private void label8_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		if ((int)e.Button != 2097152)
		{
			return;
		}
		_clickPlusing++;
		if (_clickPlusing > 5)
		{
			_clickPlusing = 0;
			EnableCtrl(isenable: false);
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.confirm_factory_reset"));
			((Form)commModalFrm).ShowDialog();
			if ((int)commModalFrm.FrmResult == 1)
			{
				GD.Inst.UpgFSM.Send_FactoryReset();
			}
		}
	}

	private void RefreshChannelIndex(int newRowIndex = -1)
	{
		for (int i = 0; i < _dgvDataList.Count; i++)
		{
			_dgvDataList[i].Channel_Index = ((i == newRowIndex) ? $"{i + 1}（new）" : (i + 1).ToString());
		}
	}

	private void addNewRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (_dgvDataList.Count >= 36)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.max_freq_count_36"), Color.Red);
			((Form)commModalFrm).ShowDialog();
			return;
		}
		List<int> values = ((IEnumerable<FreqItem>)_dgvDataList).Select((FreqItem x) => x.Value_KHz).ToList();
		if (!ValidateAllValueKHz(values, out var errMsg))
		{
			CommModalFrm commModalFrm2 = new CommModalFrm();
			commModalFrm2.SetAllTxt(Lang.T("common.title_warning"), errMsg, Color.Red);
			((Form)commModalFrm2).ShowDialog();
			return;
		}
		int count = _dgvDataList.Count;
		int num = dgv1.SelectedIndex;
		if (num <= 0)
		{
			num = 1;
		}
		if (num > count)
		{
			num = count;
		}
		int num2 = ((num == 1 || num == 2) ? 1 : ((num != count && num != count - 1) ? Math.Max(0, num - 1) : Math.Max(0, count - 2)));
		FreqItem freqItem = new FreqItem
		{
			Channel_Index = string.Empty,
			Value_KHz = 1234000
		};
		_dgvDataList.Insert(num2, freqItem);
		RefreshChannelIndex(num2);
		((Control)dgv1).Refresh();
		dgv1.SelectedIndex = num2 + 1;
	}

	private void deleteSelectedRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		int selectedIndex = dgv1.SelectedIndex;
		if (selectedIndex == 1 || selectedIndex == dgv1.DisplayRowCount - 1 || selectedIndex == dgv1.DisplayRowCount)
		{
			CommModalFrm commModalFrm = new CommModalFrm();
			commModalFrm.SetAllTxt(Lang.T("common.title_warning"), Lang.T("updateChannel.disable_delete_boundary_rows"), Color.Red);
			((Form)commModalFrm).ShowDialog();
		}
		else
		{
			_dgvDataList.RemoveAt(selectedIndex - 1);
			RefreshChannelIndex();
			((Control)dgv1).Refresh();
		}
	}

	private void dgv1_DragChanged(object sender, StringsEventArgs e)
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
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
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Expected O, but got Unknown
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Expected O, but got Unknown
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Expected O, but got Unknown
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Expected O, but got Unknown
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e4: Expected O, but got Unknown
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad8: Expected O, but got Unknown
		//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd7: Expected O, but got Unknown
		//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1e: Expected O, but got Unknown
		//IL_0d50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ece: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_115b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1165: Expected O, but got Unknown
		//IL_11a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1285: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1330: Unknown result type (might be due to invalid IL or missing references)
		//IL_133a: Expected O, but got Unknown
		//IL_136b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1464: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_14fd: Expected O, but got Unknown
		//IL_154b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1555: Expected O, but got Unknown
		//IL_15a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1609: Expected O, but got Unknown
		//IL_168a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1733: Unknown result type (might be due to invalid IL or missing references)
		//IL_173d: Expected O, but got Unknown
		//IL_178b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1844: Unknown result type (might be due to invalid IL or missing references)
		components = new Container();
		gridPanel1 = new GridPanel();
		dgv1 = new Table();
		contextMenuStrip1 = new ContextMenuStrip(components);
		addNewRowToolStripMenuItem = new ToolStripMenuItem();
		deleteSelectedRowToolStripMenuItem = new ToolStripMenuItem();
		gridPanel2 = new GridPanel();
		label2 = new Label();
		btn_SaveBB = new Button();
		btn_UploadBB = new Button();
		panel3 = new Panel();
		sel_band = new Select();
		label3 = new Label();
		lab_FactoryReset = new Label();
		label1 = new Label();
		grpan_Btn = new GridPanel();
		label5 = new Label();
		label4 = new Label();
		panel2 = new Panel();
		btn_SaveMofify = new Button();
		panel1 = new Panel();
		btn_AddListview = new Button();
		btn_quit = new Button();
		sel_portname = new Select();
		btn_refresh = new Button();
		btn_connect = new Button();
		((Control)gridPanel1).SuspendLayout();
		((Control)contextMenuStrip1).SuspendLayout();
		((Control)gridPanel2).SuspendLayout();
		((Control)panel3).SuspendLayout();
		((Control)grpan_Btn).SuspendLayout();
		((Control)panel2).SuspendLayout();
		((Control)panel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((ContainerPanel)gridPanel1).Back = Color.FromArgb(38, 41, 43);
		((Control)gridPanel1).BackColor = Color.FromArgb(38, 41, 43);
		((Control)gridPanel1).Controls.Add((Control)(object)dgv1);
		((Control)gridPanel1).Controls.Add((Control)(object)gridPanel2);
		((Control)gridPanel1).Controls.Add((Control)(object)grpan_Btn);
		((Control)gridPanel1).Dock = (DockStyle)5;
		((IControl)gridPanel1).HandCursor = Cursors.Default;
		((Control)gridPanel1).Location = new Point(0, 0);
		((Control)gridPanel1).Margin = new Padding(0);
		((Control)gridPanel1).Name = "gridPanel1";
		((Control)gridPanel1).Size = new Size(840, 648);
		gridPanel1.Span = "100%;100%;100%;100%;100%;-50  50 95% 5%";
		((Control)gridPanel1).TabIndex = 0;
		((Control)gridPanel1).Text = "gridPanel1";
		((Control)dgv1).BackColor = Color.FromArgb(38, 41, 43);
		dgv1.ClipboardCopyFocusedCell = true;
		((IControl)dgv1).ColorScheme = (TAMode)2;
		((Control)dgv1).ContextMenuStrip = contextMenuStrip1;
		((Control)dgv1).Cursor = Cursors.Default;
		((Control)dgv1).Dock = (DockStyle)5;
		dgv1.EditInputStyle = (TEditInputStyle)2;
		dgv1.EditMode = (TEditMode)2;
		dgv1.EditSelection = (TEditSelection)2;
		dgv1.Gap = 12;
		dgv1.HandShortcutKeys = false;
		((Control)dgv1).Location = new Point(5, 105);
		((Control)dgv1).Margin = new Padding(5);
		((Control)dgv1).Name = "dgv1";
		dgv1.RowSelectedBg = Color.FromArgb(35, 255, 233, 0);
		((Control)dgv1).Size = new Size(830, 511);
		((Control)dgv1).TabIndex = 8;
		((Control)dgv1).TabStop = false;
		((Control)dgv1).Text = "table1";
		dgv1.CellBeginEdit += new BeginEditEventHandler(dgv1_CellBeginEdit);
		dgv1.CellBeginEditInputStyle += new BeginEditInputStyleEventHandler(dgv1_CellBeginEditInputStyle);
		dgv1.CellEndEdit += new EndEditEventHandler(dgv1_CellEndEdit);
		dgv1.SelectIndexChanged += dgv1_SelectIndexChanged;
		((ToolStrip)contextMenuStrip1).BackColor = Color.White;
		((ToolStrip)contextMenuStrip1).Items.AddRange((ToolStripItem[])(object)new ToolStripItem[2]
		{
			(ToolStripItem)addNewRowToolStripMenuItem,
			(ToolStripItem)deleteSelectedRowToolStripMenuItem
		});
		((Control)contextMenuStrip1).Name = "contextMenuStrip1";
		((Control)contextMenuStrip1).Size = new Size(192, 48);
		((ToolStripItem)addNewRowToolStripMenuItem).BackColor = Color.White;
		((ToolStripItem)addNewRowToolStripMenuItem).ForeColor = Color.Black;
		((ToolStripItem)addNewRowToolStripMenuItem).Name = "addNewRowToolStripMenuItem";
		((ToolStripItem)addNewRowToolStripMenuItem).Size = new Size(191, 22);
		((ToolStripItem)addNewRowToolStripMenuItem).Text = "Add new row";
		((ToolStripItem)addNewRowToolStripMenuItem).Click += addNewRowToolStripMenuItem_Click;
		((ToolStripItem)deleteSelectedRowToolStripMenuItem).BackColor = Color.White;
		((ToolStripItem)deleteSelectedRowToolStripMenuItem).ForeColor = Color.Black;
		((ToolStripItem)deleteSelectedRowToolStripMenuItem).Name = "deleteSelectedRowToolStripMenuItem";
		((ToolStripItem)deleteSelectedRowToolStripMenuItem).Size = new Size(191, 22);
		((ToolStripItem)deleteSelectedRowToolStripMenuItem).Text = "Delete selected row";
		((ToolStripItem)deleteSelectedRowToolStripMenuItem).Click += deleteSelectedRowToolStripMenuItem_Click;
		((ContainerPanel)gridPanel2).Back = Color.Transparent;
		((Control)gridPanel2).BackColor = Color.Transparent;
		((ContainerPanel)gridPanel2).BorderColor = Color.FromArgb(235, 237, 240);
		((Control)gridPanel2).Controls.Add((Control)(object)label2);
		((Control)gridPanel2).Controls.Add((Control)(object)btn_SaveBB);
		((Control)gridPanel2).Controls.Add((Control)(object)btn_UploadBB);
		((Control)gridPanel2).Controls.Add((Control)(object)panel3);
		((Control)gridPanel2).Controls.Add((Control)(object)label3);
		((Control)gridPanel2).Controls.Add((Control)(object)lab_FactoryReset);
		((Control)gridPanel2).Controls.Add((Control)(object)label1);
		((IControl)gridPanel2).HandCursor = Cursors.Default;
		gridPanel1.SetIndex((Control)(object)gridPanel2, 1);
		((Control)gridPanel2).Location = new Point(0, 50);
		((Control)gridPanel2).Margin = new Padding(0);
		((Control)gridPanel2).Name = "gridPanel2";
		((Control)gridPanel2).Size = new Size(840, 50);
		gridPanel2.Span = "15% 5% 15% 15% 15% 15% 15% 5%";
		((Control)gridPanel2).TabIndex = 7;
		((Control)gridPanel2).Text = "gridPanel2";
		gridPanel2.SetIndex((Control)(object)label2, 4);
		((Control)label2).Location = new Point(299, 5);
		((Control)label2).Margin = new Padding(5);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(116, 40);
		((Control)label2).TabIndex = 45;
		label2.TextAlign = (ContentAlignment)64;
		((IControl)btn_SaveBB).ColorScheme = (TAMode)1;
		btn_SaveBB.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_SaveBB).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		btn_SaveBB.ForeColor = Color.White;
		gridPanel2.SetIndex((Control)(object)btn_SaveBB, 6);
		((Control)btn_SaveBB).Location = new Point(551, 5);
		((Control)btn_SaveBB).Margin = new Padding(5);
		((Control)btn_SaveBB).Name = "btn_SaveBB";
		((Control)btn_SaveBB).Size = new Size(116, 40);
		((Control)btn_SaveBB).TabIndex = 38;
		((Control)btn_SaveBB).Text = "Save BB_Freq";
		btn_SaveBB.WaveSize = 0;
		((Control)btn_SaveBB).Click += btn_SaveBB_Click;
		((IControl)btn_UploadBB).ColorScheme = (TAMode)1;
		btn_UploadBB.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_UploadBB).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		btn_UploadBB.ForeColor = Color.FromArgb(35, 35, 35);
		gridPanel2.SetIndex((Control)(object)btn_UploadBB, 7);
		((Control)btn_UploadBB).Location = new Point(677, 5);
		((Control)btn_UploadBB).Margin = new Padding(5);
		((Control)btn_UploadBB).Name = "btn_UploadBB";
		((Control)btn_UploadBB).Size = new Size(116, 40);
		((Control)btn_UploadBB).TabIndex = 36;
		((Control)btn_UploadBB).Text = "Upload BB_Freq";
		btn_UploadBB.WaveSize = 0;
		((Control)btn_UploadBB).Click += btn_UploadBB_Click;
		panel3.Back = Color.Transparent;
		panel3.Controls.Add((Control)(object)sel_band);
		((Control)panel3).Dock = (DockStyle)5;
		gridPanel2.SetIndex((Control)(object)panel3, 1);
		((Control)panel3).Location = new Point(0, 0);
		((Control)panel3).Margin = new Padding(0);
		((Control)panel3).Name = "panel3";
		((Control)panel3).Padding = new Padding(5);
		((Control)panel3).Size = new Size(126, 50);
		((Control)panel3).TabIndex = 44;
		((Control)panel3).Text = "panel3";
		((Input)sel_band).BackColor = Color.FromArgb(30, 34, 37);
		((Input)sel_band).BorderWidth = 0f;
		((IControl)sel_band).ColorScheme = (TAMode)2;
		((Control)sel_band).Dock = (DockStyle)5;
		((Control)sel_band).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Input)sel_band).ForeColor = Color.White;
		sel_band.Items.AddRange(new object[3] { "bandA", "bandB", "bandC" });
		sel_band.List = true;
		((Control)sel_band).Location = new Point(5, 5);
		((Control)sel_band).Margin = new Padding(5);
		((Control)sel_band).Name = "sel_band";
		sel_band.SelectedIndex = 0;
		sel_band.SelectedValue = "bandA";
		((Control)sel_band).Size = new Size(116, 40);
		((Control)sel_band).TabIndex = 26;
		((Control)sel_band).Text = "bandA";
		((Input)sel_band).WaveSize = 0;
		sel_band.SelectedIndexChanged += new IntEventHandler(sel_band_SelectedIndexChanged);
		gridPanel2.SetIndex((Control)(object)label3, 4);
		((Control)label3).Location = new Point(173, 5);
		((Control)label3).Margin = new Padding(5);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(116, 40);
		((Control)label3).TabIndex = 43;
		label3.TextAlign = (ContentAlignment)64;
		((Control)lab_FactoryReset).Cursor = Cursors.SizeAll;
		((Control)lab_FactoryReset).ForeColor = Color.White;
		gridPanel2.SetIndex((Control)(object)lab_FactoryReset, 5);
		((Control)lab_FactoryReset).Location = new Point(425, 5);
		((Control)lab_FactoryReset).Margin = new Padding(5);
		((Control)lab_FactoryReset).Name = "lab_FactoryReset";
		((Control)lab_FactoryReset).Size = new Size(116, 40);
		((Control)lab_FactoryReset).TabIndex = 41;
		((Control)lab_FactoryReset).Text = "1111";
		lab_FactoryReset.TextAlign = (ContentAlignment)64;
		((Control)lab_FactoryReset).MouseClick += new MouseEventHandler(label8_MouseClick);
		gridPanel2.SetIndex((Control)(object)label1, 4);
		((Control)label1).Location = new Point(131, 5);
		((Control)label1).Margin = new Padding(5);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(32, 40);
		((Control)label1).TabIndex = 37;
		label1.TextAlign = (ContentAlignment)64;
		((ContainerPanel)grpan_Btn).Back = Color.Transparent;
		((Control)grpan_Btn).BackColor = Color.Transparent;
		((ContainerPanel)grpan_Btn).BorderColor = Color.FromArgb(235, 237, 240);
		((Control)grpan_Btn).Controls.Add((Control)(object)label5);
		((Control)grpan_Btn).Controls.Add((Control)(object)label4);
		((Control)grpan_Btn).Controls.Add((Control)(object)panel2);
		((Control)grpan_Btn).Controls.Add((Control)(object)panel1);
		((Control)grpan_Btn).Controls.Add((Control)(object)btn_quit);
		((Control)grpan_Btn).Controls.Add((Control)(object)sel_portname);
		((Control)grpan_Btn).Controls.Add((Control)(object)btn_refresh);
		((Control)grpan_Btn).Controls.Add((Control)(object)btn_connect);
		gridPanel1.SetIndex((Control)(object)grpan_Btn, 1);
		((Control)grpan_Btn).Location = new Point(0, 0);
		((Control)grpan_Btn).Margin = new Padding(0);
		((Control)grpan_Btn).Name = "grpan_Btn";
		((Control)grpan_Btn).Size = new Size(840, 50);
		grpan_Btn.Span = "15% 5% 15% 15% 15% 15% 15% 5%";
		((Control)grpan_Btn).TabIndex = 1;
		((Control)grpan_Btn).Text = "gridPanel2";
		((Control)grpan_Btn).Click += grpan_Btn_Click;
		grpan_Btn.SetIndex((Control)(object)label5, 5);
		((Control)label5).Location = new Point(425, 5);
		((Control)label5).Margin = new Padding(5);
		((Control)label5).Name = "label5";
		((Control)label5).Size = new Size(116, 40);
		((Control)label5).TabIndex = 42;
		label5.TextAlign = (ContentAlignment)64;
		grpan_Btn.SetIndex((Control)(object)label4, 4);
		((Control)label4).Location = new Point(299, 5);
		((Control)label4).Margin = new Padding(5);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(116, 40);
		((Control)label4).TabIndex = 41;
		label4.TextAlign = (ContentAlignment)64;
		panel2.Back = Color.Transparent;
		panel2.Controls.Add((Control)(object)btn_SaveMofify);
		((Control)panel2).Dock = (DockStyle)5;
		grpan_Btn.SetIndex((Control)(object)panel2, 6);
		((Control)panel2).Location = new Point(546, 0);
		((Control)panel2).Margin = new Padding(0);
		((Control)panel2).Name = "panel2";
		((Control)panel2).Padding = new Padding(5);
		((Control)panel2).Size = new Size(126, 50);
		((Control)panel2).TabIndex = 40;
		((Control)panel2).Text = "pan_saveMofify";
		((IControl)btn_SaveMofify).ColorScheme = (TAMode)1;
		btn_SaveMofify.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_SaveMofify).Dock = (DockStyle)5;
		((Control)btn_SaveMofify).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		btn_SaveMofify.ForeColor = Color.FromArgb(255, 255, 255);
		((Control)btn_SaveMofify).Location = new Point(5, 5);
		((Control)btn_SaveMofify).Margin = new Padding(5);
		((Control)btn_SaveMofify).Name = "btn_SaveMofify";
		((Control)btn_SaveMofify).Size = new Size(116, 40);
		((Control)btn_SaveMofify).TabIndex = 36;
		((Control)btn_SaveMofify).Text = "Save Mofify";
		btn_SaveMofify.WaveSize = 0;
		((Control)btn_SaveMofify).Click += btn_SaveMofify_Click;
		panel1.Back = Color.Transparent;
		panel1.Controls.Add((Control)(object)btn_AddListview);
		((Control)panel1).Dock = (DockStyle)5;
		grpan_Btn.SetIndex((Control)(object)panel1, 7);
		((Control)panel1).Location = new Point(672, 0);
		((Control)panel1).Margin = new Padding(0);
		((Control)panel1).Name = "panel1";
		((Control)panel1).Padding = new Padding(5);
		((Control)panel1).Size = new Size(126, 50);
		((Control)panel1).TabIndex = 39;
		((Control)panel1).Text = "pan_addListview";
		((IControl)btn_AddListview).ColorScheme = (TAMode)1;
		btn_AddListview.DefaultBack = Color.FromArgb(95, 95, 96);
		((Control)btn_AddListview).Dock = (DockStyle)5;
		((Control)btn_AddListview).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		btn_AddListview.ForeColor = Color.White;
		((Control)btn_AddListview).Location = new Point(5, 5);
		((Control)btn_AddListview).Margin = new Padding(5);
		((Control)btn_AddListview).Name = "btn_AddListview";
		((Control)btn_AddListview).Size = new Size(116, 40);
		((Control)btn_AddListview).TabIndex = 39;
		((Control)btn_AddListview).Text = "Add listview";
		btn_AddListview.WaveSize = 0;
		((Control)btn_AddListview).Click += btn_AddListview_Click;
		btn_quit.DisplayStyle = (TButtonDisplayStyle)2;
		btn_quit.ForeColor = Color.White;
		btn_quit.Ghost = true;
		btn_quit.Icon = (Image)(object)Resources.关闭1;
		btn_quit.IconRatio = 1.1f;
		grpan_Btn.SetIndex((Control)(object)btn_quit, 9);
		((Control)btn_quit).Location = new Point(798, 0);
		((Control)btn_quit).Margin = new Padding(0);
		((Control)btn_quit).Name = "btn_quit";
		((Control)btn_quit).Size = new Size(42, 50);
		((Control)btn_quit).TabIndex = 35;
		((Control)btn_quit).TabStop = false;
		((Control)btn_quit).Text = "Quit";
		btn_quit.WaveSize = 0;
		((Control)btn_quit).Click += btn_quit_Click;
		((Control)btn_quit).MouseClick += new MouseEventHandler(btn_quit_MouseClick);
		((Input)sel_portname).BackColor = Color.FromArgb(30, 34, 37);
		((Input)sel_portname).BorderWidth = 0f;
		((IControl)sel_portname).ColorScheme = (TAMode)2;
		((Control)sel_portname).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Input)sel_portname).ForeColor = Color.White;
		grpan_Btn.SetIndex((Control)(object)sel_portname, 1);
		sel_portname.List = true;
		((Control)sel_portname).Location = new Point(5, 5);
		((Control)sel_portname).Margin = new Padding(5);
		((Control)sel_portname).Name = "sel_portname";
		((Control)sel_portname).Size = new Size(116, 40);
		((Control)sel_portname).TabIndex = 26;
		((Input)sel_portname).WaveSize = 0;
		sel_portname.SelectedIndexChanged += new IntEventHandler(sel_portname_SelectedIndexChanged);
		btn_refresh.DisplayStyle = (TButtonDisplayStyle)2;
		btn_refresh.ForeColor = Color.White;
		btn_refresh.Ghost = true;
		btn_refresh.Icon = (Image)(object)Resources.refresh_yellow;
		btn_refresh.IconRatio = 1f;
		grpan_Btn.SetIndex((Control)(object)btn_refresh, 2);
		((Control)btn_refresh).Location = new Point(126, 0);
		((Control)btn_refresh).Margin = new Padding(0);
		((Control)btn_refresh).Name = "btn_refresh";
		((Control)btn_refresh).Size = new Size(42, 50);
		((Control)btn_refresh).TabIndex = 29;
		((Control)btn_refresh).Text = "Refresh";
		btn_refresh.WaveSize = 0;
		((Control)btn_refresh).Click += btn_refresh_Click;
		btn_connect.DefaultBack = Color.FromArgb(255, 233, 0);
		((Control)btn_connect).Font = new Font("宋体", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		btn_connect.ForeColor = Color.FromArgb(35, 35, 35);
		grpan_Btn.SetIndex((Control)(object)btn_connect, 3);
		((Control)btn_connect).Location = new Point(173, 5);
		((Control)btn_connect).Margin = new Padding(5);
		((Control)btn_connect).Name = "btn_connect";
		((Control)btn_connect).Size = new Size(116, 40);
		((Control)btn_connect).TabIndex = 30;
		((Control)btn_connect).Text = "Connect";
		btn_connect.WaveSize = 0;
		((Control)btn_connect).Click += btn_connect_Click;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).Controls.Add((Control)(object)gridPanel1);
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "updateChannel";
		((Control)this).Size = new Size(840, 648);
		((UserControl)this).Load += updateChannel_Load;
		((Control)gridPanel1).ResumeLayout(false);
		((Control)contextMenuStrip1).ResumeLayout(false);
		((Control)gridPanel2).ResumeLayout(false);
		((Control)panel3).ResumeLayout(false);
		((Control)grpan_Btn).ResumeLayout(false);
		((Control)panel2).ResumeLayout(false);
		((Control)panel1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
