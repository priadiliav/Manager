using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum AgentWorkState
{
  Start,
  Listening,
  Processing,
  Paused,
  Error
}

public enum WorkTrigger
{
  StartListening,
  StartProcessing,
  Pause,
  Resume,
  ErrorOccurred
}

public class AgentWorkStateMachine : IAgentStateMachine
{
  private readonly StateMachine<AgentWorkState, WorkTrigger> _machine;
  private readonly ILogger<AgentWorkStateMachine> _logger;

  private readonly IEnumerable<ILongPollingRunner> _longPollingRunners;
  private readonly AgentStateContext _context;

  public AgentWorkState CurrentState => _machine.State;

  public AgentWorkStateMachine(
    IEnumerable<ILongPollingRunner> longPollingRunners,
    ILogger<AgentWorkStateMachine> logger,
    AgentStateContext context)
  {
    _longPollingRunners = longPollingRunners ?? throw new ArgumentNullException(nameof(longPollingRunners));
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _machine = new StateMachine<AgentWorkState, WorkTrigger>(AgentWorkState.Start);

    _machine.Configure(AgentWorkState.Start)
        .Permit(WorkTrigger.StartListening, AgentWorkState.Listening);

    _machine.Configure(AgentWorkState.Listening)
        .Permit(WorkTrigger.StartProcessing, AgentWorkState.Processing)
        .Permit(WorkTrigger.Pause, AgentWorkState.Paused)
        .Permit(WorkTrigger.ErrorOccurred, AgentWorkState.Error);

    _machine.Configure(AgentWorkState.Processing)
        .Permit(WorkTrigger.Pause, AgentWorkState.Paused)
        .Permit(WorkTrigger.ErrorOccurred, AgentWorkState.Error);

    _machine.Configure(AgentWorkState.Error)
        .Permit(WorkTrigger.Pause, AgentWorkState.Paused)
        .Permit(WorkTrigger.Resume, AgentWorkState.Listening);
  }

  public async Task StartAsync()
  {
    await _machine.FireAsync(WorkTrigger.StartListening);

    try
    {
      var longPollingTasks = _longPollingRunners.Select(runner =>
          runner.StartListeningAsync(_context.CancellationTokenSource.Token)).ToArray();

      await _machine.FireAsync(WorkTrigger.StartProcessing);

      await Task.WhenAll(longPollingTasks);
    }
    catch (Exception ex)
    {
      _logger.LogError("An error occurred in one of the long polling runners.");

      await _machine.FireAsync(WorkTrigger.ErrorOccurred);
    }
  }
}
