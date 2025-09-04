using Common.Messages.Agent;
using Common.Messages.Metric;
using Common.Messages.Static;
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
	public static Domain.Models.Agent ToDomain(this AgentDto source)
		=> new()
		{
			Id = source.Id,
			Name = source.Name,
			ConfigurationId = source.ConfigurationId,
		};

	public static Domain.Models.Agent ToDomain(this AgentCreateRequest source,
      Hardware hardware,
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

  #region Hardware
  public static Domain.Models.Hardware ToDomain(this HardwareMessage source, Guid agentId)
    => new()
    {
      CpuCores      = source.CpuCores,
      CpuModel      = source.CpuModel,
      CpuSpeedGHz   = source.CpuSpeedGHz,
      CpuArchitecture = source.CpuArchitecture,
      GpuModel      = source.GpuModel,
      GpuMemoryMB   = source.GpuMemoryMB,
      RamModel      = source.RamModel,
      TotalMemoryMB = source.TotalMemoryMB,
      DiskModel     = source.DiskModel,
      TotalDiskMB   = source.TotalDiskMB
    };
  #endregion

	#region Configuration
	public static Domain.Models.Configuration ToDomain(this ConfigurationDto source)
		=> new()
		{
				Id = source.Id,
				Name = source.Name,
				Agents = source.AgentIds
						.Select(id => new Domain.Models.Agent { Id = id }).ToList(),
				Processes = source.Processes
						.Select(p => p.ToDomain(source.Id)).ToList(),
				Policies = source.Policies
						.Select(p => p.ToDomain(source.Id)).ToList()
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

	private static Domain.Models.ProcessInConfiguration ToDomain(this ProcessInConfigurationDto source, long configurationId)
		=> new()
		{
				ProcessId = source.ProcessId,
				ProcessState = source.ProcessState,
				ConfigurationId = configurationId
		};

	private static Domain.Models.ProcessInConfiguration ToDomain(this ProcessInConfigurationDto source)
		=> new()
		{
				ProcessId = source.ProcessId,
				ProcessState = source.ProcessState,
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

	private static Domain.Models.PolicyInConfiguration ToDomain(this PolicyInConfigurationDto source, long configurationId)
		=> new()
		{
				PolicyId = source.PolicyId,
				RegistryValue = source.RegistryValue,
				ConfigurationId = configurationId
		};


	private static Domain.Models.PolicyInConfiguration ToDomain(this PolicyInConfigurationDto source)
		=> new()
		{
				PolicyId = source.PolicyId,
				RegistryValue = source.RegistryValue,
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

  #region Metrics

  public static Domain.Models.Metric ToDomain(this MetricMessage source, Guid agentId)
  {
    return new Domain.Models.Metric
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
}
