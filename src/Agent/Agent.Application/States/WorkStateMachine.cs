using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Stateless;

namespace Agent.Application.States;

public enum AgentWorkState
{
  Idle,
  Processing,
  Finishing,
  Stopping,
  Error
}

public enum WorkTrigger
{
  Start,
  Success,
  Stop,
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
        .Permit(WorkTrigger.Success, AgentWorkState.Finishing)
        .Permit(WorkTrigger.Stop, AgentWorkState.Stopping)
        .Permit(WorkTrigger.ErrorOccurred, AgentWorkState.Error);

    foreach (var runner in runners)
    {
      var runnerStateMachine = new RunnerStateMachine(
          wrapper, runner, context);
      _runnerMachines.Add(runnerStateMachine);
      wrapper.RegisterMachine(runnerStateMachine.Machine);
    }
  }

  public async Task StartAsync() =>
      await Task.WhenAll(_runnerMachines.Select(rm => rm.RunAsync()));
}
