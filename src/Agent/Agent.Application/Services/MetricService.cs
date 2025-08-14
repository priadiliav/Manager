using System.Runtime.CompilerServices;
using Agent.Application.Abstractions;
using Common.Messages;
using Common.Messages.Metric;

namespace Agent.Application.Services;

public interface IMetricService : IPublisherRunner
{
    // Define methods for metric service
}

public class MetricService : IMetricService
{
  private readonly IPublisherClient<MetricsMessage> _publisherClient;
  private readonly IEnumerable<IMetricCollector> _metricCollectors;

  public MetricService(IPublisherClient<MetricsMessage> publisherClient, IEnumerable<IMetricCollector> metricCollectors)
  {
    _publisherClient = publisherClient ?? throw new ArgumentNullException(nameof(publisherClient));
    _metricCollectors = metricCollectors ?? throw new ArgumentNullException(nameof(metricCollectors));
  }

  private async IAsyncEnumerable<IMessage> GenerateMetricsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
  {
    while (!cancellationToken.IsCancellationRequested)
    {
      var metrics = await Task.WhenAll(_metricCollectors.Select(c => c.CollectAsync(cancellationToken)));

      yield return new MetricsMessage
      {
          Metrics = metrics.Select(m => new MetricMessage { Name = m.Name, Value = m.Value }).ToList()
      };

      await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
    }
  }

  public Task StartPublishingAsync(CancellationToken cancellationToken = default)
    => _publisherClient.StartPublishingAsync(GenerateMetricsAsync(cancellationToken), cancellationToken);
}
