namespace Agent.Domain.Context;

public class AgentStateContext
{
  public static Guid Id => Guid.Parse("01993df8-fef4-784d-a677-d51ee89c7e6d");
  public static Guid Secret => Guid.Parse("88692b37-bb69-4513-a02b-583b62c5df88");

  public string AuthenticationToken { get; set; } = string.Empty;
  public CancellationTokenSource CancellationTokenSource { get; set; } = new();
}
