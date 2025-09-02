namespace Common.Messages.Static;

public class SyncMessage : IMessage
{
  public CpuInfoMessage Cpu { get; set; }
}
