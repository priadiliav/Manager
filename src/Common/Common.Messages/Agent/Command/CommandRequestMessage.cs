namespace Common.Messages.Agent.Command;

public class CommandRequestMessage : IMessage
{
    public string Command { get; set; } = string.Empty; // "Start", "Stop", "Restart", "RestartErrored"
    public string? WorkerName { get; set; } // Null for RestartErrored command
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

