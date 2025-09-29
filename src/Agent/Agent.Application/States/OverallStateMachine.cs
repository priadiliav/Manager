using Agent.Application.States.Workers;
using Agent.Application.Utils;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum OverallState
{
  Idle,
  Authenticating,
  Synchronizing,
  Running,
  Stopping,
  Error
}

public enum OverallTrigger
{
  Start,
  Synchronize,
  Run,
  Stop,
  ErrorOccurred
}

public class OverallStateMachine
{
  private readonly StateMachine<OverallState, OverallTrigger> _machine;
  private readonly ILogger<OverallStateMachine> _logger;

  private readonly AuthStateMachine _authMachine;
  private readonly SyncStateMachine _syncMachine;
  private readonly SupervisorStateMachine _supervisorMachine;

  public OverallStateMachine(
    ILogger<OverallStateMachine> logger,
    StateMachineWrapper wrapper,
    AuthStateMachine authMachine,
    SyncStateMachine syncMachine,
    SupervisorStateMachine supervisorMachine)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _authMachine = authMachine ?? throw new ArgumentNullException(nameof(authMachine));
    _syncMachine = syncMachine ?? throw new ArgumentNullException(nameof(syncMachine));
    _supervisorMachine = supervisorMachine ?? throw new ArgumentNullException(nameof(supervisorMachine));

    // Initialize state machine
    _machine = new StateMachine<OverallState, OverallTrigger>(OverallState.Idle);

    // Configure state machine
    ConfigureAsync();

    // Register state machine with the wrapper
    wrapper.RegisterMachine(_machine, nameof(OverallStateMachine));
  }

  private void ConfigureAsync()
  {
    _machine.Configure(OverallState.Idle)
        .Permit(OverallTrigger.Start, OverallState.Authenticating);

    _machine.Configure(OverallState.Authenticating)
        .OnEntryAsync(HandleAuthenticatingAsync)
        .Permit(OverallTrigger.Synchronize, OverallState.Synchronizing)
        .Permit(OverallTrigger.ErrorOccurred, OverallState.Error)
        .Permit(OverallTrigger.Stop, OverallState.Stopping);

    _machine.Configure(OverallState.Synchronizing)
        .OnEntryAsync(HandleSynchronizingAsync)
        .Permit(OverallTrigger.Run, OverallState.Running)
        .Permit(OverallTrigger.ErrorOccurred, OverallState.Error)
        .Permit(OverallTrigger.Stop, OverallState.Stopping);

    _machine.Configure(OverallState.Running)
        .OnEntryAsync(HandleRunningAsync)
        .Permit(OverallTrigger.ErrorOccurred, OverallState.Error)
        .Permit(OverallTrigger.Stop, OverallState.Stopping);

    _machine.Configure(OverallState.Stopping)
        .OnEntryAsync(HandleStoppingAsync)
        .Permit(OverallTrigger.Stop, OverallState.Idle);

    _machine.Configure(OverallState.Error)
        .OnEntryAsync(HandleErrorAsync)
        .Permit(OverallTrigger.Start, OverallState.Idle);
  }

  public async Task StartAsync()
    => await StateMachineWrapper.FireAsync(_machine, OverallTrigger.Start);
  public async Task StopAsync()
    => await StateMachineWrapper.FireAsync(_machine, OverallTrigger.Stop);

  #region Handlers
  /// <summary>
  /// Handles the Synchronizing state by starting the synchronization process.
  /// On success, it triggers the Run transition; on error, it triggers the Error
  /// </summary>
  private async Task HandleSynchronizingAsync()
  {
    await _syncMachine.StartAsync();

    if (_syncMachine.CurrentState is SyncState.Error)
    {
      await _machine.FireAsync(OverallTrigger.ErrorOccurred);
    }
    else
    {
      await _machine.FireAsync(OverallTrigger.Run);
    }
  }

  /// <summary>
  /// Handles the Authenticating state by starting the authentication process.
  /// On success, it triggers the Synchronize transition; on error, it triggers the Error
  /// </summary>
  private async Task HandleAuthenticatingAsync()
  {
    await _authMachine.StartAsync();

    if (_authMachine.CurrentState is AuthState.Error)
    {
      await StateMachineWrapper.FireAsync(_machine, OverallTrigger.ErrorOccurred);
    }
    else
    {
      await StateMachineWrapper.FireAsync(_machine, OverallTrigger.Synchronize);
    }
  }

  /// <summary>
  /// Handles the Running state by starting the work process.
  /// On success, it triggers the Stop transition; on error, it triggers the Error transition.
  /// </summary>
  private async Task HandleRunningAsync()
  {
    await _supervisorMachine.StartAsync();

    if (_supervisorMachine.CurrentState is SupervisorState.Error)
    {
      await StateMachineWrapper.FireAsync(_machine, OverallTrigger.ErrorOccurred);
    }
    else
    {
      await StateMachineWrapper.FireAsync(_machine, OverallTrigger.Stop);
    }
  }

  /// <summary>
  /// Handles the Stopping state by stopping all ongoing processes.
  /// Once all processes are stopped, it triggers the Stop transition to return to the Idle state
  /// </summary>
  private async Task HandleStoppingAsync()
  {
    if (_authMachine.CurrentState is AuthState.Processing)
      await _authMachine.StopAsync();

    if (_syncMachine.CurrentState is SyncState.Processing)
      await _syncMachine.StopAsync();

    if (_supervisorMachine.CurrentState is SupervisorState.Processing)
      await _supervisorMachine.StopAsync();

    _logger.LogError("Overall state machine entered Stop state. Transitioning to Idle state.");

    await StateMachineWrapper.FireAsync(_machine, OverallTrigger.Stop);
  }

  private async Task HandleErrorAsync()
  {
    _logger.LogError("Overall state machine entered Error state. Transitioning to Idle state.");

    await StateMachineWrapper.FireAsync(_machine, OverallTrigger.Start);
  }
  #endregion

  public Task TryToFireAsync(OverallTrigger trigger)
    => StateMachineWrapper.FireAsync(_machine, trigger);
}
