namespace Server.Domain.Abstractions;

public interface ICompositeEntity<out TKey> : ITrackable
{
  TKey Id { get; }
}
