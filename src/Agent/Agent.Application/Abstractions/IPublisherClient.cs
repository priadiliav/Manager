using Common.Messages;

namespace Agent.Application.Abstractions;

public interface IPublisherClient<in TMessage> where TMessage : IMessage
{
  Task PublishAsync(TMessage message, CancellationToken cancellationToken = default);
}
