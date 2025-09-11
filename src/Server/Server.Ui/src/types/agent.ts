import { ConfigurationDto } from "./configuration";
import { HardwareDto } from "./hardware";

export enum AgentState {
  Online,
  Offline,
  Unknown
}

export interface AgentDto {
  id: string;
  name: string;
  state: AgentState;
  requireSynchronization: boolean;
  configurationId: string;
}

export interface AgentCreateRequest {
  name: string;
  configurationId: string;
}

export interface AgentCreateResponse {
  id: string;
  secret: string;
}

export interface AgentModifyRequest {
  name: string;
  configurationId: string;
}

export interface AgentDetailedDto {
  id: string;
  name: string;
  state: AgentState;
  configurationId: string;
  requireSynchronization: boolean;
  configuration: ConfigurationDto;
  hardware: HardwareDto;
}
