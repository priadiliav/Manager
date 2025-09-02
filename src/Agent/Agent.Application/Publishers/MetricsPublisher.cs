using Agent.Application.Abstractions;
using Common.Messages.Metric;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Publishers;

public class MetricsPublisher(
  ILogger<MetricsPublisher> logger,
  IEnumerable<IDynamicDataCollector<double>> collectors,
  IPublisherClient<MetricMessage> publisherClient) : IPublisherRunner
{
  public async Task PublishAsync(CancellationToken cancellationToken = default)
  {
    var metrics = collectors.ToDictionary(c => c.Name, c => c.Collect(cancellationToken));

    var message = new MetricMessage
    {
      CpuUsage = metrics.GetValueOrDefault("cpu_usage", 0),
      MemoryUsage = metrics.GetValueOrDefault("memory_usage", 0),
      DiskUsage = metrics.GetValueOrDefault("disk_usage", 0),
      NetworkUsage = metrics.GetValueOrDefault("network_usage", 0),
      Uptime = metrics.GetValueOrDefault("uptime", 0),
      Timestamp = DateTimeOffset.UtcNow,
    };

    await publisherClient.PublishAsync(message, cancellationToken);

    logger.LogInformation("Published metrics: {Message}", message);
  }
}
