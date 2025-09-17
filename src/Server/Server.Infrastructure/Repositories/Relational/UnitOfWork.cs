using Server.Application.Abstractions;
using Server.Application.Abstractions.Repositories;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Repositories.Relational;

public class UnitOfWork(
	AppDbContext context,
	IPolicyRepository policyRepository,
	IConfigurationRepository configurationRepository,
	IAgentRepository agentRepository,
	IProcessRepository processRepository,
  IUserRepository userRepository) : IUnitOfWork
{
	public IAgentRepository Agents { get; } = agentRepository;
	public IConfigurationRepository Configurations { get; } = configurationRepository;
	public IPolicyRepository Policies { get; } = policyRepository;
	public IProcessRepository Processes { get; } = processRepository;
  public IUserRepository Users { get; } = userRepository;

  public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
	public void Dispose() => context.Dispose();
}
