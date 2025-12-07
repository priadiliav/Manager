using Agent.Application.Abstractions;
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
        // Poll for commands every 5 seconds by default
        return TimeSpan.FromSeconds(cfg.CommandPollIntervalSeconds > 0 ? cfg.CommandPollIntervalSeconds : 5);
    }

    protected override Task<TimeSpan> GetRetryDelayAsync()
    {
        return Task.FromResult(TimeSpan.FromSeconds(5)); // 5 seconds retry delay
    }

    protected override Task<int> GetRetryCountAsync()
    {
        return Task.FromResult(3); // 3 retries before going to error state
    }
    #endregion

    protected override Task DoWorkAsync(CancellationToken cancellationToken)
        => commandService.PollAndExecuteCommandsAsync(cancellationToken);
}
