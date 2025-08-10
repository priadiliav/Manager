namespace Server.Api.Endpoints;

public static class ClusterEndpoints
{
  public static void MapClusterEndpoints(this IEndpointRouteBuilder app)
  {
    // Define your cluster-related endpoints here
    app.MapGet("/cluster/status", () => "Cluster is running")
       .WithName("GetClusterStatus")
       .WithOpenApi();

  }
}
