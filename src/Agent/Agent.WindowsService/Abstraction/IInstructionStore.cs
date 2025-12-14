using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Abstraction;

public interface IInstructionStore
{
  /// <summary>
  /// Save instruction result
  /// </summary>
  Task SaveResultAsync(InstructionResult result, CancellationToken cancellationToken);
}
