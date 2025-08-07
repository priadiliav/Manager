namespace Common.Messages.Agent;

public class LoginRequestMessage
{
  public Guid AgentId { get; init; }
  public string Secret { get; init; } = string.Empty;
}
