using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public enum ProcessState
{
	Active,
	Banned
}

public class ProcessInConfiguration : ITrackable
{
	public long ConfigurationId { get; set; }
	public long ProcessId { get; set; }
	public ProcessState ProcessState { get; set; } = ProcessState.Active;
	
	public virtual Configuration Configuration { get; set; } = null!;
	public virtual Process Process { get; set; } = null!;
	
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? ModifiedAt { get; set; }
}