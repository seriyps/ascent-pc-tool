namespace Caddx_PCTool;

public enum CliSendPayloadError
{
	None,
	Empty,
	InvalidHexCharacter,
	OddLength,
	TooShort,
	TooLong,
	InvalidMagic,
	UnsupportedType,
	CommandTypeMismatch,
	InvalidLength,
	InvalidEnable,
	NonZeroReserved
}
