using Microsoft.EntityFrameworkCore;
using Server.Application.Abstractions;
using Server.Domain.Models;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Repositories.Relational;

public class PolicyRepository(AppDbContext dbContext) : IPolicyRepository
{
	public async Task<IEnumerable<Policy>> GetAllAsync()
		=> await dbContext.Policies.ToListAsync();

	public Task<Policy?> GetAsync(long id)
		=> dbContext.Policies
				.Include(x => x.Configurations)
				.FirstOrDefaultAsync(x => x.Id == id);

	public Task CreateAsync(Policy policy)
	{
		dbContext.Policies.Add(policy);
		return Task.CompletedTask;
	}

	public Task ModifyAsync(Policy policy)
	{
		dbContext.Policies.Update(policy);
		return Task.CompletedTask;
	}

	public Task DeleteAsync(long id)
	{
		var policy = new Policy { Id = id };
		dbContext.Policies.Remove(policy);
		return Task.CompletedTask;
	}
}
