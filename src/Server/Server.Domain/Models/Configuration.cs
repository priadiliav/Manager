using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class Configuration : IEntity<long>
{
  public long Id { get; init; }
	public string Name { get; set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }

	public virtual ICollection<Agent> Agents { get; init; } = new List<Agent>();
	public virtual ICollection<PolicyInConfiguration> Policies { get; init; } = new List<PolicyInConfiguration>();
	public virtual ICollection<ProcessInConfiguration> Processes { get; init; } = new List<ProcessInConfiguration>();

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

    Policies.Clear();
    foreach (var policy in configuration.Policies)
    {
      Policies.Add(policy);
    }

    Processes.Clear();
    foreach (var process in configuration.Processes)
    {
      Processes.Add(process);
    }
	}
}
