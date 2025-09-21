using Agent.Application.Abstractions;
using Common.Messages.Agent.State;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.Utils;

public class StateMachineWrapper(
  ILogger<StateMachineWrapper> logger,
  IAgentRepository agentRepository,
  ICommunicationClient communicationClient)
{
  public void RegisterMachine<TState, TTrigger>(StateMachine<TState, TTrigger> machine, string machineName)
      where TState : struct, Enum
      where TTrigger : struct, Enum
  {
    machine.OnTransitionedAsync(async t =>
    {
      logger.LogInformation(
          "Transition: {MachineType} | {FromState} --({Trigger})--> {ToState}",
          typeof(TState).Name,
          t.Source,
          t.Trigger,
          t.Destination);

      var agent = await agentRepository.GetAsync();
      var message = new AgentStateChangeRequestMessage
      {
          Machine = machineName,
          FromState = t.Source.ToString(),
          Trigger = t.Trigger.ToString(),
          ToState = t.Destination.ToString(),
          Timestamp = DateTime.UtcNow
      };

      try
      {
        await communicationClient.PutAsync<AgentStateChangeResponseMessage, AgentStateChangeRequestMessage>(
            url: $"api/states/{agent.Id}",
            authenticate: false,
            message: message);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Failed to send state change for {MachineName}", machineName);
      }
    });
  }

  public static Task FireAsync<TState, TTrigger>(StateMachine<TState, TTrigger> machine, TTrigger trigger)
      where TState : struct, Enum
      where TTrigger : struct, Enum
  {
    return machine.FireAsync(trigger);
  }
}
