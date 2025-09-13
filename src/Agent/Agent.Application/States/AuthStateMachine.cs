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
    Error
}

public enum AuthenticationTrigger
{
    Start,
    Success,
    ErrorOccurred
}

public class AuthStateMachine
{
    private readonly StateMachine<AgentAuthenticationState, AuthenticationTrigger> _machine;
    private readonly ICommunicationClient _communicationClient;
    private readonly StateMachineWrapper _wrapper;
    private readonly AgentStateContext _context;

    public AgentAuthenticationState CurrentState => _machine.State;

    public AuthStateMachine(
        StateMachineWrapper wrapper,
        ICommunicationClient communicationClient,
        AgentStateContext context)
    {
        _communicationClient = communicationClient ?? throw new ArgumentNullException(nameof(communicationClient));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));

        _machine = new StateMachine<AgentAuthenticationState, AuthenticationTrigger>(AgentAuthenticationState.Idle);

        ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
        _machine.Configure(AgentAuthenticationState.Idle)
            .Permit(AuthenticationTrigger.Start, AgentAuthenticationState.Processing)
            .Permit(AuthenticationTrigger.ErrorOccurred, AgentAuthenticationState.Error);

        _machine.Configure(AgentAuthenticationState.Processing)
            .OnEntryAsync(HandleProcessingAsync)
            .Permit(AuthenticationTrigger.Success, AgentAuthenticationState.Finished)
            .Permit(AuthenticationTrigger.ErrorOccurred, AgentAuthenticationState.Error);

        _machine.Configure(AgentAuthenticationState.Finished)
            .Permit(AuthenticationTrigger.ErrorOccurred, AgentAuthenticationState.Error);

        _machine.Configure(AgentAuthenticationState.Error)
            .Permit(AuthenticationTrigger.Start, AgentAuthenticationState.Processing);
    }

    public async Task StartAsync()
    {
        await _wrapper.FireAsync(_machine, AuthenticationTrigger.Start);
    }

    private async Task HandleProcessingAsync()
    {
        try
        {
            var authResponse = await _communicationClient.PostAsync<AgentLoginResponseMessage, AgentLoginRequestMessage>(
                url: "auth/agent/login",
                authenticate: false,
                message: new AgentLoginRequestMessage
                {
                    AgentId = Guid.Parse("01993df8-fef4-784d-a677-d51ee89c7e6d"),
                    Secret = "88692b37-bb69-4513-a02b-583b62c5df88"
                },
                cancellationToken: CancellationToken.None);

            if (authResponse is null || string.IsNullOrWhiteSpace(authResponse.Token))
                throw new UnauthorizedAccessException();

            _context.AuthenticationToken = authResponse.Token;

            await _wrapper.FireAsync(_machine, AuthenticationTrigger.Success);
        }
        catch (Exception)
        {
            await _wrapper.FireAsync(_machine, AuthenticationTrigger.ErrorOccurred);
        }
    }
}
