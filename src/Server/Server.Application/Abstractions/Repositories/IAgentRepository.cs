using Server.Domain.Models;

namespace Server.Application.Abstractions.Repositories;

public interface IAgentRepository : IRepository<Agent, Guid>
{
  Task<IEnumerable<Agent>> GetByConfigurationIdsAsync(IEnumerable<long> configurationIds);
}
