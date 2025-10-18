using Common.Messages.Agent.Login;
using Common.Messages.Agent.Metric;
using Common.Messages.Agent.State;
using Common.Messages.Agent.Sync.Configuration;
using Server.Application.Dtos.Agent;
using Server.Application.Dtos.Agent.Hardware;
using Server.Application.Dtos.Agent.Metric;
using Server.Application.Dtos.Agent.State;
using Server.Application.Dtos.Configuration;
using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;
using Server.Application.Dtos.User;

namespace Server.Application.Dtos;

public static class ToDtoMapper
{
	#region Agent
	public static AgentDto ToDto(this Domain.Models.Agent agent, bool isOnline)
	{
		return new AgentDto
		{
			Id = agent.Id,
			Name = agent.Name,
      IsOnline = isOnline,
      Status = agent.Status,
      LastStatusChangeAt = agent.LastStatusChangeAt,
			ConfigurationId = agent.ConfigurationId
		};
	}

  public static AgentDetailedDto ToDetailedDto(this Domain.Models.Agent source, bool isOnline)
    => new()
    {
        Id = source.Id,
        Name = source.Name,
        IsOnline = isOnline,
        ConfigurationId = source.ConfigurationId,
        Status = source.Status,
        LastStatusChangeAt = source.LastStatusChangeAt,
        Configuration = source.Configuration.ToDto(),
        Hardware = source.Hardware.ToDto()
    };

  public static AgentCreateResponse ToCreateResponse(this Domain.Models.Agent agent, string secret)
  {
    return new AgentCreateResponse
    {
      Id = agent.Id,
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

  #region Agent Metric
  public static AgentMetricDto ToDto(this Domain.Models.AgentMetric agentMetric)
  {
    return new AgentMetricDto
    {
        AgentId = agentMetric.AgentId,
        CpuUsage = agentMetric.CpuUsage,
        MemoryUsage = agentMetric.MemoryUsage,
        DiskUsage= agentMetric.DiskUsage,
        NetworkUsage = agentMetric.NetworkUsage,
        Timestamp = agentMetric.Timestamp
    };
  }

  public static AgentMetricResponseMessage ToMessage(this Domain.Models.AgentMetric agentMetric)
    => new()
    {
        // to be expanded
    };
  #endregion

  #region Agent State
  public static AgentStateDto ToDto(this Domain.Models.AgentState agentState)
    => new()
    {
        Timestamp = agentState.Timestamp,
        FromState = agentState.FromState,
        ToState = agentState.ToState,
        Machine = agentState.Machine,
        Trigger = agentState.Trigger,
        Details = agentState.Details
    };

  public static AgentStateChangeResponseMessage ToMessage(this Domain.Models.AgentState agentState)
    => new()
    {
        // to be expanded
    };
  #endregion

  #region Agent Hardware
  private static AgentHardwareDto ToDto(this Domain.Models.AgentHardware agentHardware)
    => new()
    {
        CpuCores = agentHardware.CpuCores,
        CpuModel = agentHardware.CpuModel,
        CpuSpeedGHz = agentHardware.CpuSpeedGHz,
        CpuArchitecture = agentHardware.CpuArchitecture,
        GpuModel = agentHardware.GpuModel,
        GpuMemoryMB = agentHardware.GpuMemoryMB,
        RamModel = agentHardware.RamModel,
        TotalMemoryMB = agentHardware.TotalMemoryMB,
        DiskModel = agentHardware.DiskModel,
        TotalDiskMB = agentHardware.TotalDiskMB,
        Id = agentHardware.Id,
        AgentId = agentHardware.AgentId
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
    return new ConfigurationMessage()
    {
      Processes =[],
      Policies = []
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
}
