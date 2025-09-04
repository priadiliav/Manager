using Common.Messages.Static;

namespace Common.Messages.Agent;

public class AgentSyncRequestMessage : IMessage
{
  public HardwareMessage Hardware { get; set; }
}
