namespace Server.Application.Dtos.Agent;

public class AgentModifyRequest
{
	public string Name { get; init; } = string.Empty;
	public long ConfigurationId { get; init; }
}