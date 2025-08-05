using Server.Application.Dtos.Process;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class ProcessEndpoints
{
	public static void MapProcessEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/processes", async (IProcessService processService) =>
		{
			var processes = await processService.GetProcessesAsync();
			return Results.Ok(processes);
		})
		.WithName("GetProcesses")
		.WithTags("Processes");

		app.MapGet("/processes/{id:long}", async (IProcessService processService, long id) =>
		{
			var process = await processService.GetProcessAsync(id);
			return process is not null 
					? Results.Ok(process) 
					: Results.NotFound();
		})
		.WithName("GetProcessById")
		.WithTags("Processes");
		
		app.MapPost("/processes", async (IProcessService processService, ProcessCreateRequest createRequest) =>
		{
			var createdProcess = await processService.CreateProcessAsync(createRequest);
			return createdProcess is not null 
					? Results.Created($"/processes/{createdProcess.Id}", createdProcess)
					: Results.BadRequest("Failed to create process.");
		})
		.WithName("CreateProcess")
		.WithTags("Processes");
		
		app.MapPut("/processes/{id:long}", async (IProcessService processService, long id, ProcessModifyRequest modifyRequest) =>
		{
			var updatedProcess = await processService.UpdateProcessAsync(id, modifyRequest);
			return updatedProcess is not null 
					? Results.Ok(updatedProcess) 
					: Results.NotFound();
		})
		.WithName("UpdateProcess")
		.WithTags("Processes");
	}
}