namespace Common.Messages.Agent.Sync;

public class AgentSyncMessage : IMessage
{
  public AgentHardwareMessage Hardware { get; init; }

  public override string ToString()
    => $"Hardware: {Hardware}";
}
