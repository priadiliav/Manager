namespace Agent.Application.Abstractions;

public interface ILongPollingClient<out TResponse>
{
  Task ListenAsync(Func<TResponse, Task> handleMessage, CancellationToken cancellationToken);
}
