using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Abstraction;

public interface IInstructionService
{
  /// <summary>
  /// Execute instruction
  /// </summary>
  Task ExecuteAsync(CancellationToken cancellationToken);
}
