using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Caddx_PCTool;

internal static class Program
{
	private enum ProcessDPIAwareness
	{
		DPI_Unaware,
		System_DPI_Aware,
		Per_Monitor_DPI_Aware
	}

	public static Stopwatch SplashScreenSW = new Stopwatch();

	public static ShowLogoStatus SwStatus = ShowLogoStatus.company;

	[STAThread]
	private static void Main()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		SplashScreenSW.Restart();
		bool createdNew;
		using (new Mutex(initiallyOwned: true, Application.ProductName, out createdNew))
		{
			if (createdNew)
			{
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				LoadSplashScreen();
				Application.Run((Form)(object)new MainFrm());
			}
			else
			{
				MessageBox.Show("PC Tool is already running", "Error");
				Environment.Exit(0);
			}
		}
	}

	public static void LoadSplashScreen()
	{
		ReadParam();
		Thread.Sleep(100);
		GD.Inst.CurrLang = GetLanguageCodeAndLoad(Thread.CurrentThread.CurrentUICulture);
		string text = ConfigurationManager.AppSettings["SwStatus"];
		if (Enum.TryParse<ShowLogoStatus>(text.Trim(), out var result))
		{
			SwStatus = result;
		}
		SplashScreen.ShowSplashScreen(SwStatus);
		Application.DoEvents();
		SplashScreen.SetStatus("Initialize communication objects.");
	}

	private static byte GetLanguageCodeAndLoad(CultureInfo culture)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		byte b = 2;
		string name = culture.Name;
		string text = name;
		if (text == "zh-CN")
		{
			b = 1;
		}
		GD.Inst.Title_ENPFC = new PrivateFontCollection();
		string text2 = Application.StartupPath + "\\font\\ArgentumSans-Medium.ttf";
		GD.Inst.Title_ENPFC.AddFontFile(text2);
		GD.Inst.Text_ENPFC = new PrivateFontCollection();
		string text3 = Application.StartupPath + "\\font\\ArgentumSans-Light.ttf";
		GD.Inst.Text_ENPFC.AddFontFile(text3);
		GD.Inst.Title_ZH_CNPFC = new PrivateFontCollection();
		string text4 = Application.StartupPath + "\\font\\SourceHanSans-Medium.ttf";
		GD.Inst.Title_ZH_CNPFC.AddFontFile(text4);
		GD.Inst.Text_ZH_CNPFC = new PrivateFontCollection();
		string text5 = Application.StartupPath + "\\font\\SourceHanSans-Regular.ttf";
		GD.Inst.Text_ZH_CNPFC.AddFontFile(text5);
		GD.Inst.TitlePFC = ((b == 2) ? GD.Inst.Title_ENPFC : GD.Inst.Title_ZH_CNPFC);
		GD.Inst.TextPFC = ((b == 2) ? GD.Inst.Text_ENPFC : GD.Inst.Text_ZH_CNPFC);
		return b;
	}

	private static void ReadParam()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string path = "SysParam.data";
			if (!File.Exists(path))
			{
				MessageBox.Show("参数文件不存在");
				return;
			}
			string text = File.ReadAllText(path);
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
			string path = "SysParam.data";
			JsonSerializerSettings val = new JsonSerializerSettings
			{
				Formatting = (Formatting)1,
				NullValueHandling = (NullValueHandling)1,
				ContractResolver = (IContractResolver)(object)new PropertiesOnlyContractResolver()
			};
			string contents = JsonConvert.SerializeObject((object)GD.Inst, val);
			File.WriteAllText(path, contents);
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

	private static void DisableDpiScaling()
	{
		if (Environment.OSVersion.Version.Major < 6)
		{
			return;
		}
		try
		{
			if (Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 15063)
			{
				SetProcessDpiAwarenessContext(-1);
				return;
			}
		}
		catch
		{
		}
		try
		{
			SetProcessDpiAwareness(ProcessDPIAwareness.DPI_Unaware);
			return;
		}
		catch
		{
		}
		try
		{
			SetProcessDPIAware();
		}
		catch
		{
		}
	}

	[DllImport("user32.dll")]
	private static extern bool SetProcessDPIAware();

	[DllImport("shcore.dll")]
	private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool SetProcessDpiAwarenessContext(int value);
}
