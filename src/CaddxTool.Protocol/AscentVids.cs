namespace CaddxTool.Protocol;

// Ported from Caddx_PCTool.VIDConst. Each Ascent product declares its own USB VID
// (not a real USB-IF-assigned vendor ID — these devices are USB gadgets on an
// embedded-Linux MCU, so the firmware is free to pick any VID/PID it wants).
// Values are lowercase hex, matching the format Linux sysfs idVendor/idProduct files use.
//
// The mapped name is VIDConst's own enum member name, not a verified retail
// product name — confirmed misleading at least once already (0x1D76 decodes as
// Ascent_GT_Pro here but was observed live on an Ascent Lite+ unit; see
// AGENTS.md's "confusing VID/product-name note"). Treat it as a hint in UI
// listings, not a substitute for the live device-info response.
public static class AscentVids
{
    public static readonly System.Collections.Generic.IReadOnlyDictionary<string, string> KnownVids =
        new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
        {
            ["1a86"] = "GM_V2", // gimbal, generic CH340 VID
            ["1d6b"] = "ASCENT_L_GND401",
            ["1d6c"] = "ASCENT_L_SKY402",
            ["1d6d"] = "ASCENT_H_SKY482",
            ["1d6e"] = "ASCENT_L_GND",
            ["1d6f"] = "ASCENT_GND_EX1",
            ["1d70"] = "ASCENT_SKY_EX1",
            ["1d71"] = "ASCENT_GND_EX2",
            ["1d72"] = "ASCENT_SKY_EX2",
            ["1d73"] = "ASCENT_GND_EX3",
            ["1d74"] = "ASCENT_SKY_EX3",
            ["1d75"] = "Ascent_VRX",
            ["1d76"] = "Ascent_GT_Pro",
            ["1d77"] = "Ascent_VRX_Pro", // confirmed live — matches the retail name here
            ["1d78"] = "Ascent_VRX_Max",
            ["1d79"] = "ASCENT_VRX_Cine",
            ["1d7a"] = "ASCENT_VRX_EX",
            ["1d7b"] = "ASCENT_VRX_EX2",
            ["1d80"] = "ASCENT_JoyStick",
            ["1d81"] = "OPV2_VTX",
            ["1d82"] = "OPV2_Z40VTX",
            ["1d83"] = "OPV2_Z8VTX",
            ["1d84"] = "OPV2_VRX",
        };
}
