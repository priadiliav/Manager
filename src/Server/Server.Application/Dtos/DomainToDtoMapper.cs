using Common.Messages;
using Server.Application.Dtos.Agent;
using Server.Application.Dtos.Configuration;
using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;

namespace Server.Application.Dtos;

public static class DomainToDtoMapper
{
	#region Agent
	public static AgentDto ToDto(this Domain.Models.Agent agent)
	{
		return new AgentDto
		{
			Id = agent.Id,
			Name = agent.Name,
			ConfigurationId = agent.ConfigurationId
		};
	}
	#endregion

	#region Configuration
	public static ConfigurationDto ToDto(
			this Domain.Models.Configuration configuration,
			int subscriberCount = 0)
	{
		return new ConfigurationDto
		{
			Id = configuration.Id,
			Name = configuration.Name,
			SubscriberCount = subscriberCount,
			AgentIds = configuration.Agents.Select(a => a.Id).ToList(),
			Processes = configuration.Processes.Select(p => p.ToDto()).ToList(),
			Policies = configuration.Policies.Select(p => p.ToDto()).ToList()
		};
	}

  public static ConfigurationMessage ToMessage(this Domain.Models.Configuration configuration)
  {
    return new ConfigurationMessage
    {
      // To be expanded
      Name = configuration.Name,
    };
  }
	#endregion

	#region Process
	public static ProcessDto ToDto(this Domain.Models.Process process)
	{
		return new ProcessDto
		{
			Id = process.Id,
			Name = process.Name
		};
	}

	public static ProcessInConfigurationDto ToDto(this Domain.Models.ProcessInConfiguration process)
	{
		return new ProcessInConfigurationDto
		{
			ProcessState = process.ProcessState,
			ProcessId = process.ProcessId,
		};
	}
	#endregion

	#region Policy
	public static PolicyDto ToDto(this Domain.Models.Policy policy)
	{
		return new PolicyDto
		{
			Id = policy.Id,
			Name = policy.Name,
			Description = policy.Description,
			RegistryPath = policy.RegistryPath,
			RegistryValueType = policy.RegistryValueType,
			RegistryKeyType = policy.RegistryKeyType,
			RegistryKey = policy.RegistryKey
		};
	}

	public static PolicyInConfigurationDto ToDto(this Domain.Models.PolicyInConfiguration policy)
	{
		return new PolicyInConfigurationDto
		{
			PolicyId = policy.PolicyId,
			RegistryValue = policy.RegistryValue,
		};
	}
	#endregion
}
