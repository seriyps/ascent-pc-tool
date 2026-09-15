import { writeAsciiField } from "./ascii.ts";

// SENDFILE_START payload (ArFileInfo). Ported from ArFileInfoCodec.cs —
// 328 bytes: md5Hex[64] + length:i32 + saveAsFile:i32(=1) + filePath[128] +
// fileDir[128].
//
// "fileDir" was the *local* path on the sending PC in the original app —
// a concept that doesn't really exist for a browser (File objects carry no
// filesystem path). We send the file's own name there instead, which is a
// deliberate deviation, not a bug: nothing on the device side is known to
// read this field back (it exists purely for the vendor app's own logging).
const MD5_FIELD_LENGTH = 64;
const PATH_FIELD_LENGTH = 128;
export const FILE_INFO_LENGTH = MD5_FIELD_LENGTH + 4 + 4 + PATH_FIELD_LENGTH + PATH_FIELD_LENGTH;

export function buildFileInfo(md5Hex: string, length: number, remotePath: string, localPathHint: string): Uint8Array {
  const buf = new Uint8Array(FILE_INFO_LENGTH);
  const dv = new DataView(buf.buffer);
  let o = 0;
  writeAsciiField(buf, o, MD5_FIELD_LENGTH, md5Hex); o += MD5_FIELD_LENGTH;
  dv.setInt32(o, length, true); o += 4;
  dv.setInt32(o, 1, true); o += 4; // saveAsFile
  writeAsciiField(buf, o, PATH_FIELD_LENGTH, remotePath); o += PATH_FIELD_LENGTH;

  const hint = localPathHint.length > PATH_FIELD_LENGTH ? localPathHint.slice(-PATH_FIELD_LENGTH) : localPathHint;
  writeAsciiField(buf, o, PATH_FIELD_LENGTH, hint);
  return buf;
}
