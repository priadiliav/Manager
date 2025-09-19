using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.Utils;

public static class StateMachineExecutor
{
  public static async Task ExecuteAsync<TState, TTrigger>(
      StateMachine<TState, TTrigger> machine,
      ILogger? logger,
      Func<Task> action,
      TTrigger successTrigger,
      TTrigger errorTrigger)
      where TState : struct, Enum
      where TTrigger : struct, Enum
  {
    try
    {
      await action();
      await StateMachineWrapper.FireAsync(machine, successTrigger);
    }
    catch (Exception ex)
    {
      logger?.LogError(ex, "Error during state machine action execution.");
      await StateMachineWrapper.FireAsync(machine, errorTrigger);
    }
  }
}
