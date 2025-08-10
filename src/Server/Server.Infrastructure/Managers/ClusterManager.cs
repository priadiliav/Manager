using k8s;
using Server.Application.Abstractions;
using Server.Domain.Models;

namespace Server.Infrastructure.Managers;

public class ClusterManager(IKubernetes client) : IClusterManager
{
  public async Task<IEnumerable<Deployment>> GetDeploymentsAsync(string? namespaceName = null)
  {
    var deployments = namespaceName is null
        ? await client.AppsV1.ListDeploymentForAllNamespacesAsync()
        : await client.AppsV1.ListNamespacedDeploymentAsync(namespaceName);

    var result = new List<Deployment>();

    foreach (var deployment in deployments.Items)
    {
      var labelSelector = deployment.Spec.Selector.MatchLabels;
      var selectorString = string.Join(",", labelSelector.Select(kv => $"{kv.Key}={kv.Value}"));

      var pods = await client.CoreV1.ListNamespacedPodAsync(deployment.Metadata.NamespaceProperty,
          labelSelector: selectorString);

      result.Add(new Deployment
      {
          Namespace = deployment.Metadata.NamespaceProperty ?? string.Empty,
          Name = deployment.Metadata.Name ?? string.Empty,
          Image = deployment.Spec.Template.Spec.Containers.FirstOrDefault()?.Image ?? string.Empty,
          Status = deployment.Status?.Conditions?.FirstOrDefault()?.Type ?? "Unknown",
          Pods = pods.Items.Select(pod => new Pod
          {
            Namespace = pod.Metadata.NamespaceProperty ?? string.Empty,
            Name = pod.Metadata.Name ?? string.Empty
          }).ToList()
      });
    }
    return result;
  }
}
