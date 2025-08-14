namespace Agent.Application.Abstractions;

public interface IPublisherRunner
{
  Task PublishOnceAsync(CancellationToken cancellationToken = default);
}
