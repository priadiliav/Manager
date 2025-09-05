namespace Common.Messages.Static;

public class GpuInfoMessage
{
  public string GpuModel { get; set; } = string.Empty;
  public int GpuMemoryMB { get; set; } = 0;
}
