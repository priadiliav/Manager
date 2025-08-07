namespace Common.Messages.Process;

public class ProcessMessage : IMessage
{
  public string Name { get; init; } = string.Empty;
  public int State { get; init; }
}
