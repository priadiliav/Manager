using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IAgentRepository
{
	Task<IEnumerable<Agent>> GetAllAsync();
	Task<Agent?> GetAsync(Guid id);
	Task CreateAsync(Agent agent);
	Task ModifyAsync(Agent agent);
	Task DeleteAsync(Guid id);
}