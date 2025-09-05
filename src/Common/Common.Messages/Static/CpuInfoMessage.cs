namespace Common.Messages.Static;

public class CpuInfoMessage
{
  public string CpuModel { get; set; } = string.Empty;
  public int CpuCores { get; set; } = 0;
  public double CpuSpeedGHz { get; set; } = 0.0;
  public string CpuArchitecture { get; set; } = string.Empty;
}
