using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Caddx_PCTool;

public class MSPHelper
{
	public static object lockObj = new object();

	private static MSPHelper _inst;

	public static MSPHelper Inst
	{
		get
		{
			lock (lockObj)
			{
				if (_inst == null)
				{
					_inst = new MSPHelper();
				}
				return _inst;
			}
		}
	}

	public byte[] BuildMspV1RequestFrame(int payloadSize, byte cmd, byte[] payload = null)
	{
		byte[] array = new byte[6]
		{
			MSP_General.FrameHead,
			MSP_General.V1_Protocol,
			MSP_General.requestDire,
			(byte)payloadSize,
			cmd,
			0
		};
		CalculateXOR(array, out array[5]);
		return array;
	}

	public ApiVersionResult ParseMspApiVersionResponseV1(byte[] frame)
	{
		try
		{
			if (frame.Length < 9 && (frame[0] != 36 || frame[1] != 77 || frame[2] != 62))
			{
				throw new ArgumentException("Invalid frame header");
			}
			byte b = frame[3];
			if (b != 3 || frame.Length != 9)
			{
				throw new ArgumentException("Invalid payload length for MSP_API_VERSION");
			}
			byte b2 = frame[4];
			if (b2 != 1)
			{
				throw new ArgumentException("Invalid command code for MSP_API_VERSION");
			}
			byte b3 = 0;
			for (int i = 3; i < 8; i++)
			{
				b3 ^= frame[i];
			}
			if (frame[8] != b3)
			{
				throw new ArgumentException("Checksum mismatch");
			}
			return new ApiVersionResult
			{
				ProtocolVersion = (byte)(frame[5] + 1),
				MajorVersion = frame[6],
				MinorVersion = frame[7]
			};
		}
		catch (Exception)
		{
			return new ApiVersionResult();
		}
	}

	public int ParseMspV1ResponseFrame(byte[] frame, out string[] res)
	{
		int result = 0;
		res = new string[1];
		try
		{
			int num = frame.Length;
			if (frame.Length < 6)
			{
				return result = -1;
			}
			byte b = frame[num - 1];
			byte[] array = new byte[3];
			Array.Copy(frame, array, 3);
			string text = Encoding.ASCII.GetString(array);
			if (text != "$M>")
			{
				return result = 2;
			}
			int num2 = int.Parse(frame[3].ToString());
			byte[] array2 = new byte[num2];
			Array.Copy(frame, 5, array2, 0, num2);
			res = new string[num2 + 2];
			res[0] = frame[4].ToString();
			switch (frame[4])
			{
			case 1:
			{
				for (int m = 1; m < 1 + num2; m++)
				{
					res[m] = array2[m - 1].ToString();
				}
				break;
			}
			case 2:
				res[1] = Encoding.ASCII.GetString(array2);
				break;
			case 3:
			{
				for (int k = 1; k < 1 + num2; k++)
				{
					res[k] = array2[k - 1].ToString();
				}
				break;
			}
			case 4:
			{
				byte[] array6 = new byte[4];
				Array.Copy(frame, 5, array6, 0, 4);
				res[1] = Encoding.ASCII.GetString(array6);
				int num3 = int.Parse(array2[8].ToString());
				byte[] array7 = new byte[num3];
				Array.Copy(array2, 9, array7, 0, num3);
				res[2] = Encoding.ASCII.GetString(array7);
				break;
			}
			case 10:
				res[1] = Encoding.ASCII.GetString(array2);
				break;
			case 102:
			{
				byte[] array8 = new byte[2];
				byte[] array9 = new byte[2];
				byte[] array10 = new byte[2];
				byte[] array11 = new byte[2];
				byte[] array12 = new byte[2];
				byte[] array13 = new byte[2];
				byte[] array14 = new byte[2];
				byte[] array15 = new byte[2];
				byte[] array16 = new byte[2];
				for (int l = 0; l < 2; l++)
				{
					array8[l] = array2[l];
					array9[l] = array2[l + 2];
					array10[l] = array2[l + 4];
					array11[l] = array2[l + 6];
					array12[l] = array2[l + 8];
					array13[l] = array2[l + 10];
				}
				res[1] = BitConverter.ToInt16(array8, 0).ToString();
				res[2] = BitConverter.ToInt16(array9, 0).ToString();
				res[3] = BitConverter.ToInt16(array10, 0).ToString();
				res[4] = BitConverter.ToInt16(array11, 0).ToString();
				res[5] = BitConverter.ToInt16(array12, 0).ToString();
				res[6] = BitConverter.ToInt16(array13, 0).ToString();
				break;
			}
			case 108:
			{
				byte[] array3 = new byte[2];
				byte[] array4 = new byte[2];
				byte[] array5 = new byte[2];
				for (int j = 0; j < 2; j++)
				{
					array3[j] = array2[j];
					array4[j] = array2[j + 2];
					array5[j] = array2[j + 4];
				}
				res[1] = BitConverter.ToInt16(array3, 0).ToString();
				res[2] = BitConverter.ToInt16(array4, 0).ToString();
				res[3] = BitConverter.ToInt16(array5, 0).ToString();
				break;
			}
			case 79:
			case 105:
			case 110:
			case 130:
			case 151:
			{
				for (int i = 0; i < num2; i++)
				{
					res[i + 1] = array2[i].ToString();
				}
				break;
			}
			}
			return result = 1;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI(ex.Message, Color.Red);
			return result;
		}
	}

