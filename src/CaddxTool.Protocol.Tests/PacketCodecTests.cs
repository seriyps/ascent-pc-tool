using System.Text;

namespace CaddxTool.Protocol.Tests;

public class PacketCodecTests
{
    [Fact]
    public void BuildPacket_ParseHeader_RoundTrips()
    {
        byte[] payload = Encoding.ASCII.GetBytes("hello");
        byte[] packet = ArPacketCodecV2.BuildPacket(
            cmd: 0x3C, type: ArConstantsV2.MSGTYPE_REQUEST, format: ArConstantsV2.FMT_BINARY,
            userId: 0, payload: payload, len: (uint)payload.Length, seq: 7, retry: 2);

        Assert.Equal(ArConstantsV2.HEADER_LENGTH + payload.Length, packet.Length);

        var header = ArPacketCodecV2.ParseHeader(packet);
        Assert.Equal(ArConstantsV2.MAGIC, header.magic);
        Assert.Equal(ArConstantsV2.VERSION, header.version);
        Assert.Equal(ArConstantsV2.MSGTYPE_REQUEST, header.type);
        Assert.Equal(ArConstantsV2.UNUSED, header.unused);
        Assert.Equal(0x3Cu, header.command);
        Assert.Equal(ArConstantsV2.FMT_BINARY, header.format);
        Assert.Equal((uint)payload.Length, header.length);
        Assert.Equal(7u, header.seq);
        Assert.Equal(2u, header.retry);

        byte[] actualPayload = packet[ArConstantsV2.HEADER_LENGTH..];
        Assert.Equal(payload, actualPayload);
    }

    [Fact]
    public void Crc_MatchesFreshCalculationOverHeaderAndPayload()
    {
        byte[] payload = Encoding.ASCII.GetBytes("payload-bytes");
        byte[] packet = ArPacketCodecV2.BuildPacket(
            cmd: 60, type: ArConstantsV2.MSGTYPE_ACK, format: ArConstantsV2.FMT_BINARY,
            userId: 0, payload: payload, len: (uint)payload.Length, seq: 1, retry: 0);

        var header = ArPacketCodecV2.ParseHeader(packet);
        byte[] headerBytes = packet[..ArConstantsV2.HEADER_LENGTH];

        // Recomputing over the same header+payload should reproduce the embedded crc32.
        uint recomputed = ArPacketCodecV2.CrcCalc(headerBytes, payload, payload.Length);
        Assert.Equal(header.crc32, recomputed);
    }

    [Fact]
    public void Crc_DetectsPayloadCorruption()
    {
        byte[] payload = Encoding.ASCII.GetBytes("payload-bytes");
        byte[] packet = ArPacketCodecV2.BuildPacket(
            cmd: 60, type: ArConstantsV2.MSGTYPE_ACK, format: ArConstantsV2.FMT_BINARY,
            userId: 0, payload: payload, len: (uint)payload.Length, seq: 1, retry: 0);
        var header = ArPacketCodecV2.ParseHeader(packet);
        byte[] headerBytes = packet[..ArConstantsV2.HEADER_LENGTH];

        byte[] corrupted = (byte[])payload.Clone();
        corrupted[0] ^= 0xFF;

        uint recomputed = ArPacketCodecV2.CrcCalc(headerBytes, corrupted, corrupted.Length);
        Assert.NotEqual(header.crc32, recomputed);
    }

    [Fact]
    public void ArFileInfoCodec_Build_LaysOutFieldsAtExpectedOffsets()
    {
        byte[] buf = ArFileInfoCodec.Build("abc123", length: 4096, remotePath: "/tmp/pc/fw.bin", localPath: "/home/user/fw.bin");

        Assert.Equal(ArFileInfoCodec.TOTAL_LENGTH, buf.Length);
        Assert.Equal(328, buf.Length);

        string md5Field = Encoding.ASCII.GetString(buf, 0, 64).TrimEnd('\0');
        Assert.Equal("abc123", md5Field);

        int length = BitConverter.ToInt32(buf, 64);
        Assert.Equal(4096, length);

        int saveAsFile = BitConverter.ToInt32(buf, 68);
        Assert.Equal(1, saveAsFile);

        string filePath = Encoding.ASCII.GetString(buf, 72, 128).TrimEnd('\0');
        Assert.Equal("/tmp/pc/fw.bin", filePath);

        string fileDir = Encoding.ASCII.GetString(buf, 200, 128).TrimEnd('\0');
        Assert.Equal("/home/user/fw.bin", fileDir);
    }

    [Fact]
    public void ArFileInfoCodec_Build_TruncatesLongLocalPathToLast128Chars()
    {
        string longPath = "/" + new string('a', 200) + "/fw.bin";
        byte[] buf = ArFileInfoCodec.Build("abc123", 1, "/tmp/pc/fw.bin", longPath);

        string fileDir = Encoding.ASCII.GetString(buf, 200, 128).TrimEnd('\0');
        Assert.Equal(longPath[^128..], fileDir);
    }
}
