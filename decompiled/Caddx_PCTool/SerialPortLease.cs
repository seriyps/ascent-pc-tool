using System;

namespace Caddx_PCTool;

public sealed class SerialPortLease : IDisposable
{
	private readonly Guid _leaseId;

	private bool _disposed;

	public string PortName { get; }

	public string Owner { get; }

	public bool IsDisposed => _disposed;

	internal SerialPortLease(string portName, string owner, Guid leaseId)
	{
		PortName = portName;
		Owner = owner;
		_leaseId = leaseId;
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			SerialPortLeaseManager.Release(PortName, _leaseId);
		}
	}
}
