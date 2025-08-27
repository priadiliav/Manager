using Server.Application.Abstractions;

namespace Server.Application.Services;

public interface IUserService
{
}

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
}
