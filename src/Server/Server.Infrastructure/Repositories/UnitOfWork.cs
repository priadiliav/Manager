using Server.Application.Abstractions;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Repositories;

public class UnitOfWork(
	AppDbContext context,
	IPolicyRepository policyRepository,
	IConfigurationRepository configurationRepository,
	IAgentRepository agentRepository,
	IProcessRepository processRepository) : IUnitOfWork
{
	public IAgentRepository Agents { get; } = agentRepository;
	public IConfigurationRepository Configurations { get; } = configurationRepository;
	public IPolicyRepository Policies { get; } = policyRepository;
	public IProcessRepository Processes { get; } = processRepository;

	public async Task<int> SaveChangesAsync()
	{
		return await context.SaveChangesAsync();
	}

	public void Dispose()
	{
		context.Dispose();
	}
}