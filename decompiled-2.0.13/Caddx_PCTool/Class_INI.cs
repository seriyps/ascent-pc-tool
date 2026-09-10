using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class Class_INI
{
	public string path;

	public static string str = "";

	public static string strOne = "";

	public Class_INI()
	{
	}

	public Class_INI(string FileName)
	{
		path = FileName;
	}

	[DllImport("kernel32")]
	private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

	[DllImport("kernel32")]
	private static extern int GetPrivateProfileString(string section, string key, string defVal, byte[] retVal, int size, string filePath);

	[DllImport("kernel32")]
	private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

	[DllImport("Kernel32.dll")]
	private static extern int GetPrivateProfileInt(string strAppName, string strKeyName, int nDefault, string strFileName);

	[DllImport("Kernel32.dll")]
	private static extern int GetPrivateProfileSectionNamesA(byte[] buffer, int iLen, string fileName);

	[DllImport("Kernel32.dll")]
	private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, int nSize, string lpFileName);

	[DllImport("Kernel32.dll")]
	public static extern long WritePrivateProfileSection(string strAppName, string strkeyandvalue, string strFileName);

	public string IniReadValue(string area, string key, string def)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (File.Exists(path))
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			int privateProfileString = GetPrivateProfileString(Path.GetFileNameWithoutExtension(area), key, def, stringBuilder, 1024, path);
			return stringBuilder.ToString();
		}
		MessageBox.Show("未找到ini文件");
		using (File.Create(path))
		{
		}
		return string.Empty;
	}

	public void FileExists(string filePath)
	{
		if (!File.Exists(filePath))
		{
			using (FileStream fileStream = File.Create(filePath))
			{
				fileStream.Close();
			}
		}
	}

	public ArrayList ReadAllSections()
	{
		byte[] array = new byte[65535];
		int privateProfileSectionNamesA = GetPrivateProfileSectionNamesA(array, array.GetUpperBound(0), path);
		ArrayList arrayList = new ArrayList();
		if (privateProfileSectionNamesA > 0)
		{
			int num = 0;
			int num2 = 0;
			for (num = 0; num < privateProfileSectionNamesA; num++)
			{
				if (array[num] == 0)
				{
					string text = Encoding.Default.GetString(array, num2, num - num2).Trim();
					num2 = num + 1;
					if (text != "")
					{
						arrayList.Add(text);
					}
				}
			}
		}
		return arrayList;
	}

	public string GetIniKeyValueForStr(string section, string key)
	{
		if (section.Trim().Length <= 0 || key.Trim().Length <= 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(256);
		GetPrivateProfileString(section, key, string.Empty, stringBuilder, 256, path);
		return stringBuilder.ToString().Trim();
	}

	public bool WriteIniKey(string section, string key, string value)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (File.Exists(path))
		{
			try
			{
				if (WritePrivateProfileString(section, key, value, path) > 0)
				{
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
		MessageBox.Show("未找到路径文件");
		return false;
	}

	public string inimathget(string area, string code, string filename)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		str = Application.StartupPath + "\\" + filename;
		strOne = Path.GetFileNameWithoutExtension(str);
		if (File.Exists(str))
		{
			return IniReadValue(area, code, "");
		}
		MessageBox.Show("配置文件丢失");
		return null;
	}

	public byte[] IniReadValues(string section, string key)
	{
		byte[] array = new byte[255];
		int privateProfileString = GetPrivateProfileString(section, key, "", array, 255, path);
		return array;
	}

	public string[] IniReadAllSection()
	{
		byte[] sectionByte = IniReadValues(null, null);
		return ByteToString(sectionByte);
	}

	private string[] ByteToString(byte[] sectionByte)
	{
		ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
		string text = aSCIIEncoding.GetString(sectionByte);
		return text.Split(new char[1]);
	}

	public string[] IniReadValues(string Section)
	{
		byte[] sectionByte = IniReadValues(Section, null);
		return ByteToString(sectionByte);
	}

	public static void AddToDatabase(string area, string code, string des, string filename)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		str = Application.StartupPath + "\\" + filename;
		if (File.Exists(str))
		{
			WritePrivateProfileString(area, code, des, str);
		}
		else
		{
			MessageBox.Show("对不起，你所要修改的文件不存在，请确认后再进行修改操作！", "提示信息", (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
	}

	public string inimath(string code, string filename)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		str = Application.StartupPath + "\\" + filename;
		strOne = Path.GetFileNameWithoutExtension(str);
		if (File.Exists(str))
		{
			return IniReadValue(strOne, code, "");
		}
		MessageBox.Show("配置文件丢失");
		return null;
	}
}
