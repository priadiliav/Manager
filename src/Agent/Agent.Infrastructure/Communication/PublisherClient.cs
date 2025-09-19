using Agent.Application.Abstractions;
using Common.Messages;

namespace Agent.Infrastructure.Communication;

/// <summary>
/// Client for publishing messages to a specified URL.
/// </summary>
/// <param name="communicationClient"></param>
public class PublisherClient(ICommunicationClient communicationClient) : IPublisherClient
{
  public async Task<TResponse?> PublishAsync<TResponse, TRequest>(
    string url, TRequest message, CancellationToken cancellationToken = default)
      where TResponse : IMessage
      where TRequest : IMessage
  {
    return await communicationClient.PostAsync<TResponse, TRequest>(
        url: url, authenticate: true, message: message, cancellationToken);
  }
}
