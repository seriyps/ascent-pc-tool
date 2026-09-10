using System;
using System.Configuration;
using System.IO;
using Caddx_PCTool.Update;

namespace Caddx_PCTool;

public class ConstVal
{
	private static readonly Lazy<string> SysParamPathValue = new Lazy<string>(ResolveSysParamPath);

	public static readonly string FONT_TitleENPath = ConfigurationManager.AppSettings["FONT_TitleENPath"];

	public static readonly string FONT_TextENPath = ConfigurationManager.AppSettings["FONT_TextENPath"];

	public static readonly string FONT_TitleZHPath = ConfigurationManager.AppSettings["FONT_TitleZHPath"];

	public static readonly string FONT_TextZHPath = ConfigurationManager.AppSettings["FONT_TextZHPath"];

	public static readonly string FONT_TitleRUPath = ConfigurationManager.AppSettings["FONT_TitleRUPath"];

	public static readonly string FONT_TextRUPath = ConfigurationManager.AppSettings["FONT_TextRUPath"];

	public static readonly string ASCENT_GOGGLES_Path = ConfigurationManager.AppSettings["ASCENT_GOGGLES_Path"];

	public static readonly string ASCENT_LITEVTX_Path = ConfigurationManager.AppSettings["ASCENT_LITEVTX_Path"];

	public static readonly string ASCENT_VRX_Path = ConfigurationManager.AppSettings["ASCENT_VRX_Path"];

	public static readonly string ASCENT_GTPRO_Path = ConfigurationManager.AppSettings["ASCENT_GTPRO_Path"];

	public static readonly string HUB_JSON_Path = ConfigurationManager.AppSettings["HUB_JSON_Path"];

	public static readonly string RCMODE_JSON_Path = ConfigurationManager.AppSettings["RCMODE_JSON_Path"];

	public static string SYSParamPath => SysParamPathValue.Value;

	private static string ResolveSysParamPath()
	{
		try
		{
			string variant = (Program.SoftwareVersion.Contains("_C") ? "C" : "B");
			string text = ConfigurationManager.AppSettings["UpdateUrl"] ?? string.Empty;
			VariantUpdateConfig config = VariantUpdateConfig.FromValues(variant, string.IsNullOrWhiteSpace(text) ? "about:blank" : text);
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			UpdateDataPaths updateDataPaths = new UpdateDataPaths(folderPath, config);
			return Path.Combine(updateDataPaths.DataDirectory, ConfigurationManager.AppSettings["SYSParamPath"] ?? "SysParam.data");
		}
		catch
		{
			return ConfigurationManager.AppSettings["SYSParamPath"] ?? "SysParam.data";
		}
	}
}
