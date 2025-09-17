namespace Common.Messages.Agent.Sync.Hardware;

public class RamInfoMessage
{
  public string RamModel { get; set; } = string.Empty;
  public long TotalMemoryMb { get; set; } = 0;
}
