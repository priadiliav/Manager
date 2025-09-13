using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Stateless;

namespace Agent.Application.States;

public enum AgentWorkState
{
  Idle,
  Processing,
  Finished,
  Error
}

public enum WorkTrigger
{
  Start,
  Success,
  ErrorOccurred
}

public class WorkStateMachine
{
  private readonly StateMachine<AgentWorkState, WorkTrigger> _machine;
  private readonly StateMachineWrapper _wrapper;
  public AgentWorkState CurrentState => _machine.State;

  private readonly List<RunnerStateMachine> _runnerMachines = [];

  public WorkStateMachine(
      StateMachineWrapper wrapper,
      IEnumerable<IWorkerRunner> runners,
      AgentStateContext context)
  {
    _machine = new StateMachine<AgentWorkState, WorkTrigger>(AgentWorkState.Idle);
    _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
    _machine.Configure(AgentWorkState.Idle)
        .Permit(WorkTrigger.Start, AgentWorkState.Processing);

    _machine.Configure(AgentWorkState.Processing)
        .Permit(WorkTrigger.Success, AgentWorkState.Finished)
        .Permit(WorkTrigger.ErrorOccurred, AgentWorkState.Error);

    _machine.Configure(AgentWorkState.Processing)
        .Permit(WorkTrigger.ErrorOccurred, AgentWorkState.Error);

    foreach (var runner in runners)
      _runnerMachines.Add(new RunnerStateMachine(wrapper, runner,
          context.CancellationTokenSource.Token));
  }

  public async Task StartAsync()
  {
    var tasks = _runnerMachines.Select(rm => rm.RunAsync()).ToList();
    await Task.WhenAll(tasks);
  }
}
