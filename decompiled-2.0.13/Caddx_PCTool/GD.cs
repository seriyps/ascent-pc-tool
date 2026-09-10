using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Caddx_PCTool;

public class GD
{
	private static object lockObj = new object();

	private static GD _inst;

	public bool IsSWFirstRun = true;

	public string SoftwareVersion = "V2.0.13";

	public string TestPath = "D:\\1全功能测试.txt";

	public string CustomerPath = "D:\\3客户配置导入.txt";

	public string FontFilePath = "Font\\";

	public byte CmdCode;

	public SysMode CurrSysMode = SysMode.findDevice;

	public ProtocolType CurrProtocolType = ProtocolType.ascent;

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

	public PrivateFontCollection Title_ENPFC;

	public PrivateFontCollection Text_ENPFC;

	public PrivateFontCollection Title_ZH_CNPFC;

	public PrivateFontCollection Text_ZH_CNPFC;

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

	public byte CurrLang { get; set; } = 1;

	public int FontDelay { get; set; } = 20;

	public int ReOpenDelay { get; set; } = 8500;

	public int ReOpenTimeout { get; set; } = 10000;

	public int ReElectDelay { get; set; } = 500;

	[JsonIgnore]
	public bool IsDebugMode { get; set; } = true;

	[JsonIgnore]
	public bool IsOpenAscentUpg { get; set; } = false;

	public int GimbalVID { get; set; } = 6790;

	public string CurrDeviceName { get; set; } = "未找到设备";

	public string CurrPortName { get; set; } = "COM3";

	public int FindDevTimeout_Asce { get; set; } = 8500;

	public int ReOpenDelay_Asce { get; set; } = 8500;

	public int ReOpenTimeout_Asce { get; set; } = 100000;

	public bool IsPortnameChanged { get; set; } = true;

	public string UserLevel { get; set; } = "op";

	public bool IsCangeFileData { get; set; } = false;

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

	public async Task DelayMethod(int milliSecond, bool tb = true)
	{
		int start = Environment.TickCount;
		while (Math.Abs(Environment.TickCount - start) < milliSecond)
		{
			await Task.Delay(10);
			Application.DoEvents();
		}
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
