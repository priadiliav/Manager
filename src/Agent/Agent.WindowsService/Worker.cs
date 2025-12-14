using Agent.WindowsService.Abstraction;

namespace Agent.WindowsService;

public class Worker(IStateMachine stateMachine) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    => await stateMachine.StartAsync();
}
