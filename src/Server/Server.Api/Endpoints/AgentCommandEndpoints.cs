using System.Security.Claims;
using Common.Messages.Agent.Command;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Abstractions.Providers;

namespace Server.Api.Endpoints;

public static class AgentCommandEndpoints
{
  public static void MapCommandEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapGet("/api/commands/subscribe",
      async (ILongPollingDispatcher<Guid, AgentCommandMessage> dispatcher, HttpContext context) =>
      {
        // Getting the agent ID from the authenticated user
        var agentId = context.User.FindFirst(ClaimTypes.Name)?.Value;

        Guid.TryParse(agentId, out var agentIdGuid);

        if (agentIdGuid == Guid.Empty)
          return Results.Unauthorized();

        var command = await dispatcher.WaitForUpdateAsync(agentIdGuid, cancellationToken: context.RequestAborted);
        return command is null
            ? Results.NoContent()
            : Results.Ok(command);
      })
      .WithName("SubscribeCommands")
      .WithTags("Commands");

    app.MapPost("/api/commands/notify/{agentId:guid}",
        (Guid agentId, [FromBody]AgentCommandMessage command,
        ILongPollingDispatcher<Guid, AgentCommandMessage> dispatcher) =>
      {
        dispatcher.NotifyUpdateForKey(agentId, command);
        return Results.Ok();
      })
      .WithName("NotifyCommand")
      .WithTags("Commands");
  }
}
