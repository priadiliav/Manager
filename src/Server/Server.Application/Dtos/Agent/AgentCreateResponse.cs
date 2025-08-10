namespace Server.Application.Dtos.Agent;

public class AgentCreateResponse : AgentDto
{
  public string Secret { get; init; } = string.Empty;
  public string Description => "The secret is visible only once, so make sure to save it securely. " +
                               "If you lose it, you will need to create a new agent.";
}
