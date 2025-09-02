namespace Agent.Application.Abstractions;

public interface IStaticDataCollector<out T>
{
  string Name { get; }

  T Collect(CancellationToken cancellationToken = default);
}
