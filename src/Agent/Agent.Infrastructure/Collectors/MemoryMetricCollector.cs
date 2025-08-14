using Agent.Application.Abstractions;
using Agent.Domain.Models;

namespace Agent.Infrastructure.Collectors;

public class MemoryMetricCollector : IMetricCollector
{
  public async Task<Metric> CollectAsync(CancellationToken cancellationToken = default)
  {
    await Task.Delay(100, cancellationToken); 

    var memoryMetric = new Metric
    {
      Name = "Memory Usage",
      Value = (new Random().NextDouble() * 100).ToString(System.Globalization.CultureInfo.InvariantCulture),
    };

    return memoryMetric;
  }
}
