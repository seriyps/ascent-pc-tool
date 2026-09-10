using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Caddx_PCTool;

public class BlockQueue<T>
{
	private readonly Queue<T> _queue = new Queue<T>();

	private readonly int _maxSize;

	private bool _isCompleted = false;

	private readonly object _lock = new object();

	public bool IsCompleted
	{
		get
		{
			lock (_lock)
			{
				return _isCompleted && _queue.Count == 0;
			}
		}
	}

	public int Count
	{
		get
		{
			lock (_lock)
			{
				return _queue.Count;
			}
		}
	}

	public BlockQueue(int maxSize)
	{
		if (maxSize <= 0)
		{
			_maxSize = 5;
		}
		else
		{
			_maxSize = maxSize;
		}
	}

	public void Enqueue(T item)
	{
		lock (_lock)
		{
			while (_queue.Count >= _maxSize && !_isCompleted)
			{
				Monitor.Wait(_lock);
			}
			if (_isCompleted)
			{
				throw new InvalidOperationException("队列已关闭，无法继续添加元素。");
			}
			_queue.Enqueue(item);
			if (_queue.Count == 1)
			{
				Monitor.PulseAll(_lock);
			}
		}
	}

	public T Dequeue()
	{
		lock (_lock)
		{
			while (_queue.Count == 0 && !_isCompleted)
			{
				Monitor.Wait(_lock);
			}
			if (_queue.Count == 0 && _isCompleted)
			{
				throw new InvalidOperationException("队列已关闭且无元素可出队。");
			}
			T result = _queue.Dequeue();
			if (_queue.Count == _maxSize - 1)
			{
				Monitor.PulseAll(_lock);
			}
			return result;
		}
	}

	public bool TryDequeue(TimeSpan timeout, out T value)
	{
		value = default(T);
		bool result = false;
		lock (_lock)
		{
			DateTime now = DateTime.Now;
			while (_queue.Count == 0 && !_isCompleted)
			{
				TimeSpan timeSpan = DateTime.Now - now;
				if (timeSpan >= timeout)
				{
					return false;
				}
				TimeSpan timeout2 = timeout - timeSpan;
				Monitor.Wait(_lock, timeout2);
			}
			if (_queue.Count > 0)
			{
				value = _queue.Dequeue();
				result = true;
				if (_queue.Count == _maxSize - 1)
				{
					Monitor.PulseAll(_lock);
				}
			}
			return result;
		}
	}

	public bool TryDequeue(out T value)
	{
		value = default(T);
		bool result = false;
		lock (_lock)
		{
			if (_queue.Count > 0)
			{
				value = _queue.Dequeue();
				result = true;
				if (_queue.Count == _maxSize - 1)
				{
					Monitor.PulseAll(_lock);
				}
			}
			return result;
		}
	}

	public async Task<T> WaitDequeueAsync(CancellationToken token, TimeSpan? timeout = null)
	{
		Task<T> task = null;
		await Task.Run(async delegate
		{
			T result = default(T);
			bool found = false;
			lock (_lock)
			{
				while (_queue.Count == 0 && !_isCompleted)
				{
					if (timeout.HasValue)
					{
						ManualResetEvent waitHandle = new ManualResetEvent(initialState: false);
						ThreadPool.RegisterWaitForSingleObject(waitHandle, delegate(object state, bool timedOut)
						{
							if (timedOut)
							{
								Monitor.PulseAll(_lock);
							}
						}, null, timeout.Value, executeOnlyOnce: true);
						Monitor.Wait(_lock);
					}
					else
					{
						Monitor.Wait(_lock);
					}
				}
				if (_queue.Count > 0)
				{
					result = _queue.Dequeue();
					found = true;
					if (_queue.Count == _maxSize - 1)
					{
						Monitor.PulseAll(_lock);
					}
				}
			}
			if (!found && _isCompleted)
			{
			}
			return result;
		}, token);
		return await task;
	}

	public void CompleteAdding()
	{
		lock (_lock)
		{
			_isCompleted = true;
			Monitor.PulseAll(_lock);
		}
	}

	public void ClearAndClose()
	{
		lock (_lock)
		{
			_isCompleted = true;
			_queue.Clear();
			Monitor.PulseAll(_lock);
		}
	}
}
