using Server.Application.Dtos.Agent.State;

namespace Server.Application.Abstractions;

public interface IAgentStateNotifier
{
  Task NotifyAsync(Guid agentId, AgentStateDto message);
}
