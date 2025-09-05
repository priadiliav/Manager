namespace Server.Application.Dtos.Hardware;

public class HardwareDto
{
  public long Id { get; set; }
  public Guid AgentId { get; set; }
  
  #region Cpu
  public string CpuModel { get; set; } = string.Empty;
  public int CpuCores { get; set; }
  public double CpuSpeedGHz { get; set; }
  public string CpuArchitecture { get; set; } = string.Empty;
  #endregion

  #region Gpu
  public string GpuModel { get; set; } = string.Empty;
  public int GpuMemoryMB { get; set; }
  #endregion

  #region Ram
  public string RamModel { get; set; } = string.Empty;
  public long TotalMemoryMB { get; set; }
  #endregion

  #region Disk
  public string DiskModel { get; set; } = string.Empty;
  public long TotalDiskMB { get; set; }
  #endregion
}
