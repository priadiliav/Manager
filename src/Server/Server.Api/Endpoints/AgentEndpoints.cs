using System.Security.Claims;
using Common.Messages.Agent;
using Common.Messages.Agent.Sync;
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
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetAgents");

    group.MapGet("/{agentId:guid}",
        async (Guid agentId, IAgentService agentService) =>
        {
          var agent = await agentService.GetAgentAsync(agentId);
          return agent is null
              ? Results.NotFound()
              : Results.Ok(agent);
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetAgentById");

    group.MapPost("/",
        async (AgentCreateRequest request, IAgentService agentService) => {
          var createdAgent = await agentService.CreateAgentAsync(request);
          return createdAgent is null
              ? Results.BadRequest("Failed to create agent.")
              : Results.Created($"/api/agents/{createdAgent.Id}", createdAgent);
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("CreateAgent");

    group.MapPut("/sync", async (AgentSyncRequestMessage agentSyncRequestMessage, IAgentService agentService, HttpContext context) =>
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

    group.MapPut("/{agentId:guid}",
        async (Guid agentId, AgentModifyRequest request, IAgentService agentService) =>
        {
          var updatedAgent = await agentService.UpdateAgentAsync(agentId, request);
          return updatedAgent is null
              ? Results.NotFound()
              : Results.Ok(updatedAgent);
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("UpdateAgent");
  }
}
