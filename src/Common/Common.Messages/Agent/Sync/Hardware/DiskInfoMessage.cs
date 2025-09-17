namespace Common.Messages.Agent.Sync.Hardware;

public class DiskInfoMessage
{
  public string DiskModel { get; set; } = string.Empty;
  public long TotalDiskMb { get; set; } = 0;
}
