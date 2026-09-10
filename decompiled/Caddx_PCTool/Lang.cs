using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Caddx_PCTool;

public static class Lang
{
	public class LangInfo
	{
		public string Code { get; set; }

		public string DisplayName { get; set; }

		public override string ToString()
		{
			return DisplayName + " (" + Code + ")";
		}
	}

	private class LangMeta
	{
		public string name;

		public string code;

		public string title_font;

		public string text_font;
	}

	private static class LangJsonCache
	{
		private static Dictionary<string, (LangMeta meta, Dictionary<string, string> dict)> _cache = new Dictionary<string, (LangMeta, Dictionary<string, string>)>(StringComparer.OrdinalIgnoreCase);

		public static (LangMeta meta, Dictionary<string, string> dict) Get(string langCode)
		{
			if (_cache.TryGetValue(langCode, out (LangMeta, Dictionary<string, string>) value))
			{
				return value;
			}
			string path = Path.Combine(Application.StartupPath, "resource\\language");
			string text = Path.Combine(path, langCode + ".json");
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			LangMeta item = null;
			if (File.Exists(text))
			{
				try
				{
					string text2 = File.ReadAllText(text);
					JObject val = JObject.Parse(text2);
					JToken obj = val["_meta"];
					JObject val2 = (JObject)(object)((obj is JObject) ? obj : null);
					if (val2 != null)
					{
						item = new LangMeta
						{
							name = (((object)val2["name"])?.ToString() ?? langCode),
							code = (((object)val2["code"])?.ToString() ?? langCode),
							title_font = (((object)val2["title_font"])?.ToString() ?? "ArgentumSans-Medium.ttf"),
							text_font = (((object)val2["text_font"])?.ToString() ?? "ArgentumSans-Light.ttf")
						};
					}
					foreach (JProperty item2 in val.Properties())
					{
						if (item2.Name != "_meta")
						{
							dictionary[item2.Name] = ((object)item2.Value)?.ToString() ?? "";
						}
					}
				}
				catch (Exception ex)
				{
					WriteLog.WriteLogFileToUI("LangManager: 解析语言包失败: " + text + ", " + ex.Message, Color.Red);
				}
			}
			else
			{
				WriteLog.WriteLogFileToUI("LangManager: 语言包文件不存在: " + text, Color.Red);
			}
			(LangMeta, Dictionary<string, string>) tuple = (item, dictionary);
			_cache[langCode] = tuple;
			return tuple;
		}

		public static void Clear()
		{
			_cache.Clear();
		}
	}

	private static class FontLoader
	{
		private static string _lastTitleFontPath;

		private static string _lastTextFontPath;

