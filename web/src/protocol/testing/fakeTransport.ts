import { HEADER_LENGTH } from "../constants.ts";
import { buildPacket, parseHeader, type PacketHeader } from "../packet.ts";
import { TransportTimeoutError, type SerialTransport } from "../transport.ts";

export interface SentPacket {
  header: PacketHeader;
  payload: Uint8Array;
}

// Returning undefined simulates "drop this send, don't respond" (for
// retry/timeout testing). Returning an array lets a test inject extra
// frames ahead of the real response (e.g. a stale/unrelated packet that
// should be ignored while waiting for the right one).
export type FakeResponder = (sent: SentPacket) => Uint8Array | Uint8Array[] | undefined;

// In-memory SerialTransport double for fast, deterministic protocol tests —
// no browser/serial involved. Mirrors the design intent of the .NET tests'
// fake transport, reimplemented fresh for this client.
export class FakeTransport implements SerialTransport {
  readonly sent: SentPacket[] = [];
  private incoming = new Uint8Array(0);
  private closed = false;
  private readonly responder: FakeResponder;

  constructor(responder: FakeResponder) {
    this.responder = responder;
  }

  async open(): Promise<void> {
    // No-op — nothing to open for an in-memory transport.
  }

  async write(bytes: Uint8Array): Promise<void> {
    if (this.closed) throw new Error("transport closed");
    const header = parseHeader(bytes.subarray(0, HEADER_LENGTH));
    const payload = bytes.subarray(HEADER_LENGTH, HEADER_LENGTH + header.length);
    this.sent.push({ header, payload });

    const result = this.responder({ header, payload });
    const frames = result === undefined ? [] : Array.isArray(result) ? result : [result];
    for (const frame of frames) {
      this.appendIncoming(frame);
    }
  }

  private appendIncoming(frame: Uint8Array): void {
    const combined = new Uint8Array(this.incoming.length + frame.length);
    combined.set(this.incoming, 0);
    combined.set(frame, this.incoming.length);
    this.incoming = combined;
  }

  async readExact(count: number, timeoutMs: number): Promise<Uint8Array> {
    const deadline = Date.now() + timeoutMs;
    while (this.incoming.length < count) {
      if (Date.now() >= deadline) throw new TransportTimeoutError();
      await new Promise((resolve) => setTimeout(resolve, Math.min(2, timeoutMs)));
    }
    const result = this.incoming.slice(0, count);
    this.incoming = this.incoming.slice(count);
    return result;
  }

  async close(): Promise<void> {
    this.closed = true;
  }
}

// Convenience for building a canned ack frame in tests.
export function buildAckFrame(cmd: number, seq: number, payload?: Uint8Array): Uint8Array {
  return buildPacket(cmd, 2 /* MSGTYPE_ACK */, seq, 0, payload);
}
