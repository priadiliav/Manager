namespace Server.Application.Abstractions;

public interface IJwtTokenProvider
{
  string GenerateTokenForUser(string username, string role);
  string GenerateTokenForAgent(Guid agentId);
}
