using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CaddxTool.Protocol;

// Event-based hotplug detection via libudev's netlink monitor (direct P/Invoke,
// no subprocess). udev_monitor_receive_device only returns once the kernel has
// actually broadcast an add/remove uevent for a tty device, so this is genuinely
// event-driven, not a polling loop — the only "poll" here is a short-timeout
// poll(2) on the monitor's own fd purely so Dispose()/cancellation stay
// responsive, not a re-scan-on-interval mechanism.
//
// Deliberately doesn't try to extract VID/PID from the udev_device itself
// (tty-subsystem devices don't reliably carry ID_VENDOR_ID/ID_MODEL_ID properties
// without walking to the parent USB device, which AscentDeviceFinder already does
// via sysfs). Changed just signals "something changed" — callers re-run
// AscentDeviceFinder.FindCandidates() for the authoritative current list.
public class AscentHotplugWatcher : IDisposable
{
    [StructLayout(LayoutKind.Sequential)]
    private struct PollFd
    {
        public int Fd;
        public short Events;
        public short REvents;
    }

    private const short POLLIN = 0x0001;
    private const string LibUdev = "libudev.so.1";
    private const string LibC = "libc.so.6";

    [DllImport(LibUdev)] private static extern IntPtr udev_new();
    [DllImport(LibUdev)] private static extern IntPtr udev_unref(IntPtr udev);

    [DllImport(LibUdev)]
    private static extern IntPtr udev_monitor_new_from_netlink(IntPtr udev,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    [DllImport(LibUdev)] private static extern IntPtr udev_monitor_unref(IntPtr monitor);
    [DllImport(LibUdev)]
    private static extern int udev_monitor_filter_add_match_subsystem_devtype(IntPtr monitor,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string subsystem, IntPtr devtype);
    [DllImport(LibUdev)] private static extern int udev_monitor_enable_receiving(IntPtr monitor);
    [DllImport(LibUdev)] private static extern int udev_monitor_get_fd(IntPtr monitor);
    [DllImport(LibUdev)] private static extern IntPtr udev_monitor_receive_device(IntPtr monitor);

    [DllImport(LibUdev)] private static extern IntPtr udev_device_unref(IntPtr device);
    [DllImport(LibUdev)] private static extern IntPtr udev_device_get_action(IntPtr device);

    [DllImport(LibC, SetLastError = true)]
    private static extern int poll(PollFd[] fds, nuint nfds, int timeoutMs);

    private readonly IntPtr _udev;
    private readonly IntPtr _monitor;
    private readonly int _fd;
    private readonly CancellationTokenSource _cts = new();
    private readonly Timer _debounceTimer;
    private readonly TimeSpan _debounce;
    private readonly Task _watchTask;

    // Fired (on a thread-pool thread, not necessarily the UI thread) after an
    // add/remove event settles for `debounce` — coalesces bursts like a single
    // device exposing several tty interfaces at once.
    public event Action? Changed;

    public AscentHotplugWatcher(TimeSpan? debounce = null)
    {
        _debounce = debounce ?? TimeSpan.FromMilliseconds(200);

        _udev = udev_new();
        if (_udev == IntPtr.Zero)
        {
            throw new InvalidOperationException("udev_new() failed");
        }

        _monitor = udev_monitor_new_from_netlink(_udev, "udev");
        if (_monitor == IntPtr.Zero)
        {
            udev_unref(_udev);
            throw new InvalidOperationException("udev_monitor_new_from_netlink() failed");
        }

        udev_monitor_filter_add_match_subsystem_devtype(_monitor, "tty", IntPtr.Zero);
        if (udev_monitor_enable_receiving(_monitor) < 0)
        {
            udev_monitor_unref(_monitor);
            udev_unref(_udev);
            throw new InvalidOperationException("udev_monitor_enable_receiving() failed");
        }

        _fd = udev_monitor_get_fd(_monitor);
        _debounceTimer = new Timer(_ => Changed?.Invoke(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _watchTask = Task.Run(() => WatchLoop(_cts.Token));
    }

    private void WatchLoop(CancellationToken ct)
    {
        var fds = new PollFd[1];
        fds[0].Fd = _fd;
        fds[0].Events = POLLIN;

        while (!ct.IsCancellationRequested)
        {
            fds[0].REvents = 0;
            int ready = poll(fds, 1, 500); // 500ms just bounds how long Dispose() can be blocked, not a re-scan cadence
            if (ct.IsCancellationRequested || ready <= 0)
            {
                continue;
            }

            IntPtr device = udev_monitor_receive_device(_monitor);
            if (device == IntPtr.Zero)
            {
                continue;
            }
            try
            {
                IntPtr actionPtr = udev_device_get_action(device); // owned by udev, do not free
                string? action = actionPtr == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(actionPtr);
                if (action is "add" or "remove")
                {
                    _debounceTimer.Change(_debounce, Timeout.InfiniteTimeSpan);
                }
            }
            finally
            {
                udev_device_unref(device);
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        try
        {
            _watchTask.Wait(TimeSpan.FromSeconds(2));
        }
        catch
        {
            // best-effort
        }
        _debounceTimer.Dispose();
        udev_monitor_unref(_monitor);
        udev_unref(_udev);
        _cts.Dispose();
    }
}
