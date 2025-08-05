using Server.Domain.Models;

namespace Server.Application.Dtos.Policy;

public class PolicyDto
{
	public long Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public string Description { get; init; } = string.Empty;
	public string RegistryPath { get; init; } = string.Empty;
	public RegistryValueType RegistryValueType { get; init; }
	public RegistryKeyType RegistryKeyType { get; init; }
	public string RegistryKey { get; init; } = string.Empty;
}