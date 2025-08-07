using Agent.Application.Services;

namespace Agent.Worker;

public class Worker(
    IConfigurationService configurationService,
		ILogger<Worker> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
    logger.LogInformation("Worker started.");

    var configurationLongPollingRunner = configurationService.StartListeningAsync(stoppingToken);

    await Task.WhenAll(configurationLongPollingRunner);

    logger.LogInformation("Worker stopped.");
	}
}
