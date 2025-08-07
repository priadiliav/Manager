using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Infrastructure.Communication;
using Agent.Worker;
using Common.Messages;

var builder = Host.CreateApplicationBuilder(args);

// todo: simplify
builder.Services.AddSingleton<HttpClient>(_ => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5267/api/")
});

builder.Services
    .AddSingleton<ILongPollingClient<ConfigurationMessage>, InMemoryLongPollingClient<ConfigurationMessage>>(
        sp => new InMemoryLongPollingClient<ConfigurationMessage>(
            sp.GetRequiredService<ILogger<InMemoryLongPollingClient<ConfigurationMessage>>>(),
            sp.GetRequiredService<HttpClient>(),
            "configuration/3/subscribe"));

builder.Services.AddSingleton<ILongPollingRunner, ConfigurationService>();
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
