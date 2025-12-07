namespace Common.Messages.Agent.Sync.Configuration;

public class ConfigurationMessage : IMessage
{
  public IEnumerable<ProcessMessage> Processes { get; set; } = [];
  public IEnumerable<PolicyMessage> Policies { get; set; } = [];
}
