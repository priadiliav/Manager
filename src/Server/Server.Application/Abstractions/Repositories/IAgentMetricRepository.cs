using Server.Domain.Models;

namespace Server.Application.Abstractions.Repositories;

public interface IAgentMetricRepository
{
  Task CreateAsync(AgentMetric agentMetric);
  Task<IEnumerable<AgentMetric>> GetAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to, int limit = 50);
}
