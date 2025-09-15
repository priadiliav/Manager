namespace Server.Application.Dtos.Agent.State;

public class AgentStateDto
{
  public DateTimeOffset Timestamp { get; set; }
  public string Machine { get; set; } = string.Empty;
  public string FromState { get; set; } = string.Empty;
  public string ToState { get; set; } = string.Empty;
  public string Trigger { get; set; } = string.Empty;
}
