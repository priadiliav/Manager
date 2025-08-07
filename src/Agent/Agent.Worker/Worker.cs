using Agent.Application.Abstractions;
using Agent.Application.Services;
using Agent.Application.States;
using Agent.Domain.Context;

namespace Agent.Worker;

public class Worker(AgentOverallStateMachine overallStateMachine) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await overallStateMachine.StartAsync();
}
