using Common.Messages;

public class CommandResponseMessage : IMessage
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
