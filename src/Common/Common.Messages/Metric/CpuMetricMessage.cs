namespace Common.Messages.Metric;

public class CpuMetricMessage : IMessage
{
  public double UsagePercentage { get; init; }
}
