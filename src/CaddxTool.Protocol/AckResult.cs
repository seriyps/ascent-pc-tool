namespace CaddxTool.Protocol;

// Mirrors Caddx_PCTool.AckStatusV2, trimmed to what a synchronous client
// needs (no SendFailed/Cancelled — those surface as exceptions instead here).
public enum AckStatus
{
    Matched,
    Timeout,
}

// Mirrors Caddx_PCTool.AckResultV2.
public class AckResult
{
    public required AckStatus Status { get; init; }
    public uint Cmd { get; init; }
    public uint Seq { get; init; }
    public uint RetryCount { get; init; }
    public byte[]? RawPayload { get; init; }

    public bool IsSuccess => Status == AckStatus.Matched;
}
