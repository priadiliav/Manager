using Common.Messages;

namespace Agent.Application.Abstractions;

public interface IPublisherClient
{
  /// <summary>
  /// Publish a message to the specified URL and optionally receive a response.
  /// </summary>
  /// <param name="url"></param>
  /// <param name="message"></param>
  /// <param name="cancellationToken"></param>
  /// <typeparam name="TResponse"></typeparam>
  /// <typeparam name="TRequest"></typeparam>
  /// <returns></returns>
  Task<TResponse?> PublishAsync<TResponse, TRequest>(
      string url, TRequest message, CancellationToken cancellationToken = default)
      where TResponse : IMessage
      where TRequest : IMessage;
}
