using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CaddxTool.Protocol;

namespace CaddxTool.Avalonia;

public partial class MainWindow : Window
{
    private sealed record PortListItem(CandidatePort Candidate)
    {
        public override string ToString() => Candidate.KnownName is { } name
            ? $"{Candidate.PortName}  {name}  (VID={Candidate.Vid} PID={Candidate.Pid})"
            : $"{Candidate.PortName}  (VID={Candidate.Vid} PID={Candidate.Pid})";
    }

    private readonly AscentHotplugWatcher? _hotplugWatcher;
    private IAscentTransport? _connectedTransport;
    private CandidatePort? _connectedCandidate;
    private ResDeviceInfoV2? _deviceInfo;
    private string? _firmwarePath;
    private bool _busy;

    public MainWindow()
    {
        InitializeComponent();

        // Best-effort: if libudev isn't available or behaves unexpectedly for any
        // reason, degrade gracefully to the manual Scan button rather than crashing —
        // that already covers device detection on its own, just not live.
        try
        {
            _hotplugWatcher = new AscentHotplugWatcher();
            _hotplugWatcher.Changed += OnHotplugChanged;
            AppendLog("Live hotplug detection enabled (udev) — plug/unplug updates the list automatically.");
        }
        catch (Exception ex)
        {
            AppendLog($"Live hotplug detection unavailable ({ex.Message}) — use Scan manually after plugging in.");
        }

        Closed += (_, _) =>
        {
            _hotplugWatcher?.Dispose();
            _connectedTransport?.Dispose();
        };
    }

