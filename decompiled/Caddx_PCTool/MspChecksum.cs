using System;

namespace Caddx_PCTool;

public static class MspChecksum
{
	public static byte CalcXOR(byte length, byte command, byte[] data)
	{
		byte b = (byte)(length ^ command);
		foreach (byte b2 in data)
		{
			b ^= b2;
		}
		return b;
	}

	public static byte CalcCRC8(byte[] data)
	{
		try
		{
			byte b = 0;
			int num = (data[6] | (data[7] << 8)) + 5;
			byte[] array = new byte[num];
			Array.Copy(data, 3, array, 0, num);
			byte[] array2 = array;
			foreach (byte a in array2)
			{
				b = Calc_crc(b, a);
			}
			return b;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	private static byte Calc_crc(byte crc, byte a)
	{
		crc ^= a;
		for (int i = 0; i < 8; i++)
		{
			crc = (((crc & 0x80) == 0) ? ((byte)(crc << 1)) : ((byte)((crc << 1) ^ 0xD5)));
		}
		return crc;
	}

	public static byte CalcXOR(byte[] buffer)
	{
		byte b = 0;
		try
		{
			int sourceIndex = 3;
			int num = int.Parse(buffer[3].ToString()) + 2;
			byte[] array = new byte[num];
			Array.Copy(buffer, sourceIndex, array, 0, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				b ^= array[i];
			}
			return b;
		}
		catch
		{
			return 0;
		}
	}
}
