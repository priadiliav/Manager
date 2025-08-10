using Cluster.Application.Abstractions;

namespace Cluster.Application.Services;

public interface IDeploymentService
{
    // Define methods for deployment service
}

public class DeploymentService (IDeploymentRepository deploymentRepository) : IDeploymentService
{

}
