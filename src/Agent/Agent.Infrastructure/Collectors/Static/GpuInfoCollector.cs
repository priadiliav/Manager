using System.Management;
using Agent.Application.Abstractions;
using Common.Messages.Agent.Sync.Hardware;

namespace Agent.Infrastructure.Collectors.Static;

public class GpuInfoCollector : IStaticDataCollector<GpuInfoMessage>
{
  public string Name => "gpu_info";

  public GpuInfoMessage Collect(CancellationToken cancellationToken = default)
  {
    var gpuInfo = new GpuInfoMessage();
    try
    {
      using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
      var gpu = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
      if (gpu is not null)
      {
        gpuInfo.GpuModel = gpu["Name"]?.ToString() ?? "Unknown";
        gpuInfo.GpuMemoryMb = Convert.ToInt32((uint?)gpu["AdapterRAM"] / 1024 / 1024 ?? 0);
      }
    }catch
    {
      // Ignore exceptions and return default values
    }

    return gpuInfo;
  }
}
