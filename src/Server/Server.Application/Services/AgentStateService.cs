using Common.Messages.Agent.State;
using Server.Application.Abstractions;
using Server.Application.Dtos;
using Server.Application.Dtos.Agent.State;

namespace Server.Application.Services;

public interface IAgentStateService
{
  /// <summary>
  /// Creates an agent state entry.
  /// </summary>
  /// <param name="agentStateChangeRequestMessage"></param>
  /// <param name="agentId"></param>
  /// <returns></returns>
  Task<AgentStateChangeResponseMessage> CreateAsync(Guid agentId, AgentStateChangeRequestMessage agentStateChangeRequestMessage);

  /// <summary>
  /// Gets agent states within a specified time range.
  /// </summary>
  /// <param name="agentId"></param>
  /// <param name="from"></param>
  /// <param name="to"></param>
  /// <param name="limit"></param>
  /// <returns></returns>
  Task<IEnumerable<AgentStateDto>> GetAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to, int limit = 1000);
}

public class AgentStateService (
    IAgentStateNotifier agentStateNotifier,
    IAgentStateRepository agentStateRepository) : IAgentStateService
{
  public async Task<AgentStateChangeResponseMessage> CreateAsync(Guid agentId, AgentStateChangeRequestMessage agentStateChangeRequestMessage)
  {
    var agentState = agentStateChangeRequestMessage.ToDomain(agentId);
    var agentStateDto = agentState.ToDto();
    var agentStateMessage = agentState.ToMessage();

    await agentStateRepository.CreateAsync(agentState);
    await agentStateNotifier.NotifyAsync(agentId, agentStateDto);

    return agentStateMessage;
  }

  public async Task<IEnumerable<AgentStateDto>> GetAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to, int limit = 1000)
  {
    var states = await agentStateRepository.GetAsync(agentId, from, to, limit);
    return states.Select(s => s.ToDto());
  }
}
