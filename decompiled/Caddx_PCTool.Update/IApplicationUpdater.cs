using System;
using System.Threading;
using System.Threading.Tasks;

namespace Caddx_PCTool.Update;

public interface IApplicationUpdater
{
	bool IsInstalled { get; }

	string CurrentVersion { get; }

	Task<ApplicationUpdateInfo> CheckForUpdatesAsync(CancellationToken cancellationToken);

	Task DownloadUpdatesAsync(ApplicationUpdateInfo update, IProgress<int> progress, CancellationToken cancellationToken);

	void WaitExitThenApplyUpdates(ApplicationUpdateInfo update);
}
