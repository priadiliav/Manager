using Server.Application.Abstractions;
using Server.Application.Dtos.Resources;

namespace Server.Application.Services;

public interface IClusterService
{
  Task<IEnumerable<DeploymentDto>> GetDeploymentsAsync();
}

public class ClusterService(IClusterManager clusterManager) : IClusterService
{
  public async Task<IEnumerable<DeploymentDto>> GetDeploymentsAsync()
  {
    var deployments = await clusterManager.GetDeploymentsAsync();

    var deploymentDtos = new List<DeploymentDto>();
    foreach (var deployment in deployments)
    {
      var pods = await clusterManager.GetPodsAsync(deployment.Namespace, deployment.LabelsAndSelectors);
      var services = await clusterManager.GetServicesAsync(deployment.Namespace, deployment.LabelsAndSelectors);

      var deploymentDto = new DeploymentDto
      {
        Name = deployment.Name,
        Namespace = deployment.Namespace,
        Image = deployment.Image,
        Status = deployment.Status,
        Pods = pods.Select(pod => new PodDto
        {
          Name = pod.Name,
          Namespace = pod.Namespace,
          Status = pod.Status
        }).ToList(),
        Services = services.Select(service => new ServiceDto
        {
          Name = service.Name,
          Namespace = service.Namespace,
          Type = service.Type
        }).ToList()
      };

      deploymentDtos.Add(deploymentDto);
    }

    return deploymentDtos;
  }
}
