namespace Agent.Application.Abstractions;

public static class PeriodicExecutor
{
  public static async Task RunAsync(
      Func<CancellationToken, Task> action,
      TimeSpan interval,
      CancellationToken cancellationToken)
  {
    while (!cancellationToken.IsCancellationRequested)
    {
      await action(cancellationToken);
      await Task.Delay(interval, cancellationToken);
    }
  }
}
