using Microsoft.AspNetCore.SignalR;
using Server.Application.Abstractions.Notifiers;
using Server.Application.Dtos.Agent.Metric;
using Server.Application.Dtos.Agent.State;
using Server.Infrastructure.Hubs;

namespace Server.Infrastructure.Notifiers;

public class AgentNotifier(IHubContext<AgentHub> hubContext) : IAgentStateNotifier, IAgentMetricNotifier
{
  public Task NotifyAsync(Guid agentId, AgentStateDto message)
    => hubContext.Clients.Group(agentId.ToString())
        .SendAsync("ReceiveAgentState", message);

  public Task NotifyAsync(Guid agentId, AgentMetricDto message)
    => hubContext.Clients.Group(agentId.ToString())
        .SendAsync("ReceiveAgentMetric", message);
}
