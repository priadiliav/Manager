using System.Net.Http.Headers;
using System.Net.Http.Json;
using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Common.Messages;
using Microsoft.Extensions.Logging;

namespace Agent.Infrastructure.Communication;

public class LongPollingClient<TResponse>(
  ILogger<LongPollingClient<TResponse>> logger,
  HttpClient httpClient,
  AgentStateContext context,
  string pollingUrl) : ILongPollingClient<TResponse> where TResponse : IMessage
{
  public async Task ListenAsync(Func<TResponse, Task> handleMessage, CancellationToken cancellationToken)
  {
    using var request = new HttpRequestMessage(HttpMethod.Get, pollingUrl);

    request.Headers.Authorization =
        new AuthenticationHeaderValue("Bearer", context.AuthenticationToken);

    using var response = await httpClient.SendAsync(
        request,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    if (content is not null)
      await handleMessage(content);

    logger.LogInformation("Received message of type {MessageType} from {PollingUrl}",
        typeof(TResponse).Name, pollingUrl);
  }
}

