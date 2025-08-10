using Server.Application.Abstractions;

namespace Server.Api.Endpoints;

public static class ClusterEndpoints
{
  public static void MapClusterEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/cluster")
        .WithTags("Cluster");

    group.MapGet("/deployments",
        async (IClusterManager clusterManager) =>
        {
          var deployments = await clusterManager.GetDeploymentsAsync();
          return Results.Ok(deployments);
        })
        .WithName("GetDeploymentsAsync");
  }
}
