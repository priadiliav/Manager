using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum AgentOverallState
{
  Initializing,
  Authenticating,
  Synchronizing,
  Running,
  Error,
}

public enum AgentOverallTrigger
{
  Start,
  AuthSuccess,
  SyncSuccess,
  ErrorOccurred,
  Resume,
}

public class AgentOverallStateMachine
{
  private readonly StateMachine<AgentOverallState, AgentOverallTrigger> _machine;
  private readonly ILogger<AgentOverallStateMachine> _logger;

  private readonly AgentAuthStateMachine _authMachine;
  private readonly AgentSyncStateMachine _syncMachine;
  private readonly AgentWorkStateMachine _workMachine;
  private readonly AgentStateContext _context;
  public AgentOverallState CurrentState => _machine.State;

  public AgentOverallStateMachine(
    ILogger<AgentOverallStateMachine> logger,
    AgentStateContext context,
    AgentAuthStateMachine authMachine,
    AgentSyncStateMachine syncMachine,
    AgentWorkStateMachine workMachine)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _authMachine = authMachine ?? throw new ArgumentNullException(nameof(authMachine));
    _syncMachine = syncMachine ?? throw new ArgumentNullException(nameof(syncMachine));
    _workMachine = workMachine ?? throw new ArgumentNullException(nameof(workMachine));
    _machine = new StateMachine<AgentOverallState, AgentOverallTrigger>(AgentOverallState.Initializing);

    ConfigureAsync();
  }

  private void ConfigureAsync()
  {
    _machine.Configure(AgentOverallState.Initializing)
        .Permit(AgentOverallTrigger.Start, AgentOverallState.Authenticating);

    _machine.Configure(AgentOverallState.Authenticating)
        .OnEntryAsync(HandleAuthenticationStateChangeAsync)
        .Permit(AgentOverallTrigger.AuthSuccess, AgentOverallState.Synchronizing)
        .Permit(AgentOverallTrigger.ErrorOccurred, AgentOverallState.Error);

    _machine.Configure(AgentOverallState.Synchronizing)
        .OnEntryAsync(HandleSynchronizingStateChangeAsync)
        .Permit(AgentOverallTrigger.SyncSuccess, AgentOverallState.Running)
        .Permit(AgentOverallTrigger.ErrorOccurred, AgentOverallState.Error);

    _machine.Configure(AgentOverallState.Running)
        .OnEntryAsync(HandleRunningStateChangeAsync)
        .Permit(AgentOverallTrigger.ErrorOccurred, AgentOverallState.Error);

    _machine.Configure(AgentOverallState.Error)
        .OnEntryAsync(HandleErrorStateChangeAsync)
        .Permit(AgentOverallTrigger.Resume, AgentOverallState.Authenticating);
  }

  public async Task StartAsync()
  {
    _logger.LogInformation("Starting agent overall state machine.");

    await _machine.FireAsync(AgentOverallTrigger.Start);
  }

  private async Task HandleSynchronizingStateChangeAsync()
  {
    _logger.LogInformation("Agent is synchronizing, starting synchronization state machine.");
    await _syncMachine.StartAsync();

    if (_syncMachine.CurrentState is AgentSyncState.Error)
    {
      _logger.LogWarning("Synchronization failed, transitioning to error state.");
      await _machine.FireAsync(AgentOverallTrigger.ErrorOccurred);
    }
    else
    {
      _logger.LogInformation("Synchronization successful, transitioning to running state.");
      await _machine.FireAsync(AgentOverallTrigger.SyncSuccess);
    }
  }

  private async Task HandleAuthenticationStateChangeAsync()
  {
    _logger.LogInformation("Agent is authenticating, starting authentication state machine.");
    await _authMachine.StartAsync();

    if (_authMachine.CurrentState is AgentAuthenticationState.Authenticated)
    {
      _logger.LogInformation("Authentication successful, transitioning to running state.");
      await _machine.FireAsync(AgentOverallTrigger.AuthSuccess);
    }
    else
    {
      _logger.LogWarning("Authentication failed, transitioning to error state.");
      await _machine.FireAsync(AgentOverallTrigger.ErrorOccurred);
    }
  }

  private async Task HandleRunningStateChangeAsync()
  {
    _logger.LogInformation("Agent is running, starting work state machine.");

    await _workMachine.StartAsync();

    if (_workMachine.CurrentState is AgentWorkState.Error)
    {
      await _machine.FireAsync(AgentOverallTrigger.ErrorOccurred);
    }
  }

  private async Task HandleErrorStateChangeAsync()
  {
      _logger.LogInformation("Handling error state, stopping work state machine.");
  }
}
