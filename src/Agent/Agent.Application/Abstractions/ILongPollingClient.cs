namespace Agent.Application.Abstractions;

public interface ILongPollingClient<out TResponse>
{
	Task StartListeningAsync(Func<TResponse, Task> handleMessage, CancellationToken cancellationToken);
}
