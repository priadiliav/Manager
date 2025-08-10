using Cluster.Application.Abstractions;

namespace Cluster.Application.Services;

public interface IPodService
{
    // Define methods for PodService here
}

public class PodService(IPodRepository podRepository) : IPodService
{

}
