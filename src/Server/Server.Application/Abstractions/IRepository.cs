using Server.Domain.Abstractions;

namespace Server.Application.Abstractions;

public interface IRepository<T, in TKey> where T : IEntity<TKey>
{
  Task<IEnumerable<T>> GetAllAsync();
  Task<T?> GetAsync(TKey id);
  Task CreateAsync(T entity);
  Task ModifyAsync(T entity);
  Task DeleteAsync(TKey id);
}
