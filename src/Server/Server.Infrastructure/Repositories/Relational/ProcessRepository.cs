using Microsoft.EntityFrameworkCore;
using Server.Application.Abstractions;
using Server.Domain.Models;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Repositories.Relational;

public class ProcessRepository(AppDbContext dbContext) : IProcessRepository
{
	public async Task<IEnumerable<Process>> GetAllAsync()
		=> await dbContext.Processes.ToListAsync();

	public async Task<Process?> GetAsync(long id)
		=> await dbContext.Processes
				.Include(x => x.Configurations)
				.FirstOrDefaultAsync(x => x.Id == id);

	public Task CreateAsync(Process process)
	{
		dbContext.Processes.Add(process);
		return Task.CompletedTask;
	}

	public Task ModifyAsync(Process process)
	{
		dbContext.Processes.Update(process);
		return Task.CompletedTask;
	}

	public Task DeleteAsync(long id)
	{
		var process = new Process { Id = id };
		dbContext.Processes.Remove(process);
		return Task.CompletedTask;
	}
}
