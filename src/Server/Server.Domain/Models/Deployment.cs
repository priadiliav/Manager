namespace Server.Domain.Models;

public class Deployment
{
  public string Namespace { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Image { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public IEnumerable<Pod> Pods { get; set; } = [];
}
