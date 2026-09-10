using System;
using System.Net;

namespace Caddx_PCTool.Update;

public static class UpdateErrorClassifier
{
	public static UpdateFailureKind Classify(Exception ex)
	{
		if (ex == null)
		{
			return UpdateFailureKind.Unknown;
		}
		if (ex is OperationCanceledException)
		{
			return UpdateFailureKind.Cancelled;
		}
		if (ex is TimeoutException)
		{
			return UpdateFailureKind.Network;
		}
		if (ex is WebException ex2)
		{
			if (ex2.Response is HttpWebResponse httpWebResponse)
			{
				return ClassifyStatus((int)httpWebResponse.StatusCode);
			}
			return UpdateFailureKind.Network;
		}
		string text = ex.Message ?? string.Empty;
		if (text.Contains("403"))
		{
			return UpdateFailureKind.Forbidden;
		}
		if (text.Contains("429"))
		{
			return UpdateFailureKind.RateLimited;
		}
		if (text.Contains("500") || text.Contains("502") || text.Contains("503"))
		{
			return UpdateFailureKind.Server;
		}
		if (text.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("network", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return UpdateFailureKind.Network;
		}
		return UpdateFailureKind.Unknown;
	}

	private static UpdateFailureKind ClassifyStatus(int status)
	{
		if (status == 403)
		{
			return UpdateFailureKind.Forbidden;
		}
		if (status == 429)
		{
			return UpdateFailureKind.RateLimited;
		}
		if (status >= 500)
		{
			return UpdateFailureKind.Server;
		}
		return UpdateFailureKind.Network;
	}
}
