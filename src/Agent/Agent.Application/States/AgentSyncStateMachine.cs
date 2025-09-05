using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Common.Messages.Agent;
using Common.Messages.Agent.Sync;
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

  private readonly IStaticDataCollector<CpuInfoMessage> _cpuCollector;
  private readonly IStaticDataCollector<RamInfoMessage> _memoryCollector;
  private readonly IStaticDataCollector<DiskInfoMessage> _diskCollector;
  private readonly IStaticDataCollector<GpuInfoMessage> _gpuCollector;
  private readonly ICommunicationClient _communicationClient;

  public AgentSyncState CurrentState => _machine.State;

  public AgentSyncStateMachine(
    ILogger<AgentAuthStateMachine> logger,
    IStaticDataCollector<CpuInfoMessage> cpuCollector,
    IStaticDataCollector<RamInfoMessage> memoryCollector,
    IStaticDataCollector<DiskInfoMessage> diskCollector,
    IStaticDataCollector<GpuInfoMessage> gpuCollector,
    ICommunicationClient communicationClient,
    AgentStateContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _machine = new StateMachine<AgentSyncState, SyncTrigger>(AgentSyncState.Started);
    _cpuCollector = cpuCollector ?? throw new ArgumentNullException(nameof(cpuCollector));
    _memoryCollector = memoryCollector ?? throw new ArgumentNullException(nameof(memoryCollector));
    _diskCollector = diskCollector ?? throw new ArgumentNullException(nameof(diskCollector));
    _gpuCollector = gpuCollector ?? throw new ArgumentNullException(nameof(gpuCollector));
    _communicationClient = communicationClient ?? throw new ArgumentNullException(nameof(communicationClient));

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
              Cpu = _cpuCollector.Collect(),
              Ram = _memoryCollector.Collect(),
              Disk = _diskCollector.Collect(),
              Gpu = _gpuCollector.Collect()
          }
      };

      await _communicationClient.PutAsync<AgentSyncResponseMessage, AgentSyncRequestMessage>(
          url: "agents/sync",
          authenticate: true,
          message, _context.CancellationTokenSource.Token);

      _logger.LogInformation("Synchronization successful.");
    }
    catch (Exception ex)
    {
      await _machine.FireAsync(SyncTrigger.ErrorOccurred);
    }
  }
}
