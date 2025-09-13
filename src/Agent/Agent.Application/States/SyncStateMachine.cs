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
  Idle,
  Processing,
  Finished,
  Error
}

public enum SyncTrigger
{
  Start,
  Success,
  ErrorOccurred
}

public class SyncStateMachine
{
  private readonly StateMachine<AgentSyncState, SyncTrigger> _machine;
  private readonly StateMachineWrapper _wrapper;
  private readonly AgentStateContext _context;

  private readonly IStaticDataCollector<CpuInfoMessage> _cpuCollector;
  private readonly IStaticDataCollector<RamInfoMessage> _memoryCollector;
  private readonly IStaticDataCollector<DiskInfoMessage> _diskCollector;
  private readonly IStaticDataCollector<GpuInfoMessage> _gpuCollector;
  private readonly ICommunicationClient _communicationClient;

  public AgentSyncState CurrentState => _machine.State;

  public SyncStateMachine(
    StateMachineWrapper wrapper,
    IStaticDataCollector<CpuInfoMessage> cpuCollector,
    IStaticDataCollector<RamInfoMessage> memoryCollector,
    IStaticDataCollector<DiskInfoMessage> diskCollector,
    IStaticDataCollector<GpuInfoMessage> gpuCollector,
    ICommunicationClient communicationClient,
    AgentStateContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
    _machine = new StateMachine<AgentSyncState, SyncTrigger>(AgentSyncState.Idle);
    _cpuCollector = cpuCollector ?? throw new ArgumentNullException(nameof(cpuCollector));
    _memoryCollector = memoryCollector ?? throw new ArgumentNullException(nameof(memoryCollector));
    _diskCollector = diskCollector ?? throw new ArgumentNullException(nameof(diskCollector));
    _gpuCollector = gpuCollector ?? throw new ArgumentNullException(nameof(gpuCollector));
    _communicationClient = communicationClient ?? throw new ArgumentNullException(nameof(communicationClient));

    _machine.Configure(AgentSyncState.Idle)
        .Permit(SyncTrigger.Start, AgentSyncState.Processing);

    _machine.Configure(AgentSyncState.Processing)
        .Permit(SyncTrigger.Success, AgentSyncState.Finished)
        .Permit(SyncTrigger.ErrorOccurred, AgentSyncState.Error);

    _machine.Configure(AgentSyncState.Error)
        .Permit(SyncTrigger.Start, AgentSyncState.Processing);
  }


  public async Task StartAsync()
  {
    await _wrapper.FireAsync(_machine, SyncTrigger.Start);

    try
    {
      var message = new AgentSyncRequestMessage
      {
          Hardware = new HardwareMessage
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
    }
    catch (Exception)
    {
      await _wrapper.FireAsync(_machine, SyncTrigger.ErrorOccurred);
    }
  }
}
