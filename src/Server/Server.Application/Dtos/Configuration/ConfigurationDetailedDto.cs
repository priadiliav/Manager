using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;

namespace Server.Application.Dtos.Configuration;

public class ConfigurationDetailedDto
{
  public long Id { get; init; }
  public string Name { get; init; } = string.Empty;
  public IEnumerable<Guid> AgentIds { get; init; } = [];
  public IEnumerable<PolicyInConfigurationDto> Policies { get; init; } = [];
  public IEnumerable<ProcessInConfigurationDto> Processes { get; init; } = [];
}
