using System.Diagnostics;
using Agent.Application.Abstractions;

namespace Agent.Infrastructure.Collectors.Dynamic;

public class CpuUsageCollector : IDynamicDataCollector<double>
{
  public string Name => "cpu_usage";

  public double Collect(CancellationToken cancellationToken = default)
  {
    using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
    cpuCounter.NextValue();
    Thread.Sleep(500);
    return Math.Round(cpuCounter.NextValue(), 2);
  }
}
