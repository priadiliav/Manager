using Common.Messages;
using Server.Application.Abstractions;
using Server.Application.Dtos.Configuration;
using Server.Application.Services;
using Server.Infrastructure.Communication;

namespace Server.Api.Endpoints;

public static class ConfigurationEndpoints
{
	public static void MapConfigurationEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/api/configurations", async (IConfigurationService service) =>
		{
			var configurations = await service.GetConfigurationsAsync();
			return Results.Ok(configurations);
		})
		.WithName("GetConfigurations")
		.WithTags("Configuration");

		app.MapGet("/api/configurations/{id:long}", async (IConfigurationService service, long id) =>
		{
			var configuration = await service.GetConfigurationAsync(id);
			return configuration is not null
					? Results.Ok(configuration)
					: Results.NotFound();
		})
		.WithName("GetConfigurationById")
		.WithTags("Configuration");

		app.MapPost("/api/configurations", async (ConfigurationCreateRequest request, IConfigurationService service) =>
		{
			var createdConfiguration = await service.CreateConfigurationAsync(request);
			return createdConfiguration is not null
					? Results.Created($"/api/configurations/{createdConfiguration.Id}", createdConfiguration)
					: Results.BadRequest("Failed to create configuration.");
		})
		.WithName("CreateConfiguration")
		.WithTags("Configuration");

		app.MapPut("/api/configurations/{id:long}", async (IConfigurationService service, long id, ConfigurationModifyRequest request) =>
		{
			var updatedConfiguration = await service.UpdateConfigurationAsync(id, request);
			return updatedConfiguration is not null
					? Results.Ok(updatedConfiguration)
					: Results.NotFound();
		})
		.WithName("UpdateConfiguration")
		.WithTags("Configuration");

		app.MapGet("/api/configuration/{id:long}/subscribe",
		async (long id, ILongPollingDispatcher<long, ConfigurationMessage> pollingService, CancellationToken ct) =>
		{
			var update = await pollingService.WaitForUpdateAsync(id, ct);
			return update == null
					? Results.NoContent()
					: Results.Ok(update);
		})
		.WithName("SubscribeConfigurationUpdates")
		.WithTags("Configuration");

    // Test endpoint for long pilling notification
    app.MapPost("/api/configuration/{id:long}/notify",
    (long id, ConfigurationMessage message, ILongPollingDispatcher<long, ConfigurationMessage> pollingService) =>
    {
        pollingService.NotifyUpdateForKey(id, message);
        return Results.Ok();
    })
    .WithName("NotifyConfigurationUpdate")
    .WithTags("Configuration");
	}
}
