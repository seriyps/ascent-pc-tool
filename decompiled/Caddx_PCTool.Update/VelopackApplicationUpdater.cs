using System;
using System.Threading;
using System.Threading.Tasks;
using Velopack;
using Velopack.Locators;
using Velopack.Sources;

namespace Caddx_PCTool.Update;

public sealed class VelopackApplicationUpdater : IApplicationUpdater
{
	private readonly VariantUpdateConfig _config;

	private readonly Action<string> _log;

	private readonly UpdateManager _manager;

	private Guid _lastCheckToken;

	public bool IsInstalled => _manager.IsInstalled;

	public string CurrentVersion => ((object)_manager.CurrentVersion)?.ToString() ?? string.Empty;

	public VelopackApplicationUpdater(VariantUpdateConfig config, Action<string> log)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		base._002Ector();
		_config = config ?? throw new ArgumentNullException("config");
		_log = log ?? ((Action<string>)delegate
		{
		});
		GithubSource val = new GithubSource(_config.UpdateUrl, (string)null, false, (IFileDownloader)null);
		_manager = new UpdateManager((IUpdateSource)(object)val, new UpdateOptions
		{
			ExplicitChannel = _config.Channel
		}, (IVelopackLocator)null);
	}

	public async Task<ApplicationUpdateInfo> CheckForUpdatesAsync(CancellationToken cancellationToken)
	{
		if (!_manager.IsInstalled)
		{
			throw new UpdateServiceException(UpdateFailureKind.NotInstalled, "当前运行环境不支持在线更新");
		}
		if (string.IsNullOrWhiteSpace(_config.UpdateUrl))
		{
			throw new UpdateServiceException(UpdateFailureKind.NotConfigured, "未配置更新地址");
		}
		try
		{
			UpdateInfo info = await _manager.CheckForUpdatesAsync().ConfigureAwait(continueOnCapturedContext: false);
			if (info == null)
			{
				return null;
			}
			_lastCheckToken = ApplicationUpdateInfo.NewToken();
			string currentVersion = CurrentVersion;
			VelopackAsset targetFullRelease = info.TargetFullRelease;
			string? target = ((targetFullRelease == null) ? null : ((object)targetFullRelease.Version)?.ToString());
			VelopackAsset targetFullRelease2 = info.TargetFullRelease;
			return new ApplicationUpdateInfo(currentVersion, target, (targetFullRelease2 != null) ? targetFullRelease2.NotesMarkdown : null, _lastCheckToken, info);
		}
		catch (Exception ex)
		{
			UpdateFailureKind kind = UpdateErrorClassifier.Classify(ex);
			_log($"检查更新失败：{kind}");
			throw new UpdateServiceException(kind, "检查更新失败", ex);
		}
	}

	public async Task DownloadUpdatesAsync(ApplicationUpdateInfo update, IProgress<int> progress, CancellationToken cancellationToken)
	{
		UpdateInfo vpkInfo = ValidateUpdateInfo(update);
		try
		{
			await _manager.DownloadUpdatesAsync(vpkInfo, (Action<int>)delegate(int p)
			{
				progress?.Report(p);
			}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			UpdateFailureKind kind = UpdateErrorClassifier.Classify(ex2);
			_log($"下载更新失败：{kind}");
			throw new UpdateServiceException(kind, "下载更新失败", ex2);
		}
	}

	public void WaitExitThenApplyUpdates(ApplicationUpdateInfo update)
	{
		UpdateInfo val = ValidateUpdateInfo(update);
		VelopackAsset val2 = UpdateInfo.op_Implicit(val);
		_manager.WaitExitThenApplyUpdates(val2, false, true, (string[])null);
	}

	private UpdateInfo ValidateUpdateInfo(ApplicationUpdateInfo update)
	{
		if (update == null || !update.MatchesToken(_lastCheckToken))
		{
			throw new UpdateServiceException(UpdateFailureKind.StaleResult, "更新结果无效，请重新检查");
		}
		object velopackHandle = update.VelopackHandle;
		UpdateInfo val = (UpdateInfo)((velopackHandle is UpdateInfo) ? velopackHandle : null);
		if (val == null)
		{
			throw new UpdateServiceException(UpdateFailureKind.StaleResult, "更新句柄无效");
		}
		return val;
	}
}
