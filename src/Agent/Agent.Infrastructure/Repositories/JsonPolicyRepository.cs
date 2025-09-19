using Agent.Application.Abstractions;
using Agent.Domain.Configs;
using Agent.Domain.Models;
using Microsoft.Extensions.Options;

namespace Agent.Infrastructure.Repositories;

public class JsonPolicyRepository(IOptions<PathsConfig> options) :
    JsonRepository<Policy>(options, ctx => ctx.Policy), IPolicyRepository;