		public static void Load()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected O, but got Unknown
			string path = Path.Combine(Application.StartupPath, GD.Inst.FontFilePath);
			string text = Path.Combine(path, TitleFontFile);
			string text2 = Path.Combine(path, TextFontFile);
			if (text != _lastTitleFontPath)
			{
				try
				{
					GD.Inst.TitlePFC = new PrivateFontCollection();
					if (File.Exists(text))
					{
						GD.Inst.TitlePFC.AddFontFile(text);
					}
					_lastTitleFontPath = text;
				}
				catch (Exception ex)
				{
					WriteLog.WriteLogFileToUI("LangManager: 加载标题字体失败: " + ex.Message, Color.Red);
				}
			}
			if (!(text2 != _lastTextFontPath))
			{
				return;
			}
			try
			{
				GD.Inst.TextPFC = new PrivateFontCollection();
				if (File.Exists(text2))
				{
					GD.Inst.TextPFC.AddFontFile(text2);
				}
				_lastTextFontPath = text2;
			}
			catch (Exception ex2)
			{
				WriteLog.WriteLogFileToUI("LangManager: 加载正文字体失败: " + ex2.Message, Color.Red);
			}
		}
	}

	private static readonly object _lock = new object();

	private static Dictionary<string, string> _current;

	private static string _currCode;

	private static string _currName;

	private static LangMeta _currentMeta;

	private static List<LangInfo> _availableLanguages;

	private const string FALLBACK_LANG_CODE = "en-US";

	private const string LANG_DIR = "resource\\language";

	private const string CONFIG_LANG_MODE_KEY = "LanguageMode";

	private const int LANG_MODE_SYSTEM = 99;

	public static string CurrentCode
	{
		get
		{
			lock (_lock)
			{
				return _currCode;
			}
		}
	}

	public static string CurrentName => _currName;

	public static string TitleFontFile => _currentMeta?.title_font ?? "ArgentumSans-Medium.ttf";

	public static string TextFontFile => _currentMeta?.text_font ?? "ArgentumSans-Light.ttf";

	public static event EventHandler OnLanguageChanged;

	public static byte Init()
	{
		string langCode = ResolveStartupLanguageCode(ConfigurationManager.AppSettings["LanguageMode"]);
		LoadLanguage(langCode);
		return LangCodeToByte(langCode);
	}

	public static byte Init(string langCode)
	{
		langCode = MatchSystemLanguage(langCode);
		LoadLanguage(langCode);
		return LangCodeToByte(langCode);
	}

	public static List<LangInfo> GetAvailableLanguages()
	{
		lock (_lock)
		{
			if (_availableLanguages != null)
			{
				return _availableLanguages;
			}
			_availableLanguages = new List<LangInfo>();
			string text = Path.Combine(Application.StartupPath, "resource\\language");
			if (!Directory.Exists(text))
			{
				WriteLog.WriteLogFileToUI("LangManager: 语言包目录不存在: " + text, Color.Red);
				return _availableLanguages;
			}
			try
			{
				string[] files = Directory.GetFiles(text, "*.json");
				string[] array = files;
				foreach (string path in array)
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
					string displayName = LangJsonCache.Get(fileNameWithoutExtension).meta?.name ?? fileNameWithoutExtension;
					_availableLanguages.Add(new LangInfo
					{
						Code = fileNameWithoutExtension,
						DisplayName = displayName
					});
				}
				_availableLanguages.Sort((LangInfo a, LangInfo b) => string.Compare(a.Code, b.Code, StringComparison.Ordinal));
			}
			catch (Exception ex)
			{
				WriteLog.WriteLogFileToUI("LangManager: 扫描语言包目录失败: " + ex.Message, Color.Red);
			}
			return _availableLanguages;
		}
	}

	public static void RefreshLanguageList()
	{
		lock (_lock)
		{
			_availableLanguages = null;
			LangJsonCache.Clear();
		}
	}

	private static string ResolveStartupLanguageCode(string langMode)
	{
		if (int.TryParse(langMode, out var result))
		{
			switch (result)
			{
			case 1:
				return "zh-CN";
			case 2:
				return "en-US";
			case 3:
				return "ru-RU";
			}
		}
		return MatchSystemLanguage(Thread.CurrentThread.CurrentUICulture.Name);
	}

	private static string MatchSystemLanguage(string sysLang)
	{
		if (string.IsNullOrWhiteSpace(sysLang))
		{
			return "en-US";
		}
		sysLang = sysLang.Trim();
		string path = Path.Combine(Application.StartupPath, "resource\\language");
		if (!Directory.Exists(path))
		{
			return "en-US";
		}
		string[] source = (from f in Directory.GetFiles(path, "*.json")
			select Path.GetFileNameWithoutExtension(f)).ToArray();
		string text = source.FirstOrDefault((string f) => string.Equals(f, sysLang, StringComparison.OrdinalIgnoreCase));
		if (text != null)
		{
			return text;
		}
		string prefix = sysLang.Split(new char[1] { '-' })[0];
		text = source.FirstOrDefault((string f) => f.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
		if (text != null)
		{
			return text;
		}
		return "en-US";
	}

	public static byte SwitchLanguage(string langCode)
	{
		lock (_lock)
		{
			LoadLanguageInner(langCode);
		}
		OnLanguageChanged?.Invoke(null, EventArgs.Empty);
		return LangCodeToByte(langCode);
	}

	public static string T(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return key;
		}
		lock (_lock)
		{
			if (_current != null && _current.TryGetValue(key, out var value))
			{
				return value;
			}
		}
		(LangMeta, Dictionary<string, string>) tuple = LangJsonCache.Get("en-US");
		if (tuple.Item2 != null && tuple.Item2.TryGetValue(key, out var value2))
		{
			return value2;
		}
		return key;
	}

	public static string T(string key, params object[] args)
	{
		string text = T(key);
		try
		{
			return string.Format(text, args);
		}
		catch
		{
			return text;
		}
	}

	private static void LoadLanguage(string langCode)
	{
		lock (_lock)
		{
			LoadLanguageInner(langCode);
		}
	}

	private static void LoadLanguageInner(string langCode)
	{
		_currCode = langCode;
		(LangMeta, Dictionary<string, string>) tuple = LangJsonCache.Get(langCode);
		_currName = tuple.Item1?.name ?? langCode;
		_current = tuple.Item2 ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		(_currentMeta, _) = tuple;
	}

	private static byte LangCodeToByte(string langCode)
	{
		if (string.IsNullOrEmpty(langCode))
		{
			return 2;
		}
		if (langCode.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
		{
			return 1;
		}
		if (langCode.StartsWith("ru", StringComparison.OrdinalIgnoreCase))
		{
			return 3;
		}
		return 2;
	}
}
