namespace Server.Application.Abstractions.Providers;

public interface IJwtTokenProvider
{
  string GenerateTokenForUser(string username, string role);
  string GenerateTokenForAgent(Guid agentId);
}
