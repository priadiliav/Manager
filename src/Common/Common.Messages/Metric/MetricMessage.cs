namespace Common.Messages.Metric;

public class MetricMessage : IMessage
{
  public string Name { get; init; } = string.Empty;
  public string Value { get; init; } = string.Empty;
}
