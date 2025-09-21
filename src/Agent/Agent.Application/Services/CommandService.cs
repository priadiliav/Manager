using Agent.Application.Abstractions;
using Agent.Application.States;
using Common.Messages.Agent.Command;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

public interface ICommandService
{
  /// <summary>
  /// Polls for commands from the server and executes them.
  /// </summary>
  /// <returns></returns>
  Task PollAndExecuteCommandsAsync();
}

public class CommandService(
  ILogger<CommandService> logger,
  OverallStateMachine overallStateMachine,
  IReceiverClient receiverClient) : ICommandService
{
  public async Task PollAndExecuteCommandsAsync()
    => await receiverClient.ReceiveAsync<AgentCommandMessage>(
        url: "/api/commands/subscribe",
        handler: async command =>
        {
          logger.LogInformation("Received command: {Command}", command);
          // await overallStateMachine.TryToFireAsync();
        },
        cancellationToken: CancellationToken.None);
}
