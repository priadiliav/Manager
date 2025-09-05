namespace Server.Application.Dtos.Agent;

public class AgentDto
{
	public Guid Id { get; init; }
	public string Name { get; init; } = string.Empty;
  public long ConfigurationId { get; init; }
}
