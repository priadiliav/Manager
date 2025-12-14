using Agent.WindowsService;
using Agent.WindowsService.Abstraction;
using Agent.WindowsService.Application;
using Agent.WindowsService.Application.Services;
using Agent.WindowsService.Config;
using Agent.WindowsService.Infrastructure.Executors;
using Agent.WindowsService.Infrastructure.Metric;
using Agent.WindowsService.Infrastructure.Store;
using Serilog;

Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Information()
  .WriteTo.Console()
  .WriteTo.File(PathConfig.LogsFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
  .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IStateMachine, StateMachine>();
builder.Services.AddSingleton<IMetricService, MetricService>();
builder.Services.AddSingleton<IInstructionService, InstructionService>();

builder.Services.AddSerilog();
builder.Services.AddSingleton<IConfigurationStore, JsonConfigurationStore>();
builder.Services.AddSingleton<ISecretStore, DpapiSecretStore>();
builder.Services.AddSingleton<IMetricCollector, MetricCollector>();
builder.Services.AddSingleton<IMetricStore, JsonMetricStore>();
builder.Services.AddSingleton<IInstructionStore, JsonInstructionStore>();
builder.Services.AddSingleton<IInstructionExecutor, ShellExecutor>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
