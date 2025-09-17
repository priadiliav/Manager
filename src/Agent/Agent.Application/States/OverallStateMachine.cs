using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum AgentOverallState
{
  Idle,
  Authenticating,
  Synchronizing,
  Running,
  Stopping,
  Error
}

public enum AgentOverallTrigger
{
  Start,
  Synchronize,
  Stop,
  Run,
  ErrorOccurred
}

public class OverallStateMachine
{
  public StateMachine<AgentOverallState, AgentOverallTrigger> Machine { get; }

  private readonly ILogger<OverallStateMachine> _logger;
  private readonly StateMachineWrapper _wrapper;

  private readonly AuthStateMachine _authMachine;
  private readonly SyncStateMachine _syncMachine;
  private readonly WorkStateMachine _workMachine;

  public OverallStateMachine(
    ILogger<OverallStateMachine> logger,
    StateMachineWrapper wrapper,
    AuthStateMachine authMachine,
    SyncStateMachine syncMachine,
    WorkStateMachine workMachine)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _authMachine = authMachine ?? throw new ArgumentNullException(nameof(authMachine));
    _syncMachine = syncMachine ?? throw new ArgumentNullException(nameof(syncMachine));
    _workMachine = workMachine ?? throw new ArgumentNullException(nameof(workMachine));
    _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));

    Machine = new StateMachine<AgentOverallState, AgentOverallTrigger>(AgentOverallState.Idle);
    ConfigureAsync();
  }

  private void ConfigureAsync()
  {
    Machine.Configure(AgentOverallState.Idle)
        .Permit(AgentOverallTrigger.Start, AgentOverallState.Authenticating);

    Machine.Configure(AgentOverallState.Authenticating)
        .OnEntryAsync(HandleAuthenticationStateChangeAsync)
        .Permit(AgentOverallTrigger.Synchronize, AgentOverallState.Synchronizing)
        .Permit(AgentOverallTrigger.ErrorOccurred, AgentOverallState.Error)
        .Permit(AgentOverallTrigger.Stop, AgentOverallState.Stopping);

    Machine.Configure(AgentOverallState.Synchronizing)
        .OnEntryAsync(HandleSynchronizingStateChangeAsync)
        .Permit(AgentOverallTrigger.Run, AgentOverallState.Running)
        .Permit(AgentOverallTrigger.ErrorOccurred, AgentOverallState.Error)
        .Permit(AgentOverallTrigger.Stop, AgentOverallState.Stopping);

    Machine.Configure(AgentOverallState.Running)
        .OnEntryAsync(HandleRunningStateChangeAsync)
        .Permit(AgentOverallTrigger.ErrorOccurred, AgentOverallState.Error)
        .Permit(AgentOverallTrigger.Stop, AgentOverallState.Stopping);

    Machine.Configure(AgentOverallState.Stopping)
        .OnEntryAsync(HandleStoppingStateChangeAsync)
        .Permit(AgentOverallTrigger.Stop, AgentOverallState.Idle);

    Machine.Configure(AgentOverallState.Error)
        .OnEntryAsync(HandleErrorStateChangeAsync);
  }

  public async Task StartAsync() => await _wrapper.FireAsync(Machine, AgentOverallTrigger.Start);
  public async Task StopAsync() => await _wrapper.FireAsync(Machine, AgentOverallTrigger.Stop);

  #region Handlers
  private async Task HandleSynchronizingStateChangeAsync()
  {
    await _syncMachine.StartAsync();

    if (_syncMachine.CurrentState is AgentSyncState.Error)
    {
      await Machine.FireAsync(AgentOverallTrigger.ErrorOccurred);
    }
    else
    {
      await Machine.FireAsync(AgentOverallTrigger.Run);
    }
  }
  private async Task HandleAuthenticationStateChangeAsync()
  {
    await _authMachine.StartAsync();

    if (_authMachine.CurrentState is AgentAuthenticationState.Finishing)
    {
      await _wrapper.FireAsync(Machine, AgentOverallTrigger.Synchronize);
    }
    else
    {
      await _wrapper.FireAsync(Machine, AgentOverallTrigger.ErrorOccurred);
    }
  }
  private async Task HandleRunningStateChangeAsync()
  {
    await _workMachine.StartAsync();

    if (_workMachine.CurrentState is AgentWorkState.Error)
    {
      await _wrapper.FireAsync(Machine, AgentOverallTrigger.ErrorOccurred);
    }
  }
  private async Task HandleStoppingStateChangeAsync()
  {
      if (_workMachine.CurrentState is AgentWorkState.Processing)
      {
        // Logic to stop work machine
      }

      if (_syncMachine.CurrentState is AgentSyncState.Processing)
      {
        // Logic to stop sync machine
      }

      if (_authMachine.CurrentState is AgentAuthenticationState.Processing)
      {
        // Logic to stop auth machine
      }
  }
  private Task HandleErrorStateChangeAsync()
  {
      _logger.LogInformation("Handling error state, stopping work state machine.");
      return Task.CompletedTask;
  }
  #endregion
}
