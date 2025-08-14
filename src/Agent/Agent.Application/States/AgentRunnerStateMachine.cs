namespace Agent.Application.States;

using Microsoft.Extensions.Logging;
using Stateless;

public enum RunnerState
{
    Idle,
    Working,
    Error,
    Paused
}

public enum RunnerTrigger
{
    StartWork,
    WorkDone,
    WorkFailed,
    Pause,
    Resume
}

public class RunnerStateMachine
{
    private readonly ILogger<RunnerStateMachine> _logger;

    private readonly Func<Task> _workAction;
    private readonly TimeSpan _interval;
    private readonly CancellationToken _token;

    private readonly StateMachine<RunnerState, RunnerTrigger> _machine;

    public RunnerState CurrentState => _machine.State;

    public RunnerStateMachine(
        ILogger<RunnerStateMachine> logger,
        Func<Task> workAction,
        TimeSpan interval,
        CancellationToken token)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _workAction = workAction ?? throw new ArgumentNullException(nameof(workAction));
        _interval = interval;
        _token = token;

        _machine = new StateMachine<RunnerState, RunnerTrigger>(RunnerState.Idle);

        ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
        _machine.Configure(RunnerState.Idle)
            .Permit(RunnerTrigger.StartWork, RunnerState.Working)
            .Permit(RunnerTrigger.Pause, RunnerState.Paused);

        _machine.Configure(RunnerState.Working)
            .OnEntryAsync(DoWorkAsync)
            .Permit(RunnerTrigger.WorkDone, RunnerState.Idle)
            .Permit(RunnerTrigger.WorkFailed, RunnerState.Error)
            .Permit(RunnerTrigger.Pause, RunnerState.Paused);

        _machine.Configure(RunnerState.Error)
            .Permit(RunnerTrigger.StartWork, RunnerState.Working)
            .OnEntryAsync(async () =>
            {
              _logger.LogError("An error occurred while processing work. Retrying in 5 seconds...");
              await Task.Delay(TimeSpan.FromSeconds(5), _token);
              await _machine.FireAsync(RunnerTrigger.StartWork);
            });

        _machine.Configure(RunnerState.Paused)
            .Permit(RunnerTrigger.Resume, RunnerState.Idle);
    }

    public async Task RunAsync()
    {
        while (!_token.IsCancellationRequested)
        {
            if (_machine.State == RunnerState.Idle)
                await _machine.FireAsync(RunnerTrigger.StartWork);

            await Task.Delay(_interval, _token);
        }
    }

    private async Task DoWorkAsync()
    {
        _logger.LogInformation("Starting work in state: {State}", _machine.State);

        try
        {
            await _workAction();
            await _machine.FireAsync(RunnerTrigger.WorkDone);
        }
        catch (Exception ex)
        {
            await _machine.FireAsync(RunnerTrigger.WorkFailed);
        }
    }
}
