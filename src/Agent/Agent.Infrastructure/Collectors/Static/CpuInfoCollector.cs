using System.Management;
using Agent.Application.Abstractions;
using Common.Messages.Agent.Sync.Hardware;

namespace Agent.Infrastructure.Collectors.Static;

public class CpuInfoCollector : IStaticDataCollector<CpuInfoMessage>
{
  public string Name => "cpu_info";

  public CpuInfoMessage Collect(CancellationToken cancellationToken = default)
  {
    var cpuInfo = new CpuInfoMessage();

    try
    {
      using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
      var cpu = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
      if (cpu is not null)
      {
        cpuInfo.CpuModel = cpu["Name"]?.ToString() ?? "Unknown";
        cpuInfo.CpuCores = Convert.ToInt32(cpu["NumberOfCores"] ?? 0);
        cpuInfo.CpuSpeedGHz = Convert.ToDouble(cpu["MaxClockSpeed"] ?? 0) / 1000.0;
        cpuInfo.CpuArchitecture = Convert.ToInt32(cpu["Architecture"]) switch
        {
            0 => "x86",
            9 => "x64",
            _ => "Unknown"
        };
      }
    }
    catch
    {
      // Ignore exceptions and return default values
    }

    return cpuInfo;
  }
}
