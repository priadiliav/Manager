using System.Security.Claims;
using Common.Messages.Agent.Metric;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class AgentMetricEndpoints
{
	public static void MapAgentMetricEndpoints(this IEndpointRouteBuilder app)
	{
    var group = app.MapGroup("/api/metrics").WithTags("Agent Metrics");

    group.MapPost("/publish",
        async ([FromBody] AgentMetricRequestMessage metricsMessage, IAgentMetricService metricService, HttpContext context) =>
        {
          var agentId = context.User.FindFirst(ClaimTypes.Name)?.Value;

          Guid.TryParse(agentId, out var agentIdGuid);

          if (agentIdGuid == Guid.Empty)
            return Results.Unauthorized();

          var response = await metricService.CreateAsync(agentIdGuid, metricsMessage);
          return response is not null
              ? Results.Ok(response)
              : Results.BadRequest();
        })
        .RequireAuthorization(policy => policy.RequireRole("Agent"))
        .WithName("PublishMetrics");

    group.MapGet("/",
        async (Guid agentId, DateTimeOffset from, DateTimeOffset to, IAgentMetricService metricService) =>
    {
      var metrics = await metricService.GetAsync(agentId, from, to);

      return Results.Ok(metrics);
    }).WithName("GetMetrics");
  }
}
