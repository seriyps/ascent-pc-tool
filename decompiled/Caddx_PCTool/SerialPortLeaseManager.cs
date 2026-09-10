using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Caddx_PCTool;

public static class SerialPortLeaseManager
{
	private sealed class LeaseState
	{
		public string Owner { get; }

		public Guid LeaseId { get; }

		public LeaseState(string owner, Guid leaseId)
		{
			Owner = owner;
			LeaseId = leaseId;
		}
	}

	private static readonly ConcurrentDictionary<string, LeaseState> Leases = new ConcurrentDictionary<string, LeaseState>(StringComparer.OrdinalIgnoreCase);

	public static bool TryAcquire(string portName, string owner, out SerialPortLease lease, out string currentOwner)
	{
		lease = null;
		currentOwner = null;
		string text = NormalizePortName(portName);
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		string owner2 = (string.IsNullOrWhiteSpace(owner) ? "Unknown" : owner.Trim());
		Guid leaseId = Guid.NewGuid();
		LeaseState value = new LeaseState(owner2, leaseId);
		if (Leases.TryAdd(text, value))
		{
			lease = new SerialPortLease(text, owner2, leaseId);
			return true;
		}
		if (Leases.TryGetValue(text, out var value2))
		{
			currentOwner = value2.Owner;
		}
		return false;
	}

	public static bool IsOccupied(string portName, out string currentOwner)
	{
		currentOwner = null;
		string text = NormalizePortName(portName);
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (Leases.TryGetValue(text, out var value))
		{
			currentOwner = value.Owner;
			return true;
		}
		return false;
	}

	public static IReadOnlyDictionary<string, string> Snapshot()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (KeyValuePair<string, LeaseState> lease in Leases)
		{
			dictionary[lease.Key] = lease.Value.Owner;
		}
		return dictionary;
	}

	internal static void ClearForTest()
	{
		Leases.Clear();
	}

	internal static void Release(string portName, Guid leaseId)
	{
		string text = NormalizePortName(portName);
		if (!string.IsNullOrEmpty(text) && Leases.TryGetValue(text, out var value) && value.LeaseId == leaseId)
		{
			Leases.TryRemove(text, out var _);
		}
	}

	private static string NormalizePortName(string portName)
	{
		return string.IsNullOrWhiteSpace(portName) ? null : portName.Trim().ToUpperInvariant();
	}
}
