using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Application.Utils;
using Microsoft.Extensions.Logging;

namespace Agent.Application.States;

public class MetricWorkerStateMachine(
  ILogger<MetricWorkerStateMachine> logger,
  StateMachineWrapper wrapper,
  IMetricService metricService,
  IConfigurationRepository configurationRepository)
    : WorkerStateMachine(logger, wrapper, nameof(MetricWorkerStateMachine))
{
  #region Getters
  protected override async Task<TimeSpan> GetIntervalAsync()
  {
    var cfg = await configurationRepository.GetAsync();
    return TimeSpan.FromSeconds(cfg.MetricPublishIntervalSeconds);
  }

  protected override async Task<int> GetRetryCountAsync()
  {
    var cfg = await configurationRepository.GetAsync();
    return cfg.MetricPublishRetryCount;
  }

  protected override async Task<TimeSpan> GetRetryDelayAsync()
  {
    var cfg = await configurationRepository.GetAsync();
    return TimeSpan.FromSeconds(cfg.MetricPublishRetryDelaySeconds);
  }
  #endregion

  protected override Task DoWorkAsync(CancellationToken cancellationToken)
    => metricService.CollectAndPublishMetricsAsync();
}
