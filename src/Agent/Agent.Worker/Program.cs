using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Application.States;
using Agent.Domain.Context;
using Agent.Infrastructure.Communication;
using Agent.Infrastructure.Repositories;
using Agent.Infrastructure.Supervisors;
using Agent.Worker;
using Common.Messages.Process;

var builder = Host.CreateApplicationBuilder(args);

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
    .AddSingleton<ILongPollingClient<ProcessesMessage>, LongPollingClient<ProcessesMessage>>(
        sp => new LongPollingClient<ProcessesMessage>(
            sp.GetRequiredService<ILogger<LongPollingClient<ProcessesMessage>>>(),
            sp.GetRequiredService<HttpClient>(),
            sp.GetRequiredService<AgentStateContext>(),
            "policies/subscribe"));

builder.Services.AddSingleton<IProcessRepository, ProcessRepository>();
builder.Services.AddSingleton<IProcessSupervisor, ProcessSupervisor>();
#endregion

#region Application layer configurations
builder.Services.AddSingleton<ILongPollingRunner, ProcessService>();
builder.Services.AddSingleton<IWatcherRunner, ProcessService>();
builder.Services.AddSingleton<IProcessService, ProcessService>();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
#endregion

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
