using Agent.Application.Abstractions;
using Agent.Domain.Configs;
using Agent.Domain.Models;
using Microsoft.Extensions.Options;

namespace Agent.Infrastructure.Repositories;

public class JsonConfigurationRepository(IOptions<PathsConfig> pathsConfig)
    : JsonRepository<Configuration>(pathsConfig, cfg => cfg.Configuration), IConfigurationRepository;
