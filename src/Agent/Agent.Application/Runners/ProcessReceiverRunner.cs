using Agent.Application.Abstractions;
using Common.Messages.Process;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Runners;

public class ProcessReceiverRunner(
  ILogger<ProcessReceiverRunner> logger,
  IReceiverClient receiverClient) : IWorkerRunner
{
  public TimeSpan Interval => TimeSpan.FromSeconds(3);
  public string Url => "processes/subscribe";

  public async Task RunAsync(CancellationToken cancellationToken = default)
    => await receiverClient.ReceiveAsync<ProcessesMessage>(Url, HandleReceivedMessage, cancellationToken);

  private Task HandleReceivedMessage(ProcessesMessage message)
  {
    // Implement your message handling logic here
    logger.LogInformation("Handling received process message: {Message}", message);

    return Task.CompletedTask;
  }
}
