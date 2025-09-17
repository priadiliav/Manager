namespace Server.Application.Abstractions;

public interface INotifier<in TKey, in T> where TKey : notnull
{
  public Task NotifyAsync(TKey id, T message);
}
