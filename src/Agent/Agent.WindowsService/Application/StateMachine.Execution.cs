using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Application;

public partial class StateMachine
{
  private async Task HandleExecutionEntryAsync()
  {
    _logger.LogInformation("Entering Execution state");

    try
    {
      await _instructionService.ExecuteAsync(CancellationToken.None);

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
