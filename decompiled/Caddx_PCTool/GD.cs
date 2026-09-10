using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Caddx_PCTool;

public class GD
{
	private static object lockObj = new object();

	private static GD _inst;

	public bool IsUseNewAscentUpg = false;

	public bool IsSWFirstRun = true;

	public string TestPath = "D:\\1全功能测试.txt";

	public string CustomerPath = "D:\\3客户配置导入.txt";

	public string FontFilePath = "resource\\font\\";

	public byte CmdCode;

	public MSPMode CurrMSPMode = MSPMode.msp;

	public ConnectType CurrConnType = ConnectType.serialPort;

	public SerialPortHelper MySP;

	public bool IsUseGyroTimer;

	public bool IsUseAttitTimer;

	public Stopwatch SW_TotalTest = new Stopwatch();

	public Stopwatch SW_UploadFont = new Stopwatch();

	public Stopwatch SW_AllFuntionTest = new Stopwatch();

	public Stopwatch SW_Step2 = new Stopwatch();

	public Stopwatch SW_step3 = new Stopwatch();

	public string SWCsvPath = "SWFile";

	public bool IsUserDefine = false;

	public AscentUpgStep CurrAscStep = AscentUpgStep.None;

	public ManualResetEvent SendDataReset = new ManualResetEvent(initialState: false);

	public ManualResetEvent AutoUpgradeReset = new ManualResetEvent(initialState: false);

	public Dictionary<int, string> DeviceNameDict;

	public Dictionary<int, string> PortNameDict;

	public Dictionary<int, string> VIDDict;

	public Dictionary<int, string> PIDDict;

	public UpgradeProcessFSM UpgFSM;

	public UsbSerialportFSM UsbFSM;

	public Upg_Gimbal Upg_Gim;

	public Serialport_Gimbal SP_Gim;

	public bool IsAutoRefreshGimData;

	public Stopwatch SW_Calib = new Stopwatch();

	public PrivateFontCollection TitlePFC;

	public PrivateFontCollection TextPFC;

	public PrivateFontCollection Title_EN_USPFC;

	public PrivateFontCollection Text_EN_USPFC;

	public PrivateFontCollection Title_ZH_CNPFC;

	public PrivateFontCollection Text_ZH_CNPFC;

	public PrivateFontCollection Title_RU_RUPFC;

	public PrivateFontCollection Text_RU_RUPFC;

	public static GD Inst
	{
		get
		{
			lock (lockObj)
			{
				if (_inst == null)
				{
					_inst = new GD();
				}
				return _inst;
			}
		}
	}

	public int CamHubSelVtxIndex { get; set; } = 0;

	[JsonIgnore]
	public UserLevel CurrUserLevel { get; set; } = UserLevel.op;

	public byte CurrLang { get; set; } = 1;

	public int FontDelay { get; set; } = 20;

	public int ReOpenDelay { get; set; } = 8500;

	public int ReOpenTimeout { get; set; } = 10000;

	public int ReElectDelay { get; set; } = 1000;

	[JsonIgnore]
	public FindDeviceFrm FindDeviceFrm { get; set; }

	[JsonIgnore]
	public ISerialPortSessionManager SerialPortSessions { get; private set; }

	[JsonIgnore]
	public bool IsDebugMode { get; set; } = true;

	[JsonIgnore]
	public bool IsOpenAscentUpg { get; set; } = false;

	public int GimbalVID { get; set; } = 6790;

	public string CurrDeviceName { get; set; } = "未找到设备";

	public string CurrPortName { get; set; } = "COM3";

	public int FindDevTimeout_Asce { get; set; } = 8500;

	public int FindDevRetryInterval_Asce { get; set; } = 1500;

	public int ReOpenDelay_Asce { get; set; } = 8500;

	[JsonIgnore]
	public Dictionary<string, int> ReOpenDelayByDevName_Asce { get; set; } = CreateDefaultReOpenDelayByDevName_Asce();

	[JsonIgnore]
	public Dictionary<string, int> ReOpenDelayByVID_Asce { get; set; } = CreateDefaultReOpenDelayByVID_Asce();

	public int ReOpenTimeout_Asce { get; set; } = 60000;

	public byte ForceSearchMode { get; set; } = 3;

	public byte PortRetryTimeLimit { get; set; } = 4;

	[JsonIgnore]
	public SysMode CurrSysMode { get; set; } = SysMode.findDevice;

	[JsonIgnore]
	public FuntionType CurrSelFunType { get; set; }

	public bool IsCangeFileData { get; set; } = false;

	public void InitializeSerialPortSessions()
	{
		if (SerialPortSessions == null)
		{
			SerialPortSessions = new SerialPortSessionManager(new AscentSerialPortSessionTransportFactory(), new UsbSerialMonitorDeviceEnumerator(), TimeSpan.FromSeconds(1.0));
		}
	}

	public int ResolveReOpenDelay_Asce(UsbDevInfo usbInfo, out string matchSource)
	{
		EnsureReOpenDelayMaps();
		matchSource = "default";
		if (usbInfo != null)
		{
			string text = NormalizeDeviceName(usbInfo.DevName);
			if (!string.IsNullOrEmpty(text))
			{
				foreach (KeyValuePair<string, int> item in ReOpenDelayByDevName_Asce)
				{
					if (NormalizeDeviceName(item.Key) == text && item.Value > 0)
					{
						matchSource = "DevName:" + item.Key;
						return item.Value;
					}
				}
			}
			string text2 = NormalizeVid(usbInfo.VID);
			if (!string.IsNullOrEmpty(text2))
			{
				foreach (KeyValuePair<string, int> item2 in ReOpenDelayByVID_Asce)
				{
					if (NormalizeVid(item2.Key) == text2 && item2.Value > 0)
					{
						matchSource = "VID:" + item2.Key;
						return item2.Value;
					}
				}
			}
		}
		return Math.Max(0, ReOpenDelay_Asce);
	}

