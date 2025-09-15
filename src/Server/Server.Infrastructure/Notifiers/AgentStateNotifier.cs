using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Server.Application.Abstractions;
using Server.Application.Dtos.Agent.State;
using Server.Infrastructure.Hubs;

namespace Server.Infrastructure.Notifiers;

public class AgentStateNotifier(
    ILogger<AgentStateNotifier> logger,
    IHubContext<AgentHub> hubContext) : IAgentStateNotifier
{
  public async Task NotifyAsync(Guid agentId, AgentStateDto message)
  {
    await hubContext.Clients.Group(agentId.ToString())
        .SendAsync("ReceiveAgentStateChange", message);
  }
}
