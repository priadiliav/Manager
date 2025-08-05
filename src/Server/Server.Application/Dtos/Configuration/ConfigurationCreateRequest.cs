using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;

namespace Server.Application.Dtos.Configuration;

public class ConfigurationCreateRequest
{
	public string Name { get; init; } = string.Empty;
	
	public IEnumerable<PolicyInConfigurationDto> Policies { get; init; } = [];
	public IEnumerable<ProcessInConfigurationDto> Processes { get; init; } = [];
}