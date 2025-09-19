using Agent.Application.Services;
using Agent.Application.Utils;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum SyncState
{
  Idle,
  Processing,
  Stopping,
  Error
}

public enum SyncTrigger
{
  Start,
  Success,
  Stop,
  ErrorOccurred
}

public class SyncStateMachine
{
  private readonly StateMachine<SyncState, SyncTrigger> _machine;
  private readonly ISyncService _syncService;
  private readonly ILogger<SyncStateMachine> _logger;

  public SyncStateMachine(
    ILogger<SyncStateMachine> logger,
    ISyncService syncService,
    StateMachineWrapper wrapper)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));

    // Initialize state machine
    _machine = new StateMachine<SyncState, SyncTrigger>(SyncState.Idle);

    // Configure state machine
    ConfigureStateMachine();

    // Register state machine with the wrapper
    wrapper.RegisterMachine(_machine, "Synchronization");
  }

  private void ConfigureStateMachine()
  {
    _machine.Configure(SyncState.Idle)
        .Permit(SyncTrigger.Start, SyncState.Processing);

    _machine.Configure(SyncState.Processing)
        .OnEntryAsync(HandleProcessingAsync)
        .Permit(SyncTrigger.Success, SyncState.Stopping)
        .Permit(SyncTrigger.Stop, SyncState.Stopping)
        .Permit(SyncTrigger.ErrorOccurred, SyncState.Error);

    _machine.Configure(SyncState.Stopping)
        .Permit(SyncTrigger.Start, SyncState.Idle);

    _machine.Configure(SyncState.Error)
        .Permit(SyncTrigger.Start, SyncState.Processing);
  }

  public SyncState CurrentState => _machine.State;

  public async Task StartAsync()
    => await StateMachineWrapper.FireAsync(_machine, SyncTrigger.Start);

  public async Task StopAsync()
    => await StateMachineWrapper.FireAsync(_machine, SyncTrigger.Stop);

  #region Handlers
  /// <summary>
  /// Handles the processing state by invoking the sync service.
  /// If successful, transitions to Success state; otherwise, to Error state.
  /// </summary>
  /// <returns></returns>
  private Task HandleProcessingAsync()
    => StateMachineExecutor.ExecuteAsync(_machine, _logger,
        async () => await _syncService.SyncAsync(), SyncTrigger.Success, SyncTrigger.ErrorOccurred);

  /// <summary>
  /// Handles the stopping state by performing any necessary cleanup.
  /// After a delay, it triggers the Start transition to return to the Idle state.
  /// </summary>
  private async Task HandleStoppingAsync()
    => await StateMachineExecutor.ExecuteAsync(_machine, _logger,
        async () => await Task.Delay(3000), SyncTrigger.Start, SyncTrigger.ErrorOccurred);
  #endregion
}
