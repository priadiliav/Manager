using Cluster.Application.Abstractions;

namespace Cluster.Application.Services;

public interface IServiceService
{
    // Define methods for the service here
}

public class ServiceService(IServiceRepository serviceRepository) : IServiceService
{

}
