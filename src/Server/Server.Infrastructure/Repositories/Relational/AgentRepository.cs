using Microsoft.EntityFrameworkCore;
using Server.Application.Abstractions;
using Server.Domain.Models;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Repositories.Relational;

public class AgentRepository(AppDbContext dbContext) : IAgentRepository
{
	public async Task<IEnumerable<Agent>> GetAllAsync()
		=> await dbContext.Agents.ToListAsync();

	public async Task<Agent?> GetAsync(Guid id)
		=> await dbContext.Agents
				.Include(x => x.Configuration)
				.FirstOrDefaultAsync(x => x.Id == id);

	public Task CreateAsync(Agent agent)
	{
		dbContext.Agents.Add(agent);
		return Task.CompletedTask;
	}

	public Task ModifyAsync(Agent agent)
	{
		dbContext.Agents.Update(agent);
		return Task.CompletedTask;
	}

	public Task DeleteAsync(Guid id)
	{
		var agent = new Agent { Id = id };
		dbContext.Agents.Remove(agent);
		return Task.CompletedTask;
	}
}
