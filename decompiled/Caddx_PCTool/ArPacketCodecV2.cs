using System;
using System.Runtime.InteropServices;

namespace Caddx_PCTool;

public static class ArPacketCodecV2
{
	private static readonly uint[] CrcTable = BuildCrcTable();

	public static byte[] BuildPacket(uint cmd, ushort type, ushort format, ushort userId, byte[] payload, uint len, uint seq, uint retry)
	{
		ArProtocolHeaderV2 obj = new ArProtocolHeaderV2
		{
			magic = 1095914575u,
			version = 3292,
			type = type,
			msgid = 0,
			unused = 43981,
			command = cmd,
			format = format,
			userid = userId,
			length = len,
			seq = seq,
			retry = retry,
			crc32 = 0u
		};
		byte[] headerBytes = StructToBytes(obj);
		obj.crc32 = CrcCalc(headerBytes, payload, (int)len);
		byte[] array = StructToBytes(obj);
		byte[] array2 = new byte[array.Length + len];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		if (payload != null && len != 0)
		{
			Buffer.BlockCopy(payload, 0, array2, array.Length, (int)len);
		}
		return array2;
	}

	public static ArProtocolHeaderV2 ParseHeader(byte[] headerBytes)
	{
		return BytesToStruct<ArProtocolHeaderV2>(headerBytes);
	}

	public static ResDeviceInfoV2 ParseDeviceInfo(byte[] frame)
	{
		ResDeviceInfoV2 resDeviceInfoV = new ResDeviceInfoV2();
		resDeviceInfoV.receiveMaxSize = BitConverter.ToInt32(frame, 0);
		int sourceIndex = 4;
		Array.Copy(frame, sourceIndex, resDeviceInfoV.sdkversion, 0, 32);
		int num = 36;
		Array.Copy(frame, num, resDeviceInfoV.devicename, 0, 64);
		int num2 = num + 64;
		resDeviceInfoV.cputemp = BitConverter.ToInt32(frame, num2);
		int num3 = num2 + 4;
		Array.Copy(frame, num3, resDeviceInfoV.firmwareInfo, 0, 64);
		int num4 = num3 + 64;
		Array.Copy(frame, num4, resDeviceInfoV.serialNumber, 0, 32);
		int num5 = num4 + 32;
		Array.Copy(frame, num5, resDeviceInfoV.hardwareVersion, 0, 32);
		int num6 = num5 + 32;
		resDeviceInfoV.status = BitConverter.ToInt32(frame, num6);
		int sourceIndex2 = num6 + 4;
		Array.Copy(frame, sourceIndex2, resDeviceInfoV.detail, 0, 64);
		return resDeviceInfoV;
	}

	public static ResAckInfoV2 ParseAckInfo(byte[] frame)
	{
		ResAckInfoV2 resAckInfoV = new ResAckInfoV2();
		resAckInfoV.Status = BitConverter.ToInt32(frame, 0);
		Array.Copy(frame, 4, resAckInfoV.Detail, 0, 64);
		return resAckInfoV;
	}

	public static ResUpgradeStatusV2 ParseUpgradeStatus(byte[] frame)
	{
		ResUpgradeStatusV2 resUpgradeStatusV = new ResUpgradeStatusV2();
		resUpgradeStatusV.Percent = BitConverter.ToInt32(frame, 0);
		resUpgradeStatusV.Status = BitConverter.ToInt32(frame, 4);
		int sourceIndex = 8;
		Array.Copy(frame, sourceIndex, resUpgradeStatusV.Detail, 0, 64);
		return resUpgradeStatusV;
	}

	public static ResFileDataInfoV2 ParseFileDataInfo(byte[] frame)
	{
		ResFileDataInfoV2 resFileDataInfoV = new ResFileDataInfoV2();
		resFileDataInfoV.Length = BitConverter.ToInt32(frame, 0);
		int startIndex = 4;
		resFileDataInfoV.Cursize = BitConverter.ToInt32(frame, startIndex);
		int num = 8;
		resFileDataInfoV.Totalsize = BitConverter.ToInt32(frame, num);
		int num2 = num + 4;
		resFileDataInfoV.Status = BitConverter.ToInt32(frame, num2);
		int sourceIndex = num2 + 4;
		Array.Copy(frame, sourceIndex, resFileDataInfoV.Detail, 0, 64);
		return resFileDataInfoV;
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

	public static uint CrcCalc(byte[] headerBytes, byte[] payload, int payloadLen = -1)
	{
		uint num = uint.MaxValue;
		if (headerBytes != null)
		{
			for (int i = 0; i < headerBytes.Length; i++)
			{
				num = CrcTable[(num ^ headerBytes[i]) & 0xFF] ^ (num >> 8);
			}
		}
		if (payload != null)
		{
			int num2 = ((payloadLen >= 0) ? Math.Min(payloadLen, payload.Length) : payload.Length);
			for (int j = 0; j < num2; j++)
			{
				num = CrcTable[(num ^ payload[j]) & 0xFF] ^ (num >> 8);
			}
		}
		return num ^ 0xFFFFFFFFu;
	}

	private static uint[] BuildCrcTable()
	{
		uint[] array = new uint[256];
		uint num = 3988292384u;
		for (uint num2 = 0u; num2 < 256; num2++)
		{
			uint num3 = num2;
			for (int i = 0; i < 8; i++)
			{
				num3 = (((num3 & 1) != 0) ? ((num3 >> 1) ^ num) : (num3 >> 1));
			}
			array[num2] = num3;
		}
		return array;
	}
}
