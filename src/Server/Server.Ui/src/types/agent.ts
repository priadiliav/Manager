import { ConfigurationDto } from "./configuration";
import { HardwareDto } from "./hardware";

export interface AgentDto {
  id: string;
  name: string;
  configurationId: string;
}

export interface AgentCreateRequest {
  name: string;
  configurationId: string;
}

export interface AgentCreateResponse {
  secret: string;
}

export interface AgentModifyRequest {
  name: string;
  configurationId: string;
}

export interface AgentDetailedDto {
  id: string;
  name: string;
  configuration: ConfigurationDto;
  hardware: HardwareDto;
}
