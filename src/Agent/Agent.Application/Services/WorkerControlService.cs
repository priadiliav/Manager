using Agent.Application.Abstractions;
using Agent.Application.States;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

/// <summary>
/// Service for controlling worker state machines remotely
/// </summary>
public class WorkerControlService : IWorkerControlService
{
    private readonly ILogger<WorkerControlService> _logger;
    private readonly Dictionary<string, WorkerStateMachine> _workers;

    public WorkerControlService(
        ILogger<WorkerControlService> logger,
        IEnumerable<WorkerStateMachine> workers)
    {
        _logger = logger;
        _workers = workers.ToDictionary(w => w.GetType().Name, w => w);
    }

    public Task<Dictionary<string, string>> GetWorkerStatesAsync()
    {
        var states = _workers.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.CurrentState.ToString()
        );

        return Task.FromResult(states);
    }

    public async Task<bool> RestartWorkerAsync(string workerName)
    {
        if (!_workers.TryGetValue(workerName, out var worker))
        {
            _logger.LogWarning("Worker {WorkerName} not found", workerName);
            return false;
        }

        if (worker.CurrentState != WorkerState.Error && worker.CurrentState != WorkerState.Stopping)
        {
            _logger.LogWarning("Worker {WorkerName} is not in Error or Stopping state, current state: {State}",
                workerName, worker.CurrentState);
            return false;
        }

        _logger.LogInformation("Restarting worker {WorkerName}", workerName);
        await worker.RestartAsync();
        return true;
    }

    public async Task<bool> StopWorkerAsync(string workerName)
    {
        if (!_workers.TryGetValue(workerName, out var worker))
        {
            _logger.LogWarning("Worker {WorkerName} not found", workerName);
            return false;
        }

        _logger.LogInformation("Stopping worker {WorkerName}", workerName);
        await worker.StopAsync();
        return true;
    }

    public async Task<bool> StartWorkerAsync(string workerName)
    {
        if (!_workers.TryGetValue(workerName, out var worker))
        {
            _logger.LogWarning("Worker {WorkerName} not found", workerName);
            return false;
        }

        if (worker.CurrentState != WorkerState.Idle)
        {
            _logger.LogWarning("Worker {WorkerName} is not in Idle state, current state: {State}",
                workerName, worker.CurrentState);
            return false;
        }

        _logger.LogInformation("Starting worker {WorkerName}", workerName);
        await worker.StartAsync();
        return true;
    }

    public async Task RestartErroredWorkersAsync()
    {
        var erroredWorkers = _workers
            .Where(kvp => kvp.Value.CurrentState == WorkerState.Error)
            .ToList();

        if (!erroredWorkers.Any())
        {
            _logger.LogInformation("No workers in Error state");
            return;
        }

        _logger.LogInformation("Restarting {Count} workers in Error state", erroredWorkers.Count);

        foreach (var (name, worker) in erroredWorkers)
        {
            _logger.LogInformation("Restarting worker {WorkerName}", name);
            await worker.RestartAsync();
        }
    }
}

