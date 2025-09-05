using Common.Messages.Static;

namespace Common.Messages.Agent.Sync;

public class AgentSyncRequestMessage : IMessage
{
  public HardwareMessage Hardware { get; set; }
}
