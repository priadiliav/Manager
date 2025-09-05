using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum AgentWorkState
{
  Start,
  Listening,
  Processing,
  Paused,
  Error
}

public enum WorkTrigger
{
  StartListening,
  StartProcessing,
  Pause,
  Resume,
  ErrorOccurred
}

public class AgentWorkStateMachine
{
  private readonly StateMachine<AgentWorkState, WorkTrigger> _machine;
  public AgentWorkState CurrentState => _machine.State;

  private readonly List<RunnerStateMachine> _runnerMachines;
  public AgentWorkStateMachine(IEnumerable<IWorkerRunner> runners, AgentStateContext context)
  {
    _machine = new StateMachine<AgentWorkState, WorkTrigger>(AgentWorkState.Start);

    _runnerMachines = [];

    _machine.Configure(AgentWorkState.Start)
        .Permit(WorkTrigger.StartListening, AgentWorkState.Listening);

    _machine.Configure(AgentWorkState.Listening)
        .Permit(WorkTrigger.StartProcessing, AgentWorkState.Processing)
        .Permit(WorkTrigger.Pause, AgentWorkState.Paused)
        .Permit(WorkTrigger.ErrorOccurred, AgentWorkState.Error);

    _machine.Configure(AgentWorkState.Processing)
        .Permit(WorkTrigger.Pause, AgentWorkState.Paused)
        .Permit(WorkTrigger.ErrorOccurred, AgentWorkState.Error);

    _machine.Configure(AgentWorkState.Error)
        .Permit(WorkTrigger.Pause, AgentWorkState.Paused)
        .Permit(WorkTrigger.Resume, AgentWorkState.Listening);

    foreach (var runner in runners)
      _runnerMachines.Add(new RunnerStateMachine(runner,
          context.CancellationTokenSource.Token));
  }

  public async Task StartAsync()
  {
    var tasks = _runnerMachines.Select(rm => rm.RunAsync()).ToList();
    await Task.WhenAll(tasks);
  }
}
