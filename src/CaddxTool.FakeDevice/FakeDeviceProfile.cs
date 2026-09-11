namespace CaddxTool.FakeDevice;

// Canned device identities for the FIND_DEVICE response. The DeviceName/
// FirmwareInfo/HardwareVersion/Vid values are real strings observed live
// from actual hardware (see the secret worktree's AGENTS.md device table) —
// only the serial number is a placeholder, deliberately not shaped like a
// real unit's.
public record FakeDeviceProfile(
    string Vid,
    string DeviceName,
    string FirmwareInfo,
    string HardwareVersion,
    string SerialNumber,
    int CpuTemp,
    int ReceiveMaxSize,
    string SdkVersion = "")
{
    public static FakeDeviceProfile Named(string name) => name.ToLowerInvariant() switch
    {
        "lite_plus" or "lite+" => new FakeDeviceProfile(
            Vid: "1d76",
            DeviceName: "Ascent_lite_plus",
            FirmwareInfo: "Ascent_H_Sky_18_21_10",
            HardwareVersion: "FPV-Ascent-Sky-472-V1.3-1.1",
            SerialNumber: "FAKE0000001",
            CpuTemp: 42,
            ReceiveMaxSize: 1_048_576),

        "vrx_pro" => new FakeDeviceProfile(
            Vid: "1d77",
            DeviceName: "Ascent_VRX_pro",
            FirmwareInfo: "Ascent_VRX_Pro_18_21_7",
            HardwareVersion: "Ascent-VRX-Pro-V3.0-1.0",
            SerialNumber: "FAKE0000002",
            CpuTemp: 38,
            ReceiveMaxSize: 1_048_576),

        "gt_pro" => new FakeDeviceProfile(
            Vid: "1d76",
            DeviceName: "Ascent_GT_pro",
            FirmwareInfo: "Ascent_G_Sky_17_5_15",
            HardwareVersion: "FPV-Ascent-Sky-486-V1.0-1.0",
            SerialNumber: "FAKE0000003",
            CpuTemp: 44,
            ReceiveMaxSize: 1_048_576),

        _ => throw new ArgumentException($"unknown device profile '{name}' (known: lite_plus, vrx_pro, gt_pro)"),
    };
}
