namespace Agent.Application.Abstractions;

public interface IMetricStrategy
{
  string Name { get; }
  object? Collect();
}
