using System;

namespace Caddx_PCTool;

public class MspFrameParser
{
	private readonly byte[] buffer = new byte[1024];

	private int index = 0;

	public event Action<ushort, byte[]> FrameReceived;

	public void Process(byte[] data)
	{
		foreach (byte b in data)
		{
			buffer[index++] = b;
			if (index >= 3 && buffer[0] == 36)
			{
				if (buffer[1] == 77)
				{
					ParseV1();
				}
				else if (buffer[1] == 88)
				{
					ParseV2();
				}
			}
			if (index >= buffer.Length)
			{
				Array.Clear(buffer, 0, buffer.Length);
				index = 0;
			}
		}
	}

	private void ParseV1()
	{
		if (index < 5)
		{
			return;
		}
		int num = buffer[3];
		int num2 = 6 + num;
		if (index >= num2)
		{
			byte b = buffer[4];
			byte[] array = new byte[num];
			Array.Copy(buffer, 5, array, 0, num);
			byte[] array2 = new byte[num2];
			Array.Copy(buffer, 0, array2, 0, array2.Length);
			byte b2 = buffer[num2 - 1];
			byte b3 = MspChecksum.CalcXOR((byte)num, b, array);
			byte b4 = MspChecksum.CalcXOR(array2);
			if (b2 == b3)
			{
				FrameReceived?.Invoke(b, array);
			}
			int length = index - num2;
			Array.Copy(buffer, num2, buffer, 0, length);
			index = length;
		}
	}

	private void ParseV2()
	{
		if (index < 8)
		{
			return;
		}
		int num = buffer[6];
		int num2 = buffer[7];
		int num3 = num | (num2 << 8);
		int num4 = 8 + num3 + 1;
		if (index >= num4)
		{
			byte arg = buffer[4];
			byte b = buffer[5];
			ushort num5 = BitConverter.ToUInt16(buffer, 4);
			byte[] array = new byte[num3];
			Array.Copy(buffer, 8, array, 0, num3);
			byte[] array2 = new byte[9 + num3];
			Array.Copy(buffer, 0, array2, 0, array2.Length);
			byte b2 = buffer[num4 - 1];
			byte b3 = MspChecksum.CalcCRC8(array2);
			if (b2 == b3)
			{
				FrameReceived?.Invoke(arg, array);
			}
			int length = index - num4;
			Array.Copy(buffer, num4, buffer, 0, length);
			index = length;
		}
	}
}
