namespace Common.Messages.Static;

public class DiskInfoMessage
{
  public string DiskModel { get; set; } = string.Empty;
  public long TotalDiskMB { get; set; } = 0;
}
