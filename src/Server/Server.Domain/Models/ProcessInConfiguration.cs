using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public enum ProcessState
{
	Active,
	Banned
}

public readonly struct ProcessInConfigurationId(long configurationId, long processId) : IEquatable<ProcessInConfigurationId>
{
  private long ConfigurationId => configurationId;
  private long ProcessId => processId;

  public override bool Equals(object? obj)
    => obj is ProcessInConfigurationId other &&
       ConfigurationId == other.ConfigurationId &&
       ProcessId == other.ProcessId;

  public override int GetHashCode()
    => HashCode.Combine(ConfigurationId, ProcessId);

  public bool Equals(ProcessInConfigurationId other)
    => ConfigurationId == other.ConfigurationId && ProcessId == other.ProcessId;
}

public sealed class ProcessInConfiguration : ICompositeEntity<long>
{
  public long Id => new ProcessInConfigurationId(ConfigurationId, ProcessId).GetHashCode();

  public long ConfigurationId { get; init; }
	public long ProcessId { get; init; }
	public ProcessState ProcessState { get; init; } = ProcessState.Active;

  public Configuration Configuration { get; init; } = null!;
	public Process Process { get; init; } = null!;

  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }
}
