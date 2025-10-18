using Agent.Application.Utils;
using Microsoft.Extensions.Logging;
using Stateless;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace Agent.Application.States;

public enum WorkerState
{
  Idle,
  Processing,
  Stopping,
  Error
}

public enum WorkerTrigger
{
  Start,
  Success,
  Stop,
  ErrorOccurred
}

public abstract class WorkerStateMachine
{
    private readonly StateMachine<WorkerState, WorkerTrigger> _machine;
    private readonly ILogger _logger;
    private readonly string _name;
    protected WorkerStateMachine(
        ILogger logger,
        StateMachineWrapper wrapper,
        string name)
    {
        _logger = logger;
        _name = name;
        _machine = new StateMachine<WorkerState, WorkerTrigger>(WorkerState.Idle);

        ConfigureStateMachine();

        wrapper.RegisterMachine(_machine, name);
    }

    private void ConfigureStateMachine()
    {
        _machine.Configure(WorkerState.Idle)
            .Permit(WorkerTrigger.Start, WorkerState.Processing);

        _machine.Configure(WorkerState.Processing)
            .OnEntryAsync(async () => await ScheduleAsync())
            .Permit(WorkerTrigger.Stop, WorkerState.Stopping)
            .Permit(WorkerTrigger.ErrorOccurred, WorkerState.Error);

        _machine.Configure(WorkerState.Stopping)
            .OnEntryAsync(HandleStoppingAsync)
            .Permit(WorkerTrigger.Start, WorkerState.Idle);

        _machine.Configure(WorkerState.Error)
            .OnEntryAsync(HandleErrorAsync)
            .Permit(WorkerTrigger.Start, WorkerState.Processing);
    }

    public WorkerState CurrentState => _machine.State;
    public Task StartAsync() => StateMachineWrapper.FireAsync(_machine, WorkerTrigger.Start);
    public Task StopAsync() => StateMachineWrapper.FireAsync(_machine, WorkerTrigger.Stop);

    private async Task ScheduleAsync()
    {
        var retries = await GetRetryCountAsync();
        var retryDelaySeconds = await GetRetryDelayAsync();
        var interval = await GetIntervalAsync();

        var attempt = 0;
        while (_machine.State is WorkerState.Processing)
        {
            try
            {
                _logger.LogInformation("Worker {Name} starting processing cycle", _name);
                await HandleProcessingAsync();
                attempt = 0;
            }
            catch (Exception ex)
            {
                attempt++;
                _logger.LogError(ex, "Worker {name} failed attempt {Attempt}/{Max}", _name, attempt, retries);

                if (attempt >= retries)
                {
                  _logger.LogError("Worker {name} exceeded max retries, going to Error", _name);
                  await StateMachineWrapper.FireAsync(_machine, WorkerTrigger.ErrorOccurred);
                  break;
                }

                await Task.Delay(retryDelaySeconds);
            }

            _logger.LogInformation("Worker {Name} completed cycle, waiting for {Interval}", _name, interval);

            await Task.Delay(interval);
        }
    }

    protected abstract Task<TimeSpan> GetIntervalAsync();
    protected abstract Task<TimeSpan> GetRetryDelayAsync();
    protected abstract Task<int> GetRetryCountAsync();
    protected abstract Task HandleProcessingAsync(CancellationToken cancellationToken = default);

    protected virtual Task HandleStoppingAsync()
    {
      _logger.LogInformation("Worker {Name} stopping, waiting for manual restart", _name);
      return Task.CompletedTask;
    }

    protected virtual Task HandleErrorAsync()
    {
      _logger.LogInformation("Worker {Name} in Error state, waiting for manual restart", _name);
      return Task.CompletedTask;
    }
}

