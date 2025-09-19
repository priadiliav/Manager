using Common.Messages.Agent.Metric;
using Server.Application.Abstractions.Notifiers;
using Server.Application.Abstractions.Repositories;
using Server.Application.Dtos;
using Server.Application.Dtos.Agent.Metric;

namespace Server.Application.Services;

public interface IAgentMetricService
{
  /// <summary>
  /// Creates metrics for a specific agent.
  /// </summary>
  /// <param name="agentId"></param>
  /// <param name="agentMetricRequestMessage"></param>
  /// <returns></returns>
  Task<AgentMetricResponseMessage?> CreateAsync(Guid agentId, AgentMetricRequestMessage agentMetricRequestMessage);

  /// <summary>
  /// Gets metrics for a specific agent within a given time range.
  /// </summary>
  /// <param name="agentId"></param>
  /// <param name="from"></param>
  /// <param name="to"></param>
  /// <returns></returns>
  Task<IEnumerable<AgentMetricDto>> GetAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to);
}

public class AgentMetricService(
    IAgentMetricNotifier agentMetricNotifier,
    IAgentMetricRepository agentMetricRepository) : IAgentMetricService
{
  public async Task<AgentMetricResponseMessage?> CreateAsync(Guid agentId, AgentMetricRequestMessage agentMetricRequestMessage)
  {
    var metric = agentMetricRequestMessage.ToDomain(agentId);
    var metricDto = metric.ToDto();
    var metricMessageResponse = metric.ToMessage();

    await agentMetricRepository.CreateAsync(metric);
    await agentMetricNotifier.NotifyAsync(agentId, metricDto);

    return metricMessageResponse;
  }

  public async Task<IEnumerable<AgentMetricDto>> GetAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to)
  {
    var metrics = await agentMetricRepository.GetAsync(agentId, from, to);
    return metrics.Select(m => m.ToDto());
  }
}
