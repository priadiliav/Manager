using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IClusterManager
{
  Task<IEnumerable<Deployment>> GetDeploymentsAsync(string? namespaceName = null);
}
