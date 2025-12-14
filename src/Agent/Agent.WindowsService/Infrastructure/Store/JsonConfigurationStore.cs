using System.Text.Json;
using Agent.WindowsService.Abstraction;
using Agent.WindowsService.Config;
using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Infrastructure.Store;

public sealed class JsonConfigurationStore : IConfigurationStore
{
  private readonly SemaphoreSlim _lock = new(1, 1);

  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
  };

  public async Task<Configuration> GetAsync(CancellationToken cancellationToken = default)
  {
    await _lock.WaitAsync(cancellationToken);
    try
    {
      var path = PathConfig.ConfigFilePath;

      if (!File.Exists(path))
        throw new FileNotFoundException("Configuration file not found", path);

      var json = await File.ReadAllTextAsync(path, cancellationToken);
      var configuration = JsonSerializer.Deserialize<Configuration>(json, JsonOptions);

      if (configuration is null)
        throw new InvalidOperationException("Failed to deserialize configuration");

      return configuration;
    }
    finally
    {
      _lock.Release();
    }
  }

  public async Task SaveAsync(Configuration configuration, CancellationToken cancellationToken = default)
  {
    await _lock.WaitAsync(cancellationToken);
    try
    {
      var path = PathConfig.ConfigFilePath;
      var json = JsonSerializer.Serialize(configuration, JsonOptions);

      Directory.CreateDirectory(Path.GetDirectoryName(path)!);
      await File.WriteAllTextAsync(path, json, cancellationToken);
    }
    finally
    {
      _lock.Release();
    }
  }
}
