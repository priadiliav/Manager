using Common.Messages.Agent.State;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Server.Infrastructure.Hubs;

// todo: auth
//[Authorize]
public class AgentHub : Hub
{
  public async Task SubscribeToAgent(Guid agentId)
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, agentId.ToString());
  }

  public async Task UnsubscribeFromAgent(Guid agentId)
  {
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, agentId.ToString());
  }

  public async Task SendStateChange(Guid agentId, AgentStateChangeRequestMessage message)
  {
    await Clients.Group(agentId.ToString()).SendAsync("ReceiveAgentStateChange", message);
  }
}
