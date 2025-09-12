using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class Process : IEntity<long>
{
  public long Id { get; init; }
	public string Name { get; set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }
	public virtual ICollection<ProcessInConfiguration> Configurations { get; init; } = [];

  /// <summary>
  /// Modifies the current process with the values from another process.
  /// </summary>
  /// <param name="process"></param>
  /// <exception cref="ArgumentNullException"></exception>
	public void ModifyFrom(Process process)
	{
		if (process is null)
			throw new ArgumentNullException(nameof(process));

		Name = process.Name;
	}
}
