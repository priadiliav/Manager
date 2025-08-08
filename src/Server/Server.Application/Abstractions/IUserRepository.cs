using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IUserRepository
{
  Task<User?> GetAsync(long id);
  Task<User?> GetAsync(string username);
  Task CreateAsync(User user);
}
