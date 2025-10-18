using Common.Messages.Agent.Sync.Hardware;

namespace Common.Messages.Agent.Sync;

public class AgentSyncMessage : IMessage
{
  public HardwareMessage Hardware { get; init; }

  public override string ToString()
    => $"Hardware: {Hardware}";
}
