using Agent.Application.Abstractions;
using Common.Messages.Process;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

public interface IProcessService : ILongPollingRunner
{
}

public class ProcessService : IProcessService
{
  private readonly ILogger<ProcessService> _logger;
  private readonly ILongPollingClient<ProcessesMessage> _longPollingClient;

  public ProcessService(
    ILongPollingClient<ProcessesMessage> longPollingClient,
    ILogger<ProcessService> logger)
  {
  _longPollingClient = longPollingClient ?? throw new ArgumentNullException(nameof(longPollingClient));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  private Task HandleProcessesUpdateAsync(ProcessesMessage message)
  {
    _logger.LogInformation("Received process update: {Message}", message);

    // Handle the process update message here
    // This could involve updating local state, notifying other components, etc.
    return Task.CompletedTask;
  }

  // Starts listening for processes updates using the long polling client.
  public Task StartListeningAsync(CancellationToken cancellationToken)
    => _longPollingClient.StartListeningAsync(HandleProcessesUpdateAsync, cancellationToken);
}
