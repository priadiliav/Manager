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
  public StateMachine<AgentWorkState, WorkTrigger> Machine { get; }
  public AgentWorkState CurrentState => Machine.State;

  private readonly List<RunnerStateMachine> _runnerMachines = [];

  public WorkStateMachine(
      StateMachineWrapper wrapper,
      IEnumerable<IWorkerRunner> runners,
      AgentStateContext context)
  {
    Machine = new StateMachine<AgentWorkState, WorkTrigger>(AgentWorkState.Idle);
    Machine.Configure(AgentWorkState.Idle)
        .Permit(WorkTrigger.Start, AgentWorkState.Processing);
    Machine.Configure(AgentWorkState.Processing)
        .Permit(WorkTrigger.Success, AgentWorkState.Finished)
        .Permit(WorkTrigger.ErrorOccurred, AgentWorkState.Error);
    Machine.Configure(AgentWorkState.Processing)
        .Permit(WorkTrigger.ErrorOccurred, AgentWorkState.Error);

    foreach (var runner in runners)
    {
      var runnerMachine = new RunnerStateMachine(wrapper, runner, context.CancellationTokenSource.Token);

      _runnerMachines.Add(runnerMachine);
      wrapper.RegisterMachine(runnerMachine.Machine);
    }
  }

  public async Task StartAsync()
  {
    var tasks = _runnerMachines.Select(rm => rm.RunAsync()).ToList();
    await Task.WhenAll(tasks);
  }
}
