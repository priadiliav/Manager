using System.Net.Http.Json;
using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Common.Messages;

namespace Agent.Infrastructure.Communication;

/// <summary>
/// HTTP communication client for sending and receiving messages.
/// </summary>
/// <param name="context"></param>
/// <param name="httpClient"></param>
/// todo: simplify
public class CommunicationClient(
  AgentStateContext context,
  HttpClient httpClient) : ICommunicationClient
{
  public async Task<TResponse?> PostAsync<TResponse, TRequest>(
    string url, bool authenticate, TRequest message, CancellationToken cancellationToken)
      where TRequest : IMessage
      where TResponse : IMessage
  {
    using var request = new HttpRequestMessage(HttpMethod.Post, url);
    request.Content = JsonContent.Create(message, typeof(TRequest));

    if (authenticate)
    {
      request.Headers.Authorization = new System.Net.Http.Headers
          .AuthenticationHeaderValue("Bearer", context.AuthenticationToken);
    }

    using var response = await httpClient.SendAsync(
        request,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    return content;
  }

  public async Task<TResponse?> GetAsync<TResponse>(
    string url, bool authenticate, CancellationToken cancellationToken)
      where TResponse : IMessage
  {
    using var request = new HttpRequestMessage(HttpMethod.Get, url);
    if (authenticate)
    {
      request.Headers.Authorization = new System.Net.Http.Headers
          .AuthenticationHeaderValue("Bearer", context.AuthenticationToken);
    }

    using var response = await httpClient.SendAsync(
        request,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    return content;
  }

  public async Task<TResponse?> PutAsync<TResponse, TRequest>(
    string url, bool authenticate, TRequest message, CancellationToken cancellationToken)
      where TResponse : IMessage
      where TRequest : IMessage
  {
    using var request = new HttpRequestMessage(HttpMethod.Put, url);

    request.Content = JsonContent.Create(message, typeof(TRequest));
    if (authenticate)
    {
      request.Headers.Authorization = new System.Net.Http.Headers
          .AuthenticationHeaderValue("Bearer", context.AuthenticationToken);
    }

    using var response = await httpClient.SendAsync(
        request,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    return content;
  }
}
