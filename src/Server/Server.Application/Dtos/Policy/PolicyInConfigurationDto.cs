namespace Server.Application.Dtos.Policy;

public class PolicyInConfigurationDto
{
	public long PolicyId { get; init; }
	public string RegistryValue { get; init; } = string.Empty;
}