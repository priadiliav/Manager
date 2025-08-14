using Agent.Application.Abstractions;
using Agent.Application.Dtos;
using Common.Messages.Process;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

public interface IProcessService : ILongPollingRunner
{
}

public class ProcessService(
  ILogger<ProcessService> logger,
  ILongPollingClient<ProcessesMessage> longPollingClient)
  : IProcessService
{

  private async Task HandleProcessesUpdateAsync(ProcessesMessage message)
  {
    logger.LogInformation("Received process update: {Message}", message);

    var processesDomain = message.Processes.Select(p => p.ToDomain()).ToList();

    logger.LogInformation("Converted to domain processes: {Processes}", processesDomain);
  }

  public Task ListenOnceAsync(CancellationToken cancellationToken)
    => longPollingClient.ListenAsync(HandleProcessesUpdateAsync, cancellationToken);
}
