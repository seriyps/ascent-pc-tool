using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Caddx_PCTool;

internal class SplashScreenXMLStorage
{
	private static string ms_StoredValues = "SplashScreen.xml";

	private static string ms_DefaultPercents = "";

	private static string ms_DefaultIncrement = ".015";

	public static string Percents
	{
		get
		{
			return GetValue("Percents", ms_DefaultPercents);
		}
		set
		{
			SetValue("Percents", value);
		}
	}

	public static string Interval
	{
		get
		{
			return GetValue("Interval", ms_DefaultIncrement);
		}
		set
		{
			SetValue("Interval", value);
		}
	}

	private static string StoragePath => Path.Combine(Application.UserAppDataPath, ms_StoredValues);

	private static string GetValue(string name, string defaultValue)
	{
		if (!File.Exists(StoragePath))
		{
			return defaultValue;
		}
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(StoragePath);
			return (!(xmlDocument.DocumentElement.SelectSingleNode(name) is XmlElement xmlElement)) ? defaultValue : xmlElement.InnerText;
		}
		catch
		{
			return defaultValue;
		}
	}

	public static void SetValue(string name, string stringValue)
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = null;
		if (!File.Exists(StoragePath))
		{
			xmlElement = xmlDocument.CreateElement("root");
			xmlDocument.AppendChild(xmlElement);
		}
		else
		{
			try
			{
				xmlDocument.Load(StoragePath);
				xmlElement = xmlDocument.DocumentElement;
			}
			catch
			{
				File.Delete(StoragePath);
				xmlElement = xmlDocument.CreateElement("root");
				xmlDocument.AppendChild(xmlElement);
			}
		}
		XmlElement xmlElement2 = xmlDocument.DocumentElement.SelectSingleNode(name) as XmlElement;
		if (xmlElement2 == null)
		{
			xmlElement2 = xmlDocument.CreateElement(name);
			xmlElement.AppendChild(xmlElement2);
		}
		xmlElement2.InnerText = stringValue;
		xmlDocument.Save(StoragePath);
	}
}
