using Server.Application.Abstractions;
using Server.Application.Dtos;
using Server.Application.Dtos.Agent;

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
	Task<AgentDto?> CreateAgentAsync(AgentCreateRequest request);
	
	/// <summary>
	/// Updates an existing agent.
	/// </summary>
	/// <param name="agentId"></param>
	/// <param name="request"></param>
	/// <returns></returns>
	Task<AgentDto?> UpdateAgentAsync(Guid agentId, AgentModifyRequest request);
}

public class AgentService (IUnitOfWork unitOfWork) : IAgentService
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

	public async Task<AgentDto?> CreateAgentAsync(AgentCreateRequest request)
	{
		var agentDomain = request.ToDomain();
		await unitOfWork.Agents.CreateAsync(agentDomain);
		await unitOfWork.SaveChangesAsync();

		var createdAgentDto = await GetAgentAsync(agentDomain.Id);
		return createdAgentDto;
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
}