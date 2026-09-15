// Fixed-width, null-padded ASCII fields — the shape every payload in this
// protocol uses for strings.

export function writeAsciiField(buf: Uint8Array, offset: number, fieldLength: number, value: string): void {
  const bytes = new TextEncoder().encode(value);
  const n = Math.min(bytes.length, fieldLength);
  buf.set(bytes.subarray(0, n), offset);
}

export function readAsciiField(buf: Uint8Array, offset: number, fieldLength: number): string {
  const slice = buf.subarray(offset, offset + fieldLength);
  const nul = slice.indexOf(0);
  const trimmed = nul >= 0 ? slice.subarray(0, nul) : slice;
  return new TextDecoder("ascii").decode(trimmed);
}
