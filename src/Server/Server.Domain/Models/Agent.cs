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
	public virtual Configuration Configuration { get; set; } = null!;

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
		Configuration = agent.Configuration;
	}
}
