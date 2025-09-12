using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IAgentRepository : IRepository<Agent, Guid>
{
  Task<IEnumerable<Agent>> GetByConfigurationIdsAsync(IEnumerable<long> configurationIds);
}
