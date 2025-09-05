using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IMetricRepository
{
  Task CreateAsync(Metric metric);
  Task<IEnumerable<Metric>> GetMetricsAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to, int limit = 50);
}
