namespace Agent.Application.Abstractions;

public interface IDynamicDataCollector<out T>
{
  string Name { get; }
  T Collect(CancellationToken cancellationToken = default);
}
