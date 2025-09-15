using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class Agent : IEntity<Guid>
{
  public Guid Id { get; init; }
  public long ConfigurationId { get; set; }
  public string Name { get; set; } = string.Empty;
  public byte[] SecretHash { get; init; } = default!;
  public byte[] SecretSalt { get; init; } = default!;
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }
  public bool IsSynchronized { get; set; }
  public DateTimeOffset? LastSynchronizedAt { get; set; }
  public DateTimeOffset? LastUnsynchronizedAt { get; set; }
	public virtual Configuration Configuration { get; init; } = null!;
  public virtual Hardware Hardware { get; init; } = null!;

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
  /// Mark the agent as synchronized and updates the modification timestamp.
  /// </summary>
  public void MarkAsSynchronized()
  {
    IsSynchronized = true;
    LastSynchronizedAt = DateTimeOffset.UtcNow;
  }

  /// <summary>
  /// Mark the agent as not synchronized.
  /// </summary>
  public void MarkAsUnsynchronized()
  {
    IsSynchronized = false;
    LastUnsynchronizedAt = DateTimeOffset.UtcNow;
  }
}
