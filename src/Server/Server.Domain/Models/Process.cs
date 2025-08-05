using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class Process : ITrackable
{
	public long Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public virtual ICollection<ProcessInConfiguration> Configurations { get; set; } = [];
	
	public void ModifyFrom(Process process)
	{
		if (process is null) 
			throw new ArgumentNullException(nameof(process));
		
		Name = process.Name;
	}

	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? ModifiedAt { get; set; }
}