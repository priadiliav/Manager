using Agent.Application.Abstractions;
using Agent.Application.Runners;
using Agent.Application.States;
using Agent.Domain.Context;
using Agent.Infrastructure.Collectors.Dynamic;
using Agent.Infrastructure.Collectors.Static;
using Agent.Infrastructure.Communication;
using Agent.Worker;
using Common.Messages.Static;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService();

#region Finite state machine configurations
builder.Services.AddSingleton<AgentStateContext>();
builder.Services.AddSingleton<AgentOverallStateMachine>();
builder.Services.AddSingleton<AgentAuthStateMachine>();
builder.Services.AddSingleton<AgentSyncStateMachine>();
builder.Services.AddSingleton<AgentWorkStateMachine>();
#endregion

#region Infrastructure layer configurations
builder.Services.AddSingleton<HttpClient>(_ => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5267/api/")
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
#endregion

#region Application layer configurations
builder.Services.AddSingleton<IWorkerRunner, ProcessReceiverRunner>();
builder.Services.AddSingleton<IWorkerRunner, MetricsPublisherRunner>();
#endregion

builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
