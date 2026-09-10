using System;

namespace CaddxTool.Protocol;

// Ported from Caddx_PCTool.ArPacketCodecV2. The original uses Marshal.StructureToPtr
// over a StructLayout struct; here the same 36-byte layout is packed/unpacked by hand
// via BitConverter, which avoids relying on marshaling behavior being identical across
// runtimes and is easier to verify byte-for-byte against the known wire format.
public static class ArPacketCodecV2
{
    private static readonly uint[] CrcTable = BuildCrcTable();

    public static byte[] BuildPacket(uint cmd, ushort type, ushort format, ushort userId, byte[]? payload, uint len, uint seq, uint retry)
    {
        byte[] header = new byte[ArConstantsV2.HEADER_LENGTH];
        WriteHeader(header, magic: ArConstantsV2.MAGIC, version: ArConstantsV2.VERSION, type: type,
            msgid: 0, unused: ArConstantsV2.UNUSED, command: cmd, format: format, userid: userId,
            length: len, seq: seq, retry: retry, crc32: 0);

        uint crc = CrcCalc(header, payload, (int)len);
        // crc32 is the last 4 bytes of the header.
        BitConverter.GetBytes(crc).CopyTo(header, ArConstantsV2.HEADER_LENGTH - 4);

        byte[] packet = new byte[header.Length + len];
        Buffer.BlockCopy(header, 0, packet, 0, header.Length);
        if (payload != null && len != 0)
        {
            Buffer.BlockCopy(payload, 0, packet, header.Length, (int)len);
        }
        return packet;
    }

    private static void WriteHeader(byte[] buf, uint magic, ushort version, ushort type, ushort msgid,
        ushort unused, uint command, ushort format, ushort userid, uint length, uint seq, uint retry, uint crc32)
    {
        int o = 0;
        BitConverter.GetBytes(magic).CopyTo(buf, o); o += 4;
        BitConverter.GetBytes(version).CopyTo(buf, o); o += 2;
        BitConverter.GetBytes(type).CopyTo(buf, o); o += 2;
        BitConverter.GetBytes(msgid).CopyTo(buf, o); o += 2;
        BitConverter.GetBytes(unused).CopyTo(buf, o); o += 2;
        BitConverter.GetBytes(command).CopyTo(buf, o); o += 4;
        BitConverter.GetBytes(format).CopyTo(buf, o); o += 2;
        BitConverter.GetBytes(userid).CopyTo(buf, o); o += 2;
        BitConverter.GetBytes(length).CopyTo(buf, o); o += 4;
        BitConverter.GetBytes(seq).CopyTo(buf, o); o += 4;
        BitConverter.GetBytes(retry).CopyTo(buf, o); o += 4;
        BitConverter.GetBytes(crc32).CopyTo(buf, o); o += 4;
        if (o != ArConstantsV2.HEADER_LENGTH)
        {
            throw new InvalidOperationException($"header packing produced {o} bytes, expected {ArConstantsV2.HEADER_LENGTH}");
        }
    }

    public static ArProtocolHeaderV2 ParseHeader(byte[] headerBytes)
    {
        int o = 0;
        var h = new ArProtocolHeaderV2
        {
            magic = BitConverter.ToUInt32(headerBytes, o),
        };
        o += 4;
        h.version = BitConverter.ToUInt16(headerBytes, o); o += 2;
        h.type = BitConverter.ToUInt16(headerBytes, o); o += 2;
        h.msgid = BitConverter.ToUInt16(headerBytes, o); o += 2;
        h.unused = BitConverter.ToUInt16(headerBytes, o); o += 2;
        h.command = BitConverter.ToUInt32(headerBytes, o); o += 4;
        h.format = BitConverter.ToUInt16(headerBytes, o); o += 2;
        h.userid = BitConverter.ToUInt16(headerBytes, o); o += 2;
        h.length = BitConverter.ToUInt32(headerBytes, o); o += 4;
        h.seq = BitConverter.ToUInt32(headerBytes, o); o += 4;
        h.retry = BitConverter.ToUInt32(headerBytes, o); o += 4;
        h.crc32 = BitConverter.ToUInt32(headerBytes, o); o += 4;
        return h;
    }

    public static uint CrcCalc(byte[]? headerBytes, byte[]? payload, int payloadLen = -1)
    {
        // Matches the original: CRC is computed over the header with crc32
        // zeroed, then the payload. Standard CRC-32 (IEEE/zlib) bit-reversed
        // table-driven implementation.
        uint crc = uint.MaxValue;
        if (headerBytes != null)
        {
            byte[] headerForCrc = headerBytes;
            if (headerBytes.Length >= ArConstantsV2.HEADER_LENGTH)
            {
                bool crcFieldZero = true;
                for (int i = ArConstantsV2.HEADER_LENGTH - 4; i < ArConstantsV2.HEADER_LENGTH; i++)
                {
                    if (headerBytes[i] != 0) { crcFieldZero = false; break; }
                }
                if (!crcFieldZero)
                {
                    headerForCrc = (byte[])headerBytes.Clone();
                    Array.Clear(headerForCrc, ArConstantsV2.HEADER_LENGTH - 4, 4);
                }
            }
            for (int i = 0; i < headerForCrc.Length; i++)
            {
                crc = CrcTable[(crc ^ headerForCrc[i]) & 0xFF] ^ (crc >> 8);
            }
        }
        if (payload != null)
        {
            int n = payloadLen >= 0 ? Math.Min(payloadLen, payload.Length) : payload.Length;
            for (int j = 0; j < n; j++)
            {
                crc = CrcTable[(crc ^ payload[j]) & 0xFF] ^ (crc >> 8);
            }
        }
        return crc ^ 0xFFFFFFFFu;
    }

    private static uint[] BuildCrcTable()
    {
        uint[] table = new uint[256];
        const uint poly = 0xEDB88320u;
        for (uint i = 0; i < 256; i++)
        {
            uint c = i;
            for (int k = 0; k < 8; k++)
            {
                c = (c & 1) != 0 ? (c >> 1) ^ poly : c >> 1;
            }
            table[i] = c;
        }
        return table;
    }
}
