using Server.Domain.Abstractions;

namespace Server.Domain.Resources;

public class Deployment : Resource
{
  public string Image { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public Dictionary<string, string> LabelsAndSelectors { get; set; } = new();
}
