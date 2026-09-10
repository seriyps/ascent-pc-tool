using System;

namespace Caddx_PCTool;

public static class SerialPortSessionText
{
	public static string NormalizePortName(string value)
	{
		TryNormalizePortName(value, out var normalized);
		return normalized;
	}

	public static bool TryNormalizePortName(string value, out string normalized)
	{
		normalized = null;
		if (string.IsNullOrWhiteSpace(value))
		{
			return false;
		}
		string text = value.Trim();
		if (text.StartsWith("\\\\.\\", StringComparison.OrdinalIgnoreCase))
		{
			text = text.Substring(4);
		}
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}
		normalized = text.ToUpperInvariant();
		return true;
	}
}
