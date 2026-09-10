using System;
using System.Drawing;

namespace Caddx_PCTool;

public class RingBuffer
{
	private readonly byte[] _buffer;

	private int _readIdx;

	private int _writeIdx;

	private int _count;

	private readonly object _lock = new object();

	public int Available
	{
		get
		{
			lock (_lock)
			{
				return _count;
			}
		}
	}

	public int Capacity => _buffer.Length;

	public RingBuffer(int capacity)
	{
		_buffer = new byte[capacity];
		_readIdx = 0;
		_writeIdx = 0;
		_count = 0;
	}

	public int Write(byte[] data, int offset, int count)
	{
		if (data == null)
		{
			WriteLog.WriteLogFileToUI("ringBuffer.Write data=null", Color.Red);
			return 0;
		}
		if (offset < 0 || count < 0 || offset + count > data.Length)
		{
			WriteLog.WriteLogFileToUI("ringBuffer.Write offset + count > data.Length", Color.Red);
			return 0;
		}
		if (count == 0)
		{
			return 0;
		}
		lock (_lock)
		{
			int num = Math.Min(count, _buffer.Length - _count);
			if (num <= 0)
			{
				return 0;
			}
			int num2 = Math.Min(num, _buffer.Length - _writeIdx);
			int num3 = num - num2;
			if (num2 > 0)
			{
				Buffer.BlockCopy(data, offset, _buffer, _writeIdx, num2);
			}
			if (num3 > 0)
			{
				Buffer.BlockCopy(data, offset + num2, _buffer, 0, num3);
			}
			_writeIdx = (_writeIdx + num) % _buffer.Length;
			_count += num;
			return num;
		}
	}

	public int Read(byte[] buffer, int count)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (count == 0)
		{
			return 0;
		}
		lock (_lock)
		{
			int num = Math.Min(count, _count);
			if (num <= 0)
			{
				return 0;
			}
			int num2 = Math.Min(num, _buffer.Length - _readIdx);
			int num3 = num - num2;
			if (num2 > 0)
			{
				Buffer.BlockCopy(_buffer, _readIdx, buffer, 0, num2);
			}
			if (num3 > 0)
			{
				Buffer.BlockCopy(_buffer, 0, buffer, num2, num3);
			}
			_readIdx = (_readIdx + num) % _buffer.Length;
			_count -= num;
			return num;
		}
	}

	public byte Peek(int index)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		lock (_lock)
		{
			if (index >= _count)
			{
				throw new InvalidOperationException("Peek index exceeds available data.");
			}
			return _buffer[(_readIdx + index) % _buffer.Length];
		}
	}

	public bool TryPeek(int index, out byte value)
	{
		lock (_lock)
		{
			if (index < 0 || index >= _count)
			{
				value = 0;
				return false;
			}
			value = _buffer[(_readIdx + index) % _buffer.Length];
			return true;
		}
	}

	public void Skip(int count)
	{
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		lock (_lock)
		{
			int num = Math.Min(count, _count);
			_readIdx = (_readIdx + num) % _buffer.Length;
			_count -= num;
		}
	}

	public int Peek(byte[] buffer, int count)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (count == 0)
		{
			return 0;
		}
		lock (_lock)
		{
			int num = Math.Min(count, _count);
			if (num <= 0)
			{
				return 0;
			}
			int num2 = Math.Min(num, _buffer.Length - _readIdx);
			int num3 = num - num2;
			if (num2 > 0)
			{
				Buffer.BlockCopy(_buffer, _readIdx, buffer, 0, num2);
			}
			if (num3 > 0)
			{
				Buffer.BlockCopy(_buffer, 0, buffer, num2, num3);
			}
			return num;
		}
	}

	public int PeekHeader(byte[] headerBuf, int headerLen)
	{
		lock (_lock)
		{
			if (_count < headerLen)
			{
				return -1;
			}
			int num = Math.Min(headerLen, _buffer.Length - _readIdx);
			int num2 = headerLen - num;
			if (num > 0)
			{
				Buffer.BlockCopy(_buffer, _readIdx, headerBuf, 0, num);
			}
			if (num2 > 0)
			{
				Buffer.BlockCopy(_buffer, 0, headerBuf, num, num2);
			}
			return _count;
		}
	}

	public int PeekAt(int offset, byte[] buffer, int count)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0 || count < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		lock (_lock)
		{
			if (offset + count > _count)
			{
				return -1;
			}
			int num = (_readIdx + offset) % _buffer.Length;
			int num2 = Math.Min(count, _buffer.Length - num);
			int num3 = count - num2;
			if (num2 > 0)
			{
				Buffer.BlockCopy(_buffer, num, buffer, 0, num2);
			}
			if (num3 > 0)
			{
				Buffer.BlockCopy(_buffer, 0, buffer, num2, num3);
			}
			return _count;
		}
	}

	public void Clear()
	{
		lock (_lock)
		{
			_readIdx = 0;
			_writeIdx = 0;
			_count = 0;
		}
	}

	public void ClearAllBuffer()
	{
		lock (_lock)
		{
			Array.Clear(_buffer, 0, _buffer.Length);
			_readIdx = 0;
			_writeIdx = 0;
			_count = 0;
		}
	}
}
