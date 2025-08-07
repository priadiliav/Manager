using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public enum ProcessState
{
	Active,
	Banned
}

public class ProcessInConfiguration : ITrackable
{
	public long ConfigurationId { get; init; }
	public long ProcessId { get; init; }
	public ProcessState ProcessState { get; init; } = ProcessState.Active;

	public virtual Configuration Configuration { get; init; } = null!;
	public virtual Process Process { get; init; } = null!;

	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? ModifiedAt { get; set; }
}
