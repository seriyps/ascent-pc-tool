# CaddxTool.FakeDevice

Simulates the device side of the Ascent wire protocol (FIND_DEVICE, REBOOT,
REMOTE_UPGRADE, SENDFILE_START/DATA/END, UPGRADE_STATUS) over a real
`System.IO.Ports.SerialPort`, so a real client can run a full firmware-upgrade
flow against it with no actual hardware involved — useful for testing the
protocol/framing/CRC/retry logic, and for capturing wire traffic to diff
between two different clients talking to the exact same simulated device.

Kept as a permanent, committed tool (not a throwaway script) so this kind of
comparison can be re-run whenever needed — e.g. against a newer release of
this project's own client, or to re-verify protocol conformance after a
refactor.

## Console only — not the GUI

`CaddxTool.Avalonia`'s device scan (`AscentDeviceFinder.FindCandidates()`) does
sysfs-based **USB** device enumeration by VID/PID. A simulated serial port
backed by a PTY (or any non-USB tty) is not a USB device and will never appear
there, no matter what's listening on the other end. Use `CaddxTool.Console`'s
`upgrade` subcommand instead — it takes the port path directly and bypasses
device scanning entirely (see below), which is also why it's the right tool
for this rather than a stripped-down GUI mode.

## Setup (no kernel module needed)

A plain `socat` pty pair is enough to test this project's own native client
against `FakeDevice` — no root, no kernel module:

```bash
# 1. Create a linked PTY pair. raw+echo=0 matters: this is a binary framed
#    protocol, canonical/echo terminal mode would corrupt it.
socat -d -d pty,raw,echo=0,link=/tmp/ascent-fake-a pty,raw,echo=0,link=/tmp/ascent-fake-b &

# 2. Start the fake device on one end. --device picks a canned identity
#    (lite_plus / vrx_pro / gt_pro, see FakeDeviceProfile.cs); --log writes a
#    diff-friendly capture (one line per frame: cmd/seq/retry/len/payload-hex).
#    Large SENDFILE_DATA payloads are summarized as length+CRC32 rather than
#    hex-dumped in full, since their content is just the input file's own bytes.
dotnet run --project CaddxTool.FakeDevice -- /tmp/ascent-fake-b --device lite_plus --log /tmp/capture.log

# 3. Drive it with the console client. Any file works as the "firmware" here
#    (FakeDevice never inspects its contents) — the upgrade subcommand bypasses
#    real AscentDeviceFinder scanning for the post-reboot reconnects (a PTY
#    never disappears/reappears the way real USB hardware does) by always
#    reopening the same fixed path.
dotnet run --project CaddxTool.Console -- upgrade /tmp/ascent-fake-a <firmware-file> 1d76 ascent_lite_plus
```

A full run (FIND_DEVICE → REBOOT → REMOTE_UPGRADE → SENDFILE_START →
SENDFILE_DATA chunks → SENDFILE_END → UPGRADE_STATUS polls → final device-info
refresh) takes well under a minute over a plain PTY — `socat` doesn't enforce
any baud-rate pacing, so chunk transfer is effectively instant regardless of
file size.

## Diffing two runs

Point two different clients at the same `--log` path (one run at a time) and
`diff` the resulting files. Expect the command sequence, chunk CRCs, and ack
payload structures to match exactly between conformant clients; a client-
specific detail like the starting sequence number, or a locally-resolved file
path baked into the `SENDFILE_START` payload, is not a bug by itself.
