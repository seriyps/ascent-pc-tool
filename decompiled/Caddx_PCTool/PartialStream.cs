using System;
using System.IO;

namespace Caddx_PCTool;

public class PartialStream : Stream
{
	private readonly Stream _inner;

	private readonly long _length;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public override long Length => _length;

	public override long Position { get; set; }

	public PartialStream(Stream inner, long length)
	{
		_inner = inner;
		_length = length;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		long num = _length - Position;
		if (num <= 0)
		{
			return 0;
		}
		int count2 = (int)Math.Min(count, num);
		return _inner.Read(buffer, offset, count2);
	}

	public override void Flush()
	{
		_inner.Flush();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}
}
