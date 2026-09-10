using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Caddx_PCTool;

public static class FileOperation
{
	public static bool SaveFontCT(string path, long FontCT, long Step2CT)
	{
		try
		{
			string text = DateTime.Now.ToString("yy-MM-dd");
			string path2 = Path.Combine(path, text + "_Font.csv");
			Directory.CreateDirectory(path);
			if (!File.Exists(path2))
			{
				using StreamWriter streamWriter = new StreamWriter(path2, append: false, Encoding.UTF8);
				streamWriter.WriteLine("DateTime,FontCT,Step2CT");
			}
			using (StreamWriter streamWriter2 = new StreamWriter(path2, append: true, Encoding.UTF8))
			{
				string arg = DateTime.Now.ToString("HH_mm_ss_ff");
				streamWriter2.WriteLine($"{arg},{FontCT},{Step2CT}");
			}
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("写入CSV失败：" + ex.Message);
			return false;
		}
	}

	public static bool SaveSearchDeviceCT(string path, long sendCT, string state)
	{
		try
		{
			string text = DateTime.Now.ToString("yy-MM-dd");
			string path2 = Path.Combine(path, text + "_SearchCT.csv");
			Directory.CreateDirectory(path);
			if (!File.Exists(path2))
			{
				using StreamWriter streamWriter = new StreamWriter(path2, append: false, Encoding.UTF8);
				streamWriter.WriteLine("DateTime,costtime,state");
			}
			using (StreamWriter streamWriter2 = new StreamWriter(path2, append: true, Encoding.UTF8))
			{
				string arg = DateTime.Now.ToString("HH_mm_ss_ff");
				streamWriter2.WriteLine($"{arg},{sendCT},{state}");
			}
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("写入CSV失败：" + ex.Message);
			return false;
		}
	}

	public static bool LoadAllCTData(string path, out List<string> strList, out List<uint> ctList)
	{
		strList = new List<string>();
		ctList = new List<uint>();
		try
		{
			using (StreamReader streamReader = new StreamReader(path))
			{
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					if (text.Contains("测试总耗时"))
					{
						strList.Add(text);
						string[] array = text.Split(new char[1] { '=' });
						if (array.Length >= 2)
						{
							ctList.Add(uint.Parse(array[1]));
						}
					}
				}
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool SaveAllCTData(string path, List<uint> ctList)
	{
		try
		{
			string text = DateTime.Now.ToString("yy-MM-dd");
			string path2 = Path.Combine(path, text + "_CTData.csv");
			Directory.CreateDirectory(path);
			if (!File.Exists(path2))
			{
				using StreamWriter streamWriter = new StreamWriter(path2, append: false, Encoding.UTF8);
				streamWriter.WriteLine("DateTime,ctList");
			}
			using (StreamWriter streamWriter2 = new StreamWriter(path2, append: true, Encoding.UTF8))
			{
				for (int i = 0; i < ctList.Count; i++)
				{
					string arg = DateTime.Now.ToString("HH_mm_ss_ff");
					streamWriter2.WriteLine($"{arg},{ctList[i]}");
				}
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
