using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Application;
public partial class StateMachine
{
  private async Task HandleRunningEntryAsync()
  {
    _logger.LogInformation("Entering RunningStart state");

    try
    {
      await _metricService.SendAsync(CancellationToken.None);

      // Additional senders can be added here

      _logger.LogInformation("RunningStart iteration done");
      await _machine.FireAsync(Triggers.Success);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in RunningStart");
      await _machine.FireAsync(Triggers.Failed);
    }
  }

  private Task HandleRunningExitAsync()
  {
    _logger.LogInformation("Exiting RunningStart state");
    return Task.CompletedTask;
  }
}
