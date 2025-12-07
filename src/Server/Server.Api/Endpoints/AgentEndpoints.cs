using System.Security.Claims;
using Common.Messages.Agent.Command;
using Common.Messages.Agent.Sync;
using Server.Application.Abstractions.Providers;
using Server.Application.Dtos.Agent;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class AgentEndpoints
{
	public static void MapAgentEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/agents")
        .WithTags("Agents");

    group.MapGet("/",
        async (IAgentService agentService) =>
        {
          var agents = await agentService.GetAgentsAsync();
          return Results.Ok(agents);
        })
        // .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetAgents");

    group.MapGet("/{agentId:guid}",
        async (Guid agentId, IAgentService agentService) =>
        {
          var agent = await agentService.GetAgentAsync(agentId);
          return agent is null
              ? Results.NotFound()
              : Results.Ok(agent);
        })
        // .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetAgentById");

    group.MapPost("/",
        async (AgentCreateRequest request, IAgentService agentService) => {
          var createdAgent = await agentService.CreateAgentAsync(request);
          return createdAgent is null
              ? Results.BadRequest("Failed to create agent.")
              : Results.Created($"/api/agents/{createdAgent.Id}", createdAgent);
        })
        // .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("CreateAgent");

     group.MapPut("/sync", async (AgentSyncMessage agentSyncRequestMessage, IAgentService agentService, HttpContext context) =>
        {
          // Getting the agent ID from the authenticated user
          var agentId = context.User.FindFirst(ClaimTypes.Name)?.Value;

          Guid.TryParse(agentId, out var agentIdGuid);

          if (agentIdGuid == Guid.Empty)
            return Results.Unauthorized();

          var result = await agentService.SyncAgentAsync(agentIdGuid, agentSyncRequestMessage);
          return result is null
              ? Results.NotFound()
              : Results.Ok(result);
        })
        .RequireAuthorization(policy => policy.RequireRole("Agent"))
        .WithName("SyncAgents");

     group.MapGet("/sync/subscribe",
         async (ILongPollingDispatcher<Guid, ServerSyncMessage> dispatcher, HttpContext context) =>
         {
           // Getting the agent ID from the authenticated user
           var agentId = context.User.FindFirst(ClaimTypes.Name)?.Value;

           Guid.TryParse(agentId, out var agentIdGuid);

           if (agentIdGuid == Guid.Empty)
             return Results.Unauthorized();

           var syncMessage =
               await dispatcher.WaitForUpdateAsync(agentIdGuid, cancellationToken: context.RequestAborted);
           return syncMessage is null
               ? Results.NoContent()
               : Results.Ok(syncMessage);
         })
         .WithName("SubscribeAgentSync");

      group.MapPut("/sync/notify/{agentId:guid}",
          async (Guid agentId, IAgentService agentService, ILongPollingDispatcher<Guid, ServerSyncMessage> dispatcher) =>
          {
            var syncMessage = await agentService.SyncAgentAsync(agentId, null);
            if (syncMessage is null)
              return Results.BadRequest();

            dispatcher.NotifyUpdateForKey(agentId, syncMessage);
            return Results.Ok();
          })
          // .RequireAuthorization(policy => policy.RequireRole("User"))
          .WithName("NotifyAgentSync");

      group.MapPut("/{agentId:guid}",
          async (Guid agentId, AgentModifyRequest request, IAgentService agentService) =>
          {
            var updatedAgent = await agentService.UpdateAgentAsync(agentId, request);
            return updatedAgent is null
                ? Results.NotFound()
                : Results.Ok(updatedAgent);
          })
          // .RequireAuthorization(policy => policy.RequireRole("User"))
          .WithName("UpdateAgent");

      // Worker Command Endpoints
      group.MapPost("/{agentId:guid}/command", (Guid agentId, CommandRequestMessage command, ILongPollingDispatcher<Guid, CommandRequestMessage> commandDispatcher) =>
          {
            commandDispatcher.NotifyUpdateForKey(agentId, command);
            return Task.FromResult(Results.Accepted());
          })
          // .RequireAuthorization(policy => policy.RequireRole("User"))
          .WithName("SendWorkerCommand");

      group.MapGet("/commands",
          async (ILongPollingDispatcher<Guid, CommandRequestMessage> commandDispatcher, HttpContext context) =>
          {
            // Getting the agent ID from the authenticated user
            var agentId = context.User.FindFirst(ClaimTypes.Name)?.Value;

            Guid.TryParse(agentId, out var agentIdGuid);

            if (agentIdGuid == Guid.Empty)
              return Results.Unauthorized();

            var command =
                await commandDispatcher.WaitForUpdateAsync(agentIdGuid, cancellationToken: context.RequestAborted);
            return command is null
                ? Results.NoContent()
                : Results.Ok(command);
          })
          .RequireAuthorization(policy => policy.RequireRole("Agent"))
          .WithName("PollWorkerCommands");

      group.MapPut("/command-response",
          async (CommandResponseMessage response, HttpContext context) =>
          {
            var agentId = context.User.FindFirst(ClaimTypes.Name)?.Value;

            Guid.TryParse(agentId, out var agentIdGuid);

            if (agentIdGuid == Guid.Empty)
              return Results.Unauthorized();

            return Results.Ok(new { agentId = agentIdGuid, response.Success, response.Message });
          })
          .RequireAuthorization(policy => policy.RequireRole("Agent"))
          .WithName("ReceiveCommandResponse");
  }
}
