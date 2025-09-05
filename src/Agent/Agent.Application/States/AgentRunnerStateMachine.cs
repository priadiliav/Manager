using Agent.Application.Abstractions;

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
    private readonly StateMachine<RunnerState, RunnerTrigger> _machine;
    public RunnerState CurrentState => _machine.State;

    private readonly IWorkerRunner _workerRunner;
    private readonly CancellationToken _token;

    public RunnerStateMachine(IWorkerRunner workerRunner, CancellationToken token)
    {
      _token = token;
      _workerRunner = workerRunner ?? throw new ArgumentNullException(nameof(workerRunner));
      _machine = new StateMachine<RunnerState, RunnerTrigger>(RunnerState.Idle);
      ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
        _machine.Configure(RunnerState.Idle)
            .Permit(RunnerTrigger.StartWork, RunnerState.Working)
            .Permit(RunnerTrigger.Pause, RunnerState.Paused);

        _machine.Configure(RunnerState.Working)
            .OnEntryAsync(WorkAsync)
            .Permit(RunnerTrigger.WorkDone, RunnerState.Idle)
            .Permit(RunnerTrigger.WorkFailed, RunnerState.Error)
            .Permit(RunnerTrigger.Pause, RunnerState.Paused);

        _machine.Configure(RunnerState.Error)
            .Permit(RunnerTrigger.StartWork, RunnerState.Working)
            .OnEntryAsync(async () =>
            {
              await Task.Delay(_workerRunner.Interval, _token);
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

            await Task.Delay(_workerRunner.Interval, _token);
        }
    }

  private async Task WorkAsync()
  {
    try
    {
      await _workerRunner.RunAsync(_token);

      await _machine.FireAsync(RunnerTrigger.WorkDone);
    }
    catch (Exception ex)
    {
      await _machine.FireAsync(RunnerTrigger.WorkFailed);
    }
  }
}
