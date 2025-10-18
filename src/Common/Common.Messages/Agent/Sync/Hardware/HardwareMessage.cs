using Common.Messages.Agent.Sync.Hardware;

namespace Common.Messages.Agent.Sync.Hardware;

public class HardwareMessage : IMessage
{
  public CpuInfoMessage Cpu { get; init; }
  public GpuInfoMessage Gpu { get; init; }
  public RamInfoMessage Ram { get; init; }
  public DiskInfoMessage Disk { get; init; }

  public override string ToString()
   => $"CPU: {Cpu}, RAM: {Ram}, DISK: {Disk}, GPU: {Gpu}";
}
