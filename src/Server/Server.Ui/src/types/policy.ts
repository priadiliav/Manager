export enum RegistryValueType {
  String,
  Binary,
  Dword,
  Qword
}

export enum RegistryKeyType {
  Hklm,
  Hkcu,
  Hkcr,
  Hkus
}

export interface PolicyDto {
  id: string;
  name: string;
  description: string;
  registryPath: string;
  registryValueType: RegistryValueType;
  registryKeyType: RegistryKeyType;
  registryKey: string;
}

export interface PolicyInConfigurationDto {
  policyId: string;
  registryValue: string;
}

export interface PolicyCreateRequest {
  name: string;
  description: string;
  registryPath: string;
  registryValueType: RegistryValueType;
  registryKeyType: RegistryKeyType;
  registryKey: string;
}

export interface PolicyModifyRequest {
  name: string;
  description: string;
  registryPath: string;
  registryValueType: RegistryValueType;
  registryKeyType: RegistryKeyType;
  registryKey: string;
}
