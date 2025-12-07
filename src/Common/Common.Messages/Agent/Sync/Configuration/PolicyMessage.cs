namespace Common.Messages.Agent.Sync.Configuration;

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

public class PolicyMessage : IMessage
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string RegistryPath { get; set; } = string.Empty;
  public RegistryValueType RegistryValueType { get; set; }
  public RegistryKeyType RegistryKeyType { get; set; }
  public string RegistryKey { get; set; } = string.Empty;
  public object? RegistryValue { get; set; }
}
