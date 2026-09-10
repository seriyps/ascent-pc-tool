using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Caddx_PCTool;

public static class MavlinkUtil
{
	public static bool UseUnsafe { get; set; } = true;

	public static TMavlinkPacket ByteArrayToStructure<TMavlinkPacket>(this byte[] bytearray, int startoffset = 6) where TMavlinkPacket : struct
	{
		if (UseUnsafe)
		{
			return ReadUsingPointer<TMavlinkPacket>(bytearray, startoffset);
		}
		return ByteArrayToStructureGC<TMavlinkPacket>(bytearray, startoffset);
	}

	public static TMavlinkPacket ByteArrayToStructureBigEndian<TMavlinkPacket>(this byte[] bytearray, int startoffset = 6) where TMavlinkPacket : struct
	{
		object obj = new TMavlinkPacket();
		ByteArrayToStructureEndian(bytearray, ref obj, startoffset);
		return (TMavlinkPacket)obj;
	}

	public static void ByteArrayToStructure(byte[] bytearray, ref object obj, int startoffset, int payloadlength = 0)
	{
		if (bytearray == null || bytearray.Length < startoffset + payloadlength || payloadlength == 0)
		{
			return;
		}
		int num = Marshal.SizeOf(obj);
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = Marshal.AllocHGlobal(num);
			for (int i = 0; i < num / 8; i++)
			{
				Marshal.WriteInt64(intPtr, i * 8, 0L);
			}
			for (int j = num - num % 8; j < num; j++)
			{
				Marshal.WriteByte(intPtr, j, 0);
			}
			Marshal.Copy(bytearray, startoffset, intPtr, payloadlength);
			obj = Marshal.PtrToStructure(intPtr, obj.GetType());
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public static TMavlinkPacket ByteArrayToStructureT<TMavlinkPacket>(byte[] bytearray, int startoffset)
	{
		if (bytearray == null || bytearray.Length < startoffset)
		{
			return default(TMavlinkPacket);
		}
		int num = bytearray.Length - startoffset;
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			Marshal.Copy(bytearray, startoffset, intPtr, num);
		}
		catch (Exception ex)
		{
			Console.WriteLine("ByteArrayToStructure FAIL " + ex.Message);
		}
		object obj = Marshal.PtrToStructure(intPtr, typeof(TMavlinkPacket));
		Marshal.FreeHGlobal(intPtr);
		return (TMavlinkPacket)obj;
	}

	public static byte[] trim_payload(ref byte[] payload)
	{
		int num = payload.Length;
		while (num > 1 && payload[num - 1] == 0)
		{
			num--;
		}
		if (num != payload.Length)
		{
			Array.Resize(ref payload, num);
		}
		return payload;
	}

	public unsafe static T ReadUsingPointer<T>(byte[] data, int startoffset) where T : struct
	{
		if (data == null || data.Length < startoffset)
		{
			return default(T);
		}
		fixed (byte* value = &data[startoffset])
		{
			return (T)Marshal.PtrToStructure(new IntPtr(value), typeof(T));
		}
	}

	public static T ByteArrayToStructureGC<T>(byte[] bytearray, int startoffset) where T : struct
	{
		GCHandle gCHandle = GCHandle.Alloc(bytearray, GCHandleType.Pinned);
		try
		{
			return (T)Marshal.PtrToStructure(new IntPtr(gCHandle.AddrOfPinnedObject().ToInt64() + startoffset), typeof(T));
		}
		finally
		{
			gCHandle.Free();
		}
	}

