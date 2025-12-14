namespace Agent.WindowsService.Abstraction;

public interface ISecretStore
{
  /// <summary>
  /// Retrieves the value for the specified key from the secret store.
  /// </summary>
  Task<ReadOnlyMemory<byte>?> GetAsync(string key, CancellationToken cancellationToken = default);

  /// <summary>
  /// Sets the value for the specified key in the secret store.
  /// </summary>
  Task SetAsync(string key, ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default);
}
