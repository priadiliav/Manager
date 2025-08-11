using k8s;
using Server.Application.Abstractions;
using Server.Domain.Resources;

namespace Server.Infrastructure.Managers;

public class K8ClusterManager(IKubernetes client) : IClusterManager
{
    public async Task<IEnumerable<Deployment>> GetDeploymentsAsync(string? namespaceName = null, Dictionary<string, string>? labelSelector = null)
    {
        var selector = labelSelector != null && labelSelector.Any()
            ? string.Join(",", labelSelector.Select(kv => $"{kv.Key}={kv.Value}"))
            : null;

        var deployments = namespaceName is null
            ? await client.AppsV1.ListDeploymentForAllNamespacesAsync(labelSelector: selector)
            : await client.AppsV1.ListNamespacedDeploymentAsync(namespaceName, labelSelector: selector);

        return deployments.Items.Select(deployment => new Deployment
        {
            Namespace = deployment.Metadata.NamespaceProperty ?? string.Empty,
            Name = deployment.Metadata.Name ?? string.Empty,
            LabelsAndSelectors = deployment.Metadata.Labels != null
                ? new Dictionary<string, string>(deployment.Metadata.Labels)
                : new Dictionary<string, string>(),
            Image = deployment.Spec.Template.Spec.Containers.FirstOrDefault()?.Image ?? "Unknown",
            Status = deployment.Status.Conditions?.FirstOrDefault()?.Type ?? "Unknown",
        }).ToList();
    }

    public async Task<IEnumerable<Pod>> GetPodsAsync(string? namespaceName = null, Dictionary<string, string>? labelSelector = null)
    {
        var selector = labelSelector != null && labelSelector.Any()
            ? string.Join(",", labelSelector.Select(kv => $"{kv.Key}={kv.Value}"))
            : null;

        var pods = namespaceName is null
            ? await client.CoreV1.ListPodForAllNamespacesAsync(labelSelector: selector)
            : await client.CoreV1.ListNamespacedPodAsync(namespaceName, labelSelector: selector);

        return pods.Items.Select(pod => new Pod
        {
            Namespace = pod.Metadata.NamespaceProperty ?? string.Empty,
            Name = pod.Metadata.Name ?? string.Empty,
            Status = pod.Status.Phase ?? "Unknown",
        }).ToList();
    }

    public async Task<IEnumerable<Service>> GetServicesAsync(string? namespaceName = null, Dictionary<string, string>? labelSelector = null)
    {
        var selector = labelSelector != null && labelSelector.Any()
            ? string.Join(",", labelSelector.Select(kv => $"{kv.Key}={kv.Value}"))
            : null;

        var services = namespaceName is null
            ? await client.CoreV1.ListServiceForAllNamespacesAsync(labelSelector: selector)
            : await client.CoreV1.ListNamespacedServiceAsync(namespaceName, labelSelector: selector);

        return services.Items.Select(service => new Service
        {
            Namespace = service.Metadata.NamespaceProperty ?? string.Empty,
            Name = service.Metadata.Name ?? string.Empty,
            Type = service.Spec.Type ?? "Unknown",
        }).ToList();
    }
}