	public byte[] BuildMspFrame(int size, byte cmd, byte protocolVersion, byte[] data = null)
	{
		return protocolVersion switch
		{
			1 => BuildMspV1Frame(size, cmd, data), 
			2 => BuildMspV2Frame(size, cmd, data), 
			_ => null, 
		};
	}

	public byte[] BuildMspV1Frame(int size, byte cmd, byte[] payload = null)
	{
		List<byte> list = new List<byte>();
		try
		{
			list.Add(MSP_General.FrameHead);
			list.Add(MSP_General.V1_Protocol);
			list.Add(MSP_General.requestDire);
			list.Add((byte)size);
			list.Add(cmd);
			if (payload != null)
			{
				list.AddRange(payload);
			}
			byte item = MspChecksum.CalcXOR(list.ToArray());
			list.Add(item);
			return list.ToArray();
		}
		catch (Exception)
		{
			return list.ToArray();
		}
	}

	public byte[] BuildMspV2Frame(int size, byte cmd, byte[] payload = null)
	{
		List<byte> list = new List<byte>();
		try
		{
			list.Add(MSP_General.FrameHead);
			list.Add(MSP_General.V2_Protocol);
			list.Add(MSP_General.requestDire);
			list.Add(0);
			list.Add((byte)(cmd & 0xFF));
			list.Add((byte)((cmd >> 8) & 0xFF));
			list.Add((byte)(size & 0xFF));
			list.Add((byte)((size >> 8) & 0xFF));
			if (payload != null)
			{
				list.AddRange(payload);
			}
			byte item = MspChecksum.CalcCRC8(list.ToArray());
			list.Add(item);
			return list.ToArray();
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI(ex.Message, Color.Red);
			return list.ToArray();
		}
	}

	public bool CalculateXOR(byte[] buffer, out byte chesum)
	{
		chesum = 0;
		try
		{
			int num = 3;
			int num2 = int.Parse(buffer[3].ToString());
			if (num2 == 0)
			{
				num2++;
			}
			byte[] array = new byte[num2 + 1];
			Array.Copy(buffer, 3, array, 0, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				chesum ^= array[i];
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public bool CalculateCRC8(byte[] data, out byte crc_cs)
	{
		crc_cs = 0;
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
			crc_cs = b;
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private byte Calc_crc(byte crc, byte a)
	{
		crc ^= a;
		for (int i = 0; i < 8; i++)
		{
			crc = (((crc & 0x80) == 0) ? ((byte)(crc << 1)) : ((byte)((crc << 1) ^ 0xD5)));
		}
		return crc;
	}
}
