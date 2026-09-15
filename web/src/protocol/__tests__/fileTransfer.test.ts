import { describe, expect, it } from "vitest";
import { buildFileInfo, FILE_INFO_LENGTH } from "../fileTransfer.ts";
import { readAsciiField } from "../ascii.ts";

describe("buildFileInfo (ArFileInfo codec)", () => {
  it("produces a 328-byte payload with the documented field layout", () => {
    const buf = buildFileInfo("d41d8cd98f00b204e9800998ecf8427e", 12345, "/tmp/pc/firmware.img", "firmware.img");
    expect(buf.length).toBe(328);
    expect(FILE_INFO_LENGTH).toBe(328);

    expect(readAsciiField(buf, 0, 64)).toBe("d41d8cd98f00b204e9800998ecf8427e");

    const dv = new DataView(buf.buffer);
    expect(dv.getInt32(64, true)).toBe(12345);
    expect(dv.getInt32(68, true)).toBe(1); // saveAsFile

    expect(readAsciiField(buf, 72, 128)).toBe("/tmp/pc/firmware.img");
    expect(readAsciiField(buf, 200, 128)).toBe("firmware.img");
  });

  it("right-truncates a local-path hint longer than 128 chars", () => {
    const longHint = "x".repeat(140) + "END";
    const buf = buildFileInfo("abc", 1, "/tmp/pc/f.img", longHint);
    const got = readAsciiField(buf, 200, 128);
    expect(got.length).toBe(128);
    expect(got.endsWith("END")).toBe(true);
  });
});
