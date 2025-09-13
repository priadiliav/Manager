using Agent.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public class StateMachineWrapper(
    ILogger<StateMachineWrapper> logger,
    ICommunicationClient communicationClient)
{
  public async Task FireAsync<TState, TTrigger>(StateMachine<TState, TTrigger> machine, TTrigger trigger)
      where TState : struct, Enum
      where TTrigger : struct, Enum
  {
    var fromState = machine.State;
    await machine.FireAsync(trigger);
    var toState = machine.State;

    var machineType = typeof(TState).Name;

    logger.LogInformation(
        "State Machine Transition: {MachineType} | {FromState} --({Trigger})--> {ToState}",
        machineType, fromState, trigger, toState);

    // await communicationClient.SendAsync(new
    // {
    //     Machine = machineType,
    //     From = fromState.ToString(),
    //     Trigger = trigger.ToString(),
    //     To = toState.ToString()
    // });
  }
}
