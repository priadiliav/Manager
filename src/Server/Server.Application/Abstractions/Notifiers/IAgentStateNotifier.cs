using Server.Application.Dtos.Agent.State;

namespace Server.Application.Abstractions.Notifiers;

public interface IAgentStateNotifier : INotifier<Guid, AgentStateDto>;
