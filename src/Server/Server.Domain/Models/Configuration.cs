using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class Configuration : ITrackable
{
	public long Id { get; init; }
	public string Name { get; set; } = string.Empty;
	public virtual ICollection<Agent> Agents { get; set; } = new List<Agent>();
	public virtual ICollection<PolicyInConfiguration> Policies { get; set; } = new List<PolicyInConfiguration>();
	public virtual ICollection<ProcessInConfiguration> Processes { get; set; } = new List<ProcessInConfiguration>();

	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? ModifiedAt { get; set; }

	public void ModifyFrom(Configuration configuration)
	{
		if (configuration is null)
			throw new ArgumentNullException(nameof(configuration));

		Name = configuration.Name;
		Agents = configuration.Agents;
		Policies = configuration.Policies;
		Processes = configuration.Processes;
	}
}
