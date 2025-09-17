namespace Common.Messages.Agent.Sync;

public class AgentSyncRequestMessage : IMessage
{
  public AgentHardwareMessage Hardware { get; init; }
}
