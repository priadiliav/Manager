using Common.Messages.Agent.Sync;
using Server.Application.Abstractions.Providers;
using Server.Application.Abstractions.Repositories;
using Server.Application.Dtos;
using Server.Application.Dtos.Agent;
using Server.Domain.Models;

namespace Server.Application.Services;

public interface IAgentService
{
	/// <summary>
	/// Gets all agents.
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<AgentDto>> GetAgentsAsync();

	/// <summary>
	/// Gets a specific agent by its ID.
	/// </summary>
	/// <param name="agentId"></param>
	/// <returns></returns>
	Task<AgentDetailedDto?> GetAgentAsync(Guid agentId);

	/// <summary>
	/// Creates a new agent.
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	Task<AgentCreateResponse?> CreateAgentAsync(AgentCreateRequest request);

	/// <summary>
	/// Updates an existing agent.
	/// </summary>
	/// <param name="agentId"></param>
	/// <param name="request"></param>
	/// <returns></returns>
	Task<AgentModifyResponse?> UpdateAgentAsync(Guid agentId, AgentModifyRequest request);

  /// <summary>
  /// Syncs an agent with the provided static information, such as agentHardware details.
  /// </summary>
  /// <param name="agentId"></param>
  /// <param name="message"></param>
  /// <returns></returns>
  Task<ServerSyncMessage?> SyncAgentAsync(Guid agentId, AgentSyncMessage? message);
}

public class AgentService (
  ILongPollingDispatcher<Guid, ServerSyncMessage> longPollingDispatcher,
  IPasswordHasher passwordHasher,
  IUnitOfWork unitOfWork) : IAgentService
{
	public async Task<IEnumerable<AgentDto>> GetAgentsAsync()
	{
		var agents = await unitOfWork.Agents.GetAllAsync();

    // Agent is fully online only if it has an active long-polling connection on the sync channel
		return agents.Select(x => x.ToDto(longPollingDispatcher.IsKeySubscribed(x.Id)));
	}

	public async Task<AgentDetailedDto?> GetAgentAsync(Guid agentId)
	{
		var agent = await unitOfWork.Agents.GetAsync(agentId);

    // Agent is fully online only if it has an active long-polling connection on the sync channel
    var isOnline = longPollingDispatcher.IsKeySubscribed(agentId);
		return agent?.ToDetailedDto(isOnline);
	}

	public async Task<AgentCreateResponse?> CreateAgentAsync(AgentCreateRequest request)
  {
    var randomGuidString = Guid.NewGuid().ToString();

    var (secretHash, secretSalt) = passwordHasher.CreatePasswordHash(randomGuidString);

    var hardware = AgentHardware.Empty();
    var agentDomain = request.ToDomain(hardware, secretHash, secretSalt);

    // New agents are not synchronized until they check in for the first time
    agentDomain.UpdateStatus(AgentStatus.NotSynchronized);

    await unitOfWork.Agents.CreateAsync(agentDomain);
    await unitOfWork.SaveChangesAsync();

    var createdAgentDto = await unitOfWork.Agents.GetAsync(agentDomain.Id);
		return createdAgentDto?.ToCreateResponse(randomGuidString);
	}

	public async Task<AgentModifyResponse?> UpdateAgentAsync(Guid agentId, AgentModifyRequest request)
	{
		var existingAgentDomain = await unitOfWork.Agents.GetAsync(agentId);
		if (existingAgentDomain is null)
			return null;

		var agentDomain = request.ToDomain(agentId);
		existingAgentDomain.ModifyFrom(agentDomain);

		await unitOfWork.SaveChangesAsync();

		var updatedAgentDto = await unitOfWork.Agents.GetAsync(agentId);
		return updatedAgentDto?.ToModifyResponse();
	}

  public async Task<ServerSyncMessage?> SyncAgentAsync(Guid agentId, AgentSyncMessage? message)
  {
    var existingAgentDomain = await unitOfWork.Agents.GetAsync(agentId);
    if (existingAgentDomain is null)
      return null;

    // Another static agent information can be handled here in the future
    if (message is not null)
    {
      var hardwareDomain = message.Hardware.ToDomain(agentId);
      existingAgentDomain.Hardware.ModifyFrom(hardwareDomain);
      existingAgentDomain.UpdateStatus(AgentStatus.Ok);
    }
    else
    {
      // If no message is provided, then the agent is just synchronizing server state
      existingAgentDomain.UpdateStatus(AgentStatus.Ok);
    }

    await unitOfWork.SaveChangesAsync();

    return new ServerSyncMessage();
  }
}
