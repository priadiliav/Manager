using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class AgentHardware : IEntity<long>
{
  public long Id { get; init; }

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

  public virtual Agent Agent { get; init; } = null!;

  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }

  /// <summary>
  /// Modifies the current agentHardware with the values from another agentHardware.
  /// </summary>
  /// <param name="agentHardware"></param>
  /// <exception cref="ArgumentNullException"></exception>
  public void ModifyFrom(AgentHardware agentHardware)
  {
    if (agentHardware is null)
      throw new ArgumentNullException(nameof(agentHardware));

    CpuModel = agentHardware.CpuModel;
    CpuCores = agentHardware.CpuCores;
    CpuSpeedGHz = agentHardware.CpuSpeedGHz;
    CpuArchitecture = agentHardware.CpuArchitecture;
    GpuModel = agentHardware.GpuModel;
    GpuMemoryMB = agentHardware.GpuMemoryMB;
    RamModel = agentHardware.RamModel;
    TotalMemoryMB = agentHardware.TotalMemoryMB;
    DiskModel = agentHardware.DiskModel;
    TotalDiskMB = agentHardware.TotalDiskMB;
  }

  /// <summary>
  /// Empty agentHardware instance with default values.
  /// </summary>
  /// <returns></returns>
  public static AgentHardware Empty() => new()
  {
    CpuModel = string.Empty,
    CpuCores = 0,
    CpuSpeedGHz = 0,
    CpuArchitecture = string.Empty,
    GpuModel = string.Empty,
    GpuMemoryMB = 0,
    RamModel = string.Empty,
    TotalMemoryMB = 0,
    DiskModel = string.Empty,
    TotalDiskMB = 0,
    CreatedAt = DateTimeOffset.UtcNow,
  };
}
