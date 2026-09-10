using System.Text;

namespace Caddx_PCTool;

public static class CRC32_Gimbal
{
	private const uint Polynomial = 3988292384u;

	private static readonly uint[] Table;

	private static bool isInit;

	static CRC32_Gimbal()
	{
		Table = new uint[256];
		isInit = false;
		for (uint num = 0u; num < 256; num++)
		{
			uint num2 = num;
			for (int i = 0; i < 8; i++)
			{
				num2 = (((num2 & 1) != 1) ? (num2 >> 1) : ((num2 >> 1) ^ 0xEDB88320u));
			}
			Table[num] = num2;
		}
	}

	private static void InitTable()
	{
		if (isInit)
		{
			return;
		}
		for (uint num = 0u; num < 256; num++)
		{
			uint num2 = num;
			for (int i = 0; i < 8; i++)
			{
				num2 = (((num2 & 1) != 1) ? (num2 >> 1) : ((num2 >> 1) ^ 0xEDB88320u));
			}
			Table[num] = num2;
		}
		isInit = true;
	}

	public static uint Calculate(byte[] bytes, uint crc = uint.MaxValue)
	{
		InitTable();
		foreach (byte b in bytes)
		{
			byte b2 = (byte)(crc ^ b);
			crc = (crc >> 8) ^ Table[b2];
		}
		return crc ^ 0xFFFFFFFFu;
	}

	public static uint Calculate(string input, uint crc = uint.MaxValue)
	{
		if (string.IsNullOrEmpty(input))
		{
			return Calculate(new byte[0], crc);
		}
		byte[] bytes = Encoding.Default.GetBytes(input);
		return Calculate(bytes, crc);
	}
}
