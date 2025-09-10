export enum ProcessState {
  Active,
  Banned
}

export interface ProcessDto {
  id: string;
  name: string;
}

export interface ProcessInConfigurationDto {
  processId: string;
  processState: ProcessState;
}

export interface ProcessCreateRequest {
  name: string;
}

export interface ProcessModifyRequest {
  name: string;
}
