using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class PolicyInConfiguration : ITrackable
{
	public long ConfigurationId { get; set; }
	public long PolicyId { get; set; }
	public string RegistryValue { get; set; } = string.Empty;
	public virtual Configuration Configuration { get; set; } = null!;
	public virtual Policy Policy { get; set; } = null!;
	
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? ModifiedAt { get; set; }
}