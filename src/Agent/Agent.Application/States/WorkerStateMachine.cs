using Agent.Application.Utils;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States;

public enum WorkerState
{
  Idle,
  Processing,
  Success,
  WaitingForInterval,
  Stopping,
  Error
}

public enum WorkerTrigger
{
  Start,
  JobCompleted,
  IntervalElapsed,
  Stop,
  Restart,
  ErrorOccurred
}

public abstract class WorkerStateMachine
{
    private readonly StateMachine<WorkerState, WorkerTrigger> _machine;
    private readonly ILogger _logger;
    private readonly string _name;
    private int _currentAttempt;
    private CancellationTokenSource? _cancellationTokenSource;

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
            .OnEntryAsync(HandleProcessingAsync)
            .Permit(WorkerTrigger.JobCompleted, WorkerState.Success)
            .Permit(WorkerTrigger.Stop, WorkerState.Stopping)
            .Permit(WorkerTrigger.ErrorOccurred, WorkerState.Error);

        _machine.Configure(WorkerState.Success)
            .OnEntryAsync(HandleSuccessAsync)
            .Permit(WorkerTrigger.IntervalElapsed, WorkerState.WaitingForInterval)
            .Permit(WorkerTrigger.Stop, WorkerState.Stopping);

        _machine.Configure(WorkerState.WaitingForInterval)
            .OnEntryAsync(HandleWaitingAsync)
            .Permit(WorkerTrigger.Start, WorkerState.Processing)
            .Permit(WorkerTrigger.Stop, WorkerState.Stopping);

        _machine.Configure(WorkerState.Stopping)
            .OnEntryAsync(HandleStoppingAsync)
            .Permit(WorkerTrigger.Start, WorkerState.Idle);

        _machine.Configure(WorkerState.Error)
            .OnEntryAsync(HandleErrorAsync)
            .Permit(WorkerTrigger.Restart, WorkerState.Processing)
            .Permit(WorkerTrigger.Stop, WorkerState.Stopping);
    }

    public WorkerState CurrentState => _machine.State;

    public Task StartAsync() => StateMachineWrapper.FireAsync(_machine, WorkerTrigger.Start);

    public async Task StopAsync()
    {
        await _cancellationTokenSource?.CancelAsync()!;
        await StateMachineWrapper.FireAsync(_machine, WorkerTrigger.Stop);
    }

    public Task RestartAsync() => StateMachineWrapper.FireAsync(_machine, WorkerTrigger.Restart);

    private async Task HandleProcessingAsync()
    {
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            _logger.LogInformation("Worker {Name} starting processing cycle (attempt {Attempt})", _name, _currentAttempt + 1);

            await DoWorkAsync(token);

            _currentAttempt = 0; // Reset on success
            _logger.LogInformation("Worker {Name} job completed successfully", _name);

            await StateMachineWrapper.FireAsync(_machine, WorkerTrigger.JobCompleted);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker {Name} processing was cancelled", _name);
            // Don't transition, let Stop handle it
        }
        catch (Exception ex)
        {
            _currentAttempt++;
            var retries = await GetRetryCountAsync();

            _logger.LogError(ex, "Worker {Name} failed attempt {Attempt}/{Max}", _name, _currentAttempt, retries);

            if (_currentAttempt >= retries)
            {
                _logger.LogError("Worker {Name} exceeded max retries, going to Error state", _name);
                _currentAttempt = 0; // Reset for next restart
                await StateMachineWrapper.FireAsync(_machine, WorkerTrigger.ErrorOccurred);
            }
            else
            {
                // Retry after delay
                var retryDelay = await GetRetryDelayAsync();
                _logger.LogInformation("Worker {Name} retrying in {Delay}", _name, retryDelay);
                await Task.Delay(retryDelay);
                await StateMachineWrapper.FireAsync(_machine, WorkerTrigger.Start);
            }
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private async Task HandleSuccessAsync()
    {
        _logger.LogInformation("Worker {Name} in Success state", _name);
        await StateMachineWrapper.FireAsync(_machine, WorkerTrigger.IntervalElapsed);
    }

    private async Task HandleWaitingAsync()
    {
        var interval = await GetIntervalAsync();
        _logger.LogInformation("Worker {Name} waiting for {Interval} before next cycle", _name, interval);

        try
        {
            await Task.Delay(interval, _cancellationTokenSource?.Token ?? CancellationToken.None);

            if (_machine.State == WorkerState.WaitingForInterval)
            {
                await StateMachineWrapper.FireAsync(_machine, WorkerTrigger.Start);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker {Name} waiting was cancelled", _name);
        }
    }

    protected abstract Task<TimeSpan> GetIntervalAsync();
    protected abstract Task<TimeSpan> GetRetryDelayAsync();
    protected abstract Task<int> GetRetryCountAsync();
    protected abstract Task DoWorkAsync(CancellationToken cancellationToken);

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
