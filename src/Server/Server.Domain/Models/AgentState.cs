namespace Server.Domain.Models;

public class AgentState
{
  public Guid AgentId { get; set; }
  public DateTimeOffset Timestamp { get; set; }
  public string Machine { get; set; } = string.Empty;
  public string FromState { get; set; } = string.Empty;
  public string ToState { get; set; } = string.Empty;
  public string Trigger { get; set; } = string.Empty;
  public string Details { get; set; } = string.Empty;
}
