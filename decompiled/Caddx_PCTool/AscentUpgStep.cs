namespace Caddx_PCTool;

public enum AscentUpgStep : byte
{
	None,
	FindDevice_Normal,
	Reboot_Clean,
	RemoteUpgrade,
	SendFileStart,
	SendFileData,
	SendFileEnd,
	UpgradeStatus,
	Reboot,
	FindDevice_Clean
}
