namespace Common.Messages.Agent.Login;

public class AgentLoginRequestMessage : IMessage
{
  public Guid AgentId { get; init; }
  public string Secret { get; init; } = string.Empty;
}
