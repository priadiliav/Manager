using Agent.Application.Utils;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Agent.Application.States.Workers;

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
    protected readonly StateMachine<WorkerState, WorkerTrigger> Machine;
    protected readonly ILogger Logger;
    private readonly string _name;
    protected WorkerStateMachine(
        ILogger logger,
        StateMachineWrapper wrapper,
        string name)
    {
        Logger = logger;
        _name = name;
        Machine = new StateMachine<WorkerState, WorkerTrigger>(WorkerState.Idle);

        ConfigureStateMachine();
        wrapper.RegisterMachine(Machine, name);
    }

    private void ConfigureStateMachine()
    {
        Machine.Configure(WorkerState.Idle)
            .Permit(WorkerTrigger.Start, WorkerState.Processing);

        Machine.Configure(WorkerState.Processing)
            .OnEntryAsync(async () => await ScheduleAsync())
            .Permit(WorkerTrigger.Stop, WorkerState.Stopping)
            .Permit(WorkerTrigger.ErrorOccurred, WorkerState.Error);

        Machine.Configure(WorkerState.Stopping)
            .OnEntryAsync(HandleStoppingAsync)
            .Permit(WorkerTrigger.Start, WorkerState.Idle);

        Machine.Configure(WorkerState.Error)
            .OnEntryAsync(HandleErrorAsync)
            .Permit(WorkerTrigger.Start, WorkerState.Processing);
    }

    public WorkerState CurrentState => Machine.State;
    public Task StartAsync() => Machine.FireAsync(WorkerTrigger.Start);
    public Task StopAsync() => Machine.FireAsync(WorkerTrigger.Stop);

    private async Task ScheduleAsync()
    {
        var retries = await GetRetryCountAsync();
        var retryDelaySeconds = await GetRetryDelayAsync();
        var interval = await GetIntervalAsync();

        var attempt = 0;
        while (Machine.State == WorkerState.Processing)
        {
            try
            {
                Logger.LogInformation("Worker {Name} starting processing cycle", _name);
                await HandleProcessingAsync();
                attempt = 0;
            }
            catch (Exception ex)
            {
                attempt++;
                Logger.LogError(ex, "Worker {name} failed attempt {Attempt}/{Max}", _name, attempt, retries);

                if (attempt >= retries)
                {
                  Logger.LogError("Worker {name} exceeded max retries, going to Error", _name);
                  await Machine.FireAsync(WorkerTrigger.ErrorOccurred);
                  break;
                }

                await Task.Delay(retryDelaySeconds);
            }

            Logger.LogInformation("Worker {Name} completed cycle, waiting for {Interval}", _name, interval);

            await Task.Delay(interval);
        }
    }

    protected abstract Task<TimeSpan> GetIntervalAsync();
    protected abstract Task<TimeSpan> GetRetryDelayAsync();
    protected abstract Task<int> GetRetryCountAsync();

    protected abstract Task HandleProcessingAsync();
    protected virtual Task HandleStoppingAsync() => Task.CompletedTask;
    protected virtual Task HandleErrorAsync() => Task.CompletedTask;
}

