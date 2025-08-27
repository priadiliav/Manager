namespace Common.Messages.Metric;

public class MetricMessage : IMessage
{
    public DateTimeOffset Timestamp { get; init; } = DateTime.UtcNow;
    public float CpuUsage { get; set; }
    public float MemoryUsage { get; set; }
    public float DiskUsage { get; set; }
}
