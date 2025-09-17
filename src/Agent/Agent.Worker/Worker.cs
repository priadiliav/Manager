using Agent.Application.States;

namespace Agent.Worker;

public class Worker(OverallStateMachine overallStateMachine) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await Task.Delay(1000, stoppingToken);

    await overallStateMachine.StartAsync();
  }

  public override async Task StopAsync(CancellationToken cancellationToken)
  {
    await overallStateMachine.StopAsync();

    await base.StopAsync(cancellationToken);
  }
}
