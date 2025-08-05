namespace Server.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
	IAgentRepository Agents { get; }
	IConfigurationRepository Configurations { get; }
	IPolicyRepository Policies { get; }
	IProcessRepository Processes { get; }
	Task<int> SaveChangesAsync();
}