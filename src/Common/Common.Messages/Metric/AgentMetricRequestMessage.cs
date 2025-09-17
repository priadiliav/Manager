namespace Common.Messages.Metric;

//todo: replace by record
public class AgentMetricRequestMessage : IMessage
{
    public DateTimeOffset Timestamp { get; init; } = DateTime.UtcNow;
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public double NetworkUsage { get; set; }
    public double Uptime { get; set; }

    public override string ToString()
        => $"Timestamp: {Timestamp}, CPU: {CpuUsage}%, Memory: {MemoryUsage}%, Disk: {DiskUsage}%, Network: {NetworkUsage} B/s, Uptime: {Uptime}s";
}
