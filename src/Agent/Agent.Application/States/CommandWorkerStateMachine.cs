using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Application.States.Workers;
using Agent.Application.Utils;
using Microsoft.Extensions.Logging;

namespace Agent.Application.States;

public class CommandWorkerStateMachine(
    ILogger<CommandWorkerStateMachine> logger,
    StateMachineWrapper wrapper,
    ICommandService commandService,
    IConfigurationRepository configurationRepository)
    : WorkerStateMachine(logger, wrapper, nameof(CommandWorkerStateMachine))
{
  #region Getters
  protected override async Task<TimeSpan> GetIntervalAsync()
  {
    var cfg = await configurationRepository.GetAsync();
    return TimeSpan.FromSeconds(cfg.CommandPollIntervalSeconds);
  }

  protected override async Task<TimeSpan> GetRetryDelayAsync()
  {
    var cfg = await configurationRepository.GetAsync();
    return TimeSpan.FromSeconds(cfg.CommandExecutionRetryDelaySeconds);
  }

  protected override async Task<int> GetRetryCountAsync()
  {
    var cfg = await configurationRepository.GetAsync();
    return cfg.CommandExecutionRetryCount;
  }
  #endregion

  protected override Task HandleProcessingAsync()
    => commandService.PollAndExecuteCommandsAsync();
}
