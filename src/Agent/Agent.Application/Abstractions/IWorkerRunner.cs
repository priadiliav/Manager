namespace Agent.Application.Abstractions;

public interface IWorkerRunner
{
  TimeSpan Interval { get; }
  string Url { get; }

  Task RunAsync(CancellationToken cancellationToken = default);
}
