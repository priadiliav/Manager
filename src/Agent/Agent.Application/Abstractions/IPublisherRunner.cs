namespace Agent.Application.Abstractions;

public interface IPublisherRunner
{
  Task StartPublishingAsync(CancellationToken cancellationToken = default);
}
