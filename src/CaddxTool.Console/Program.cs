using CaddxTool.Protocol;

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
