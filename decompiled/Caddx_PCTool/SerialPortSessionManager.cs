using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Caddx_PCTool;

public sealed class SerialPortSessionManager : ISerialPortSessionManager
{
	private sealed class SessionEntry
	{
		public Guid SessionId;

		public long Generation;

		public SerialPortOwner Owner;

		public string PortName;

		public UsbDevInfo Device;

		public SerialPortDeviceMatcher Matcher;

		public SerialPortSessionState State;

		public ISerialPortSessionTransport Transport;

		public CancellationTokenSource Cancellation;

		public DateTime OpenedAt;
	}

	private readonly object _syncRoot = new object();

	private readonly Dictionary<Guid, SessionEntry> _sessionsById = new Dictionary<Guid, SessionEntry>();

	private readonly Dictionary<string, Guid> _sessionsByPort = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

	private readonly ISerialPortSessionTransportFactory _transportFactory;

	private readonly IUsbDeviceEnumerator _deviceEnumerator;

	private readonly TimeSpan _closeTimeout;

	private bool _isStopping;

	public SerialPortSessionManager(ISerialPortSessionTransportFactory transportFactory, IUsbDeviceEnumerator deviceEnumerator, TimeSpan closeTimeout)
	{
		_transportFactory = transportFactory ?? throw new ArgumentNullException("transportFactory");
		_deviceEnumerator = deviceEnumerator ?? throw new ArgumentNullException("deviceEnumerator");
		_closeTimeout = ((closeTimeout <= TimeSpan.Zero) ? TimeSpan.FromSeconds(1.0) : closeTimeout);
	}

