using Server.Application.Abstractions;
using Server.Application.Dtos;
using Server.Application.Dtos.User;

namespace Server.Application.Services;

public interface IUserService
{
}

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
}
