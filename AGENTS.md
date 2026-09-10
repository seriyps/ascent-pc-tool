# Caddx PC Tool — reverse-engineering & Linux enablement

## What this project is

The user owns Caddx Ascent FPV video gear (a VTX "Ascent Lite+", an "Ascent VRX Pro"
receiver, and an "Ascent GT Pro" VTX) and wants to
configure/flash it from Linux. Caddx only ships a Windows .NET Framework 4.8 WinForms
tool ("Caddx PC Tool", installer `CADDX_PC_Tool_V2.2.9_win_Setup.exe`, v2.2.9_C). This
directory holds everything produced while (a) getting that tool running under Wine, and
(b) starting a native/portable .NET reimplementation of the parts of it that matter.

Two tracks exist side by side:
1. **Wine track** — patch the real vendor binary with IL-level fixes so it runs under
   Wine (done, working). See "The Wine track" below.
2. **Native rewrite track** — a from-scratch modern .NET implementation of a useful
   subset of the app's functionality, talking to the hardware directly on Linux (no
   Wine). In progress — see `src/PROJECT.md` for the detailed plan/status.

## Directory layout

```
caddx-pc-tool/
├── AGENTS.md              this file
├── origin/                 original installer + original/patched exe (canonical copies)
├── decompiled/              full ilspycmd decompile of "Caddx PC Tool.exe" v2.2.9_C
├── patcher/                Mono.Cecil IL patcher (produces the patched exe)
├── screenshots/             UI reference screenshots (see "Screenshots" below)
└── src/                    native .NET rewrite — see src/PROJECT.md
```

**`origin/` is not tracked in git** (it's the vendor's own installer/binaries — large,
and not ours to redistribute). To reconstruct it:
1. Download the installer from Caddx's own download center:
   https://www.caddxfpv.com/pages/download-center — or from their GitHub releases,
   https://github.com/CaddxFPV-Tech/Caddx-PC-Tool-Release/releases (as of this
   writing that page tops out at 2.2.8, no 2.2.9 build — worth rechecking later,
   but the download center above has always had it).
2. Get **exactly version 2.2.9** (`CADDX_PC_Tool_V2.2.9_win_Setup.exe`) — everything
   in `decompiled/`, `patcher/`, and this doc is specific to that build; a different
   version will have different IL offsets/method bodies and the patcher will not
   apply cleanly.
3. Run the installer (in Wine or a Windows VM — see "The Wine track" below), then
   copy `Caddx PC Tool.exe` and `Caddx PC Tool.exe.config` out of the install
   directory into `origin/` here. `origin/Caddx PC Tool.patched.exe` is regenerated
   by running `patcher/` against the pristine original — see the patcher section.

## The hardware

- USB: all Ascent devices enumerate as standard **USB CDC-ACM** (Communications
  Device Class) — the Linux kernel auto-binds the built-in `cdc_acm` driver, no custom
  driver needed, shows up as `/dev/ttyACM0` etc. Confirmed via `udevadm monitor` on
  live plug events.
- The device reports USB manufacturer/product strings `"Vendor"` / `"SerialGadget"` —
  these are the **literal default strings from the Linux kernel's own USB gadget
  framework** (`g_serial`/CDC-ACM gadget). Strongly suggests the Ascent's own MCU runs
  embedded Linux with the gadget framework for its PC-tool channel, not a bare-metal
  USB stack.
