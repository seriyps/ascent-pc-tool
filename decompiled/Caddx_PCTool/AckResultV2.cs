namespace Caddx_PCTool;

public class AckResultV2
{
	public AckStatusV2 Status { get; set; }

	public uint Cmd { get; set; }

	public uint Seq { get; set; }

	public uint RetryCount { get; set; }

	public byte[] RawPayload { get; set; }

	public string ErrorMessage { get; set; }

	public bool IsSuccess => Status == AckStatusV2.Matched;
}
