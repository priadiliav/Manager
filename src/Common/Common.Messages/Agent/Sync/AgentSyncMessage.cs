using Common.Messages.Agent.Sync.Hardware;

namespace Common.Messages.Agent.Sync;

public class AgentSyncMessage : IMessage
{
  public HardwareMessage Hardware { get; init; } = new();
  public string RsopReportBase64 { get; init; } = string.Empty;
  public override string ToString()
    => $"Hardware: {Hardware}," +
       $"RsopReportBase64 Length: {RsopReportBase64.Length}";
}
