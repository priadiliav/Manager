using Server.Application.Dtos.Agent;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class AgentEndpoints
{
	public static void MapAgentEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/api/agents", async (IAgentService agentService) =>
		{
			var agents = await agentService.GetAgentsAsync();
			return Results.Ok(agents);
		})
		.WithName("GetAgents")
		.WithTags("Agents");

		app.MapGet("/api/agents/{agentId:guid}", async (Guid agentId, IAgentService agentService) =>
		{
			var agent = await agentService.GetAgentAsync(agentId);
			return agent is null 
					? Results.NotFound()
					: Results.Ok(agent);
		})
		.WithName("GetAgentById")
		.WithTags("Agents");

		app.MapPost("/api/agents", async (AgentCreateRequest request, IAgentService agentService) =>
		{
			var createdAgent = await agentService.CreateAgentAsync(request);
			return createdAgent is null 
					? Results.BadRequest("Failed to create agent.")
					: Results.Created($"/api/agents/{createdAgent.Id}", createdAgent);
		})
		.WithName("CreateAgent")
		.WithTags("Agents");

		app.MapPut("/api/agents/{agentId:guid}", async (Guid agentId, AgentModifyRequest request, IAgentService agentService) =>
		{
			var updatedAgent = await agentService.UpdateAgentAsync(agentId, request);
			return updatedAgent is null 
					? Results.NotFound()
					: Results.Ok(updatedAgent); 
		})
		.WithName("UpdateAgent")
		.WithTags("Agents");
	}
}