	public async Task<SerialPortAcquireResult> AcquireAsync(UsbDevInfo device, SerialPortOwner owner, SerialPortSessionOpenOptions options, CancellationToken cancellationToken)
	{
		UsbDevInfo snapshot = UsbDevInfoSnapshot.Clone(device);
		if (snapshot == null || !SerialPortSessionText.TryNormalizePortName(snapshot.PortName, out var portName))
		{
			return Failure<SerialPortAcquireResult>(SerialPortSessionFailureReason.InvalidPortName);
		}
		if (!SerialPortDeviceMatcher.TryCreate(snapshot, out var matcher))
		{
			return Failure<SerialPortAcquireResult>(SerialPortSessionFailureReason.InvalidDevice);
		}
		if (options == null || options.BaudRate <= 0)
		{
			return Failure<SerialPortAcquireResult>(SerialPortSessionFailureReason.InvalidOptions);
		}
		if (cancellationToken.IsCancellationRequested)
		{
			return Failure<SerialPortAcquireResult>(SerialPortSessionFailureReason.Cancelled);
		}
		SessionEntry entry;
		lock (_syncRoot)
		{
			if (_isStopping)
			{
				return Failure<SerialPortAcquireResult>(SerialPortSessionFailureReason.ManagerStopping);
			}
			if (TryGetEntryByPortLocked(portName, out var occupied))
			{
				LogSession(occupied, "AcquireRejectedBusy");
				return Failure<SerialPortAcquireResult>(SerialPortSessionFailureReason.Busy, occupied);
			}
			entry = new SessionEntry
			{
				SessionId = Guid.NewGuid(),
				Generation = 1L,
				Owner = owner,
				PortName = portName,
				Device = snapshot,
				Matcher = matcher,
				State = SerialPortSessionState.Reserved,
				Cancellation = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[1] { cancellationToken })
			};
			_sessionsById.Add(entry.SessionId, entry);
			_sessionsByPort.Add(entry.PortName, entry.SessionId);
			LogSession(entry, "AcquireReserved");
		}
		ISerialPortSessionTransport transport = null;
		bool opened;
		try
		{
			Tuple<ISerialPortSessionTransport, bool> openResult = await Task.Run(delegate
			{
				ISerialPortSessionTransport serialPortSessionTransport = _transportFactory.Create(options.TransportKind);
				if (!serialPortSessionTransport.Open(portName, UsbDevInfoSnapshot.Clone(snapshot), options.BaudRate))
				{
					serialPortSessionTransport.Dispose();
					return Tuple.Create<ISerialPortSessionTransport, bool>(null, item2: false);
				}
				return Tuple.Create(serialPortSessionTransport, item2: true);
			}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			transport = openResult.Item1;
			opened = openResult.Item2;
		}
		catch (OperationCanceledException)
		{
			return await CancelAcquireAsync(entry, transport).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch
		{
			opened = false;
		}
		lock (_syncRoot)
		{
			if (opened && IsCurrentLocked(entry, entry.Generation, SerialPortSessionState.Reserved))
			{
				entry.Transport = transport;
				entry.State = SerialPortSessionState.Open;
				entry.OpenedAt = DateTime.UtcNow;
				LogSession(entry, "OpenSucceeded");
				return new SerialPortAcquireResult
				{
					Succeeded = true,
					Handle = CreateHandleLocked(entry),
					Snapshot = CreateSnapshotLocked(entry)
				};
			}
			if (IsCurrentLocked(entry, entry.Generation, SerialPortSessionState.Reserved))
			{
				_sessionsById.Remove(entry.SessionId);
				_sessionsByPort.Remove(entry.PortName);
				entry.State = SerialPortSessionState.Faulted;
				LogSession(entry, "OpenFailed");
			}
		}
		CloseTransportSafely(transport);
		return Failure<SerialPortAcquireResult>(opened ? SerialPortSessionFailureReason.StaleHandle : SerialPortSessionFailureReason.OpenFailed);
	}

	public async Task<SerialPortReleaseResult> ReleaseAsync(SerialPortSessionHandle source, SerialPortReleaseReason reason)
	{
		if (source == null)
		{
			return Failure<SerialPortReleaseResult>(SerialPortSessionFailureReason.StaleHandle);
		}
		SessionEntry entry;
		ISerialPortSessionTransport transport;
		long closeGeneration;
		lock (_syncRoot)
		{
			if (!TryValidateHandleLocked(source, out entry))
			{
				return Failure<SerialPortReleaseResult>(SerialPortSessionFailureReason.StaleHandle);
			}
			if (entry.State != SerialPortSessionState.Open)
			{
				return Failure<SerialPortReleaseResult>(SerialPortSessionFailureReason.InvalidState, entry);
			}
			entry.Generation++;
			entry.State = SerialPortSessionState.Closing;
			entry.Cancellation.Cancel();
			transport = entry.Transport;
			closeGeneration = entry.Generation;
			LogSession(entry, "ReleaseStarted", reason.ToString());
		}
		Task closeTask = Task.Run(delegate
		{
			CloseTransportSafely(transport);
		});
		Task timeoutTask = Task.Delay(_closeTimeout);
		if (await Task.WhenAny(new Task[2] { closeTask, timeoutTask }).ConfigureAwait(continueOnCapturedContext: false) != closeTask)
		{
			lock (_syncRoot)
			{
				if (IsCurrentLocked(entry, closeGeneration, SerialPortSessionState.Closing))
				{
					entry.State = SerialPortSessionState.CloseTimedOut;
					LogSession(entry, "CloseTimedOut", reason.ToString());
					ObserveLateClose(entry, closeGeneration, closeTask, reason);
					return Failure<SerialPortReleaseResult>(SerialPortSessionFailureReason.CloseTimedOut, entry);
				}
			}
			return Failure<SerialPortReleaseResult>(SerialPortSessionFailureReason.StaleHandle);
		}
		await closeTask.ConfigureAwait(continueOnCapturedContext: false);
		lock (_syncRoot)
		{
			if (!IsCurrentLocked(entry, closeGeneration, SerialPortSessionState.Closing))
			{
				LogSession(entry, "StaleCallbackDropped", "close completion");
				return Failure<SerialPortReleaseResult>(SerialPortSessionFailureReason.StaleHandle);
			}
			RemoveEntryLocked(entry);
			entry.State = SerialPortSessionState.Closed;
			entry.Cancellation.Dispose();
			LogSession(entry, "CloseSucceeded", reason.ToString());
			return new SerialPortReleaseResult
			{
				Succeeded = true,
				Snapshot = CreateSnapshotLocked(entry)
			};
		}
	}

	public void HandleDeviceRemoved(string portName)
	{
		if (!SerialPortSessionText.TryNormalizePortName(portName, out var normalized))
		{
			return;
		}
		lock (_syncRoot)
		{
			if (!TryGetEntryByPortLocked(normalized, out var entry) || (entry.State != SerialPortSessionState.Open && entry.State != SerialPortSessionState.Reserved && entry.State != SerialPortSessionState.Opening && entry.State != SerialPortSessionState.Reconnecting && entry.State != SerialPortSessionState.Closing && entry.State != SerialPortSessionState.CloseTimedOut))
			{
				return;
			}
			entry.Generation++;
			entry.Cancellation.Cancel();
			ISerialPortSessionTransport transport = entry.Transport;
			RemoveEntryLocked(entry);
			entry.State = SerialPortSessionState.Closed;
			LogSession(entry, "DeviceRemovedForcedCleanup");
			if (transport != null)
			{
				Task.Run(delegate
				{
					CloseTransportSafely(transport);
				});
			}
		}
	}

	public bool TryGetSnapshot(string portName, out SerialPortSessionSnapshot snapshot)
	{
		snapshot = null;
		if (!SerialPortSessionText.TryNormalizePortName(portName, out var normalized))
		{
			return false;
		}
		lock (_syncRoot)
		{
			if (!TryGetEntryByPortLocked(normalized, out var entry))
			{
				return false;
			}
			snapshot = CreateSnapshotLocked(entry);
			return true;
		}
	}

	public Task StopAsync()
	{
		List<SerialPortSessionHandle> handles;
		lock (_syncRoot)
		{
			_isStopping = true;
			handles = _sessionsById.Values.Where((SessionEntry entry) => entry.State == SerialPortSessionState.Open).Select(CreateHandleLocked).ToList();
		}
		return StopSessionsAsync(handles);
	}

	private async Task StopSessionsAsync(List<SerialPortSessionHandle> handles)
	{
		IEnumerable<Task<SerialPortReleaseResult>> releases = handles.Select((SerialPortSessionHandle handle) => ReleaseAsync(handle, SerialPortReleaseReason.ApplicationStopping));
		await Task.WhenAll(releases).ConfigureAwait(continueOnCapturedContext: false);
	}

	public SerialPortTransferResult TryTransfer(SerialPortSessionHandle source, SerialPortOwner targetOwner)
	{
		if (source == null)
		{
			return Failure<SerialPortTransferResult>(SerialPortSessionFailureReason.StaleHandle);
		}
		lock (_syncRoot)
		{
			if (!_sessionsById.TryGetValue(source.SessionId, out var value))
			{
				return Failure<SerialPortTransferResult>(SerialPortSessionFailureReason.StaleHandle);
			}
			if (value.Generation != source.Generation)
			{
				return Failure<SerialPortTransferResult>(SerialPortSessionFailureReason.StaleHandle, value);
			}
			if (value.Owner != source.Owner)
			{
				return Failure<SerialPortTransferResult>(SerialPortSessionFailureReason.OwnerMismatch, value);
			}
			if (value.State != SerialPortSessionState.Open)
			{
				return Failure<SerialPortTransferResult>(SerialPortSessionFailureReason.InvalidState, value);
			}
			value.Generation++;
			value.Owner = targetOwner;
			LogSession(value, "OwnershipTransferred", source.Owner.ToString() + "->" + targetOwner);
			return new SerialPortTransferResult
			{
				Succeeded = true,
				Handle = CreateHandleLocked(value),
				Snapshot = CreateSnapshotLocked(value)
			};
		}
	}

	public Task<SerialPortReconnectResult> ReconnectByVidPidAsync(SerialPortSessionHandle source, SerialPortReconnectOptions options, CancellationToken cancellationToken)
	{
		return Task.FromResult(Failure<SerialPortReconnectResult>(SerialPortSessionFailureReason.InvalidState));
	}

	public bool TryGetTransport(SerialPortSessionHandle handle, out ISerialPortSessionTransport transport)
	{
		transport = null;
		if (handle == null)
		{
			return false;
		}
		lock (_syncRoot)
		{
			if (!TryValidateHandleLocked(handle, out var entry) || entry.State != SerialPortSessionState.Open)
			{
				return false;
			}
			transport = entry.Transport;
			return transport != null;
		}
	}

	private void ObserveLateClose(SessionEntry entry, long closeGeneration, Task closeTask, SerialPortReleaseReason reason)
	{
		closeTask.ContinueWith(delegate
		{
			lock (_syncRoot)
			{
				if (!IsCurrentLocked(entry, closeGeneration, SerialPortSessionState.CloseTimedOut))
				{
					LogSession(entry, "StaleCallbackDropped", "late close completion");
				}
				else
				{
					RemoveEntryLocked(entry);
					entry.State = SerialPortSessionState.Closed;
					entry.Cancellation.Dispose();
					LogSession(entry, "CloseSucceeded", reason.ToString() + " (late)");
				}
			}
		}, TaskScheduler.Default);
	}

	private async Task<SerialPortAcquireResult> CancelAcquireAsync(SessionEntry entry, ISerialPortSessionTransport transport)
	{
		lock (_syncRoot)
		{
			if (IsCurrentLocked(entry, entry.Generation, SerialPortSessionState.Reserved))
			{
				RemoveEntryLocked(entry);
				entry.State = SerialPortSessionState.Closed;
			}
		}
		await Task.Run(delegate
		{
			CloseTransportSafely(transport);
		}).ConfigureAwait(continueOnCapturedContext: false);
		return Failure<SerialPortAcquireResult>(SerialPortSessionFailureReason.Cancelled);
	}

	private bool TryGetEntryByPortLocked(string portName, out SessionEntry entry)
	{
		entry = null;
		if (_sessionsByPort.TryGetValue(portName, out var value))
		{
			return _sessionsById.TryGetValue(value, out entry);
		}
		return false;
	}

	private bool TryValidateHandleLocked(SerialPortSessionHandle handle, out SessionEntry entry)
	{
		entry = null;
		return _sessionsById.TryGetValue(handle.SessionId, out entry) && entry.Generation == handle.Generation && entry.Owner == handle.Owner;
	}

	private bool IsCurrentLocked(SessionEntry entry, long generation, SerialPortSessionState state)
	{
		SessionEntry value;
		return entry != null && _sessionsById.TryGetValue(entry.SessionId, out value) && value == entry && entry.Generation == generation && entry.State == state;
	}

	private void RemoveEntryLocked(SessionEntry entry)
	{
		_sessionsById.Remove(entry.SessionId);
		if (_sessionsByPort.TryGetValue(entry.PortName, out var value) && value == entry.SessionId)
		{
			_sessionsByPort.Remove(entry.PortName);
		}
	}

	private static SerialPortSessionHandle CreateHandleLocked(SessionEntry entry)
	{
		return new SerialPortSessionHandle(entry.SessionId, entry.Generation, entry.Owner, entry.PortName, entry.Matcher?.Vid, entry.Matcher?.Pid);
	}

	private static SerialPortSessionSnapshot CreateSnapshotLocked(SessionEntry entry)
	{
		return new SerialPortSessionSnapshot
		{
			SessionId = entry.SessionId,
			Generation = entry.Generation,
			Owner = entry.Owner,
			State = entry.State,
			PortName = entry.PortName,
			Vid = entry.Matcher?.Vid,
			Pid = entry.Matcher?.Pid,
			OpenedAt = entry.OpenedAt
		};
	}

	private static T Failure<T>(SerialPortSessionFailureReason reason, SessionEntry entry = null) where T : SerialPortSessionResult, new()
	{
		return new T
		{
			Succeeded = false,
			FailureReason = reason,
			CurrentOwner = entry?.Owner,
			CurrentState = entry?.State,
			Snapshot = ((entry == null) ? null : CreateSnapshotLocked(entry))
		};
	}

	private static void CloseTransportSafely(ISerialPortSessionTransport transport)
	{
		if (transport == null)
		{
			return;
		}
		try
		{
			transport.Close();
		}
		finally
		{
			transport.Dispose();
		}
	}

	private static void LogSession(SessionEntry entry, string eventName, string reason = null)
	{
		if (entry != null)
		{
			WriteLog.WriteLogFileToUI($"[PortSession] session={entry.SessionId}, generation={entry.Generation}, owner={entry.Owner}, " + string.Format("state={0}, port={1}, vid={2}, pid={3}, ", new object[4]
			{
				entry.State,
				entry.PortName,
				entry.Matcher?.Vid,
				entry.Matcher?.Pid
			}) + "event=" + eventName + ", reason=" + reason, Color.DarkCyan);
		}
	}
}
