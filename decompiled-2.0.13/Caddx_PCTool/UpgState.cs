namespace Caddx_PCTool;

public enum UpgState : byte
{
	Idle,
	FindDevice_Normal,
	Reboot_Clean,
	WaitCleanOnline,
	RemoteUpgrade,
	SendFileStart,
	SendFileData,
	SendFileEnd,
	UpgradeStatus,
	Completed,
	Failed,
	SendFileStart_Json,
	SendFileData_Json,
	SendFileEnd_Json,
	Completed_Json
}
