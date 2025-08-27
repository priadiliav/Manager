using Common.Messages.Metric;
using Server.Application.Abstractions;
using Server.Application.Dtos;

namespace Server.Application.Services;

public interface IMetricService
{
  /// <summary>
  /// Creates metrics for a specific agent.
  /// </summary>
  /// <param name="agentId"></param>
  /// <param name="metricMessage"></param>
  /// <returns></returns>
  Task CreateMetricsAsync(Guid agentId, MetricMessage metricMessage);
}

public class MetricService(IMetricRepository metricRepository) : IMetricService
{
  public async Task CreateMetricsAsync(Guid agentId, MetricMessage metricMessage)
  {
    var metric = metricMessage.ToDomain(agentId);

    await metricRepository.CreateAsync(metric);
  }
}
