using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Common.Messages.Agent.Login;
using Stateless;

namespace Agent.Application.States;

public enum AgentAuthenticationState
{
    Idle,
    Processing,
    Finished,
    Stopping,
    Error
}

public enum AuthenticationTrigger
{
    Start,
    Success,
    Stop,
    ErrorOccurred
}

public class AuthStateMachine
{
    public StateMachine<AgentAuthenticationState, AuthenticationTrigger> Machine { get; }

    private readonly ICommunicationClient _communicationClient;
    private readonly StateMachineWrapper _wrapper;
    private readonly AgentStateContext _context;

    public AgentAuthenticationState CurrentState => Machine.State;

    public AuthStateMachine(
        ICommunicationClient communicationClient,
        StateMachineWrapper wrapper,
        AgentStateContext context)
    {
        _communicationClient = communicationClient;
        _context = context;
        _wrapper = wrapper;

        // Initialize state machine
        Machine = new StateMachine<AgentAuthenticationState, AuthenticationTrigger>(AgentAuthenticationState.Idle);

        // Configure state machine
        ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
        Machine.Configure(AgentAuthenticationState.Idle)
            .Permit(AuthenticationTrigger.Start, AgentAuthenticationState.Processing)
            .Permit(AuthenticationTrigger.ErrorOccurred, AgentAuthenticationState.Error);

        Machine.Configure(AgentAuthenticationState.Processing)
            .OnEntryAsync(HandleProcessingAsync)
            .Permit(AuthenticationTrigger.Success, AgentAuthenticationState.Finished)
            .Permit(AuthenticationTrigger.Stop, AgentAuthenticationState.Stopping)
            .Permit(AuthenticationTrigger.ErrorOccurred, AgentAuthenticationState.Error);

        Machine.Configure(AgentAuthenticationState.Finished)
            .Permit(AuthenticationTrigger.ErrorOccurred, AgentAuthenticationState.Error);

        Machine.Configure(AgentAuthenticationState.Stopping)
            .Permit(AuthenticationTrigger.Start, AgentAuthenticationState.Idle);

        Machine.Configure(AgentAuthenticationState.Error)
            .Permit(AuthenticationTrigger.Start, AgentAuthenticationState.Processing);
    }

    public async Task StartAsync() => await _wrapper.FireAsync(Machine, AuthenticationTrigger.Start);

    #region Handlers
    private async Task HandleProcessingAsync()
    {
        try
        {
            var authResponse = await _communicationClient.PostAsync<AgentLoginResponseMessage, AgentLoginRequestMessage>(
                url: "auth/agent/login",
                authenticate: false,
                message: new AgentLoginRequestMessage
                {
                    AgentId = AgentStateContext.Id,
                    Secret = AgentStateContext.Secret.ToString()
                },
                cancellationToken: CancellationToken.None);

            if (authResponse is null || string.IsNullOrWhiteSpace(authResponse.Token))
                throw new UnauthorizedAccessException();

            _context.AuthenticationToken = authResponse.Token;

            await _wrapper.FireAsync(Machine, AuthenticationTrigger.Success);
        }
        catch (Exception)
        {
            await _wrapper.FireAsync(Machine, AuthenticationTrigger.ErrorOccurred);
        }
    }
    #endregion
}
