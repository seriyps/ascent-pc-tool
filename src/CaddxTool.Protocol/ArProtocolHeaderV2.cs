using System.Runtime.InteropServices;

namespace CaddxTool.Protocol;

// Ported near-verbatim from the decompiled Caddx PC Tool (Caddx_PCTool.ArProtocolHeaderV2).
// 36-byte fixed header, little-endian, Pack=1.
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ArProtocolHeaderV2
{
    public uint magic;
    public ushort version;
    public ushort type;
    public ushort msgid;
    public ushort unused;
    public uint command;
    public ushort format;
    public ushort userid;
    public uint length;
    public uint seq;
    public uint retry;
    public uint crc32;
}
