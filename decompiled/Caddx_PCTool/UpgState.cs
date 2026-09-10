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
	Completed_Json,
	GetBBFreq,
	SetBBFreq,
	SendFileStart_BBFreq,
	SendFileData_BBFreq,
	SendFileEnd_BBFreq,
	Completed_SetBBFreq,
	FactoryReset_BBFreq
}
