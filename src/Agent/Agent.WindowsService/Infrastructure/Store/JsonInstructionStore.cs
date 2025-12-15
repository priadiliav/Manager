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

  public async Task RemoveAllAsync(CancellationToken cancellationToken)
  {
    await _lock.WaitAsync(cancellationToken);
    try
    {
      var path = PathConfig.InstructionResultsDirectory;

      if(!Directory.Exists(path))
        throw new DirectoryNotFoundException($"Instruction results directory not found: {path}");

      var files = Directory.GetFiles(path, "instruction-result-*.json");
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

  public async Task<IReadOnlyList<InstructionResult>> GetAllResultsAsync(CancellationToken cancellationToken)
  {
    await _lock.WaitAsync(cancellationToken);
    try
    {
      var path = PathConfig.InstructionResultsDirectory;
      if (!Directory.Exists(path))
        throw new DirectoryNotFoundException($"Instruction results directory not found: {path}");

      var results = new List<InstructionResult>();
      var files = Directory.GetFiles(path, "instruction-result-*.json");

      // improvement: parallelize
      foreach (var file in files)
      {
        var json = await File.ReadAllTextAsync(file, cancellationToken);
        var fileResult = JsonSerializer.Deserialize<InstructionResult>(json, JsonOptions);
        if (fileResult != null)
        {
          results.Add(fileResult);
        }
      }

      return results;
    }
    finally
    {
      _lock.Release();
    }
  }

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
