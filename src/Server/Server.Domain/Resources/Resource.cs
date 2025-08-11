namespace Server.Domain.Resources;

public abstract class Resource
{
  public string Name { get; set; } = string.Empty;
  public string Namespace { get; set; } = string.Empty;
}
