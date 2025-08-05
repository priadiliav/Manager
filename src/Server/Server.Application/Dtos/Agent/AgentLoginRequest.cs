namespace Server.Application.Dtos.Agent;

public class AgentLoginRequest
{
	public Guid Id { get; init; }
	public string Secret { get; init; } = string.Empty;
}