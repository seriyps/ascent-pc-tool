using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Caddx_PCTool;

public static class AscentDeviceNameResolver
{
	private static string ConsumerVersionMarker = "_C";

	public static HashSet<string> ConsumerAllowedDeviceNameKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ascent_lite", "ascent_lite_plus", "ascent_vrx", "ascent_vrx_pro", "caddxsimgm", "caddx_gm3", "caddx_gm1" };

	public static string ResolveDisplayName(ResAscentInfo info)
	{
		if (info == null)
		{
			return null;
		}
		string devNameKey = Normalize(info.DevName);
		if (IsConsumerVersion(Program.SoftwareVersion))
		{
			if (ShouldResolveFromVersion(devNameKey))
			{
				return ResolveFromVersionFields(info);
			}
			if (!IsConsumerDeviceNameAllowed(devNameKey))
			{
				return "";
			}
			return ResolveFromDeviceName(info.DevName);
		}
		if (!string.IsNullOrWhiteSpace(info.DevName))
		{
			string text = ResolveFromDeviceName(info.DevName);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		return ResolveFromVersionFields(info);
	}

	public static bool IsConsumerVersion(string softwareVersion)
	{
		return !string.IsNullOrWhiteSpace(softwareVersion) && !string.IsNullOrWhiteSpace(ConsumerVersionMarker) && softwareVersion.IndexOf(ConsumerVersionMarker, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	private static string ResolveFromVersionFields(ResAscentInfo info)
	{
		string text = ResolveFromFirmwareVersion(info.FWVers);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		if (!string.IsNullOrWhiteSpace(info.HWVers))
		{
			string text2 = ResolveFromHardwareVersion(info.HWVers, info.FWVers);
			if (!string.IsNullOrEmpty(text2))
			{
				return text2;
			}
		}
		return null;
	}

	private static bool ShouldResolveFromVersion(string devNameKey)
	{
		return string.IsNullOrEmpty(devNameKey) || devNameKey == "ascent";
	}

	private static bool IsConsumerDeviceNameAllowed(string devNameKey)
	{
		return ConsumerAllowedDeviceNameKeys != null && ConsumerAllowedDeviceNameKeys.Contains(devNameKey);
	}

	public static string ResolveFromDeviceName(string devName)
	{
		string text = Normalize(devName);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (text.Contains("z40") || text.Contains("z8") || text.Contains("hub"))
		{
			text = devName.ToLower();
		}
		else if (text.Contains("ascent_lite") && text.Length > 13)
		{
			text = (text.Contains("ascent_lite_plus") ? "ascent_lite_plus" : "ascent_lite");
		}
		else if (text.Contains("yohd_micro") || text.Contains("ascent_rc"))
		{
			text = "yohd_micro";
		}
		switch (text)
		{
		case "ascent_vrx_cine":
			return "Ascent VRX Cine";
		case "ascent_vrx_max":
			return "Ascent VRX Max";
		case "ascent_vrx_max_hf":
			return "Ascent VRX Max HF";
		case "ascent_vrx_max_wf":
			return "Ascent VRX Max WF";
		case "ascent_vrx":
			return "Ascent VRX";
		case "cx485_pro":
		case "ascent_vrx_pro":
			return "Ascent VRX Pro";
		case "ascent_gt_pro_z40":
			return "Ascent GT Pro Z40";
		case "ascent_gt_pro_z8":
			return "Ascent GT Pro Z8";
		case "ascent_gt_pro_hub":
			return "Ascent GT Pro Hub";
		case "ascent_lite_plus":
			return "Ascent Lite +";
		case "ascent_lite":
			return "Ascent Lite VTX";
		case "yohd_micro":
		case "ascent_rc":
			return "YoHD Micro";
		case "caddx_gm3":
			return "GM3 V2";
		case "caddx_gm2":
			return "GM2 V2";
		case "caddx_gm1":
			return "GM1 V2";
		case "ascent_gt_pro":
			return "Ascent GT Pro";
		case "ascent_gt":
			return "Ascent GT";
		case "ascent_gt_27k":
			return "Ascent GT 2.7k";
		case "ascent_gt_night":
			return "Ascent GT night";
		case "ascent_gt_ultra":
			return "Ascent GT ultra";
		case "ascent_gt_max":
			return "Ascent GT Max";
		case "ascent_gt_max_hf":
			return "Ascent GT Max HF";
		case "ascent_gt_max_wf":
			return "Ascent GT Max WF";
		case "ascent_gt_max_hf_z40":
			return "Ascent GT Max HF Z40";
		case "ascent_gt_max_wf_z40":
			return "Ascent GT Max WF Z40";
		case "ascent_gt_max_hf_z8":
			return "Ascent GT Max HF Z8";
		case "ascent_gt_max_wf_z8":
			return "Ascent GT Max WF Z8";
		case "ascent_gt_max_hf_hub":
			return "Ascent GT Max HF Hub";
		case "ascent_gt_max_wf_hub":
			return "Ascent GT Max WF Hub";
		case "caddxsimgm":
			return "SimGM";
		case "ascent_goggles":
			return "Ascent Goggles L";
		case "ascent_relay":
			return "Ascent Repeater";
		case "optical_photon_sky":
			return "Optical Photon VTX";
		case "optical_photon_sky_z40":
			return "Optical Photon Z40 VTX";
		case "optical_photon_sky_z8":
			return "Optical Photon Z8 VTX";
		case "optical_photon_gnd":
			return "Optical Photon VRX";
		case "ascent":
			return "Ascent";
		default:
			return "";
		}
	}

	public static string ResolveFromHardwareVersion(string hwVers, string fwVers)
	{
		if (string.IsNullOrWhiteSpace(hwVers))
		{
			return null;
		}
		string text = hwVers.ToLower();
		if (text.Contains("gm1"))
		{
			return "GM1_V2";
		}
		if (text.Contains("gm3"))
		{
			return "GM3_V2";
		}
		if (text.Contains("vrx-pro"))
		{
			return "Ascent VRX Pro";
		}
		if (text.Contains("vrx-max"))
		{
			return "Ascent VRX Max";
		}
		if (text.Contains("vrx-cine"))
		{
			return "Ascent VRX Cine";
		}
		if (text.Contains("ascent-relay"))
		{
			return "Ascent Repeater";
		}
		Match match = Regex.Match(hwVers, "\\d+");
		if (!match.Success)
		{
			return null;
		}
		string value = match.Value;
		if (value.Contains("485"))
		{
			string text2 = ResolveFromFirmwareVersion(fwVers);
			return (text2 == "Ascent VRX Pro") ? "Ascent VRX Pro" : "Ascent VRX";
		}
		if (value.Contains("482"))
		{
			return "Ascent Lite VTX";
		}
		if (value.Contains("472"))
		{
			return "Ascent Lite +";
		}
		if (value.Contains("492"))
		{
			return "YoHD Micro";
		}
		return null;
	}

	public static string ResolveFromFirmwareVersion(string fwVers)
	{
		if (string.IsNullOrWhiteSpace(fwVers))
		{
			return null;
		}
		string text = Normalize(fwVers).Replace('-', '_');
		if (text.Contains("ascent_vrx_pro") || text.Contains("vrx_pro"))
		{
			return "Ascent VRX Pro";
		}
		if (text.Contains("ascent_vrx_cine") || text.Contains("vrx_cine"))
		{
			return "Ascent VRX Cine";
		}
		if (text.Contains("ascent_vrx_max") || text.Contains("vrx_max"))
		{
			return "Ascent VRX Max";
		}
		if (text.Contains("ascent_vrx") || text.Contains("g_gnd"))
		{
			return "Ascent VRX";
		}
		if (text.Contains("ascent_lite_plus") || text.Contains("h_sky"))
		{
			return "Ascent Lite +";
		}
		if (text.Contains("ascent_lite") || text.Contains("h_sky"))
		{
			return "Ascent Lite VTX";
		}
		if (text.Contains("yohd_micro") || text.Contains("ascent_rc"))
		{
			return "YoHD Micro";
		}
		if (text.Contains("caddx_gm3") || text.Contains("gm3"))
		{
			return "GM3_V2";
		}
		if (text.Contains("caddx_gm1") || text.Contains("gm1"))
		{
			return "GM1_V2";
		}
		return null;
	}

	private static string Normalize(string value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
	}
}
