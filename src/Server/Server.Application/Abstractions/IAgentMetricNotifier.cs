using Server.Application.Dtos.Agent.Metric;

namespace Server.Application.Abstractions;

public interface IAgentMetricNotifier
{
  Task NotifyAsync(Guid agentId, AgentMetricDto message);
}
