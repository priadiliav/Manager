namespace Agent.Application.Abstractions;

public interface ILongPollingRunner
{
  Task ListenOnceAsync(CancellationToken cancellationToken);
}
