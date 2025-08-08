namespace Common.Messages.Agent;

public class AgentLoginRequestMessage
{
  public Guid AgentId { get; init; }
  public string Secret { get; init; } = string.Empty;
}
