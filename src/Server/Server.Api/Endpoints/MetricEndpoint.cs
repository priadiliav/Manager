using System.Security.Claims;
using Common.Messages.Metric;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class MetricEndpoint
{
	public static void MapMetricEndpoints(this IEndpointRouteBuilder app)
	{
    var group = app.MapGroup("/api/metrics")
        .WithTags("Metrics");

    group.MapPost("/publish",
        async ([FromBody] MetricRequestMessage metricsMessage, IMetricService metricService, HttpContext context) =>
        {
          var agentId = context.User.FindFirst(ClaimTypes.Name)?.Value;

          Guid.TryParse(agentId, out var agentIdGuid);

          if (agentIdGuid == Guid.Empty)
            return Results.Unauthorized();

          var response = await metricService.CreateMetricsAsync(agentIdGuid, metricsMessage);
          return response is not null
              ? Results.Ok(response)
              : Results.BadRequest();
        })
        .RequireAuthorization(policy => policy.RequireRole("Agent"))
        .WithName("PublishMetrics");
  }
}
