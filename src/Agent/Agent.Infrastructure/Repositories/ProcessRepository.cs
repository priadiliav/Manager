using Agent.Application.Abstractions;
using Agent.Domain.Models;

namespace Agent.Infrastructure.Repositories;

public class ProcessRepository : IProcessRepository
{
  public Task<IEnumerable<Process>> GetRangeByStatus(ProcessState processState)
  {
    throw new NotImplementedException();
  }

  public Task<Process?> GetByName(string name)
  {
    throw new NotImplementedException();
  }

  public Task AddRange(IEnumerable<Process> processes)
  {
    throw new NotImplementedException();
  }

  public Task Add(Process process)
  {
    throw new NotImplementedException();
  }
}