- Serial params: 115200 8N1 (mostly cosmetic — real transport is USB, not a UART).
- USB VID/PID: each Ascent product declares its **own VID** (not a real
  USB-IF-assigned vendor ID — since the firmware is a USB gadget it can declare
  anything). The full set is in `Caddx_PCTool.VIDConst` (decompiled source) and is
  mirrored in `src/CaddxTool.Protocol/AscentVids.cs`. **`0x1D76` and `0x1D77` have
  been empirically confirmed** — `0x1D76` via live `udevadm` capture and the
  native tool's own sysfs discovery (on *both* an Ascent Lite+ and a real Ascent
  GT Pro — see "confusing VID/product-name note" below), `0x1D77` via the native
  Linux tool's sysfs discovery connecting to a real Ascent VRX Pro (`VID=1d77
  PID=a4a2`, matching `VIDConst`'s `Ascent_VRX_Pro` mapping — unlike `0x1D76`,
  this one's VID naming *does* match the retail product and isn't shared with
  anything else observed so far). The rest of the table is taken from decompiled
  source only, not yet verified against real hardware.
- **Confusing VID/product-name note:** `0x1D76` decodes as `Ascent_GT_Pro` in
  `VIDConst`, but is **genuinely shared by two different retail products** — a
  real Ascent GT Pro (`Ascent_GT_pro` / `Ascent_G_Sky_17_5_15` /
  `FPV-Ascent-Sky-486-V1.0-1.0`) *and* a real Ascent Lite+ (`Ascent_lite_plus` /
  `Ascent_H_Sky_18_21_10` / `FPV-Ascent-Sky-472-V1.3-1.1`) both enumerate with
  `VID=1d76`. So this isn't just a naming quirk — the VID alone cannot
  distinguish these two products at all; only the live device-info response
  (`ResDeviceInfoV2.DeviceName`/`FirmwareInfo`/`HardwareVersion`) can. Don't
  assume any VID uniquely identifies a product without confirming live.
- Why the vendor app shows the GT Pro as "unsupported" — see Patch 4 in the
  patcher section below, which already covers `AscentDeviceNameResolver`'s
  consumer-build allowlist gate and how it's patched around.
- Devices/firmware seen so far:
  | Product (as connected)     | DeviceName          | Firmware               | Hardware                        | VID (real/assumed) |
  |-----------------------------|---------------------|-------------------------|----------------------------------|---------------------|
  | Ascent Lite+ (VTX)          | Ascent_lite_plus     | Ascent_H_Sky_18_21_10   | FPV-Ascent-Sky-472-V1.3-1.1      | `1d76` (confirmed via udev) |
  | Ascent VRX Pro (receiver)   | Ascent_VRX_pro       | Ascent_VRX_Pro_18_21_7  | Ascent-VRX-Pro-V3.0-1.0          | `1d77` (confirmed via native tool's sysfs discovery; the Wine-track patcher still reports a hardcoded `1d76` for any device — see patcher section, unaffected by this) |
  | Ascent GT Pro (VTX)         | Ascent_GT_pro        | Ascent_G_Sky_17_5_15    | FPV-Ascent-Sky-486-V1.0-1.0       | `1d76` (confirmed via native tool's sysfs discovery — **same VID as the Lite+**, see note below) |

## What's in `decompiled/`

Full `ilspycmd` decompile of `origin/Caddx PC Tool.exe` (namespace `Caddx_PCTool`,
~176 `.cs` files). Decompiling loses some fidelity — files sometimes have
`//IL_XXXX: Unknown result type` comments where ilspy couldn't perfectly reconstruct
source; this mostly clusters around WMI/COM-interop code and WinForms designer glue,
not the protocol logic (which decompiled cleanly).

Key findings from reading it:

- **App is a bundle of several Caddx product tools sharing one codebase.** Evidence:
  a large (~13k line) auto-generated `MAVLink.cs`, `MSPHelper`/`MSPCMD`,
  `MavSerialport`/`MavlinkUtil`, PX4 mode enums, and a `CaddxModel` enum with entries
  `fpv1_6`, `f405Wing`, `headTrack` — none of which are referenced anywhere else in
  the codebase (verified by grep). These almost certainly belong to Caddx's
  FPV-drone/flight-controller product line (the user's own guess: their "Protos"
  drone line or similar), bundled in but **dead code for the Ascent line**. Safe to
  ignore.
- **Packaging:** Inno Setup 6.7.2 installer wrapping a **Velopack**-packaged app
  (Velopack = Squirrel-style self-updater, itself cross-platform-capable). Installer
  aborts before copying any files if .NET Framework 4.8 isn't present in the registry
  (`InitializeSetup` check) — this is why the Wine popup says "PC Tool requires
  Microsoft .NET Framework 4.8" immediately, before any UI shows.
- **Hidden UserLevel gate:** `GD.CurrUserLevel` (default `UserLevel.op`) gates which
  functions are visible. `PasswordFrm.cs` contains hardcoded passwords —
  **`caddx905`** → `UserLevel.caddx` (unlocks everything), **`roi`** →
  `UserLevel.customA` (partial). **`PasswordFrm` is never actually invoked anywhere in
  this build** (verified by grep for all instantiation sites) — it's dead/orphaned
  code, presumably reachable via some button in an internal Caddx engineering build
  that got stripped for retail release. Our power-user patch (below) recreates the
  effect via an env var instead of trying to resurrect a UI trigger for it.
