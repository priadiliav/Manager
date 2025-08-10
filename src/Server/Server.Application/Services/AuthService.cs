using Common.Messages.Agent;
using Server.Application.Abstractions;
using Server.Application.Dtos;
using Server.Application.Dtos.User;

namespace Server.Application.Services;

public interface IAuthService
{
  /// <summary>
  /// Logs in a user using their username and password.
  /// </summary>
  /// <param name="request">The login request containing username and password.</param>
  /// <returns>A task that represents the asynchronous operation, containing the login response.</returns>
  Task<UserLoginResponse?> LoginUserAsync(UserLoginRequest request);

  /// <summary>
  /// Registers a new user with the provided registration details.
  /// </summary>
  /// <param name="request">The registration request containing user details.</param>
  /// <returns>A task that represents the asynchronous operation, containing the registration response.</returns>
  Task<UserRegisterResponse?> RegisterUserAsync(UserRegisterRequest request);

  /// <summary>
  /// Logs in an agent using their agent ID and secret.
  /// </summary>
  /// <param name="request">The login request containing agent ID and secret.</param>
  /// <returns>A task that represents the asynchronous operation, containing the agent login response.</returns>
  Task<AgentLoginResponseMessage?> LoginAgentAsync(AgentLoginRequestMessage request);
}

public class AuthService(
  IJwtTokenProvider jwtTokenProvider,
  IPasswordHasher passwordHasher,
  IUnitOfWork unitOfWork) : IAuthService
{
  public async Task<UserLoginResponse?> LoginUserAsync(UserLoginRequest request)
  {
    var user = await unitOfWork.Users.GetAsync(request.Username);
    if (user is null || !passwordHasher.IsPasswordValid(request.Password, user.PasswordHash, user.PasswordSalt))
      return null;

    var token = jwtTokenProvider.GenerateTokenForUser(user.Username, user.Role);
    return user.ToResponse(token);
  }

  public async Task<UserRegisterResponse?> RegisterUserAsync(UserRegisterRequest request)
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

  public async Task<AgentLoginResponseMessage?> LoginAgentAsync(AgentLoginRequestMessage request)
  {
    var agent = await unitOfWork.Agents.GetAsync(request.AgentId);
    if (agent is null || !passwordHasher.IsPasswordValid(request.Secret, agent.SecretHash, agent.SecretSalt))
      return null;

    var token = jwtTokenProvider.GenerateTokenForAgent(agent.Id);
    return agent.ToLoginResponse(token);
  }
}
