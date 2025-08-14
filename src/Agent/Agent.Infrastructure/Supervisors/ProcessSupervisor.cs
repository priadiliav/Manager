using Agent.Application.Abstractions;

namespace Agent.Infrastructure.Supervisors;

public class ProcessSupervisor : IProcessSupervisor
{
  public void KillProcess(int processId)
  {
    throw new NotImplementedException();
  }

  public void KillProcess(string processName)
  {
    throw new NotImplementedException();
  }

  public async Task StartProcessWatcherAsync(Func<string, int, Task> onProcessStarted, CancellationToken cancellationToken = default)
  {
    while (!cancellationToken.IsCancellationRequested)
    {
      await Task.Delay(1000, cancellationToken); // Simulate delay for process checking

      // Example of invoking the callback with dummy data
      await onProcessStarted("DummyProcess", 1234);
    }
  }
}