    private void OnHotplugChanged()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (!_busy)
            {
                _ = RunScanAsync();
            }
        });
    }

    private async void OnScanClick(object? sender, RoutedEventArgs e) => await RunScanAsync();

    private async Task RunScanAsync()
    {
        if (_busy)
        {
            return;
        }

        _busy = true;
        BtnScan.IsEnabled = false;
        AppendLog("Scanning for Ascent devices...");
        try
        {
            List<CandidatePort> candidates = await Task.Run(AscentDeviceFinder.FindCandidates);
            LstPorts.ItemsSource = candidates.Select(c => new PortListItem(c)).ToList();
            AppendLog(candidates.Count == 0
                ? "No candidate devices found."
                : $"Found {candidates.Count} candidate port(s).");
        }
        catch (Exception ex)
        {
            AppendLog($"Scan failed: {ex.Message}");
        }
        finally
        {
            BtnScan.IsEnabled = true;
            _busy = false;
        }
    }

    private void OnPortSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        BtnConnect.IsEnabled = LstPorts.SelectedItem is PortListItem;
    }

    private async void OnConnectClick(object? sender, RoutedEventArgs e)
    {
        if (_busy || LstPorts.SelectedItem is not PortListItem item)
        {
            return;
        }

        _busy = true;
        BtnConnect.IsEnabled = false;
        AppendLog($"Connecting to {item.Candidate.PortName}...");
        try
        {
            (IAscentTransport transport, ResDeviceInfoV2 info) = await Task.Run(() =>
            {
                var t = new SerialPortAscentTransport(item.Candidate.PortName);
                var c = new AscentClient(t);
                return ((IAscentTransport)t, c.GetDeviceInfo());
            });

            _connectedTransport?.Dispose();
            _connectedTransport = transport;
            _connectedCandidate = item.Candidate;
            _deviceInfo = info;

            // A previous session's success/failure color shouldn't carry over
            // to a different (re)connection.
            TxtStatus.ClearValue(TextBlock.ForegroundProperty);
            ProgressUpgrade.ClearValue(ProgressBar.ForegroundProperty);

            TxtDeviceInfo.Text = info.ToString();
            TxtFirmwareHint.Text = $"Expect a firmware filename containing \"{FirmwareNameValidator.ExpectedPrefix(Ascii(info.FirmwareInfo))}\" for this device.";
            BtnChooseFile.IsEnabled = true;
            AppendLog("Connected.");
        }
        catch (Exception ex)
        {
            AppendLog($"Connect failed: {ex.Message}");
        }
        finally
        {
            BtnConnect.IsEnabled = true;
            _busy = false;
        }
    }

    private async void OnChooseFileClick(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select firmware .img",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("Firmware image") { Patterns = new[] { "*.img" } } },
        });

        if (files.Count == 0)
        {
            return;
        }

        _firmwarePath = files[0].Path.LocalPath;
        TxtFilePath.Text = _firmwarePath;

        if (_deviceInfo is not null)
        {
            string firmwareInfo = Ascii(_deviceInfo.FirmwareInfo);
            string fileName = Path.GetFileName(_firmwarePath);
            if (FirmwareNameValidator.LooksMismatched(fileName, firmwareInfo))
            {
                bool proceed = await ConfirmDialog.Ask(this,
                    $"\"{fileName}\" doesn't look like it matches this device's firmware (\"{firmwareInfo}\").\n\n" +
                    "Flashing the wrong image can brick the device. Continue anyway?");
                if (!proceed)
                {
                    _firmwarePath = null;
                    TxtFilePath.Text = "";
                    BtnUpgrade.IsEnabled = false;
                    AppendLog("Firmware selection cancelled (name doesn't match connected device).");
                    return;
                }
                AppendLog($"Warning: \"{fileName}\" doesn't match device firmware \"{firmwareInfo}\" — proceeding anyway (user override).");
            }
        }

        BtnUpgrade.IsEnabled = _connectedTransport is not null;
    }

    private async void OnUpgradeClick(object? sender, RoutedEventArgs e)
    {
        if (_busy || _connectedTransport is null || _connectedCandidate is null || _deviceInfo is null || _firmwarePath is null)
        {
            return;
        }

        string deviceName = Ascii(_deviceInfo.DeviceName);
        bool confirmed = await ConfirmDialog.Ask(this,
            $"Flash \"{Path.GetFileName(_firmwarePath)}\" to {deviceName}?\n\n" +
            "This writes to the device's flash and cannot be undone. A failed or " +
            "interrupted upgrade can brick the device.");
        if (!confirmed)
        {
            return;
        }

        // FirmwareUpgradeFlow takes ownership of the transport (it closes/reopens it
        // across the device's two reboots), so this connection is no longer usable —
        // the UI must rescan/reconnect afterward regardless of outcome.
        IAscentTransport transport = _connectedTransport;
        string vid = _connectedCandidate.Vid;
        _connectedTransport = null;

        _busy = true;
        SetControlsForUpgradeRunning(true);
        ProgressUpgrade.Value = 0;

        var flow = new FirmwareUpgradeFlow(transport, vid, deviceName);
        var progress = new Progress<FlowProgress>(p =>
        {
            ProgressUpgrade.Value = Math.Clamp(p.Percent * 100, 0, 100);
            TxtStatus.Text = $"{p.StepName}: {p.Message}";
            AppendLog($"[{p.Percent * 100:F0}%] {p.StepName}: {p.Message}");
        });

        FlowResult result = await flow.RunAsync(_firmwarePath, progress);

        TxtStatus.Text = result.Success ? "Upgrade complete." : $"Upgrade failed: {result.Message}";
        TxtStatus.Foreground = result.Success ? Brushes.Green : Brushes.OrangeRed;
        ProgressUpgrade.Foreground = result.Success ? Brushes.Green : Brushes.OrangeRed;
        AppendLog(result.Success ? $"Upgrade complete: {result.Message}" : $"Upgrade failed: {result.Message}");

        TxtDeviceInfo.Text = "Not connected — rescan and reconnect to verify the new firmware.";
        BtnChooseFile.IsEnabled = false;
        _busy = false;
        SetControlsForUpgradeRunning(false);
        BtnUpgrade.IsEnabled = false;
    }

    private void SetControlsForUpgradeRunning(bool running)
    {
        BtnScan.IsEnabled = !running;
        BtnConnect.IsEnabled = !running && LstPorts.SelectedItem is PortListItem;
        BtnChooseFile.IsEnabled = !running && _connectedTransport is not null;
        BtnUpgrade.IsEnabled = !running && _connectedTransport is not null && _firmwarePath is not null;
    }

    private void AppendLog(string line)
    {
        TxtLog.Text = string.IsNullOrEmpty(TxtLog.Text) ? line : TxtLog.Text + Environment.NewLine + line;
    }

    private static string Ascii(byte[] bytes) => Encoding.ASCII.GetString(bytes).TrimEnd('\0');
}
