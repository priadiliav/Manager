namespace Agent.Domain.Models;

public enum ProcessState
{
  Active,
  Banned
}

public class Process
{
  public string Name { get; init; } = string.Empty;
  public ProcessState ProcessState { get; init; }
}
