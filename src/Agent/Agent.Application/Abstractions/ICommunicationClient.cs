using Common.Messages;

namespace Agent.Application.Abstractions;

public interface ICommunicationClient
{
  /// <summary>
  /// Posts a message to the specified URL and expects a response.
  /// </summary>
  /// <param name="url"></param>
  /// <param name="authenticate"></param>
  /// <param name="message"></param>
  /// <param name="cancellationToken"></param>
  /// <typeparam name="TRequest"></typeparam>
  /// <typeparam name="TResponse"></typeparam>
  /// <returns></returns>
  Task<TResponse?> PostAsync<TResponse, TRequest>(
      string url, bool authenticate, TRequest message, CancellationToken cancellationToken = default)
      where TRequest : IMessage
      where TResponse : IMessage;

  /// <summary>
  /// Gets a message from the specified URL.
  /// </summary>
  /// <param name="url"></param>
  /// <param name="authenticate"></param>
  /// <param name="cancellationToken"></param>
  /// <typeparam name="TResponse"></typeparam>
  /// <returns></returns>
  Task<TResponse?> GetAsync<TResponse>(
      string url, bool authenticate, CancellationToken cancellationToken = default)
      where TResponse : IMessage;


  /// <summary>
  /// Sends a PUT request with a message to the specified URL.
  /// </summary>
  /// <param name="url"></param>
  /// <param name="authenticate"></param>
  /// <param name="message"></param>
  /// <param name="cancellationToken"></param>
  /// <typeparam name="TResponse"></typeparam>
  /// <typeparam name="TRequest"></typeparam>
  /// <returns></returns>
  Task<TResponse?> PutAsync<TResponse, TRequest>(
      string url, bool authenticate, TRequest message, CancellationToken cancellationToken = default)
      where TRequest : IMessage
      where TResponse : IMessage;
}
