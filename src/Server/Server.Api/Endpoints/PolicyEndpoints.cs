using Server.Application.Dtos.Policy;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class PolicyEndpoints
{
	public static void MapPolicyEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/api/policies", async (IPolicyService policyService) =>
		{
			var policies = await policyService.GetPoliciesAsync();
			return Results.Ok(policies);
		})
		.WithName("GetPolicies")
		.WithTags("Policies");

		app.MapGet("/api/policies/{id:long}", async (IPolicyService policyService, long id) =>
		{
			var policy = await policyService.GetPolicyAsync(id);
			return policy is not null 
					? Results.Ok(policy) 
					: Results.NotFound();
		})
		.WithName("GetPolicyById")
		.WithTags("Policies");

		app.MapPost("/api/policies", async (IPolicyService policyService, PolicyCreateRequest createRequest) =>
		{
			var createdPolicy = await policyService.CreatePolicyAsync(createRequest);
			return createdPolicy is not null
					? Results.Created($"/api/policies/{createdPolicy.Id}", createdPolicy)
					: Results.BadRequest("Failed to create policy.");
		})
		.WithName("CreatePolicy")
		.WithTags("Policies");
		
		app.MapPut("/api/policies/{id:long}", async (IPolicyService policyService, long id, PolicyModifyRequest modifyRequest) =>
		{
			var updatedPolicy = await policyService.UpdatePolicyAsync(id, modifyRequest);
			return updatedPolicy is not null 
					? Results.Ok(updatedPolicy) 
					: Results.NotFound();
		})
		.WithName("UpdatePolicy")
		.WithTags("Policies");
	}
}

