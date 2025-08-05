namespace Server.Application.Dtos.Agent;

public class AgentCreateRequest
{
	public string Name { get; init; } = string.Empty;
	public long ConfigurationId { get; init; }
}