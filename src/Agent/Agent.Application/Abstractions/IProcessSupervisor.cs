namespace Agent.Application.Abstractions;

public interface IProcessSupervisor
{
  void KillProcess(int processId);
  void KillProcess(string processName);
  Task StartProcessWatcherAsync(Func<string, int, Task> onProcessStarted, CancellationToken cancellationToken = default);
}
