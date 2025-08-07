using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class PolicyInConfiguration : ITrackable
{
	public long ConfigurationId { get; init; }
	public long PolicyId { get; init; }
	public string RegistryValue { get; init; } = string.Empty;
	public virtual Configuration Configuration { get; init; } = null!;
	public virtual Policy Policy { get; init; } = null!;

	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? ModifiedAt { get; set; }
}
