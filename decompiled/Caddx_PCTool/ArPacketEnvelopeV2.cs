namespace Caddx_PCTool;

public class ArPacketEnvelopeV2
{
	public uint Cmd { get; set; }

	public uint Seq { get; set; }

	public byte[] RawPayload { get; set; }
}
