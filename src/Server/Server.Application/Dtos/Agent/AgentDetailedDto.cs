using Server.Application.Dtos.Agent.Hardware;
using Server.Application.Dtos.Configuration;
using Server.Domain.Models;

namespace Server.Application.Dtos.Agent;

public class AgentDetailedDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = string.Empty;
  public long ConfigurationId { get; init; }
  public bool IsSynchronized { get; init; }

  public AgentStatus Status { get; init; }
  public DateTimeOffset? LastStatusChangeAt { get; init; }
  public ConfigurationDto Configuration { get; init; } = null!;
  public AgentHardwareDto AgentHardware { get; init; } = null!;
}
