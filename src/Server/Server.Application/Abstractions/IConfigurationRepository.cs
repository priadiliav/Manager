using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IConfigurationRepository : IRepository<Configuration, long>;
