using Server.Application.Abstractions;
using Server.Application.Dtos;
using Server.Application.Dtos.User;

namespace Server.Application.Services;

public interface IUserService
{
  Task<UserLoginResponse?> LoginAsync(UserLoginRequest request);
  Task<UserRegisterResponse?> RegisterAsync(UserRegisterRequest request);

}

public class UserService(
  IPasswordHasher passwordHasher,
  IJwtTokenProvider jwtTokenProvider,
  IUnitOfWork unitOfWork) : IUserService
{
  #region Authentication
  public async Task<UserLoginResponse?> LoginAsync(UserLoginRequest request)
  {
    var user = await unitOfWork.Users.GetAsync(request.Username);
    if (user is null || !passwordHasher.IsPasswordValid(request.Password, user.PasswordHash, user.PasswordSalt))
      return null;

    var token = jwtTokenProvider.GenerateTokenForAgent(user.Username, user.Role);
    return user.ToResponse(token);
  }

  public async Task<UserRegisterResponse?> RegisterAsync(UserRegisterRequest request)
  {
    var existingUser = await unitOfWork.Users.GetAsync(request.Username);
    if (existingUser != null)
      return null;

    var (passwordHash, passwordSalt) = passwordHasher.CreatePasswordHash(request.Password);

    var newUser = request.ToDomain(passwordHash, passwordSalt);

    await unitOfWork.Users.CreateAsync(newUser);
    await unitOfWork.SaveChangesAsync();

    return new UserRegisterResponse();
  }
  #endregion
}
