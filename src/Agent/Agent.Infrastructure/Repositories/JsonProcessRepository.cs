using Agent.Application.Abstractions;
using Agent.Domain.Configs;
using Agent.Domain.Models;
using Microsoft.Extensions.Options;

namespace Agent.Infrastructure.Repositories;

public class JsonProcessRepository(IOptions<PathsConfig> options) :
    JsonRepository<Process>(options, ctx => ctx.Process), IProcessRepository;
