using Agent.WindowsService.Abstraction;
using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Application.Services;

public class MetricService(
  ILogger<MetricService> logger,
  IMetricCollector collector,
  IMetricStore store) : IMetricService
{
  public async Task SendAsync(CancellationToken cancellationToken)
  {
    var metricCollection = new List<Metric>();

    var storedBuffer = await store.GetAllAsync(cancellationToken);
    var currentCollected = await collector.CollectAsync(cancellationToken);

    metricCollection.AddRange(storedBuffer);
    metricCollection.AddRange(currentCollected);

    // convert to message
    var metricMessages = metricCollection.Select(m => m.ToMessage());
    logger.LogInformation("Sending {Count} metrics to server", metricMessages.Count());
    await Task.Delay(500, cancellationToken);

    var success = false;
    if (success)
    {
      await store.RemoveAllAsync(cancellationToken);
    }
    else
    {
      await store.StoreAsync(currentCollected, cancellationToken);
    }
  }
}
