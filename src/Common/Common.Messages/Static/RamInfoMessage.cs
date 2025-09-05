namespace Common.Messages.Static;

public class RamInfoMessage
{
  public string RamModel { get; set; } = string.Empty;
  public long TotalMemoryMB { get; set; } = 0;
}
