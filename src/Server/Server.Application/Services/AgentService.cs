using System.Security.Claims;
using Common.Messages.Agent;
using Server.Application.Abstractions;
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
	Task<AgentDto?> GetAgentAsync(Guid agentId);

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
	Task<AgentDto?> UpdateAgentAsync(Guid agentId, AgentModifyRequest request);

  /// <summary>
  /// Syncs an agent with the provided static information, such as hardware details.
  /// </summary>
  /// <param name="agentId"></param>
  /// <param name="message"></param>
  /// <returns></returns>
  Task<AgentDto?> SyncAgentAsync(Guid agentId, AgentSyncRequestMessage message);
}

public class AgentService (
  IPasswordHasher passwordHasher,
  IUnitOfWork unitOfWork) : IAgentService
{
	public async Task<IEnumerable<AgentDto>> GetAgentsAsync()
	{
		var agents = await unitOfWork.Agents.GetAllAsync();
		return agents.Select(x => x.ToDto());
	}

	public async Task<AgentDto?> GetAgentAsync(Guid agentId)
	{
		var agent = await unitOfWork.Agents.GetAsync(agentId);
		return agent?.ToDto();
	}

	public async Task<AgentCreateResponse?> CreateAgentAsync(AgentCreateRequest request)
  {
    var randomGuidString = Guid.NewGuid().ToString();

    var (secretHash, secretSalt) = passwordHasher.CreatePasswordHash(randomGuidString);

    var hardware = Hardware.Empty();
    var agentDomain = request.ToDomain(hardware, secretHash, secretSalt);

    await unitOfWork.Agents.CreateAsync(agentDomain);
    await unitOfWork.SaveChangesAsync();

    var createdAgentDto = await unitOfWork.Agents.GetAsync(agentDomain.Id);
		return createdAgentDto?.ToCreateResponse(randomGuidString);
	}

	public async Task<AgentDto?> UpdateAgentAsync(Guid agentId, AgentModifyRequest request)
	{
		var existingAgentDomain = await unitOfWork.Agents.GetAsync(agentId);
		if (existingAgentDomain is null)
			return null;

		var agentDomain = request.ToDomain(agentId);
		existingAgentDomain.ModifyFrom(agentDomain);

		await unitOfWork.Agents.ModifyAsync(existingAgentDomain);
		await unitOfWork.SaveChangesAsync();

		var updatedAgentDto = await GetAgentAsync(agentId);
		return updatedAgentDto;
	}

  public async Task<AgentDto?> SyncAgentAsync(Guid agentId, AgentSyncRequestMessage message)
  {
    var existingAgentDomain = await unitOfWork.Agents.GetAsync(agentId);
    if (existingAgentDomain is null)
      return null;

    // Software updates and another static agent information can be handled here in the future
    var hardwareDomain = message.Hardware.ToDomain(agentId);
    existingAgentDomain.Hardware.ModifyFrom(hardwareDomain);
    await unitOfWork.Hardware.ModifyAsync(existingAgentDomain.Hardware);

    await unitOfWork.SaveChangesAsync();

    var updatedAgentDto = await GetAgentAsync(agentId);
    return updatedAgentDto;
  }
}
