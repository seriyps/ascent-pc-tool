namespace CaddxTool.Protocol.Tests;

// Deterministic stand-in for DateTime.Now, so retry/timeout logic can be
// exercised without actually waiting. FakeAscentTransport advances this by
// the simulated read-timeout duration whenever it reports "no data", modeling
// how a real SerialPort would have blocked for that long before giving up.
public class ManualClock
{
    public DateTime Current { get; private set; } = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Advance(TimeSpan span) => Current += span;

    public DateTime Now() => Current;
}
