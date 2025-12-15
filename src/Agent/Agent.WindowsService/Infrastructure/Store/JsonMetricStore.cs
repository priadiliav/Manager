using System.Text.Json;
using Agent.WindowsService.Abstraction;
using Agent.WindowsService.Config;

namespace Agent.WindowsService.Infrastructure.Store;

// improvement: parallelize
// improvement: cache file list
// improvement: implement pagination
public class JsonMetricStore : IMetricStore
{
  private readonly SemaphoreSlim _lock = new(1, 1);

  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    WriteIndented = true
  };

  public async Task StoreAsync(IReadOnlyList<Domain.Metric> metrics, CancellationToken cancellationToken)
  {
    await _lock.WaitAsync(cancellationToken);
    try
    {
      var path = PathConfig.CreateMetricFilePath;
      var json = JsonSerializer.Serialize(metrics, JsonOptions);
      Directory.CreateDirectory(Path.GetDirectoryName(path)!);

      await File.WriteAllTextAsync(path, json, cancellationToken);
    }
    finally
    {
      _lock.Release();
    }
  }

  public async Task<IReadOnlyList<Domain.Metric>> GetAllAsync(CancellationToken cancellationToken)
  {
    await _lock.WaitAsync(cancellationToken);
    try
    {
      var path = PathConfig.MetricsDirectory;
      if (!Directory.Exists(path))
        throw new DirectoryNotFoundException($"Metrics directory not found: {path}");

      var metrics = new List<Domain.Metric>();
      var files = Directory.GetFiles(path, "metrics-*.json");

      // improvement: parallelize
      foreach (var file in files)
      {
        var json = await File.ReadAllTextAsync(file, cancellationToken);
        var fileMetrics = JsonSerializer.Deserialize<List<Domain.Metric>>(json, JsonOptions);
        if (fileMetrics != null)
        {
          metrics.AddRange(fileMetrics);
        }
      }

      return metrics;
    }
    finally
    {
      _lock.Release();
    }
  }

  public async Task RemoveAllAsync(CancellationToken cancellationToken)
  {
    await _lock.WaitAsync(cancellationToken);
    try
    {
      var path = PathConfig.MetricsDirectory;
      if (!Directory.Exists(path))
        throw new DirectoryNotFoundException($"Metrics directory not found: {path}");

      var files = Directory.GetFiles(path, "metrics-*.json");

      // improvement: parallelize
      foreach (var file in files)
      {
        File.Delete(file);
      }
    }
    finally
    {
      _lock.Release();
    }
  }
}
