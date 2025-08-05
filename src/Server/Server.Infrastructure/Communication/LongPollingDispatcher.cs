using System.Collections.Concurrent;
using Server.Application.Abstractions;

namespace Server.Infrastructure.Communication;

public class LongPollingDispatcher<TKey, T> : ILongPollingDispatcher<TKey, T> 
		where TKey : notnull 
		where T : notnull
{
	private readonly ConcurrentDictionary<TKey, TaskCompletionSource<T?>> _waitingClients = new();
	
	public Task<T?> WaitForUpdateAsync(TKey key, CancellationToken cancellationToken)
	{
		var tcs = new TaskCompletionSource<T?>(TaskCreationOptions.RunContinuationsAsynchronously);

		_waitingClients.AddOrUpdate(key, tcs, (_, old) =>
		{
			old.TrySetResult(default);
			return tcs;
		});

		cancellationToken.Register(() =>
		{
			_waitingClients.TryRemove(key, out _);
			tcs.TrySetResult(default);
		});

		return tcs.Task;
	}
	
	public void NotifyUpdateForKey(TKey key, T update)
	{
		if (_waitingClients.TryRemove(key, out var tcs))
		{
			tcs.TrySetResult(update);
		}
	}

	public void NotifyUpdate(T update)
	{
		foreach (var client in _waitingClients.Values)
		{
			client.TrySetResult(update);
		}
		_waitingClients.Clear();
	}

	public void RemoveSubscriber(TKey key)
		=> _waitingClients.TryRemove(key, out _);

	public int GetSubscriberCount(TKey key)
		=> _waitingClients.TryGetValue(key, out var tcs) ? (tcs.Task.IsCompleted ? 0 : 1) : 0;
}