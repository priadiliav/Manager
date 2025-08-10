using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IUserRepository : IRepository<User, long>
{
  Task<User?> GetAsync(string username);
}
