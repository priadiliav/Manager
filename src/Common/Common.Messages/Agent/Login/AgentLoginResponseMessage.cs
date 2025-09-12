namespace Common.Messages.Agent.Login;

public class AgentLoginResponseMessage : IMessage
{
  public string Token { get; init; } = string.Empty;
}
