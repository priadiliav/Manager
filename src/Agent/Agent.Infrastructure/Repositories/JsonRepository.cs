using System.Text.Json;
using Agent.Domain.Configs;
using Microsoft.Extensions.Options;

namespace Agent.Infrastructure.Repositories;

public class JsonRepository<T>(IOptions<PathsConfig> pathsConfig, Func<PathsConfig, string> pathSelector)
    where T : class
{
  private readonly string _filePath = pathSelector(pathsConfig.Value);
  private T? _cache;

  public async Task<T> GetAsync()
  {
    if (_cache is not null)
      return _cache;

    if (!File.Exists(_filePath))
      throw new InvalidOperationException($"{typeof(T).Name} is not configured.");

    try
    {
      await using var stream = File.OpenRead(_filePath);
      _cache = await JsonSerializer.DeserializeAsync<T>(stream);

      if (_cache is null)
        throw new InvalidOperationException($"{typeof(T).Name} is not configured.");

      return _cache;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"{typeof(T).Name} is not configured.", ex);
    }
  }

  public async Task SaveAsync(T entity)
  {
    var json = JsonSerializer.Serialize(entity, new JsonSerializerOptions
    {
        WriteIndented = true
    });

    await File.WriteAllTextAsync(_filePath, json);

    _cache = entity;
  }
}

