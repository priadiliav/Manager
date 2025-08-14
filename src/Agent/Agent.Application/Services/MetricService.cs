using Agent.Application.Abstractions;
using Common.Messages.Metric;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

public interface IMetricService : IPublisherRunner
{
}

public class MetricService(
  ILogger<MetricService> logger,
  IPublisherClient<MetricsMessage> publisherClient,
  IEnumerable<IMetricCollector> metricCollectors)
  : IMetricService
{
  private async Task<MetricsMessage> GetMetricsAsync(CancellationToken cancellationToken)
  {
    var collectionTasks = metricCollectors.Select(c => c.CollectAsync(cancellationToken));
    var metrics = await Task.WhenAll(collectionTasks);

    // todo: replace with a dto mapper
    return new MetricsMessage
    {
        Metrics = metrics.Select(m => new MetricMessage
        {
            Name = m.Name,
            Value = m.Value
        }).ToList()
    };
  }

  public async Task PublishOnceAsync(CancellationToken cancellationToken = default)
     => await publisherClient.PublishAsync(await GetMetricsAsync(cancellationToken), cancellationToken);
}
