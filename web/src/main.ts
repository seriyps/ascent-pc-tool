import "./style.css";
import { AscentClient } from "./protocol/client.ts";
import { KNOWN_VIDS } from "./protocol/constants.ts";
import type { DeviceInfo } from "./protocol/deviceInfo.ts";
import { expectedPrefix, looksMismatched } from "./protocol/firmwareNameValidator.ts";
import { WebSerialTransport } from "./protocol/transport.ts";
import { runFirmwareUpgrade, type FlowProgress } from "./protocol/upgradeFlow.ts";

// Web Serial's requestPort() filters match on USB vendor/product ID only —
// harmless to apply for the real app (unlike the PTY-based test spike,
// where filters would have hidden the fake device entirely, since PTYs have
// no USB vendor/product ID at all). This just keeps the picker from also
// listing every legacy ttyS0-31 stub and unrelated USB-serial device.
const ASCENT_VID_FILTERS = Object.keys(KNOWN_VIDS).map((vid) => ({ usbVendorId: Number(vid) }));

const els = {
  unsupportedBanner: byId("unsupported-banner"),
  btnConnect: byId<HTMLButtonElement>("btn-connect"),
  deviceInfo: byId("device-info"),
  firmwareHint: byId("firmware-hint"),
  fileInput: byId<HTMLInputElement>("file-input"),
  btnUpgrade: byId<HTMLButtonElement>("btn-upgrade"),
  progressBar: byId("progress-bar"),
  statusText: byId("status-text"),
  log: byId("log"),
};

function byId<T extends HTMLElement = HTMLElement>(id: string): T {
  const el = document.getElementById(id);
  if (!el) throw new Error(`missing element #${id}`);
  return el as T;
}

let port: SerialPort | null = null;
let deviceInfo: DeviceInfo | null = null;
let selectedFile: File | null = null;

function appendLog(line: string): void {
  els.log.textContent += line + "\n";
  els.log.scrollTop = els.log.scrollHeight;
}

function resetResultStyling(): void {
  els.progressBar.classList.remove("success", "failure");
  els.progressBar.style.width = "0%";
  els.statusText.classList.remove("success", "failure");
  els.statusText.textContent = "";
}

async function connect(): Promise<void> {
  if (!navigator.serial) return;
  try {
    const grantedPort = await navigator.serial.requestPort({ filters: ASCENT_VID_FILTERS });
    const transport = new WebSerialTransport(grantedPort);
    await transport.open();
    const client = new AscentClient(transport);
    const info = await client.getDeviceInfo();
    await transport.close();

    port = grantedPort;
    deviceInfo = info;
    resetResultStyling();
    els.deviceInfo.textContent =
      `Device: ${info.deviceName} | Firmware: ${info.firmwareInfo} | ` +
      `Hardware: ${info.hardwareVersion} | Serial: ${info.serialNumber} | CPU: ${info.cpuTemp}C`;
    els.firmwareHint.textContent = `Expect a firmware filename containing "${expectedPrefix(info.firmwareInfo)}" for this device.`;
    els.fileInput.disabled = false;
    els.fileInput.value = "";
    els.btnUpgrade.disabled = true;
    selectedFile = null;
    appendLog(`Connected: ${info.deviceName}`);
  } catch (err) {
    appendLog(`Connect failed: ${err instanceof Error ? err.message : String(err)}`);
  }
}

async function onFileChosen(): Promise<void> {
  const file = els.fileInput.files?.[0];
  if (!file) return;

  if (deviceInfo && looksMismatched(file.name, deviceInfo.firmwareInfo)) {
    const proceed = window.confirm(
      `"${file.name}" doesn't look like it matches this device's firmware ("${deviceInfo.firmwareInfo}").\n\n` +
        "Flashing the wrong image can brick the device. Continue anyway?",
    );
    if (!proceed) {
      els.fileInput.value = "";
      els.btnUpgrade.disabled = true;
      selectedFile = null;
      appendLog("Firmware selection cancelled (name doesn't match connected device).");
      return;
    }
    appendLog(`Warning: "${file.name}" doesn't match device firmware "${deviceInfo.firmwareInfo}" — proceeding anyway (user override).`);
  }

  selectedFile = file;
  els.btnUpgrade.disabled = false;
}

async function upgrade(): Promise<void> {
  if (!port || !deviceInfo || !selectedFile) return;
  const proceed = window.confirm(
    "During the upgrade, the device may flash its status light abnormally or restart on its own — this is normal. " +
      "Keep this computer, the USB connection, and the device powered until it finishes. Continue?",
  );
  if (!proceed) return;

  els.btnConnect.disabled = true;
  els.btnUpgrade.disabled = true;
  els.fileInput.disabled = true;
  resetResultStyling();

  const onProgress = (p: FlowProgress) => {
    els.progressBar.style.width = `${Math.round(p.percent * 100)}%`;
    els.statusText.textContent = `${p.message} (${Math.round(p.percent * 100)}%)`;
    appendLog(`[${p.stepName}] ${p.message} — ${Math.round(p.percent * 100)}%`);
  };

  const upgradePort = port;
  const result = await runFirmwareUpgrade(() => new WebSerialTransport(upgradePort), deviceInfo.deviceName, selectedFile, onProgress);

  if (result.success) {
    els.progressBar.classList.add("success");
    els.statusText.classList.add("success");
    els.statusText.textContent = "Upgrade complete.";
  } else {
    els.progressBar.classList.add("failure");
    els.statusText.classList.add("failure");
    els.statusText.textContent = `Upgrade failed: ${result.message}`;
  }
  appendLog(result.success ? "Upgrade complete." : `Upgrade failed: ${result.message}`);

  els.btnConnect.disabled = false;
  els.fileInput.disabled = false;
  els.btnUpgrade.disabled = false;
}

function init(): void {
  if (!navigator.serial) {
    els.unsupportedBanner.hidden = false;
    els.btnConnect.disabled = true;
    return;
  }
  els.btnConnect.addEventListener("click", () => void connect());
  els.fileInput.addEventListener("change", () => void onFileChosen());
  els.btnUpgrade.addEventListener("click", () => void upgrade());
}

init();
