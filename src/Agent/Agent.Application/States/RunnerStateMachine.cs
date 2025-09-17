using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Stateless;

namespace Agent.Application.States;

public enum RunnerState
{
    Idle,
    Processing,
    Finishing,
    Stopping,
    Error
}

public enum RunnerTrigger
{
    Start,
    Success,
    Stop,
    ErrorOccured
}

public class RunnerStateMachine
{
    public StateMachine<RunnerState, RunnerTrigger> Machine { get; }

    private readonly IWorkerRunner _workerRunner;
    private readonly AgentStateContext _context;
    private readonly StateMachineWrapper _wrapper;

    public RunnerStateMachine(
        StateMachineWrapper wrapper,
        IWorkerRunner workerRunner,
        AgentStateContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
      _workerRunner = workerRunner ?? throw new ArgumentNullException(nameof(workerRunner));

      Machine = new StateMachine<RunnerState, RunnerTrigger>(RunnerState.Idle);

      ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
      Machine.Configure(RunnerState.Idle)
          .Permit(RunnerTrigger.Start, RunnerState.Processing);

      Machine.Configure(RunnerState.Processing)
          .OnEntryAsync(WorkAsync)
          .Permit(RunnerTrigger.Success, RunnerState.Finishing)
          .Permit(RunnerTrigger.ErrorOccured, RunnerState.Error);

      Machine.Configure(RunnerState.Finishing)
          .OnEntryAsync(async () =>
          {
            await Task.Delay(_workerRunner.Interval, _context.CancellationTokenSource.Token);
            await Machine.FireAsync(RunnerTrigger.Start);
          })
          .Permit(RunnerTrigger.Start, RunnerState.Processing);

      Machine.Configure(RunnerState.Error)
          .Permit(RunnerTrigger.Start, RunnerState.Processing)
          .OnEntryAsync(async () =>
          {
            await Task.Delay(_workerRunner.Interval, _context.CancellationTokenSource.Token);
            await Machine.FireAsync(RunnerTrigger.Start);
          });
    }

    public async Task RunAsync()
    {
      if (Machine.State == RunnerState.Idle)
          await _wrapper.FireAsync(Machine, RunnerTrigger.Start);
    }

    private async Task WorkAsync()
    {
      try
      {
        await _workerRunner.RunAsync(_context.CancellationTokenSource.Token);

        await _wrapper.FireAsync(Machine, RunnerTrigger.Success);
      }
      catch (Exception ex)
      {
        _context.DetailsMessage = ex.Message;
        await _wrapper.FireAsync(Machine, RunnerTrigger.ErrorOccured);
      }
    }
}
