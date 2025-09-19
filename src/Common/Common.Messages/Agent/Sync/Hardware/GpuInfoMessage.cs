namespace Common.Messages.Agent.Sync.Hardware;

public class GpuInfoMessage
{
  public string GpuModel { get; set; } = string.Empty;
  public int GpuMemoryMb { get; set; } = 0;

  public override string ToString()
    => $"{GpuModel}, {GpuMemoryMb} MB";
}
