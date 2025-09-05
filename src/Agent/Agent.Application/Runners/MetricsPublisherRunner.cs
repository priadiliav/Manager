using Agent.Application.Abstractions;
using Common.Messages.Metric;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Runners;

public class MetricsPublisherRunner(
  ILogger<MetricsPublisherRunner> logger,
  IEnumerable<IDynamicDataCollector<double>> collectors,
  IPublisherClient publisherClient) : IWorkerRunner
{
  public TimeSpan Interval => TimeSpan.FromSeconds(10);
  public string Url => "metrics/publish";

  public async Task RunAsync(CancellationToken cancellationToken = default)
  {
    var metrics = collectors.ToDictionary(c => c.Name,
        c => c.Collect(cancellationToken));

    var message = new MetricRequestMessage
    {
      CpuUsage = metrics.GetValueOrDefault("cpu_usage", 0),
      MemoryUsage = metrics.GetValueOrDefault("memory_usage", 0),
      DiskUsage = metrics.GetValueOrDefault("disk_usage", 0),
      NetworkUsage = metrics.GetValueOrDefault("network_usage", 0),
      Uptime = metrics.GetValueOrDefault("uptime", 0),
      Timestamp = DateTimeOffset.UtcNow,
    };

    await publisherClient.PublishAsync<MetricResponseMessage, MetricRequestMessage>(
        Url, message, cancellationToken);

    logger.LogInformation("Published metrics: {Message}", message);
  }
}
