namespace Agent.Domain.Models;

public class Configuration
{
  public int MetricPublishIntervalSeconds { get; set; }
  public int MetricPublishRetryCount { get; set; }
  public int MetricPublishRetryDelaySeconds { get; set; }

  public int CommandPollIntervalSeconds { get; set; }
  public int CommandExecutionRetryCount { get; set; }
  public int CommandExecutionRetryDelaySeconds { get; set; }
}
