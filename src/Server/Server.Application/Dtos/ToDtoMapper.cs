using Common.Messages.Agent.Login;
using Common.Messages.Configuration;
using Server.Application.Dtos.Agent;
using Server.Application.Dtos.Configuration;
using Server.Application.Dtos.Hardware;
using Server.Application.Dtos.Metric;
using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;
using Server.Application.Dtos.User;

namespace Server.Application.Dtos;

public static class ToDtoMapper
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

  public static AgentDetailedDto ToDetailedDto(this Domain.Models.Agent source)
    => new()
    {
        Id = source.Id,
        Name = source.Name,
        ConfigurationId = source.ConfigurationId,
        Configuration = source.Configuration.ToDto(),
        Hardware = source.Hardware.ToDto()
    };

  public static AgentCreateResponse ToCreateResponse(this Domain.Models.Agent agent, string secret)
  {
    return new AgentCreateResponse
    {
      Id = agent.Id,
      Name = agent.Name,
      ConfigurationId = agent.ConfigurationId,
      Secret = secret
    };
  }

  public static AgentLoginResponseMessage ToLoginResponse(this Domain.Models.Agent agent, string token)
  {
    return new AgentLoginResponseMessage
    {
      Token = token
    };
  }

  public static AgentModifyResponse ToModifyResponse(this Domain.Models.Agent agent)
  {
    return new AgentModifyResponse
    {
      Id = agent.Id,
      Name = agent.Name,
      ConfigurationId = agent.ConfigurationId
    };
  }
	#endregion

  #region Hardware
  public static HardwareDto ToDto(this Domain.Models.Hardware hardware)
    => new()
    {
        CpuCores = hardware.CpuCores,
        CpuModel = hardware.CpuModel,
        CpuSpeedGHz = hardware.CpuSpeedGHz,
        CpuArchitecture = hardware.CpuArchitecture,
        GpuModel = hardware.GpuModel,
        GpuMemoryMB = hardware.GpuMemoryMB,
        RamModel = hardware.RamModel,
        TotalMemoryMB = hardware.TotalMemoryMB,
        DiskModel = hardware.DiskModel,
        TotalDiskMB = hardware.TotalDiskMB,
        Id = hardware.Id,
        AgentId = hardware.AgentId
    };
  #endregion

	#region Configuration
	public static ConfigurationDto ToDto(
			this Domain.Models.Configuration configuration)
	{
		return new ConfigurationDto
		{
			Id = configuration.Id,
			Name = configuration.Name,
		};
	}

  public static ConfigurationDetailedDto ToDetailedDto(
    this Domain.Models.Configuration configuration)
  {
    return new ConfigurationDetailedDto
    {
      Id = configuration.Id,
      Name = configuration.Name,
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

  #region User
  public static UserLoginResponse ToResponse(this Domain.Models.User user, string token)
  {
    return new UserLoginResponse
    {
      Username = user.Username,
      Role = user.Role,
      Token = token
    };
  }
  #endregion

  #region Metric
  public static MetricDto ToDto(this Domain.Models.Metric metric)
  {
    return new MetricDto
    {
      AgentId = metric.AgentId,
      CpuUsage = metric.CpuUsage,
      MemoryUsage = metric.MemoryUsage,
      DiskUsage= metric.DiskUsage,
      NetworkUsage = metric.NetworkUsage,
      Timestamp = metric.Timestamp
    };
  }
  #endregion
}
