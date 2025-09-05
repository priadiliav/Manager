using Common.Messages;

namespace Agent.Application.Abstractions;

public interface IReceiverClient
{
  Task ReceiveAsync<TResponse>(string url, Func<TResponse, Task> handler, CancellationToken cancellationToken)
      where TResponse : IMessage;
}
