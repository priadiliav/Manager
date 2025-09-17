using Microsoft.AspNetCore.SignalR;

namespace Server.Infrastructure.Hubs;

public class AgentHub : Hub
{
  public Task SubscribeToAgent(Guid agentId)
    => Groups.AddToGroupAsync(Context.ConnectionId, agentId.ToString());

  public Task UnsubscribeFromAgent(Guid agentId)
    => Groups.RemoveFromGroupAsync(Context.ConnectionId, agentId.ToString());
}
