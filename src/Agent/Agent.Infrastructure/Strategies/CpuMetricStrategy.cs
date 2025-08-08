using Agent.Application.Abstractions;

namespace Agent.Infrastructure.Strategies;

public class CpuMetricStrategy : IMetricStrategy
{
  public string Name => "cpu";

  public object Collect()
  {
    return new { Usage = 45 };
  }
}
