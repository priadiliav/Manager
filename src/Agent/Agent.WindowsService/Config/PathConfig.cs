namespace Agent.WindowsService.Config;

public static class PathConfig
{
  private static readonly string BaseDirectory
    = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Manager");

  /// <summary>
  /// Metrics directory in the common application data directory.
  /// </summary>
  public static string MetricsDirectory
    => Path.Combine(BaseDirectory, "metrics");

  /// <summary>
  /// Log file path in the common application data directory.
  /// </summary>
  public static readonly string LogsFilePath
    = Path.Combine(BaseDirectory, "logs", "agent-.log");

  /// <summary>
  /// Config file path in the common application data directory.
  /// </summary>
  public static readonly string ConfigFilePath
    = Path.Combine(BaseDirectory, "config.json");

  /// <summary>
  /// Secret file path in the common application data directory.
  /// </summary>
  public static readonly string SecretFilePath
    = Path.Combine(BaseDirectory, "secrets.dat");

  #region Fabrics

  /// <summary>
  /// Creates a new metric file path with a timestamp in the metrics directory.
  /// </summary>
  public static string CreateMetricFilePath
    => Path.Combine(MetricsDirectory, $"metrics-{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}.json");

  /// <summary>
  /// Creates a new instruction result file path with a timestamp in the instructions directory.
  /// </summary>
  public static string CreateInstructionResultFilePath
    => Path.Combine(BaseDirectory, "instructions", $"instruction-result-{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}.json");

  #endregion
}
