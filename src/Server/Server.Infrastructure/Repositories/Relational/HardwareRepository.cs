using Microsoft.EntityFrameworkCore;
using Server.Application.Abstractions;
using Server.Domain.Models;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Repositories.Relational;

public class HardwareRepository(AppDbContext dbContext) : IHardwareRepository
{
  public async Task<IEnumerable<Hardware>> GetAllAsync()
    => await dbContext.Hardware.ToListAsync();

  public Task<Hardware?> GetAsync(long id)
    => dbContext.Hardware.FindAsync(id).AsTask();

  public Task CreateAsync(Hardware entity)
  {
    dbContext.Hardware.Add(entity);
    return Task.CompletedTask;
  }

  public Task ModifyAsync(Hardware entity)
  {
    dbContext.Hardware.Update(entity);
    return Task.CompletedTask;
  }

  public Task DeleteAsync(long id)
  {
    var hardware = new Hardware { Id = id };
    dbContext.Hardware.Remove(hardware);
    return Task.CompletedTask;
  }
}
