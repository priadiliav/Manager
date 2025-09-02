using Common.Messages.Static;

namespace Server.Api.Endpoints;

public static class SyncEndpoints
{
  public static void MapSyncEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/sync", async (SyncMessage syncMessage, CancellationToken cancellationToken) =>
    {
      await Task.Delay(10000, cancellationToken);

      Console.WriteLine(
      $"""
       Received sync message with
             CPU info: {syncMessage.Cpu.Model},
             Cores: {syncMessage.Cpu.Cores},
             Model: {syncMessage.Cpu.Model},
             Speed: {syncMessage.Cpu.SpeedGHz} GHz
       """);

      return Results.Ok();
    })
    .RequireAuthorization(policy => policy.RequireRole("Agent"));
  }
}
