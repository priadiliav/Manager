using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Application;

public partial class StateMachine
{
  private async Task HandleExecutionEntryAsync()
  {
    _logger.LogInformation("Entering Execution state");

    try
    {
      var getInstructionsFromServer = new List<Instruction>()
      {
        new()
        {
          AssociativeId = Guid.NewGuid(),
          Type = InstructionType.ShellCommand,
          Payload = new Dictionary<string, string>
            { { "commandj", "ipconfig /all" }, }
        }
      };

      foreach (var instruction in getInstructionsFromServer)
      {
        var result = await _executors
          .First(e => e.CanExecute(instruction.Type))
          .ExecuteAsync(instruction, CancellationToken.None);

        await _instrStore.SaveResultAsync(result, CancellationToken.None);
      }

      _logger.LogInformation("Execution iteration done");
      await _machine.FireAsync(Triggers.Success);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in Execution");
      await _machine.FireAsync(Triggers.Failed);
    }
  }

  private Task HandleExecutionExitAsync()
  {
    _logger.LogInformation("Exiting Execution state");
    return Task.CompletedTask;
  }
}
