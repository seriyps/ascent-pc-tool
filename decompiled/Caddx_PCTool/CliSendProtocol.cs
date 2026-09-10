using System;
using System.Collections.Generic;
using System.Text;

namespace Caddx_PCTool;

public static class CliSendProtocol
{
	public const int StructuredLength = 26;

	public const int PayloadLength = 28;

	public const int AckPayloadLength = 68;

	public const byte DataLength = 20;

	private const int MinimumInputLength = 7;

	public static bool TryBuildPayload(string input, CliSendCommandType selectedType, out byte[] payload, out CliSendPayloadError error)
	{
		payload = null;
		error = CliSendPayloadError.None;
		if (!TryParseHex(input, out var bytes, out error))
		{
			return false;
		}
		if (bytes.Length < 7)
		{
			error = CliSendPayloadError.TooShort;
			return false;
		}
		if (bytes.Length > 26)
		{
			error = CliSendPayloadError.TooLong;
			return false;
		}
		if (bytes[0] != 65 || bytes[1] != 83 || bytes[2] != 87 || bytes[3] != 0)
		{
			error = CliSendPayloadError.InvalidMagic;
			return false;
		}
		byte b = bytes[4];
		if (bytes[5] != 20)
		{
			error = CliSendPayloadError.InvalidLength;
			return false;
		}
		if (b == 2)
		{
			if (bytes[6] > 1)
			{
				error = CliSendPayloadError.InvalidEnable;
				return false;
			}
			for (int i = 7; i < bytes.Length; i++)
			{
				if (bytes[i] != 0)
				{
					error = CliSendPayloadError.NonZeroReserved;
					return false;
				}
			}
		}
		payload = new byte[28];
		Buffer.BlockCopy(bytes, 0, payload, 0, bytes.Length);
		return true;
	}

	public static bool TryParseAck(byte[] payload, out int status, out string detail)
	{
		status = -1;
		detail = string.Empty;
		if (payload == null || payload.Length < 68)
		{
			return false;
		}
		uint num = (uint)(payload[0] | (payload[1] << 8) | (payload[2] << 16) | (payload[3] << 24));
		status = (int)num;
		detail = Encoding.ASCII.GetString(payload, 4, 64).TrimEnd(new char[1]);
		return true;
	}

	public static bool TryValidateAckFrame(byte[] headerBytes, byte[] payload, out CliSendAckFrameError error)
	{
		error = CliSendAckFrameError.None;
		if (headerBytes == null || headerBytes.Length < 36)
		{
			error = CliSendAckFrameError.HeaderLength;
			return false;
		}
		if (ReadUInt32LittleEndian(headerBytes, 0) != 1095914575)
		{
			error = CliSendAckFrameError.Magic;
			return false;
		}
		if (ReadUInt16LittleEndian(headerBytes, 6) != 2)
		{
			error = CliSendAckFrameError.MessageType;
			return false;
		}
		if (ReadUInt32LittleEndian(headerBytes, 12) != 119)
		{
			error = CliSendAckFrameError.Command;
			return false;
		}
		uint num = ReadUInt32LittleEndian(headerBytes, 20);
		if (payload == null || payload.Length < 68 || num != payload.Length)
		{
			error = CliSendAckFrameError.PayloadLength;
			return false;
		}
		uint num2 = ReadUInt32LittleEndian(headerBytes, 32);
		byte[] array = new byte[36];
		Buffer.BlockCopy(headerBytes, 0, array, 0, array.Length);
		Array.Clear(array, 32, 4);
		if (CalculateCrc32(array, payload) != num2)
		{
			error = CliSendAckFrameError.CrcMismatch;
			return false;
		}
		return true;
	}

	private static ushort ReadUInt16LittleEndian(byte[] bytes, int offset)
	{
		return (ushort)(bytes[offset] | (bytes[offset + 1] << 8));
	}

	private static uint ReadUInt32LittleEndian(byte[] bytes, int offset)
	{
		return (uint)(bytes[offset] | (bytes[offset + 1] << 8) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 24));
	}

	private static uint CalculateCrc32(byte[] headerBytes, byte[] payload)
	{
		uint crc = uint.MaxValue;
		crc = UpdateCrc32(crc, headerBytes);
		crc = UpdateCrc32(crc, payload);
		return crc ^ 0xFFFFFFFFu;
	}

	private static uint UpdateCrc32(uint crc, byte[] bytes)
	{
		foreach (byte b in bytes)
		{
			crc ^= b;
			for (int j = 0; j < 8; j++)
			{
				crc = (((crc & 1) != 0) ? ((crc >> 1) ^ 0xEDB88320u) : (crc >> 1));
			}
		}
		return crc;
	}

	private static bool TryParseHex(string input, out byte[] bytes, out CliSendPayloadError error)
	{
		bytes = null;
		error = CliSendPayloadError.None;
		if (string.IsNullOrWhiteSpace(input))
		{
			error = CliSendPayloadError.Empty;
			return false;
		}
		List<char> list = new List<char>(input.Length);
		foreach (char c in input)
		{
			if (!char.IsWhiteSpace(c) && c != '-')
			{
				if (!Uri.IsHexDigit(c))
				{
					error = CliSendPayloadError.InvalidHexCharacter;
					return false;
				}
				list.Add(c);
			}
		}
		if (list.Count == 0)
		{
			error = CliSendPayloadError.Empty;
			return false;
		}
		if ((list.Count & 1) != 0)
		{
			error = CliSendPayloadError.OddLength;
			return false;
		}
		bytes = new byte[list.Count / 2];
		for (int j = 0; j < bytes.Length; j++)
		{
			bytes[j] = Convert.ToByte(new string(new char[2]
			{
				list[j * 2],
				list[j * 2 + 1]
			}), 16);
		}
		return true;
	}
}
