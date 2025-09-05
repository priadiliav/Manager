using Agent.Application.Abstractions;
using Microsoft.VisualBasic.Devices;

namespace Agent.Infrastructure.Collectors.Dynamic;

public class MemoryUsageCollector : IDynamicDataCollector<double>
{
  public string Name => "memory_usage";

  public double Collect(CancellationToken cancellationToken = default)
  {
    var pc = new ComputerInfo();
    var used = pc.TotalPhysicalMemory - pc.AvailablePhysicalMemory;
    return Math.Round((double)used / pc.TotalPhysicalMemory * 100, 2);
  }
}
