namespace Server.Application.Dtos.Agent;
//todo: add status enum 'Online', 'Offline'
public class AgentDto
{
	public Guid Id { get; init; }
	public string Name { get; init; } = string.Empty;

  public bool IsSynchronized { get; init; }
  public long ConfigurationId { get; init; }
}
