using Common.Messages.Agent.Command;

namespace Agent.Application.Abstractions;

/// <summary>
/// Service for handling worker commands from the server
/// </summary>
public interface ICommandService
{
    /// <summary>
    /// Poll for and execute commands from the server
    /// </summary>
    Task PollAndExecuteCommandsAsync(CancellationToken cancellationToken = default);
}

