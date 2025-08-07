namespace Agent.Domain.Context;

public class AgentStateContext
{
  public Guid Id { get; set; }
  public string AuthenticationToken { get; set; } = string.Empty;
  public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
}
