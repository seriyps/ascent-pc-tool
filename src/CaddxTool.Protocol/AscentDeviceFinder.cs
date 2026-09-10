using System.IO.Ports;

namespace CaddxTool.Protocol;

// KnownName is AscentVids' lookup for Vid, when recognized — see AscentVids for
// why it's a hint (VIDConst's declared name), not a verified retail product name.
public record CandidatePort(string PortName, string Vid, string Pid, string? KnownName = null);

// Linux-native replacement for the original app's WMI-based Win32_PnPEntity lookup.
// Reads the real VID/PID straight out of sysfs instead of relying on a PnP database.
public static class AscentDeviceFinder
{
    public static System.Collections.Generic.List<CandidatePort> FindCandidates()
    {
        var result = new System.Collections.Generic.List<CandidatePort>();
        foreach (var portName in SerialPort.GetPortNames())
        {
            var ids = TryGetUsbIds(portName);
            if (ids is null) continue;
            var (vid, pid) = ids.Value;
            if (AscentVids.KnownVids.TryGetValue(vid, out string? knownName))
            {
                result.Add(new CandidatePort(portName, vid, pid, knownName));
            }
        }
        return result;
    }

    private static (string vid, string pid)? TryGetUsbIds(string portDevicePath)
    {
        // portDevicePath looks like "/dev/ttyACM0"; the matching sysfs class
        // entry is /sys/class/tty/ttyACM0/device, a symlink into the USB
        // interface's node. idVendor/idProduct live on the parent usb_device
        // node, typically two levels up.
        string className = System.IO.Path.GetFileName(portDevicePath);
        string classLink = $"/sys/class/tty/{className}/device";
        // File.ResolveLinkTarget mis-resolves this particular chain of relative
        // symlinks under /sys (observed: collapses to a bogus short path).
        // `readlink -f` handles it correctly, so shell out rather than fight
        // .NET's resolver.
        string? real = ResolveViaReadlink(classLink);
        if (real is null) return null;

        string? current = real;
        for (int i = 0; i < 6 && current != null; i++)
        {
            string vidFile = System.IO.Path.Combine(current, "idVendor");
            string pidFile = System.IO.Path.Combine(current, "idProduct");
            if (System.IO.File.Exists(vidFile) && System.IO.File.Exists(pidFile))
            {
                string vid = System.IO.File.ReadAllText(vidFile).Trim();
                string pid = System.IO.File.ReadAllText(pidFile).Trim();
                return (vid, pid);
            }
            current = System.IO.Path.GetDirectoryName(current);
        }
        return null;
    }

    private static string? ResolveViaReadlink(string path)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo("readlink")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            psi.ArgumentList.Add("-f");
            psi.ArgumentList.Add(path);
            using var proc = System.Diagnostics.Process.Start(psi);
            if (proc is null) return null;
            string output = proc.StandardOutput.ReadToEnd().Trim();
            proc.WaitForExit();
            return proc.ExitCode == 0 && output.Length > 0 ? output : null;
        }
        catch
        {
            return null;
        }
    }
}
