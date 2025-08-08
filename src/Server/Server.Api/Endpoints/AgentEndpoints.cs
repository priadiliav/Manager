using Common.Messages.Agent;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Dtos.Agent;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class AgentEndpoints
{

	public static void MapAgentEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/agents")
        .WithTags("Agents")
        .RequireAuthorization(policy => policy.RequireRole("User"));

    group.MapGet("/",
        async (IAgentService agentService) =>
        {
          var agents = await agentService.GetAgentsAsync();
          return Results.Ok(agents);
        })
        .WithName("GetAgents");

    group.MapGet("/{agentId:guid}",
        async (Guid agentId, IAgentService agentService) =>
        {
          var agent = await agentService.GetAgentAsync(agentId);
          return agent is null
              ? Results.NotFound()
              : Results.Ok(agent);
        })
        .WithName("GetAgentById");

    group.MapPost("/",
        async (AgentCreateRequest request, IAgentService agentService) => {
          var createdAgent = await agentService.CreateAgentAsync(request);
          return createdAgent is null
              ? Results.BadRequest("Failed to create agent.")
              : Results.Created($"/api/agents/{createdAgent.Id}", createdAgent);
        })
        .WithName("CreateAgent");

    group.MapPut("/{agentId:guid}",
        async (Guid agentId, AgentModifyRequest request, IAgentService agentService) =>
        {
          var updatedAgent = await agentService.UpdateAgentAsync(agentId, request);
          return updatedAgent is null
              ? Results.NotFound()
              : Results.Ok(updatedAgent);
        })
        .WithName("UpdateAgent");

    group.MapPost("/login",
        async ([FromBody] AgentLoginRequestMessage request, IAgentService agentService) =>
        {
          var loginResponse = await agentService.LoginAsync(request);
          return loginResponse is null
              ? Results.Unauthorized()
              : Results.Ok(loginResponse);
        })
        .WithName("AgentLogin");
  }
}
