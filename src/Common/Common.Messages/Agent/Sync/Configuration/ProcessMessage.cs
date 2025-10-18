namespace Common.Messages.Agent.Sync.Configuration;

public enum ProcessState
{
  Active,
  Blocked
}

public class ProcessMessage : IMessage
{
  public string ProcessName { get; set; } = string.Empty;
  public ProcessState State { get; set; }
}
