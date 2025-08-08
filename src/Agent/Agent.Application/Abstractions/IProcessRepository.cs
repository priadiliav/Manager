using Agent.Domain.Models;

namespace Agent.Application.Abstractions;

public interface IProcessRepository
{
  Task<IEnumerable<Process>> GetRangeByStatus(ProcessState processState);
  Task<Process?> GetByName(string name);
  Task AddRange(IEnumerable<Process> processes);
  Task Add(Process process);
}
