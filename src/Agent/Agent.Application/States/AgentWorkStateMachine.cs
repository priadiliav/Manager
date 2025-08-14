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

  private readonly IEnumerable<IPublisherRunner> _publisherRunners;
  private readonly IEnumerable<ILongPollingRunner> _longPollingRunners;
  private readonly IEnumerable<IWatcherRunner> _watcherRunners;

  private readonly AgentStateContext _context;

  public AgentWorkState CurrentState => _machine.State;

  public AgentWorkStateMachine(
    IEnumerable<IPublisherRunner> publisherRunners,
    IEnumerable<IWatcherRunner> watcherRunners,
    IEnumerable<ILongPollingRunner> longPollingRunners,
    ILogger<AgentWorkStateMachine> logger,
    AgentStateContext context)
  {
    _publisherRunners = publisherRunners ?? throw new ArgumentNullException(nameof(publisherRunners));
    _watcherRunners = watcherRunners ?? throw new ArgumentNullException(nameof(watcherRunners));
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
       var publisherTasks = _publisherRunners
          .Select(runner => runner.StartPublishingAsync(_context.CancellationTokenSource.Token))
          .ToArray();

      var longPollingTasks = _longPollingRunners
          .Select(runner => runner.StartListeningAsync(_context.CancellationTokenSource.Token))
          .ToArray();

      var watcherTasks = _watcherRunners
          .Select(runner => runner.StartWatchingAsync(_context.CancellationTokenSource.Token))
          .ToArray();

      await _machine.FireAsync(WorkTrigger.StartProcessing);

      await Task.WhenAll(longPollingTasks
          .Concat(watcherTasks)
          .Concat(publisherTasks));
    }
    catch (Exception ex)
    {
      _logger.LogError("An error occurred in one of the long polling runners.");

      await _machine.FireAsync(WorkTrigger.ErrorOccurred);
    }
  }
}