- **`FuntionType` enum** (sic — typo is in the original code, and even in the shipped
  UI: the dialog title literally reads "Fuction select"): `findDevice`, `upgrade`,
  `gmSet` (gimbal), `camhub` (a separate camera-hub accessory board), `rcMode`,
  `bb_freq`. Availability per `UserLevel` is in
  `MainFrm.GetAvailableFunctions(devname, level)`. At normal (`op`) level, ordinary
  customers only ever see `upgrade` — matches what ships.
- **`rcMode`** ("RC Mode" screen): the Ascent hardware extracts 4 independent RC
  "slots" by watching value ranges inside the **MSP DisplayPort** stream already
  flowing from a connected Betaflight/INAV flight controller over the video link (this
  is *not* the app talking MSP directly — the parsing happens on the Ascent's own
  firmware; the PC tool only configures the slot→range→action mapping). Actions:
  zoom control, zoom reset, cam switching, cam reset. Data model: `SlotData.cs` (4
  slots, each `value`/`min`/`max`). Config is serialized to JSON and pushed to the
  device via the **same generic file-transfer mechanism used for firmware upgrades**
  (`GD.Inst.UpgFSM.SetFilePath_Json(...)`, `Send_FileStart_Json()`) — see
  `RCModeCtrl.cs`.
- **`bb_freq`** ("Update channel" screen): uploads a named RF channel/frequency table
  (band names like `bandA`/`bandB`/`bandC`, classic race-band-style naming even though
  the link is digital) to the device, again via the same file-transfer mechanism
  (`Send_FileStart_BBFreq(path, "/factory/fpv_bb_freq.json")`). See `updateChannel.cs`.
  Button semantics (verified by reading the click handlers, not guessed):
    - **"Add listview"** — 100% local/safe. Opens a `.bin`/JSON file from disk and
      populates the on-screen grid only. No device I/O.
    - **"Save BB_Freq"** — calls `Send_GetBBFreq(...)`, a **read** from the device. See
      "known-broken" note below.
    - **"Upload BB_Freq"** — the dangerous one: validates, then calls
      `Send_FileStart_BBFreq(...)`, which **writes** the table to the device. Has a
      built-in safety check (RF-board-type mismatch → refuses and shows a warning
      dialog before sending anything).
  **Known bug (pre-existing, not caused by our patches):** the band dropdown is
  hardcoded in designer code to always offer `["bandA","bandB","bandC"]`
  (`InitializeComponent`), but `_bandDict` only contains whatever bands the loaded file
  actually defines. Switching to a band absent from the loaded file throws an
  unhandled `KeyNotFoundException` in `RefreshDictVal` (no existence check). Harmless —
  happens entirely in local UI code, no device I/O involved — safe to click
  "Continue" on the resulting dialog.
  **Known-broken op:** `Send_GetBBFreq` (read current table from device) times out on
  real firmware. Confirmed via log analysis on *both* the Lite+ and VRX Pro: the
  device ACKs the request (`cmd=0x78`) but never sends the expected follow-up data
  frame (`cmd 116`/`0x74`); after 2 retries the app marks it `Failed` internally with
  **no visible error dialog** (looks like the button "does nothing"). Either this op
  isn't implemented in these firmware builds, or it needs some precondition we're not
  meeting. Not yet investigated further — would need a packet-level look at what's
  actually inside that 104-byte ACK payload.
- **`camhub`** and **`gmSet`** (gimbal) screens exist and work (per power-user
  screenshots) but are out of scope for the user right now (no camhub board, no
  gimbal owned — "nice to have, not now").
- **Wire protocol** — see `src/PROJECT.md` "Protocol reference" for the full writeup;
  it's ported into `src/CaddxTool.Protocol` and has been validated live.

## Screenshots

`screenshots/*.png` — normal customer flow (what ships to retail: Device tab →
connect → straight to Firmware upgrade, no function picker).

`screenshots/power-user/*.png` — the unlocked engineering menu (via the
`CADDX_POWER_USER=1` patch, see below): function-picker grid, RC Mode screen, Cam Hub
screen (with an actual PCB diagram + port-routing UI), Gimbal (GM Set, partially
untranslated Chinese labels), and the bb_freq "Update channel" screen including the
RF-mismatch warning and the band-switch crash dialog.

## The Wine track

### Setup

- Wine prefix: `~/wine-caddx-test` (separate from the user's normal `~/.wine` on
  purpose — throwaway/disposable), `WINEARCH=win64`.
