using Agent.Application.Abstractions;
using Agent.Domain.Configs;
using Microsoft.Extensions.Options;

namespace Agent.Infrastructure.Repositories;

public class JsonAgentRepository(IOptions<PathsConfig> pathsConfig)
    : JsonRepository<Domain.Models.Agent>(pathsConfig, cfg => cfg.Agent), IAgentRepository;
