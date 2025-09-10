using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;

namespace Server.Application.Dtos.Configuration;

public class ConfigurationCreateRequest
{
	public string Name { get; init; } = string.Empty;

  // todo: remove, we cant add policies and processes during creation
	public IEnumerable<PolicyInConfigurationDto> Policies { get; init; } = [];
	public IEnumerable<ProcessInConfigurationDto> Processes { get; init; } = [];
}
