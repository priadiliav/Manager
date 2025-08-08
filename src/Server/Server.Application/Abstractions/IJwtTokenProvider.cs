namespace Server.Application.Abstractions;

public interface IJwtTokenProvider
{
  string GenerateTokenForAgent(string username, string role);
  string GenerateTokenForAgent(Guid agentId);
}
