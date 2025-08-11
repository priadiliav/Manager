using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class ClusterEndpoints
{
  public static void MapClusterEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/cluster")
        .WithTags("Cluster");

    group.MapGet("/deployments",
        async (IClusterService clusterService) =>
        {
          var deployments = await clusterService.GetDeploymentsAsync();
          return Results.Ok(deployments);
        })
        .WithName("GetDeploymentsAsync");
  }
}
