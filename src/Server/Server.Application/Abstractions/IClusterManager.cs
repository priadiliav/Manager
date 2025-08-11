using Server.Domain.Models;
using Server.Domain.Resources;

namespace Server.Application.Abstractions;

public interface IClusterManager
{
  Task<IEnumerable<Deployment>> GetDeploymentsAsync(string? namespaceName = null, Dictionary<string, string>? labelSelector = null);
  Task<IEnumerable<Pod>> GetPodsAsync(string? namespaceName = null, Dictionary<string, string>? labelSelector = null);
  Task<IEnumerable<Service>> GetServicesAsync(string? namespaceName = null, Dictionary<string, string>? labelSelector = null);
}
