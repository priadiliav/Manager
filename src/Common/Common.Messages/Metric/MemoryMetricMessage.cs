namespace Common.Messages.Metric;

public class MemoryMetricMessage : IMessage
{
  public long TotalMemory { get; init; }
}
