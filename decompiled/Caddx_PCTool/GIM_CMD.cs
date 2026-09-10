namespace Caddx_PCTool;

public enum GIM_CMD : byte
{
	StartGim = 0,
	StopGim = 1,
	CalibGyro = 2,
	SetRollGain = 3,
	SetPitchGain = 4,
	SetYawGain = 5,
	SetModeChann = 6,
	SetSensChann = 7,
	SetRollChann = 8,
	SetPitchChann = 9,
	SetYawChann = 10,
	ConnModeSel = 11,
	PushParam = 12,
	DownParam = 13,
	WriteParam = 14,
	SetGMMode = 21,
	SetSensNum = 22,
	SetRollNum = 23,
	SetPitchNum = 24,
	SetYawNum = 25,
	AngProtect = 31,
	ModeSlot = 33,
	PosCalib = 36,
	SetCamType = 37
}
