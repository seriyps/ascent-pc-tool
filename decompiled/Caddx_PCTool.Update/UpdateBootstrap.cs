using System;
using System.Collections.Generic;

namespace Caddx_PCTool.Update;

public static class UpdateBootstrap
{
	private static readonly string[] _seedFiles = new string[1] { "SysParam.data" };

	public static IReadOnlyList<string> SeedRelativeFiles => _seedFiles;

	public static int RunSeedRecovery(string localAppDataRoot, string installDir, VariantUpdateConfig config, Action<string> log)
	{
		UpdateDataPaths updateDataPaths = new UpdateDataPaths(localAppDataRoot, config);
		return updateDataPaths.RestoreMissingSeedFiles(installDir, _seedFiles, log);
	}
}
