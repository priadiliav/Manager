using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;

namespace Server.Application.Dtos.Configuration;

public class ConfigurationDto
{
	public long Id { get; init; }
	public string Name { get; init; } = string.Empty;
}
