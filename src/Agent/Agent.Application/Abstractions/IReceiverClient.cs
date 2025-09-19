using Common.Messages;

namespace Agent.Application.Abstractions;

public interface IReceiverClient
{
  /// <summary>
  /// Receive messages from the specified URL and handle them with the provided handler function.
  /// </summary>
  /// <param name="url"></param>
  /// <param name="handler"></param>
  /// <param name="cancellationToken"></param>
  /// <typeparam name="TResponse"></typeparam>
  /// <returns></returns>
  Task ReceiveAsync<TResponse>(
      string url, Func<TResponse, Task> handler, CancellationToken cancellationToken)
      where TResponse : IMessage;
}
