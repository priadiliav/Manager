using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Application.Utils;
using Microsoft.Extensions.Logging;

namespace Agent.Application.States;

public class SyncWorkerStateMachine(
    ILogger<SyncWorkerStateMachine> logger,
    StateMachineWrapper wrapper,
    ISyncService syncService,
    IConfigurationRepository configurationRepository)
    : WorkerStateMachine(logger, wrapper, nameof(SyncWorkerStateMachine))
{
  #region Getters
  protected override async Task<TimeSpan> GetIntervalAsync()
  {
    var cfg = await configurationRepository.GetAsync();
    return TimeSpan.FromSeconds(cfg.SyncPollIntervalSeconds);
  }

  protected override async Task<TimeSpan> GetRetryDelayAsync()
  {
    var cfg = await configurationRepository.GetAsync();
    return TimeSpan.FromSeconds(cfg.SyncExecutionRetryDelaySeconds);
  }

  protected override async Task<int> GetRetryCountAsync()
  {
    var cfg = await configurationRepository.GetAsync();
    return cfg.SyncExecutionRetryCount;
  }
  #endregion

  protected override Task DoWorkAsync(CancellationToken cancellationToken)
    => syncService.SyncAsync(isInitial: false);
}
