namespace Caddx_PCTool;

public enum CliSendAckFrameError
{
	None,
	HeaderLength,
	Magic,
	MessageType,
	Command,
	PayloadLength,
	CrcMismatch
}
