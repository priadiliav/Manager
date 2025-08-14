using Agent.Domain.Models;

namespace Agent.Application.Abstractions;

public interface IMetricCollector
{
  Task<Metric> CollectAsync(CancellationToken cancellationToken = default);
}
