import { describe, expect, it } from "vitest";
import { expectedPrefix, looksMismatched } from "../firmwareNameValidator.ts";

describe("expectedPrefix", () => {
  it("falls back to a 12-char prefix when there's no trailing 3+ digit run", () => {
    // Real firmware-info strings observed live (see CaddxTool.FakeDevice's
    // FakeDeviceProfile.cs) — both end in a 1-2 digit version suffix, too
    // short to trigger the trailing-digit-run branch.
    expect(expectedPrefix("Ascent_H_Sky_18_21_10")).toBe("Ascent_H_Sky");
    expect(expectedPrefix("Ascent_VRX_Pro_18_21_7")).toBe("Ascent_VRX_P"); // truncates mid-word, harmless
  });

  it("strips a trailing run of 3+ digits and drops the preceding separator, when present", () => {
    expect(expectedPrefix("Foo_Bar_123456")).toBe("Foo_Bar");
  });

  it("also strips the literal word 'Ascent' when the digit-run branch fires", () => {
    expect(expectedPrefix("Ascent_Foo_123456")).toBe("_Foo");
  });

  it("doesn't throw on a short string (JS slice() tolerates negative indices, unlike C# Substring)", () => {
    // The .NET port's C# Substring(0, -1) throws here, caught by a
    // try/catch to fail open; JS's slice() just clamps instead — same
    // fail-open *outcome* via a different mechanism, worth a test so the
    // language difference doesn't get "fixed" into a throw by accident.
    expect(() => expectedPrefix("123")).not.toThrow();
  });
});

describe("looksMismatched", () => {
  it("returns false for a filename containing the expected prefix", () => {
    expect(looksMismatched("Ascent_H_Sky_18_21_10.img", "Ascent_H_Sky_18_21_10")).toBe(false);
  });

  it("returns true for a filename that clearly belongs to a different device", () => {
    expect(looksMismatched("Ascent_VRX_Pro_18_21_7.img", "Ascent_H_Sky_18_21_10")).toBe(true);
  });

  it("fails open (no mismatch) rather than throwing on genuinely malformed input", () => {
    // Simulates unexpected non-string input (e.g. a corrupted device
    // response) reaching this far — .replace()/regex would throw on
    // null/undefined, which the try/catch should swallow.
    expect(looksMismatched(null as unknown as string, "Ascent_H_Sky_18_21_10")).toBe(false);
  });
});
