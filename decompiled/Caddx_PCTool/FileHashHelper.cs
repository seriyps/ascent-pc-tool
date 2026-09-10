using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Caddx_PCTool;

public class FileHashHelper
{
	public static string ReadLast32Bytes(string filePath)
	{
		using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		if (fileStream.Length < 64)
		{
			throw new InvalidOperationException("文件大小不足 32 字节");
		}
		byte[] array = new byte[64];
		fileStream.Seek(-64L, SeekOrigin.End);
		fileStream.Read(array, 0, 64);
		return Encoding.UTF8.GetString(array);
	}

	public static string ComputeMd5WithoutLast32Bytes(string filePath)
	{
		using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		if (fileStream.Length < 32)
		{
			throw new InvalidOperationException("文件大小不足 32 字节");
		}
		using MD5 mD = MD5.Create();
		byte[] array = mD.ComputeHash(new PartialStream(fileStream, fileStream.Length - 32));
		return BitConverter.ToString(array).Replace("-", "").ToLower();
	}
}
