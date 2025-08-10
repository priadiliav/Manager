namespace Server.Domain.Abstractions;

public interface IEntity<TKey> : ITrackable
{
  TKey Id { get; init; }
}