- Required one-time installs into that prefix:
  - `winetricks dotnet48` — the installer's own `InitializeSetup` check refuses to
    copy any files at all without a real .NET Framework 4.8 registry footprint
    present (this is why the "requires .NET Framework 4.8" popup appears
    immediately under plain Wine, before any UI shows).
  - `winetricks gdiplus` — **required**, and non-obvious why: the app loads several
    bundled fonts unconditionally at every startup regardless of chosen UI language
    (`Program.GetLanguageCodeAndLoad`). One of them, `SourceHanSans-*.ttf` (used for
    the Chinese UI), is actually **OpenType/CFF** (PostScript outlines, `OTTO` sfnt
    signature) rather than TrueType (`glyf` outlines). Classic GDI+
    `PrivateFontCollection.AddFontFile` has **never supported CFF-flavored OpenType
    fonts** — only TrueType. This is true of both Wine's own reimplemented
    `gdiplus.dll` (throws a misleading `OutOfMemoryException`) *and* the old
    Vista/Win7-era `gdiplus.dll` that `winetricks gdiplus` installs (throws
    `FileNotFoundException` instead — equally misleading). Modern Windows 10/11's
    real `gdiplus.dll` apparently handles this fine, which is presumably why it works
    for Caddx's own QA. **Fix applied:** edited
    `Caddx PC Tool.exe.config` to repoint `FONT_TitleZHPath`/`FONT_TextZHPath` from
    the `SourceHanSans-*.ttf` (CFF) files to the `ArgentumSans-*.ttf` (TrueType) files
    already bundled — Chinese glyphs would render as tofu if that UI language were
    ever selected, but English/Russian UI (what the user actually uses) is
    unaffected, and the app now launches. This is a config-file edit, not a binary
    patch — trivial to redo on a fresh install.
  - Also tried (and abandoned) converting `SourceHanSans-*.ttf` from a `.ttc`-style
    font collection to a single font via `fontTools` — that fixed the "is it a
    collection" problem but not the underlying CFF-outline incompatibility, so the
    font-path-redirect approach above is the one actually in use.
- **WMI is fundamentally broken in Wine** for this purpose: `wbemprox`'s
  `Win32_PnPEntity` provider returns fake stub entries (`Name = "Wine PnP Device"`
  for everything), not real device data. Confirmed by direct reproduction: querying
  `Win32_PnPEntity` in Wine returns 11 stub rows regardless of what's actually
  plugged in. This is *the* root blocker for device discovery under Wine, and is not
  fixable via winetricks/config — hence the IL patches below.

### Patcher (`patcher/`)

A small C# console tool (`patcher/Program.cs`, references `Mono.Cecil` — found
bundled with the `ilspycmd` dotnet-tool install, no separate NuGet needed) that does
**binary IL patching** of `origin/Caddx PC Tool.exe` directly — it does **not**
recompile from the decompiled source.

**Why patch the binary instead of recompiling `decompiled/`:** two independent
reasons, both hard blockers, not just convenience:
1. .NET has no C/C++-style "compile each file separately, link object files"
   model — a whole assembly is one atomic compilation unit. There's no way to
   "recompile just the changed class" using the normal build pipeline.
2. Even a hand-assembled *separate* replacement assembly for one class wouldn't
   work: the methods we need to fix call back into **private** fields/methods of
   their own class (e.g. `UsbSerialMonitor.UpdateDeviceSnapshot`, `_connectedDevices`).
   Private access is enforced by the CLR based on true type identity, so a
   separately-compiled type with the same name is a different type and can't reach
   them. Patching the existing compiled method in place means the new code runs *as*
   the original type, so it can call those private members for free.

