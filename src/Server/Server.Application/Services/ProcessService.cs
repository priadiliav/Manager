using Server.Application.Abstractions;
using Server.Application.Dtos;
using Server.Application.Dtos.Process;
using Server.Domain.Models;

namespace Server.Application.Services;

public interface IProcessService
{
	/// <summary>
	/// Gets a specific process by its ID.
	/// </summary>
	/// <param name="processId"></param>
	/// <returns></returns>
	Task<ProcessDto?> GetProcessAsync(long processId);

	/// <summary>
	/// Gets all processes.
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<ProcessDto>> GetProcessesAsync();

	/// <summary>
	/// Creates a new process.
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	Task<ProcessDto?> CreateProcessAsync(ProcessCreateRequest request);

	/// <summary>
	/// Updates an existing process.
	/// </summary>
	/// <param name="processId"></param>
	/// <param name="request"></param>
	/// <returns></returns>
	Task<ProcessDto?> UpdateProcessAsync(long processId, ProcessModifyRequest request);
}

public class ProcessService (IUnitOfWork unitOfWork) : IProcessService
{
	public async Task<ProcessDto?> GetProcessAsync(long processId)
	{
		var process = await unitOfWork.Processes.GetAsync(processId);
		return process?.ToDto();
	}

	public async Task<IEnumerable<ProcessDto>> GetProcessesAsync()
	{
		var processes = await unitOfWork.Processes.GetAllAsync();
		return processes.Select(x => x.ToDto());
	}

	public async Task<ProcessDto?> CreateProcessAsync(ProcessCreateRequest request)
	{
		var processDomain = request.ToDomain();
		await unitOfWork.Processes.CreateAsync(processDomain);
		await unitOfWork.SaveChangesAsync();

		var createdProcessDto = await GetProcessAsync(processDomain.Id);
		return createdProcessDto;
	}

	public async Task<ProcessDto?> UpdateProcessAsync(long processId, ProcessModifyRequest request)
	{
		var existingProcess = await unitOfWork.Processes.GetAsync(processId);
		if (existingProcess == null)
			return null;

		var processDomain = request.ToDomain(processId);
		existingProcess.ModifyFrom(processDomain);

    // Mark all agents in configurations that use this process as not synchronized
    var configurationIds = existingProcess.Configurations.Select(c => c.ConfigurationId).ToList();
    var agentsInConfigurations = await unitOfWork.Agents.GetByConfigurationIdsAsync(configurationIds);

    MarkAgentsAsNotSynchronizedAsync(agentsInConfigurations);

		await unitOfWork.SaveChangesAsync();

		var updatedProcessDto = await GetProcessAsync(processId);
		return updatedProcessDto;
	}

  private void MarkAgentsAsNotSynchronizedAsync(IEnumerable<Agent> agentsInConfigurations)
  {
    foreach (var agent in agentsInConfigurations)
      agent.UpdateStatus(AgentStatus.NotSynchronized);
  }
}
