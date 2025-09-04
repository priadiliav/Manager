using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class Configuration : IEntity<long>
{
  public long Id { get; init; }
	public string Name { get; set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }

	public virtual ICollection<Agent> Agents { get; set; } = new List<Agent>();
	public virtual ICollection<PolicyInConfiguration> Policies { get; set; } = new List<PolicyInConfiguration>();
	public virtual ICollection<ProcessInConfiguration> Processes { get; set; } = new List<ProcessInConfiguration>();

  /// <summary>
  /// Modifies the current configuration with the values from another configuration.
  /// </summary>
  /// <param name="configuration"></param>
  /// <exception cref="ArgumentNullException"></exception>
	public void ModifyFrom(Configuration configuration)
	{
		if (configuration is null)
			throw new ArgumentNullException(nameof(configuration));

		Name = configuration.Name;
	}
}
