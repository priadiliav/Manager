using Agent.Application.Abstractions;

namespace Agent.Infrastructure.Collectors;

public class UptimeCollector : IDynamicDataCollector<double>
{
  public string Name => "uptime";

  public double Collect(CancellationToken cancellationToken = default)
  {
    return Environment.TickCount64 / 1000.0;
  }
}
