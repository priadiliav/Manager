using Common.Messages;

namespace Agent.Application.Abstractions;

public interface IPublisherClient
{
  Task<TResponse?> PublishAsync<TResponse, TRequest>(
      string url, TRequest message, CancellationToken cancellationToken = default)
      where TResponse : IMessage
      where TRequest : IMessage;
}
