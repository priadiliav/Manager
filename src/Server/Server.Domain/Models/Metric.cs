namespace Server.Domain.Models;

public class Metric
{
  public Guid AgentId { get; set; }
  public DateTimeOffset Timestamp { get; set; }
  public float CpuUsage { get; set; }
  public float MemoryUsage { get; set; }
  public float DiskUsage { get; set; }
}
