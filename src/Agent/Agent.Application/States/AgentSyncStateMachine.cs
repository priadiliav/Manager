using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Domain.Context;
using Common.Messages.Agent;
using Common.Messages.Metric;
using Common.Messages.Static;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum AgentSyncState
{
  Started,
  Synchronizing,
  Synchronized,
  Error,
}

public enum SyncTrigger
{
  Start,
  Success,
  ErrorOccurred
}

public class AgentSyncStateMachine
{
  private readonly StateMachine<AgentSyncState, SyncTrigger> _machine;
  private readonly ILogger<AgentAuthStateMachine> _logger;
  private readonly AgentStateContext _context;

  private readonly IStaticDataCollector<HardwareMessage> _cpuInfoCollector;

  public AgentSyncState CurrentState => _machine.State;

  public AgentSyncStateMachine(
    ILogger<AgentAuthStateMachine> logger,
    IStaticDataCollector<HardwareMessage> cpuInfoCollector,
    AgentStateContext context)
  {
    _cpuInfoCollector = cpuInfoCollector ?? throw new ArgumentNullException(nameof(cpuInfoCollector));
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _machine = new StateMachine<AgentSyncState, SyncTrigger>(AgentSyncState.Started);

    _machine.Configure(AgentSyncState.Started)
        .Permit(SyncTrigger.Start, AgentSyncState.Synchronizing);

    _machine.Configure(AgentSyncState.Synchronizing)
        .Permit(SyncTrigger.Success, AgentSyncState.Synchronized)
        .Permit(SyncTrigger.ErrorOccurred, AgentSyncState.Error);

    _machine.Configure(AgentSyncState.Error)
        .Permit(SyncTrigger.Start, AgentSyncState.Synchronizing);
  }


  public async Task StartAsync()
  {
    await _machine.FireAsync(SyncTrigger.Start);

    try
    {
      var message = new AgentSyncRequestMessage()
      {
          Hardware = new HardwareMessage()
          {
          }
      };

      _logger.LogInformation("Synchronization successful.");
      // _context.IsSynchronized = true;
    }
    catch (Exception ex)
    {
      await _machine.FireAsync(SyncTrigger.ErrorOccurred);
    }
  }
}
