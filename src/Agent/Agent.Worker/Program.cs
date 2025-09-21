using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Application.States;
using Agent.Application.States.Workers;
using Agent.Application.Utils;
using Agent.Domain.Configs;
using Agent.Infrastructure.Collectors.Dynamic;
using Agent.Infrastructure.Collectors.Static;
using Agent.Infrastructure.Communication;
using Agent.Infrastructure.Repositories;
using Common.Messages.Agent.Sync.Hardware;
using Agent.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService();

#region Application layer configurations
builder.Services.AddSingleton<StateMachineWrapper>();
builder.Services.AddSingleton<AuthStateMachine>();
builder.Services.AddSingleton<SyncStateMachine>();
builder.Services.AddSingleton<SupervisorStateMachine>();
builder.Services.AddSingleton<WorkerStateMachine, MetricWorkerStateMachine>();
builder.Services.AddSingleton<OverallStateMachine>();

builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<ISyncService, SyncService>();
builder.Services.AddSingleton<IMetricService, MetricService>();
#endregion

#region Infrastructure layer configurations

// Add configuration for PathsConfig
builder.Services.AddOptions<PathsConfig>().Bind(builder.Configuration.GetSection("Paths")).ValidateOnStart();
builder.Services.AddOptions<EndpointsConfig>().Bind(builder.Configuration.GetSection("Endpoints")).ValidateOnStart();

builder.Services.AddSingleton<HttpClient>(_ => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration
        .GetSection("Endpoints").Get<EndpointsConfig>()?.BaseUrl
                          ?? throw new InvalidOperationException("BaseUrl is not configured."))
});

builder.Services.AddSingleton<ICommunicationClient, CommunicationClient>();

builder.Services.AddSingleton<IReceiverClient, ReceiverClient>();
builder.Services.AddSingleton<IPublisherClient, PublisherClient>();

builder.Services.AddSingleton<IDynamicDataCollector<double>, CpuUsageCollector>();
builder.Services.AddSingleton<IDynamicDataCollector<double>, MemoryUsageCollector>();
builder.Services.AddSingleton<IDynamicDataCollector<double>, DiskUsageCollector>();
builder.Services.AddSingleton<IDynamicDataCollector<double>, NetworkUsageCollector>();
builder.Services.AddSingleton<IDynamicDataCollector<double>, UptimeCollector>();

builder.Services.AddSingleton<IStaticDataCollector<CpuInfoMessage>, CpuInfoCollector>();
builder.Services.AddSingleton<IStaticDataCollector<RamInfoMessage>, RamInfoCollector>();
builder.Services.AddSingleton<IStaticDataCollector<GpuInfoMessage>, GpuInfoCollector>();
builder.Services.AddSingleton<IStaticDataCollector<DiskInfoMessage>, DiskInfoCollector>();

builder.Services.AddSingleton<IAgentRepository, JsonAgentRepository>();
builder.Services.AddSingleton<IConfigurationRepository, JsonConfigurationRepository>();
builder.Services.AddSingleton<IProcessRepository, JsonProcessRepository>();
builder.Services.AddSingleton<IPolicyRepository, JsonPolicyRepository>();
#endregion

builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
