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

    var message = new AgentMetricRequestMessage
    {
      CpuUsage = metrics.GetValueOrDefault("cpu_usage", 0), // value in percentage
      MemoryUsage = metrics.GetValueOrDefault("memory_usage", 0), // value in percentage
      DiskUsage = metrics.GetValueOrDefault("disk_usage", 0), // value in percentage
      NetworkUsage = metrics.GetValueOrDefault("network_usage", 0), // value in bytes per second
      Uptime = metrics.GetValueOrDefault("uptime", 0), // value in seconds
      Timestamp = DateTimeOffset.UtcNow, // current timestamp
    };

    await publisherClient.PublishAsync<AgentMetricResponseMessage, AgentMetricRequestMessage>(
        Url, message, cancellationToken);

    logger.LogInformation("Published metrics: {Message}", message.ToString());
  }
}
