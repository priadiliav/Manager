namespace Server.Application.Abstractions;

public interface IJwtTokenProvider
{
  string GenerateToken(string username, string role);
}
