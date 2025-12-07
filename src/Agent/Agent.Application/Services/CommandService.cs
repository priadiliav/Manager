using Agent.Application.Abstractions;
using Common.Messages.Agent.Command;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

/// <summary>
/// Service for handling worker commands from the server
/// </summary>
public class CommandService : ICommandService
{
    private readonly ILogger<CommandService> _logger;
    private readonly IWorkerControlService _workerControlService;
    private readonly IReceiverClient _receiverClient;
    private readonly IPublisherClient _publisherClient;

    public CommandService(
        ILogger<CommandService> logger,
        IWorkerControlService workerControlService,
        IReceiverClient receiverClient,
        IPublisherClient publisherClient)
    {
        _logger = logger;
        _workerControlService = workerControlService;
        _receiverClient = receiverClient;
        _publisherClient = publisherClient;
    }

    public async Task PollAndExecuteCommandsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Polling for worker commands from server");

            await _receiverClient.ReceiveAsync<CommandRequestMessage>(
                "/api/agent/commands",
                async (command) => await HandleCommandAsync(command),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error polling for worker commands");
            throw;
        }
    }

    private async Task HandleCommandAsync(CommandRequestMessage commandRequest)
    {
        _logger.LogInformation("Received commandRequest: {Command} for worker: {Worker}",
            commandRequest.Command, commandRequest.WorkerName ?? "All");

        CommandResponseMessage response;

        try
        {
            switch (commandRequest.Command.ToLowerInvariant())
            {
                case "start":
                    if (string.IsNullOrEmpty(commandRequest.WorkerName))
                    {
                        response = new CommandResponseMessage
                        {
                            Success = false,
                            Message = "WorkerName is required for Start commandRequest"
                        };
                    }
                    else
                    {
                        var success = await _workerControlService.StartWorkerAsync(commandRequest.WorkerName);
                        response = new CommandResponseMessage
                        {
                            Success = success,
                            Message = success
                                ? $"Worker {commandRequest.WorkerName} started successfully"
                                : $"Failed to start worker {commandRequest.WorkerName}",
                        };
                    }
                    break;

                case "stop":
                    if (string.IsNullOrEmpty(commandRequest.WorkerName))
                    {
                        response = new CommandResponseMessage
                        {
                            Success = false,
                            Message = "WorkerName is required for Stop commandRequest"
                        };
                    }
                    else
                    {
                        var success = await _workerControlService.StopWorkerAsync(commandRequest.WorkerName);
                        response = new CommandResponseMessage
                        {
                            Success = success,
                            Message = success
                                ? $"Worker {commandRequest.WorkerName} stopped successfully"
                                : $"Failed to stop worker {commandRequest.WorkerName}",
                        };
                    }
                    break;

                case "restart":
                    if (string.IsNullOrEmpty(commandRequest.WorkerName))
                    {
                        response = new CommandResponseMessage
                        {
                            Success = false,
                            Message = "WorkerName is required for Restart commandRequest"
                        };
                    }
                    else
                    {
                        var success = await _workerControlService.RestartWorkerAsync(commandRequest.WorkerName);
                        response = new CommandResponseMessage
                        {
                            Success = success,
                            Message = success
                                ? $"Worker {commandRequest.WorkerName} restarted successfully"
                                : $"Failed to restart worker {commandRequest.WorkerName}",
                        };
                    }
                    break;

                case "restarterrored":
                    await _workerControlService.RestartErroredWorkersAsync();
                    response = new CommandResponseMessage
                    {
                        Success = true,
                        Message = "All errored workers restarted",
                    };
                    break;

                case "getstates":
                    response = new CommandResponseMessage
                    {
                        Success = true,
                        Message = "Worker states retrieved successfully",
                    };
                    break;

                default:
                    response = new CommandResponseMessage
                    {
                        Success = false,
                        Message = $"Unknown commandRequest: {commandRequest.Command}"
                    };
                    break;
            }

            await _publisherClient.PublishAsync<CommandRequestMessage, CommandResponseMessage>(
                "/api/agent/commandRequest-response", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing commandRequest: {Command}", commandRequest.Command);

            response = new CommandResponseMessage
            {
                Success = false,
                Message = $"Error executing commandRequest: {ex.Message}"
            };

            await _publisherClient.PublishAsync<CommandRequestMessage, CommandResponseMessage>(
                "/api/agent/commandRequest-response", response);
        }
    }
}
