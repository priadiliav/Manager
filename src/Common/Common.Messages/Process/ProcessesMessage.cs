namespace Common.Messages.Process;

public class ProcessesMessage : IMessage
{
  public IEnumerable<ProcessMessage> Processes { get; init; } = [];
}
