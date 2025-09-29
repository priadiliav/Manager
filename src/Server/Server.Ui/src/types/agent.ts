import { ConfigurationDto } from "./configuration";
import { HardwareDto } from "./hardware";

export enum AgentStatus {
  Ok,
  NotSynchronized,
}

export interface AgentDto {
  id: string;
  name: string;
  status: AgentStatus;
  isOnline: boolean;
  lastStatusChangeAt: string | null;
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
  isOnline: boolean;
  configurationId: string;
  status: AgentStatus;
  lastStatusChangeAt: string | null;
  configuration: ConfigurationDto;
  hardware: HardwareDto;
}
