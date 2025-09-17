using Server.Application.Abstractions.Repositories;

namespace Server.Application.Services;

public interface IUserService
{
}

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
}
