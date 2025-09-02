using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Domain.Context;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum AgentAuthenticationState
{
  Started,
  Authenticating,
  Authenticated,
  Error,
}

public enum AuthenticationTrigger
{
  Start,
  Success,
  ErrorOccurred
}

public class AgentAuthStateMachine
{
  private readonly StateMachine<AgentAuthenticationState, AuthenticationTrigger> _machine;
  private readonly ILogger<AgentAuthStateMachine> _logger;

  private readonly  IAuthenticationService _authenticationService;
  private readonly AgentStateContext _context;

  public AgentAuthenticationState CurrentState => _machine.State;

  public AgentAuthStateMachine(
    ILogger<AgentAuthStateMachine> logger,
    IAuthenticationService authenticationService,
    AgentStateContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _machine = new StateMachine<AgentAuthenticationState, AuthenticationTrigger>(AgentAuthenticationState.Started);

    _machine.Configure(AgentAuthenticationState.Started)
        .Permit(AuthenticationTrigger.Start, AgentAuthenticationState.Authenticating);

    _machine.Configure(AgentAuthenticationState.Authenticating)
        .Permit(AuthenticationTrigger.Success, AgentAuthenticationState.Authenticated)
        .Permit(AuthenticationTrigger.ErrorOccurred, AgentAuthenticationState.Error);

    _machine.Configure(AgentAuthenticationState.Error)
        .Permit(AuthenticationTrigger.Start, AgentAuthenticationState.Authenticating);
  }

  public async Task StartAsync()
  {
    await _machine.FireAsync(AuthenticationTrigger.Start);

    try
    {
      await _authenticationService.AuthenticateAsync(_context.CancellationTokenSource.Token);

      await _machine.FireAsync(AuthenticationTrigger.Success);
    }
    catch (Exception)
    {
      _logger.LogError("An error occurred during authentication");

      await _machine.FireAsync(AuthenticationTrigger.ErrorOccurred);
    }
  }
}
