using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class Hardware : IEntity<long>
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

  public virtual Agent Agent { get; set; } = null!;

  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }

  /// <summary>
  /// Modifies the current hardware with the values from another hardware.
  /// </summary>
  /// <param name="hardware"></param>
  /// <exception cref="ArgumentNullException"></exception>
  public void ModifyFrom(Hardware hardware)
  {
    if (hardware is null)
      throw new ArgumentNullException(nameof(hardware));

    CpuModel = hardware.CpuModel;
    CpuCores = hardware.CpuCores;
    CpuSpeedGHz = hardware.CpuSpeedGHz;
    CpuArchitecture = hardware.CpuArchitecture;
    GpuModel = hardware.GpuModel;
    GpuMemoryMB = hardware.GpuMemoryMB;
    RamModel = hardware.RamModel;
    TotalMemoryMB = hardware.TotalMemoryMB;
    DiskModel = hardware.DiskModel;
    TotalDiskMB = hardware.TotalDiskMB;
  }

  /// <summary>
  /// Empty hardware instance with default values.
  /// </summary>
  /// <returns></returns>
  public static Hardware Empty() => new()
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
