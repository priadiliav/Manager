import { PolicyInConfigurationDto } from "./policy";
import { ProcessInConfigurationDto } from "./process";

export interface ConfigurationDto {
  id: string;
  name: string;
}

export interface ConfigurationDetailedDto {
  id: string;
  name: string;
  policies: PolicyInConfigurationDto[];
  processes: ProcessInConfigurationDto[];
}

export interface ConfigurationCreateRequest {
  name: string;
}

export interface ConfigurationModifyRequest {
  name: string;
  policies: PolicyInConfigurationDto[];
  processes: ProcessInConfigurationDto[];
}

