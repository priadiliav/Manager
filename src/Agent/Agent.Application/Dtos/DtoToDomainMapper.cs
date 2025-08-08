using Agent.Domain.Models;
using Common.Messages.Process;

namespace Agent.Application.Dtos;

public static class DtoToDomainMapper
{
  #region Process
  public static Domain.Models.Process ToDomain(this ProcessMessage message)
  {
    return new Domain.Models.Process
    {
      Name = message.Name,
      ProcessState = (ProcessState) message.State
    };
  }
  #endregion
}
