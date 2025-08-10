using System.Security.Claims;
using Common.Messages.Configuration;
using Server.Application.Abstractions;
using Server.Application.Dtos.Configuration;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class ConfigurationEndpoints
{
	public static void MapConfigurationEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/configurations")
        .WithTags("Configuration");

    group.MapGet("/",
        async (IConfigurationService service) =>
        {
          var configurations = await service.GetConfigurationsAsync();
          return Results.Ok(configurations);
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetConfigurations");

    group.MapGet("/{id:long}",
        async (IConfigurationService service, long id) =>
        {
          var configuration = await service.GetConfigurationAsync(id);
          return configuration is not null
              ? Results.Ok(configuration)
              : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetConfigurationById");

    group.MapPost("/",
        async (ConfigurationCreateRequest request, IConfigurationService service) =>
        {
          var createdConfiguration = await service.CreateConfigurationAsync(request);
          return createdConfiguration is not null
              ? Results.Created($"/api/configurations/{createdConfiguration.Id}", createdConfiguration)
              : Results.BadRequest("Failed to create configuration.");
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("CreateConfiguration");

    group.MapPut("/{id:long}",
        async (IConfigurationService service, long id, ConfigurationModifyRequest request) =>
        {
          var updatedConfiguration = await service.UpdateConfigurationAsync(id, request);
          return updatedConfiguration is not null
              ? Results.Ok(updatedConfiguration)
              : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("UpdateConfiguration");

    group.MapGet("/subscribe",
        async (ILongPollingDispatcher<Guid, ConfigurationMessage> pollingService, CancellationToken ct, HttpContext context) =>
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
        .WithName("SubscribeToConfigurationUpdates");
	}
}
