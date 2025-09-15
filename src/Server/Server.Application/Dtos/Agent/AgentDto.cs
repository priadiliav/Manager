using Server.Domain.Models;

namespace Server.Application.Dtos.Agent;

public class AgentDto
{
	public Guid Id { get; init; }
	public string Name { get; init; } = string.Empty;
  public AgentStatus Status { get; init; }
  public DateTimeOffset? LastStatusChangeAt { get; init; }
  public long ConfigurationId { get; init; }
}
