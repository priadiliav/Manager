namespace Common.Messages.Configuration;

public class ConfigurationMessage : IMessage
{
  public string Name { get; init; } = string.Empty;
}