	public static void ByteArrayToStructureEndian(byte[] bytearray, ref object obj, int startoffset)
	{
		int num = Marshal.SizeOf(obj);
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		byte[] array = (byte[])bytearray.Clone();
		obj = Marshal.PtrToStructure(intPtr, obj.GetType());
		object obj2 = obj;
		Type type = obj2.GetType();
		int num2 = startoffset;
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			object value = fieldInfo.GetValue(obj2);
			TypeCode typeCode = Type.GetTypeCode(value.GetType());
			if (typeCode != TypeCode.Object)
			{
				Array.Reverse((Array)array, num2, Marshal.SizeOf(value));
				num2 += Marshal.SizeOf(value);
			}
			else
			{
				int num3 = Marshal.SizeOf(((Array)value).GetValue(0));
				num2 += ((Array)value).Length * num3;
			}
		}
		try
		{
			Marshal.Copy(array, startoffset, intPtr, num);
		}
		catch (Exception ex)
		{
			Console.WriteLine("ByteArrayToStructure FAIL" + ex.ToString());
		}
		obj = Marshal.PtrToStructure(intPtr, obj.GetType());
		Marshal.FreeHGlobal(intPtr);
	}

	public static byte[] StructureToByteArray(object obj)
	{
		try
		{
			(from a in obj.GetType().GetFields()
				where a.FieldType.IsArray && a.FieldType.UnderlyingSystemType == typeof(byte[])
				select a).Where(delegate(FieldInfo a)
			{
				object[] customAttributes = a.GetCustomAttributes(typeof(MarshalAsAttribute), inherit: false);
				if (customAttributes.Length != 0)
				{
					MarshalAsAttribute marshalAsAttribute = (MarshalAsAttribute)customAttributes[0];
					int sizeConst = marshalAsAttribute.SizeConst;
					byte[] array2 = (byte[])a.GetValue(obj);
					if (array2 == null)
					{
						array2 = new byte[sizeConst];
					}
					else if (array2.Length != sizeConst)
					{
						Array.Resize(ref array2, sizeConst);
						a.SetValue(obj, array2);
					}
				}
				return false;
			}).ToList();
		}
		catch
		{
		}
		int num = Marshal.SizeOf(obj);
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(obj, intPtr, fDeleteOld: true);
		Marshal.Copy(intPtr, array, 0, num);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	public static byte[] StructureToByteArrayBigEndian(params object[] list)
	{
		object obj = list[0];
		Type type = obj.GetType();
		int num = 0;
		byte[] array = new byte[Marshal.SizeOf(obj)];
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			object value = fieldInfo.GetValue(obj);
			TypeCode typeCode = Type.GetTypeCode(value.GetType());
			switch (typeCode)
			{
			case TypeCode.Single:
			{
				byte[] bytes = BitConverter.GetBytes((float)value);
				Array.Reverse((Array)bytes);
				Array.Copy(bytes, 0, array, num, 4);
				break;
			}
			case TypeCode.Int32:
			{
				byte[] bytes = BitConverter.GetBytes((int)value);
				Array.Reverse((Array)bytes);
				Array.Copy(bytes, 0, array, num, 4);
				break;
			}
			case TypeCode.UInt32:
			{
				byte[] bytes = BitConverter.GetBytes((uint)value);
				Array.Reverse((Array)bytes);
				Array.Copy(bytes, 0, array, num, 4);
				break;
			}
			case TypeCode.Int16:
			{
				byte[] bytes = BitConverter.GetBytes((short)value);
				Array.Reverse((Array)bytes);
				Array.Copy(bytes, 0, array, num, 2);
				break;
			}
			case TypeCode.UInt16:
			{
				byte[] bytes = BitConverter.GetBytes((ushort)value);
				Array.Reverse((Array)bytes);
				Array.Copy(bytes, 0, array, num, 2);
				break;
			}
			case TypeCode.Int64:
			{
				byte[] bytes = BitConverter.GetBytes((long)value);
				Array.Reverse((Array)bytes);
				Array.Copy(bytes, 0, array, num, 8);
				break;
			}
			case TypeCode.UInt64:
			{
				byte[] bytes = BitConverter.GetBytes((ulong)value);
				Array.Reverse((Array)bytes);
				Array.Copy(bytes, 0, array, num, 8);
				break;
			}
			case TypeCode.Double:
			{
				byte[] bytes = BitConverter.GetBytes((double)value);
				Array.Reverse((Array)bytes);
				Array.Copy(bytes, 0, array, num, 8);
				break;
			}
			case TypeCode.Byte:
				array[num] = (byte)value;
				break;
			}
			if (typeCode == TypeCode.Object)
			{
				int num2 = ((byte[])value).Length;
				Array.Copy((byte[])value, 0, array, num, num2);
				num += num2;
			}
			else
			{
				num += Marshal.SizeOf(value);
			}
		}
		return array;
	}

	public static MAVLink.message_info GetMessageInfo(this MAVLink.message_info[] source, uint msgid)
	{
		for (int i = 0; i < source.Length; i++)
		{
			MAVLink.message_info result = source[i];
			if (result.msgid == msgid)
			{
				return result;
			}
		}
		Console.WriteLine("Unknown Packet " + msgid);
		return default(MAVLink.message_info);
	}
}
