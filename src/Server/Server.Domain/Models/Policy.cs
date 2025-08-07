using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public enum RegistryKeyType
{
	Hklm, // HKEY_LOCAL_MACHINE
	Hkcu, // HKEY_CURRENT_USER
	Hkcr, // HKEY_CLASSES_ROOT
	Hkus, // HKEY_USERS
}

public enum RegistryValueType
{
	String, // REG_SZ
	Binary, // REG_BINARY
	Dword, // REG_DWORD
	Qword, // REG_QWORD
}

public class Policy : ITrackable
{
	public long Id { get; init; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string RegistryPath { get; set; } = string.Empty;
	public RegistryValueType RegistryValueType { get; set; }
	public RegistryKeyType RegistryKeyType { get; set; }
	public string RegistryKey { get; set; } = string.Empty;
	public virtual ICollection<PolicyInConfiguration> Configurations { get; init; } = [];
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }

	public void ModifyFrom(Policy policy)
	{
		if (policy is null)
			throw new ArgumentNullException(nameof(policy));

		Name = policy.Name;
		Description = policy.Description;
		RegistryPath = policy.RegistryPath;
		RegistryValueType = policy.RegistryValueType;
		RegistryKeyType = policy.RegistryKeyType;
		RegistryKey = policy.RegistryKey;
	}
}


