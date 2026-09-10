using System;
using System.Collections.Generic;
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

	private const string FALLBACK_LANG_CODE = "en-US";

	private const string LANG_DIR = "resource\\language";

	private static Dictionary<string, string> _current;

	private static Dictionary<string, string> _fallback;

	private static string _currentCode;

	private static LangMeta _currentMeta;

	private static List<LangInfo> _availableLanguages;

	public static string CurrentCode => _currentCode;

	public static string TitleFontFile => _currentMeta?.title_font ?? "ArgentumSans-Medium.ttf";

	public static string TextFontFile => _currentMeta?.text_font ?? "ArgentumSans-Light.ttf";

	public static byte Init()
	{
		string name = Thread.CurrentThread.CurrentUICulture.Name;
		string langCode = MatchSystemLanguage(name);
		LoadLanguage(langCode);
		return LangCodeToByte(langCode);
	}

	public static byte Init(string langCode)
	{
		LoadLanguage(langCode);
		return LangCodeToByte(langCode);
	}

	public static List<LangInfo> GetAvailableLanguages()
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
			foreach (string text2 in array)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text2);
				string displayName = ReadLangDisplayName(text2, fileNameWithoutExtension);
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

	public static void RefreshLanguageList()
	{
		_availableLanguages = null;
	}

	private static string ReadLangDisplayName(string filePath, string fallbackName)
	{
		try
		{
			string text = File.ReadAllText(filePath);
			JObject val = JObject.Parse(text);
			JToken obj = val["_meta"];
			JObject val2 = (JObject)(object)((obj is JObject) ? obj : null);
			if (val2 != null)
			{
				string text2 = ((object)val2["name"])?.ToString();
				if (!string.IsNullOrEmpty(text2))
				{
					return text2;
				}
			}
		}
		catch
		{
		}
		return fallbackName;
	}

	private static string MatchSystemLanguage(string sysLang)
	{
		if (string.IsNullOrEmpty(sysLang))
		{
			return "en-US";
		}
		string path = Path.Combine(Application.StartupPath, "resource\\language");
		if (!Directory.Exists(path))
		{
			return "en-US";
		}
		string[] source = (from f in Directory.GetFiles(path, "*.json")
			select Path.GetFileNameWithoutExtension(f)).ToArray();
		if (Enumerable.Contains<string>(source, sysLang, StringComparer.OrdinalIgnoreCase))
		{
			return sysLang;
		}
		string prefix = sysLang.Split(new char[1] { '-' })[0];
		string text = source.FirstOrDefault((string f) => f.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
		if (text != null)
		{
			return text;
		}
		return "en-US";
	}

	public static byte SwitchLanguage(string langCode)
	{
		LoadLanguage(langCode);
		LoadFonts();
		return LangCodeToByte(langCode);
	}

	public static string T(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return key;
		}
		if (_current != null && _current.TryGetValue(key, out var value))
		{
			return value;
		}
		if (_fallback != null && _fallback.TryGetValue(key, out var value2))
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
		_currentCode = langCode;
		string basePath = Path.Combine(Application.StartupPath, "resource\\language");
		_current = LoadJsonDict(basePath, langCode, out _currentMeta);
		if (!string.Equals(langCode, "en-US", StringComparison.OrdinalIgnoreCase))
		{
			_fallback = LoadJsonDict(basePath, "en-US", out var _);
		}
		else
		{
			_fallback = null;
		}
	}

	private static Dictionary<string, string> LoadJsonDict(string basePath, string langCode, out LangMeta meta)
	{
		meta = null;
		string text = Path.Combine(basePath, langCode + ".json");
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		if (!File.Exists(text))
		{
			WriteLog.WriteLogFileToUI("LangManager: 语言包文件不存在: " + text, Color.Red);
			return dictionary;
		}
		try
		{
			string text2 = File.ReadAllText(text);
			JObject val = JObject.Parse(text2);
			JToken obj = val["_meta"];
			JObject val2 = (JObject)(object)((obj is JObject) ? obj : null);
			if (val2 != null)
			{
				meta = new LangMeta
				{
					name = (((object)val2["name"])?.ToString() ?? langCode),
					code = (((object)val2["code"])?.ToString() ?? langCode),
					title_font = (((object)val2["title_font"])?.ToString() ?? "ArgentumSans-Medium.ttf"),
					text_font = (((object)val2["text_font"])?.ToString() ?? "ArgentumSans-Light.ttf")
				};
			}
			foreach (JProperty item in val.Properties())
			{
				if (!(item.Name == "_meta"))
				{
					dictionary[item.Name] = ((object)item.Value)?.ToString() ?? "";
				}
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("LangManager: 解析语言包失败: " + text + ", " + ex.Message, Color.Red);
		}
		return dictionary;
	}

	private static void LoadFonts()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		string path = Path.Combine(Application.StartupPath, GD.Inst.FontFilePath);
		GD.Inst.TitlePFC = new PrivateFontCollection();
		string text = Path.Combine(path, TitleFontFile);
		if (File.Exists(text))
		{
			GD.Inst.TitlePFC.AddFontFile(text);
		}
		GD.Inst.TextPFC = new PrivateFontCollection();
		string text2 = Path.Combine(path, TextFontFile);
		if (File.Exists(text2))
		{
			GD.Inst.TextPFC.AddFontFile(text2);
		}
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