	private void EnsureReOpenDelayMaps()
	{
		if (ReOpenDelayByDevName_Asce == null)
		{
			ReOpenDelayByDevName_Asce = CreateDefaultReOpenDelayByDevName_Asce();
			WriteLog.WriteLogFileToUI($"重连延时配置已初始化 DevName 表，条目数={ReOpenDelayByDevName_Asce.Count}", Color.DarkCyan);
		}
		else
		{
			MergeDelayMap("DevName", ReOpenDelayByDevName_Asce, CreateDefaultReOpenDelayByDevName_Asce(), NormalizeDeviceName);
		}
		if (ReOpenDelayByVID_Asce == null)
		{
			ReOpenDelayByVID_Asce = CreateDefaultReOpenDelayByVID_Asce();
			WriteLog.WriteLogFileToUI($"重连延时配置已初始化 VID 表，条目数={ReOpenDelayByVID_Asce.Count}", Color.DarkCyan);
		}
		else
		{
			MergeDelayMap("VID", ReOpenDelayByVID_Asce, CreateDefaultReOpenDelayByVID_Asce(), NormalizeVid);
		}
	}

	private static void MergeDelayMap(string mapName, Dictionary<string, int> currentMap, Dictionary<string, int> defaultMap, Func<string, string> normalize)
	{
		foreach (KeyValuePair<string, int> item in defaultMap)
		{
			string normalizedDefaultKey = normalize(item.Key);
			string text = currentMap.Keys.FirstOrDefault((string key) => normalize(key) == normalizedDefaultKey);
			if (string.IsNullOrEmpty(text))
			{
				currentMap[item.Key] = item.Value;
				WriteLog.WriteLogFileToUI($"重连延时配置补齐 {mapName} 项：key={item.Key}, delay={item.Value}", Color.DarkCyan);
			}
			else if (currentMap[text] <= 0)
			{
				currentMap[text] = item.Value;
				WriteLog.WriteLogFileToUI($"重连延时配置修正 {mapName} 项：key={text}, delay={item.Value}", Color.DarkCyan);
			}
		}
	}

	private static string NormalizeDeviceName(string devName)
	{
		if (string.IsNullOrWhiteSpace(devName))
		{
			return string.Empty;
		}
		return devName.Trim().Replace(" ", "").ToUpperInvariant();
	}

	private static string NormalizeVid(string vid)
	{
		if (string.IsNullOrWhiteSpace(vid))
		{
			return string.Empty;
		}
		return vid.Trim().ToUpperInvariant().Replace("0X", "");
	}

	private static Dictionary<string, int> CreateDefaultReOpenDelayByDevName_Asce()
	{
		return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
		{
			["ascent_vrx"] = 9500,
			["ascent_vrx_pro"] = 12000,
			["cx485_pro"] = 12000,
			["ascent_vrx_max"] = 12000,
			["ascent_vrx_max_hf"] = 12000,
			["ascent_vrx_max_wf"] = 12000,
			["ascent_vrx_cine"] = 12000,
			["ascent_gt_pro"] = 9500,
			["ascent_gt_pro_z40"] = 9500,
			["ascent_gt_pro_z8"] = 9500,
			["ascent_gt_pro_hub"] = 9500,
			["ascent_gt"] = 9500,
			["ascent_gt_27"] = 9500,
			["ascent_gt_night"] = 9500,
			["ascent_gt_ultra"] = 9500,
			["ascent_gt_max"] = 11500,
			["ascent_lite"] = 8000,
			["ascent_lite_plus"] = 8000,
			["yohd_micro"] = 8000,
			["ascent_rc"] = 8000,
			["caddx_gm3"] = 6000,
			["caddx_gm1"] = 6000
		};
	}

	private static Dictionary<string, int> CreateDefaultReOpenDelayByVID_Asce()
	{
		return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
		{
			[7541.ToString("X4")] = 9500,
			[7543.ToString("X4")] = 12000,
			[7544.ToString("X4")] = 12000,
			[7545.ToString("X4")] = 12000,
			[7546.ToString("X4")] = 10000,
			[7547.ToString("X4")] = 10000,
			[7542.ToString("X4")] = 11500,
			[7553.ToString("X4")] = 9500,
			[7554.ToString("X4")] = 9500,
			[7555.ToString("X4")] = 9500,
			[7556.ToString("X4")] = 9500,
			[7531.ToString("X4")] = 8000,
			[7532.ToString("X4")] = 8000,
			[7533.ToString("X4")] = 8000,
			[7534.ToString("X4")] = 8000,
			[7535.ToString("X4")] = 8000,
			[7536.ToString("X4")] = 8000,
			[7537.ToString("X4")] = 8000,
			[7538.ToString("X4")] = 8000,
			[7539.ToString("X4")] = 8000,
			[7540.ToString("X4")] = 8000,
			[7552.ToString("X4")] = 8000,
			[6790.ToString("X4")] = 6000
		};
	}

	public GD()
	{
		InitVariable();
	}

	public GD(GD myGD)
	{
		_inst = myGD;
		InitVariable();
	}

	private void InitVariable()
	{
	}

	public void DelayMethod(int milliSecond)
	{
		int tickCount = Environment.TickCount;
		while (Math.Abs(Environment.TickCount - tickCount) < milliSecond)
		{
			Application.DoEvents();
		}
	}
}
