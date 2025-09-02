namespace Common.Messages.Static;

public class CpuInfoMessage : IMessage
{
  public string Model { get; set; }
  public int Cores { get; set; }
  public double SpeedGHz { get; set; }
  public string Architecture { get; set; }
}
