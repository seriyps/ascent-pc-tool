using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caddx_PCTool.Update;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Velopack;

namespace Caddx_PCTool;

internal static class Program
{
	public static Stopwatch SplashScreenSW = new Stopwatch();

	public static readonly string SoftwareVersion = "V2.2.9_C";

	public static DispLogoStatus LogoStatus = DispLogoStatus.consumer;

	[STAThread]
	private static void Main()
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		VelopackApp.Build().SetAutoApplyOnStartup(false).Run();
		SplashScreenSW.Restart();
		bool createdNew;
		using (new Mutex(initiallyOwned: true, Application.ProductName, out createdNew))
		{
			if (createdNew)
			{
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				LogoStatus = ((!SoftwareVersion.Contains("_C")) ? DispLogoStatus.caddx_Industry : DispLogoStatus.consumer);
				LoadSplashScreen();
				GD.Inst.InitializeSerialPortSessions();
				try
				{
					Application.Run((Form)(object)new MainFrm());
					return;
				}
				finally
				{
					try
					{
						Task.Run(async delegate
						{
							if (GD.Inst.SerialPortSessions != null)
							{
								await GD.Inst.SerialPortSessions.StopAsync().ConfigureAwait(continueOnCapturedContext: false);
							}
						}).GetAwaiter().GetResult();
					}
					catch (Exception ex)
					{
						WriteLog.WriteLogFileToUI("停止串口会话管理器失败，desc=" + ex.Message, Color.Red);
					}
				}
			}
			MessageBox.Show("PC Tool is already running", "Error");
			Environment.Exit(0);
		}
	}

	public static void LoadSplashScreen()
	{
		TryRestoreSeedData();
		ReadParam();
		Thread.Sleep(100);
		GD.Inst.CurrLang = GetLanguageCodeAndLoad();
		SplashScreen.ShowSplashScreen();
		Application.DoEvents();
		SplashScreen.SetStatus("Initialize communication objects.");
	}

	private static void TryRestoreSeedData()
	{
		try
		{
			string variant = (SoftwareVersion.Contains("_C") ? "C" : "B");
			string text = ConfigurationManager.AppSettings["UpdateUrl"] ?? string.Empty;
			VariantUpdateConfig config = VariantUpdateConfig.FromValues(variant, string.IsNullOrEmpty(text) ? "about:blank" : text);
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			int num = UpdateBootstrap.RunSeedRecovery(folderPath, Application.StartupPath, config, delegate(string m)
			{
				WriteLog.WriteLogFileToUI(m, Color.Gray);
			});
			if (num > 0)
			{
				WriteLog.WriteLogFileToUI($"启动恢复种子文件 {num} 个", Color.Gray);
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("种子恢复失败，desc=" + ex.Message, Color.Red);
		}
	}

	private static byte GetLanguageCodeAndLoad()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		byte b = Lang.Init();
		GD.Inst.Title_EN_USPFC = new PrivateFontCollection();
		GD.Inst.Title_EN_USPFC.AddFontFile(ConstVal.FONT_TitleENPath);
		GD.Inst.Text_EN_USPFC = new PrivateFontCollection();
		GD.Inst.Text_EN_USPFC.AddFontFile(ConstVal.FONT_TextENPath);
		GD.Inst.Title_ZH_CNPFC = new PrivateFontCollection();
		GD.Inst.Title_ZH_CNPFC.AddFontFile(ConstVal.FONT_TitleZHPath);
		GD.Inst.Text_ZH_CNPFC = new PrivateFontCollection();
		GD.Inst.Text_ZH_CNPFC.AddFontFile(ConstVal.FONT_TextZHPath);
		GD.Inst.Title_RU_RUPFC = new PrivateFontCollection();
		GD.Inst.Title_RU_RUPFC.AddFontFile(ConstVal.FONT_TitleRUPath);
		GD.Inst.Text_RU_RUPFC = new PrivateFontCollection();
		GD.Inst.Text_RU_RUPFC.AddFontFile(ConstVal.FONT_TextRUPath);
		switch ((LangType)b)
		{
		case LangType.zh_CN:
			GD.Inst.TitlePFC = GD.Inst.Title_ZH_CNPFC;
			GD.Inst.TextPFC = GD.Inst.Text_ZH_CNPFC;
			break;
		default:
			GD.Inst.TitlePFC = GD.Inst.Title_EN_USPFC;
			GD.Inst.TextPFC = GD.Inst.Text_EN_USPFC;
			break;
		case LangType.ru_RU:
			GD.Inst.TitlePFC = GD.Inst.Title_RU_RUPFC;
			GD.Inst.TextPFC = GD.Inst.Text_RU_RUPFC;
			break;
		}
		return b;
	}

	private static void ReadParam()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!File.Exists(ConstVal.SYSParamPath))
			{
				MessageBox.Show("参数文件不存在");
				return;
			}
			string text = File.ReadAllText(ConstVal.SYSParamPath);
			GD myGD = JsonConvert.DeserializeObject<GD>(text);
			GD gD = new GD(myGD);
		}
		catch (Exception ex)
		{
			MessageBox.Show("参数读取失败,Desc=" + ex.Message);
		}
	}

	public static void WriteParam()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		try
		{
			string sYSParamPath = ConstVal.SYSParamPath;
			JsonSerializerSettings val = new JsonSerializerSettings
			{
				Formatting = (Formatting)1,
				NullValueHandling = (NullValueHandling)1,
				ContractResolver = (IContractResolver)(object)new PropertiesOnlyContractResolver()
			};
			string contents = JsonConvert.SerializeObject((object)GD.Inst, val);
			File.WriteAllText(sYSParamPath, contents);
		}
		catch (Exception ex)
		{
			MessageBox.Show("参数保存失败,Desc=" + ex.Message);
		}
	}

	private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
	{
		string text = args.Name.Split(new char[1] { ',' })[0];
		string text2 = Path.Combine(Application.StartupPath, "Libs", text + ".dll");
		if (File.Exists(text2))
		{
			return Assembly.LoadFrom(text2);
		}
		return null;
	}
}
