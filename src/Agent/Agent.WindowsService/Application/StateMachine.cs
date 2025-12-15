using Agent.WindowsService.Abstraction;
using Agent.WindowsService.Domain;
using FluentValidation;

namespace Agent.WindowsService.Application;

public partial class StateMachine : IStateMachine
{
    private readonly Stateless.StateMachine<States, Triggers> _machine;
    private readonly IEnumerable<IInstructionExecutor> _executors;
    private readonly ILogger<StateMachine> _logger;
    private readonly IMetricCollector _metricCollector;
    private readonly IMetricStore _metricStore;
    private readonly IInstructionStore _instrStore;
    private readonly ISecretStore _secretStore;

    public StateMachine(
      ILogger<StateMachine> logger,
      IEnumerable<IInstructionExecutor> executors,
      IMetricCollector metricCollector,
      IMetricStore metricStore,
      IInstructionStore instrStore,
      ISecretStore secretStore)
    {
        _logger = logger;
        _executors = executors;
        _metricCollector = metricCollector;
        _metricStore = metricStore;
        _instrStore = instrStore;
        _secretStore = secretStore;

        _machine = new Stateless.StateMachine<States, Triggers>(States.Idle);
        ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
        _machine.Configure(States.Idle)
            .Permit(Triggers.Start, States.Authentication);

        _machine.Configure(States.Authentication)
            .OnEntryAsync(HandleAuthenticationEntryAsync)
            .OnExitAsync(HandleAuthenticationExitAsync)
            .Permit(Triggers.Success, States.Synchronization)
            .Permit(Triggers.Failed, States.Error)
            .Permit(Triggers.Stop, States.Idle);

        _machine.Configure(States.Synchronization)
            .OnEntryAsync(async () =>
            {
                _logger.LogInformation("Entering Synchronization state");
                await Task.Delay(500);
                await _machine.FireAsync(Triggers.Success);
            })
            .Permit(Triggers.Success, States.Running)
            .Permit(Triggers.Failed, States.Error)
            .Permit(Triggers.Stop, States.Idle);

        _machine.Configure(States.Running)
            .OnEntryAsync(HandleRunningEntryAsync)
            .OnExitAsync(HandleRunningExitAsync)
            .Permit(Triggers.Success, States.Execution)
            .Permit(Triggers.Failed, States.Error)
            .Permit(Triggers.Stop, States.Idle);

        _machine.Configure(States.Execution)
            .OnEntryAsync(HandleExecutionEntryAsync)
            .OnExitAsync(HandleExecutionExitAsync)
            .Permit(Triggers.Success, States.Delaying)
            .Permit(Triggers.Failed, States.Error)
            .Permit(Triggers.Stop, States.Idle);

        _machine.Configure(States.Delaying)
            .OnEntryAsync(HandleDelayingEntryAsync)
            .OnExitAsync(HandleDelayingExitAsync)
            .Permit(Triggers.Success, States.Running)
            .Permit(Triggers.Failed, States.Error)
            .Permit(Triggers.Stop, States.Idle);

        _machine.Configure(States.Error)
            .OnEntryAsync(async () =>
            {
                _logger.LogWarning("Entering Error state");
                await Task.Delay(500);
                await _machine.FireAsync(Triggers.Retry);
            })
            .Permit(Triggers.Retry, States.Idle)
            .Permit(Triggers.Stop, States.Idle);
    }

    public async Task StartAsync()
        => await _machine.FireAsync(Triggers.Start);

    public async Task StopAsync()
        => await _machine.FireAsync(Triggers.Stop);
}
