using Agent.Application.Abstractions;
using Agent.Application.Dtos;
using Agent.Domain.Models;
using Common.Messages.Process;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

public interface IProcessService : ILongPollingRunner, IWatcherRunner
{
}

public class ProcessService : IProcessService
{
  private readonly ILongPollingClient<ProcessesMessage> _longPollingClient;
  private readonly ILogger<ProcessService> _logger;
  private readonly IProcessRepository _processRepository;
  private readonly IProcessSupervisor _processSupervisor;

  public ProcessService(
    ILongPollingClient<ProcessesMessage> longPollingClient,
    IProcessRepository processRepository,
    IProcessSupervisor processSupervisor,
    ILogger<ProcessService> logger)
  {
    _processRepository = processRepository ?? throw new ArgumentNullException(nameof(processRepository));
    _processSupervisor = processSupervisor ?? throw new ArgumentNullException(nameof(processSupervisor));
    _longPollingClient = longPollingClient ?? throw new ArgumentNullException(nameof(longPollingClient));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  #region Long polling
  private async Task HandleProcessesUpdateAsync(ProcessesMessage message)
  {
    _logger.LogInformation("Received process update: {Message}", message);

    var processesDomain = message.Processes.Select(p => p.ToDomain()).ToList();

    await _processRepository.AddRange(processesDomain);
  }

  public Task StartListeningAsync(CancellationToken cancellationToken)
    => _longPollingClient.StartListeningAsync(HandleProcessesUpdateAsync, cancellationToken);
  #endregion

  #region Watchers
  private async Task HandleProcessStartedAsync(string name, int pid)
  {
    var existingProcess = await _processRepository.GetByName(name);

    if (existingProcess is { ProcessState: ProcessState.Banned })
    {
      _processSupervisor.KillProcess(pid);

      _logger.LogWarning("Process {Name} with PID {Pid} is banned and has been killed.", name, pid);
    }
  }

  public Task StartWatchingAsync(CancellationToken cancellationToken = default)
    => _processSupervisor.StartProcessWatcherAsync(HandleProcessStartedAsync, cancellationToken);
  #endregion
}
