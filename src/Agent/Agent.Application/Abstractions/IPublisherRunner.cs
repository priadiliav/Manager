namespace Agent.Application.Abstractions;

public interface IPublisherRunner
{
  Task PublishAsync(CancellationToken cancellationToken = default);
}
