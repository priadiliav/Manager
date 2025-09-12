using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Common.Messages.Agent.Login;
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
  private readonly ICommunicationClient _communicationClient;
  private readonly AgentStateContext _context;

  public AgentAuthenticationState CurrentState => _machine.State;

  public AgentAuthStateMachine(
    ILogger<AgentAuthStateMachine> logger,
    ICommunicationClient communicationClient,
    AgentStateContext context)
  {
    _communicationClient = communicationClient ?? throw new ArgumentNullException(nameof(communicationClient));
    _context = context ?? throw new ArgumentNullException(nameof(context));
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
      var authResponse = await _communicationClient.PostAsync<AgentLoginResponseMessage, AgentLoginRequestMessage>(
          url: "auth/agent/login",
          authenticate: false,
          message: new AgentLoginRequestMessage
          {
            AgentId = Guid.Parse("01993df8-fef4-784d-a677-d51ee89c7e6d"),
            Secret = "88692b37-bb69-4513-a02b-583b62c5df88"
          }, cancellationToken: CancellationToken.None);

      if (authResponse is null || string.IsNullOrWhiteSpace(authResponse.Token))
        throw new UnauthorizedAccessException();

      _context.AuthenticationToken = authResponse.Token;

      await _machine.FireAsync(AuthenticationTrigger.Success);
    }
    catch (Exception)
    {
      _logger.LogError("An error occurred during authentication");

      await _machine.FireAsync(AuthenticationTrigger.ErrorOccurred);
    }
  }
}
