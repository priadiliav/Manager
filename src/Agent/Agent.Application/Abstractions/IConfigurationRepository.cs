using Agent.Domain.Models;

namespace Agent.Application.Abstractions;

public interface IConfigurationRepository
{
  Task<Configuration> GetAsync();
  Task SaveAsync(Configuration configuration);
}
