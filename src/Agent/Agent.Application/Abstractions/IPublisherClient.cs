using Common.Messages;

namespace Agent.Application.Abstractions;

public interface IPublisherClient<in TMessage>
    where TMessage : IMessage
{
  Task StartPublishingAsync(IAsyncEnumerable<IMessage> messageStream, CancellationToken cancellationToken);
}
