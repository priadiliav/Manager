using System.Management;
using Agent.Application.Abstractions;
using Common.Messages.Agent.Sync.Hardware;
using Microsoft.VisualBasic.Devices;

namespace Agent.Infrastructure.Collectors.Static;

public class RamInfoCollector : IStaticDataCollector<RamInfoMessage>
{
  public string Name => "ram_info";

  public RamInfoMessage Collect(CancellationToken cancellationToken = default)
  {
    var ramInfo = new RamInfoMessage();
    var computer = new ComputerInfo();

    try
    {
      ramInfo.TotalMemoryMb = (long)(computer.TotalPhysicalMemory / 1024 / 1024);
      using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
      var ram = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
      if (ram is not null)
      {
        ramInfo.RamModel = ram["Manufacturer"]?.ToString() ?? "Unknown";
      }
    }catch
    {
      // Ignore exceptions and return default values
    }

    return ramInfo;
  }
}
