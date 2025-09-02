using Agent.Application.Abstractions;
using Agent.Application.Publishers;
using Agent.Application.Receivers;
using Agent.Application.Services;
using Agent.Application.States;
using Agent.Domain.Context;
using Agent.Infrastructure.Collectors;
using Agent.Infrastructure.Communication;
using Agent.Worker;
using Common.Messages.Metric;
using Common.Messages.Process;
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

builder.Services
    .AddSingleton<ILongPollingClient<ProcessesMessage>, LongPollingClient<ProcessesMessage>>(
        sp => new LongPollingClient<ProcessesMessage>(
            sp.GetRequiredService<ILogger<LongPollingClient<ProcessesMessage>>>(),
            sp.GetRequiredService<HttpClient>(),
            sp.GetRequiredService<AgentStateContext>(),
            "processes/subscribe"));

builder.Services
    .AddSingleton<IPublisherClient<MetricMessage>, PublisherClient<MetricMessage>>(
        sp => new PublisherClient<MetricMessage>(
            sp.GetRequiredService<ILogger<PublisherClient<MetricMessage>>>(),
            sp.GetRequiredService<HttpClient>(),
            sp.GetRequiredService<AgentStateContext>(),
            "metrics/publish"));

builder.Services.AddSingleton<IDynamicDataCollector<double>, CpuUsageCollector>();
builder.Services.AddSingleton<IDynamicDataCollector<double>, MemoryUsageCollector>();
builder.Services.AddSingleton<IDynamicDataCollector<double>, DiskUsageCollector>();
builder.Services.AddSingleton<IDynamicDataCollector<double>, NetworkUsageCollector>();
builder.Services.AddSingleton<IDynamicDataCollector<double>, UptimeCollector>();

builder.Services.AddSingleton<IStaticDataCollector<CpuInfoMessage>, CpuInfoDataCollector>();
#endregion

#region Application layer configurations
builder.Services.AddSingleton<IReceiverRunner, ProcessReceiver>();
builder.Services.AddSingleton<IPublisherRunner, MetricsPublisher>();
builder.Services.AddSingleton<IAuthenticationService, AuthService>();
#endregion

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
