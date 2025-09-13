using Agent.Application.Abstractions;
using Common.Messages.Agent.State;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public class StateMachineWrapper(
    ILogger<StateMachineWrapper> logger,
    ICommunicationClient communicationClient)
{
  public void RegisterMachine<TState, TTrigger>(StateMachine<TState, TTrigger> machine)
      where TState : struct, Enum
      where TTrigger : struct, Enum
  {
    machine.OnTransitioned(t =>
    {
      logger.LogInformation(
          "Transition: {MachineType} | {FromState} --({Trigger})--> {ToState}",
          typeof(TState).Name,
          t.Source,
          t.Trigger,
          t.Destination);

      _ = communicationClient.PutAsync<AgentStateChangeResponseMessage, AgentStateChangeRequestMessage>(
          url: "agents/state",
          message: new AgentStateChangeRequestMessage
          {
              Machine = typeof(TState).Name,
              From = t.Source.ToString(),
              Trigger = t.Trigger.ToString(),
              To = t.Destination.ToString(),
              Timestamp = DateTime.UtcNow
          },
          authenticate: true,
          cancellationToken: CancellationToken.None);
    });
  }

  public Task FireAsync<TState, TTrigger>(StateMachine<TState, TTrigger> machine, TTrigger trigger)
      where TState : struct, Enum
      where TTrigger : struct, Enum
  {
    return machine.FireAsync(trigger);
  }
}
