using Common.Messages.Agent.State;
using Server.Application.Resources;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class AgentStateEndpoints
{
  public static void MapAgentStateEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/states").WithTags("Agent State");

    group.MapPut("/{agentId:guid}", async (Guid agentId,
      AgentStateChangeRequestMessage agentStateChangeRequestMessage,
      IAgentStateService agentStateService) =>
      {
        var response = await agentStateService
            .CreateAsync(agentId, agentStateChangeRequestMessage);

        return Results.Ok(response);
      })
      .WithTags("Agent State")
      .WithName("CreateAgentState");

      group.MapGet("/template", () => Results.Ok(AgentStateTree.TemplateTree))
      .WithTags("Agent State")
      .WithName("GetAgentStateTemplate");

    group.MapGet("/{agentId:guid}", async (Guid agentId,
        DateTimeOffset from,
        DateTimeOffset to,
        int limit,
        IAgentStateService agentStateService) =>
    {
      var states = await agentStateService.GetAsync(agentId, from, to, limit);

      return Results.Ok(states);
    })
    .WithTags("Agent State")
    .WithName("GetAgentStates");
  }
}
