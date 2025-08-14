namespace Common.Messages.Metric;

public class MetricsMessage : IMessage
{
  public List<MetricMessage> Metrics { get; set; } =  [];
}
