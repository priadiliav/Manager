using Agent.Application.Abstractions;
using Agent.Domain.Configs;
using Common.Messages.Agent.Sync;
using Common.Messages.Agent.Sync.Hardware;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agent.Application.Services;

public interface ISyncService
{
  Task SyncAsync();
}

public class SyncService(
    ILogger<SyncService> logger,
    IOptions<EndpointsConfig> endpointsConfig,
    IStaticDataCollector<CpuInfoMessage> cpuCollector,
    IStaticDataCollector<RamInfoMessage> memoryCollector,
    IStaticDataCollector<DiskInfoMessage> diskCollector,
    IStaticDataCollector<GpuInfoMessage> gpuCollector,
    ICommunicationClient communicationClient,
    IConfigurationRepository configurationRepository) : ISyncService
{
  public async Task SyncAsync()
  {
    var message = new AgentSyncRequestMessage
    {
        Hardware = new AgentHardwareMessage
        {
            Cpu = cpuCollector.Collect(),
            Ram = memoryCollector.Collect(),
            Disk = diskCollector.Collect(),
            Gpu = gpuCollector.Collect()
        },
    };

    logger.LogInformation("Synchronization: {Message}", message);

    await communicationClient.PutAsync<AgentSyncResponseMessage, AgentSyncRequestMessage>(
        url: endpointsConfig.Value.Sync,
        authenticate: true,
        message: message);
  }
}
