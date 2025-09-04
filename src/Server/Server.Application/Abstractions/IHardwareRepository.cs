using Server.Domain.Models;

namespace Server.Application.Abstractions;

public interface IHardwareRepository : IRepository<Domain.Models.Hardware, long>
{
}
