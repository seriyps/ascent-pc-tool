namespace CaddxTool.Protocol.Tests;

// IProgress<T>.Report is synchronous; Progress<T> only guarantees eventual
// delivery via the captured SynchronizationContext, which is flaky in unit
// tests (none is set up). This records reports the instant they happen.
public class SyncProgress<T> : IProgress<T>
{
    public List<T> Reports { get; } = new();

    public void Report(T value) => Reports.Add(value);
}
