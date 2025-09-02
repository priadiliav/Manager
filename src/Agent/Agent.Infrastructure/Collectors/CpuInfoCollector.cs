using System.Management;
using Agent.Application.Abstractions;
using Common.Messages.Static;

namespace Agent.Infrastructure.Collectors;

public class CpuInfoDataCollector : IStaticDataCollector<CpuInfoMessage>
{
  public string Name => "cpu_info";

  public CpuInfoMessage Collect(CancellationToken cancellationToken = default)
  {
    var model = string.Empty;
    var cores = 0;
    double speedGHz = 0;
    var architecture = string.Empty;

    using var searcher = new ManagementObjectSearcher("select Name, NumberOfCores, MaxClockSpeed, Architecture from Win32_Processor");

    var cpu = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

    if (cpu is null)
      return new CpuInfoMessage
      {
          Model = model,
          Cores = cores,
          SpeedGHz = speedGHz,
          Architecture = architecture
      };

    model = cpu["Name"]?.ToString() ?? "Unknown";
    cores = Convert.ToInt32(cpu["NumberOfCores"]);
    speedGHz = Math.Round(Convert.ToDouble(cpu["MaxClockSpeed"]) / 1000, 2);

    var arch = Convert.ToInt32(cpu["Architecture"]);
    architecture = arch switch
    {
        0 => "x86",
        1 => "MIPS",
        2 => "Alpha",
        3 => "PowerPC",
        5 => "ARM",
        6 => "ia64",
        9 => "x64",
        _ => "Unknown"
    };

    return new CpuInfoMessage
    {
        Model = model,
        Cores = cores,
        SpeedGHz = speedGHz,
        Architecture = architecture
    };
  }
}
