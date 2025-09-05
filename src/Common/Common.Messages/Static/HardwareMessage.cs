namespace Common.Messages.Static;

public class HardwareMessage : IMessage
{
  public CpuInfoMessage Cpu { get; set; }
  public GpuInfoMessage Gpu { get; set; }
  public RamInfoMessage Ram { get; set; }
  public DiskInfoMessage Disk { get; set; }
}
