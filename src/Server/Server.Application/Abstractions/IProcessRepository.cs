namespace Server.Application.Abstractions;

public interface IProcessRepository
{
	Task<IEnumerable<Domain.Models.Process>> GetAllAsync();
	Task<Domain.Models.Process?> GetAsync(long id);
	Task CreateAsync(Domain.Models.Process process);
	Task ModifyAsync(Domain.Models.Process process);
	Task DeleteAsync(long id);
}