using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class WriteLog : IDisposable
{
	private static RichTextBox Rtxbox = null;

	private static Form ParentForm = null;

	private static int L_Count = 1000;

	private static string tempcontents = "";

	private static object writelogobj = new object();

	private static readonly object syncFileObj = new object();

	public WriteLog(RichTextBox Richbox, Form WinForm)
	{
		Rtxbox = Richbox;
		ParentForm = WinForm;
	}

	public static void WriteLogFileToUI(string info, Color color)
	{
		new Task(delegate
		{
			OpenLogThread(info, color);
		}).Start();
	}

	public static void writeLog(Exception errinfo, string unhandleinfo)
	{
		string text = DateTime.Now.ToString("yyyy-MM-dd");
		try
		{
			lock (writelogobj)
			{
				StreamWriter streamWriter = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + "\\Log\\" + text + ".txt", append: true, Encoding.UTF8);
				streamWriter.WriteLine(GetExceptionMsg(errinfo, unhandleinfo));
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}
		catch (Exception)
		{
			string path = Application.StartupPath + "\\Log";
			Directory.CreateDirectory(path);
		}
	}

	public static void writeLog(string text)
	{
		try
		{
			lock (writelogobj)
			{
				StreamWriter streamWriter = new StreamWriter(Application.StartupPath + "\\Log.txt", append: true, Encoding.UTF8);
				streamWriter.WriteLine(text);
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}
		catch (Exception)
		{
		}
	}

	private static void writetofile(string msg)
	{
		lock (syncFileObj)
		{
			string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
			string text = directoryName + "\\Log\\";
			if (!Directory.Exists(text))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				directoryInfo.Create();
			}
			string text2 = DateTime.Now.ToString("yyyy-MM-dd");
			string text3 = string.Format(text2 + ".log", Array.Empty<object>());
			StreamWriter streamWriter = File.AppendText(text + "\\" + text3);
			streamWriter.WriteLine(msg);
			streamWriter.Close();
			streamWriter.Dispose();
		}
	}

	public static void writehistory(string msg)
	{
		lock (syncFileObj)
		{
			string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
			string text = directoryName + "\\confighistory\\" + DateTime.Now.ToString("yyyy-MM-dd");
			if (!Directory.Exists(text))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				directoryInfo.Create();
			}
			string text2 = DateTime.Now.ToString("yyyy-MM-dd");
			string text3 = "confighistory.log";
			StreamWriter streamWriter = File.AppendText(text + "\\" + text3);
			streamWriter.WriteLine(msg);
			streamWriter.Close();
			streamWriter.Dispose();
		}
	}

	private static void readfromTemp1(out string sr)
	{
		lock (syncFileObj)
		{
			string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
			string text = directoryName + "\\Log\\";
			if (!Directory.Exists(text))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				directoryInfo.Create();
			}
			string text2 = string.Format("Temp1.log", Array.Empty<object>());
			string path = text + "\\" + text2;
			sr = File.ReadAllText(path);
		}
	}

	private static void writetoTemp1(string msg)
	{
		lock (syncFileObj)
		{
			string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
			string text = directoryName + "\\Log\\";
			if (!Directory.Exists(text))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				directoryInfo.Create();
			}
			string text2 = DateTime.Now.ToString("yyyy-MM-dd");
			string text3 = string.Format("Temp1.log", Array.Empty<object>());
			StreamWriter streamWriter = File.AppendText(text + "\\" + text3);
			streamWriter.WriteLine(msg);
			streamWriter.Close();
			streamWriter.Dispose();
		}
	}

	private static void OpenLogThread(string info, Color color)
	{
		try
		{
			string strtimeAll = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
			string strAppend = info + "\n";
			if (Rtxbox == null)
			{
				writetofile(strtimeAll + ":\n " + strAppend);
				return;
			}
			((Control)ParentForm).Invoke((Delegate)(Action)delegate
			{
				if (((TextBoxBase)Rtxbox).Lines.Length > L_Count)
				{
					string text = Path.GetDirectoryName(Application.ExecutablePath) + "\\Log";
					string text2 = DateTime.Now.ToString("yyyy-MM-dd");
					string text3 = string.Format(text2 + ".log", Array.Empty<object>());
					string path = text + "\\" + text3;
					((TextBoxBase)Rtxbox).Clear();
					if (File.Exists(path))
					{
						string sr = "";
						try
						{
							readfromfileSomeLine(50, out sr);
						}
						catch
						{
						}
						Rtxbox.AppendTextColorful(sr, Color.GreenYellow, addNewline: false);
						((TextBoxBase)Rtxbox).ScrollToCaret();
					}
				}
				((TextBoxBase)Rtxbox).AppendText(strtimeAll + ":\n ");
				Rtxbox.AppendTextColorful(strAppend, color, addNewline: false);
				((TextBoxBase)Rtxbox).ScrollToCaret();
				writetofile(strtimeAll + ":\n " + strAppend);
			});
		}
		catch (Exception)
		{
		}
	}

	private static void readfromfileSomeLine(int NeedNum, out string sr)
	{
		lock (syncFileObj)
		{
			sr = "";
			string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
			string text = directoryName + "\\Log\\";
			if (!Directory.Exists(text))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				directoryInfo.Create();
			}
			string text2 = DateTime.Now.ToString("yyyy-MM-dd");
			string text3 = string.Format(text2 + ".log", Array.Empty<object>());
			string path = text + "\\" + text3;
			string input = File.ReadAllText(path);
			string[] array = Regex.Split(input, "\n");
			if (array.Length - NeedNum >= 0)
			{
				for (int i = array.Length - NeedNum; i < array.Length; i++)
				{
					sr += array[i];
				}
			}
			else if (array.Length < L_Count && array.Length < 50)
			{
				for (int j = 0; j < array.Length; j++)
				{
					sr += array[j];
				}
			}
		}
	}

	public static string GetExceptionMsg(Exception ex, string backStr)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("****************************异常文本****************************");
		stringBuilder.AppendLine("【出现时间】：" + DateTime.Now);
		if (ex != null)
		{
			stringBuilder.AppendLine("【异常类型】：" + ex.GetType().Name);
			stringBuilder.AppendLine("【异常信息】：" + ex.Message);
			stringBuilder.AppendLine("【堆栈调用】：" + ex.StackTrace);
		}
		else
		{
			stringBuilder.AppendLine("【未处理异常】：" + backStr);
		}
		stringBuilder.AppendLine("***************************************************************");
		return stringBuilder.ToString();
	}

	public void Dispose()
	{
	}
}
