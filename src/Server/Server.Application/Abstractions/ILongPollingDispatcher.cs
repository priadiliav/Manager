namespace Server.Application.Abstractions;

public interface ILongPollingDispatcher<in TKey, TResponse>
		where TKey : notnull
		where TResponse : notnull
{
	Task<TResponse?> WaitForUpdateAsync(TKey key, CancellationToken cancellationToken);
	void NotifyUpdateForKey(TKey key, TResponse update);
	int GetSubscriberCount(TKey key);
	void NotifyUpdate(TResponse update);
	void RemoveSubscriber(TKey key);
}