using Agent.Application.Abstractions;
using Stateless;

namespace Agent.Application.States;

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
    public StateMachine<RunnerState, RunnerTrigger> Machine { get; }

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

      Machine = new StateMachine<RunnerState, RunnerTrigger>(RunnerState.Idle);

      ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
      Machine.Configure(RunnerState.Idle)
          .Permit(RunnerTrigger.Start, RunnerState.Working);

      Machine.Configure(RunnerState.Working)
          .OnEntryAsync(WorkAsync)
          .Permit(RunnerTrigger.Success, RunnerState.Idle)
          .Permit(RunnerTrigger.ErrorOccured, RunnerState.Error);

        Machine.Configure(RunnerState.Error)
            .Permit(RunnerTrigger.Start, RunnerState.Working)
            .OnEntryAsync(async () =>
            {
              await Task.Delay(_workerRunner.Interval, _token);
              await Machine.FireAsync(RunnerTrigger.Start);
            });
    }

    public async Task RunAsync()
    {
        while (!_token.IsCancellationRequested)
        {
            if (Machine.State == RunnerState.Idle)
                await _wrapper.FireAsync(Machine, RunnerTrigger.Start);

            await Task.Delay(_workerRunner.Interval, _token);
        }
    }

  private async Task WorkAsync()
  {
    try
    {
      await _workerRunner.RunAsync(_token);

      await _wrapper.FireAsync(Machine, RunnerTrigger.Success);
    }
    catch (Exception ex)
    {
      await _wrapper.FireAsync(Machine, RunnerTrigger.ErrorOccured);
    }
  }
}
