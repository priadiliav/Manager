namespace Agent.Application.Abstractions;

public interface IDataCollector<out T>
{
  string Name { get; }

  /// <summary>
  /// Collect data
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  T Collect(CancellationToken cancellationToken = default);
}
