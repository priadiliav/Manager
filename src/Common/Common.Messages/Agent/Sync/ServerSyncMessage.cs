using Common.Messages.Agent.Sync.Configuration;

namespace Common.Messages.Agent.Sync;

public class ServerSyncMessage : IMessage
{
  public ConfigurationMessage? Configuration { get; set; }
}
