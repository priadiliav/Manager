namespace Common.Messages.Agent;

public class AgentLoginResponseMessage : IMessage
{
  public string Token { get; init; } = string.Empty;
}
