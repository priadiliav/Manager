namespace Common.Messages.Static;

public class HardwareMessage : IMessage
{
  public CpuInfoMessage Cpu { get; init; }
  public GpuInfoMessage Gpu { get; init; }
  public RamInfoMessage Ram { get; init; }
  public DiskInfoMessage Disk { get; init; }
}
