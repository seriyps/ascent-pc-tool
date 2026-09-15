export class TransportTimeoutError extends Error {
  constructor(message = "transport read timed out") {
    super(message);
    this.name = "TransportTimeoutError";
  }
}

export interface SerialTransport {
  open(): Promise<void>;
  // Writes `bytes`, optionally split into smaller slices so `onProgress`
  // (cumulative bytes handed to the underlying transport) can drive a
  // smoother progress bar during a large SENDFILE_DATA write — this is a
  // transmission-progress signal (bytes queued to the OS/USB layer), not a
  // device-processed signal. The protocol-level chunk size (SENDFILE_DATA
  // payload length) is unaffected by this — see upgradeFlow.ts.
  write(bytes: Uint8Array, onProgress?: (bytesWritten: number) => void): Promise<void>;
  // Blocks until exactly `count` bytes are available or `timeoutMs` elapses,
  // in which case it throws TransportTimeoutError. Mirrors the .NET port's
  // AscentClient.ReadExact semantics.
  readExact(count: number, timeoutMs: number): Promise<Uint8Array>;
  close(): Promise<void>;
}

const DEFAULT_WRITE_SLICE_SIZE = 32 * 1024;

// Wraps a granted Web Serial `SerialPort`. Streams deliver arbitrary-sized
// chunks, not frame-aligned, so a background pump loop continuously drains
// the port's readable stream into a byte buffer; readExact only waits for
// enough bytes to accumulate, it never issues its own throwaway read() —
// that would risk silently dropping bytes that arrive just after a
// readExact() call gives up and its caller resends (ReadableStreamDefaultReader
// has no per-read cancel, so an abandoned read() would still resolve with
// real data that nothing is listening for).
export class WebSerialTransport implements SerialTransport {
  private writer: WritableStreamDefaultWriter<Uint8Array> | null = null;
  private reader: ReadableStreamDefaultReader<Uint8Array> | null = null;
  private pending = new Uint8Array(0);
  private pumpError: unknown = null;
  private waiters: Array<() => void> = [];
  private readonly port: SerialPort;
  private readonly writeSliceSize: number;

  constructor(port: SerialPort, writeSliceSize = DEFAULT_WRITE_SLICE_SIZE) {
    this.port = port;
    this.writeSliceSize = writeSliceSize;
  }

  async open(baudRate = 115200): Promise<void> {
    await this.port.open({ baudRate });
    if (!this.port.readable || !this.port.writable) {
      throw new Error("serial port has no readable/writable stream after open()");
    }
    this.writer = this.port.writable.getWriter();
    this.reader = this.port.readable.getReader();
    void this.pumpLoop();
  }

  private async pumpLoop(): Promise<void> {
    const reader = this.reader;
    if (!reader) return;
    try {
      while (true) {
        const { value, done } = await reader.read();
        if (done) break;
        if (value && value.length > 0) {
          const combined = new Uint8Array(this.pending.length + value.length);
          combined.set(this.pending, 0);
          combined.set(value, this.pending.length);
          this.pending = combined;
          this.wakeWaiters();
        }
      }
    } catch (err) {
      this.pumpError = err;
      this.wakeWaiters();
    }
  }

  private wakeWaiters(): void {
    const waiters = this.waiters;
    this.waiters = [];
    for (const wake of waiters) wake();
  }

  private waitForDataOrTimeout(timeoutMs: number): Promise<void> {
    return new Promise((resolve) => {
      const timer = setTimeout(resolve, timeoutMs);
      this.waiters.push(() => {
        clearTimeout(timer);
        resolve();
      });
    });
  }

  async write(bytes: Uint8Array, onProgress?: (bytesWritten: number) => void): Promise<void> {
    if (!this.writer) throw new Error("transport not open");
    let offset = 0;
    while (offset < bytes.length) {
      const end = Math.min(offset + this.writeSliceSize, bytes.length);
      await this.writer.write(bytes.subarray(offset, end));
      offset = end;
      onProgress?.(offset);
    }
    if (bytes.length === 0) onProgress?.(0);
  }

  async readExact(count: number, timeoutMs: number): Promise<Uint8Array> {
    const deadline = Date.now() + timeoutMs;
    while (this.pending.length < count) {
      if (this.pumpError) throw this.pumpError;
      const remaining = deadline - Date.now();
      if (remaining <= 0) throw new TransportTimeoutError();
      await this.waitForDataOrTimeout(remaining);
      if (this.pending.length < count && Date.now() >= deadline) {
        throw new TransportTimeoutError();
      }
    }
    const result = this.pending.slice(0, count);
    this.pending = this.pending.slice(count);
    return result;
  }

  async close(): Promise<void> {
    try {
      await this.reader?.cancel();
    } catch {
      // best-effort
    }
    try {
      this.writer?.releaseLock();
    } catch {
      // best-effort
    }
    this.reader = null;
    this.writer = null;
    try {
      await this.port.close();
    } catch {
      // best-effort
    }
  }
}
