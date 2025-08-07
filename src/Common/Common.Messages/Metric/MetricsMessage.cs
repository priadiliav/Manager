namespace Common.Messages.Metric;

public class MetricsMessage : IMessage
{
  public CpuMetricMessage? Cpu { get; init; }
  public MemoryMetricMessage? Memory { get; init; }
}
