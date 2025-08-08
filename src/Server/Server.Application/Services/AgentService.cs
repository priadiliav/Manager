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
  /// Logs in an agent using its ID and secret.
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  Task<AgentLoginResponseMessage?> LoginAsync(AgentLoginRequestMessage request);
}

public class AgentService (
  IJwtTokenProvider jwtTokenProvider,
  IPasswordHasher passwordHasher,
  IUnitOfWork unitOfWork) : IAgentService
{
  #region Crud
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
    var agentDomain = request.ToDomain(secretHash, secretSalt);

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
  #endregion

  #region Authentication
  public async Task<AgentLoginResponseMessage?> LoginAsync(AgentLoginRequestMessage request)
  {
    var agent = await unitOfWork.Agents.GetAsync(request.AgentId);
    if (agent is null || !passwordHasher.IsPasswordValid(request.Secret, agent.SecretHash, agent.SecretSalt))
      return null;

    var token = jwtTokenProvider.GenerateTokenForAgent(agent.Id);
    return agent.ToLoginResponse(token);
  }
  #endregion
}
