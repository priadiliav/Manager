using Server.Application.Abstractions;
using Server.Application.Abstractions.Repositories;
using Server.Application.Dtos;
using Server.Application.Dtos.Configuration;
using Server.Domain.Models;

namespace Server.Application.Services;

public interface IConfigurationService
{
	/// <summary>
	/// Gets a specific configuration by its ID.
	/// </summary>
	/// <param name="configurationId"></param>
	/// <returns></returns>
	Task<ConfigurationDetailedDto?> GetConfigurationAsync(long configurationId);

	/// <summary>
	/// Gets all configurations.
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<ConfigurationDto>> GetConfigurationsAsync();

	/// <summary>
	/// Creates a new configuration.
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	Task<ConfigurationDetailedDto?> CreateConfigurationAsync(ConfigurationCreateRequest request);

	/// <summary>
	/// Updates an existing configuration.
	/// </summary>
	/// <param name="configurationId"></param>
	/// <param name="request"></param>
	/// <returns></returns>
	Task<ConfigurationDetailedDto?> UpdateConfigurationAsync(long configurationId, ConfigurationModifyRequest request);
}

public class ConfigurationService (IUnitOfWork unitOfWork) : IConfigurationService
{
	public async Task<ConfigurationDetailedDto?> GetConfigurationAsync(long configurationId)
	{
		var configuration = await unitOfWork.Configurations.GetAsync(configurationId);
		return configuration?.ToDetailedDto();
	}

	public async Task<IEnumerable<ConfigurationDto>> GetConfigurationsAsync()
	{
		var configurations = await unitOfWork.Configurations.GetAllAsync();
		return configurations.Select(x => x.ToDto());
	}

	public async Task<ConfigurationDetailedDto?> CreateConfigurationAsync(ConfigurationCreateRequest request)
	{
		var configurationDomain = request.ToDomain();
		await unitOfWork.Configurations.CreateAsync(configurationDomain);
		await unitOfWork.SaveChangesAsync();

		var createdConfigurationDto = await GetConfigurationAsync(configurationDomain.Id);
		return createdConfigurationDto;
	}

	public async Task<ConfigurationDetailedDto?> UpdateConfigurationAsync(long configurationId, ConfigurationModifyRequest request)
	{
		var existingConfigurationDomain = await unitOfWork.Configurations.GetAsync(configurationId);
		if (existingConfigurationDomain is null)
			return null;

		var configurationDomain = request.ToDomain(configurationId);
		existingConfigurationDomain.ModifyFrom(configurationDomain);

    // Mark all associated agents as not synchronized
    MarkAgentsAsNotSynchronized(existingConfigurationDomain.Agents);

		await unitOfWork.SaveChangesAsync();

		var updatedConfigurationDto = await GetConfigurationAsync(configurationId);
		return updatedConfigurationDto;
	}

  private static void MarkAgentsAsNotSynchronized(IEnumerable<Agent> agents)
  {
    foreach (var agent in agents)
      agent.UpdateStatus(AgentStatus.NotSynchronized);
  }
}
