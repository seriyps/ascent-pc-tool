# Caddx PC Tool (native Linux)

A native Linux/.NET tool for configuring Caddx Ascent FPV video gear (VTX/receiver
units) over USB, with no Windows or Wine dependency.

## Build & run

```bash
cd src
dotnet build CaddxTool.slnx   # build everything
dotnet run --project CaddxTool.Avalonia   # launch the GUI
dotnet test CaddxTool.Protocol.Tests/CaddxTool.Protocol.Tests.csproj   # run tests
```

### Standalone executable

`dotnet run` needs the .NET SDK installed. To get a single native-feeling
binary with no SDK/runtime dependency instead:

```bash
cd src
dotnet publish CaddxTool.Avalonia -c Release -r linux-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ../publish
```

This produces `publish/ascent-flasher` — a self-contained ELF executable
(~100MB, since it bundles the .NET runtime) you can run directly:
`./publish/ascent-flasher`.

CI builds this on every push (download it from the workflow run's Artifacts
tab) and attaches it as a `.tar.gz` to the GitHub Release created for any
pushed `v*` tag.

## Disclaimer

Caddx's website doesn't publish a license agreement for the PC Tool, and I don't
have a Windows machine to run their original app on and see whether it shows one
during install or first run. So there's no way for me to verify how legal this
independent client implementation is — use your own judgment.

This talks to your hardware's flash over USB, including firmware upgrades. Use
it at your own risk — no responsibility is taken for any damage to your device.

## Development

Feature work happens on `main`. A sibling git worktree
(`../caddx-pc-tool-secret`, branch `secret-not-secret`) carries extra background
material not tracked here; sync it with `git merge main` from that worktree.
A fresh clone doesn't set this up automatically — recreate it with:

```bash
git worktree add ../caddx-pc-tool-secret secret-not-secret
```
