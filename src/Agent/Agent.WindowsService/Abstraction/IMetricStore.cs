namespace Agent.WindowsService.Abstraction;

// for the offline version, we need to store if we cant send to the server,
// then get them all and send them in batch when we have connection again.
// and remove them from the local store when sent successfully
public interface IMetricStore
{
  /// <summary>
  /// Stores the given metrics asynchronously.
  /// </summary>
  Task StoreAsync(IEnumerable<Domain.Metric> metrics, CancellationToken cancellationToken);

  /// <summary>
  /// Retrieves all stored metrics asynchronously.
  /// </summary>
  Task<IEnumerable<Domain.Metric>> GetAllAsync(CancellationToken cancellationToken);

  /// <summary>
  /// Removes all stored metrics asynchronously.
  /// </summary>
  Task RemoveAllAsync(CancellationToken cancellationToken);
}
