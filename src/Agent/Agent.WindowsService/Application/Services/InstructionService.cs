using Agent.WindowsService.Abstraction;
using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Application.Services;

public class InstructionService(
  IEnumerable<IInstructionExecutor> executors,
  IInstructionStore store) : IInstructionService
{
  public async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    var getInstructionsFromServer = new List<Instruction>()
    {
      new()
      {
        AssociativeId = Guid.NewGuid(),
        Type = InstructionType.ShellCommand,
        Payload = new Dictionary<string, string>
          { { "command", "ipconfig /all" }, }
      }
    };

    foreach (var instruction in getInstructionsFromServer)
    {
      var executor = executors.First(e => e.CanExecute(instruction.Type));
      var result = await executor.ExecuteAsync(instruction, cancellationToken);

      await store.SaveResultAsync(result, cancellationToken);
    }
  }
}
