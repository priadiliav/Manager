namespace Common.Messages.Metric;

//todo: replace by record
public class MetricRequestMessage : IMessage
{
    public DateTimeOffset Timestamp { get; init; } = DateTime.UtcNow;
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public double NetworkUsage { get; set; }
    public double Uptime { get; set; }
}
