using Server.Domain.Resources;

namespace Server.Application.Dtos.Resources;

public class DeploymentDto
{
  public string Name { get; set; } = string.Empty;
  public string Namespace { get; set; } = string.Empty;

  public string LabelsAndSelectors { get; set; } = string.Empty;
  public string Image { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;

  public IEnumerable<PodDto> Pods { get; set; } = [];
  public IEnumerable<ServiceDto> Services { get; set; } = [];
}
