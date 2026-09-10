using System;

namespace Caddx_PCTool.Update;

public sealed class ApplicationUpdateInfo
{
	public string CurrentVersion { get; }

	public string TargetVersion { get; }

	public string ReleaseNotes { get; }

	public Guid CheckToken { get; }

	public object VelopackHandle { get; }

	public ApplicationUpdateInfo(string current, string target, string notes, Guid token, object handle)
	{
		CurrentVersion = current;
		TargetVersion = target;
		ReleaseNotes = notes;
		CheckToken = token;
		VelopackHandle = handle;
	}

	public static Guid NewToken()
	{
		return Guid.NewGuid();
	}

	public bool MatchesToken(Guid expected)
	{
		return expected != Guid.Empty && expected == CheckToken;
	}
}
