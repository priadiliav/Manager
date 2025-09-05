using Agent.Application.Abstractions;

namespace Agent.Infrastructure.Collectors.Dynamic;

public class DiskUsageCollector(string driveLetter = "C") : IDynamicDataCollector<double>
{
  public string Name => "disk_usage";

  public double Collect(CancellationToken cancellationToken = default)
  {
    var drive = new DriveInfo(driveLetter);

    if (!drive.IsReady)
      return 0;

    return Math.Round((double)(drive.TotalSize - drive.TotalFreeSpace) / drive.TotalSize * 100, 2);
  }
}
