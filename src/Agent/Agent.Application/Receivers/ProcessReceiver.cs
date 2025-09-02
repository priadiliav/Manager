using Agent.Application.Abstractions;
using Common.Messages.Process;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Receivers;

public class ProcessReceiver(
  ILogger<ProcessReceiver> logger,
  ILongPollingClient<ProcessesMessage> longPollingClient) : IReceiverRunner
{
  public Task ReceiveAsync(CancellationToken cancellationToken)
    => longPollingClient.ListenAsync(async (ProcessesMessage message) =>
    {
      logger.LogInformation("Received process update: {Message}", message);

      await Task.CompletedTask;
    }, cancellationToken);
}
