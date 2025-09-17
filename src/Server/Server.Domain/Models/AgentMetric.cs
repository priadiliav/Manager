namespace Server.Domain.Models;

public class AgentMetric
{
  public Guid AgentId { get; set; }
  public double CpuUsage { get; set; }
  public double MemoryUsage { get; set; }
  public double NetworkUsage { get; set; }
  public double DiskUsage { get; set; }
  public double Uptime { get; set; }

  public DateTimeOffset Timestamp { get; set; }
}
