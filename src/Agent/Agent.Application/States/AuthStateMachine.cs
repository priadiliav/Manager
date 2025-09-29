using Agent.Application.Services;
using Agent.Application.Utils;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum AuthState
{
    Idle,
    Processing,
    Stopping,
    Error
}

public enum AuthTrigger
{
    Start,
    Success,
    Stop,
    ErrorOccurred
}

public class AuthStateMachine
{
    private readonly StateMachine<AuthState, AuthTrigger> _machine;
    private readonly ILogger<AuthStateMachine> _logger;

    private readonly IAuthService _authService;

    public AuthStateMachine(
        ILogger<AuthStateMachine> logger,
        IAuthService authService,
        StateMachineWrapper wrapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        // Initialize state machine
        _machine = new StateMachine<AuthState, AuthTrigger>(AuthState.Idle);

        // Configure state machine
        ConfigureStateMachine();

        // Register state machine with the wrapper
        wrapper.RegisterMachine(_machine, nameof(AuthStateMachine));
    }

    private void ConfigureStateMachine()
    {
      _machine.Configure(AuthState.Idle)
          .Permit(AuthTrigger.Start, AuthState.Processing);

      _machine.Configure(AuthState.Processing)
          .OnEntryAsync(HandleProcessingAsync)
          .Permit(AuthTrigger.Success, AuthState.Stopping)
          .Permit(AuthTrigger.Stop, AuthState.Stopping)
          .Permit(AuthTrigger.ErrorOccurred, AuthState.Error);

      _machine.Configure(AuthState.Stopping)
          .Permit(AuthTrigger.Start, AuthState.Idle);

      _machine.Configure(AuthState.Error)
          .Permit(AuthTrigger.Start, AuthState.Processing);
    }

    public AuthState CurrentState => _machine.State;
    public async Task StartAsync()
      => await StateMachineWrapper.FireAsync(_machine, AuthTrigger.Start);
    public async Task StopAsync()
      => await StateMachineWrapper.FireAsync(_machine, AuthTrigger.Stop);

    #region Handlers
    /// <summary>
    /// Handles the processing state by attempting to authenticate.
    /// On success, it triggers the Success transition; on failure, it triggers the ErrorOccurred transition.
    /// </summary>
    private async Task HandleProcessingAsync()
      => await StateMachineExecutor.ExecuteAsync(_machine, _logger,
          async () => await _authService.AuthenticateAsync(),
          AuthTrigger.Success, AuthTrigger.ErrorOccurred);

    /// <summary>
    /// Handles the stopping state by performing any necessary cleanup.
    /// After a delay, it triggers the Start transition to return to the Idle state.
    /// </summary>
    private async Task HandleStoppingAsync()
      => await StateMachineExecutor.ExecuteAsync(_machine, _logger,
          async () => await Task.Delay(3000),
          AuthTrigger.Start, AuthTrigger.ErrorOccurred);
    #endregion
}
