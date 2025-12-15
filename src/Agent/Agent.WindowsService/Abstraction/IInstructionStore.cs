using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Abstraction;

public interface IInstructionStore
{
  /// <summary>
  /// Save instruction result
  /// </summary>
  Task SaveResultAsync(InstructionResult result, CancellationToken cancellationToken);

  /// <summary>
  /// Get all instruction results
  /// </summary>
  Task<IReadOnlyList<InstructionResult>> GetAllResultsAsync(CancellationToken cancellationToken);

  /// <summary>
  /// Remove all instruction results
  /// </summary>
  Task RemoveAllAsync(CancellationToken cancellationToken);
}
