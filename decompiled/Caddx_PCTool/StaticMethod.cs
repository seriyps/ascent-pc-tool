using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Caddx_PCTool;

public static class StaticMethod
{
	private static uint[] _crcTable = new uint[256];

	public static readonly IntPtr HWND_TOP = new IntPtr(0);

	public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

	public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

	public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

	public const uint SWP_NOSIZE = 1u;

	public const uint SWP_NOMOVE = 2u;

	public const uint SWP_NOACTIVATE = 16u;

	public static string BytesToHexString(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(data.Length * 3);
		foreach (byte b in data)
		{
			stringBuilder.AppendFormat("{0:X2} ", b);
		}
		return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
	}

	public static byte[] HexStringToByteArray(string hex, string Splitter = " ")
	{
		hex = hex.Replace(Splitter, "");
		int num = hex.Length / 2;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
		}
		return array;
	}

	public static byte[] StructToBytes<T>(T obj) where T : struct
	{
		int num = Marshal.SizeOf(obj);
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			Marshal.StructureToPtr(obj, intPtr, fDeleteOld: false);
			Marshal.Copy(intPtr, array, 0, num);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return array;
	}

	public static T BytesToStruct<T>(byte[] bytes) where T : struct
	{
		GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		try
		{
			return Marshal.PtrToStructure<T>(gCHandle.AddrOfPinnedObject());
		}
		finally
		{
			gCHandle.Free();
		}
	}

	public static float ReadFloat(byte[] data, int offset)
	{
		byte[] array = new byte[4];
		Buffer.BlockCopy(data, offset, array, 0, 4);
		return BitConverter.ToSingle(array, 0);
	}

	public static ushort ReadUInt16(byte[] data, int offset)
	{
		return BitConverter.ToUInt16(data, offset);
	}

	public static uint ReadUInt32(byte[] data, int offset)
	{
		return BitConverter.ToUInt32(data, offset);
	}

	public static byte ReadByte(byte[] data, int offset)
	{
		return data[offset];
	}

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll")]
	public static extern bool BringWindowToTop(IntPtr hWnd);
}
