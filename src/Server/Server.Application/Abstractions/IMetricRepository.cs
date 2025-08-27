using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IMetricRepository
{
  Task CreateAsync(Metric metric);
}
