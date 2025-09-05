using Agent.Application.Abstractions;

namespace Agent.Infrastructure.Collectors.Dynamic;

public class UptimeCollector : IDynamicDataCollector<double>
{
  public string Name => "uptime";
  public double Collect(CancellationToken cancellationToken = default) => Environment.TickCount64 / 1000.0;
}
