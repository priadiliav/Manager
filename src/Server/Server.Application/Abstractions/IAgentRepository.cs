using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IAgentRepository : IRepository<Agent, Guid>;
