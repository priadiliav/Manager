using Agent.Application.Abstractions;
using Microsoft.VisualBasic.Devices;

namespace Agent.Infrastructure.Collectors;

public class MemoryUsageCollector : IDynamicDataCollector<double>
{
  public string Name => "memory_usage";

  public double Collect(CancellationToken cancellationToken = default)
  {
    var pc = new Microsoft.VisualBasic.Devices.ComputerInfo();
    var used = pc.TotalPhysicalMemory - pc.AvailablePhysicalMemory;
    return Math.Round((double)used / pc.TotalPhysicalMemory * 100, 2);
  }
}
