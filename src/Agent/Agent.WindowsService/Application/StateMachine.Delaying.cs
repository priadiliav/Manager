using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Application;

public partial class StateMachine
{
  private async Task HandleDelayingEntryAsync()
  {
    _logger.LogInformation("Entering Delaying state");

    await Task.Delay(5000);

    await _machine.FireAsync(Triggers.Success);
  }

  private Task HandleDelayingExitAsync()
  {
    _logger.LogInformation("Exiting Delaying state");
    return Task.CompletedTask;
  }
}
