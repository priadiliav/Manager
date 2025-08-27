using Agent.Application.Abstractions;
using Common.Messages.Metric;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

public interface IMetricService : IPublisherRunner
{
}

public class MetricService(
  ILogger<MetricService> logger,
  IPublisherClient<MetricMessage> publisherClient,
  IEnumerable<IMetricCollector> metricCollectors)
  : IMetricService
{
  private async Task<MetricMessage> GetMetricsAsync(CancellationToken cancellationToken)
  {
    var collectionTasks = metricCollectors.Select(c => c.CollectAsync(cancellationToken));
    var metrics = await Task.WhenAll(collectionTasks);

    return new MetricMessage
    {
      CpuUsage = 50 + new Random().Next(0, 50),
      MemoryUsage = 1024 + new Random().Next(0, 1024),
      DiskUsage = 500 + new Random().Next(0, 500),
      Timestamp = DateTime.UtcNow,
    };
  }

  public async Task PublishOnceAsync(CancellationToken cancellationToken = default)
     => await publisherClient.PublishAsync(await GetMetricsAsync(cancellationToken), cancellationToken);
}
