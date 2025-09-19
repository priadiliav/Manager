namespace Agent.Application.Abstractions;

public interface IAgentRepository
{
  /// <summary>
  /// Retrieves the agent information from the repository.
  /// </summary>
  /// <returns></returns>
  Task<Domain.Models.Agent> GetAsync();

  /// <summary>
  /// Stores or updates the agent information in the repository.
  /// </summary>
  /// <param name="agent"></param>
  /// <returns></returns>
  Task SaveAsync(Domain.Models.Agent agent);
}
