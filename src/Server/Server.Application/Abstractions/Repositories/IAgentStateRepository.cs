using Server.Domain.Models;

namespace Server.Application.Abstractions.Repositories;

public interface IAgentStateRepository
{
  Task CreateAsync(AgentState agentState);
  Task<AgentState?> GetCurrentStateAsync(Guid agentId);
  Task<IEnumerable<AgentState>> GetAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to, int limit = 1000);
}
