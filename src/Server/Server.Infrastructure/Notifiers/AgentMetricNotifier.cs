using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Server.Application.Abstractions;
using Server.Application.Dtos.Agent.Metric;
using Server.Infrastructure.Hubs;

namespace Server.Infrastructure.Notifiers;

public class AgentMetricNotifier(
    ILogger<AgentMetricNotifier> logger,
    IHubContext<AgentHub> hubContext) : IAgentMetricNotifier
{
  public async Task NotifyAsync(Guid agentId, AgentMetricDto message)
  {
    await hubContext.Clients.Group(agentId.ToString())
        .SendAsync("AgentMetricCreated", message);
  }
}
