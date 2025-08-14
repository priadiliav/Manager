using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Application.States;
using Agent.Domain.Context;
using Agent.Infrastructure.Collectors;
using Agent.Infrastructure.Communication;
using Agent.Worker;
using Common.Messages.Metric;
using Common.Messages.Process;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService();

#region Finite state machine configurations
builder.Services.AddSingleton<AgentStateContext>();
builder.Services.AddSingleton<AgentOverallStateMachine>();
builder.Services.AddSingleton<AgentAuthStateMachine>();
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
    .AddSingleton<IPublisherClient<MetricsMessage>, PublisherClient<MetricsMessage>>(
        sp => new PublisherClient<MetricsMessage>(
            sp.GetRequiredService<ILogger<PublisherClient<MetricsMessage>>>(),
            sp.GetRequiredService<HttpClient>(),
            sp.GetRequiredService<AgentStateContext>(),
            "metrics/publish"));

builder.Services.AddSingleton<IMetricCollector, CpuMetricCollector>();
builder.Services.AddSingleton<IMetricCollector, MemoryMetricCollector>();
#endregion

#region Application layer configurations
builder.Services.AddSingleton<ILongPollingRunner, ProcessService>();
builder.Services.AddSingleton<IProcessService, ProcessService>();

builder.Services.AddSingleton<IMetricService, MetricService>();
builder.Services.AddSingleton<IPublisherRunner, MetricService>();

builder.Services.AddSingleton<IAuthenticationService, AuthService>();
#endregion

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
