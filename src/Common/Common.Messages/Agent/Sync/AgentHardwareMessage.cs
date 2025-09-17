using Common.Messages.Agent.Sync.Hardware;

namespace Common.Messages.Agent.Sync;

public class AgentHardwareMessage : IMessage
{
  public CpuInfoMessage Cpu { get; init; }
  public GpuInfoMessage Gpu { get; init; }
  public RamInfoMessage Ram { get; init; }
  public DiskInfoMessage Disk { get; init; }
}
