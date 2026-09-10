namespace Caddx_PCTool;

public enum SerialPortReleaseReason
{
	OwnerRequested,
	CommandCompleted,
	DiscoveryCompleted,
	DeviceRemoved,
	UpgradeCompleted,
	UpgradeFailed,
	Cancelled,
	TransportUnavailable,
	ApplicationStopping
}
