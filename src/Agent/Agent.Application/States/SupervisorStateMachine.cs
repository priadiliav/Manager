using Agent.Application.States.Workers;
using Agent.Application.Utils;
using Stateless;

namespace Agent.Application.States;

public enum SupervisorState
{
  Idle,
  Processing,
  Stopping,
  Error
}

public enum SupervisorTrigger
{
  Start,
  Success,
  Stop,
  ErrorOccurred
}

public class SupervisorStateMachine
{
  private readonly StateMachine<SupervisorState, SupervisorTrigger> _machine;
  private readonly IEnumerable<WorkerStateMachine> _runnerMachines;

  public SupervisorStateMachine(
      IEnumerable<WorkerStateMachine> runnerMachines,
      StateMachineWrapper wrapper)
  {
    _runnerMachines = runnerMachines ?? throw new ArgumentNullException(nameof(runnerMachines));

    // Initialize state machine
    _machine = new StateMachine<SupervisorState, SupervisorTrigger>(SupervisorState.Idle);

    // Configure state machine
    ConfigureStateMachine();

    // Register state machine with the wrapper
    wrapper.RegisterMachine(_machine, nameof(SupervisorStateMachine));
  }

  private void ConfigureStateMachine()
  {
    _machine.Configure(SupervisorState.Idle)
        .Permit(SupervisorTrigger.Start, SupervisorState.Processing);

    _machine.Configure(SupervisorState.Processing)
        .OnEntryAsync(HandleProcessingAsync)
        .Permit(SupervisorTrigger.Success, SupervisorState.Stopping)
        .Permit(SupervisorTrigger.Stop, SupervisorState.Stopping)
        .Permit(SupervisorTrigger.ErrorOccurred, SupervisorState.Error);

    _machine.Configure(SupervisorState.Stopping)
        .OnEntryAsync(HandleStoppingAsync)
        .Permit(SupervisorTrigger.Start, SupervisorState.Idle);
  }

  public SupervisorState CurrentState => _machine.State;
  public async Task StartAsync()
    => await _machine.FireAsync(SupervisorTrigger.Start);
  public async Task StopAsync()
    => await _machine.FireAsync(SupervisorTrigger.Stop);

  #region Handlers
  private async Task HandleProcessingAsync()
  {
    var tasks = _runnerMachines
        .Where(runner => runner.CurrentState is WorkerState.Idle or WorkerState.Stopping)
        .Select(runner => runner.StartAsync());

    await Task.WhenAll(tasks);

    await _machine.FireAsync(SupervisorTrigger.Success);
  }


  private async Task HandleStoppingAsync()
  {
    var tasks = _runnerMachines
        .Where(runner => runner.CurrentState == WorkerState.Processing)
        .Select(runner => runner.StopAsync());

    await Task.WhenAll(tasks);

    await _machine.FireAsync(SupervisorTrigger.Start);
  }
  #endregion
}
