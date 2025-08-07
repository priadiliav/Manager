using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum AgentOverallState
{
  Initializing,
  Authenticating,
  Running,
  Error,
}

public enum AgentOverallTrigger
{
  Start,
  AuthSuccess,
  ErrorOccurred,
  Resume,
}

public class AgentOverallStateMachine : IAgentStateMachine
{
  private readonly StateMachine<AgentOverallState, AgentOverallTrigger> _machine;
  private readonly ILogger<AgentOverallStateMachine> _logger;

  private readonly AgentStateContext _context;
  private readonly AgentAuthStateMachine _authMachine;
  private readonly AgentWorkStateMachine _workMachine;

  public AgentOverallState CurrentState => _machine.State;

  public AgentOverallStateMachine(
    ILogger<AgentOverallStateMachine> logger,
    AgentStateContext context,
    AgentAuthStateMachine authMachine,
    AgentWorkStateMachine workMachine)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _authMachine = authMachine ?? throw new ArgumentNullException(nameof(authMachine));
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
        .Permit(AgentOverallTrigger.AuthSuccess, AgentOverallState.Running)
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
