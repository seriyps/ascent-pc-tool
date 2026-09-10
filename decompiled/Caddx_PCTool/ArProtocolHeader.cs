using System.Runtime.InteropServices;

namespace Caddx_PCTool;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ArProtocolHeader
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
