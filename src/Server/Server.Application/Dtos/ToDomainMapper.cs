using Common.Messages.Agent.State;
using Common.Messages.Agent.Sync;
using Common.Messages.Metric;
using Server.Application.Dtos.Agent;
using Server.Application.Dtos.Configuration;
using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;
using Server.Application.Dtos.User;
using Server.Domain.Models;

namespace Server.Application.Dtos;

public static class ToDomainMapper
{
	#region Agent
	public static Domain.Models.Agent ToDomain(this AgentCreateRequest source,
      Domain.Models.AgentHardware hardware,
      byte[] secretHash,
      byte[] secretSalt)
		=> new()
		{
				Name = source.Name,
				ConfigurationId = source.ConfigurationId,
        SecretHash = secretHash,
        SecretSalt = secretSalt,
        Hardware = hardware
		};

	public static Domain.Models.Agent ToDomain(this AgentModifyRequest source, Guid id)
		=> new()
		{
				Id = id,
				Name = source.Name,
				ConfigurationId = source.ConfigurationId
		};
	#endregion

  #region Agent Metrics
  public static AgentMetric ToDomain(this AgentMetricRequestMessage source, Guid agentId)
  {
    return new AgentMetric
    {
        AgentId = agentId,
        Timestamp = source.Timestamp,
        CpuUsage = source.CpuUsage,
        MemoryUsage = source.MemoryUsage,
        NetworkUsage = source.NetworkUsage,
        Uptime = source.Uptime,
        DiskUsage = source.DiskUsage
    };
  }
  #endregion

  #region Agent State
  public static AgentState ToDomain(this AgentStateChangeRequestMessage source, Guid agentId)
    => new()
    {
      AgentId = agentId,
      Timestamp = source.Timestamp,
      Machine = source.Machine,
      FromState = source.FromState,
      ToState = source.ToState,
      Trigger = source.Trigger
    };
  #endregion

  #region Agent Hardware
  public static Domain.Models.AgentHardware ToDomain(this AgentHardwareMessage source, Guid agentId)
    => new()
    {
      CpuCores      = source.Cpu.CpuCores,
      CpuModel      = source.Cpu.CpuModel,
      CpuSpeedGHz   = source.Cpu.CpuSpeedGHz,
      CpuArchitecture = source.Cpu.CpuArchitecture,
      GpuModel      = source.Gpu.GpuModel,
      GpuMemoryMB   = source.Gpu.GpuMemoryMb,
      RamModel      = source.Ram.RamModel,
      TotalMemoryMB = source.Ram.TotalMemoryMb,
      DiskModel     = source.Disk.DiskModel,
      TotalDiskMB   = source.Disk.TotalDiskMb,
      AgentId       = agentId
    };
  #endregion

	#region Configuration
	public static Domain.Models.Configuration ToDomain(this ConfigurationDto source)
		=> new()
		{
				Id = source.Id,
				Name = source.Name,
		};

	public static Domain.Models.Configuration ToDomain(this ConfigurationCreateRequest source)
		=> new()
		{
				Name = source.Name,
				Processes = source.Processes
						.Select(p => p.ToDomain()).ToList(),
				Policies = source.Policies
						.Select(p => p.ToDomain()).ToList()
		};

	public static Domain.Models.Configuration ToDomain(this ConfigurationModifyRequest source, long id)
		=> new()
		{
				Id = id,
				Name = source.Name,
				Processes = source.Processes
						.Select(p => p.ToDomain()).ToList(),
				Policies = source.Policies
						.Select(p => p.ToDomain()).ToList()
		};
	#endregion

	#region Process
	public static Domain.Models.Process ToDomain(this ProcessDto source)
		=> new()
		{
				Id = source.Id,
				Name = source.Name
		};

	public static Domain.Models.Process ToDomain(this ProcessCreateRequest source)
		=> new()
		{
				Name = source.Name
		};

	public static Domain.Models.Process ToDomain(this ProcessModifyRequest source, long id)
		=> new()
		{
				Id = id,
				Name = source.Name
		};

	private static ProcessInConfiguration ToDomain(this ProcessInConfigurationDto source)
		=> new()
		{
				ProcessId = source.ProcessId,
				ProcessState = source.ProcessState
		};
	#endregion

	#region Policy
	public static Domain.Models.Policy ToDomain(this PolicyDto source)
		=> new()
		{
				Id = source.Id,
				Name = source.Name,
				Description = source.Description,
				RegistryPath = source.RegistryPath,
				RegistryValueType = source.RegistryValueType,
				RegistryKeyType = source.RegistryKeyType,
				RegistryKey = source.RegistryKey
		};

	public static Domain.Models.Policy ToDomain(this PolicyCreateRequest source)
	 		=> new()
		{
				Name = source.Name,
				Description = source.Description,
				RegistryPath = source.RegistryPath,
				RegistryValueType = source.RegistryValueType,
				RegistryKeyType = source.RegistryKeyType,
				RegistryKey = source.RegistryKey
		};

	public static Domain.Models.Policy ToDomain(this PolicyModifyRequest source, long id)
	 		=> new()
		{
				Id = id,
				Name = source.Name,
				Description = source.Description,
				RegistryPath = source.RegistryPath,
				RegistryValueType = source.RegistryValueType,
				RegistryKeyType = source.RegistryKeyType,
				RegistryKey = source.RegistryKey
		};

	private static PolicyInConfiguration ToDomain(this PolicyInConfigurationDto source)
		=> new()
		{
				PolicyId = source.PolicyId,
				RegistryValue = source.RegistryValue
		};
	#endregion

  #region User
  public static Domain.Models.User ToDomain(this UserRegisterRequest source, byte[] passwordHash, byte[] passwordSalt)
  {
    return new Domain.Models.User
    {
      Username = source.Username,
      Role = "User",
      PasswordHash = passwordHash,
      PasswordSalt = passwordSalt
    };
  }
  #endregion
}
