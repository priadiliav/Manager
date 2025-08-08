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

  public Task StartProcessWatcherAsync(Func<string, int, Task> onProcessStarted, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
