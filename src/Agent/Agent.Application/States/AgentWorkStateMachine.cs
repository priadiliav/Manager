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

  private readonly List<RunnerStateMachine> _runnerMachines;

  public AgentWorkState CurrentState => _machine.State;

  public AgentWorkStateMachine(
    IEnumerable<IPublisherRunner> publisherRunners,
    IEnumerable<ILongPollingRunner> longPollingRunners,
    ILogger<AgentWorkStateMachine> logger,
    ILoggerFactory loggerFactory,
    AgentStateContext context)
  {
    _publisherRunners = publisherRunners ?? throw new ArgumentNullException(nameof(publisherRunners));
    _longPollingRunners = longPollingRunners ?? throw new ArgumentNullException(nameof(longPollingRunners));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _machine = new StateMachine<AgentWorkState, WorkTrigger>(AgentWorkState.Start);

    _runnerMachines = [];

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

    foreach (var runner in publisherRunners)
    {
      _runnerMachines.Add(new RunnerStateMachine(
          logger: loggerFactory.CreateLogger<RunnerStateMachine>(),
          () => runner.PublishOnceAsync(context.CancellationTokenSource.Token),
          TimeSpan.FromSeconds(10),
          context.CancellationTokenSource.Token
      ));
    }

    foreach (var runner in longPollingRunners)
    {
      _runnerMachines.Add(new RunnerStateMachine(
          logger: loggerFactory.CreateLogger<RunnerStateMachine>(),
          () => runner.ListenOnceAsync(context.CancellationTokenSource.Token),
          TimeSpan.FromSeconds(1),
          context.CancellationTokenSource.Token
      ));
    }
  }

  public async Task StartAsync()
  {
    _logger.LogInformation("Agent started");

    var tasks = _runnerMachines.Select(rm => rm.RunAsync()).ToList();
    await Task.WhenAll(tasks);
  }
}
