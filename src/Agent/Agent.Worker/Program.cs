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
builder.Services.AddSingleton<StateMachineWrapper>(sp =>
{
  var logger = sp.GetRequiredService<ILogger<StateMachineWrapper>>();
  var client = sp.GetRequiredService<ICommunicationClient>();
  return new StateMachineWrapper(logger, client, sp.GetRequiredService<AgentStateContext>());
});
builder.Services.AddSingleton<AuthStateMachine>(sp =>
{
  var wrapper = sp.GetRequiredService<StateMachineWrapper>();
  var context = sp.GetRequiredService<AgentStateContext>();
  var client = sp.GetRequiredService<ICommunicationClient>();
  return new AuthStateMachine(client, wrapper, context);
});
builder.Services.AddSingleton<SyncStateMachine>();
builder.Services.AddSingleton<WorkStateMachine>();
builder.Services.AddSingleton<OverallStateMachine>(sp =>
{
  var wrapper = sp.GetRequiredService<StateMachineWrapper>();
  var authMachine = sp.GetRequiredService<AuthStateMachine>();
  var syncMachine = sp.GetRequiredService<SyncStateMachine>();
  var workMachine = sp.GetRequiredService<WorkStateMachine>();
  var logger = sp.GetRequiredService<ILogger<OverallStateMachine>>();

  var overall = new OverallStateMachine(logger, wrapper, authMachine, syncMachine, workMachine);

  wrapper.RegisterMachine(overall.Machine);
  wrapper.RegisterMachine(authMachine.Machine);
  wrapper.RegisterMachine(syncMachine.Machine);
  wrapper.RegisterMachine(workMachine.Machine);

  return overall;
});
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
