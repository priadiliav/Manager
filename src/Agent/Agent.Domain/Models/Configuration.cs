namespace Agent.Domain.Models;

public class Configuration
{
  public int MetricPublishIntervalSeconds { get; set; }
  public int MetricPublishRetryCount { get; set; }
  public int MetricPublishRetryDelaySeconds { get; set; }
  public int SyncPollIntervalSeconds { get; set; }
  public int SyncExecutionRetryCount { get; set; }
  public int SyncExecutionRetryDelaySeconds { get; set; }
}
