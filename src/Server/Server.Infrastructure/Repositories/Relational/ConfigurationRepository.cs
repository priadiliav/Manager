using Microsoft.EntityFrameworkCore;
using Server.Application.Abstractions;
using Server.Application.Abstractions.Repositories;
using Server.Domain.Models;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Repositories.Relational;

public class ConfigurationRepository(AppDbContext dbContext) : IConfigurationRepository
{
	public async Task<IEnumerable<Configuration>> GetAllAsync()
		=> await dbContext.Configurations.ToListAsync();

	public Task<Configuration?> GetAsync(long id)
		=> dbContext.Configurations
				.Include(x => x.Agents)
				.Include(x => x.Policies)
				.Include(x => x.Processes)
				.FirstOrDefaultAsync(x => x.Id == id);

	public Task CreateAsync(Configuration configuration)
	{
		dbContext.Configurations.Add(configuration);
		return Task.CompletedTask;
	}

	public Task ModifyAsync(Configuration configuration)
	{
		dbContext.Configurations.Update(configuration);
		return Task.CompletedTask;
	}

	public Task DeleteAsync(long id)
	{
		var configuration = new Configuration { Id = id };
		dbContext.Configurations.Remove(configuration);
		return Task.CompletedTask;
	}
}
