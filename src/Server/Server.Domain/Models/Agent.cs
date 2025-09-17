using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public enum AgentStatus
{
  Ok,
  NotSynchronized
}

public class Agent : IEntity<Guid>
{
  public Guid Id { get; init; }
  public long ConfigurationId { get; set; }
  public string Name { get; set; } = string.Empty;
  public byte[] SecretHash { get; init; } = default!;
  public byte[] SecretSalt { get; init; } = default!;

  public AgentStatus Status { get; private set; } = AgentStatus.Ok;
  public DateTimeOffset? LastStatusChangeAt { get; private set; }

	public virtual Configuration Configuration { get; init; } = null!;
  public virtual AgentHardware Hardware { get; init; } = null!;

  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }
  /// <summary>
  /// Modifies the current agent with the values from another agent.
  /// </summary>
  /// <param name="agent"></param>
  /// <exception cref="ArgumentNullException"></exception>
  public void ModifyFrom(Agent agent)
	{
		if (agent is null)
			throw new ArgumentNullException(nameof(agent));

		Name = agent.Name;
		ConfigurationId = agent.ConfigurationId;
	}

  /// <summary>
  /// Update the agent status and set the LastStatusChangeAt if the status has changed.
  /// </summary>
  /// <param name="newStatus"></param>
  public void UpdateStatus(AgentStatus newStatus)
  {
    if (Status == newStatus)
      return;

    Status = newStatus;
    LastStatusChangeAt = DateTimeOffset.UtcNow;
  }
}
