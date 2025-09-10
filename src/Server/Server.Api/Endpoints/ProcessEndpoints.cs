using System.Security.Claims;
using Common.Messages.Process;
using Server.Application.Abstractions;
using Server.Application.Dtos.Process;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class ProcessEndpoints
{
	public static void MapProcessEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/processes")
        .WithTags("Processes");

    group.MapGet("/",
        async (IProcessService processService) =>
        {
          var processes = await processService.GetProcessesAsync();
          return Results.Ok(processes);
        })
        // .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetProcesses");

    group.MapGet("/{id:long}",
        async (IProcessService processService, long id) =>
        {
          var process = await processService.GetProcessAsync(id);
          return process is not null
              ? Results.Ok(process)
              : Results.NotFound();
        })
        // .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("GetProcessById");

    group.MapPost("/",
        async (IProcessService processService, ProcessCreateRequest createRequest) =>
        {
          var createdProcess = await processService.CreateProcessAsync(createRequest);
          return createdProcess is not null
              ? Results.Created($"/processes/{createdProcess.Id}", createdProcess)
              : Results.BadRequest("Failed to create process.");
        })
        // .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("CreateProcess");

    group.MapPut("/{id:long}",
        async (IProcessService processService, long id, ProcessModifyRequest modifyRequest) =>
        {
          var updatedProcess = await processService.UpdateProcessAsync(id, modifyRequest);
          return updatedProcess is not null
              ? Results.Ok(updatedProcess)
              : Results.NotFound();
        })
        // .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("UpdateProcess");

    group.MapGet("/subscribe",
        async (ILongPollingDispatcher<Guid, ProcessesMessage> pollingService, CancellationToken ct, HttpContext context) =>
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
        .WithName("SubscribeToProcessesUpdates");
  }
}
