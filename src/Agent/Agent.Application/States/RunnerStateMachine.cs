using Agent.Application.Abstractions;

namespace Agent.Application.States;
using Stateless;

public enum RunnerState
{
    Idle,
    Working,
    Error
}

public enum RunnerTrigger
{
    Start,
    Success,
    ErrorOccured
}

public class RunnerStateMachine
{
    private readonly StateMachine<RunnerState, RunnerTrigger> _machine;
    private readonly IWorkerRunner _workerRunner;
    private readonly CancellationToken _token;
    private readonly StateMachineWrapper _wrapper;

    public RunnerStateMachine(
        StateMachineWrapper wrapper,
        IWorkerRunner workerRunner,
        CancellationToken token)
    {
      _token = token;
      _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
      _workerRunner = workerRunner ?? throw new ArgumentNullException(nameof(workerRunner));
      _machine = new StateMachine<RunnerState, RunnerTrigger>(RunnerState.Idle);

      ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
      _machine.Configure(RunnerState.Idle)
          .Permit(RunnerTrigger.Start, RunnerState.Working);

      _machine.Configure(RunnerState.Working)
          .OnEntryAsync(WorkAsync)
          .Permit(RunnerTrigger.Success, RunnerState.Idle)
          .Permit(RunnerTrigger.ErrorOccured, RunnerState.Error);

        _machine.Configure(RunnerState.Error)
            .Permit(RunnerTrigger.Start, RunnerState.Working)
            .OnEntryAsync(async () =>
            {
              await Task.Delay(_workerRunner.Interval, _token);
              await _machine.FireAsync(RunnerTrigger.Start);
            });
    }

    public async Task RunAsync()
    {
        while (!_token.IsCancellationRequested)
        {
            if (_machine.State == RunnerState.Idle)
                await _wrapper.FireAsync(_machine, RunnerTrigger.Start);

            await Task.Delay(_workerRunner.Interval, _token);
        }
    }

  private async Task WorkAsync()
  {
    try
    {
      await _workerRunner.RunAsync(_token);

      await _wrapper.FireAsync(_machine, RunnerTrigger.Success);
    }
    catch (Exception ex)
    {
      await _wrapper.FireAsync(_machine, RunnerTrigger.ErrorOccured);
    }
  }
}
