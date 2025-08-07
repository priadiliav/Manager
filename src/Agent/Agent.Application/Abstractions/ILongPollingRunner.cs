namespace Agent.Application.Abstractions;

public interface ILongPollingRunner
{
  Task StartListeningAsync (CancellationToken cancellationToken);
}
