using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Caddx_PCTool;

public class FileTransferSession : IDisposable
{
	private readonly FileStream _stream;

	private readonly byte[] _buffer;

	private long _fileLength;

	private long _sent;

	private int _chunkSize;

	private int _lastChunkLength;

	private byte[] _md5;

	public string LocalPath { get; }

	public string RemotePath { get; }

	public bool IsComplete => _sent >= _fileLength;

	public float Progress => (_fileLength > 0) ? ((float)_sent / (float)_fileLength) : 0f;

	public long BytesSent => _sent;

	public long TotalBytes => _fileLength;

	public FileTransferSession(string localPath, string remotePath, FileStream stream, int chunkSize)
	{
		LocalPath = localPath ?? throw new ArgumentNullException("localPath");
		RemotePath = remotePath ?? throw new ArgumentNullException("remotePath");
		_stream = stream ?? throw new ArgumentNullException("stream");
		_buffer = null;
		_fileLength = stream.Length;
		_chunkSize = ((chunkSize > 0) ? chunkSize : 1048576);
		_sent = 0L;
		_md5 = ComputeMD5(localPath);
	}

	public FileTransferSession(string localPath, string remotePath)
	{
		LocalPath = localPath ?? throw new ArgumentNullException("localPath");
		RemotePath = remotePath ?? throw new ArgumentNullException("remotePath");
		_buffer = File.ReadAllBytes(localPath);
		_stream = null;
		_fileLength = _buffer.Length;
		_chunkSize = (int)_fileLength;
		_sent = 0L;
		_md5 = ComputeMD5(localPath);
	}

	public ArFileInfo BuildFileInfo()
	{
		ArFileInfo result = new ArFileInfo
		{
			MD5 = new byte[64],
			filePath = new byte[128],
			fileDir = new byte[128],
			length = (int)_fileLength,
			saveAsFile = 1
		};
		byte[] md = _md5;
		Array.Copy(md, result.MD5, Math.Min(md.Length, 64));
		byte[] bytes = Encoding.ASCII.GetBytes(RemotePath);
		Array.Copy(bytes, result.filePath, bytes.Length);
		string text = LocalPath.Replace('\\', '/');
		if (text.Length > 128)
		{
			text = text.Remove(0, text.Length - 128);
			int length = text.Length;
		}
		byte[] bytes2 = Encoding.ASCII.GetBytes(text);
		Array.Copy(bytes2, result.fileDir, bytes2.Length);
		return result;
	}

	public byte[] GetNextChunk(out int length)
	{
		if (_sent >= _fileLength)
		{
			length = 0;
			return null;
		}
		long val = _fileLength - _sent;
		length = (int)Math.Min(_chunkSize, val);
		byte[] array;
		if (_stream != null)
		{
			array = new byte[length];
			int num = _stream.Read(array, 0, length);
			if (num < length)
			{
				length = num;
			}
		}
		else
		{
			array = new byte[length];
			Array.Copy(_buffer, _sent, array, 0L, length);
		}
		_lastChunkLength = length;
		return array;
	}

	public void Advance()
	{
		_sent += _lastChunkLength;
	}

	public void Dispose()
	{
		try
		{
			_stream?.Dispose();
		}
		catch
		{
		}
	}

	private static byte[] ComputeMD5(string path)
	{
		using FileStream inputStream = File.OpenRead(path);
		MD5 mD = MD5.Create();
		byte[] array = mD.ComputeHash(inputStream);
		string s = BitConverter.ToString(array).Replace("-", "").ToLower();
		return Encoding.ASCII.GetBytes(s);
	}
}
