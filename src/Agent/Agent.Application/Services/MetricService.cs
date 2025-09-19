using Agent.Application.Abstractions;
using Agent.Domain.Configs;
using Common.Messages.Agent.Metric;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agent.Application.Services;

public interface IMetricService
{
  /// <summary>
  /// Collects system metrics and publishes them to server.
  /// </summary>
  /// <returns></returns>
  Task CollectAndPublishMetricsAsync();
}

public class MetricService(
  ILogger<MetricService> logger,
  IOptions<EndpointsConfig> endpointsConfig,
  IEnumerable<IDynamicDataCollector<double>> collectors,
  IPublisherClient publisherClient) : IMetricService
{
  public async Task CollectAndPublishMetricsAsync()
  {
    var metrics = collectors.ToDictionary(
        c => c.Name,
        c => c.Collect());

    var message = new AgentMetricRequestMessage
    {
        CpuUsage = metrics.GetValueOrDefault("cpu_usage", 0),
        MemoryUsage = metrics.GetValueOrDefault("memory_usage", 0),
        DiskUsage = metrics.GetValueOrDefault("disk_usage", 0),
        NetworkUsage = metrics.GetValueOrDefault("network_usage", 0),
        Uptime = metrics.GetValueOrDefault("uptime", 0),
        Timestamp = DateTimeOffset.UtcNow,
    };

    logger.LogInformation("Collected metrics: {Metrics}", message);

    await publisherClient.PublishAsync<AgentMetricResponseMessage, AgentMetricRequestMessage>(
        url: endpointsConfig.Value.Metrics,
        message: message);
  }
}
