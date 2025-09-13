namespace Common.Messages.Agent.State;

public class AgentStateChangeRequestMessage : IMessage
{
    public string Machine { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Trigger { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public override string ToString()
     => $"{Machine}: {From} --({Trigger})--> {To} at {Timestamp.ToUniversalTime():o}";
}
