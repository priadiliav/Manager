using System.Net.Http.Json;
using Agent.Application.Abstractions;
using Common.Messages;

namespace Agent.Infrastructure.Communication;

/// <summary>
/// HTTP communication client for sending and receiving messages.
/// </summary>
public class CommunicationClient(
    IAgentRepository context,
    HttpClient httpClient) : ICommunicationClient
{
    public Task<TResponse?> PostAsync<TResponse, TRequest>(
        string url, bool authenticate, TRequest message, CancellationToken cancellationToken = default)
        where TRequest : IMessage
        where TResponse : IMessage
        => SendAsync<TResponse>(HttpMethod.Post, url, authenticate, message, cancellationToken);

    public Task<TResponse?> PutAsync<TResponse, TRequest>(
        string url, bool authenticate, TRequest message, CancellationToken cancellationToken = default)
        where TRequest : IMessage
        where TResponse : IMessage
        => SendAsync<TResponse>(HttpMethod.Put, url, authenticate, message, cancellationToken);

    public Task<TResponse?> GetAsync<TResponse>(
        string url, bool authenticate, CancellationToken cancellationToken = default)
        where TResponse : IMessage
        => SendAsync<TResponse>(HttpMethod.Get, url, authenticate, null, cancellationToken);

    #region Prive Methods
    /// <summary>
    /// Sends an HTTP request and returns the response deserialized as TResponse.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="url"></param>
    /// <param name="authenticate"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    private async Task<TResponse?> SendAsync<TResponse>(
        HttpMethod method, string url, bool authenticate, object? message, CancellationToken cancellationToken)
        where TResponse : IMessage
    {
      using var request = new HttpRequestMessage(method, url);

      if (message is not null)
        request.Content = JsonContent.Create(message);

      if (authenticate)
      {
        var currentAgent = await context.GetAsync();
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", currentAgent.Token);
      }
      using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
      response.EnsureSuccessStatusCode();

      return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    }
    #endregion
}
