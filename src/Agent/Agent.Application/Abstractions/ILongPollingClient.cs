namespace Agent.Application.Abstractions;

public interface ILongPollingClient<out TResponse>
{
	Task StartListeningAsync(Func<TResponse, Task> onUpdate, CancellationToken cancellationToken);
}
