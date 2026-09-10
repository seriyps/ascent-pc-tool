using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class GoggleHelper
{
	private static object lockObj = new object();

	private static GoggleHelper _inst;

	private const uint BUFFER_SIZE = 1048576u;

	private int g_download_fileLength = 0;

	private byte[] g_download_fileContent = new byte[1048576];

	public static GoggleHelper Inst
	{
		get
		{
			lock (lockObj)
			{
				if (_inst == null)
				{
					_inst = new GoggleHelper();
				}
				return _inst;
			}
		}
	}

	public List<byte> BuildGoggleFrame(byte cmd, byte[] buffer = null)
	{
		List<byte> list = null;
		try
		{
			list = new List<byte>();
			List<byte> list2 = list;
			byte[] obj = new byte[5] { 255, 85, 18, 52, 0 };
			obj[4] = cmd;
			list2.AddRange(obj);
			if (buffer != null)
			{
				list.AddRange(new byte[2]
				{
					(byte)((buffer.Length >> 8) & 0xFF),
					(byte)(buffer.Length & 0xFF)
				});
				list.AddRange(buffer);
				list.Add(CheckSum(buffer));
			}
			else
			{
				list.AddRange(new byte[3]);
			}
			return list;
		}
		catch (Exception)
		{
			return list;
		}
	}

	public byte CheckSum(byte[] data)
	{
		byte b = 0;
		try
		{
			if (data == null)
			{
				return b;
			}
			for (int i = 0; i < data.Length; i++)
			{
				b += data[i];
			}
			return b;
		}
		catch (Exception)
		{
			return b;
		}
	}

	public bool ReadBinFile(string filePath, out byte[] fileContent)
	{
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		fileContent = new byte[1048576];
		try
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				WriteLog.WriteLogFileToUI("固件路径为空", Color.DarkRed);
				return false;
			}
			if (!File.Exists(filePath))
			{
				WriteLog.WriteLogFileToUI("文件不存在", Color.DarkRed);
				return false;
			}
			FileInfo fileInfo = new FileInfo(filePath);
			long length = fileInfo.Length;
			if (length <= 0)
			{
				WriteLog.WriteLogFileToUI("文件为空或无法读取", Color.DarkRed);
				return false;
			}
			if (length > 1048576)
			{
				WriteLog.WriteLogFileToUI($"文件过大：{length} 字节，超过 1MB 限制！", Color.DarkRed);
				return false;
			}
			for (int i = 0; (long)i < 1048576L; i++)
			{
				fileContent[i] = byte.MaxValue;
			}
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				int num = fileStream.Read(g_download_fileContent, 0, (int)length);
				if (num != length)
				{
					MessageBox.Show("读取文件失败或中断！", "Error");
					return false;
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
