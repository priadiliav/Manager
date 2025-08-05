namespace Server.Application.Abstractions;

public interface IPolicyRepository
{
	Task<IEnumerable<Domain.Models.Policy>> GetAllAsync();
	Task<Domain.Models.Policy?> GetAsync(long id);
	Task CreateAsync(Domain.Models.Policy policy);
	Task ModifyAsync(Domain.Models.Policy policy);
	Task DeleteAsync(long id);
}