# Caddx Ascent web tool (TypeScript, static site)

A minimal browser-based alternative to `CaddxTool.Avalonia` for connecting
to and upgrading Caddx Ascent devices — same wire protocol, reimplemented
natively in TypeScript for the [Web Serial
API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Serial_API).
Builds to a static HTML/JS/CSS bundle with no server component.

Needs a browser with the Web Serial API: Chrome/Edge (or other
Chromium-based browsers), or Firefox 151+. Not implemented in Safari.

## Using it

```bash
npm install
npm run build    # static bundle in dist/
```

Serve `dist/` with any static file server (e.g. `python3 -m http.server`)
over `http://localhost` or HTTPS — Web Serial requires a secure context.
Open it in a supported browser, click Connect and pick the device's port,
then choose a `.img` firmware file and click Upgrade.

## Developing

```bash
npm run dev      # local dev server with hot reload
npm run test     # vitest — protocol-layer unit + integration tests, no browser needed
```

The protocol implementation lives under `src/protocol/`, one module per
concern (packet framing, CRC, response codecs, the firmware-upgrade
orchestrator). It's tested against an in-memory fake transport
(`src/protocol/testing/fakeTransport.ts`) and, for the upgrade flow, an
integration-level replay of a real captured `CaddxTool.FakeDevice` session
(`src/protocol/__tests__/upgradeFlow.test.ts`) — no real hardware or
browser needed to run the test suite.

To test live against `../src/CaddxTool.FakeDevice` instead of real
hardware, you need a virtual serial port pair that Chrome's Web Serial
picker can actually see — a plain `socat` PTY does not work, but the
[`tty0tty`](https://github.com/freemed/tty0tty) kernel module does. Note
that `main.ts`'s Connect button filters the port picker by USB vendor ID
(`ASCENT_VID_FILTERS`), which hides `tty0tty` devices (they have no USB
vendor ID); drop the `filters` option temporarily when testing this way.

Known deliberate deviations from the .NET port: reconnect polls
`port.open()` instead of re-scanning by VID (Web Serial's `SerialPort`
object persists across a device reboot); MD5 is computed via the `js-md5`
package since browsers only expose SHA hashes.
