using System.Globalization;
using Agent.Application.Abstractions;
using Agent.Domain.Models;

namespace Agent.Infrastructure.Collectors;

public class CpuMetricCollector : IMetricCollector
{
  public async Task<Metric> CollectAsync(CancellationToken cancellationToken = default)
  {
    await Task.Delay(100, cancellationToken); 

    var cpuMetric = new Metric
    {
       Name = "CPU Usage",
       Value = (new Random().NextDouble() * 100).ToString(CultureInfo.InvariantCulture),
    };

    return cpuMetric;
  }
}
