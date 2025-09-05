using Agent.Application.Abstractions;
using Common.Messages;

namespace Agent.Infrastructure.Communication;

public class ReceiverClient(ICommunicationClient communicationClient) : IReceiverClient
{
  public async Task ReceiveAsync<TResponse>(
    string url, Func<TResponse, Task> handler, CancellationToken cancellationToken)
      where TResponse : IMessage
  {
    var response = await communicationClient.GetAsync<TResponse>(
        url: url, authenticate: true, cancellationToken);

    if (response is not null)
      await handler(response);
  }
}

