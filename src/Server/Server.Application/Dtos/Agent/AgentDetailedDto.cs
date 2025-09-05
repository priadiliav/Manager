using Server.Application.Dtos.Configuration;
using Server.Application.Dtos.Hardware;

namespace Server.Application.Dtos.Agent;

public class AgentDetailedDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = string.Empty;
  public long ConfigurationId { get; init; }

  public ConfigurationDto Configuration { get; init; }
  public HardwareDto Hardware { get; init; }
}
