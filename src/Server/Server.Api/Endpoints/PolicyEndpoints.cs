using System.Security.Claims;
using Common.Messages.Policy;
using Server.Application.Abstractions;
using Server.Application.Dtos.Policy;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class PolicyEndpoints
{
	public static void MapPolicyEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/policies")
        .WithTags("Policies");

    group.MapGet("/",
        async (IPolicyService policyService) =>
        {
          var policies = await policyService.GetPoliciesAsync();
          return Results.Ok(policies);
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetPolicies");

    group.MapGet("/{id:long}",
        async (IPolicyService policyService, long id) =>
        {
          var policy = await policyService.GetPolicyAsync(id);
          return policy is not null
              ? Results.Ok(policy)
              : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetPolicyById");

    group.MapPost("/",
        async (IPolicyService policyService, PolicyCreateRequest createRequest) =>
        {
          var createdPolicy = await policyService.CreatePolicyAsync(createRequest);
          return createdPolicy is not null
              ? Results.Created($"/api/policies/{createdPolicy.Id}", createdPolicy)
              : Results.BadRequest("Failed to create policy.");
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("CreatePolicy");

    group.MapPut("/api/policies/{id:long}",
        async (IPolicyService policyService, long id, PolicyModifyRequest modifyRequest) =>
        {
          var updatedPolicy = await policyService.UpdatePolicyAsync(id, modifyRequest);
          return updatedPolicy is not null
              ? Results.Ok(updatedPolicy)
              : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("UpdatePolicy");

    group.MapGet("/subscribe",
        async (ILongPollingDispatcher<Guid, PoliciesMessage> pollingService, CancellationToken ct, HttpContext context) =>
        {
          // Getting the agent ID from the authenticated user
          var agentId = context.User.FindFirst(ClaimTypes.Name)?.Value;

          Guid.TryParse(agentId, out var agentIdGuid);

          if (agentIdGuid == Guid.Empty)
            return Results.Unauthorized();

          var update = await pollingService.WaitForUpdateAsync(agentIdGuid, ct);
          return update is null
              ? Results.NoContent()
              : Results.Ok(update);
        })
        .RequireAuthorization(policy => policy.RequireRole("Agent"))
        .WithName("SubscribeToPoliciesUpdates");
  }
}

