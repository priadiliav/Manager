using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public readonly struct PolicyInConfigurationKey(long configurationId, long policyId) : IEquatable<PolicyInConfigurationKey>
{
  private long ConfigurationId => configurationId;
  private long PolicyId => policyId;

  public override bool Equals(object? obj)
    => obj is PolicyInConfigurationKey other &&
       ConfigurationId == other.ConfigurationId &&
       PolicyId == other.PolicyId;

  public override int GetHashCode()
    => HashCode.Combine(ConfigurationId, PolicyId);

  public bool Equals(PolicyInConfigurationKey other)
    => ConfigurationId == other.ConfigurationId && PolicyId == other.PolicyId;
}

public sealed class PolicyInConfiguration : ICompositeEntity<long>
{
  public long Id => new PolicyInConfigurationKey(ConfigurationId, PolicyId).GetHashCode();

	public long ConfigurationId { get; init; }
	public long PolicyId { get; init; }
	public string RegistryValue { get; init; } = string.Empty;

	public Configuration Configuration { get; init; } = null!;
	public Policy Policy { get; init; } = null!;

  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }
}
