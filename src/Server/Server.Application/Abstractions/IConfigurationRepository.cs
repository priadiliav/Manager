using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IConfigurationRepository
{
	Task<IEnumerable<Configuration>> GetAllAsync();
	Task<Configuration?> GetAsync(long id);
	Task CreateAsync(Configuration configuration);
	Task ModifyAsync(Configuration configuration);
	Task DeleteAsync(long id);
}