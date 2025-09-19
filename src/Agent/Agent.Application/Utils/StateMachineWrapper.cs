using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Common.Messages.Agent.State;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.Utils;

public class StateMachineWrapper(
    ILogger<StateMachineWrapper> logger,
    ICommunicationClient communicationClient,
    AgentStateContext agentStateContext)
{
  public void RegisterMachine<TState, TTrigger>(StateMachine<TState, TTrigger> machine, string machineName)
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
          url: $"states/{AgentStateContext.Id}",
          message: new AgentStateChangeRequestMessage
          {
              Machine = machineName,
              FromState = t.Source.ToString(),
              Trigger = t.Trigger.ToString(),
              ToState = t.Destination.ToString(),
              Details = agentStateContext.DetailsMessage,
              Timestamp = DateTime.UtcNow
          },
          authenticate: false,
          cancellationToken: CancellationToken.None);
    });
  }

  public static Task FireAsync<TState, TTrigger>(StateMachine<TState, TTrigger> machine, TTrigger trigger)
      where TState : struct, Enum
      where TTrigger : struct, Enum
  {
    return machine.FireAsync(trigger);
  }
}
