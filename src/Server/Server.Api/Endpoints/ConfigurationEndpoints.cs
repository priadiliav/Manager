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
        .WithTags("Configuration")
        .RequireAuthorization(policy => policy.RequireRole("User"));

    group.MapGet("/",
        async (IConfigurationService service) =>
        {
          var configurations = await service.GetConfigurationsAsync();
          return Results.Ok(configurations);
        })
        .WithName("GetConfigurations");

    app.MapGet("/{id:long}",
        async (IConfigurationService service, long id) =>
        {
          var configuration = await service.GetConfigurationAsync(id);
          return configuration is not null
              ? Results.Ok(configuration)
              : Results.NotFound();
        })
        .WithName("GetConfigurationById");

    app.MapPost("/",
        async (ConfigurationCreateRequest request, IConfigurationService service) =>
        {
          var createdConfiguration = await service.CreateConfigurationAsync(request);
          return createdConfiguration is not null
              ? Results.Created($"/api/configurations/{createdConfiguration.Id}", createdConfiguration)
              : Results.BadRequest("Failed to create configuration.");
        })
        .WithName("CreateConfiguration");

    app.MapPut("/{id:long}",
        async (IConfigurationService service, long id, ConfigurationModifyRequest request) =>
        {
          var updatedConfiguration = await service.UpdateConfigurationAsync(id, request);
          return updatedConfiguration is not null
              ? Results.Ok(updatedConfiguration)
              : Results.NotFound();
        })
        .WithName("UpdateConfiguration");

    app.MapGet("/subscribe",
        async (ILongPollingDispatcher<Guid, ConfigurationMessage> pollingService, CancellationToken ct, HttpContext context) =>
        {
          // Getting the agent ID from the authenticated user
          var agentId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
