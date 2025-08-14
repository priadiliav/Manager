using System.Net.Http.Json;
using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Common.Messages;
using Microsoft.Extensions.Logging;

namespace Agent.Infrastructure.Communication;

public class PublisherClient<TMessage> : IPublisherClient<IMessage> where TMessage : IMessage
{
  private readonly ILogger<PublisherClient<TMessage>> _logger;
  private readonly HttpClient _httpClient;
  private readonly string _publisherUrl;
  private readonly AgentStateContext _context;
  public PublisherClient(
    AgentStateContext context,
    ILogger<PublisherClient<TMessage>> logger,
    HttpClient httpClient,
    string publisherUrl)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    _publisherUrl = publisherUrl ?? throw new ArgumentNullException(nameof(publisherUrl));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task StartPublishingAsync(IAsyncEnumerable<IMessage> messageStream, CancellationToken cancellationToken)
  {
    await foreach (var message in messageStream.WithCancellation(cancellationToken))
    {
      using var request = new HttpRequestMessage(HttpMethod.Post, _publisherUrl);

      request.Content = JsonContent.Create(message, typeof(TMessage));
      request.Headers.Authorization = new System.Net.Http.Headers
          .AuthenticationHeaderValue("Bearer", _context.AuthenticationToken);

      using var response = await _httpClient.SendAsync(
          request,
          HttpCompletionOption.ResponseHeadersRead,
          cancellationToken);

      response.EnsureSuccessStatusCode();

      _logger.LogInformation("Message of type {MessageType} published successfully to {PublisherUrl}",
          message.GetType().Name, _publisherUrl);
    }
  }
}
