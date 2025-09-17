using Server.Application.Dtos.Agent.Metric;

namespace Server.Application.Abstractions.Notifiers;

public interface IAgentMetricNotifier : INotifier<Guid, AgentMetricDto>;
