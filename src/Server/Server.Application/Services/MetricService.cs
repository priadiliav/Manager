using Common.Messages.Metric;
using Server.Application.Abstractions;
using Server.Application.Dtos;
using Server.Application.Dtos.Metric;

namespace Server.Application.Services;

public interface IMetricService
{
  /// <summary>
  /// Creates metrics for a specific agent.
  /// </summary>
  /// <param name="agentId"></param>
  /// <param name="metricRequestMessage"></param>
  /// <returns></returns>
  Task<MetricResponseMessage?> CreateMetricsAsync(Guid agentId, MetricRequestMessage metricRequestMessage);

  /// <summary>
  /// Gets metrics for a specific agent within a given time range.
  /// </summary>
  /// <param name="agentId"></param>
  /// <param name="from"></param>
  /// <param name="to"></param>
  /// <returns></returns>
  Task<IEnumerable<MetricDto>> GetMetricsAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to);
}

public class MetricService(IMetricRepository metricRepository) : IMetricService
{
  public async Task<MetricResponseMessage?> CreateMetricsAsync(Guid agentId, MetricRequestMessage metricRequestMessage)
  {
    var metric = metricRequestMessage.ToDomain(agentId);
    await metricRepository.CreateAsync(metric);
    return new MetricResponseMessage();
  }

  public async Task<IEnumerable<MetricDto>> GetMetricsAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to)
  {
    var metrics = await metricRepository.GetMetricsAsync(agentId, from, to);
    return metrics.Select(m => m.ToDto());
  }
}
