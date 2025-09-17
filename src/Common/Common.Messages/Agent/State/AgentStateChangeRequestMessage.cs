namespace Common.Messages.Agent.State;

public class AgentStateChangeRequestMessage : IMessage
{
    public string Machine { get; set; } = string.Empty;
    public string FromState { get; set; } = string.Empty;
    public string Trigger { get; set; } = string.Empty;
    public string ToState { get; set; } = string.Empty;

    public string? Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public override string ToString()
     => $"{Machine}: {FromState} --({Trigger})--> {ToState} at {Timestamp.ToUniversalTime():o}";
}
