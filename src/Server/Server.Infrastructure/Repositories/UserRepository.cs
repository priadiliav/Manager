using Microsoft.EntityFrameworkCore;
using Server.Application.Abstractions;
using Server.Domain.Models;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
  public async Task<User?> GetAsync(long id)
    => await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

  public async Task<User?> GetAsync(string username)
    => await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

  public Task CreateAsync(User user)
  {
    dbContext.Users.Add(user);
    return Task.CompletedTask;
  }
}
