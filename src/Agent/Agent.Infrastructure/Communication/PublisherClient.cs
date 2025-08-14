using System.Net.Http.Json;
using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Common.Messages;
using Microsoft.Extensions.Logging;

namespace Agent.Infrastructure.Communication;

public class PublisherClient<TMessage>(
  ILogger<PublisherClient<TMessage>> logger,
  HttpClient httpClient,
  AgentStateContext context,
  string publisherUrl) : IPublisherClient<IMessage> where TMessage : IMessage
{
  public async Task PublishAsync(IMessage message, CancellationToken cancellationToken = default)
  {
    using var request = new HttpRequestMessage(HttpMethod.Post, publisherUrl);

    request.Content = JsonContent.Create(message, typeof(TMessage));
    request.Headers.Authorization = new System.Net.Http.Headers
        .AuthenticationHeaderValue("Bearer", context.AuthenticationToken);

    using var response = await httpClient.SendAsync(
        request,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    logger.LogInformation("Message of type {MessageType} published successfully to {PublisherUrl}",
        message.GetType().Name, publisherUrl);
  }
}
