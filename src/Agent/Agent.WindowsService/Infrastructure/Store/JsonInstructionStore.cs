using System.Text.Json;
using Agent.WindowsService.Abstraction;
using Agent.WindowsService.Config;
using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Infrastructure.Store;

public class JsonInstructionStore : IInstructionStore
{
  private readonly SemaphoreSlim _lock = new(1, 1);

  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    WriteIndented = true
  };
  
  public async Task SaveResultAsync(InstructionResult result, CancellationToken cancellationToken)
  {
    await _lock.WaitAsync(cancellationToken);
    try
    {
      var path = PathConfig.CreateInstructionResultFilePath;
      var json = JsonSerializer.Serialize(result, JsonOptions);
      Directory.CreateDirectory(Path.GetDirectoryName(path)!);

      await File.WriteAllTextAsync(path, json, cancellationToken);
    }
    finally
    {
      _lock.Release();
    }
  }
}
