using Common.Messages.Metric;

namespace Server.Application.Services;

public interface IMetricService
{
  Task CreateMetricsAsync(Guid agentId, MetricsMessage metricsMessage);
}

public class MetricService : IMetricService
{
  public async Task CreateMetricsAsync(Guid agentId, MetricsMessage metricsMessage)
  {
    // Simulate processing the metrics message
    Console.WriteLine($"Received metrics from agent {agentId}:");
    Console.WriteLine(string.Join(", ", metricsMessage.Metrics.Select(m => $"{m.Name}: {m.Value}")));
  }
}
