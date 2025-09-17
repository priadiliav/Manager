using System.Management;
using Agent.Application.Abstractions;
using Common.Messages.Agent.Sync.Hardware;
using Microsoft.VisualBasic.Devices;

namespace Agent.Infrastructure.Collectors.Static;

public class DiskInfoCollector : IStaticDataCollector<DiskInfoMessage>
{
  public string Name => "disk_info";

  public DiskInfoMessage Collect(CancellationToken cancellationToken = default)
  {
    var diskInfo = new DiskInfoMessage();
    var computer = new Computer();

    try
    {
      var drive = computer.FileSystem.Drives
          .FirstOrDefault(d => d.DriveType == System.IO.DriveType.Fixed && d.IsReady);
      if (drive is not null)
      {
        diskInfo.TotalDiskMb = drive.TotalSize / 1024 / 1024; // Convert bytes to MB
      }

      using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
      var disk = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
      if (disk is not null)
      {
        diskInfo.DiskModel = disk["Model"]?.ToString() ?? "Unknown";
      }
    }
    catch
    {
      // Ignore exceptions and return default values
    }
    return diskInfo;
  }
}
