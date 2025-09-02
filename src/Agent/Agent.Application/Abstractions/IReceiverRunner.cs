namespace Agent.Application.Abstractions;

public interface IReceiverRunner
{
  Task ReceiveAsync(CancellationToken cancellationToken);
}
