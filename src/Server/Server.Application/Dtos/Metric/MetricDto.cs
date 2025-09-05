namespace Server.Application.Dtos.Metric;

public class MetricDto
{
  public Guid AgentId { get; init; }
  public DateTimeOffset Timestamp { get; init; }
  public double CpuUsage { get; init; }
  public double MemoryUsage { get; init; }
  public double DiskUsage { get; init; }
  public double NetworkUsage { get; init; }
}