Run it with: `cd patcher && dotnet build && dotnet run -- "<in>.exe" "<out>.exe"`.
Deploy by copying the output over
`~/wine-caddx-test/drive_c/CaddxPCTool/current/Caddx PC Tool.exe` (that's also where
the app's data files — `resource/`, `Log/`, `3rdLibs/` — already live; don't move the
exe elsewhere without them). `origin/Caddx PC Tool.patched.exe` holds the last patched
build; `origin/Caddx PC Tool.exe` is the pristine original (also duplicated as
`...exe.orig-backup` inside the Wine prefix's `current/` folder for quick A/B).

**Patch 1 — `UsbSerialMonitor.ManualSearchDevices()`:** the main device-discovery path
used for the Device tab. Original builds a `Win32_PnPEntity` WQL query per COM port
and regexes `VID_xxxx&PID_xxxx` out of `PNPDeviceID` — dead on arrival given the WMI
stub problem above. Patched version instead: calls `SerialPort.GetPortNames()`
directly, skips anything in the `COM1`–`COM32` range (Wine's fixed legacy mapping for
`/dev/ttyS0`–`ttyS31`, which exist as real kernel-created device nodes on this
machine even with no physical UART hardware — confirmed via `ls /dev/ttyS*` — so
"port exists" isn't a usable discriminator on its own), and tags anything else with a
hardcoded `VID="1D76"`, `PID="0101"`. **This means any single non-legacy serial port
gets reported as if it were that specific Ascent variant, regardless of which Ascent
product is actually plugged in** — fine for a single-device dev/test box, but a real
limitation to know about if the user ever has two different serial devices attached
at once, or wants the VID to reflect the true connected product.

**Patch 2 — `updateChannel.ManualSearchDevices(out List<UsbDevInfo>)`:** the bb_freq
screen has its **own separate, duplicated** implementation of the exact same
WMI-based lookup (not shared with `UsbSerialMonitor`) — patched identically. Note
the signature difference (`bool`-returning, `out` parameter) required a different IL
shape; see "IL-patching gotchas" below for the bug this produced initially.

**Patch 3 — power-user unlock (`Program.Main()`):** inserts a check for
`CADDX_POWER_USER=1` that sets `GD.Inst.CurrUserLevel = UserLevel.caddx` (same effect
as typing the `caddx905` password, had that dialog still been reachable). **Must be
inserted immediately before `new MainFrm()` is constructed, not at the top of
`Main()`** — `Program.ReadParam()` (called earlier, from `LoadSplashScreen()`)
deserializes a saved `GD` object from disk and does `_inst = myGD`, **replacing the
entire singleton wholesale**; `CurrUserLevel` is `[JsonIgnore]`'d, so that
replacement silently resets it back to `UserLevel.op`, wiping out an earlier unlock.
Includes `WriteLog.writeLog(...)` diagnostic calls (writes to
`current/Log.txt`, distinct from the dated `current/Log/YYYY-MM-DD.log`) confirming
the env var value seen and whether the unlock applied — useful if this is ever
suspected of regressing again.

**Patch 4 — drop the consumer-build device allowlist gate
(`AscentDeviceNameResolver.IsConsumerVersion()`):** `ResolveDisplayName()` only
trusts its full device-name lookup table when `IsConsumerVersion(Program.
SoftwareVersion)` is false; that check looks for a `_C` marker, which this
retail installer's version (`v2.2.9_C`) always has. For "consumer" builds it
instead requires the device name to appear in a small hardcoded allowlist
(`ConsumerAllowedDeviceNameKeys`) that Caddx never updated for newer models —
`ascent_gt_pro` (and its z40/z8/hub variants) isn't in it, even though the rest
of the resolution/display code for those models is fully present and
functional. This is why the stock retail app shows a connected GT Pro as
"unsupported" — a deliberate product-tier gate, not a technical limitation.
Patch makes `IsConsumerVersion()` unconditionally return `false`, so
`ResolveDisplayName` always takes the non-consumer path and recognizes every
device the resolver already knows how to name. **Confirmed working against a
real Ascent GT Pro.**

Launch: same binary serves both modes.
```bash
# normal
WINEPREFIX=~/wine-caddx-test WINEARCH=win64 wine "$HOME/wine-caddx-test/drive_c/CaddxPCTool/current/Caddx PC Tool.exe"
# power-user (unlocked function grid)
CADDX_POWER_USER=1 WINEPREFIX=~/wine-caddx-test WINEARCH=win64 wine "$HOME/wine-caddx-test/drive_c/CaddxPCTool/current/Caddx PC Tool.exe"
```
No hotplug in either mode yet — device discovery only runs once, at app startup
(`StartMonitoring()` calls `ManualSearchDevices()` exactly once; the WMI event
watchers that would normally drive live hotplug are the same broken mechanism, and
were left as-is rather than patched, since they fail silently rather than crash).
**Device must already be plugged in and enumerated (`/dev/ttyACM0` present) before
launching the app.** A timer-based poll patch (call `ManualSearchDevices()` every few
seconds instead of once) would fix this but hasn't been built — see `src/PROJECT.md`
for why the native rewrite makes this moot anyway (real `udev`/inotify monitoring
instead of polling).

### IL-patching gotchas (learned the hard way — read before touching `patcher/Program.cs` again)

- **Never emit a bare `ret` inside a `try`/`catch` region.** CIL requires every exit
  from a protected region to go through `leave` to a shared point *outside* all
  protected regions; a direct `ret` compiles fine, round-trips fine through
  `ilspycmd` (which reconstructs plausible-looking C# regardless), but throws
  `System.InvalidProgramException` at JIT time. This was the exact cause of a real
  crash during this project (patch 2's catch handler originally used `ret` directly).
  **Decompiling the patched output is not sufficient verification** — ilspy will
  happily decompile invalid IL into innocent-looking source. The reliable check used
  in this project: force JIT compilation via reflection
  (`RuntimeHelpers.PrepareMethod(methodInfo.MethodHandle)`) in a tiny harness compiled
  and run *inside* the same Wine/.NET-Framework environment the real app uses — this
  actually throws `InvalidProgramException` at the right moment if the IL is bad,
  without needing to click through the UI.
- **Don't use `typeof()`/reflection in the patcher to build references to BCL
  types.** The patcher itself runs on modern .NET (this box has only the net10.0
  SDK/runtime installed), whose core types live in `System.Private.CoreLib` — a
  completely different assembly identity than .NET Framework 4.8's `mscorlib`/
  `System`, which is what the *target* binary needs. Cecil's `ImportReference(Type)`
  does not remap this for you; it imports whatever assembly the host runtime reports,
  producing a reference the Framework 4.8 CLR can't resolve (`FileNotFoundException:
  System.Private.CoreLib...` at runtime — silent until the exact line executes).
  `module.TypeSystem.{Void,Boolean,Int32,String}` are safe (Cecil pre-scopes these
  correctly to the target module's own corlib). Anything else (`DateTime`,
  `Exception`, `Environment`, `SerialPort`, etc.) must be built by hand:
  `new TypeReference(ns, name, module, targetAssemblyRef)`, where
  `targetAssemblyRef` comes from `module.AssemblyReferences.First(a => a.Name ==
  "mscorlib" | "System")` — i.e. a reference the *target* module already declares.
- **Generic BCL method references (e.g. `List<T>.Add`) need the open type's generic
  parameter as the argument type, not the closed argument type.** i.e. build
  `new MethodReference("Add", voidType, closedListType) { HasThis = true }` and add a
  `ParameterDefinition(openListTypeRef.GenericParameters[0])` — using the *closed*
  element type directly there produces a `MissingMethodException` at runtime (a
  signature-encoding mismatch invisible in decompiled output).
- `patcher/refs/System.IO.Ports.dll` is a copy of the official NuGet
  `system.io.ports` package's `lib/net462` build. It's only there to stop Cecil's
  metadata resolver from stack-overflowing (a genuine Cecil bug: circular
  `ExportedType` forwarder resolution) while resolving an **unrelated, pre-existing**
  parameter default value (`SerialPortHelper`'s constructor has a `Handshake`
  enum-typed default parameter) that Cecil insists on resolving during *any* write of
  the assembly, not just when we touch that method. The actual fix applied is
  simpler than perfect resolution: the patcher strips `HasConstant`/
  `ParameterAttributes.HasDefault` from any parameter whose type lives in
  `System.IO.Ports` before writing — safe, since already-compiled call sites never
  rely on default-parameter reflection metadata at runtime.

## Sandbox/environment quirks (specific to this dev machine, not the project)

- `lsusb` is blocked by an AppArmor profile that denies it writing to this Bash
  tool's output-capture file — use `/sys/bus/usb/devices/*/idVendor` +`idProduct`
  +`product` directly instead, or `udevadm monitor --udev --environment` for live
  plug/unplug events.
- ImageMagick's `import` (screenshot) fails with a mysterious "missing an image
  filename" error regardless of syntax — looks like another sandbox restriction, not
  a usage bug. Screenshots for this project were taken by the user manually and
  dropped into `screenshots/`.
- `.NET`'s own `File.ResolveLinkTarget(path, returnFinalTarget: true)`
  **mis-resolves** the particular chain of relative symlinks under
  `/sys/class/tty/<name>/device` (observed: collapses to a bogus short path like
  `/sys/3-9:1.0` instead of the real
  `/sys/devices/pci.../usb3/3-9/3-9:1.0`). `readlink -f` (shelled out via `Process`)
  resolves it correctly. Worked around in
  `src/CaddxTool.Protocol/AscentDeviceFinder.cs` — see that file/`src/PROJECT.md`.
