using Agent.Application.Abstractions;
using Agent.Domain.Configs;
using Common.Messages.Agent.Sync;
using Common.Messages.Agent.Sync.Hardware;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agent.Application.Services;

public interface ISyncService
{
  Task SyncAsync(bool isInitial = false);
}

public class SyncService(
  ILogger<SyncService> logger,
  IOptions<EndpointsConfig> endpointsConfig,
  IStaticDataCollector<CpuInfoMessage> cpuCollector,
  IStaticDataCollector<RamInfoMessage> memoryCollector,
  IStaticDataCollector<DiskInfoMessage> diskCollector,
  IStaticDataCollector<GpuInfoMessage> gpuCollector,
  ICommunicationClient communicationClient,
  IPolicyProvider policyProvider) : ISyncService
{
  public async Task SyncAsync(bool isInitial = false)
  {
    logger.LogInformation("Starting synchronization. IsInitial: {IsInitial}", isInitial);

    ServerSyncMessage? responseMessage;
    if (isInitial)
    {
      var message = new AgentSyncMessage
      {
          Hardware = new HardwareMessage
          {
              Cpu = cpuCollector.Collect(),
              Ram = memoryCollector.Collect(),
              Disk = diskCollector.Collect(),
              Gpu = gpuCollector.Collect()
          },
          RsopReportBase64 = policyProvider.GenerateRsopReport()
      };

      responseMessage = await communicationClient.PutAsync<ServerSyncMessage, AgentSyncMessage>(
          url: endpointsConfig.Value.Sync,
          authenticate: true,
          message: message);
    }
    else
    {
      responseMessage = await communicationClient.GetAsync<ServerSyncMessage>(
          url: endpointsConfig.Value.SyncSubscribe,
          authenticate: true);
    }

    if (responseMessage is null)
    {
      logger.LogWarning("No response received from sync endpoint.");
      return;
    }

    logger.LogInformation("Received sync response: {Message}", responseMessage);
  }
}
