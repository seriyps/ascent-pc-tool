using CaddxTool.Protocol;

if (args.Length >= 1 && args[0] == "upgrade")
{
    return await RunUpgradeAsync(args);
}

Console.WriteLine("Scanning for Ascent devices (native Linux, no Wine)...");
var candidates = AscentDeviceFinder.FindCandidates();

if (candidates.Count == 0)
{
    Console.WriteLine("No candidate devices found. Is anything plugged in?");
    return 1;
}

foreach (var candidate in candidates)
{
    Console.WriteLine($"Found candidate: {candidate.PortName} (VID={candidate.Vid} PID={candidate.Pid})");
}

var target = candidates[0];
Console.WriteLine($"Connecting to {target.PortName}...");

using var client = AscentClient.OpenSerial(target.PortName);
var info = client.GetDeviceInfo();
Console.WriteLine("Device info: " + info);
return 0;

// Drives FirmwareUpgradeFlow against a fixed port path instead of real
// AscentDeviceFinder scanning for the post-reboot reconnects — real hardware
// re-enumerates on its own after rebooting, but a socat PTY pair never
// disappears/reappears, so there's nothing for udev-based scanning to find.
// Built for the fake-device wire-capture harness (CaddxTool.FakeDevice) — see
// src/PROJECT.md's "Wire-capture harness" section.
static async Task<int> RunUpgradeAsync(string[] args)
{
    if (args.Length < 4)
    {
        Console.Error.WriteLine("Usage: CaddxTool.Console upgrade <port> <firmware-file> <vid> [device-name]");
        Console.Error.WriteLine("  <device-name> defaults to \"ascent_lite_plus\" (controls the reboot-wait delay);");
        Console.Error.WriteLine("  see ArConstantsV2.RebootWaitByDeviceName for the full list of known names.");
        return 1;
    }

    string port = args[1];
    string firmwarePath = args[2];
    string vid = args[3];
    string deviceName = args.Length > 4 ? args[4] : "ascent_lite_plus";

    var transport = new SerialPortAscentTransport(port);
    var flow = new FirmwareUpgradeFlow(
        transport,
        vid: vid,
        deviceName: deviceName,
        transportFactory: _ => new SerialPortAscentTransport(port),
        scanCandidates: () => new[] { new CandidatePort(port, vid, "0000") });

    var progress = new Progress<FlowProgress>(p =>
        Console.WriteLine($"[{p.Percent:P0}] {p.StepName}: {p.Message}"));

    FlowResult result = await flow.RunAsync(firmwarePath, progress);
    Console.WriteLine(result.Success ? $"OK: {result.Message}" : $"FAILED: {result.Message}");
    return result.Success ? 0 : 1;
}
