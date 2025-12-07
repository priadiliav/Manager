namespace Agent.Application.Abstractions;

/// <summary>
/// Service for controlling worker state machines remotely
/// </summary>
public interface IWorkerControlService
{
    /// <summary>
    /// Get the current state of all workers
    /// </summary>
    Task<Dictionary<string, string>> GetWorkerStatesAsync();

    /// <summary>
    /// Restart a specific worker by name
    /// </summary>
    Task<bool> RestartWorkerAsync(string workerName);

    /// <summary>
    /// Stop a specific worker by name
    /// </summary>
    Task<bool> StopWorkerAsync(string workerName);

    /// <summary>
    /// Start a specific worker by name
    /// </summary>
    Task<bool> StartWorkerAsync(string workerName);

    /// <summary>
    /// Restart all workers in error state
    /// </summary>
    Task RestartErroredWorkersAsync();
}

